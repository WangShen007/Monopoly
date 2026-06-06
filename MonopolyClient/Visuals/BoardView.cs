using MonopolyModels.Dtos;

namespace MonopolyClient.Visuals;

public class BoardView : Control
{
    private readonly Dictionary<int, string> _playerTokens = [];
    private GameStateDto? _state;

    public BoardView()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(31, 54, 42);
        MinimumSize = new Size(560, 560);
    }

    public void SetToken(int userId, string imageFile)
    {
        if (userId > 0 && !string.IsNullOrWhiteSpace(imageFile))
        {
            _playerTokens[userId] = imageFile;
            Invalidate();
        }
    }

    public void ApplyState(GameStateDto state)
    {
        _state = state;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        var board = GetBoardBounds();
        DrawBoardBackground(e.Graphics, board);

        if (_state is null)
        {
            DrawEmptyBoard(e.Graphics, board);
            return;
        }

        DrawCells(e.Graphics, board, _state);
    }

    private Rectangle GetBoardBounds()
    {
        var size = Math.Min(ClientSize.Width, ClientSize.Height) - 16;
        size = Math.Max(320, size);
        return new Rectangle(
            (ClientSize.Width - size) / 2,
            (ClientSize.Height - size) / 2,
            size,
            size);
    }

    private static Rectangle GetCellRect(Rectangle board, int index)
    {
        var geometry = BoardGeometry.From(board);
        return index switch
        {
            0 => geometry.BottomLeftCorner,
            >= 1 and <= 6 => geometry.HorizontalSegment(index - 1, true),
            7 => geometry.BottomRightCorner,
            >= 8 and <= 13 => geometry.VerticalSegment(13 - index, true),
            14 => geometry.TopRightCorner,
            >= 15 and <= 20 => geometry.HorizontalSegment(20 - index, false),
            21 => geometry.TopLeftCorner,
            >= 22 and <= 27 => geometry.VerticalSegment(index - 22, false),
            _ => Rectangle.Empty
        };
    }

    private static Rectangle GetCenterRect(Rectangle board)
    {
        return BoardGeometry.From(board).Center;
    }

    private static void DrawBoardBackground(Graphics graphics, Rectangle board)
    {
        using var outer = new SolidBrush(Color.FromArgb(28, 61, 47));
        using var line = new Pen(Color.FromArgb(218, 171, 78), 5);
        graphics.FillRectangle(outer, board);
        graphics.DrawRectangle(line, Rectangle.Inflate(board, -3, -3));

        var center = GetCenterRect(board);
        var centerImage = AssetCatalog.GetImage("棋盘中间.png");
        if (centerImage is not null)
        {
            graphics.DrawImage(centerImage, center);
        }
        else
        {
            using var brush = new SolidBrush(Color.FromArgb(238, 218, 164));
            graphics.FillRectangle(brush, center);
        }

        using var centerLine = new Pen(Color.FromArgb(116, 83, 39), 2);
        graphics.DrawRectangle(centerLine, center);
    }

    private void DrawEmptyBoard(Graphics graphics, Rectangle board)
    {
        foreach (var cell in AssetCatalog.DefaultBoard)
        {
            DrawCell(graphics, GetCellRect(board, cell.Index), cell.Index, cell.Name, cell.Type, cell.ImageFile, [], null);
        }
    }

    private void DrawCells(Graphics graphics, Rectangle board, GameStateDto state)
    {
        var cellsByIndex = state.MapCells.ToDictionary(x => x.CellIndex);
        foreach (var fallback in AssetCatalog.DefaultBoard)
        {
            cellsByIndex.TryGetValue(fallback.Index, out var cell);
            var players = state.Players
                .Where(x => x.Position == fallback.Index && !x.IsBankrupt)
                .ToList();
            var property = cell is null
                ? null
                : state.Properties.FirstOrDefault(x => x.MapCellId == cell.Id);
            DrawCell(
                graphics,
                GetCellRect(board, fallback.Index),
                fallback.Index,
                cell?.CellName ?? fallback.Name,
                cell?.CellType ?? fallback.Type,
                cell is null ? fallback.ImageFile : AssetCatalog.GetCellImageFile(cell),
                players,
                property);
        }
    }

    private void DrawCell(
        Graphics graphics,
        Rectangle rect,
        int index,
        string name,
        string type,
        string imageFile,
        List<PlayerStateDto> players,
        PropertyStateDto? property)
    {
        var inner = Rectangle.Inflate(rect, -4, -4);
        var image = AssetCatalog.GetImage(imageFile);
        if (image is not null)
        {
            graphics.DrawImage(image, inner);
        }
        else
        {
            using var brush = new SolidBrush(FallbackCellColor(type));
            graphics.FillRectangle(brush, inner);
        }

        using var border = new Pen(players.Count == 0 ? Color.FromArgb(102, 70, 31) : Color.FromArgb(255, 226, 118), players.Count == 0 ? 2 : 4);
        graphics.DrawRectangle(border, inner);

        if (property?.OwnerUserName is not null)
        {
            DrawOwnerBadge(graphics, inner, property.OwnerUserName);
        }

        DrawCellCaption(graphics, inner, index, name);
        DrawPlayers(graphics, inner, players);
    }

    private static void DrawOwnerBadge(Graphics graphics, Rectangle rect, string owner)
    {
        var badge = new Rectangle(rect.Left + 6, rect.Top + 6, Math.Min(rect.Width - 12, 76), 22);
        using var brush = new SolidBrush(Color.FromArgb(218, 38, 46, 50));
        using var font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.White);
        graphics.FillRoundedRectangle(brush, badge, 5);
        graphics.DrawString(owner, font, textBrush, badge, CenterFormat());
    }

    private static void DrawCellCaption(Graphics graphics, Rectangle rect, int index, string name)
    {
        var caption = new Rectangle(rect.Left + 5, rect.Bottom - 27, rect.Width - 10, 22);
        using var brush = new SolidBrush(Color.FromArgb(215, 246, 225, 176));
        using var font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.FromArgb(58, 35, 17));
        graphics.FillRoundedRectangle(brush, caption, 5);
        graphics.DrawString($"{index} {name}", font, textBrush, caption, CenterFormat());
    }

    private void DrawPlayers(Graphics graphics, Rectangle rect, List<PlayerStateDto> players)
    {
        if (players.Count == 0)
        {
            return;
        }

        var available = Rectangle.Inflate(rect, -7, -7);
        available.Height = Math.Max(12, available.Height - 29);
        var columns = available.Width >= 58 && players.Count > 1 ? 2 : 1;
        var rows = (int)Math.Ceiling(players.Count / (double)columns);
        const int gap = 3;
        var tokenSize = Math.Min(
            34,
            Math.Max(
                14,
                Math.Min(
                    (available.Width - (columns - 1) * gap) / columns,
                    (available.Height - (rows - 1) * gap) / Math.Max(1, rows))));
        var startX = available.Right - columns * tokenSize - (columns - 1) * gap;
        var startY = available.Top;

        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var x = startX + (i % columns) * (tokenSize + gap);
            var y = startY + (i / columns) * (tokenSize + gap);
            var tokenRect = new Rectangle(x, y, tokenSize, tokenSize);
            var file = _playerTokens.GetValueOrDefault(player.UserId);
            if (string.IsNullOrWhiteSpace(file))
            {
                file = string.IsNullOrWhiteSpace(player.TokenImageFile)
                    ? AssetCatalog.TokenOptions[player.UserId % AssetCatalog.TokenOptions.Length].ImageFile
                    : player.TokenImageFile;
            }

            var token = AssetCatalog.GetImage(file);
            if (token is not null)
            {
                graphics.DrawImage(token, tokenRect);
            }
            else
            {
                using var brush = new SolidBrush(Color.FromArgb(221, 64, 47));
                graphics.FillEllipse(brush, tokenRect);
            }
        }
    }

    private static Color FallbackCellColor(string type)
    {
        return type switch
        {
            "Start" => Color.FromArgb(218, 242, 229),
            "Property" => Color.FromArgb(225, 236, 252),
            "Chance" => Color.FromArgb(255, 244, 204),
            "Tax" => Color.FromArgb(255, 224, 224),
            _ => Color.FromArgb(239, 241, 244)
        };
    }

    private static StringFormat CenterFormat()
    {
        return new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };
    }
}

