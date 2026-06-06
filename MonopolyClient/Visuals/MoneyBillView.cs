namespace MonopolyClient.Visuals;

public class MoneyBillView : Control
{
    private int _money;
    private bool _isBankrupt;

    public MoneyBillView()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        DoubleBuffered = true;
        MinimumSize = new Size(92, 36);
        BackColor = Color.Transparent;
    }

    public int Money
    {
        get => _money;
        set
        {
            if (_money == value)
            {
                return;
            }

            _money = value;
            Invalidate();
        }
    }

    public bool IsBankrupt
    {
        get => _isBankrupt;
        set
        {
            if (_isBankrupt == value)
            {
                return;
            }

            _isBankrupt = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        var bounds = ClientRectangle;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var image = AssetCatalog.GetImage("钞票.png") ?? AssetCatalog.GetImage("钱.png");
        if (image is not null)
        {
            e.Graphics.DrawImage(image, bounds);
        }
        else
        {
            using var fallback = new SolidBrush(Color.FromArgb(218, 184, 82));
            e.Graphics.FillRectangle(fallback, bounds);
        }

        var text = IsBankrupt ? "破产" : Money.ToString("N0");
        var textRect = Rectangle.Inflate(bounds, -12, -8);
        textRect.Offset(0, 1);
        using var font = FitFont(e.Graphics, text, textRect, FontStyle.Bold);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };
        using var shadow = new SolidBrush(Color.FromArgb(180, 44, 31, 16));
        using var textBrush = new SolidBrush(IsBankrupt ? Color.FromArgb(155, 35, 32) : Color.FromArgb(55, 31, 13));
        var shadowRect = textRect;
        shadowRect.Offset(1, 1);
        e.Graphics.DrawString(text, font, shadow, shadowRect, format);
        e.Graphics.DrawString(text, font, textBrush, textRect, format);
    }

    private static Font FitFont(Graphics graphics, string text, Rectangle textRect, FontStyle style)
    {
        var maxSize = Math.Max(10F, Math.Min(16F, textRect.Height * 0.64F));
        for (var size = maxSize; size >= 8F; size -= 0.5F)
        {
            var candidate = new Font("Microsoft YaHei UI", size, style);
            var measured = graphics.MeasureString(text, candidate);
            if (measured.Width <= textRect.Width && measured.Height <= textRect.Height)
            {
                return candidate;
            }

            candidate.Dispose();
        }

        return new Font("Microsoft YaHei UI", 8F, style);
    }
}
