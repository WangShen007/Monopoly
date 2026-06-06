using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MonopolyModels.Data;
using MonopolyModels.Dtos;
using MonopolyModels.Entities;
using MonopolyServer.Services;

namespace MonopolyServer.Networking;

public class GameTcpServer
{
    private const string PasswordHashPrefix = "pbkdf2-sha256$";
    private const int PasswordHashIterations = 100_000;
    private const int PasswordHashSize = 32;

    private readonly string _databasePath;
    private readonly RecordService _recordService;
    private readonly RoomService _roomService;
    private readonly List<ClientUser> _onlineUsers = [];
    private readonly object _onlineGate = new();
    private TcpListener? _listener;

    public GameTcpServer(string databasePath)
    {
        _databasePath = databasePath;
        _recordService = new RecordService(databasePath);
        _roomService = new RoomService(databasePath, _recordService);
    }

    public async Task StartAsync(string ip, int port, CancellationToken cancellationToken)
    {
        _listener = new TcpListener(IPAddress.Parse(ip), port);
        _listener.Start();
        Console.WriteLine($"服务器已启动：{ip}:{port}");
        Console.WriteLine($"SQLite 数据库：{_databasePath}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
            var user = new ClientUser(tcpClient);
            lock (_onlineGate)
            {
                _onlineUsers.Add(user);
            }

            _ = Task.Run(() => ReceiveLoopAsync(user), cancellationToken);
        }
    }