internal readonly record struct BoardGeometry(Rectangle Board, int RingThickness)
{
    private const int MiddleSegments = 6;

    public Rectangle Center => Rectangle.FromLTRB(
        Board.Left + RingThickness,
        Board.Top + RingThickness,
        Board.Right - RingThickness,
        Board.Bottom - RingThickness);

    public Rectangle TopLeftCorner => new(Board.Left, Board.Top, RingThickness, RingThickness);
    public Rectangle TopRightCorner => new(Board.Right - RingThickness, Board.Top, RingThickness, RingThickness);
    public Rectangle BottomLeftCorner => new(Board.Left, Board.Bottom - RingThickness, RingThickness, RingThickness);
    public Rectangle BottomRightCorner => new(Board.Right - RingThickness, Board.Bottom - RingThickness, RingThickness, RingThickness);

    public static BoardGeometry From(Rectangle board)
    {
        var ring = Math.Clamp((int)Math.Round(board.Width * 0.20), 58, board.Width / 4);
        return new BoardGeometry(board, ring);
    }

    public Rectangle HorizontalSegment(int segmentIndex, bool bottom)
    {
        var segment = MiddleSegment(Board.Left + RingThickness, Board.Width - RingThickness * 2, segmentIndex);
        return new Rectangle(segment.Start, bottom ? Board.Bottom - RingThickness : Board.Top, segment.Length, RingThickness);
    }

    public Rectangle VerticalSegment(int segmentIndex, bool right)
    {
        var segment = MiddleSegment(Board.Top + RingThickness, Board.Height - RingThickness * 2, segmentIndex);
        return new Rectangle(right ? Board.Right - RingThickness : Board.Left, segment.Start, RingThickness, segment.Length);
    }

    private static (int Start, int Length) MiddleSegment(int start, int totalLength, int index)
    {
        var segmentStart = start + totalLength * index / MiddleSegments;
        var segmentEnd = start + totalLength * (index + 1) / MiddleSegments;
        return (segmentStart, segmentEnd - segmentStart);
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int radius)
    {
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        graphics.FillPath(brush, path);
    }
}
