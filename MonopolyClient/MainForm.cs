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
    private readonly Panel _loginCard = new();
    private readonly TextBox _txtIp = new() { Text = "127.0.0.1", Width = 190 };
    private readonly NumericUpDown _numPort = new() { Minimum = 1, Maximum = 65535, Value = 9000, Width = 95 };
    private readonly TextBox _txtUser = new() { Text = "player1", Width = 260 };
    private readonly TextBox _txtPassword = new() { Text = "123456", Width = 260, UseSystemPasswordChar = true };
    private readonly ComboBox _tokenCombo = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _lblLoginStatus = new() { AutoSize = false, Height = 28, Text = "请先连接服务器", TextAlign = ContentAlignment.MiddleCenter };
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
    private readonly DataGridView _propertyGrid = Grid();
    private readonly ListBox _logList = new() { Dock = DockStyle.Fill, IntegralHeight = false };
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
        _tokenCombo.DataSource = AssetCatalog.TokenOptions.ToList();
        _tokenCombo.DisplayMember = nameof(TokenOption.DisplayName);

        Controls.Add(_mainShell);
        Controls.Add(_loginPage);
        BuildLoginPage();
        BuildMainShell();
        _loginPage.BringToFront();
    }

    private void BuildLoginPage()
    {
        _loginPage.BackColor = Color.FromArgb(36, 45, 33);
        _loginPage.BackgroundImage = AssetCatalog.GetImage("登录界面.png");
        _loginPage.BackgroundImageLayout = ImageLayout.Stretch;
        _loginPage.Resize += (_, _) => CenterLoginCard();

        _loginCard.Size = new Size(520, 430);
        _loginCard.BackColor = Color.FromArgb(242, 219, 165);
        _loginCard.Padding = new Padding(26, 24, 26, 20);
        _loginCard.Paint += (_, e) =>
        {
            using var border = new Pen(Color.FromArgb(146, 95, 41), 2);
            e.Graphics.DrawRectangle(border, 1, 1, _loginCard.Width - 3, _loginCard.Height - 3);
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 8,
            BackColor = Color.Transparent
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        for (var i = 1; i < 7; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        }
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Text = "玩家通行证",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(91, 45, 19)
        };
        layout.Controls.Add(title, 0, 0);
        layout.SetColumnSpan(title, 2);

        AddFormRow(layout, 1, "服务器", BuildServerRow());
        AddFormRow(layout, 2, "玩家名 / ID", _txtUser);
        AddFormRow(layout, 3, "通行码", _txtPassword);
        AddFormRow(layout, 4, "棋子", _tokenCombo);
        AddFormRow(layout, 5, "房间名", _txtRoomName);
        AddFormRow(layout, 6, "人数", _numMaxPlayers);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };
        buttons.Controls.AddRange([
            Button("连接", ConnectAsync),
            Button("注册", RegisterAsync),
            Button("登录进入大厅", LoginAsync)
        ]);

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bottom.Controls.Add(buttons, 0, 0);
        bottom.Controls.Add(_lblLoginStatus, 0, 1);
        layout.Controls.Add(bottom, 0, 7);
        layout.SetColumnSpan(bottom, 2);

        _loginCard.Controls.Add(layout);
        _loginPage.Controls.Add(_loginCard);
        CenterLoginCard();
    }

    private Control BuildServerRow()
    {
        var row = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Margin = Padding.Empty };
        row.Controls.Add(_txtIp);
        row.Controls.Add(_numPort);
        return row;
    }

    private static void AddFormRow(TableLayoutPanel layout, int row, string label, Control control)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(89, 50, 25)
        }, 0, row);
        control.Margin = new Padding(0, 6, 0, 0);
        layout.Controls.Add(control, 1, row);
    }

    private void CenterLoginCard()
    {
        _loginCard.Left = Math.Max(24, (_loginPage.Width - _loginCard.Width) / 2);
        _loginCard.Top = Math.Max(150, (_loginPage.Height - _loginCard.Height) / 2 + 12);
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _mainShell.Controls.Add(root);

        var header = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.FromArgb(238, 224, 184)
        };
        header.Controls.AddRange([
            _lblUser,
            Spacer(16),
            _lblToken,
            Spacer(16),
            Button("刷新房间", () => SendAsync("GetRoomList")),
            Button("回到登录页", ShowLoginPage),
            Spacer(18),
            _lblStatus
        ]);
        root.Controls.Add(header, 0, 0);

        _tabs.TabPages.Add(BuildLobbyTab());
        _tabs.TabPages.Add(BuildGameTab());
        _tabs.TabPages.Add(BuildManageTab());
        _tabs.TabPages.Add(BuildRankTab());
        root.Controls.Add(_tabs, 0, 1);
    }

    private TabPage BuildLobbyTab()
    {
        var page = Page("房间大厅");
        var layout = Split();
        page.Controls.Add(layout);

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        toolbar.Controls.AddRange([
            Button("创建房间", CreateRoomAsync),
            Button("加入选中", JoinRoomAsync),
            Button("离开房间", () => SendAsync("LeaveRoom")),
            Button("准备", () => SendAsync("Ready"))
        ]);
        left.Controls.Add(toolbar, 0, 0);
        left.Controls.Add(_roomGrid, 0, 1);
        layout.Panel1.Controls.Add(left);

        var preview = new Panel
        {
            Dock = DockStyle.Fill,
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        page.Controls.Add(layout);

        var boardHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(29, 58, 44)
        };
        boardHost.Controls.Add(_boardView);
        layout.Controls.Add(boardHost, 0, 0);

        var side = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, Padding = new Padding(8) };
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 32));

        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        controls.Controls.AddRange([_dicePicture, _btnRoll, _btnBuy, _btnEnd]);
        side.Controls.Add(BuildTurnStatusPanel(), 0, 0);
        side.Controls.Add(controls, 0, 1);
        side.Controls.Add(Group("玩家状态", _playerCards), 0, 2);
        side.Controls.Add(BuildChatPanel(), 0, 3);
        side.Controls.Add(Group("详情", BuildDetailTabs()), 0, 4);
        layout.Controls.Add(side, 1, 0);
        return page;
    }

    private Control BuildTurnStatusPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            BackColor = Color.FromArgb(245, 236, 207),
            Padding = new Padding(8, 4, 8, 4)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        panel.Controls.Add(_lblTurn, 0, 0);
        panel.Controls.Add(_lblActionHint, 0, 1);
        return panel;
    }

    private Control BuildChatPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            BackColor = Color.FromArgb(245, 236, 207),
            Padding = new Padding(2)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var quick = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent
        };
        foreach (var text in new[] { "我等到花儿也谢了", "不要走，决战到天亮", "快点吧", "这把稳了" })
        {
            quick.Controls.Add(SmallButton(text, () => SendQuickChat(text)));
        }

        var input = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        input.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68));
        input.Controls.Add(_txtChat, 0, 0);
        input.Controls.Add(SmallButton("发送", SendChatFromInput), 1, 0);

        var tools = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        tools.Controls.Add(SmallButton("扔鸡蛋", () => SendReaction("Egg")));
        tools.Controls.Add(SmallButton("送鲜花", () => SendReaction("Flower")));
        tools.Controls.Add(SmallButton("喝彩", () => SendReaction("Cheer")));
        tools.Controls.Add(new Button
        {
            Text = "语音待实现",
            Enabled = false,
            AutoSize = true,
            Height = 28,
            Margin = new Padding(4, 3, 4, 3)
        });

        layout.Controls.Add(_chatMessages, 0, 0);
        layout.Controls.Add(quick, 0, 1);
        layout.Controls.Add(input, 0, 2);
        layout.Controls.Add(tools, 0, 3);
        return Group("聊天互动", layout);
    }

    private Control BuildDetailTabs()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var propertyPage = new TabPage("地产") { BackColor = Color.FromArgb(245, 236, 207), Padding = new Padding(4) };
        var logPage = new TabPage("日志") { BackColor = Color.FromArgb(245, 236, 207), Padding = new Padding(4) };
        propertyPage.Controls.Add(_propertyGrid);
        logPage.Controls.Add(_logList);
        tabs.TabPages.Add(propertyPage);
        tabs.TabPages.Add(logPage);
        return tabs;
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
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
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
        return new AuthRequest(_txtUser.Text.Trim(), _txtPassword.Text, SelectedToken().ImageFile);
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
                case "RentPaid":
                case "ChanceResult":
                case "TaxResult":
                case "PlayerBankrupt":
                    break;
                case "MoveResult":
                    ApplyMove(message.ReadData<MoveResultDto>());
                    break;
                case "GameOver":
                    var over = message.ReadData<GameOverDto>();
                    _lastGameStatus = "";
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
        var token = SelectedToken();
        _boardView.SetToken(_userId, token.ImageFile);
        _lblUser.Text = $"当前玩家：{_userName} (ID {_userId})";
        _lblToken.Text = $"棋子：{token.DisplayName}";
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
        RenderPlayerCards(state);
        _propertyGrid.DataSource = state.Properties;
        _logList.DataSource = state.Logs.ToList();
        var isMyTurn = state.Status == "Playing" && state.CurrentPlayerUserId == _userId;
        _lblActionHint.Text = state.Status != "Playing"
            ? "等待玩家准备"
            : isMyTurn
                ? state.CanBuyProperty ? "轮到你行动，可购买当前位置地产" : "轮到你行动"
                : $"等待 {state.CurrentPlayerName} 行动";
        SetGameButtons(isMyTurn, state.CanBuyProperty);
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
            Height = 92,
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
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Color.Transparent
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));

        var token = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = AssetCatalog.GetImage(player.TokenImageFile)
                ?? AssetCatalog.GetImage(AssetCatalog.TokenOptions[player.UserId % AssetCatalog.TokenOptions.Length].ImageFile),
            Margin = new Padding(0, 4, 8, 4)
        };
        layout.Controls.Add(token, 0, 0);

        var main = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, BackColor = Color.Transparent };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        main.Controls.Add(new Label
        {
            Text = player.UserName,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(56, 36, 19)
        }, 0, 0);
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
            Text = $"地产 {player.OwnedProperties} 处 · 免租卡 {player.FreeRentCards} 张",
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(92, 67, 38)
        }, 0, 2);
        layout.Controls.Add(main, 1, 0);

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, BackColor = Color.Transparent };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.Controls.Add(new MoneyBillView
        {
            Dock = DockStyle.Fill,
            Money = player.Money,
            IsBankrupt = player.IsBankrupt,
            Margin = new Padding(0, 0, 0, 2)
        }, 0, 0);

        var badges = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.Transparent
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
        right.Controls.Add(badges, 0, 1);
        right.Controls.Add(new Label
        {
            Text = player.IsReady ? "已准备" : "等待中",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopRight,
            ForeColor = Color.FromArgb(92, 67, 38)
        }, 0, 2);
        layout.Controls.Add(right, 2, 0);

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
            Padding = new Padding(7, 3, 7, 3),
            Margin = new Padding(4, 2, 0, 2),
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
        var panel = new Panel
        {
            Height = message.Text.Length > 28 ? 74 : 60,
            Width = Math.Max(260, _chatMessages.ClientSize.Width - 28),
            Margin = new Padding(0, 0, 6, 6),
            Padding = new Padding(7),
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
        textLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 21));
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
            Text = message.MessageType == "Reaction" ? $"互动：{message.Text}" : message.Text,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
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
        var width = Math.Max(260, _chatMessages.ClientSize.Width - 28);
        foreach (var bubble in _chatMessages.Controls.OfType<Panel>())
        {
            bubble.Width = width;
        }
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
        SetStatus($"{dice.UserName} 掷出 {dice.Dice} 点", false);
    }

    private void ApplyMove(MoveResultDto move)
    {
        _boardView.ShowMove(move);
    }

    private void ShowBuyResult(NetMessage message)
    {
        try
        {
            var buy = message.ReadData<BuyPropertyResultDto>();
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

    private TokenOption SelectedToken()
    {
        return _tokenCombo.SelectedItem as TokenOption ?? AssetCatalog.TokenOptions[0];
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

    private static SplitContainer Split()
    {
        return new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 760,
            BackColor = Color.Transparent
        };
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

    private static TabPage ManagePage(string title, DataGridView grid, Action add, Action save, Action delete)
    {
        var page = new TabPage(title) { Padding = new Padding(8), BackColor = Color.FromArgb(238, 224, 184) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
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
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Height = 32,
            Margin = new Padding(5, 5, 5, 5),
            BackColor = Color.FromArgb(125, 51, 27),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold)
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => click();
        return button;
    }

    private static Button SmallButton(string text, Action click)
    {
        var button = Button(text, click);
        button.Height = 28;
        button.Margin = new Padding(4, 3, 4, 3);
        button.Font = new Font("Microsoft YaHei UI", 8F, FontStyle.Bold);
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
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold)
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(252, 248, 235),
                SelectionBackColor = Color.FromArgb(171, 116, 52),
                SelectionForeColor = Color.White
            }
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