    private async Task ReceiveLoopAsync(ClientUser user)
    {
        try
        {
            await user.SendAsync(NetMessage.Create("Connected", new BasicResult(true, "已连接服务器")));
            while (user.TcpClient.Connected)
            {
                var line = await user.Reader.ReadLineAsync();
                if (line is null)
                {
                    break;
                }

                var message = JsonSerializer.Deserialize<NetMessage>(line, JsonProtocol.Options);
                if (message is not null)
                {
                    await HandleMessageAsync(user, message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端断开或异常：{ex.Message}");
        }
        finally
        {
            await RemoveClientAsync(user);
        }
    }

    private async Task HandleMessageAsync(ClientUser user, NetMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case "Register":
                    await RegisterAsync(user, message.ReadData<AuthRequest>());
                    break;
                case "Login":
                    await LoginAsync(user, message.ReadData<AuthRequest>());
                    break;
                case "GetRoomList":
                    await user.SendAsync(NetMessage.Create("RoomListResult", new RoomListResult(_roomService.GetRoomSummaries())));
                    break;
                case "CreateRoom":
                    await CreateRoomAsync(user, message.ReadData<CreateRoomRequest>());
                    break;
                case "JoinRoom":
                    await JoinRoomAsync(user, message.ReadData<JoinRoomRequest>());
                    break;
                case "LeaveRoom":
                    await LeaveRoomAsync(user);
                    break;
                case "Ready":
                    await UpdateRoomAndBroadcastAsync(_roomService.SetReady(user), "ReadyResult", new BasicResult(true, "准备成功"));
                    break;
                case "RollDice":
                    await UpdateRoomAndBroadcastAsync(_roomService.RollDice(user));
                    break;
                case "BuyProperty":
                    await UpdateRoomAndBroadcastAsync(_roomService.BuyProperty(user), "BuyPropertyResult", new BasicResult(true, "购买成功"));
                    break;
                case "EndTurn":
                    await UpdateRoomAndBroadcastAsync(_roomService.EndTurn(user));
                    break;
                case "SendChat":
                    await SendChatAsync(user, message.ReadData<ChatRequest>());
                    break;
                case "SendReaction":
                    await SendReactionAsync(user, message.ReadData<ReactionRequest>());
                    break;
                case "GetManageData":
                    RequireLoggedIn(user);
                    await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
                    break;
                case "AddMapCell":
                    await SaveMapCellAsync(user, message.ReadData<MapCellEditDto>(), false);
                    break;
                case "UpdateMapCell":
                    await SaveMapCellAsync(user, message.ReadData<MapCellEditDto>(), true);
                    break;
                case "DeleteMapCell":
                    await DeleteMapCellAsync(user, message.ReadData<EntityIdRequest>().Id);
                    break;
                case "AddProperty":
                    await SavePropertyAsync(user, message.ReadData<PropertyEditDto>(), false);
                    break;
                case "UpdateProperty":
                    await SavePropertyAsync(user, message.ReadData<PropertyEditDto>(), true);
                    break;
                case "DeleteProperty":
                    await DeletePropertyAsync(user, message.ReadData<EntityIdRequest>().Id);
                    break;
                case "AddEventCard":
                    await SaveEventCardAsync(user, message.ReadData<EventCardEditDto>(), false);
                    break;
                case "UpdateEventCard":
                    await SaveEventCardAsync(user, message.ReadData<EventCardEditDto>(), true);
                    break;
                case "DeleteEventCard":
                    await DeleteEventCardAsync(user, message.ReadData<EntityIdRequest>().Id);
                    break;
                case "GetRankList":
                    await user.SendAsync(NetMessage.Create("RankListResult", new RankListResult(_recordService.GetRankList())));
                    break;
                case "GetHistory":
                    await user.SendAsync(NetMessage.Create("HistoryResult", new HistoryResult(_recordService.GetHistory())));
                    break;
                default:
                    await SendErrorAsync(user, $"未知命令：{message.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            await SendErrorAsync(user, ex.Message);
        }
    }

    private async Task RegisterAsync(ClientUser user, AuthRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            throw new InvalidOperationException("用户名不能为空");
        }

        using var db = new MonopolyDbContext(_databasePath);
        var name = request.UserName.Trim();
        if (await db.Users.AnyAsync(x => x.UserName == name))
        {
            await user.SendAsync(NetMessage.Create("RegisterResult", new BasicResult(false, "用户名已存在")));
            return;
        }

        db.Users.Add(new User
        {
            UserName = name,
            Password = HashPassword(request.Password),
            CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("RegisterResult", new BasicResult(true, "注册成功")));
    }

    private async Task LoginAsync(ClientUser user, AuthRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            throw new InvalidOperationException("用户名不能为空");
        }

        using var db = new MonopolyDbContext(_databasePath);
        var name = request.UserName.Trim();
        var entity = await db.Users.FirstOrDefaultAsync(x => x.UserName == name);
        if (entity is null)
        {
            entity = new User
            {
                UserName = name,
                Password = HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };
            db.Users.Add(entity);
            await db.SaveChangesAsync();
        }
        else if (!VerifyPassword(entity.Password, request.Password))
        {
            await user.SendAsync(NetMessage.Create("LoginResult", new LoginResult(false, "密码错误", 0, string.Empty)));
            return;
        }
        else if (!IsHashedPassword(entity.Password))
        {
            entity.Password = HashPassword(request.Password);
            await db.SaveChangesAsync();
        }

        user.UserId = entity.Id;
        user.UserName = entity.UserName;
        user.TokenImageFile = NormalizeTokenImageFile(request.TokenImageFile);
        await user.SendAsync(NetMessage.Create("LoginResult", new LoginResult(true, "登录成功", user.UserId, user.UserName)));
        await user.SendAsync(NetMessage.Create("RoomListResult", new RoomListResult(_roomService.GetRoomSummaries())));
    }

    private async Task SendChatAsync(ClientUser user, ChatRequest request)
    {
        var text = NormalizeChatText(request.Text);
        var message = BuildChatMessage(user, "Text", text);
        await BroadcastToRoomAsync(user, NetMessage.Create("ChatMessage", message));
    }

    private async Task SendReactionAsync(ClientUser user, ReactionRequest request)
    {
        var reaction = (request.ReactionType ?? string.Empty).Trim();
        var text = reaction switch
        {
            "Egg" => "扔出一个鸡蛋",
            "Flower" => "送出一朵小花",
            "Cheer" => "发来一声喝彩",
            _ => throw new InvalidOperationException("未知互动道具")
        };
        var message = BuildChatMessage(user, "Reaction", text);
        await BroadcastToRoomAsync(user, NetMessage.Create("ChatMessage", message));
    }

    private ChatMessageDto BuildChatMessage(ClientUser user, string messageType, string text)
    {
        RequireLoggedIn(user);
        if (!user.RoomId.HasValue)
        {
            throw new InvalidOperationException("请先进入房间");
        }

        return new ChatMessageDto(
            user.RoomId.Value,
            user.UserId,
            user.UserName,
            user.TokenImageFile,
            messageType,
            text,
            DateTime.Now);
    }

    private async Task BroadcastToRoomAsync(ClientUser user, NetMessage message)
    {
        if (!user.RoomId.HasValue)
        {
            throw new InvalidOperationException("请先进入房间");
        }

        var room = _roomService.GetRoom(user.RoomId.Value)
            ?? throw new InvalidOperationException("房间不存在");
        foreach (var client in room.Players.ToList())
        {
            await client.SendAsync(message);
        }
    }

    private static string NormalizeChatText(string? text)
    {
        var normalized = (text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException("聊天内容不能为空");
        }

        return normalized.Length <= 80 ? normalized : normalized[..80];
    }

    private static string NormalizeTokenImageFile(string? tokenImageFile)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "棋子-1.png",
            "棋子-2.png",
            "文旅盲盒.png",
            "钱.png"
        };

        if (string.IsNullOrWhiteSpace(tokenImageFile))
        {
            return "棋子-1.png";
        }

        var fileName = Path.GetFileName(tokenImageFile.Trim());
        return allowed.Contains(fileName) ? fileName : "棋子-1.png";
    }

