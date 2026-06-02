using System.ComponentModel;
using MonopolyClient.Networking;
using MonopolyModels.Dtos;

namespace MonopolyClient;

public class MainForm : Form
{
    private readonly GameTcpClient _client = new();
    private readonly TextBox _txtIp = new() { Text = "127.0.0.1", Width = 110 };
    private readonly NumericUpDown _numPort = new() { Minimum = 1, Maximum = 65535, Value = 9000, Width = 80 };
    private readonly TextBox _txtUser = new() { Text = "player1", Width = 120 };
    private readonly TextBox _txtPassword = new() { Text = "123456", Width = 120, UseSystemPasswordChar = true };
    private readonly Label _lblStatus = new() { AutoSize = true, Text = "未连接" };
    private readonly DataGridView _roomGrid = Grid();
    private readonly TextBox _txtRoomName = new() { Text = "课程设计房间", Width = 180 };
    private readonly NumericUpDown _numMaxPlayers = new() { Minimum = 2, Maximum = 4, Value = 2, Width = 60 };
    private readonly Button[] _cellButtons = new Button[20];
    private readonly DataGridView _playerGrid = Grid();
    private readonly DataGridView _propertyGrid = Grid();
    private readonly ListBox _logList = new() { Dock = DockStyle.Fill, IntegralHeight = false };
    private readonly Label _lblTurn = new() { AutoSize = true, Text = "当前回合：-" };
    private readonly Label _lblUser = new() { AutoSize = true, Text = "未登录" };
    private readonly Button _btnRoll = new() { Text = "掷骰子", Width = 90 };
    private readonly Button _btnBuy = new() { Text = "购买地产", Width = 90 };
    private readonly Button _btnEnd = new() { Text = "结束回合", Width = 90 };
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

