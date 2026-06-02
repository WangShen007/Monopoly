using Microsoft.EntityFrameworkCore;
using MonopolyModels.Data;
using MonopolyModels.Dtos;
using MonopolyModels.Entities;
using MonopolyModels.States;

namespace MonopolyServer.Services;

public class RecordService
{
    private readonly string _databasePath;

    public RecordService(string databasePath)
    {
        _databasePath = databasePath;
    }

    public void SaveGame(GameRoom room, PlayerState winner, string endReason)
    {
        if (room.RecordsSaved)
        {
            return;
        }

        using var db = new MonopolyDbContext(_databasePath);
        var record = new GameRecord
        {
            RoomId = room.RoomId,
            WinnerUserId = winner.UserId,
            StartedAt = room.StartedAt,
            EndedAt = DateTime.Now,
            EndReason = endReason
        };

        db.GameRecords.Add(record);
        db.SaveChanges();

        var ranking = room.PlayerStates
            .OrderByDescending(x => GameRules.CalculateAsset(x, room.PropertyStates))
            .Select((player, index) => new { Player = player, Rank = index + 1 })
            .ToList();

        foreach (var row in ranking)
        {
            db.PlayerGameRecords.Add(new PlayerGameRecord
            {
                GameRecordId = record.Id,
                UserId = row.Player.UserId,
                FinalMoney = row.Player.Money,
                PropertyCount = room.PropertyStates.Count(x => x.OwnerUserId == row.Player.UserId),
                Rank = row.Rank,
                IsWinner = row.Player.UserId == winner.UserId
            });

            var user = db.Users.FirstOrDefault(x => x.Id == row.Player.UserId);
            if (user is null)
            {
                continue;
            }

            user.TotalGames += 1;
            user.TotalMoney += Math.Max(0, row.Player.Money);
            if (row.Player.UserId == winner.UserId)
            {
                user.Wins += 1;
            }
            else
            {
                user.Losses += 1;
            }
        }

        foreach (var action in room.PendingActions)
        {
            db.ActionRecords.Add(new ActionRecord
            {
                GameRecordId = record.Id,
                UserId = action.UserId,
                RoundNumber = action.RoundNumber,
                ActionType = action.ActionType,
                Description = action.Description,
                CreatedAt = action.CreatedAt
            });
        }

        db.SaveChanges();
        room.RecordsSaved = true;
    }

    public List<RankUserDto> GetRankList()
    {
        using var db = new MonopolyDbContext(_databasePath);
        return db.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Wins)
            .ThenByDescending(x => x.TotalMoney)
            .ThenBy(x => x.UserName)
            .Take(20)
            .Select(x => new RankUserDto(
                x.Id,
                x.UserName,
                x.Wins,
                x.Losses,
                x.TotalGames,
                x.TotalMoney,
                x.TotalGames == 0 ? 0 : Math.Round((double)x.Wins / x.TotalGames, 3)))
            .ToList();
    }

    public List<HistoryRecordDto> GetHistory()
    {
        using var db = new MonopolyDbContext(_databasePath);
        var users = db.Users.AsNoTracking().ToDictionary(x => x.Id, x => x.UserName);
        return db.GameRecords
            .AsNoTracking()
            .OrderByDescending(x => x.EndedAt)
            .Take(50)
            .AsEnumerable()
            .Select(x => new HistoryRecordDto(
                x.Id,
                x.RoomId,
                x.WinnerUserId.HasValue && users.TryGetValue(x.WinnerUserId.Value, out var name) ? name : "无",
                x.StartedAt,
                x.EndedAt,
                x.EndReason))
            .ToList();
    }
}