    private async Task CreateRoomAsync(ClientUser user, CreateRoomRequest request)
    {
        var room = _roomService.CreateRoom(user, request.RoomName, request.MaxPlayers);
        await user.SendAsync(NetMessage.Create("CreateRoomResult", new BasicResult(true, "创建房间成功")));
        await BroadcastRoomListAsync();
        await BroadcastGameStateAsync(room);
    }

    private async Task JoinRoomAsync(ClientUser user, JoinRoomRequest request)
    {
        var room = _roomService.JoinRoom(user, request.RoomId);
        await user.SendAsync(NetMessage.Create("JoinRoomResult", new BasicResult(true, "加入房间成功")));
        await BroadcastRoomListAsync();
        await BroadcastGameStateAsync(room);
    }

    private async Task LeaveRoomAsync(ClientUser user)
    {
        var room = _roomService.LeaveRoom(user);
        await user.SendAsync(NetMessage.Create("LeaveRoomResult", new BasicResult(true, "已离开房间")));
        await BroadcastRoomListAsync();
        if (room is not null)
        {
            await BroadcastGameStateAsync(room);
        }
    }

    private async Task UpdateRoomAndBroadcastAsync(GameRoom room, string? directType = null, object? directPayload = null)
    {
        if (directType is not null && directPayload is not null)
        {
            foreach (var client in room.Players.ToList())
            {
                await client.SendAsync(NetMessage.Create(directType, directPayload));
            }
        }

        await BroadcastRoomListAsync();
        await BroadcastGameStateAsync(room);
    }

    private async Task BroadcastGameStateAsync(GameRoom room)
    {
        var notifications = _roomService.TakeNotifications(room);
        var state = _roomService.ToGameState(room);
        foreach (var client in room.Players.ToList())
        {
            foreach (var notification in notifications)
            {
                await client.SendAsync(notification);
            }

            await client.SendAsync(NetMessage.Create("GameState", state));
            if (state.Status == "Playing" && state.CurrentPlayerUserId == client.UserId)
            {
                await client.SendAsync(NetMessage.Create("YourTurn", new BasicResult(true, "轮到你行动")));
            }
            else if (state.Status == "Finished" && state.Players.Count > 0)
            {
                var winner = room.WinnerUserId.HasValue
                    ? state.Players.FirstOrDefault(x => x.UserId == room.WinnerUserId.Value)
                    : null;
                winner ??= state.Players.OrderByDescending(x => x.Money).ThenBy(x => x.UserId).First();
                await client.SendAsync(NetMessage.Create("GameOver", new GameOverDto(
                    winner.UserId,
                    winner.UserName,
                    room.EndReason ?? "Finished",
                    state.Players.OrderByDescending(x => x.Money).ToList())));
            }
        }
    }

    private async Task BroadcastRoomListAsync()
    {
        var message = NetMessage.Create("RoomListResult", new RoomListResult(_roomService.GetRoomSummaries()));
        foreach (var client in GetOnlineUsers())
        {
            if (client.IsLoggedIn)
            {
                await client.SendAsync(message);
            }
        }
    }

    private List<ClientUser> GetOnlineUsers()
    {
        lock (_onlineGate)
        {
            return _onlineUsers.ToList();
        }
    }

    private async Task RemoveClientAsync(ClientUser user)
    {
        lock (_onlineGate)
        {
            _onlineUsers.Remove(user);
        }

        var room = _roomService.LeaveRoom(user);
        user.Close();
        await BroadcastRoomListAsync();
        if (room is not null)
        {
            await BroadcastGameStateAsync(room);
        }
    }

