using Microsoft.EntityFrameworkCore;
using MonopolyModels.Entities;

namespace MonopolyModels.Data;

public static class DatabaseBootstrapper
{
    public static void EnsureCreatedAndSeeded(string? databasePath = null)
    {
        using var db = string.IsNullOrWhiteSpace(databasePath)
            ? new MonopolyDbContext()
            : new MonopolyDbContext(databasePath);

        db.Database.EnsureCreated();
        SeedMap(db);
        SeedEvents(db);
    }

    private static void SeedMap(MonopolyDbContext db)
    {
        if (db.MapCells.Any())
        {
            return;
        }

        var cells = new[]
        {
            new MapCell { CellIndex = 0, CellName = "起点", CellType = "Start", Description = "经过或停留获得200金币" },
            new MapCell { CellIndex = 1, CellName = "桃园路", CellType = "Property", Description = "低价地产" },
            new MapCell { CellIndex = 2, CellName = "机会", CellType = "Chance", Description = "抽取机会卡" },
            new MapCell { CellIndex = 3, CellName = "学院街", CellType = "Property", Description = "低价地产" },
            new MapCell { CellIndex = 4, CellName = "税务局", CellType = "Tax", Description = "缴纳固定税金" },
            new MapCell { CellIndex = 5, CellName = "图书馆", CellType = "Property", Description = "校园地产" },
            new MapCell { CellIndex = 6, CellName = "休息区", CellType = "Empty", Description = "无事件" },
            new MapCell { CellIndex = 7, CellName = "实验楼", CellType = "Property", Description = "校园地产" },
            new MapCell { CellIndex = 8, CellName = "机会", CellType = "Chance", Description = "抽取机会卡" },
            new MapCell { CellIndex = 9, CellName = "体育馆", CellType = "Property", Description = "公共设施" },
            new MapCell { CellIndex = 10, CellName = "商业街", CellType = "Property", Description = "中价地产" },
            new MapCell { CellIndex = 11, CellName = "税务局", CellType = "Tax", Description = "缴纳固定税金" },
            new MapCell { CellIndex = 12, CellName = "科技园", CellType = "Property", Description = "中价地产" },
            new MapCell { CellIndex = 13, CellName = "机会", CellType = "Chance", Description = "抽取机会卡" },
            new MapCell { CellIndex = 14, CellName = "金融中心", CellType = "Property", Description = "高价地产" },
            new MapCell { CellIndex = 15, CellName = "休息区", CellType = "Empty", Description = "无事件" },
            new MapCell { CellIndex = 16, CellName = "软件园", CellType = "Property", Description = "高价地产" },
            new MapCell { CellIndex = 17, CellName = "机会", CellType = "Chance", Description = "抽取机会卡" },
            new MapCell { CellIndex = 18, CellName = "会展中心", CellType = "Property", Description = "高价地产" },
            new MapCell { CellIndex = 19, CellName = "税务局", CellType = "Tax", Description = "缴纳固定税金" }
        };

        db.MapCells.AddRange(cells);
        db.SaveChanges();

        var propertyCells = db.MapCells
            .Where(x => x.CellType == "Property")
            .OrderBy(x => x.CellIndex)
            .ToList();

        var properties = propertyCells
            .Select((cell, index) => new Property
            {
                MapCellId = cell.Id,
                PropertyName = cell.CellName,
                Price = 180 + index * 45,
                Rent = 45 + index * 12,
                ColorGroup = index < 2 ? "蓝色" : index < 5 ? "绿色" : "红色"
            })
            .ToList();

        db.Properties.AddRange(properties);
        db.SaveChanges();
    }

    private static void SeedEvents(MonopolyDbContext db)
    {
        if (db.EventCards.Any())
        {
            return;
        }

        db.EventCards.AddRange(
            new EventCard { EventName = "奖学金", EventType = "AddMoney", Value = 180, Description = "获得奖学金180金币", IsEnabled = true },
            new EventCard { EventName = "设备维修", EventType = "MinusMoney", Value = 120, Description = "支付维修费120金币", IsEnabled = true },
            new EventCard { EventName = "快车道", EventType = "MoveForward", Value = 3, Description = "前进3步", IsEnabled = true },
            new EventCard { EventName = "迷路", EventType = "MoveBackward", Value = 2, Description = "后退2步", IsEnabled = true },
            new EventCard { EventName = "回到起点", EventType = "GoToStart", Value = 200, Description = "回到起点并获得200金币", IsEnabled = true },
            new EventCard { EventName = "免租卡", EventType = "FreeRent", Value = 1, Description = "获得一次免租机会", IsEnabled = true }
        );
        db.SaveChanges();
    }
}
