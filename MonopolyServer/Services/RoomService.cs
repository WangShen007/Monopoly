using Microsoft.EntityFrameworkCore;
using MonopolyModels.Data;
using MonopolyModels.Dtos;
using MonopolyModels.Entities;
using MonopolyModels.States;

namespace MonopolyServer.Services;

public class RoomService
{
    private readonly string _databasePath;
    private readonly RecordService _recordService;
    private readonly Dictionary<int, GameRoom> _rooms = [];
    private readonly object _gate = new();
    private readonly Random _random = new();
    private int _nextRoomId = 1;

    public RoomService(string databasePath, RecordService recordService)
    {
        _databasePath = databasePath;
        _recordService = recordService;
    }

    public GameRoom? GetRoom(int roomId)
    {
        lock (_gate)
        {
            return _rooms.GetValueOrDefault(roomId);
        }
    }

    public List<RoomSummaryDto> GetRoomSummaries()
    {
        lock (_gate)
        {
            return _rooms.Values
                .OrderByDescending(x => x.CreatedAt)
                .Select(ToSummary)
                .ToList();
        }
    }

    public GameRoom SelectToken(ClientUser user, string tokenImageFile)
    {
        if (!user.IsLoggedIn)
        {
            throw new InvalidOperationException("请先登录");
        }

        var normalized = NormalizeTokenImageFile(tokenImageFile);
        var room = RequireRoom(user);
        lock (room.Gate)
        {
            if (room.Status != "Waiting")
            {
                throw new InvalidOperationException("游戏已经开始，不能更换棋子");
            }

            if (room.PlayerStates.Any(x => x.UserId != user.UserId && string.Equals(x.TokenImageFile, normalized, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("这个棋子已经被别人选了");
            }

            var state = room.PlayerStates.First(x => x.UserId == user.UserId);
            state.TokenImageFile = normalized;
            user.TokenImageFile = normalized;
            AddLog(room, $"{user.UserName} 选择了 {normalized}");
            return room;
        }
    }

    public GameRoom CreateRoom(ClientUser owner, string roomName, int maxPlayers)
    {
        if (!owner.IsLoggedIn)
        {
            throw new InvalidOperationException("请先登录");
        }

        if (owner.RoomId.HasValue)
        {
            throw new InvalidOperationException("请先离开当前房间");
        }

        lock (_gate)
        {
            var room = new GameRoom
            {
                RoomId = _nextRoomId++,
                RoomName = string.IsNullOrWhiteSpace(roomName) ? $"{owner.UserName}的房间" : roomName.Trim(),
                OwnerUserId = owner.UserId,
                MaxPlayers = Math.Clamp(maxPlayers, 2, 4)
            };
            _rooms.Add(room.RoomId, room);
            AddPlayerLocked(room, owner);
            return room;
        }
    }

    public GameRoom JoinRoom(ClientUser user, int roomId)
    {
        if (!user.IsLoggedIn)
        {
            throw new InvalidOperationException("请先登录");
        }

        lock (_gate)
        {
            var room = _rooms.GetValueOrDefault(roomId) ?? throw new InvalidOperationException("房间不存在");
            lock (room.Gate)
            {
                if (user.RoomId.HasValue && user.RoomId.Value != roomId)
                {
                    throw new InvalidOperationException("请先离开当前房间");
                }

                if (room.Status != "Waiting")
                {
                    throw new InvalidOperationException("房间已经开始游戏");
                }

                if (room.Players.Any(x => x.UserId == user.UserId))
                {
                    return room;
                }

                if (room.Players.Count >= room.MaxPlayers)
                {
                    throw new InvalidOperationException("房间人数已满");
                }

                AddPlayerLocked(room, user);
                return room;
            }
        }
    }

    public GameRoom? LeaveRoom(ClientUser user)
    {
        if (!user.RoomId.HasValue)
        {
            return null;
        }

        lock (_gate)
        {
            var room = _rooms.GetValueOrDefault(user.RoomId.Value);
            if (room is null)
            {
                user.RoomId = null;
                return null;
            }

            lock (room.Gate)
            {
                room.Players.RemoveAll(x => x.UserId == user.UserId);
                var state = room.PlayerStates.FirstOrDefault(x => x.UserId == user.UserId);
                if (state is not null && room.Status == "Playing")
                {
                    state.IsBankrupt = true;
                    state.Money = 0;
                    AddLog(room, $"{user.UserName} 离线，按破产处理");
                    CheckEndOrAdvanceLocked(room);
                }
                else if (state is not null)
                {
                    room.PlayerStates.Remove(state);
                }

                user.RoomId = null;
                if (room.Players.Count == 0 || room.Status == "Finished")
                {
                    _rooms.Remove(room.RoomId);
                }

                return room;
            }
        }
    }

    public GameRoom SetReady(ClientUser user)
    {
        var room = RequireRoom(user);
        lock (room.Gate)
        {
            ClearNotificationsLocked(room);
            var state = room.PlayerStates.First(x => x.UserId == user.UserId);
            state.IsReady = true;
            AddLog(room, $"{user.UserName} 已准备");

            if (room.Status == "Waiting" && room.PlayerStates.Count >= 2 && room.PlayerStates.All(x => x.IsReady))
            {
                StartGameLocked(room);
            }
        }

        return room;
    }

    public GameRoom RollDice(ClientUser user)
    {
        var room = RequirePlayingRoom(user);
        lock (room.Gate)
        {
            ClearNotificationsLocked(room);
            var current = GetCurrentPlayerLocked(room);
            if (current.UserId != user.UserId)
            {
                throw new InvalidOperationException("还没有轮到你");
            }

            if (room.CurrentPlayerRolled)
            {
                throw new InvalidOperationException("本回合已经掷过骰子");
            }

            room.CurrentPlayerRolled = true;
            var oldPosition = current.Position;
            var dice = _random.Next(1, 7);
            var (newPosition, passedStart) = GameRules.Move(current, dice, room.MapCells.Count);

            AddAction(room, current.UserId, "RollDice", $"{current.UserName} 掷出 {dice} 点，从 {oldPosition} 到 {newPosition}");
            AddLog(room, $"{current.UserName} 掷出 {dice} 点，从 {oldPosition} 到 {newPosition}");
            AddNotification(room, "DiceResult", new DiceResultDto(current.UserId, current.UserName, dice, oldPosition, newPosition));
            AddNotification(room, "MoveResult", new MoveResultDto(current.UserId, current.UserName, oldPosition, newPosition, passedStart, current.Money));
            if (passedStart)
            {
                AddLog(room, $"{current.UserName} 经过起点，获得 {GameRules.StartReward} 金币");
            }

            ResolveCurrentCellLocked(room, current, passedStart);
            CheckBankruptLocked(room, current);
            CheckEndOrAdvanceLocked(room);
        }

        return room;
    }

    public GameRoom BuyProperty(ClientUser user)
    {
        var room = RequirePlayingRoom(user);
        lock (room.Gate)
        {
            ClearNotificationsLocked(room);
            var current = GetCurrentPlayerLocked(room);
            if (current.UserId != user.UserId)
            {
                throw new InvalidOperationException("还没有轮到你");
            }

            var cell = room.MapCells.First(x => x.CellIndex == current.Position);
            if (cell.CellType != "Property")
            {
                throw new InvalidOperationException("当前位置不是地产");
            }

            var property = room.PropertyStates.FirstOrDefault(x => x.MapCellId == cell.Id)
                ?? throw new InvalidOperationException("该格没有地产配置");
            if (property.OwnerUserId.HasValue)
            {
                throw new InvalidOperationException("地产已有主人");
            }

            if (current.Money < property.Price)
            {
                throw new InvalidOperationException("金币不足，无法购买");
            }

            current.Money -= property.Price;
            property.OwnerUserId = current.UserId;
            property.OwnerUserName = current.UserName;
            AddAction(room, current.UserId, "BuyProperty", $"{current.UserName} 购买 {property.PropertyName}，花费 {property.Price}");
            AddLog(room, $"{current.UserName} 购买 {property.PropertyName}");
            AddNotification(room, "BuyPropertyResult", new BuyPropertyResultDto(true, $"{current.UserName} 购买 {property.PropertyName}", current.UserId, property.PropertyId));
            CheckBankruptLocked(room, current);
            CheckEndOrAdvanceLocked(room);
        }

        return room;
    }

    public GameRoom EndTurn(ClientUser user)
    {
        var room = RequirePlayingRoom(user);
        lock (room.Gate)
        {
            ClearNotificationsLocked(room);
            var current = GetCurrentPlayerLocked(room);
            if (current.UserId != user.UserId)
            {
                throw new InvalidOperationException("还没有轮到你");
            }

            AdvanceTurnLocked(room);
        }

        return room;
    }

    public List<NetMessage> TakeNotifications(GameRoom room)
    {
        lock (room.Gate)
        {
            var messages = room.PendingNotifications.ToList();
            room.PendingNotifications.Clear();
            return messages;
        }
    }

    public GameStateDto ToGameState(GameRoom room)
    {
        lock (room.Gate)
        {
            var current = room.Status == "Playing" ? GetCurrentPlayerLocked(room) : null;
            var canBuy = false;
            if (current is not null)
            {
                var cell = room.MapCells.FirstOrDefault(x => x.CellIndex == current.Position);
                if (cell?.CellType == "Property")
                {
                    var property = room.PropertyStates.FirstOrDefault(x => x.MapCellId == cell.Id);
                    canBuy = property is { OwnerUserId: null } && current.Money >= property.Price;
                }
            }

            return new GameStateDto(
                room.RoomId,
                room.Status,
                room.RoundNumber,
                current?.UserId,
                current?.UserName ?? string.Empty,
                room.MapCells.OrderBy(x => x.CellIndex).Select(x => new MapCellDto(x.Id, x.CellIndex, x.CellName, x.CellType, x.Description)).ToList(),
                room.PropertyStates.OrderBy(x => x.MapCellId).Select(x => new PropertyStateDto(x.PropertyId, x.MapCellId, x.PropertyName, x.Price, x.Rent, x.ColorGroup, x.OwnerUserId, x.OwnerUserName)).ToList(),
                room.PlayerStates.Select(x => new PlayerStateDto(
                    x.UserId,
                    x.UserName,
                    x.Position,
                    x.Money,
                    x.IsBankrupt,
                    x.IsReady,
                    room.PropertyStates.Count(p => p.OwnerUserId == x.UserId),
                    x.FreeRentCards,
                    x.SkipTurnRounds,
                    x.TokenImageFile)).ToList(),
                room.Logs.TakeLast(80).ToList(),
                canBuy);
        }
    }

    private void StartGameLocked(GameRoom room)
    {
        using var db = new MonopolyDbContext(_databasePath);
        room.MapCells = db.MapCells.AsNoTracking().OrderBy(x => x.CellIndex).ToList();
        room.EventCards = db.EventCards.AsNoTracking().Where(x => x.IsEnabled).ToList();
        room.PropertyStates = db.Properties
            .AsNoTracking()
            .OrderBy(x => x.MapCellId)
            .Select(x => new PropertyState
            {
                PropertyId = x.Id,
                MapCellId = x.MapCellId,
                PropertyName = x.PropertyName,
                Price = x.Price,
                Rent = x.Rent,
                ColorGroup = x.ColorGroup
            })
            .ToList();

        foreach (var player in room.PlayerStates)
        {
            player.Money = GameRules.InitialMoney;
            player.Position = 0;
            player.IsBankrupt = false;
            player.FreeRentCards = 0;
            player.SkipTurnRounds = 0;
        }

        room.Status = "Playing";
        room.StartedAt = DateTime.Now;
        room.CurrentPlayerIndex = 0;
        room.RoundNumber = 1;
        room.CurrentPlayerRolled = false;
        AddLog(room, "游戏开始");
        AddLog(room, $"轮到 {GetCurrentPlayerLocked(room).UserName}");
        AddNotification(room, "GameStart", new BasicResult(true, "游戏开始"));
    }

    private void ResolveCurrentCellLocked(GameRoom room, PlayerState player, bool passedStart)
    {
        var cell = room.MapCells.First(x => x.CellIndex == player.Position);
        switch (cell.CellType)
        {
            case "Start":
                if (!passedStart)
                {
                    player.Money += GameRules.StartReward;
                    AddAction(room, player.UserId, "Start", $"{player.UserName} 停留起点，获得 {GameRules.StartReward}");
                    AddLog(room, $"{player.UserName} 停留起点，获得 {GameRules.StartReward} 金币");
                }
                break;
            case "Tax":
                player.Money -= GameRules.TaxAmount;
                AddAction(room, player.UserId, "Tax", $"{player.UserName} 缴税 {GameRules.TaxAmount}");
                AddLog(room, $"{player.UserName} 缴税 {GameRules.TaxAmount} 金币");
                AddNotification(room, "TaxResult", new TaxResultDto(player.UserId, GameRules.TaxAmount));
                break;
            case "Chance":
                ApplyChanceLocked(room, player);
                break;
            case "Property":
                ResolvePropertyLocked(room, player, cell.Id);
                break;
            case "Empty":
                ApplyRestCellLocked(room, player, cell);
                break;
            default:
                AddLog(room, $"{player.UserName} 来到 {cell.CellName}，无事发生");
                break;
        }
    }

    private void ApplyRestCellLocked(GameRoom room, PlayerState player, MapCell cell)
    {
        player.SkipTurnRounds = Math.Max(player.SkipTurnRounds, 1);
        AddAction(room, player.UserId, "Rest", $"{player.UserName} 来到 {cell.CellName}，下回合休息一轮");
        AddLog(room, $"{player.UserName} 来到 {cell.CellName}，下回合休息一轮");
    }

    private void ResolvePropertyLocked(GameRoom room, PlayerState player, int mapCellId)
    {
        var property = room.PropertyStates.FirstOrDefault(x => x.MapCellId == mapCellId);
        if (property is null)
        {
            AddLog(room, "该格没有地产配置");
            return;
        }

        if (!property.OwnerUserId.HasValue)
        {
            AddLog(room, $"{player.UserName} 来到无主地产 {property.PropertyName}，可购买");
            return;
        }

        if (property.OwnerUserId == player.UserId)
        {
            AddLog(room, $"{player.UserName} 来到自己的地产 {property.PropertyName}");
            return;
        }

        var owner = room.PlayerStates.FirstOrDefault(x => x.UserId == property.OwnerUserId.Value);
        if (owner is null || owner.IsBankrupt)
        {
            return;
        }

        if (player.FreeRentCards > 0)
        {
            player.FreeRentCards -= 1;
            AddAction(room, player.UserId, "PayRent", $"{player.UserName} 使用免租卡，免付 {property.PropertyName} 租金");
            AddLog(room, $"{player.UserName} 使用免租卡，免付租金");
            AddNotification(room, "RentPaid", new RentPaidDto(player.UserId, owner.UserId, property.Rent, true));
            return;
        }

        player.Money -= property.Rent;
        owner.Money += property.Rent;
        AddAction(room, player.UserId, "PayRent", $"{player.UserName} 向 {owner.UserName} 支付 {property.Rent} 租金");
        AddLog(room, $"{player.UserName} 向 {owner.UserName} 支付 {property.Rent} 租金");
        AddNotification(room, "RentPaid", new RentPaidDto(player.UserId, owner.UserId, property.Rent, false));
    }

    private void ApplyChanceLocked(GameRoom room, PlayerState player)
    {
        if (room.EventCards.Count == 0)
        {
            AddLog(room, "没有启用的机会卡");
            return;
        }

        var card = room.EventCards[_random.Next(room.EventCards.Count)];
        var oldPosition = player.Position;
        var passedStart = false;
        var isBackward = false;
        GameRules.ApplyEvent(
            player,
            card,
            steps =>
            {
                isBackward = steps < 0;
                var result = GameRules.Move(player, steps, room.MapCells.Count);
                passedStart = result.PassedStart;
            },
            () =>
            {
                isBackward = ShouldAnimateBackwardToStart(player.Position, room.MapCells.Count);
                player.Position = 0;
            });
        AddAction(room, player.UserId, "Chance", $"{player.UserName} 触发机会卡：{card.Description}");
        AddLog(room, $"{player.UserName} 触发机会卡：{card.Description}");
        AddNotification(room, "ChanceResult", new ChanceResultDto(player.UserId, card.EventName, card.EventType, card.Value, card.Description));
        if (oldPosition != player.Position)
        {
            AddNotification(room, "MoveResult", new MoveResultDto(
                player.UserId,
                player.UserName,
                oldPosition,
                player.Position,
                passedStart,
                player.Money,
                isBackward));
            ResolveCurrentCellLocked(room, player, passedStart || string.Equals(card.EventType, "GoToStart", StringComparison.OrdinalIgnoreCase));
        }
    }

    private static bool ShouldAnimateBackwardToStart(int position, int mapCellCount)
    {
        if (mapCellCount <= 0)
        {
            return false;
        }

        var normalized = ((position % mapCellCount) + mapCellCount) % mapCellCount;
        var forwardDistance = (mapCellCount - normalized) % mapCellCount;
        var backwardDistance = normalized;
        return backwardDistance <= forwardDistance;
    }

    private void CheckBankruptLocked(GameRoom room, PlayerState player)
    {
        if (player.Money > 0 || player.IsBankrupt)
        {
            return;
        }

        player.IsBankrupt = true;
        foreach (var property in room.PropertyStates.Where(x => x.OwnerUserId == player.UserId))
        {
            property.OwnerUserId = null;
            property.OwnerUserName = null;
        }

        AddAction(room, player.UserId, "Bankrupt", $"{player.UserName} 破产");
        AddLog(room, $"{player.UserName} 破产，名下地产回收");
        AddNotification(room, "PlayerBankrupt", new PlayerBankruptDto(player.UserId, player.UserName));
    }

    private void CheckEndOrAdvanceLocked(GameRoom room)
    {
        var active = room.PlayerStates.Where(x => !x.IsBankrupt).ToList();
        if (active.Count <= 1)
        {
            FinishGameLocked(room, active.FirstOrDefault() ?? room.PlayerStates.First(), "Bankruptcy");
            return;
        }

        if (room.RoundNumber >= GameRules.MaxRounds)
        {
            var winner = GameRules.GetWinnerByAsset(room.PlayerStates, room.PropertyStates) ?? active.First();
            FinishGameLocked(room, winner, "MaxRound");
        }
    }

    private void FinishGameLocked(GameRoom room, PlayerState winner, string reason)
    {
        if (room.Status == "Finished")
        {
            return;
        }

        room.Status = "Finished";
        room.WinnerUserId = winner.UserId;
        room.WinnerUserName = winner.UserName;
        room.EndReason = reason;
        AddLog(room, $"游戏结束，获胜者：{winner.UserName}");
        _recordService.SaveGame(room, winner, reason);
    }

    private void AdvanceTurnLocked(GameRoom room)
    {
        if (room.Status != "Playing")
        {
            return;
        }

        var guard = 0;
        do
        {
            room.CurrentPlayerIndex = (room.CurrentPlayerIndex + 1) % room.PlayerStates.Count;
            if (room.CurrentPlayerIndex == 0)
            {
                room.RoundNumber += 1;
            }

            guard += 1;
        } while (room.PlayerStates[room.CurrentPlayerIndex].IsBankrupt && guard <= room.PlayerStates.Count);

        guard = 0;
        while (guard < room.PlayerStates.Count)
        {
            var current = room.PlayerStates[room.CurrentPlayerIndex];
            if (current.IsBankrupt)
            {
                room.CurrentPlayerIndex = (room.CurrentPlayerIndex + 1) % room.PlayerStates.Count;
                if (room.CurrentPlayerIndex == 0)
                {
                    room.RoundNumber += 1;
                }

                guard += 1;
                continue;
            }

            if (current.SkipTurnRounds <= 0)
            {
                break;
            }

            current.SkipTurnRounds -= 1;
            AddLog(room, $"{current.UserName} 正在休息，自动跳过本回合");
            room.CurrentPlayerIndex = (room.CurrentPlayerIndex + 1) % room.PlayerStates.Count;
            if (room.CurrentPlayerIndex == 0)
            {
                room.RoundNumber += 1;
            }

            guard += 1;
        }

        room.CurrentPlayerRolled = false;
        AddLog(room, $"轮到 {GetCurrentPlayerLocked(room).UserName}");
    }

    private PlayerState GetCurrentPlayerLocked(GameRoom room)
    {
        if (room.CurrentPlayerIndex >= room.PlayerStates.Count)
        {
            room.CurrentPlayerIndex = 0;
        }

        return room.PlayerStates[room.CurrentPlayerIndex];
    }

    private GameRoom RequireRoom(ClientUser user)
    {
        if (!user.RoomId.HasValue)
        {
            throw new InvalidOperationException("请先进入房间");
        }

        return GetRoom(user.RoomId.Value) ?? throw new InvalidOperationException("房间不存在");
    }

    private GameRoom RequirePlayingRoom(ClientUser user)
    {
        var room = RequireRoom(user);
        if (room.Status != "Playing")
        {
            throw new InvalidOperationException("游戏尚未开始");
        }

        return room;
    }

    private static void AddPlayerLocked(GameRoom room, ClientUser user)
    {
        user.TokenImageFile = FirstAvailableToken(room, user.TokenImageFile);
        user.RoomId = room.RoomId;
        room.Players.Add(user);
        room.PlayerStates.Add(new PlayerState
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Position = 0,
            Money = GameRules.InitialMoney,
            SkipTurnRounds = 0,
            TokenImageFile = user.TokenImageFile
        });
    }

    private static string FirstAvailableToken(GameRoom room, string preferredToken)
    {
        var normalizedPreferred = NormalizeTokenImageFile(preferredToken);
        var used = room.PlayerStates
            .Select(x => x.TokenImageFile)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!used.Contains(normalizedPreferred))
        {
            return normalizedPreferred;
        }

        return new[] { "棋子-红.png", "棋子-黄.png", "棋子-蓝.png", "棋子-绿.png" }
            .FirstOrDefault(x => !used.Contains(x))
            ?? normalizedPreferred;
    }

