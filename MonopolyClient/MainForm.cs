using System.ComponentModel;
using MonopolyClient.Networking;
using MonopolyClient.Visuals;
using MonopolyModels.Dtos;

namespace MonopolyClient;

public class MainForm : Form
{
    private readonly GameTcpClient _client = new();
    private readonly Panel _loginPage = new() { Dock = DockStyle.Fill };
    private readonly Panel _mainShell = new() { Dock = DockStyle.Fill, Visible = false };
    private readonly TranslucentPanel _loginCard = new();
    private readonly TableLayoutPanel _loginLayout = new();
    private readonly TableLayoutPanel _loginButtons = new();
    private float _lastLoginScale = -1f;
    private Rectangle _lastLoginBounds = Rectangle.Empty;
    private readonly TextBox _txtIp = new() { Text = "127.0.0.1", Width = 220 };
    private readonly NumericUpDown _numPort = new() { Minimum = 1, Maximum = 65535, Value = 9000, Width = 110 };
    private readonly TextBox _txtUser = new() { Text = "player1", Width = 320 };
    private readonly TextBox _txtPassword = new() { Text = "123456", Width = 320, UseSystemPasswordChar = true };
    private readonly ComboBox _roomTokenCombo = new() { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _lblLoginStatus = new() { AutoSize = false, Height = 34, Text = "请先连接服务器", TextAlign = ContentAlignment.MiddleCenter };
    private readonly Label _lblStatus = new() { AutoSize = true, Text = "未连接" };
    private readonly Label _lblUser = new() { AutoSize = true, Text = "未登录" };
    private readonly Label _lblToken = new() { AutoSize = true, Text = "棋子：-" };
    private readonly DataGridView _roomGrid = Grid();
    private readonly TextBox _txtRoomName = new() { Text = "河南文旅房间", Width = 180 };
    private readonly NumericUpDown _numMaxPlayers = new() { Minimum = 2, Maximum = 4, Value = 2, Width = 60 };
    private readonly BoardView _boardView = new() { Dock = DockStyle.Fill };
    private readonly FlowLayoutPanel _playerCards = new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = true,
        Padding = new Padding(2),
        BackColor = Color.FromArgb(245, 236, 207)
    };
    private readonly FlowLayoutPanel _chatMessages = new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = true,
        Padding = new Padding(2),
        BackColor = Color.FromArgb(252, 248, 235)
    };
    private readonly TextBox _txtChat = new()
    {
        Dock = DockStyle.Fill,
        MaxLength = 80
    };
    private readonly GameSoundPlayer _sounds = new();
    private readonly CheckBox _chkSound = new()
    {
        Text = "音效",
        AutoSize = true,
        Checked = true,
        Margin = new Padding(0, 12, 8, 0),
        ForeColor = Color.FromArgb(64, 44, 25)
    };
    private readonly DataGridView _propertyGrid = Grid();
    private readonly TextBox _logText = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        WordWrap = true,
        BorderStyle = BorderStyle.None,
        BackColor = Color.FromArgb(252, 248, 235),
        ForeColor = Color.FromArgb(55, 38, 22),
        Font = new Font("Microsoft YaHei UI", 9F)
    };
    private readonly Label _lblTurn = new()
    {
        AutoSize = false,
        Dock = DockStyle.Fill,
        Text = "当前回合：-",
        TextAlign = ContentAlignment.MiddleLeft,
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
        ForeColor = Color.FromArgb(64, 44, 25)
    };
    private readonly Label _lblActionHint = new()
    {
        AutoSize = false,
        Dock = DockStyle.Fill,
        Text = "等待开局",
        TextAlign = ContentAlignment.MiddleLeft,
        ForeColor = Color.FromArgb(92, 67, 38)
    };
    private readonly PictureBox _dicePicture = new() { Width = 58, Height = 58, SizeMode = PictureBoxSizeMode.Zoom };
    private readonly Button _btnRoll = new() { Text = "掷骰子", Width = 92, Height = 34 };
    private readonly Button _btnBuy = new() { Text = "购买地产", Width = 92, Height = 34 };
    private readonly Button _btnEnd = new() { Text = "结束回合", Width = 92, Height = 34 };
    private readonly DataGridView _mapGrid = Grid(true);
    private readonly DataGridView _managePropertyGrid = Grid(true);
    private readonly DataGridView _eventGrid = Grid(true);
    private readonly DataGridView _rankGrid = Grid();
    private readonly DataGridView _historyGrid = Grid();
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };

    private int _userId;
    private string _userName = string.Empty;
    private string _currentTokenImageFile = "棋子-红.png";
    private bool _updatingRoomTokenCombo;
    private BindingList<MapCellRow> _mapRows = [];
    private BindingList<PropertyRow> _propertyRows = [];
    private BindingList<EventCardRow> _eventRows = [];
    private string _lastGameStatus = "";
    private int? _currentRoomId;

    public MainForm()
    {
        Text = "河南文旅大富翁";
        MinimumSize = new Size(1240, 820);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft YaHei UI", 9F);
        BackColor = Color.FromArgb(31, 54, 42);

        _client.MessageReceived += message =>
        {
            if (!IsDisposed)
            {
                BeginInvoke(() => HandleMessage(message));
            }
        };
        _client.Disconnected += reason =>
        {
            if (!IsDisposed)
            {
                BeginInvoke(() => SetStatus($"连接断开：{reason}", true));
            }
        };

        BuildUi();
        WireEvents();
        SetGameButtons(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _client.Dispose();
        }

        base.Dispose(disposing);
    }

    private void BuildUi()
    {
        _roomTokenCombo.DisplayMember = nameof(TokenOption.DisplayName);
        _roomTokenCombo.ValueMember = nameof(TokenOption.ImageFile);
        _roomTokenCombo.DataSource = AssetCatalog.TokenOptions.ToList();

        Controls.Add(_mainShell);
        Controls.Add(_loginPage);
        ConfigureRoomGrid();
        ConfigurePropertyGrid();
        BuildLoginPage();
        BuildMainShell();
        _loginPage.BringToFront();
    }

    private void BuildLoginPage()
    {
        _loginPage.BackColor = Color.FromArgb(36, 45, 33);
        _loginPage.BackgroundImage = AssetCatalog.GetImage("登录界面.png");
        _loginPage.BackgroundImageLayout = ImageLayout.Stretch;
        EnableDoubleBuffering(_loginPage);
        EnableDoubleBuffering(_loginCard);
        _loginPage.Resize += (_, _) => CenterLoginCard();

        _loginCard.BackColor = Color.Transparent;
        _loginCard.FillColor = Color.Transparent;
        _loginCard.BorderColor = Color.Transparent;
        _loginCard.Margin = Padding.Empty;
        _loginCard.BorderStyle = BorderStyle.None;

        _loginLayout.SuspendLayout();
        _loginLayout.Dock = DockStyle.Fill;
        _loginLayout.ColumnCount = 2;
        _loginLayout.RowCount = 8;
        _loginLayout.BackColor = Color.Transparent;
        _loginLayout.Margin = Padding.Empty;
        _loginLayout.Padding = Padding.Empty;
        _loginLayout.ColumnStyles.Clear();
        _loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        _loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _loginLayout.RowStyles.Clear();
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        for (var i = 1; i < 7; i++)
        {
            _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        }
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
        _loginLayout.ResumeLayout(false);

        AddLoginFormRow(1, "服务器", BuildServerRow());
        AddLoginFormRow(2, "玩家名 / ID", _txtUser);
        AddLoginFormRow(3, "通行码", _txtPassword);
        AddLoginFormRow(4, "房间名", _txtRoomName);
        AddLoginFormRow(5, "人数", _numMaxPlayers);

        _loginButtons.SuspendLayout();
        _loginButtons.Dock = DockStyle.Fill;
        _loginButtons.ColumnCount = 3;
        _loginButtons.RowCount = 1;
        _loginButtons.BackColor = Color.Transparent;
        _loginButtons.Margin = Padding.Empty;
        _loginButtons.Padding = new Padding(0, 12, 0, 0);
        _loginButtons.Controls.Clear();
        _loginButtons.ColumnStyles.Clear();
        _loginButtons.RowStyles.Clear();
        for (var i = 0; i < 3; i++)
        {
            _loginButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3f));
        }
        _loginButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var loginActionButtons = new[]
        {
            Button("连接", ConnectAsync),
            Button("注册", RegisterAsync),
            Button("进入大厅", LoginAsync)
        };
        for (var i = 0; i < loginActionButtons.Length; i++)
        {
            StyleLoginButton(loginActionButtons[i]);
            _loginButtons.Controls.Add(loginActionButtons[i], i, 0);
        }
        _loginButtons.ResumeLayout(false);

        _lblLoginStatus.Dock = DockStyle.Fill;
        _lblLoginStatus.Margin = new Padding(0, 8, 0, 0);
        _lblLoginStatus.Tag = "status";
        _lblLoginStatus.ForeColor = Color.FromArgb(91, 45, 19);

        var bottom = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Tag = "bottom"
        };
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bottom.Controls.Add(_loginButtons, 0, 0);
        bottom.Controls.Add(_lblLoginStatus, 0, 1);
        _loginLayout.Controls.Add(bottom, 0, 7);
        _loginLayout.SetColumnSpan(bottom, 2);

        _loginCard.Controls.Add(_loginLayout);
        _loginPage.Controls.Add(_loginCard);
        CenterLoginCard(force: true);
    }

    private TableLayoutPanel BuildServerRow()
    {
        var row = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent,
            Tag = "input"
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        row.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        PrepareLoginInput(_txtIp);
        PrepareLoginInput(_numPort);
        _txtIp.Dock = DockStyle.Fill;
        _numPort.Dock = DockStyle.Fill;
        _txtIp.Margin = new Padding(0, 4, 8, 4);
        _numPort.Margin = new Padding(8, 4, 0, 4);
        _txtIp.MinimumSize = new Size(0, 32);
        _numPort.MinimumSize = new Size(0, 32);
        row.Controls.Add(_txtIp, 0, 0);
        row.Controls.Add(_numPort, 1, 0);
        return row;
    }

    private void AddLoginFormRow(int rowIndex, string labelText, Control input)
    {
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Color.FromArgb(91, 45, 19),
            Margin = Padding.Empty,
            Tag = "label"
        };
        _loginLayout.Controls.Add(label, 0, rowIndex);

        PrepareLoginInput(input);
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(12, 5, 0, 5);
        _loginLayout.Controls.Add(input, 1, rowIndex);
    }

    private static void PrepareLoginInput(Control control)
    {
        control.Margin = new Padding(12, 7, 0, 7);
        control.BackColor = Color.FromArgb(255, 251, 235);
        control.ForeColor = Color.FromArgb(68, 44, 18);
        control.Tag = "input";
        if (control is TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.MinimumSize = new Size(0, 32);
        }
        else if (control is ComboBox comboBox)
        {
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.MinimumSize = new Size(0, 32);
        }
        else if (control is NumericUpDown numeric)
        {
            numeric.MinimumSize = new Size(0, 32);
        }
    }

    private static void StyleLoginButton(Button button)
    {
        button.Dock = DockStyle.Fill;
        button.Margin = new Padding(5, 4, 5, 4);
        button.MinimumSize = new Size(0, 38);
        button.AutoSize = false;
        button.Padding = new Padding(2, 0, 2, 0);
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Color.FromArgb(135, 82, 37);
        button.ForeColor = Color.White;
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.Tag = "button";
    }

    private void CenterLoginCard(bool force = false)
    {
        var clientSize = _loginPage.ClientSize;
        if (clientSize.Width <= 0 || clientSize.Height <= 0)
        {
            return;
        }

        var scale = Math.Clamp(Math.Min(clientSize.Width / 1240f, clientSize.Height / 820f), 0.78f, 1.18f);
        var cardWidth = Scaled(680, scale);
        var cardHeight = Scaled(500, scale);
        cardWidth = Math.Min(cardWidth, Math.Max(380, clientSize.Width - 56));
        cardHeight = Math.Min(Math.Max(cardHeight, 420), Math.Max(360, clientSize.Height - 56));
        var bounds = new Rectangle(
            Math.Max(24, (clientSize.Width - cardWidth) / 2),
            Math.Max(24, (clientSize.Height - cardHeight) / 2),
            cardWidth,
            cardHeight);

        if (!force && Math.Abs(scale - _lastLoginScale) <= 0.01f && _lastLoginBounds == bounds)
        {
            return;
        }

        _lastLoginScale = scale;
        _lastLoginBounds = bounds;
        _loginPage.SuspendLayout();
        _loginCard.SuspendLayout();
        _loginCard.Bounds = bounds;
        _loginCard.Padding = new Padding(Scaled(34, scale), Scaled(24, scale), Scaled(34, scale), Scaled(24, scale));
        UpdateLoginLayoutMetrics(scale);
        ApplyLoginFonts(_loginCard, scale);
        _loginCard.ResumeLayout(true);
        _loginPage.ResumeLayout(false);
    }

    private void UpdateLoginLayoutMetrics(float scale)
    {
        _loginLayout.ColumnStyles[0].Width = Scaled(190, scale);

        var availableHeight = Math.Max(330, _loginCard.ClientSize.Height - _loginCard.Padding.Vertical);
        var desiredTitle = Scaled(44, scale);
        var desiredField = Scaled(52, scale);
        var desiredBottom = Scaled(118, scale);
        var desiredTotal = desiredTitle + desiredField * 5 + desiredBottom;
        if (desiredTotal > availableHeight)
        {
            var compact = availableHeight / (float)desiredTotal;
            desiredTitle = Math.Max(30, (int)Math.Floor(desiredTitle * compact));
            desiredField = Math.Max(42, (int)Math.Floor(desiredField * compact));
            desiredBottom = Math.Max(88, availableHeight - desiredTitle - desiredField * 5);
        }

        _loginLayout.RowStyles[0].Height = desiredTitle;
        for (var i = 1; i < 6; i++)
        {
            _loginLayout.RowStyles[i].Height = desiredField;
        }
        _loginLayout.RowStyles[6].Height = 1;
        _loginLayout.RowStyles[7].Height = desiredBottom;

        if (_loginButtons.Parent is TableLayoutPanel bottom && bottom.RowStyles.Count >= 2)
        {
            bottom.RowStyles[0].Height = Math.Max(56, Math.Min(Scaled(66, scale), desiredBottom - 30));
        }

        var buttonTopPadding = Math.Max(5, Math.Min(Scaled(10, scale), desiredBottom / 10));
        _loginButtons.Padding = new Padding(0, buttonTopPadding, 0, 0);
        _lblLoginStatus.Margin = new Padding(0, Math.Max(4, Scaled(6, scale)), 0, 0);
        foreach (Control button in _loginButtons.Controls)
        {
            button.Margin = new Padding(Scaled(6, scale), Scaled(4, scale), Scaled(6, scale), Scaled(4, scale));
        }
    }

    private static void ApplyLoginFonts(Control parent, float scale)
    {
        var fontScale = Math.Clamp(scale, 0.88f, 1.16f);
        foreach (Control control in parent.Controls)
        {
            var role = control.Tag as string;
            if (role == "title")
            {
                control.Font = new Font("Microsoft YaHei UI", 23F * fontScale, FontStyle.Bold);
            }
            else if (role == "label")
            {
                control.Font = new Font("Microsoft YaHei UI", 11.5F * fontScale, FontStyle.Bold);
            }
            else if (role == "button" || control is Button)
            {
                control.Font = new Font("Microsoft YaHei UI", 10.8F * fontScale, FontStyle.Bold);
            }
            else if (role == "status")
            {
                control.Font = new Font("Microsoft YaHei UI", 10.5F * fontScale, FontStyle.Regular);
            }
            else if (role == "input" || control is TextBox or ComboBox or NumericUpDown)
            {
                control.Font = new Font("Microsoft YaHei UI", 11F * fontScale, FontStyle.Regular);
            }

            if (control.HasChildren)
            {
                ApplyLoginFonts(control, scale);
            }
        }
    }

    private static int Scaled(int value, float scale)
    {
        return Math.Max(1, (int)Math.Round(value * scale));
    }

    private static void EnableDoubleBuffering(Control control)
    {
        var property = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        property?.SetValue(control, true, null);
    }


    private void BuildMainShell()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(238, 224, 184)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 134));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _mainShell.Controls.Add(root);

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.FromArgb(238, 224, 184),
            Margin = Padding.Empty,
            Padding = new Padding(0, 0, 0, 4)
        };
        header.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        header.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));

        var identityRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 2, 0, 0)
        };
        var actionRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 0, 0, 0)
        };

        var headerFont = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        var headerStrongFont = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblUser.Font = headerFont;
        _lblToken.Font = headerFont;
        _lblStatus.Font = headerFont;
        _chkSound.Font = headerFont;
        _roomTokenCombo.Font = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Regular);
        _roomTokenCombo.Width = 166;

        identityRow.Controls.AddRange([
            _lblUser,
            Spacer(34),
            _lblToken,
            Spacer(38),
            new Label
            {
                Text = "房间棋子",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 18, 6, 0),
                Font = headerStrongFont,
                ForeColor = Color.FromArgb(64, 44, 25)
            },
            _roomTokenCombo,
            Button("选择棋子", SelectRoomTokenAsync),
            Spacer(28),
            _chkSound,
            Spacer(22),
            _lblStatus
        ]);
        actionRow.Controls.AddRange([
            Button("刷新房间", () => SendAsync("GetRoomList")),
            Button("回到登录页", ShowLoginPage)
        ]);
        _roomTokenCombo.Margin = new Padding(0, 12, 12, 0);
        _chkSound.Margin = new Padding(2, 17, 10, 0);
        _lblUser.Margin = new Padding(0, 18, 0, 0);
        _lblToken.Margin = new Padding(0, 18, 0, 0);
        _lblStatus.Margin = new Padding(8, 18, 0, 0);
        header.Controls.Add(identityRow, 0, 0);
        header.Controls.Add(actionRow, 0, 1);
        root.Controls.Add(header, 0, 0);

        _tabs.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _tabs.ItemSize = new Size(230, 44);
        _tabs.SizeMode = TabSizeMode.Fixed;
        _tabs.TabPages.Add(BuildLobbyTab());
        _tabs.TabPages.Add(BuildGameTab());
        _tabs.TabPages.Add(BuildManageTab());
        _tabs.TabPages.Add(BuildRankTab());
        root.Controls.Add(_tabs, 0, 1);
    }

    private TabPage BuildLobbyTab()
    {
        var page = Page("房间大厅");
        var layout = Split(0.78f);
        page.Controls.Add(layout);

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            BackColor = Color.FromArgb(245, 236, 207),
            Padding = new Padding(12)
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 68));

        var roomActions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.FromArgb(252, 248, 235),
            Padding = new Padding(16, 12, 16, 10),
            Margin = new Padding(0, 0, 0, 10)
        };
        roomActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        roomActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        var lobbyHint = new Label
        {
            Dock = DockStyle.Fill,
            Text = "选择房间后加入，房主可准备开局。",
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold),
            ForeColor = Color.FromArgb(92, 67, 38),
            AutoEllipsis = true
        };
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        toolbar.Controls.AddRange([
            Button("创建房间", CreateRoomAsync),
            Button("加入选中", JoinRoomAsync),
            Button("离开房间", () => SendAsync("LeaveRoom")),
            Button("准备", () => SendAsync("Ready"))
        ]);
        roomActions.Controls.Add(lobbyHint, 0, 0);
        roomActions.Controls.Add(toolbar, 0, 1);

        var roomListPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(252, 248, 235),
            Padding = new Padding(0)
        };
        roomListPanel.Controls.Add(_roomGrid);
        left.Controls.Add(roomActions, 0, 0);
        left.Controls.Add(roomListPanel, 0, 1);
        layout.Panel1.Controls.Add(left);

        var preview = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            BackgroundImage = AssetCatalog.GetImage("棋盘中间.png"),
            BackgroundImageLayout = ImageLayout.Zoom,
            BackColor = Color.FromArgb(33, 67, 49)
        };
        layout.Panel2.Controls.Add(preview);
        return page;
    }

    private TabPage BuildGameTab()
    {
        var page = Page("游戏棋盘");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 470));
        page.Controls.Add(layout);

        var boardHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4),
            BackColor = Color.FromArgb(29, 58, 44)
        };
        boardHost.Controls.Add(_boardView);
        layout.Controls.Add(boardHost, 0, 0);

        var sideScroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(238, 224, 184)
        };
        var side = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = false,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(8),
            BackColor = Color.Transparent
        };
        side.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 212));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

        side.Controls.Add(BuildTurnStatusPanel(), 0, 0);
        side.Controls.Add(BuildGameControlsPanel(), 0, 1);
        side.Controls.Add(Group("玩家状态", _playerCards), 0, 2);
        side.Controls.Add(BuildChatPanel(), 0, 3);
        side.Controls.Add(Group("详情", BuildDetailTabs()), 0, 4);
        sideScroll.Controls.Add(side);
        ResizeGameSidePanel();
        sideScroll.Resize += (_, _) => ResizeGameSidePanel();
        layout.Controls.Add(sideScroll, 1, 0);
        return page;

        void ResizeGameSidePanel()
        {
            var width = Math.Max(0, sideScroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
            var height = Math.Max(980, sideScroll.ClientSize.Height - 2);
            side.Width = width;
            side.Height = height;
        }
    }

    private Control BuildTurnStatusPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            BackColor = Color.FromArgb(245, 236, 207),
            Padding = new Padding(10, 7, 10, 7)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        _lblTurn.MinimumSize = new Size(0, 30);
        _lblActionHint.MinimumSize = new Size(0, 26);
        _lblTurn.Margin = new Padding(0, 2, 0, 2);
        _lblActionHint.Margin = new Padding(0, 2, 0, 2);
        panel.Controls.Add(_lblTurn, 0, 0);
        panel.Controls.Add(_lblActionHint, 0, 1);
        return panel;
    }

    private Control BuildGameControlsPanel()
    {
        StyleCommandButton(_btnRoll);
        StyleCommandButton(_btnBuy);
        StyleCommandButton(_btnEnd);

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(245, 236, 207),
            Padding = new Padding(8, 7, 8, 7)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _dicePicture.Dock = DockStyle.Fill;
        _dicePicture.Margin = new Padding(0, 2, 8, 2);
        _dicePicture.BackColor = Color.FromArgb(252, 248, 235);
        panel.Controls.Add(_dicePicture, 0, 0);

        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Color.Transparent
        };
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        actions.Controls.Add(_btnRoll, 0, 0);
        actions.Controls.Add(_btnBuy, 1, 0);
        actions.Controls.Add(_btnEnd, 2, 0);
        panel.Controls.Add(actions, 1, 0);
        return panel;
    }

    private Control BuildChatPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            BackColor = Color.FromArgb(245, 236, 207),
            Margin = Padding.Empty,
            Padding = new Padding(2)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));

        _chatMessages.BackColor = Color.FromArgb(252, 248, 235);
        _chatMessages.BorderStyle = BorderStyle.None;
        _chatMessages.WrapContents = false;
        _chatMessages.AutoScroll = true;

        var input = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 4, 0, 2)
        };
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        _txtChat.MinimumSize = new Size(0, 34);
        _txtChat.Margin = new Padding(0, 0, 6, 0);
        input.Controls.Add(_txtChat, 0, 0);
        var sendButton = SmallButton("发送", SendChatFromInput);
        sendButton.Dock = DockStyle.Fill;
        input.Controls.Add(sendButton, 1, 0);

        // Single readable row. The previous two-row TableLayout was compressed by WinForms
        // scaling into ~17px-high buttons, clipping Chinese glyphs. Horizontal scrolling keeps
        // every quick-chat/reaction button at a stable height in the compact game window.
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 3, 0, 0)
        };

        var chatButtons = new (string Text, Action Click)[]
        {
            (ShortQuickChat("我等到花儿也谢了"), () => SendQuickChat("我等到花儿也谢了")),
            (ShortQuickChat("不要走，决战到天亮"), () => SendQuickChat("不要走，决战到天亮")),
            (ShortQuickChat("快点吧"), () => SendQuickChat("快点吧")),
            (ShortQuickChat("这把稳了"), () => SendQuickChat("这把稳了")),
            ("鸡蛋", () => SendReaction("Egg")),
            ("鲜花", () => SendReaction("Flower")),
            ("喝彩", () => SendReaction("Cheer"))
        };
        foreach (var (text, click) in chatButtons)
        {
            actions.Controls.Add(SmallButton(text, click));
        }

        var voiceButton = new Button
        {
            Text = "语音",
            Enabled = false,
            AutoSize = false,
            Width = 52,
            Height = 38,
            MinimumSize = new Size(48, 36),
            BackColor = Color.FromArgb(222, 211, 185),
            ForeColor = Color.FromArgb(111, 90, 65),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(3, 0, 3, 0),
            Margin = new Padding(2, 4, 2, 2)
        };
        actions.Controls.Add(voiceButton);

        layout.Controls.Add(_chatMessages, 0, 0);
        layout.Controls.Add(input, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        var group = Group("聊天互动", layout);
        group.MinimumSize = new Size(0, 280);
        return group;
    }


    private Control BuildDetailTabs()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var propertyPage = new TabPage("地产") { BackColor = Color.FromArgb(245, 236, 207), Padding = new Padding(4) };
        var logPage = new TabPage("日志") { BackColor = Color.FromArgb(245, 236, 207), Padding = new Padding(4) };
        propertyPage.Controls.Add(_propertyGrid);
        logPage.Controls.Add(_logText);
        tabs.TabPages.Add(propertyPage);
        tabs.TabPages.Add(logPage);
        return tabs;
    }

    private static string ShortQuickChat(string text)
    {
        return text switch
        {
            "我等到花儿也谢了" => "等你",
            "不要走，决战到天亮" => "决战",
            "这把稳了" => "稳了",
            _ => text
        };
    }

    private TabPage BuildManageTab()
    {
        var page = Page("数据管理");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(ManagePage("地图格子", _mapGrid, AddLocalMapRow, SaveMapAsync, DeleteMapAsync));
        tabs.TabPages.Add(ManagePage("地产配置", _managePropertyGrid, AddLocalPropertyRow, SavePropertyAsync, DeletePropertyAsync));
        tabs.TabPages.Add(ManagePage("事件卡", _eventGrid, AddLocalEventRow, SaveEventAsync, DeleteEventAsync));
        page.Controls.Add(tabs);
        return page;
    }

    private TabPage BuildRankTab()
    {
        var page = Page("排行榜与历史");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        buttons.Controls.AddRange([Button("刷新排行榜", () => SendAsync("GetRankList")), Button("刷新历史", () => SendAsync("GetHistory"))]);
        layout.Controls.Add(buttons, 0, 0);
        layout.SetColumnSpan(buttons, 2);
        layout.Controls.Add(Group("排行榜", _rankGrid), 0, 1);
        layout.Controls.Add(Group("历史对局", _historyGrid), 1, 1);
        page.Controls.Add(layout);
        return page;
    }

    private void WireEvents()
    {
        _btnRoll.Click += (_, _) => SendAsync("RollDice");
        _btnBuy.Click += (_, _) => SendAsync("BuyProperty");
        _btnEnd.Click += (_, _) => SendAsync("EndTurn");
        _chkSound.CheckedChanged += (_, _) => _sounds.Enabled = _chkSound.Checked;
        _roomTokenCombo.SelectedIndexChanged += (_, _) =>
        {
            if (!_updatingRoomTokenCombo && _currentRoomId.HasValue)
            {
                SelectRoomTokenAsync();
            }
        };
        _playerCards.Resize += (_, _) => ResizePlayerCards();
        _chatMessages.Resize += (_, _) => ResizeChatMessages();
        _txtChat.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendChatFromInput();
            }
        };
    }

    private async void ConnectAsync()
    {
        try
        {
            await _client.ConnectAsync(_txtIp.Text.Trim(), (int)_numPort.Value);
            SetStatus("已连接服务器", false);
        }
        catch (Exception ex)
        {
            SetStatus($"连接失败：{ex.Message}", true);
        }
    }

    private void RegisterAsync()
    {
        SendAsync("Register", BuildAuthRequest());
    }

    private void LoginAsync()
    {
        SendAsync("Login", BuildAuthRequest());
    }

    private AuthRequest BuildAuthRequest()
    {
        return new AuthRequest(_txtUser.Text.Trim(), _txtPassword.Text);
    }

    private void SelectRoomTokenAsync()
    {
        if (!_currentRoomId.HasValue || _roomTokenCombo.SelectedItem is not TokenOption token)
        {
            return;
        }

        SendAsync("SelectToken", new SelectTokenRequest(token.ImageFile));
    }

    private void CreateRoomAsync()
    {
        SendAsync("CreateRoom", new CreateRoomRequest(_txtRoomName.Text.Trim(), (int)_numMaxPlayers.Value));
    }

    private void JoinRoomAsync()
    {
        if (_roomGrid.CurrentRow?.DataBoundItem is RoomSummaryDto room)
        {
            SendAsync("JoinRoom", new JoinRoomRequest(room.RoomId));
        }
    }

    private void HandleMessage(NetMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case "Connected":
                    SetStatus(message.ReadData<BasicResult>().Message, false);
                    break;
                case "RegisterResult":
                case "CreateRoomResult":
                case "JoinRoomResult":
                case "ReadyResult":
                    ShowBasic(message.ReadData<BasicResult>());
                    break;
                case "BuyPropertyResult":
                    ShowBuyResult(message);
                    break;
                case "DiceResult":
                    ApplyDice(message.ReadData<DiceResultDto>());
                    break;
                case "LeaveRoomResult":
                    _lastGameStatus = "";
                    _currentRoomId = null;
                    _roomTokenCombo.Enabled = false;
                    _lblToken.Text = "棋子：进入房间后选择";
                    ClearChatMessages();
                    ShowBasic(message.ReadData<BasicResult>());
                    break;
                case "LoginResult":
                    ApplyLogin(message.ReadData<LoginResult>());
                    break;
                case "RoomListResult":
                    _roomGrid.DataSource = message.ReadData<RoomListResult>().Rooms;
                    break;
                case "GameState":
                    ApplyGameState(message.ReadData<GameStateDto>());
                    break;
                case "ManageDataResult":
                    ApplyManageData(message.ReadData<ManageDataDto>());
                    break;
                case "RankListResult":
                    _rankGrid.DataSource = message.ReadData<RankListResult>().Users;
                    break;
                case "HistoryResult":
                    _historyGrid.DataSource = message.ReadData<HistoryResult>().Records;
                    break;
                case "ChatMessage":
                    AddChatMessage(message.ReadData<ChatMessageDto>());
                    break;
                case "YourTurn":
                    SetStatus("轮到你行动", false);
                    break;
                case "GameStart":
                    _sounds.Play(GameSound.Success);
                    break;
                case "RentPaid":
                    _sounds.Play(GameSound.Cost);
                    break;
                case "ChanceResult":
                    _sounds.Play(GameSound.Success);
                    break;
                case "TaxResult":
                    _sounds.Play(GameSound.Cost);
                    break;
                case "PlayerBankrupt":
                    _sounds.Play(GameSound.Cost);
                    break;
                case "MoveResult":
                    ApplyMove(message.ReadData<MoveResultDto>());
                    break;
                case "GameOver":
                    var over = message.ReadData<GameOverDto>();
                    _lastGameStatus = "";
                    _sounds.Play(GameSound.GameOver);
                    MessageBox.Show(this, $"游戏结束，获胜者：{over.WinnerUserName}", "GameOver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "Error":
                    ShowBasic(message.ReadData<BasicResult>());
                    break;
            }
        }
        catch (Exception ex)
        {
            SetStatus($"消息处理失败：{ex.Message}", true);
        }
    }

    private void ApplyLogin(LoginResult result)
    {
        if (!result.Success)
        {
            SetStatus(result.Message, true);
            return;
        }

        _userId = result.UserId;
        _userName = result.UserName;
        _currentTokenImageFile = "棋子-红.png";
        _boardView.SetToken(_userId, _currentTokenImageFile);
        _lblUser.Text = $"当前玩家：{_userName} (ID {_userId})";
        _lblToken.Text = "棋子：进入房间后选择";
        SetStatus(result.Message, false);
        ShowMainShell();
        SendAsync("GetRoomList");
        SendAsync("GetManageData");
        SendAsync("GetRankList");
        SendAsync("GetHistory");
    }

    private void ApplyGameState(GameStateDto state)
    {
        if (state.Status == "Playing" && _lastGameStatus != "Playing")
        {
            _tabs.SelectedIndex = 1;
        }
        if (_currentRoomId != state.RoomId)
        {
            _currentRoomId = state.RoomId;
            ClearChatMessages();
        }
        _lastGameStatus = state.Status;

        _lblTurn.Text = state.Status == "Playing"
            ? $"房间 {state.RoomId} | 第 {state.RoundNumber} 回合 | 当前：{state.CurrentPlayerName}"
            : $"房间 {state.RoomId} | 状态：{state.Status}";

        _boardView.ApplyState(state);
        RefreshRoomTokenOptions(state);
        RenderPlayerCards(state);
        _propertyGrid.DataSource = BuildGamePropertyRows(state);
        _logText.Text = string.Join(Environment.NewLine, state.Logs);
        _logText.SelectionStart = _logText.TextLength;
        _logText.ScrollToCaret();
        var isMyTurn = state.Status == "Playing" && state.CurrentPlayerUserId == _userId;
        _lblActionHint.Text = state.Status != "Playing"
            ? "等待玩家准备"
            : isMyTurn
                ? state.CanBuyProperty ? "轮到你行动，可购买当前位置地产" : "轮到你行动"
                : $"等待 {state.CurrentPlayerName} 行动";
        SetGameButtons(isMyTurn, state.CanBuyProperty);
    }

    private void RefreshRoomTokenOptions(GameStateDto state)
    {
        var me = state.Players.FirstOrDefault(x => x.UserId == _userId);
        var myToken = me?.TokenImageFile ?? _currentTokenImageFile;
        var usedByOthers = state.Players
            .Where(x => x.UserId != _userId)
            .Select(x => x.TokenImageFile)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var options = AssetCatalog.TokenOptions
            .Where(x => string.Equals(x.ImageFile, myToken, StringComparison.OrdinalIgnoreCase)
                || !usedByOthers.Contains(x.ImageFile))
            .ToList();
        if (options.Count == 0)
        {
            options.Add(AssetCatalog.TokenOptions[0]);
        }

        _updatingRoomTokenCombo = true;
        _roomTokenCombo.DataSource = options;
        _roomTokenCombo.DisplayMember = nameof(TokenOption.DisplayName);
        _roomTokenCombo.ValueMember = nameof(TokenOption.ImageFile);
        _roomTokenCombo.SelectedItem = options.FirstOrDefault(x => string.Equals(x.ImageFile, myToken, StringComparison.OrdinalIgnoreCase))
            ?? options[0];
        _roomTokenCombo.Enabled = state.Status == "Waiting" && _currentRoomId.HasValue;
        _updatingRoomTokenCombo = false;

        var selectedToken = options.FirstOrDefault(x => string.Equals(x.ImageFile, myToken, StringComparison.OrdinalIgnoreCase));
        if (selectedToken is not null)
        {
            _currentTokenImageFile = selectedToken.ImageFile;
            _boardView.SetToken(_userId, selectedToken.ImageFile);
            _lblToken.Text = $"棋子：{selectedToken.DisplayName}";
        }
    }

    private static List<GamePropertyRow> BuildGamePropertyRows(GameStateDto state)
    {
        var cellsById = state.MapCells.ToDictionary(x => x.Id);
        return state.Properties
            .OrderBy(x => cellsById.TryGetValue(x.MapCellId, out var cell) ? cell.CellIndex : int.MaxValue)
            .ThenBy(x => x.PropertyName)
            .Select(x =>
            {
                cellsById.TryGetValue(x.MapCellId, out var cell);
                var position = cell is null ? "-" : cell.CellIndex.ToString();
                var owner = string.IsNullOrWhiteSpace(x.OwnerUserName) ? "未购" : x.OwnerUserName;
                return new GamePropertyRow(position, x.PropertyName, x.Price, x.Rent, owner);
            })
            .ToList();
    }

    private void RenderPlayerCards(GameStateDto state)
    {
        _playerCards.SuspendLayout();
        _playerCards.Controls.Clear();

        foreach (var player in state.Players.OrderBy(x => x.UserId))
        {
            _playerCards.Controls.Add(BuildPlayerCard(player, state));
        }

        _playerCards.ResumeLayout();
        ResizePlayerCards();
    }

    private Control BuildPlayerCard(PlayerStateDto player, GameStateDto state)
    {
        var isMe = player.UserId == _userId;
        var isCurrent = player.UserId == state.CurrentPlayerUserId;
        var borderColor = player.IsBankrupt
            ? Color.FromArgb(124, 99, 93)
            : isCurrent
                ? Color.FromArgb(212, 151, 38)
                : isMe
                    ? Color.FromArgb(33, 116, 84)
                    : Color.FromArgb(185, 163, 116);
        var card = new Panel
        {
            Height = 168,
            Width = Math.Max(280, _playerCards.ClientSize.Width - 26),
            Margin = new Padding(0, 0, 6, 8),
            Padding = new Padding(8),
            BackColor = player.IsBankrupt
                ? Color.FromArgb(224, 215, 205)
                : isMe
                    ? Color.FromArgb(224, 243, 228)
                    : Color.FromArgb(252, 248, 235)
        };
        card.Paint += (_, e) =>
        {
            using var pen = new Pen(borderColor, isCurrent || isMe ? 2 : 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(0, 255, 255, 255)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 62));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var token = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = AssetCatalog.GetImage(player.TokenImageFile)
                ?? AssetCatalog.GetImage(AssetCatalog.TokenOptions[player.UserId % AssetCatalog.TokenOptions.Length].ImageFile),
            Margin = new Padding(0, 6, 10, 6)
        };
        layout.Controls.Add(token, 0, 0);

        var main = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, BackColor = Color.Transparent };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 146));
        header.Controls.Add(new Label
        {
            Text = player.UserName,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(56, 36, 19)
        }, 0, 0);
        header.Controls.Add(new MoneyBillView
        {
            Dock = DockStyle.Fill,
            Money = player.Money,
            IsBankrupt = player.IsBankrupt,
            Margin = new Padding(8, 0, 0, 2)
        }, 1, 0);
        main.Controls.Add(header, 0, 0);

        main.Controls.Add(new Label
        {
            Text = $"位置 {player.Position} · {CellNameAt(state, player.Position)}",
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(92, 67, 38)
        }, 0, 1);

        main.Controls.Add(new Label
        {
            Text = player.SkipTurnRounds > 0
                ? $"地产 {player.OwnedProperties} 处 · 免租卡 {player.FreeRentCards} 张 · 休息中"
                : $"地产 {player.OwnedProperties} 处 · 免租卡 {player.FreeRentCards} 张",
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(92, 67, 38)
        }, 0, 2);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1, BackColor = Color.Transparent };
        footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var badges = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 2, 0, 0)
        };
        if (isCurrent)
        {
            badges.Controls.Add(Badge("当前", Color.FromArgb(212, 151, 38)));
        }
        if (isMe)
        {
            badges.Controls.Add(Badge("自己", Color.FromArgb(33, 116, 84)));
        }
        if (!player.IsReady && state.Status != "Playing")
        {
            badges.Controls.Add(Badge("未准备", Color.FromArgb(133, 91, 58)));
        }
        if (player.SkipTurnRounds > 0 && !player.IsBankrupt)
        {
            badges.Controls.Add(Badge("休息", Color.FromArgb(113, 92, 38)));
        }
        footer.Controls.Add(badges, 0, 0);
        main.Controls.Add(footer, 0, 3);

        layout.Controls.Add(main, 1, 0);

        card.Controls.Add(layout);
        return card;
    }

    private static Label Badge(string text, Color color)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Height = 22,
            Padding = new Padding(8, 2, 8, 2),
            Margin = new Padding(0, 3, 6, 3),
            BackColor = color,
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold)
        };
    }

    private static string CellNameAt(GameStateDto state, int position)
    {
        return state.MapCells.FirstOrDefault(x => x.CellIndex == position)?.CellName
            ?? AssetCatalog.DefaultBoard.FirstOrDefault(x => x.Index == position)?.Name
            ?? "-";
    }

    private void ResizePlayerCards()
    {
        var width = Math.Max(280, _playerCards.ClientSize.Width - 26);
        foreach (var card in _playerCards.Controls.OfType<Panel>())
        {
            card.Width = width;
        }
    }

    private void SendChatFromInput()
    {
        var text = _txtChat.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        SendAsync("SendChat", new ChatRequest(text));
        _txtChat.Clear();
    }

    private void SendQuickChat(string text)
    {
        SendAsync("SendChat", new ChatRequest(text));
    }

    private void SendReaction(string reactionType)
    {
        SendAsync("SendReaction", new ReactionRequest(reactionType));
    }

    private void AddChatMessage(ChatMessageDto message)
    {
        if (_currentRoomId.HasValue && message.RoomId != _currentRoomId.Value)
        {
            return;
        }

        _sounds.Play(message.MessageType == "Reaction" ? GameSound.Success : GameSound.Message);
        _chatMessages.SuspendLayout();
        while (_chatMessages.Controls.Count >= 60)
        {
            _chatMessages.Controls.RemoveAt(0);
        }

        var bubble = BuildChatBubble(message);
        _chatMessages.Controls.Add(bubble);
        _chatMessages.ResumeLayout();
        ResizeChatMessages();
        _chatMessages.ScrollControlIntoView(bubble);
    }

    private Control BuildChatBubble(ChatMessageDto message)
    {
        var isMe = message.SenderUserId == _userId;
        var width = ChatBubbleWidth();
        var displayText = message.MessageType == "Reaction" ? $"互动：{message.Text}" : message.Text;
        var panel = new Panel
        {
            Width = width,
            Height = ChatBubbleHeight(displayText, width),
            Margin = new Padding(0, 0, 6, 6),
            Padding = new Padding(7),
            Tag = displayText,
            BackColor = message.MessageType == "Reaction"
                ? Color.FromArgb(255, 244, 214)
                : isMe
                    ? Color.FromArgb(226, 244, 231)
                    : Color.FromArgb(255, 252, 242)
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(isMe ? Color.FromArgb(33, 116, 84) : Color.FromArgb(185, 163, 116), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 38));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = AssetCatalog.GetImage(message.TokenImageFile),
            Margin = new Padding(0, 2, 7, 2)
        }, 0, 0);

        var textLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, BackColor = Color.Transparent };
        textLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        textLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        textLayout.Controls.Add(new Label
        {
            Text = $"{message.SenderUserName}  {message.SentAt:HH:mm}",
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(77, 50, 25)
        }, 0, 0);
        textLayout.Controls.Add(new Label
        {
            Text = displayText,
            Dock = DockStyle.Fill,
            AutoEllipsis = false,
            AutoSize = false,
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular),
            Padding = new Padding(0, 2, 0, 0),
            TextAlign = ContentAlignment.TopLeft,
            ForeColor = Color.FromArgb(55, 38, 22)
        }, 0, 1);
        layout.Controls.Add(textLayout, 1, 0);

        panel.Controls.Add(layout);
        return panel;
    }

    private void ClearChatMessages()
    {
        _chatMessages.Controls.Clear();
    }

    private void ResizeChatMessages()
    {
        var width = ChatBubbleWidth();
        foreach (var bubble in _chatMessages.Controls.OfType<Panel>())
        {
            bubble.Width = width;
            if (bubble.Tag is string text)
            {
                bubble.Height = ChatBubbleHeight(text, width);
            }
        }
    }

    private int ChatBubbleWidth()
    {
        return Math.Max(220, _chatMessages.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 16);
    }

    private static int ChatBubbleHeight(string text, int bubbleWidth)
    {
        const int verticalChrome = 48;
        var textWidth = Math.Max(120, bubbleWidth - 58 - 18);
        using var font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);
        var measured = TextRenderer.MeasureText(
            text,
            font,
            new Size(textWidth, 0),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
        return Math.Clamp(verticalChrome + measured.Height, 72, 136);
    }

    private void ApplyManageData(ManageDataDto data)
    {
        _mapRows = new BindingList<MapCellRow>(data.MapCells.Select(x => new MapCellRow(x)).ToList());
        _propertyRows = new BindingList<PropertyRow>(data.Properties.Select(x => new PropertyRow(x)).ToList());
        _eventRows = new BindingList<EventCardRow>(data.EventCards.Select(x => new EventCardRow(x)).ToList());
        _mapGrid.DataSource = _mapRows;
        _managePropertyGrid.DataSource = _propertyRows;
        _eventGrid.DataSource = _eventRows;
    }

    private void ApplyDice(DiceResultDto dice)
    {
        _boardView.ShowDice(dice);
        _dicePicture.Image = AssetCatalog.GetImage($"{dice.Dice}.png");
        _sounds.Play(GameSound.Dice);
        SetStatus($"{dice.UserName} 掷出 {dice.Dice} 点", false);
    }

    private void ApplyMove(MoveResultDto move)
    {
        _boardView.ShowMove(move);
        _sounds.Play(GameSound.Move);
    }

    private void ShowBuyResult(NetMessage message)
    {
        try
        {
            var buy = message.ReadData<BuyPropertyResultDto>();
            _sounds.Play(buy.Success ? GameSound.Success : GameSound.Cost);
            SetStatus(buy.Message, !buy.Success);
        }
        catch
        {
            ShowBasic(message.ReadData<BasicResult>());
        }
    }

    private void ShowBasic(BasicResult result)
    {
        SetStatus(result.Message, !result.Success);
    }

    private void ShowMainShell()
    {
        _loginPage.Visible = false;
        _mainShell.Visible = true;
        _mainShell.BringToFront();
    }

    private void ShowLoginPage()
    {
        _mainShell.Visible = false;
        _loginPage.Visible = true;
        _loginPage.BringToFront();
    }

    private void SetGameButtons(bool isMyTurn, bool canBuy = false)
    {
        _btnRoll.Enabled = isMyTurn;
        _btnBuy.Enabled = isMyTurn && canBuy;
        _btnEnd.Enabled = isMyTurn;
    }

    private void AddLocalMapRow()
    {
        var nextIndex = _mapRows.Count == 0 ? 0 : _mapRows.Max(x => x.CellIndex) + 1;
        _mapRows.Add(new MapCellRow { CellIndex = nextIndex, CellName = "新格子", CellType = "Empty", Description = "" });
    }

    private void AddLocalPropertyRow()
    {
        _propertyRows.Add(new PropertyRow { PropertyName = "新地产", Price = 200, Rent = 50, ColorGroup = "默认" });
    }

    private void AddLocalEventRow()
    {
        _eventRows.Add(new EventCardRow { EventName = "新事件", EventType = "AddMoney", Value = 100, Description = "获得100金币", IsEnabled = true });
    }

    private void SaveMapAsync()
    {
        _mapGrid.EndEdit();
        if (_mapGrid.CurrentRow?.DataBoundItem is not MapCellRow row) return;
        var dto = new MapCellEditDto(row.Id, row.CellIndex, row.CellName, row.CellType, row.Description);
        SendAsync(row.Id == 0 ? "AddMapCell" : "UpdateMapCell", dto);
    }

    private void DeleteMapAsync()
    {
        if (_mapGrid.CurrentRow?.DataBoundItem is MapCellRow row && row.Id > 0)
        {
            SendAsync("DeleteMapCell", new EntityIdRequest(row.Id));
        }
    }

    private void SavePropertyAsync()
    {
        _managePropertyGrid.EndEdit();
        if (_managePropertyGrid.CurrentRow?.DataBoundItem is not PropertyRow row) return;
        var dto = new PropertyEditDto(row.Id, row.MapCellId, row.PropertyName, row.Price, row.Rent, row.ColorGroup);
        SendAsync(row.Id == 0 ? "AddProperty" : "UpdateProperty", dto);
    }

    private void DeletePropertyAsync()
    {
        if (_managePropertyGrid.CurrentRow?.DataBoundItem is PropertyRow row && row.Id > 0)
        {
            SendAsync("DeleteProperty", new EntityIdRequest(row.Id));
        }
    }

    private void SaveEventAsync()
    {
        _eventGrid.EndEdit();
        if (_eventGrid.CurrentRow?.DataBoundItem is not EventCardRow row) return;
        var dto = new EventCardEditDto(row.Id, row.EventName, row.EventType, row.Value, row.Description, row.IsEnabled);
        SendAsync(row.Id == 0 ? "AddEventCard" : "UpdateEventCard", dto);
    }

    private void DeleteEventAsync()
    {
        if (_eventGrid.CurrentRow?.DataBoundItem is EventCardRow row && row.Id > 0)
        {
            SendAsync("DeleteEventCard", new EntityIdRequest(row.Id));
        }
    }

    private async void SendAsync<T>(string type, T data)
    {
        try
        {
            await _client.SendAsync(type, data);
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, true);
        }
    }

    private async void SendAsync(string type)
    {
        try
        {
            await _client.SendAsync(type);
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, true);
        }
    }

    private void SetStatus(string text, bool error)
    {
        _lblStatus.Text = text;
        _lblStatus.ForeColor = error ? Color.FromArgb(173, 35, 38) : Color.FromArgb(25, 99, 66);
        _lblLoginStatus.Text = text;
        _lblLoginStatus.ForeColor = _lblStatus.ForeColor;
    }

    private static TabPage Page(string title)
    {
        return new TabPage(title) { BackColor = Color.FromArgb(238, 224, 184), Padding = new Padding(10) };
    }

    private static SplitContainer Split(float leftRatio = 0.74f)
    {
        const int minLeftWidth = 520;
        const int minRightWidth = 260;
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel2,
            BackColor = Color.Transparent
        };
        split.SizeChanged += (_, _) =>
        {
            var width = split.ClientSize.Width;
            if (width <= minLeftWidth + minRightWidth)
            {
                return;
            }

            split.SplitterDistance = Math.Clamp(
                (int)Math.Round(width * leftRatio),
                minLeftWidth,
                Math.Max(minLeftWidth, width - minRightWidth));
        };
        return split;
    }

    private static GroupBox Group(string title, Control content)
    {
        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            BackColor = Color.FromArgb(245, 236, 207),
            ForeColor = Color.FromArgb(72, 45, 25)
        };
        content.Dock = DockStyle.Fill;
        group.Controls.Add(content);
        return group;
    }

    private void ConfigurePropertyGrid()
    {
        _propertyGrid.AutoGenerateColumns = false;
        _propertyGrid.Columns.Clear();
        _propertyGrid.RowTemplate.Height = 38;
        _propertyGrid.ColumnHeadersHeight = 40;
        _propertyGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        _propertyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _propertyGrid.Columns.Add(GridColumn(nameof(GamePropertyRow.Position), "格", 44));
        _propertyGrid.Columns.Add(GridColumn(nameof(GamePropertyRow.Name), "地产", 128, true));
        _propertyGrid.Columns.Add(GridColumn(nameof(GamePropertyRow.Price), "价", 58));
        _propertyGrid.Columns.Add(GridColumn(nameof(GamePropertyRow.Rent), "租", 58));
        _propertyGrid.Columns.Add(GridColumn(nameof(GamePropertyRow.Owner), "归属", 92, true));
    }

    private void ConfigureRoomGrid()
    {
        _roomGrid.AutoGenerateColumns = false;
        _roomGrid.Columns.Clear();
        _roomGrid.RowTemplate.Height = 42;
        _roomGrid.ColumnHeadersHeight = 42;
        _roomGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.RoomId), "房间", 74));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.RoomName), "名称", 180, true));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.OwnerUserName), "房主", 104));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.PlayerCount), "人数", 74));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.MaxPlayers), "上限", 74));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.ReadyCount), "准备", 74));
        _roomGrid.Columns.Add(GridColumn(nameof(RoomSummaryDto.Status), "状态", 90));
        foreach (DataGridViewColumn column in _roomGrid.Columns)
        {
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.FillWeight = column.DataPropertyName == nameof(RoomSummaryDto.RoomName) ? 210 : column.Width;
            column.MinimumWidth = column.Width;
        }
    }

    private static DataGridViewTextBoxColumn GridColumn(string propertyName, string header, int width, bool fill = false)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = header,
            Width = width,
            MinimumWidth = header.Length <= 1 ? 40 : 64,
            AutoSizeMode = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };
    }

    private static TabPage ManagePage(string title, DataGridView grid, Action add, Action save, Action delete)
    {
        var page = new TabPage(title) { Padding = new Padding(8), BackColor = Color.FromArgb(238, 224, 184) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        toolbar.Controls.AddRange([Button("刷新", () => ActiveFormSend("GetManageData")), Button("新增行", add), Button("保存选中", save), Button("删除数据库记录", delete)]);
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(grid, 0, 1);
        page.Controls.Add(layout);
        return page;

        static void ActiveFormSend(string type)
        {
            if (Application.OpenForms.OfType<MainForm>().FirstOrDefault() is { } form)
            {
                form.SendAsync(type);
            }
        }
    }

    private static Button Button(string text, Action click)
    {
        using var buttonFont = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        var measured = TextRenderer.MeasureText(text, buttonFont);
        var button = new SolidTextButton
        {
            Text = text,
            AutoSize = false,
            Width = Math.Max(108, measured.Width + 30),
            Height = 54,
            MinimumSize = new Size(92, 54),
            Margin = new Padding(7, 6, 7, 6),
            Padding = new Padding(8, 4, 8, 8),
            BackColor = Color.FromArgb(125, 51, 27),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = false
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => click();
        return button;
    }

    private static void StyleCommandButton(Button button)
    {
        button.AutoSize = false;
        button.Dock = DockStyle.Fill;
        button.Margin = new Padding(4, 8, 4, 8);
        button.Height = 52;
        button.MinimumSize = new Size(96, 46);
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Color.FromArgb(125, 51, 27);
        button.ForeColor = Color.White;
        button.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
    }

    private static Button SmallButton(string text, Action click)
    {
        var button = Button(text, click);
        button.AutoSize = false;
        button.Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold);
        button.Width = Math.Max(56, TextRenderer.MeasureText(text, button.Font).Width + 22);
        button.Height = 38;
        button.MinimumSize = new Size(52, 36);
        button.Margin = new Padding(2, 4, 2, 2);
        button.Padding = new Padding(4, 0, 4, 0);
        button.TextAlign = ContentAlignment.MiddleCenter;
        return button;
    }


    private static Control Spacer(int width)
    {
        return new Panel { Width = width, Height = 1 };
    }

    private static DataGridView Grid(bool editable = false)
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = !editable,
            BackgroundColor = Color.FromArgb(252, 248, 235),
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(81, 60, 34),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 4, 0)
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(252, 248, 235),
                SelectionBackColor = Color.FromArgb(171, 116, 52),
                SelectionForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Regular),
                Padding = new Padding(8, 0, 4, 0)
            },
            GridColor = Color.FromArgb(224, 210, 171)
        };
    }

    public class MapCellRow
    {
        public MapCellRow()
        {
        }

        public MapCellRow(MapCellDto dto)
        {
            Id = dto.Id;
            CellIndex = dto.CellIndex;
            CellName = dto.CellName;
            CellType = dto.CellType;
            Description = dto.Description;
        }

        public int Id { get; set; }
        public int CellIndex { get; set; }
        public string CellName { get; set; } = string.Empty;
        public string CellType { get; set; } = "Empty";
        public string Description { get; set; } = string.Empty;
    }

    public class PropertyRow
    {
        public PropertyRow()
        {
        }

        public PropertyRow(PropertyDto dto)
        {
            Id = dto.Id;
            MapCellId = dto.MapCellId;
            CellIndex = dto.CellIndex;
            PropertyName = dto.PropertyName;
            Price = dto.Price;
            Rent = dto.Rent;
            ColorGroup = dto.ColorGroup;
        }

        public int Id { get; set; }
        public int MapCellId { get; set; }
        public int CellIndex { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Rent { get; set; }
        public string ColorGroup { get; set; } = string.Empty;
    }

    private sealed record GamePropertyRow(string Position, string Name, int Price, int Rent, string Owner);

    public class EventCardRow
    {
        public EventCardRow()
        {
        }

        public EventCardRow(EventCardDto dto)
        {
            Id = dto.Id;
            EventName = dto.EventName;
            EventType = dto.EventType;
            Value = dto.Value;
            Description = dto.Description;
            IsEnabled = dto.IsEnabled;
        }

        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }
}

