using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MonopolyModels.Data;
using MonopolyModels.Dtos;
using MonopolyModels.Entities;
using MonopolyModels.States;
using MonopolyServer.Services;

namespace MonopolyTests;

public class GameRulesTests
{
    [Fact]
    public void Move_PassingStart_AddsRewardAndWrapsPosition()
    {
        var player = new PlayerState { UserId = 1, Position = 18, Money = 1500 };

        var result = GameRules.Move(player, 5, 20);

        Assert.Equal(3, result.NewPosition);
        Assert.True(result.PassedStart);
        Assert.Equal(1700, player.Money);
    }

    [Fact]
    public void ApplyEvent_FreeRent_AddsCard()
    {
        var player = new PlayerState { UserId = 1, Money = 1000 };
        var card = new EventCard { EventType = "FreeRent", Value = 1 };

        GameRules.ApplyEvent(player, card, _ => { }, () => { });

        Assert.Equal(1, player.FreeRentCards);
        Assert.Equal(1000, player.Money);
    }

    [Fact]
    public void WinnerByAsset_UsesMoneyPlusOwnedPropertyPrice()
    {
        var lowCashOwner = new PlayerState { UserId = 1, UserName = "A", Money = 100 };
        var highCashPlayer = new PlayerState { UserId = 2, UserName = "B", Money = 500 };
        var properties = new[]
        {
            new PropertyState { OwnerUserId = 1, Price = 700 },
            new PropertyState { OwnerUserId = 2, Price = 100 }
        };

        var winner = GameRules.GetWinnerByAsset([lowCashOwner, highCashPlayer], properties);

        Assert.Equal(1, winner?.UserId);
    }

    [Fact]
    public void NetMessage_RoundTripsTypedJsonPayload()
    {
        var source = NetMessage.Create("Login", new AuthRequest("alice", "pw"));
        var json = JsonSerializer.Serialize(source, JsonProtocol.Options);
        var restored = JsonSerializer.Deserialize<NetMessage>(json, JsonProtocol.Options)!;

        var payload = restored.ReadData<AuthRequest>();

        Assert.Equal("Login", restored.Type);
        Assert.Equal("alice", payload.UserName);
        Assert.Equal("pw", payload.Password);
    }

    [Fact]
    public void NetMessage_RoundTripsChatPayload()
    {
        var sentAt = DateTime.Now;
        var source = NetMessage.Create(
            "ChatMessage",
            new ChatMessageDto(7, 21, "alice", "棋子-红.png", "Text", "我等到花儿也谢了", sentAt));
        var json = JsonSerializer.Serialize(source, JsonProtocol.Options);
        var restored = JsonSerializer.Deserialize<NetMessage>(json, JsonProtocol.Options)!;

        var payload = restored.ReadData<ChatMessageDto>();

        Assert.Equal("ChatMessage", restored.Type);
        Assert.Equal(7, payload.RoomId);
        Assert.Equal(21, payload.SenderUserId);
        Assert.Equal("alice", payload.SenderUserName);
        Assert.Equal("Text", payload.MessageType);
        Assert.Equal("我等到花儿也谢了", payload.Text);
    }