    private static RoomSummaryDto ToSummary(GameRoom room)
    {
        lock (room.Gate)
        {
            return new RoomSummaryDto(
                room.RoomId,
                room.RoomName,
                room.Players.FirstOrDefault(x => x.UserId == room.OwnerUserId)?.UserName ?? string.Empty,
                room.MaxPlayers,
                room.Status,
                room.Players.Count,
                room.PlayerStates.Count(x => x.IsReady),
                room.PlayerStates
                    .Select(x => x.TokenImageFile)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList());
        }
    }

    private static string NormalizeTokenImageFile(string? tokenImageFile)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "棋子-红.png",
            "棋子-黄.png",
            "棋子-蓝.png",
            "棋子-绿.png"
        };

        if (string.IsNullOrWhiteSpace(tokenImageFile))
        {
            return "棋子-红.png";
        }

        var fileName = Path.GetFileName(tokenImageFile.Trim());
        return allowed.Contains(fileName) ? fileName : "棋子-红.png";
    }

    private static void AddLog(GameRoom room, string text)
    {
        room.Logs.Add($"[{DateTime.Now:HH:mm:ss}] {text}");
        if (room.Logs.Count > 200)
        {
            room.Logs.RemoveRange(0, room.Logs.Count - 200);
        }
    }

    private static void AddAction(GameRoom room, int userId, string actionType, string description)
    {
        room.PendingActions.Add(new PendingActionRecord(userId, room.RoundNumber, actionType, description, DateTime.Now));
    }

    private static void AddNotification<T>(GameRoom room, string type, T payload)
    {
        room.PendingNotifications.Add(NetMessage.Create(type, payload));
    }

    private static void ClearNotificationsLocked(GameRoom room)
    {
        room.PendingNotifications.Clear();
    }
}
