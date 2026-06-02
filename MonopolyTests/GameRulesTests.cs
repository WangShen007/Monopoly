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
    public void Bootstrapper_CreatesSeededDatabase()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"monopoly-test-{Guid.NewGuid():N}");
        var dbPath = Path.Combine(tempDir, "test.db");
        try
        {
            DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);

            using var db = new MonopolyDbContext(dbPath);

            Assert.Equal(20, db.MapCells.Count());
            Assert.True(db.Properties.Count() >= 8);
            Assert.True(db.EventCards.Count(x => x.IsEnabled) >= 5);
            Assert.Equal("Start", db.MapCells.Single(x => x.CellIndex == 0).CellType);
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
