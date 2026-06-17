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
        if (!db.MapCells.Any())
        {
            var cells = new[]
            {
                new MapCell { CellIndex = 0, CellName = "起点", CellType = "Start", Description = "经过或停留获得200金币" },
                new MapCell { CellIndex = 1, CellName = "郑州", CellType = "Property", Description = "二七纪念塔" },
                new MapCell { CellIndex = 2, CellName = "机会", CellType = "Chance", Description = "豫见机会卡" },
                new MapCell { CellIndex = 3, CellName = "开封", CellType = "Property", Description = "龙亭大殿" },
                new MapCell { CellIndex = 4, CellName = "烩面税", CellType = "Tax", Description = "烩面顶级套餐消费税" },
                new MapCell { CellIndex = 5, CellName = "洛阳", CellType = "Property", Description = "隋唐洛阳城应天门" },
                new MapCell { CellIndex = 6, CellName = "休息", CellType = "Empty", Description = "黄河生态廊道露营" },
                new MapCell { CellIndex = 7, CellName = "平顶山", CellType = "Property", Description = "中原大佛" },
                new MapCell { CellIndex = 8, CellName = "机会", CellType = "Chance", Description = "豫见机会卡" },
                new MapCell { CellIndex = 9, CellName = "安阳", CellType = "Property", Description = "中国文字博物馆" },
                new MapCell { CellIndex = 10, CellName = "鹤壁", CellType = "Property", Description = "鹿台阁 / 朝歌文化地标" },
                new MapCell { CellIndex = 11, CellName = "胡辣汤税", CellType = "Tax", Description = "胡辣汤顶级套餐消费税" },
                new MapCell { CellIndex = 12, CellName = "新乡", CellType = "Property", Description = "潞王陵" },
                new MapCell { CellIndex = 13, CellName = "机会", CellType = "Chance", Description = "豫见机会卡" },
                new MapCell { CellIndex = 14, CellName = "焦作", CellType = "Property", Description = "嘉应观" },
                new MapCell { CellIndex = 15, CellName = "濮阳", CellType = "Property", Description = "戚城遗址会盟台" },
                new MapCell { CellIndex = 16, CellName = "面壁休息", CellType = "Empty", Description = "少林寺面壁 / 旁观练武" },
                new MapCell { CellIndex = 17, CellName = "许昌", CellType = "Property", Description = "曹魏古城" },
                new MapCell { CellIndex = 18, CellName = "机会", CellType = "Chance", Description = "豫见机会卡" },
                new MapCell { CellIndex = 19, CellName = "漯河", CellType = "Property", Description = "许慎文化园" },
                new MapCell { CellIndex = 20, CellName = "三门峡", CellType = "Property", Description = "函谷关" },
                new MapCell { CellIndex = 21, CellName = "文旅购物税", CellType = "Tax", Description = "文旅购物消费税" },
                new MapCell { CellIndex = 22, CellName = "南阳", CellType = "Property", Description = "卧龙岗武侯祠" },
                new MapCell { CellIndex = 23, CellName = "商丘", CellType = "Property", Description = "商丘古城" },
                new MapCell { CellIndex = 24, CellName = "机会", CellType = "Chance", Description = "豫见机会卡" },
                new MapCell { CellIndex = 25, CellName = "信阳", CellType = "Property", Description = "鸡公山报晓峰建筑群" },
                new MapCell { CellIndex = 26, CellName = "周口", CellType = "Property", Description = "太昊伏羲陵" },
                new MapCell { CellIndex = 27, CellName = "驻马店", CellType = "Property", Description = "嵖岈山西游文化地标" }
            };

            db.MapCells.AddRange(cells);
            db.SaveChanges();
        }

        SeedMissingProperties(db);
    }

    private static void SeedMissingProperties(MonopolyDbContext db)
    {
        var propertyCells = db.MapCells
            .Where(x => x.CellType == "Property")
            .OrderBy(x => x.CellIndex)
            .ToList();

        if (propertyCells.Count == 0)
        {
            return;
        }

        var configuredMapCellIds = db.Properties
            .Select(x => x.MapCellId)
            .ToHashSet();

        var missingProperties = propertyCells
            .Where(cell => !configuredMapCellIds.Contains(cell.Id))
            .Select((cell, index) =>
            {
                var boardOrder = propertyCells.FindIndex(x => x.Id == cell.Id);
                return new Property
                {
                    MapCellId = cell.Id,
                    PropertyName = cell.CellName,
                    Price = 200 + boardOrder * 20,
                    Rent = 40 + boardOrder * 8,
                    ColorGroup = boardOrder < 6 ? "豫北" : boardOrder < 12 ? "豫中" : "豫南"
                };
            })
            .ToList();

        if (missingProperties.Count == 0)
        {
            return;
        }

        db.Properties.AddRange(missingProperties);
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
