using MonopolyModels.Dtos;

namespace MonopolyClient.Visuals;

public class BoardView : Control
{
    private const int DiceAnimationDurationMs = 1000;
    private const int TokenMoveStepDurationMs = 620;

    private readonly Dictionary<int, string> _playerTokens = [];
    private static readonly Dictionary<int, int> DefaultPropertyPrices = AssetCatalog.DefaultBoard
        .Where(x => x.Type == "Property")
        .OrderBy(x => x.Index)
        .Select((cell, order) => new { cell.Index, Price = 200 + order * 20 })
        .ToDictionary(x => x.Index, x => x.Price);

    private readonly Random _diceRandom = new();
    private readonly System.Windows.Forms.Timer _diceTimer = new() { Interval = 16 };
    private readonly System.Windows.Forms.Timer _tokenMoveTimer = new() { Interval = 16 };
    private readonly Queue<TokenMoveAnimation> _tokenMoveQueue = [];
    private GameStateDto? _state;
    private DiceDisplay? _diceDisplay;
    private TokenMoveAnimation? _tokenMoveAnimation;
    private DateTime _diceAnimationStartedAtUtc;
    private DateTime _tokenMoveAnimationStartedAtUtc;

    public BoardView()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(31, 54, 42);
        MinimumSize = new Size(560, 560);
        _diceTimer.Tick += (_, _) => AdvanceDiceAnimation();
        _tokenMoveTimer.Tick += (_, _) => AdvanceTokenMoveAnimation();
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
        if (state.Status != "Playing")
        {
            _diceTimer.Stop();
            _tokenMoveTimer.Stop();
            _tokenMoveQueue.Clear();
            _diceDisplay = null;
            _tokenMoveAnimation = null;
        }

