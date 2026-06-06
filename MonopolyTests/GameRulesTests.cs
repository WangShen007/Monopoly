using System.Net;
using System.Net.Sockets;
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
            new ChatMessageDto(7, 21, "alice", "棋子-1.png", "Text", "我等到花儿也谢了", sentAt));
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