    public MainForm()
    {
        Text = "多人回合制地产经营游戏系统";
        MinimumSize = new Size(1180, 760);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft YaHei UI", 9F);
        BackColor = Color.FromArgb(244, 246, 248);

        _client.MessageReceived += message => BeginInvoke(() => HandleMessage(message));
        _client.Disconnected += reason => BeginInvoke(() => SetStatus($"连接断开：{reason}", true));

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
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(14),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var header = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        header.Controls.AddRange([
            Label("服务器"), _txtIp, Label("端口"), _numPort, Button("连接", ConnectAsync),
            Label("用户名"), _txtUser, Label("密码"), _txtPassword, Button("注册", RegisterAsync), Button("登录", LoginAsync),
            Spacer(20), _lblUser, Spacer(14), _lblStatus
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
        var page = Page("大厅");
        var layout = Split();
        page.Controls.Add(layout);

        var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        toolbar.Controls.AddRange([
            Button("刷新房间", () => SendAsync("GetRoomList")),
            Label("房间名"), _txtRoomName,
            Label("人数"), _numMaxPlayers,
            Button("创建房间", CreateRoomAsync),
            Button("加入选中", JoinRoomAsync),
            Button("离开房间", () => SendAsync("LeaveRoom")),
            Button("准备", () => SendAsync("Ready"))
        ]);
        left.Controls.Add(toolbar, 0, 0);
        left.Controls.Add(_roomGrid, 0, 1);
        layout.Panel1.Controls.Add(left);

        var help = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(250, 251, 252),
            Text = "演示流程\r\n\r\n1. 启动 MonopolyServer。\r\n2. 打开两个客户端，使用不同用户名登录。\r\n3. 一个客户端创建房间，另一个加入。\r\n4. 双方点击准备后自动开局。\r\n5. 轮到自己时掷骰、购买、结束回合。\r\n\r\n该客户端通过 TcpClient + NetworkStream 与服务器通信；数据库表数据通过 DataGridView 管理。"
        };
        layout.Panel2.Controls.Add(help);
        return page;
    }

    private TabPage BuildGameTab()
    {
        var page = Page("游戏");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        page.Controls.Add(layout);

        var boardPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 4, Padding = new Padding(4) };
        for (var i = 0; i < 5; i++) boardPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        for (var i = 0; i < 4; i++) boardPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        for (var i = 0; i < _cellButtons.Length; i++)
        {
            var button = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(Font.FontFamily, 9F, FontStyle.Bold),
                Enabled = false
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(199, 205, 214);
            _cellButtons[i] = button;
            boardPanel.Controls.Add(button, i % 5, i / 5);
        }
        layout.Controls.Add(boardPanel, 0, 0);

        var side = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 6, Padding = new Padding(8) };
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        side.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        side.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        controls.Controls.AddRange([_btnRoll, _btnBuy, _btnEnd]);
        side.Controls.Add(_lblTurn, 0, 0);
        side.Controls.Add(controls, 0, 1);
        side.Controls.Add(Group("玩家状态", _playerGrid), 0, 2);
        side.Controls.Add(Group("地产状态", _propertyGrid), 0, 3);
        side.Controls.Add(Group("游戏日志", _logList), 0, 4);
        side.Controls.Add(new Panel(), 0, 5);
        layout.Controls.Add(side, 1, 0);
        return page;
    }

    private TabPage BuildManageTab()
    {
        var page = Page("数据管理");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(ManagePage("地图格子", _mapGrid, () => AddLocalMapRow(), SaveMapAsync, DeleteMapAsync));
        tabs.TabPages.Add(ManagePage("地产配置", _managePropertyGrid, () => AddLocalPropertyRow(), SavePropertyAsync, DeletePropertyAsync));
        tabs.TabPages.Add(ManagePage("事件卡", _eventGrid, () => AddLocalEventRow(), SaveEventAsync, DeleteEventAsync));
        page.Controls.Add(tabs);
        return page;
    }

    private TabPage BuildRankTab()
    {
        var page = Page("排行榜与历史");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
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
        SendAsync("Register", new AuthRequest(_txtUser.Text.Trim(), _txtPassword.Text));
    }

    private void LoginAsync()
    {
        SendAsync("Login", new AuthRequest(_txtUser.Text.Trim(), _txtPassword.Text));
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
                case "LeaveRoomResult":
                case "ReadyResult":
                case "BuyPropertyResult":
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
                case "YourTurn":
                    SetStatus("轮到你行动", false);
                    break;
                case "GameOver":
                    var over = message.ReadData<GameOverDto>();
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
        _lblUser.Text = $"当前用户：{_userName} (ID {_userId})";
        SetStatus(result.Message, false);
        SendAsync("GetRoomList");
        SendAsync("GetManageData");
        SendAsync("GetRankList");
        SendAsync("GetHistory");
    }

    private void ApplyGameState(GameStateDto state)
    {
        _lblTurn.Text = state.Status == "Playing"
            ? $"房间 {state.RoomId} | 第 {state.RoundNumber} 回合 | 当前：{state.CurrentPlayerName}"
            : $"房间 {state.RoomId} | 状态：{state.Status}";

        for (var i = 0; i < _cellButtons.Length; i++)
        {
            var cell = state.MapCells.FirstOrDefault(x => x.CellIndex == i);
            var players = state.Players.Where(x => x.Position == i && !x.IsBankrupt).Select(x => x.UserName).ToList();
            var property = cell is null ? null : state.Properties.FirstOrDefault(x => x.MapCellId == cell.Id);
            _cellButtons[i].Text = cell is null
                ? $"{i}\r\n未配置"
                : $"{i} {cell.CellName}\r\n{cell.CellType}\r\n{(property?.OwnerUserName is null ? string.Empty : $"业主:{property.OwnerUserName}")}\r\n{string.Join(",", players)}";
            _cellButtons[i].BackColor = CellColor(cell?.CellType);
        }

        _playerGrid.DataSource = state.Players;
        _propertyGrid.DataSource = state.Properties;
        _logList.DataSource = state.Logs.ToList();
        SetGameButtons(state.Status == "Playing" && state.CurrentPlayerUserId == _userId, state.CanBuyProperty);
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

    private void ShowBasic(BasicResult result)
    {
        SetStatus(result.Message, !result.Success);
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
        _lblStatus.ForeColor = error ? Color.FromArgb(182, 42, 42) : Color.FromArgb(28, 109, 72);
    }

    private static TabPage Page(string title)
    {
        return new TabPage(title) { BackColor = Color.FromArgb(244, 246, 248), Padding = new Padding(10) };
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
        var group = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(8) };
        content.Dock = DockStyle.Fill;
        group.Controls.Add(content);
        return group;
    }

    private static TabPage ManagePage(string title, DataGridView grid, Action add, Action save, Action delete)
    {
        var page = new TabPage(title) { Padding = new Padding(8), BackColor = Color.FromArgb(244, 246, 248) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
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
            Height = 30,
            Margin = new Padding(4, 4, 4, 4),
            BackColor = Color.FromArgb(34, 93, 137),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => click();
        return button;
    }

    private static Label Label(string text)
    {
        return new Label { AutoSize = true, Text = text, TextAlign = ContentAlignment.MiddleCenter, Margin = new Padding(10, 9, 2, 0) };
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
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(38, 50, 56),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold)
            }
        };
    }

    private static Color CellColor(string? type)
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
