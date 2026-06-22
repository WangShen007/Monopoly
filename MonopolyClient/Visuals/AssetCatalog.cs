using MonopolyModels.Dtos;

namespace MonopolyClient.Visuals;

public static class AssetCatalog
{
    private static readonly Dictionary<string, Image> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static readonly TokenOption[] TokenOptions =
    [
        new("红色棋子", "棋子-红.png"),
        new("黄色棋子", "棋子-黄.png"),
        new("蓝色棋子", "棋子-蓝.png"),
        new("绿色棋子", "棋子-绿.png")
    ];

    public static readonly BoardCellInfo[] DefaultBoard =
    [
        new(0, "起点", "Start", "起点.png"),
        new(1, "郑州", "Property", "郑州.png"),
        new(2, "机会", "Chance", "机会卡.png"),
        new(3, "开封", "Property", "开封.png"),
        new(4, "烩面税", "Tax", "烩面.png"),
        new(5, "洛阳", "Property", "洛阳.png"),
        new(6, "休息", "Empty", "休息.png"),
        new(7, "平顶山", "Property", "平顶山.png"),
        new(8, "机会", "Chance", "机会卡.png"),
        new(9, "安阳", "Property", "安阳.png"),
        new(10, "鹤壁", "Property", "鹤壁.png"),
        new(11, "胡辣汤税", "Tax", "胡辣汤.png"),
        new(12, "新乡", "Property", "新乡.png"),
        new(13, "机会", "Chance", "机会卡.png"),
        new(14, "焦作", "Property", "焦作.png"),
        new(15, "濮阳", "Property", "濮阳.png"),
        new(16, "休息", "Empty", "休息-2.png"),
        new(17, "许昌", "Property", "许昌.png"),
        new(18, "机会", "Chance", "机会卡.png"),
        new(19, "漯河", "Property", "漯河.png"),
        new(20, "三门峡", "Property", "三门峡.png"),
        new(21, "文旅税", "Tax", "文旅盲盒.png"),
        new(22, "南阳", "Property", "南阳.png"),
        new(23, "商丘", "Property", "商丘.png"),
        new(24, "机会", "Chance", "机会卡.png"),
        new(25, "信阳", "Property", "信阳.png"),
        new(26, "周口", "Property", "周口.png"),
        new(27, "驻马店", "Property", "驻马店.png")
    ];

    public static string AssetDirectory { get; } = ResolveAssetDirectory();

    public static Image? GetImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        if (Cache.TryGetValue(fileName, out var cached))
        {
            return cached;
        }

        var path = Path.Combine(AssetDirectory, fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var loaded = Image.FromStream(stream);
        var image = new Bitmap(loaded);
        Cache[fileName] = image;
        return image;
    }

    public static string GetCellImageFile(MapCellDto cell)
    {
        if (cell.CellType == "Start")
        {
            return "起点.png";
        }

        if (cell.CellType == "Chance")
        {
            return "机会卡.png";
        }

        if (cell.CellType == "Tax")
        {
            if (cell.CellName.Contains("胡辣汤", StringComparison.OrdinalIgnoreCase))
            {
                return "胡辣汤.png";
            }

            if (cell.CellName.Contains("文旅", StringComparison.OrdinalIgnoreCase)
                || cell.CellName.Contains("购物", StringComparison.OrdinalIgnoreCase))
            {
                return "文旅盲盒.png";
            }

            return "烩面.png";
        }

        if (cell.CellType == "Empty")
        {
            return cell.CellName.Contains("面壁", StringComparison.OrdinalIgnoreCase)
                || cell.CellName.Contains("少林", StringComparison.OrdinalIgnoreCase)
                ? "休息-2.png"
                : "休息.png";
        }

        var cityFile = $"{cell.CellName}.png";
        if (File.Exists(Path.Combine(AssetDirectory, cityFile)))
        {
            return cityFile;
        }

        return DefaultBoard.FirstOrDefault(x => x.Index == cell.CellIndex)?.ImageFile ?? string.Empty;
    }

    private static string ResolveAssetDirectory()
    {
        foreach (var candidate in EnumerateAssetDirectories())
        {
            if (IsAssetDirectory(candidate))
            {
                return candidate;
            }
        }

        return AppContext.BaseDirectory;
    }

    private static IEnumerable<string> EnumerateAssetDirectories()
    {
        var anchors = new[]
        {
            Path.GetDirectoryName(typeof(AssetCatalog).Assembly.Location),
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var anchor in anchors.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            var directory = anchor;
            while (!string.IsNullOrWhiteSpace(directory))
            {
                foreach (var candidate in new[]
                {
                    Path.Combine(directory, "Assets"),
                    Path.Combine(directory, "MonopolyClient", "Assets")
                })
                {
                    var fullPath = Path.GetFullPath(candidate);
                    if (seen.Add(fullPath))
                    {
                        yield return fullPath;
                    }
                }

                directory = Directory.GetParent(directory)?.FullName;
            }
        }

        var sourceAssets = GetSourceAssetDirectory();
        if (!string.IsNullOrWhiteSpace(sourceAssets))
        {
            var fullPath = Path.GetFullPath(sourceAssets);
            if (seen.Add(fullPath))
            {
                yield return fullPath;
            }
        }
    }

    private static string? GetSourceAssetDirectory(
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var sourceDirectory = Path.GetDirectoryName(sourceFilePath);
        return string.IsNullOrWhiteSpace(sourceDirectory)
            ? null
            : Path.GetFullPath(Path.Combine(sourceDirectory, "..", "Assets"));
    }

    private static bool IsAssetDirectory(string directory)
    {
        return Directory.Exists(directory)
            && File.Exists(Path.Combine(directory, "棋盘中间.png"))
            && File.Exists(Path.Combine(directory, "烩面.png"));
    }
}

public record TokenOption(string DisplayName, string ImageFile);

public record BoardCellInfo(int Index, string Name, string Type, string ImageFile);