    private ManageDataDto GetManageData()
    {
        using var db = new MonopolyDbContext(_databasePath);
        var map = db.MapCells
            .AsNoTracking()
            .OrderBy(x => x.CellIndex)
            .Select(x => new MapCellDto(x.Id, x.CellIndex, x.CellName, x.CellType, x.Description))
            .ToList();
        var cellIndexes = map.ToDictionary(x => x.Id, x => x.CellIndex);
        var propertyEntities = db.Properties
            .AsNoTracking()
            .OrderBy(x => x.MapCellId)
            .ToList();
        var properties = propertyEntities
            .Select(x => new PropertyDto(
                x.Id,
                x.MapCellId,
                cellIndexes.GetValueOrDefault(x.MapCellId),
                x.PropertyName,
                x.Price,
                x.Rent,
                x.ColorGroup))
            .ToList();
        var events = db.EventCards
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new EventCardDto(x.Id, x.EventName, x.EventType, x.Value, x.Description, x.IsEnabled))
            .ToList();
        return new ManageDataDto(map, properties, events);
    }

    private async Task SaveMapCellAsync(ClientUser user, MapCellEditDto dto, bool update)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        MapCell entity;
        if (update)
        {
            entity = await db.MapCells.FindAsync(dto.Id) ?? throw new InvalidOperationException("地图格子不存在");
        }
        else
        {
            entity = new MapCell();
            db.MapCells.Add(entity);
        }

        entity.CellIndex = dto.CellIndex;
        entity.CellName = dto.CellName;
        entity.CellType = dto.CellType;
        entity.Description = dto.Description;
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private async Task DeleteMapCellAsync(ClientUser user, int id)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        var entity = await db.MapCells.FindAsync(id) ?? throw new InvalidOperationException("地图格子不存在");
        db.MapCells.Remove(entity);
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private async Task SavePropertyAsync(ClientUser user, PropertyEditDto dto, bool update)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        Property entity;
        if (update)
        {
            entity = await db.Properties.FindAsync(dto.Id) ?? throw new InvalidOperationException("地产不存在");
        }
        else
        {
            entity = new Property();
            db.Properties.Add(entity);
        }

        entity.MapCellId = dto.MapCellId;
        entity.PropertyName = dto.PropertyName;
        entity.Price = dto.Price;
        entity.Rent = dto.Rent;
        entity.ColorGroup = dto.ColorGroup;
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private async Task DeletePropertyAsync(ClientUser user, int id)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        var entity = await db.Properties.FindAsync(id) ?? throw new InvalidOperationException("地产不存在");
        db.Properties.Remove(entity);
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private async Task SaveEventCardAsync(ClientUser user, EventCardEditDto dto, bool update)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        EventCard entity;
        if (update)
        {
            entity = await db.EventCards.FindAsync(dto.Id) ?? throw new InvalidOperationException("事件卡不存在");
        }
        else
        {
            entity = new EventCard();
            db.EventCards.Add(entity);
        }

        entity.EventName = dto.EventName;
        entity.EventType = dto.EventType;
        entity.Value = dto.Value;
        entity.Description = dto.Description;
        entity.IsEnabled = dto.IsEnabled;
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private async Task DeleteEventCardAsync(ClientUser user, int id)
    {
        RequireLoggedIn(user);
        using var db = new MonopolyDbContext(_databasePath);
        var entity = await db.EventCards.FindAsync(id) ?? throw new InvalidOperationException("事件卡不存在");
        db.EventCards.Remove(entity);
        await db.SaveChangesAsync();
        await user.SendAsync(NetMessage.Create("ManageDataResult", GetManageData()));
    }

    private static Task SendErrorAsync(ClientUser user, string message)
    {
        return user.SendAsync(NetMessage.Create("Error", new BasicResult(false, message)));
    }

    private static void RequireLoggedIn(ClientUser user)
    {
        if (!user.IsLoggedIn)
        {
            throw new InvalidOperationException("请先登录");
        }
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            PasswordHashIterations,
            HashAlgorithmName.SHA256,
            PasswordHashSize);

        return $"{PasswordHashPrefix}{PasswordHashIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string storedPassword, string password)
    {
        if (!IsHashedPassword(storedPassword))
        {
            return storedPassword == password;
        }

        var parts = storedPassword.Split('$');
        if (parts.Length != 4
            || !int.TryParse(parts[1], out var iterations)
            || iterations <= 0)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsHashedPassword(string password)
    {
        return password.StartsWith(PasswordHashPrefix, StringComparison.Ordinal);
    }
}