        Invalidate();
    }

    public void ShowDice(DiceResultDto dice)
    {
        if (dice.Dice is < 1 or > 6)
        {
            return;
        }

        _diceAnimationStartedAtUtc = DateTime.UtcNow;
        _diceDisplay = new DiceDisplay(
            dice.UserId,
            dice.UserName,
            dice.Dice,
            _diceRandom.Next(1, 7),
            0F);
        _diceTimer.Stop();
        _diceTimer.Start();
        Invalidate();
    }

    public void ShowMove(MoveResultDto move)
    {
        var mapCellCount = Math.Max(_state?.MapCells.Count ?? 0, AssetCatalog.DefaultBoard.Length);
        var path = BuildMovePath(move.OldPosition, move.NewPosition, mapCellCount, move.IsBackward);
        if (path.Count < 2)
        {
            return;
        }

        var animation = new TokenMoveAnimation(
            move.UserId,
            move.UserName,
            ResolvePlayerTokenFile(move.UserId),
            path);

        if (_tokenMoveAnimation is not null || _tokenMoveQueue.Count > 0 || IsDiceAnimationInProgress())
        {
            _tokenMoveQueue.Enqueue(animation);
        }
        else
        {
            StartTokenMoveAnimation(animation);
        }

        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _diceTimer.Dispose();
            _tokenMoveTimer.Dispose();
        }

        base.Dispose(disposing);
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
        }
        else
        {
            DrawCells(e.Graphics, board, _state);
        }

        DrawMovingPlayer(e.Graphics, board);
        DrawDiceDisplay(e.Graphics, GetCenterRect(board));
        DrawPlayerMoneyLedger(e.Graphics, board);
    }

    private Rectangle GetBoardBounds()
    {
        var size = Math.Min(ClientSize.Width, ClientSize.Height) - 4;
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
            DrawCell(
                graphics,
                GetCellRect(board, cell.Index),
                cell.Index,
                cell.Name,
                cell.Type,
                cell.ImageFile,
                [],
                null,
                DefaultPropertyPrices.TryGetValue(cell.Index, out var emptyBoardPrice) ? emptyBoardPrice : null);
        }
    }

    private void DrawCells(Graphics graphics, Rectangle board, GameStateDto state)
    {
        var cellsByIndex = state.MapCells.ToDictionary(x => x.CellIndex);
        var movingUserId = GetDisplayedMovingUserId();
        foreach (var fallback in AssetCatalog.DefaultBoard)
        {
            cellsByIndex.TryGetValue(fallback.Index, out var cell);
            var players = state.Players
                .Where(x => x.Position == fallback.Index && !x.IsBankrupt && x.UserId != movingUserId)
                .ToList();
            var type = cell?.CellType ?? fallback.Type;
            var property = cell is null
                ? null
                : state.Properties.FirstOrDefault(x => x.MapCellId == cell.Id);
            var displayPrice = property?.Price
                ?? (type == "Property" && DefaultPropertyPrices.TryGetValue(fallback.Index, out var fallbackPrice)
                    ? fallbackPrice
                    : null);
            DrawCell(
                graphics,
                GetCellRect(board, fallback.Index),
                fallback.Index,
                cell?.CellName ?? fallback.Name,
                type,
                cell is null ? fallback.ImageFile : AssetCatalog.GetCellImageFile(cell),
                players,
                property,
                displayPrice);
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
        PropertyStateDto? property,
        int? displayPrice)
    {
        var inner = Rectangle.Inflate(rect, -4, -4);
        var image = AssetCatalog.GetImage(imageFile);
        if (image is not null)
        {
            using var matte = new SolidBrush(Color.FromArgb(231, 214, 164));
            graphics.FillRectangle(matte, inner);
            DrawImageFit(graphics, image, GetCellImageArea(inner));
        }
        else
        {
            using var brush = new SolidBrush(FallbackCellColor(type));
            graphics.FillRectangle(brush, inner);
        }

        using var border = new Pen(players.Count == 0 ? Color.FromArgb(102, 70, 31) : Color.FromArgb(255, 226, 118), players.Count == 0 ? 2 : 4);
        graphics.DrawRectangle(border, inner);

        if (displayPrice is not null)
        {
            DrawPriceBadge(graphics, inner, displayPrice.Value);
        }

        if (property?.OwnerUserName is not null)
        {
            DrawOwnerBadge(graphics, inner, property.OwnerUserName);
        }

        DrawCellInfo(graphics, inner, index, name, property);
        DrawPlayers(graphics, inner, players);
    }

    private static void DrawPriceBadge(Graphics graphics, Rectangle rect, int price)
    {
        var text = $"¥{price}";
        using var font = new Font("Microsoft YaHei UI", rect.Width < 78 ? 7.2F : 8.2F, FontStyle.Bold);
        var measured = Size.Ceiling(graphics.MeasureString(text, font));
        var badgeHeight = Math.Clamp(rect.Height / 7, 18, 24);
        var badgeWidth = Math.Min(rect.Width - 10, Math.Max(52, measured.Width + 16));
        if (badgeWidth <= 40 || badgeHeight <= 16)
        {
            return;
        }

        var footerReserve = CellInfoHeight(rect) + 5;
        var badge = new Rectangle(
            rect.Left + (rect.Width - badgeWidth) / 2,
            Math.Max(rect.Top + 5, rect.Bottom - footerReserve - badgeHeight - 2),
            badgeWidth,
            badgeHeight);

        using var shadowBrush = new SolidBrush(Color.FromArgb(150, 42, 24, 8));
        var shadow = badge;
        shadow.Offset(1, 2);
        var radius = Math.Max(5, Math.Min(9, badge.Height / 2));
        graphics.FillRoundedRectangle(shadowBrush, shadow, radius);

        using var fillBrush = new SolidBrush(Color.FromArgb(244, 255, 221, 77));
        using var borderPen = new Pen(Color.FromArgb(245, 111, 58, 12), 2F);
        using var textBrush = new SolidBrush(Color.FromArgb(74, 33, 8));
        graphics.FillRoundedRectangle(fillBrush, badge, radius);
        graphics.DrawRoundedRectangle(borderPen, badge, radius);
        graphics.DrawString(text, font, textBrush, badge, CenterFormat());
    }

    private void DrawPlayerMoneyLedger(Graphics graphics, Rectangle board)
    {
        if (_state is null || _state.Players.Count == 0)
        {
            return;
        }

        var players = _state.Players
            .OrderBy(x => x.UserId)
            .ToList();
        var area = GetMoneyLedgerArea(board, out var vertical);
        if (area.Width <= 0 || area.Height <= 0)
        {
            return;
        }

        if (vertical)
        {
            DrawVerticalMoneyLedger(graphics, area, players);
        }
        else
        {
            DrawHorizontalMoneyLedger(graphics, area, players);
        }
    }

    private Rectangle GetMoneyLedgerArea(Rectangle board, out bool vertical)
    {
        const int padding = 14;
        var rightArea = new Rectangle(
            board.Right + padding,
            board.Top + padding,
            ClientSize.Width - board.Right - padding * 2,
            board.Height - padding * 2);
        if (rightArea.Width >= 150 && rightArea.Height >= 86)
        {
            vertical = true;
            return rightArea;
        }

        var bottomArea = new Rectangle(
            board.Left + padding,
            board.Bottom + padding,
            board.Width - padding * 2,
            ClientSize.Height - board.Bottom - padding * 2);
        if (bottomArea.Width >= 150 && bottomArea.Height >= 76)
        {
            vertical = false;
            return bottomArea;
        }

        vertical = true;
        return Rectangle.Empty;
    }

    private static void DrawVerticalMoneyLedger(Graphics graphics, Rectangle area, IReadOnlyList<PlayerStateDto> players)
    {
        var aspect = GetMoneyBillAspect();
        var gap = Math.Clamp(area.Height / 90, 7, 12);
        var maxBillHeight = (area.Height - gap * (players.Count - 1)) / players.Count;
        var billWidth = Math.Min(area.Width, Math.Min(340, (int)Math.Round(maxBillHeight * aspect)));
        var billHeight = (int)Math.Round(billWidth / aspect);
        if (billHeight * players.Count + gap * (players.Count - 1) > area.Height)
        {
            billHeight = Math.Max(1, (area.Height - gap * (players.Count - 1)) / players.Count);
            billWidth = Math.Min(area.Width, (int)Math.Round(billHeight * aspect));
        }

        if (billWidth < 136 || billHeight < 64)
        {
            return;
        }

        var totalHeight = billHeight * players.Count + gap * (players.Count - 1);
        var x = area.Right - billWidth;
        var y = area.Bottom - totalHeight;
        for (var i = 0; i < players.Count; i++)
        {
            DrawMoneyBillRecord(graphics, new Rectangle(x, y + i * (billHeight + gap), billWidth, billHeight), players[i]);
        }
    }

    private static void DrawHorizontalMoneyLedger(Graphics graphics, Rectangle area, IReadOnlyList<PlayerStateDto> players)
    {
        var aspect = GetMoneyBillAspect();
        var gap = Math.Clamp(area.Width / 80, 7, 12);
        var billWidth = Math.Min(300, (area.Width - gap * (players.Count - 1)) / players.Count);
        var billHeight = Math.Min(area.Height, (int)Math.Round(billWidth / aspect));
        billWidth = Math.Min(billWidth, (int)Math.Round(billHeight * aspect));
        if (billWidth < 136 || billHeight < 64)
        {
            return;
        }

        var totalWidth = billWidth * players.Count + gap * (players.Count - 1);
        var x = area.Right - totalWidth;
        var y = area.Bottom - billHeight;
        for (var i = 0; i < players.Count; i++)
        {
            DrawMoneyBillRecord(graphics, new Rectangle(x + i * (billWidth + gap), y, billWidth, billHeight), players[i]);
        }
    }

    private static void DrawMoneyBillRecord(Graphics graphics, Rectangle rect, PlayerStateDto player)
    {
        var shadow = rect;
        shadow.Offset(3, 4);
        using var shadowBrush = new SolidBrush(Color.FromArgb(90, 15, 25, 18));
        graphics.FillRoundedRectangle(shadowBrush, shadow, 8);

        var image = AssetCatalog.GetImage("钞票.png") ?? AssetCatalog.GetImage("钱.png");
        if (image is not null)
        {
            graphics.DrawImage(image, rect);
        }
        else
        {
            using var fallback = new SolidBrush(Color.FromArgb(232, 202, 120));
            graphics.FillRoundedRectangle(fallback, rect, 8);
        }

        var nameRect = new Rectangle(
            rect.Left + (int)Math.Round(rect.Width * 0.22F),
            rect.Top + (int)Math.Round(rect.Height * 0.38F),
            (int)Math.Round(rect.Width * 0.60F),
            Math.Max(10, (int)Math.Round(rect.Height * 0.12F)));
        var moneyRect = new Rectangle(
            rect.Left + (int)Math.Round(rect.Width * 0.24F),
            rect.Top + (int)Math.Round(rect.Height * 0.51F),
            (int)Math.Round(rect.Width * 0.56F),
            Math.Max(18, (int)Math.Round(rect.Height * 0.24F)));
        var moneyText = player.IsBankrupt ? "破产" : player.Money.ToString();

        using var nameFont = FitFont(graphics, player.UserName, nameRect, FontStyle.Bold, 5.5F, Math.Max(7F, rect.Height * 0.10F));
        using var moneyFont = FitFont(graphics, moneyText, moneyRect, FontStyle.Bold, 10F, Math.Max(13F, rect.Height * 0.24F));
        using var format = CenterFormat();
        using var nameBrush = new SolidBrush(Color.FromArgb(86, 53, 20));
        using var moneyBrush = new SolidBrush(player.IsBankrupt ? Color.FromArgb(150, 32, 28) : Color.FromArgb(58, 32, 12));
        using var outlineBrush = new SolidBrush(Color.FromArgb(155, 255, 235, 174));
        DrawOutlinedString(graphics, player.UserName, nameFont, nameBrush, outlineBrush, nameRect);
        graphics.DrawString(moneyText, moneyFont, moneyBrush, moneyRect, format);
    }

    private static float GetMoneyBillAspect()
    {
        var image = AssetCatalog.GetImage("钞票.png") ?? AssetCatalog.GetImage("钱.png");
        return image is null ? 1.986F : image.Width / (float)image.Height;
    }

    private static Font FitFont(Graphics graphics, string text, Rectangle textRect, FontStyle style, float minSize, float maxSize)
    {
        var clampedMax = Math.Max(minSize, maxSize);
        for (var size = clampedMax; size >= minSize; size -= 0.5F)
        {
            var candidate = new Font("Microsoft YaHei UI", size, style);
            var measured = graphics.MeasureString(text, candidate);
            if (measured.Width <= textRect.Width && measured.Height <= textRect.Height)
            {
                return candidate;
            }

            candidate.Dispose();
        }

        return new Font("Microsoft YaHei UI", minSize, style);
    }

    private static void DrawOwnerBadge(Graphics graphics, Rectangle rect, string owner)
    {
        var badgeWidth = Math.Min(rect.Width - 8, 36);
        var badge = new Rectangle(rect.Right - badgeWidth - 4, rect.Top + 4, badgeWidth, 18);
        using var brush = new SolidBrush(Color.FromArgb(210, 38, 46, 50));
        using var font = new Font("Microsoft YaHei UI", 7F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.White);
        graphics.FillRoundedRectangle(brush, badge, 5);
        graphics.DrawString("已购", font, textBrush, badge, CenterFormat());
    }

    private static void DrawCellInfo(Graphics graphics, Rectangle rect, int index, string name, PropertyStateDto? property)
    {
        var infoHeight = CellInfoHeight(rect);
        var info = new Rectangle(rect.Left + 4, rect.Bottom - infoHeight - 4, rect.Width - 8, infoHeight);
        using var nameFont = new Font("Microsoft YaHei UI", rect.Width < 72 ? 7.5F : 8F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.FromArgb(58, 35, 17));
        using var shadowBrush = new SolidBrush(Color.FromArgb(220, 255, 248, 218));
        DrawOutlinedString(graphics, name, nameFont, textBrush, shadowBrush, info);
    }

    private static Rectangle GetCellImageArea(Rectangle rect)
    {
        // Artwork should keep its original aspect ratio; fill the tile background first so
        // any letterboxing looks intentional. Text stays as outlined overlay, not an opaque panel.
        return Rectangle.Inflate(rect, -2, -2);
    }

    private static int CellInfoHeight(Rectangle rect)
    {
        return rect.Height >= 92 ? 36 : 24;
    }

    private static void DrawOutlinedString(Graphics graphics, string text, Font font, Brush textBrush, Brush outlineBrush, Rectangle layout)
    {
        using var format = CenterFormat();
        var offsets = new[]
        {
            new Point(-1, 0),
            new Point(1, 0),
            new Point(0, -1),
            new Point(0, 1)
        };

        foreach (var offset in offsets)
        {
            var shadowRect = new Rectangle(layout.Left + offset.X, layout.Top + offset.Y, layout.Width, layout.Height);
            graphics.DrawString(text, font, outlineBrush, shadowRect, format);
        }

        graphics.DrawString(text, font, textBrush, layout, format);
    }

    private void DrawPlayers(Graphics graphics, Rectangle rect, List<PlayerStateDto> players)
    {
        if (players.Count == 0)
        {
            return;
        }

        var tokenRects = GetTokenRects(rect, players.Count);
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            DrawToken(graphics, tokenRects[i], ResolvePlayerTokenFile(player), false);
        }
    }

    private void AdvanceTokenMoveAnimation()
    {
        if (_tokenMoveAnimation is null)
        {
            StartNextQueuedTokenMoveOrStop();
            return;
        }

        var elapsed = (DateTime.UtcNow - _tokenMoveAnimationStartedAtUtc).TotalMilliseconds;
        var duration = Math.Max(TokenMoveStepDurationMs, (_tokenMoveAnimation.Value.Path.Count - 1) * TokenMoveStepDurationMs);
        if (elapsed >= duration)
        {
            StartNextQueuedTokenMoveOrStop();
        }

        Invalidate();
    }

    private void StartTokenMoveAnimation(TokenMoveAnimation animation)
    {
        _tokenMoveAnimationStartedAtUtc = DateTime.UtcNow;
        _tokenMoveAnimation = animation;
        _tokenMoveTimer.Stop();
        _tokenMoveTimer.Start();
        Invalidate();
    }

    private void StartNextQueuedTokenMoveOrStop()
    {
        _tokenMoveTimer.Stop();
        _tokenMoveAnimation = null;

        if (IsDiceAnimationInProgress())
        {
            Invalidate();
            return;
        }

        if (_tokenMoveQueue.Count > 0)
        {
            StartTokenMoveAnimation(_tokenMoveQueue.Dequeue());
            return;
        }
    }

    private void DrawMovingPlayer(Graphics graphics, Rectangle board)
    {
        if (_tokenMoveAnimation is null)
        {
            if (!IsDiceAnimationInProgress() || _tokenMoveQueue.Count == 0)
            {
                return;
            }

            var pending = _tokenMoveQueue.Peek();
            var pendingRect = GetAnimatedTokenRect(board, pending.Path[0], pending.UserId);
            DrawToken(graphics, pendingRect, pending.TokenImageFile, false);
            return;
        }

        var animation = _tokenMoveAnimation.Value;
        var stepCount = animation.Path.Count - 1;
        if (stepCount <= 0)
        {
            return;
        }

        var elapsed = (DateTime.UtcNow - _tokenMoveAnimationStartedAtUtc).TotalMilliseconds;
        var stepOffset = Math.Clamp(elapsed / TokenMoveStepDurationMs, 0D, stepCount);
        var stepIndex = Math.Min(stepCount - 1, (int)Math.Floor(stepOffset));
        var stepProgress = (float)Math.Clamp(stepOffset - stepIndex, 0D, 1D);
        var fromRect = GetAnimatedTokenRect(board, animation.Path[stepIndex], animation.UserId);
        var toRect = GetAnimatedTokenRect(board, animation.Path[stepIndex + 1], animation.UserId);
        var eased = EaseInOutCubic(stepProgress);
        var centerX = Lerp(fromRect.Left + fromRect.Width / 2F, toRect.Left + toRect.Width / 2F, eased);
        var centerY = Lerp(fromRect.Top + fromRect.Height / 2F, toRect.Top + toRect.Height / 2F, eased);
        var size = Lerp(fromRect.Width, toRect.Width, eased);
        var bounce = (float)Math.Sin(stepProgress * Math.PI);
        var liftedCenterY = centerY - Math.Min(24F, size * 0.62F) * bounce;
        var scaledSize = Math.Min(76F, size * (1.48F + 0.07F * bounce));
        var tokenRect = new RectangleF(
            centerX - scaledSize / 2F,
            liftedCenterY - scaledSize / 2F,
            scaledSize,
            scaledSize);

        DrawToken(graphics, tokenRect, animation.TokenImageFile, true);
    }

    private static void DrawImageFit(Graphics graphics, Image image, Rectangle destination)
    {
        if (destination.Width <= 0 || destination.Height <= 0)
        {
            return;
        }

        var sourceAspect = image.Width / (float)image.Height;
        var destinationAspect = destination.Width / (float)destination.Height;
        Rectangle drawRect;
        if (sourceAspect >= destinationAspect)
        {
            var height = (int)Math.Round(destination.Width / sourceAspect);
            drawRect = new Rectangle(destination.Left, destination.Top + (destination.Height - height) / 2, destination.Width, height);
        }
        else
        {
            var width = (int)Math.Round(destination.Height * sourceAspect);
            drawRect = new Rectangle(destination.Left + (destination.Width - width) / 2, destination.Top, width, destination.Height);
        }

        graphics.DrawImage(image, drawRect);
    }

    private Rectangle GetAnimatedTokenRect(Rectangle board, int cellIndex, int userId)
    {
        var rect = GetCellRect(board, cellIndex);
        var inner = Rectangle.Inflate(rect, -4, -4);
        var otherPlayers = _state?.Players
            .Where(x => !x.IsBankrupt && x.UserId != userId && x.Position == cellIndex)
            .ToList() ?? [];
        var tokenRects = GetTokenRects(inner, otherPlayers.Count + 1);
        return tokenRects[Math.Min(otherPlayers.Count, tokenRects.Count - 1)];
    }

    private static List<Rectangle> GetTokenRects(Rectangle rect, int count)
    {
        const int gap = 3;
        const int margin = 6;
        var footerReserve = rect.Height >= 92 ? 40 : 28;
        var available = new Rectangle(
            rect.Left + margin,
            rect.Top + margin,
            Math.Max(20, rect.Width - margin * 2),
            Math.Max(20, rect.Height - footerReserve - margin));
        var columns = available.Width >= 74 && count > 1 ? 2 : 1;
        var rows = (int)Math.Ceiling(count / (double)columns);
        var tokenSize = Math.Min(
            48,
            Math.Max(
                20,
                Math.Min(
                    (available.Width - (columns - 1) * gap) / columns,
                    (available.Height - (rows - 1) * gap) / Math.Max(1, rows))));
        var startX = available.Right - columns * tokenSize - (columns - 1) * gap;
        var startY = available.Top;
        var tokenRects = new List<Rectangle>(count);

        for (var i = 0; i < count; i++)
        {
            tokenRects.Add(new Rectangle(
                startX + (i % columns) * (tokenSize + gap),
                startY + (i / columns) * (tokenSize + gap),
                tokenSize,
                tokenSize));
        }

        return tokenRects;
    }

    private string ResolvePlayerTokenFile(PlayerStateDto player)
    {
        var file = _playerTokens.GetValueOrDefault(player.UserId);
        if (string.IsNullOrWhiteSpace(file))
        {
            file = string.IsNullOrWhiteSpace(player.TokenImageFile)
                ? AssetCatalog.TokenOptions[player.UserId % AssetCatalog.TokenOptions.Length].ImageFile
                : player.TokenImageFile;
        }

        return file;
    }

    private string ResolvePlayerTokenFile(int userId)
    {
        if (_state?.Players.FirstOrDefault(x => x.UserId == userId) is { } player)
        {
            return ResolvePlayerTokenFile(player);
        }

        var file = _playerTokens.GetValueOrDefault(userId);
        return string.IsNullOrWhiteSpace(file)
            ? AssetCatalog.TokenOptions[Math.Abs(userId) % AssetCatalog.TokenOptions.Length].ImageFile
            : file;
    }

    private static void DrawToken(Graphics graphics, Rectangle rect, string imageFile, bool withShadow)
    {
        DrawToken(graphics, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), imageFile, withShadow);
    }

    private static void DrawToken(Graphics graphics, RectangleF rect, string imageFile, bool withShadow)
    {
        if (withShadow)
        {
            using var shadowBrush = new SolidBrush(Color.FromArgb(92, 30, 24, 13));
            graphics.FillEllipse(shadowBrush, rect.Left + 2, rect.Top + rect.Height - 6, rect.Width - 4, 7);
        }

        var token = AssetCatalog.GetImage(imageFile);
        if (token is not null)
        {
            graphics.DrawImage(token, rect);
        }
        else
        {
            using var brush = new SolidBrush(Color.FromArgb(221, 64, 47));
            graphics.FillEllipse(brush, rect);
        }
    }

    private static List<int> BuildMovePath(int oldPosition, int newPosition, int mapCellCount, bool isBackward)
    {
        var path = new List<int>();
        if (mapCellCount <= 0)
        {
            return path;
        }

        var current = NormalizeCellIndex(oldPosition, mapCellCount);
        var target = NormalizeCellIndex(newPosition, mapCellCount);
        path.Add(current);
        for (var guard = 0; guard < mapCellCount && current != target; guard++)
        {
            current = isBackward
                ? (current - 1 + mapCellCount) % mapCellCount
                : (current + 1) % mapCellCount;
            path.Add(current);
        }

        return path;
    }

    private static int NormalizeCellIndex(int index, int mapCellCount)
    {
        return ((index % mapCellCount) + mapCellCount) % mapCellCount;
    }

    private void AdvanceDiceAnimation()
    {
        if (_diceDisplay is null)
        {
            _diceTimer.Stop();
            return;
        }

        var elapsed = (DateTime.UtcNow - _diceAnimationStartedAtUtc).TotalMilliseconds;
        var progress = Math.Clamp((float)(elapsed / DiceAnimationDurationMs), 0F, 1F);
        var display = _diceDisplay.Value;
        _diceDisplay = progress >= 1F
            ? display with { DisplayValue = display.FinalValue, Progress = 1F }
            : display with { DisplayValue = _diceRandom.Next(1, 7), Progress = progress };

        if (progress >= 1F)
        {
            _diceTimer.Stop();
            if (_tokenMoveAnimation is null && _tokenMoveQueue.Count > 0)
            {
                StartNextQueuedTokenMoveOrStop();
            }
        }

        Invalidate();
    }

    private bool IsDiceAnimationInProgress()
    {
        return _diceDisplay is not null && _diceTimer.Enabled;
    }

    private int? GetDisplayedMovingUserId()
    {
        if (_tokenMoveAnimation is not null)
        {
            return _tokenMoveAnimation.Value.UserId;
        }

        return IsDiceAnimationInProgress() && _tokenMoveQueue.Count > 0
            ? _tokenMoveQueue.Peek().UserId
            : null;
    }

    private void DrawDiceDisplay(Graphics graphics, Rectangle center)
    {
        if (_diceDisplay is null || center.Width <= 0 || center.Height <= 0)
        {
            return;
        }

        var display = _diceDisplay.Value;
        var eased = EaseOutCubic(display.Progress);
        var panelWidth = Math.Min(center.Width - 28, Math.Max(260, Math.Min(310, (int)Math.Round(center.Width * 0.68))));
        var panelHeight = Math.Min(center.Height - 18, Math.Max(290, Math.Min(350, (int)Math.Round(center.Height * 0.78))));
        var panel = CenteredRectangle(center, panelWidth, panelHeight);
        var shadow = panel;
        shadow.Offset(0, 5);

        using var shadowBrush = new SolidBrush(Color.FromArgb(92, 30, 24, 13));
        using var panelBrush = new SolidBrush(Color.FromArgb(224, 246, 225, 176));
        using var border = new Pen(Color.FromArgb(145, 88, 36), 2);
        graphics.FillRoundedRectangle(shadowBrush, shadow, 14);
        graphics.FillRoundedRectangle(panelBrush, panel, 14);
        graphics.DrawRoundedRectangle(border, panel, 14);

        using var titleFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
        using var valueFont = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
        using var darkBrush = new SolidBrush(Color.FromArgb(72, 45, 25));
        using var accentBrush = new SolidBrush(Color.FromArgb(125, 51, 27));

        var titleRect = new Rectangle(panel.Left + 14, panel.Top + 8, panel.Width - 28, 28);
        var valueRect = new Rectangle(panel.Left + 14, panel.Bottom - 40, panel.Width - 28, 32);
        graphics.DrawString($"{display.UserName} 掷出", titleFont, darkBrush, titleRect, CenterFormat());
        graphics.DrawString($"{display.DisplayValue} 点", valueFont, accentBrush, valueRect, CenterFormat());

        var diceTop = titleRect.Bottom + 8;
        var diceBottom = valueRect.Top - 10;
        var diceSize = Math.Min(
            250,
            Math.Max(
                190,
                Math.Min(panel.Width - 32, diceBottom - diceTop)));
        var diceRect = new Rectangle(
            panel.Left + (panel.Width - diceSize) / 2,
            diceTop + Math.Max(0, (diceBottom - diceTop - diceSize) / 2),
            diceSize,
            diceSize);
        DrawAnimatedDice(graphics, diceRect, display.DisplayValue, eased);
    }

    private static void DrawAnimatedDice(Graphics graphics, Rectangle rect, int value, float progress)
    {
        var wobble = (float)Math.Sin(progress * Math.PI * 4) * (1F - progress);
        var angle = wobble * 9F;
        var lift = (int)Math.Round(-7 * Math.Sin(progress * Math.PI) * (1F - progress));
        var scale = 0.96F + 0.04F * progress + 0.03F * (float)Math.Sin(progress * Math.PI) * (1F - progress);
        var centerX = rect.Left + rect.Width / 2F;
        var centerY = rect.Top + rect.Height / 2F + lift;
        var saved = graphics.Save();

        graphics.TranslateTransform(centerX, centerY);
        graphics.RotateTransform(angle);
        graphics.ScaleTransform(scale, scale);
        var drawRect = new Rectangle(
            -rect.Width / 2,
            -rect.Height / 2,
            rect.Width,
            rect.Height);

        var image = AssetCatalog.GetImage($"{value}.png");
        if (image is not null)
        {
            graphics.DrawImage(image, drawRect);
        }
        else
        {
            DrawDiceFallback(graphics, drawRect, value);
        }

        graphics.Restore(saved);
    }

    private static void DrawDiceFallback(Graphics graphics, Rectangle rect, int value)
    {
        using var faceBrush = new SolidBrush(Color.White);
        using var border = new Pen(Color.FromArgb(117, 74, 35), 2);
        using var pipBrush = new SolidBrush(Color.FromArgb(44, 32, 23));
        graphics.FillRoundedRectangle(faceBrush, rect, 10);
        graphics.DrawRoundedRectangle(border, rect, 10);

        var pip = Math.Max(6, rect.Width / 9);
        var left = rect.Left + rect.Width / 4;
        var center = rect.Left + rect.Width / 2;
        var right = rect.Right - rect.Width / 4;
        var top = rect.Top + rect.Height / 4;
        var middle = rect.Top + rect.Height / 2;
        var bottom = rect.Bottom - rect.Height / 4;

        void Pip(int x, int y)
        {
            graphics.FillEllipse(pipBrush, x - pip / 2, y - pip / 2, pip, pip);
        }

        if (value is 1 or 3 or 5)
        {
            Pip(center, middle);
        }

        if (value is >= 2)
        {
            Pip(left, top);
            Pip(right, bottom);
        }

        if (value is >= 4)
        {
            Pip(right, top);
            Pip(left, bottom);
        }

        if (value == 6)
        {
            Pip(left, middle);
            Pip(right, middle);
        }
    }

    private static Rectangle CenteredRectangle(Rectangle bounds, int width, int height)
    {
        return new Rectangle(
            bounds.Left + (bounds.Width - width) / 2,
            bounds.Top + (bounds.Height - height) / 2,
            width,
            height);
    }

    private static float EaseOutCubic(float value)
    {
        var inverse = 1F - value;
        return 1F - inverse * inverse * inverse;
    }

    private static float EaseInOutCubic(float value)
    {
        return value < 0.5F
            ? 4F * value * value * value
            : 1F - MathF.Pow(-2F * value + 2F, 3F) / 2F;
    }

    private static float Lerp(float from, float to, float progress)
    {
        return from + (to - from) * progress;
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

internal readonly record struct DiceDisplay(int UserId, string UserName, int FinalValue, int DisplayValue, float Progress);

internal readonly record struct TokenMoveAnimation(int UserId, string UserName, string TokenImageFile, IReadOnlyList<int> Path);

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
        // Keep every outer-track cell visually consistent: 8 equal cells per side
        // means each corner and middle segment is approximately square.
        var ring = Math.Clamp((int)Math.Round(board.Width / 8D), 56, board.Width / 4);
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

    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int radius)
    {
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        graphics.DrawPath(pen, path);
    }
}