    [Fact]
    public void Bootstrapper_CreatesSeededDatabase()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-test-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);

            using var db = new MonopolyDbContext(dbPath);

            Assert.Equal(28, db.MapCells.Count());
            Assert.Equal(17, db.Properties.Count());
            Assert.True(db.EventCards.Count(x => x.IsEnabled) >= 5);
            Assert.Equal("Start", db.MapCells.Single(x => x.CellIndex == 0).CellType);
            Assert.Equal("郑州", db.MapCells.Single(x => x.CellIndex == 1).CellName);
        }
        finally
        {
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    [Fact]
    public void Bootstrapper_DoesNotOverwriteExistingMapData()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-existing-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        try
        {
            using (var db = new MonopolyDbContext(dbPath))
            {
                db.Database.EnsureCreated();
                db.MapCells.Add(new MapCell
                {
                    CellIndex = 0,
                    CellName = "自定义起点",
                    CellType = "Start",
                    Description = "保留后台维护的数据"
                });
                db.SaveChanges();
            }

            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);

            using var verify = new MonopolyDbContext(dbPath);
            Assert.Single(verify.MapCells);
            Assert.Equal("自定义起点", verify.MapCells.Single().CellName);
            Assert.True(verify.EventCards.Count(x => x.IsEnabled) >= 5);
        }
        finally
        {
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    [Fact]
    public void Bootstrapper_BackfillsMissingPropertiesForExistingMap()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-missing-props-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);
            using (var db = new MonopolyDbContext(dbPath))
            {
                db.Properties.RemoveRange(db.Properties);
                db.SaveChanges();
            }

            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);

            using var verify = new MonopolyDbContext(dbPath);
            Assert.Equal(
                verify.MapCells.Count(x => x.CellType == "Property"),
                verify.Properties.Count());
            Assert.DoesNotContain(
                verify.MapCells.Where(x => x.CellType == "Property"),
                cell => verify.Properties.All(property => property.MapCellId != cell.Id));
        }
        finally
        {
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    [Fact]
    public void RecordService_PersistsGameRecordAndUpdatesRankingStats()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-record-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);
            using (var db = new MonopolyDbContext(dbPath))
            {
                db.Users.AddRange(
                    new User { Id = 9001, UserName = "winner", Password = "1" },
                    new User { Id = 9002, UserName = "loser", Password = "1" });
                db.SaveChanges();
            }

            var room = new GameRoom
            {
                RoomId = 77,
                RoomName = "test",
                StartedAt = DateTime.Now.AddMinutes(-5),
                Status = "Finished"
            };
            var winner = new PlayerState { UserId = 9001, UserName = "winner", Money = 1600 };
            room.PlayerStates.Add(winner);
            room.PlayerStates.Add(new PlayerState { UserId = 9002, UserName = "loser", Money = 1000 });
            room.PropertyStates.Add(new PropertyState { OwnerUserId = 9001, Price = 300 });
            room.PendingActions.Add(new PendingActionRecord(9001, 1, "RollDice", "掷骰", DateTime.Now));

            var service = new RecordService(dbPath);
            service.SaveGame(room, winner, "MaxRound");

            using var verify = new MonopolyDbContext(dbPath);
            Assert.Single(verify.GameRecords);
            Assert.Equal(2, verify.PlayerGameRecords.Count());
            Assert.Single(verify.ActionRecords);
            Assert.Equal(1, verify.Users.Single(x => x.Id == 9001).Wins);
            Assert.Equal(1, verify.Users.Single(x => x.Id == 9002).Losses);
        }
        finally
        {
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    [Fact]
    public async Task RoomService_PreventsSameUserFromEnteringMultipleRooms()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"monopoly-room-{Guid.NewGuid():N}.db");
        var firstPair = await CreateTcpPairAsync();
        var secondPair = await CreateTcpPairAsync();
        try
        {
            var service = new RoomService(dbPath, new RecordService(dbPath));
            var firstUser = new ClientUser(firstPair.ServerSide)
            {
                UserId = 1001,
                UserName = "first"
            };
            var secondUser = new ClientUser(secondPair.ServerSide)
            {
                UserId = 1002,
                UserName = "second"
            };

            service.CreateRoom(firstUser, "one", 2);
            var secondRoom = service.CreateRoom(secondUser, "two", 2);

            Assert.Throws<InvalidOperationException>(() => service.CreateRoom(firstUser, "again", 2));
            Assert.Throws<InvalidOperationException>(() => service.JoinRoom(firstUser, secondRoom.RoomId));
        }
        finally
        {
            firstPair.Client.Dispose();
            firstPair.ServerSide.Dispose();
            secondPair.Client.Dispose();
            secondPair.ServerSide.Dispose();
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Fact]
    public async Task RoomService_AssignsUniqueTokensAndRejectsDuplicates()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"monopoly-token-{Guid.NewGuid():N}.db");
        var firstPair = await CreateTcpPairAsync();
        var secondPair = await CreateTcpPairAsync();
        try
        {
            var service = new RoomService(dbPath, new RecordService(dbPath));
            var firstUser = new ClientUser(firstPair.ServerSide)
            {
                UserId = 1201,
                UserName = "first",
                TokenImageFile = "棋子-红.png"
            };
            var secondUser = new ClientUser(secondPair.ServerSide)
            {
                UserId = 1202,
                UserName = "second",
                TokenImageFile = "棋子-红.png"
            };

            var room = service.CreateRoom(firstUser, "tokens", 2);
            room = service.JoinRoom(secondUser, room.RoomId);

            lock (room.Gate)
            {
                Assert.Equal(2, room.PlayerStates.Select(x => x.TokenImageFile).Distinct().Count());
            }

            Assert.Throws<InvalidOperationException>(() => service.SelectToken(secondUser, "棋子-红.png"));
            room = service.SelectToken(secondUser, "棋子-蓝.png");

            lock (room.Gate)
            {
                Assert.Equal("棋子-蓝.png", room.PlayerStates.Single(x => x.UserId == secondUser.UserId).TokenImageFile);
            }
        }
        finally
        {
            firstPair.Client.Dispose();
            firstPair.ServerSide.Dispose();
            secondPair.Client.Dispose();
            secondPair.ServerSide.Dispose();
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Fact]
    public async Task RoomService_RestCellSkipsPlayersNextTurn()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-rest-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        var firstPair = await CreateTcpPairAsync();
        var secondPair = await CreateTcpPairAsync();
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);
            var service = new RoomService(dbPath, new RecordService(dbPath));
            var firstUser = new ClientUser(firstPair.ServerSide)
            {
                UserId = 1101,
                UserName = "first"
            };
            var secondUser = new ClientUser(secondPair.ServerSide)
            {
                UserId = 1102,
                UserName = "second"
            };

            var room = service.CreateRoom(firstUser, "rest", 2);
            service.JoinRoom(secondUser, room.RoomId);
            service.SetReady(firstUser);
            room = service.SetReady(secondUser);

            var resolveCurrentCell = typeof(RoomService).GetMethod(
                "ResolveCurrentCellLocked",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(resolveCurrentCell);
            lock (room.Gate)
            {
                room.PlayerStates[0].Position = 6;
                resolveCurrentCell!.Invoke(service, [room, room.PlayerStates[0], false]);
                room.CurrentPlayerIndex = 0;
                room.CurrentPlayerRolled = true;
                Assert.Equal(1, room.PlayerStates[0].SkipTurnRounds);
            }

            room = service.EndTurn(firstUser);

            lock (room.Gate)
            {
                Assert.Equal(1, room.CurrentPlayerIndex);
                Assert.Equal(1, room.PlayerStates[0].SkipTurnRounds);
                Assert.Equal("second", room.PlayerStates[room.CurrentPlayerIndex].UserName);
            }

            lock (room.Gate)
            {
                room.CurrentPlayerRolled = true;
            }

            room = service.EndTurn(secondUser);

            lock (room.Gate)
            {
                Assert.Equal(1, room.CurrentPlayerIndex);
                Assert.Equal(0, room.PlayerStates[0].SkipTurnRounds);
                Assert.Equal("second", room.PlayerStates[room.CurrentPlayerIndex].UserName);
            }
        }
        finally
        {
            firstPair.Client.Dispose();
            firstPair.ServerSide.Dispose();
            secondPair.Client.Dispose();
            secondPair.ServerSide.Dispose();
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    [Fact]
    public async Task RoomService_ChanceMoveToTaxCell_ChargesTax()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-chance-tax-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        var firstPair = await CreateTcpPairAsync();
        var secondPair = await CreateTcpPairAsync();
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);
            var service = new RoomService(dbPath, new RecordService(dbPath));
            var firstUser = new ClientUser(firstPair.ServerSide)
            {
                UserId = 1301,
                UserName = "first"
            };
            var secondUser = new ClientUser(secondPair.ServerSide)
            {
                UserId = 1302,
                UserName = "second"
            };

            var room = service.CreateRoom(firstUser, "chance-tax", 2);
            service.JoinRoom(secondUser, room.RoomId);
            service.SetReady(firstUser);
            room = service.SetReady(secondUser);

            var resolveCurrentCell = typeof(RoomService).GetMethod(
                "ResolveCurrentCellLocked",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(resolveCurrentCell);

            lock (room.Gate)
            {
                room.EventCards =
                [
                    new EventCard
                    {
                        EventName = "快车道",
                        EventType = "MoveForward",
                        Value = 3,
                        Description = "前进3步",
                        IsEnabled = true
                    }
                ];
                room.PlayerStates[0].Position = 8;
                room.PlayerStates[0].Money = GameRules.InitialMoney;
                resolveCurrentCell!.Invoke(service, [room, room.PlayerStates[0], false]);

                Assert.Equal(11, room.PlayerStates[0].Position);
                Assert.Equal(GameRules.InitialMoney - GameRules.TaxAmount, room.PlayerStates[0].Money);
                Assert.Contains(
                    room.PendingNotifications,
                    x => x.Type == "TaxResult" && x.ReadData<TaxResultDto>().UserId == room.PlayerStates[0].UserId);
            }
        }
        finally
        {
            firstPair.Client.Dispose();
            firstPair.ServerSide.Dispose();
            secondPair.Client.Dispose();
            secondPair.ServerSide.Dispose();
            DeleteDirectoryWithRetry(tempDir);
        }
    }

    private static async Task<(TcpClient Client, TcpClient ServerSide)> CreateTcpPairAsync()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var endpoint = (IPEndPoint)listener.LocalEndpoint;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var acceptTask = listener.AcceptTcpClientAsync(cts.Token);
        var client = new TcpClient();
        await client.ConnectAsync(endpoint.Address, endpoint.Port, cts.Token);
        return (client, await acceptTask);
    }

    private static void DeleteDirectoryWithRetry(string path)
    {
        for (var i = 0; i < 5; i++)
        {
            try
            {
                SqliteConnection.ClearAllPools();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive: true);
                }

                return;
            }
            catch (IOException) when (i < 4)
            {
                Thread.Sleep(50);
            }
            catch (IOException)
            {
                return;
            }
        }
    }
}