internal sealed class TranslucentPanel : Panel
{
    public Color FillColor { get; set; } = Color.FromArgb(180, Color.White);

    public Color BorderColor { get; set; } = Color.FromArgb(100, Color.Black);

    public TranslucentPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        using var fill = new SolidBrush(FillColor);
        using var border = new Pen(BorderColor, 1);
        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;
        e.Graphics.FillRectangle(fill, rect);
        e.Graphics.DrawRectangle(border, rect);
        base.OnPaint(e);
    }
}

internal sealed class SolidTextButton : Button
{
    public SolidTextButton()
    {
        SetStyle(ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var backColor = Enabled
            ? BackColor
            : Color.FromArgb(154, 120, 101);
        if (ClientRectangle.Contains(PointToClient(Cursor.Position)) && Enabled)
        {
            backColor = ControlPaint.Light(backColor, 0.12f);
        }

        using var background = new SolidBrush(backColor);
        e.Graphics.FillRectangle(background, ClientRectangle);

        var textRect = ClientRectangle;
        textRect.Inflate(-Padding.Horizontal / 2, -Padding.Vertical / 2);
        textRect.Offset(0, -1);
        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            textRect,
            Enabled ? ForeColor : Color.FromArgb(235, 224, 214),
            TextFormatFlags.HorizontalCenter
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine
                | TextFormatFlags.NoPrefix
                | TextFormatFlags.NoClipping);
    }
}
