#nullable disable

namespace MonopolyClient;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private Panel _loginPage;
    private Panel _mainShell;
    private TranslucentPanel _loginCard;
    private TableLayoutPanel _loginLayout;
    private TableLayoutPanel _loginButtons;
    private TextBox _txtIp;
    private NumericUpDown _numPort;
    private TextBox _txtUser;
    private TextBox _txtPassword;
    private ComboBox _roomTokenCombo;
    private Label _lblLoginStatus;
    private Label _lblStatus;
    private Label _lblUser;
    private Label _lblToken;
    private DataGridView _roomGrid;
    private TextBox _txtRoomName;
    private NumericUpDown _numMaxPlayers;
    private MonopolyClient.Visuals.BoardView _boardView;
    private FlowLayoutPanel _playerCards;
    private FlowLayoutPanel _chatMessages;
    private TextBox _txtChat;
    private CheckBox _chkSound;
    private DataGridView _propertyGrid;
    private TextBox _logText;
    private Label _lblTurn;
    private Label _lblActionHint;
    private PictureBox _dicePicture;
    private DataGridView _mapGrid;
    private DataGridView _managePropertyGrid;
    private DataGridView _eventGrid;
    private DataGridView _rankGrid;
    private DataGridView _historyGrid;
    private TabControl _tabs;

    private Label designerServerLabel;
    private Label designerUserLabel;
    private Label designerPasswordLabel;
    private Label designerRoomLabel;
    private Label designerPlayersLabel;
    private Panel designerServerRow;
    private TableLayoutPanel designerLoginBottom;
    private TableLayoutPanel designerMainRoot;
    private TableLayoutPanel designerHeader;
    private FlowLayoutPanel designerIdentityRow;
    private FlowLayoutPanel designerActionRow;
    private Label designerTokenPromptLabel;
    private TabPage designerLobbyPage;
    private TabPage designerGamePage;
    private TabPage designerManagePage;
    private TabPage designerRankPage;
    private SplitContainer designerLobbySplit;
    private TableLayoutPanel designerLobbyLeft;
    private TableLayoutPanel designerRoomActions;
    private Label designerLobbyHint;
    private FlowLayoutPanel designerLobbyToolbar;
    private Panel designerRoomListPanel;
    private Panel designerLobbyPreview;
    private TableLayoutPanel designerGameLayout;
    private Panel designerBoardHost;
    private Panel designerSideScroll;
    private TableLayoutPanel designerSidePanel;
    private TableLayoutPanel designerTurnPanel;
    private TableLayoutPanel designerGameControlsPanel;
    private TableLayoutPanel designerGameActionButtons;
    private GroupBox designerPlayersGroup;
    private GroupBox designerChatGroup;
    private TableLayoutPanel designerChatLayout;
    private TableLayoutPanel designerChatInputRow;
    private FlowLayoutPanel designerQuickChatButtons;
    private GroupBox designerDetailsGroup;
    private TabControl designerDetailsTabs;
    private TabPage designerPropertyTab;
    private TabPage designerLogTab;
    private TabControl designerManageTabs;
    private TabPage designerMapManageTab;
    private TabPage designerPropertyManageTab;
    private TabPage designerEventManageTab;
    private TableLayoutPanel designerMapManageLayout;
    private TableLayoutPanel designerPropertyManageLayout;
    private TableLayoutPanel designerEventManageLayout;
    private FlowLayoutPanel designerMapManageToolbar;
    private FlowLayoutPanel designerPropertyManageToolbar;
    private FlowLayoutPanel designerEventManageToolbar;
    private TableLayoutPanel designerRankLayout;
    private FlowLayoutPanel designerRankToolbar;
    private GroupBox designerRankGroup;
    private GroupBox designerHistoryGroup;

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        _loginPage = new Panel();
        _loginCard = new TranslucentPanel();
        _loginLayout = new TableLayoutPanel();
        designerServerLabel = new Label();
        designerServerRow = new Panel();
        _txtIp = new TextBox();
        _numPort = new NumericUpDown();
        designerUserLabel = new Label();
        _txtUser = new TextBox();
        designerPasswordLabel = new Label();
        _txtPassword = new TextBox();
        designerRoomLabel = new Label();
        _txtRoomName = new TextBox();
        designerPlayersLabel = new Label();
        _numMaxPlayers = new NumericUpDown();
        designerLoginBottom = new TableLayoutPanel();
        _loginButtons = new TableLayoutPanel();
        designerConnectButton = new SolidTextButton();
        designerRegisterButton = new SolidTextButton();
        designerEnterLobbyButton = new SolidTextButton();
        _lblLoginStatus = new Label();
        _mainShell = new Panel();
        designerMainRoot = new TableLayoutPanel();
        designerHeader = new TableLayoutPanel();
        designerIdentityRow = new FlowLayoutPanel();
        _lblUser = new Label();
        _lblToken = new Label();
        designerTokenPromptLabel = new Label();
        _roomTokenCombo = new ComboBox();
        designerSelectTokenButton = new SolidTextButton();
        _chkSound = new CheckBox();
        _lblStatus = new Label();
        designerActionRow = new FlowLayoutPanel();
        designerRefreshRoomsButton = new SolidTextButton();
        designerBackLoginButton = new SolidTextButton();
        _tabs = new TabControl();
        designerLobbyPage = new TabPage();
        designerLobbySplit = new SplitContainer();
        designerLobbyLeft = new TableLayoutPanel();
        designerRoomActions = new TableLayoutPanel();
        designerLobbyHint = new Label();
        designerLobbyToolbar = new FlowLayoutPanel();
        designerCreateRoomButton = new SolidTextButton();
        designerJoinRoomButton = new SolidTextButton();
        designerLeaveRoomButton = new SolidTextButton();
        designerReadyButton = new SolidTextButton();
        designerRoomListPanel = new Panel();
        _roomGrid = new DataGridView();
        designerLobbyPreview = new Panel();
        designerGamePage = new TabPage();
        designerGameLayout = new TableLayoutPanel();
        designerBoardHost = new Panel();
        _boardView = new MonopolyClient.Visuals.BoardView();
        designerSideScroll = new Panel();
        designerSidePanel = new TableLayoutPanel();
        designerTurnPanel = new TableLayoutPanel();
        _lblTurn = new Label();
        _lblActionHint = new Label();
        designerGameControlsPanel = new TableLayoutPanel();
        _dicePicture = new PictureBox();
        designerGameActionButtons = new TableLayoutPanel();
        _btnRoll = new SolidTextButton();
        _btnBuy = new SolidTextButton();
        _btnEnd = new SolidTextButton();
        designerPlayersGroup = new GroupBox();
        _playerCards = new FlowLayoutPanel();
        designerChatGroup = new GroupBox();
        designerChatLayout = new TableLayoutPanel();
        _chatMessages = new FlowLayoutPanel();
        designerChatInputRow = new TableLayoutPanel();
        _txtChat = new TextBox();
        designerSendChatButton = new SolidTextButton();
        designerQuickChatButtons = new FlowLayoutPanel();
        designerQuickWaitButton = new SolidTextButton();
        designerQuickDuelButton = new SolidTextButton();
        designerQuickFastButton = new SolidTextButton();
        designerQuickSteadyButton = new SolidTextButton();
        designerEggButton = new SolidTextButton();
        designerFlowerButton = new SolidTextButton();
        designerCheerButton = new SolidTextButton();
        designerVoiceButton = new SolidTextButton();
        designerDetailsGroup = new GroupBox();
        designerDetailsTabs = new TabControl();
        designerPropertyTab = new TabPage();
        _propertyGrid = new DataGridView();
        designerLogTab = new TabPage();
        _logText = new TextBox();
        designerManagePage = new TabPage();
        designerManageTabs = new TabControl();
        designerMapManageTab = new TabPage();
        designerMapManageLayout = new TableLayoutPanel();
        designerMapManageToolbar = new FlowLayoutPanel();
        _mapGrid = new DataGridView();
        designerPropertyManageTab = new TabPage();
        designerPropertyManageLayout = new TableLayoutPanel();
        designerPropertyManageToolbar = new FlowLayoutPanel();
        _managePropertyGrid = new DataGridView();
        designerEventManageTab = new TabPage();
        designerEventManageLayout = new TableLayoutPanel();
        designerEventManageToolbar = new FlowLayoutPanel();
        _eventGrid = new DataGridView();
        designerRankPage = new TabPage();
        designerRankLayout = new TableLayoutPanel();
        designerRankToolbar = new FlowLayoutPanel();
        designerRankGroup = new GroupBox();
        _rankGrid = new DataGridView();
        designerHistoryGroup = new GroupBox();
        _historyGrid = new DataGridView();
        _loginPage.SuspendLayout();
        _loginCard.SuspendLayout();
        _loginLayout.SuspendLayout();
        designerServerRow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_numPort).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_numMaxPlayers).BeginInit();
        designerLoginBottom.SuspendLayout();
        _loginButtons.SuspendLayout();
        _mainShell.SuspendLayout();
        designerMainRoot.SuspendLayout();
        designerHeader.SuspendLayout();
        designerIdentityRow.SuspendLayout();
        designerActionRow.SuspendLayout();
        _tabs.SuspendLayout();
        designerLobbyPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)designerLobbySplit).BeginInit();
        designerLobbySplit.Panel1.SuspendLayout();
        designerLobbySplit.Panel2.SuspendLayout();
        designerLobbySplit.SuspendLayout();
        designerLobbyLeft.SuspendLayout();
        designerRoomActions.SuspendLayout();
        designerLobbyToolbar.SuspendLayout();
        designerRoomListPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_roomGrid).BeginInit();
        designerGamePage.SuspendLayout();
        designerGameLayout.SuspendLayout();
        designerBoardHost.SuspendLayout();
        designerSideScroll.SuspendLayout();
        designerSidePanel.SuspendLayout();
        designerTurnPanel.SuspendLayout();
        designerGameControlsPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_dicePicture).BeginInit();
        designerGameActionButtons.SuspendLayout();
        designerPlayersGroup.SuspendLayout();
        designerChatGroup.SuspendLayout();
        designerChatLayout.SuspendLayout();
        designerChatInputRow.SuspendLayout();
        designerQuickChatButtons.SuspendLayout();
        designerDetailsGroup.SuspendLayout();
        designerDetailsTabs.SuspendLayout();
        designerPropertyTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_propertyGrid).BeginInit();
        designerLogTab.SuspendLayout();
        designerManagePage.SuspendLayout();
        designerManageTabs.SuspendLayout();
        designerMapManageTab.SuspendLayout();
        designerMapManageLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_mapGrid).BeginInit();
        designerPropertyManageTab.SuspendLayout();
        designerPropertyManageLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_managePropertyGrid).BeginInit();
        designerEventManageTab.SuspendLayout();
        designerEventManageLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_eventGrid).BeginInit();
        designerRankPage.SuspendLayout();
        designerRankLayout.SuspendLayout();
        designerRankGroup.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_rankGrid).BeginInit();
        designerHistoryGroup.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_historyGrid).BeginInit();
        SuspendLayout();
        //
        // _loginPage
        //
        _loginPage.BackColor = Color.FromArgb(36, 45, 33);
        _loginPage.BackgroundImageLayout = ImageLayout.Stretch;
        _loginPage.Controls.Add(_loginCard);
        _loginPage.Dock = DockStyle.Fill;
        _loginPage.Location = new Point(0, 0);
        _loginPage.Margin = new Padding(6, 5, 6, 5);
        _loginPage.Name = "_loginPage";
        _loginPage.Size = new Size(2480, 1495);
        _loginPage.TabIndex = 1;
        //
        // _loginCard
        //
        _loginCard.BackColor = Color.Transparent;
        _loginCard.BorderColor = Color.Transparent;
        _loginCard.Controls.Add(_loginLayout);
        _loginCard.FillColor = Color.Transparent;
        _loginCard.Location = new Point(560, 292);
        _loginCard.Margin = new Padding(6, 5, 6, 5);
        _loginCard.Name = "_loginCard";
        _loginCard.Padding = new Padding(68, 44, 68, 44);
        _loginCard.Size = new Size(1360, 912);
        _loginCard.TabIndex = 0;
        //
        // _loginLayout
        //
        _loginLayout.BackColor = Color.Transparent;
        _loginLayout.ColumnCount = 2;
        _loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380F));
        _loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _loginLayout.Controls.Add(designerServerLabel, 0, 1);
        _loginLayout.Controls.Add(designerServerRow, 1, 1);
        _loginLayout.Controls.Add(designerUserLabel, 0, 2);
        _loginLayout.Controls.Add(_txtUser, 1, 2);
        _loginLayout.Controls.Add(designerPasswordLabel, 0, 3);
        _loginLayout.Controls.Add(_txtPassword, 1, 3);
        _loginLayout.Controls.Add(designerRoomLabel, 0, 4);
        _loginLayout.Controls.Add(_txtRoomName, 1, 4);
        _loginLayout.Controls.Add(designerPlayersLabel, 0, 5);
        _loginLayout.Controls.Add(_numMaxPlayers, 1, 5);
        _loginLayout.Controls.Add(designerLoginBottom, 0, 7);
        _loginLayout.Dock = DockStyle.Fill;
        _loginLayout.Location = new Point(68, 44);
        _loginLayout.Margin = new Padding(6, 5, 6, 5);
        _loginLayout.Name = "_loginLayout";
        _loginLayout.RowCount = 8;
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
        _loginLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _loginLayout.Size = new Size(1224, 824);
        _loginLayout.TabIndex = 0;
        //
        // designerServerLabel
        //
        designerServerLabel.Dock = DockStyle.Fill;
        designerServerLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerServerLabel.ForeColor = Color.FromArgb(91, 45, 19);
        designerServerLabel.Location = new Point(6, 80);
        designerServerLabel.Margin = new Padding(6, 0, 6, 0);
        designerServerLabel.Name = "designerServerLabel";
        designerServerLabel.Size = new Size(368, 95);
        designerServerLabel.TabIndex = 0;
        designerServerLabel.Text = "服务器";
        designerServerLabel.TextAlign = ContentAlignment.MiddleRight;
        //
        // designerServerRow
        //
        designerServerRow.BackColor = Color.Transparent;
        designerServerRow.Controls.Add(_txtIp);
        designerServerRow.Controls.Add(_numPort);
        designerServerRow.Dock = DockStyle.Fill;
        designerServerRow.Location = new Point(380, 80);
        designerServerRow.Margin = new Padding(0);
        designerServerRow.Name = "designerServerRow";
        designerServerRow.Size = new Size(844, 95);
        designerServerRow.TabIndex = 1;
        //
        // _txtIp
        //
        _txtIp.BackColor = Color.FromArgb(255, 251, 235);
        _txtIp.BorderStyle = BorderStyle.FixedSingle;
        _txtIp.Font = new Font("Microsoft YaHei UI", 11F);
        _txtIp.Location = new Point(0, 13);
        _txtIp.Margin = new Padding(0);
        _txtIp.MinimumSize = new Size(2, 32);
        _txtIp.Name = "_txtIp";
        _txtIp.Size = new Size(558, 45);
        _txtIp.TabIndex = 0;
        _txtIp.Text = "127.0.0.1";
        //
        // _numPort
        //
        _numPort.BackColor = Color.FromArgb(255, 251, 235);
        _numPort.BorderStyle = BorderStyle.FixedSingle;
        _numPort.Font = new Font("Microsoft YaHei UI", 11F);
        _numPort.Location = new Point(576, 13);
        _numPort.Margin = new Padding(0);
        _numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        _numPort.MaximumSize = new Size(256, 0);
        _numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _numPort.MinimumSize = new Size(256, 0);
        _numPort.Name = "_numPort";
        _numPort.Size = new Size(256, 45);
        _numPort.TabIndex = 1;
        _numPort.Value = new decimal(new int[] { 9000, 0, 0, 0 });
        //
        // designerUserLabel
        //
        designerUserLabel.Dock = DockStyle.Fill;
        designerUserLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerUserLabel.ForeColor = Color.FromArgb(91, 45, 19);
        designerUserLabel.Location = new Point(6, 175);
        designerUserLabel.Margin = new Padding(6, 0, 6, 0);
        designerUserLabel.Name = "designerUserLabel";
        designerUserLabel.Size = new Size(368, 95);
        designerUserLabel.TabIndex = 2;
        designerUserLabel.Text = "玩家名 / ID";
        designerUserLabel.TextAlign = ContentAlignment.MiddleRight;
        //
        // _txtUser
        //
        _txtUser.BackColor = Color.FromArgb(255, 251, 235);
        _txtUser.BorderStyle = BorderStyle.FixedSingle;
        _txtUser.Dock = DockStyle.Fill;
        _txtUser.Font = new Font("Microsoft YaHei UI", 11F);
        _txtUser.Location = new Point(404, 188);
        _txtUser.Margin = new Padding(24, 13, 0, 13);
        _txtUser.Name = "_txtUser";
        _txtUser.Size = new Size(820, 45);
        _txtUser.TabIndex = 3;
        _txtUser.Text = "player1";
        //
        // designerPasswordLabel
        //
        designerPasswordLabel.Dock = DockStyle.Fill;
        designerPasswordLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerPasswordLabel.ForeColor = Color.FromArgb(91, 45, 19);
        designerPasswordLabel.Location = new Point(6, 270);
        designerPasswordLabel.Margin = new Padding(6, 0, 6, 0);
        designerPasswordLabel.Name = "designerPasswordLabel";
        designerPasswordLabel.Size = new Size(368, 95);
        designerPasswordLabel.TabIndex = 4;
        designerPasswordLabel.Text = "通行码";
        designerPasswordLabel.TextAlign = ContentAlignment.MiddleRight;
        //
        // _txtPassword
        //
        _txtPassword.BackColor = Color.FromArgb(255, 251, 235);
        _txtPassword.BorderStyle = BorderStyle.FixedSingle;
        _txtPassword.Dock = DockStyle.Fill;
        _txtPassword.Font = new Font("Microsoft YaHei UI", 11F);
        _txtPassword.Location = new Point(404, 283);
        _txtPassword.Margin = new Padding(24, 13, 0, 13);
        _txtPassword.Name = "_txtPassword";
        _txtPassword.Size = new Size(820, 45);
        _txtPassword.TabIndex = 5;
        _txtPassword.Text = "123456";
        _txtPassword.UseSystemPasswordChar = true;
        //
        // designerRoomLabel
        //
        designerRoomLabel.Dock = DockStyle.Fill;
        designerRoomLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerRoomLabel.ForeColor = Color.FromArgb(91, 45, 19);
        designerRoomLabel.Location = new Point(6, 365);
        designerRoomLabel.Margin = new Padding(6, 0, 6, 0);
        designerRoomLabel.Name = "designerRoomLabel";
        designerRoomLabel.Size = new Size(368, 95);
        designerRoomLabel.TabIndex = 6;
        designerRoomLabel.Text = "房间名";
        designerRoomLabel.TextAlign = ContentAlignment.MiddleRight;
        //
        // _txtRoomName
        //
        _txtRoomName.BackColor = Color.FromArgb(255, 251, 235);
        _txtRoomName.BorderStyle = BorderStyle.FixedSingle;
        _txtRoomName.Dock = DockStyle.Fill;
        _txtRoomName.Font = new Font("Microsoft YaHei UI", 11F);
        _txtRoomName.Location = new Point(404, 378);
        _txtRoomName.Margin = new Padding(24, 13, 0, 13);
        _txtRoomName.Name = "_txtRoomName";
        _txtRoomName.Size = new Size(820, 45);
        _txtRoomName.TabIndex = 7;
        _txtRoomName.Text = "河南文旅房间";
        //
        // designerPlayersLabel
        //
        designerPlayersLabel.Dock = DockStyle.Fill;
        designerPlayersLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerPlayersLabel.ForeColor = Color.FromArgb(91, 45, 19);
        designerPlayersLabel.Location = new Point(6, 460);
        designerPlayersLabel.Margin = new Padding(6, 0, 6, 0);
        designerPlayersLabel.Name = "designerPlayersLabel";
        designerPlayersLabel.Size = new Size(368, 95);
        designerPlayersLabel.TabIndex = 8;
        designerPlayersLabel.Text = "人数";
        designerPlayersLabel.TextAlign = ContentAlignment.MiddleRight;
        //
        // _numMaxPlayers
        //
        _numMaxPlayers.BackColor = Color.FromArgb(255, 251, 235);
        _numMaxPlayers.Font = new Font("Microsoft YaHei UI", 11F);
        _numMaxPlayers.Location = new Point(404, 473);
        _numMaxPlayers.Margin = new Padding(24, 13, 0, 13);
        _numMaxPlayers.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
        _numMaxPlayers.MaximumSize = new Size(256, 0);
        _numMaxPlayers.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        _numMaxPlayers.MinimumSize = new Size(256, 0);
        _numMaxPlayers.Name = "_numMaxPlayers";
        _numMaxPlayers.Size = new Size(256, 45);
        _numMaxPlayers.TabIndex = 9;
        _numMaxPlayers.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // designerLoginBottom
        //
        designerLoginBottom.ColumnCount = 1;
        _loginLayout.SetColumnSpan(designerLoginBottom, 2);
        designerLoginBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerLoginBottom.Controls.Add(_loginButtons, 0, 0);
        designerLoginBottom.Controls.Add(_lblLoginStatus, 0, 1);
        designerLoginBottom.Dock = DockStyle.Fill;
        designerLoginBottom.Location = new Point(6, 562);
        designerLoginBottom.Margin = new Padding(6, 5, 6, 5);
        designerLoginBottom.Name = "designerLoginBottom";
        designerLoginBottom.RowCount = 2;
        designerLoginBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 124F));
        designerLoginBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerLoginBottom.Size = new Size(1212, 257);
        designerLoginBottom.TabIndex = 10;
        //
        // _loginButtons
        //
        _loginButtons.ColumnCount = 3;
        _loginButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        _loginButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        _loginButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        _loginButtons.Controls.Add(designerConnectButton, 0, 0);
        _loginButtons.Controls.Add(designerRegisterButton, 1, 0);
        _loginButtons.Controls.Add(designerEnterLobbyButton, 2, 0);
        _loginButtons.Dock = DockStyle.Fill;
        _loginButtons.Location = new Point(6, 5);
        _loginButtons.Margin = new Padding(6, 5, 6, 5);
        _loginButtons.Name = "_loginButtons";
        _loginButtons.Padding = new Padding(0, 18, 0, 0);
        _loginButtons.RowCount = 1;
        _loginButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _loginButtons.Size = new Size(1200, 114);
        _loginButtons.TabIndex = 0;
        //
        // designerConnectButton
        //
        designerConnectButton.BackColor = Color.FromArgb(135, 82, 37);
        designerConnectButton.Dock = DockStyle.Fill;
        designerConnectButton.FlatStyle = FlatStyle.Flat;
        designerConnectButton.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
        designerConnectButton.ForeColor = Color.White;
        designerConnectButton.Location = new Point(6, 23);
        designerConnectButton.Margin = new Padding(6, 5, 6, 5);
        designerConnectButton.Name = "designerConnectButton";
        designerConnectButton.Size = new Size(387, 86);
        designerConnectButton.TabIndex = 0;
        designerConnectButton.Text = "连接";
        designerConnectButton.UseVisualStyleBackColor = false;
        //
        // designerRegisterButton
        //
        designerRegisterButton.BackColor = Color.FromArgb(135, 82, 37);
        designerRegisterButton.Dock = DockStyle.Fill;
        designerRegisterButton.FlatStyle = FlatStyle.Flat;
        designerRegisterButton.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
        designerRegisterButton.ForeColor = Color.White;
        designerRegisterButton.Location = new Point(405, 23);
        designerRegisterButton.Margin = new Padding(6, 5, 6, 5);
        designerRegisterButton.Name = "designerRegisterButton";
        designerRegisterButton.Size = new Size(388, 86);
        designerRegisterButton.TabIndex = 1;
        designerRegisterButton.Text = "注册";
        designerRegisterButton.UseVisualStyleBackColor = false;
        //
        // designerEnterLobbyButton
        //
        designerEnterLobbyButton.BackColor = Color.FromArgb(135, 82, 37);
        designerEnterLobbyButton.Dock = DockStyle.Fill;
        designerEnterLobbyButton.FlatStyle = FlatStyle.Flat;
        designerEnterLobbyButton.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
        designerEnterLobbyButton.ForeColor = Color.White;
        designerEnterLobbyButton.Location = new Point(805, 23);
        designerEnterLobbyButton.Margin = new Padding(6, 5, 6, 5);
        designerEnterLobbyButton.Name = "designerEnterLobbyButton";
        designerEnterLobbyButton.Size = new Size(389, 86);
        designerEnterLobbyButton.TabIndex = 2;
        designerEnterLobbyButton.Text = "进入大厅";
        designerEnterLobbyButton.UseVisualStyleBackColor = false;
        //
        // _lblLoginStatus
        //
        _lblLoginStatus.Dock = DockStyle.Fill;
        _lblLoginStatus.ForeColor = Color.FromArgb(91, 45, 19);
        _lblLoginStatus.Location = new Point(6, 124);
        _lblLoginStatus.Margin = new Padding(6, 0, 6, 0);
        _lblLoginStatus.Name = "_lblLoginStatus";
        _lblLoginStatus.Size = new Size(1200, 133);
        _lblLoginStatus.TabIndex = 1;
        _lblLoginStatus.Text = "请先连接服务器";
        _lblLoginStatus.TextAlign = ContentAlignment.MiddleCenter;
        //
        // _mainShell
        //
        _mainShell.BackColor = Color.FromArgb(238, 224, 184);
        _mainShell.Controls.Add(designerMainRoot);
        _mainShell.Dock = DockStyle.Fill;
        _mainShell.Location = new Point(0, 0);
        _mainShell.Margin = new Padding(6, 5, 6, 5);
        _mainShell.Name = "_mainShell";
        _mainShell.Size = new Size(2480, 1495);
        _mainShell.TabIndex = 0;
        _mainShell.Visible = false;
        //
        // designerMainRoot
        //
        designerMainRoot.BackColor = Color.FromArgb(238, 224, 184);
        designerMainRoot.ColumnCount = 1;
        designerMainRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerMainRoot.Controls.Add(designerHeader, 0, 0);
        designerMainRoot.Controls.Add(_tabs, 0, 1);
        designerMainRoot.Dock = DockStyle.Fill;
        designerMainRoot.Location = new Point(0, 0);
        designerMainRoot.Margin = new Padding(6, 5, 6, 5);
        designerMainRoot.Name = "designerMainRoot";
        designerMainRoot.Padding = new Padding(24, 22, 24, 22);
        designerMainRoot.RowCount = 2;
        designerMainRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 244F));
        designerMainRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerMainRoot.Size = new Size(2480, 1495);
        designerMainRoot.TabIndex = 0;
        //
        // designerHeader
        //
        designerHeader.BackColor = Color.FromArgb(238, 224, 184);
        designerHeader.ColumnCount = 1;
        designerHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerHeader.Controls.Add(designerIdentityRow, 0, 0);
        designerHeader.Controls.Add(designerActionRow, 0, 1);
        designerHeader.Dock = DockStyle.Fill;
        designerHeader.Location = new Point(30, 27);
        designerHeader.Margin = new Padding(6, 5, 6, 5);
        designerHeader.Name = "designerHeader";
        designerHeader.RowCount = 2;
        designerHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 113F));
        designerHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 113F));
        designerHeader.Size = new Size(2420, 234);
        designerHeader.TabIndex = 0;
        //
        // designerIdentityRow
        //
        designerIdentityRow.BackColor = Color.Transparent;
        designerIdentityRow.Controls.Add(_lblUser);
        designerIdentityRow.Controls.Add(_lblToken);
        designerIdentityRow.Controls.Add(designerTokenPromptLabel);
        designerIdentityRow.Controls.Add(_roomTokenCombo);
        designerIdentityRow.Controls.Add(designerSelectTokenButton);
        designerIdentityRow.Controls.Add(_chkSound);
        designerIdentityRow.Controls.Add(_lblStatus);
        designerIdentityRow.Dock = DockStyle.Fill;
        designerIdentityRow.Location = new Point(6, 5);
        designerIdentityRow.Margin = new Padding(6, 5, 6, 5);
        designerIdentityRow.Name = "designerIdentityRow";
        designerIdentityRow.Size = new Size(2408, 103);
        designerIdentityRow.TabIndex = 0;
        designerIdentityRow.WrapContents = false;
        //
        // _lblUser
        //
        _lblUser.AutoSize = true;
        _lblUser.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblUser.Location = new Point(0, 33);
        _lblUser.Margin = new Padding(0, 33, 68, 0);
        _lblUser.Name = "_lblUser";
        _lblUser.Size = new Size(114, 42);
        _lblUser.TabIndex = 0;
        _lblUser.Text = "未登录";
        //
        // _lblToken
        //
        _lblToken.AutoSize = true;
        _lblToken.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblToken.Location = new Point(182, 33);
        _lblToken.Margin = new Padding(0, 33, 76, 0);
        _lblToken.Name = "_lblToken";
        _lblToken.Size = new Size(128, 42);
        _lblToken.TabIndex = 1;
        _lblToken.Text = "棋子：-";
        //
        // designerTokenPromptLabel
        //
        designerTokenPromptLabel.AutoSize = true;
        designerTokenPromptLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerTokenPromptLabel.Location = new Point(386, 33);
        designerTokenPromptLabel.Margin = new Padding(0, 33, 12, 0);
        designerTokenPromptLabel.Name = "designerTokenPromptLabel";
        designerTokenPromptLabel.Size = new Size(146, 42);
        designerTokenPromptLabel.TabIndex = 2;
        designerTokenPromptLabel.Text = "房间棋子";
        //
        // _roomTokenCombo
        //
        _roomTokenCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _roomTokenCombo.Font = new Font("Microsoft YaHei UI", 11.5F);
        _roomTokenCombo.Location = new Point(544, 22);
        _roomTokenCombo.Margin = new Padding(0, 22, 24, 0);
        _roomTokenCombo.Name = "_roomTokenCombo";
        _roomTokenCombo.Size = new Size(328, 48);
        _roomTokenCombo.TabIndex = 3;
        //
        // designerSelectTokenButton
        //
        designerSelectTokenButton.BackColor = Color.FromArgb(125, 51, 27);
        designerSelectTokenButton.FlatStyle = FlatStyle.Flat;
        designerSelectTokenButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerSelectTokenButton.ForeColor = Color.White;
        designerSelectTokenButton.Location = new Point(902, 5);
        designerSelectTokenButton.Margin = new Padding(6, 5, 6, 5);
        designerSelectTokenButton.Name = "designerSelectTokenButton";
        designerSelectTokenButton.Size = new Size(150, 42);
        designerSelectTokenButton.TabIndex = 4;
        designerSelectTokenButton.Text = "选择棋子";
        designerSelectTokenButton.UseVisualStyleBackColor = false;
        //
        // _chkSound
        //
        _chkSound.AutoSize = true;
        _chkSound.Checked = true;
        _chkSound.CheckState = CheckState.Checked;
        _chkSound.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _chkSound.ForeColor = Color.FromArgb(64, 44, 25);
        _chkSound.Location = new Point(1098, 31);
        _chkSound.Margin = new Padding(40, 31, 20, 0);
        _chkSound.Name = "_chkSound";
        _chkSound.Size = new Size(114, 46);
        _chkSound.TabIndex = 5;
        _chkSound.Text = "音效";
        //
        // _lblStatus
        //
        _lblStatus.AutoSize = true;
        _lblStatus.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblStatus.Location = new Point(1248, 33);
        _lblStatus.Margin = new Padding(16, 33, 0, 0);
        _lblStatus.Name = "_lblStatus";
        _lblStatus.Size = new Size(114, 42);
        _lblStatus.TabIndex = 6;
        _lblStatus.Text = "未连接";
        //
        // designerActionRow
        //
        designerActionRow.BackColor = Color.Transparent;
        designerActionRow.Controls.Add(designerRefreshRoomsButton);
        designerActionRow.Controls.Add(designerBackLoginButton);
        designerActionRow.Dock = DockStyle.Fill;
        designerActionRow.Location = new Point(6, 118);
        designerActionRow.Margin = new Padding(6, 5, 6, 5);
        designerActionRow.Name = "designerActionRow";
        designerActionRow.Size = new Size(2408, 111);
        designerActionRow.TabIndex = 1;
        designerActionRow.WrapContents = false;
        //
        // designerRefreshRoomsButton
        //
        designerRefreshRoomsButton.BackColor = Color.FromArgb(125, 51, 27);
        designerRefreshRoomsButton.FlatStyle = FlatStyle.Flat;
        designerRefreshRoomsButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerRefreshRoomsButton.ForeColor = Color.White;
        designerRefreshRoomsButton.Location = new Point(6, 5);
        designerRefreshRoomsButton.Margin = new Padding(6, 5, 6, 5);
        designerRefreshRoomsButton.Name = "designerRefreshRoomsButton";
        designerRefreshRoomsButton.Size = new Size(150, 42);
        designerRefreshRoomsButton.TabIndex = 0;
        designerRefreshRoomsButton.Text = "刷新房间";
        designerRefreshRoomsButton.UseVisualStyleBackColor = false;
        //
        // designerBackLoginButton
        //
        designerBackLoginButton.BackColor = Color.FromArgb(125, 51, 27);
        designerBackLoginButton.FlatStyle = FlatStyle.Flat;
        designerBackLoginButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerBackLoginButton.ForeColor = Color.White;
        designerBackLoginButton.Location = new Point(168, 5);
        designerBackLoginButton.Margin = new Padding(6, 5, 6, 5);
        designerBackLoginButton.Name = "designerBackLoginButton";
        designerBackLoginButton.Size = new Size(150, 42);
        designerBackLoginButton.TabIndex = 1;
        designerBackLoginButton.Text = "回到登录页";
        designerBackLoginButton.UseVisualStyleBackColor = false;
        //
        // _tabs
        //
        _tabs.Controls.Add(designerLobbyPage);
        _tabs.Controls.Add(designerGamePage);
        _tabs.Controls.Add(designerManagePage);
        _tabs.Controls.Add(designerRankPage);
        _tabs.Dock = DockStyle.Fill;
        _tabs.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _tabs.ItemSize = new Size(230, 44);
        _tabs.Location = new Point(30, 271);
        _tabs.Margin = new Padding(6, 5, 6, 5);
        _tabs.Name = "_tabs";
        _tabs.SelectedIndex = 0;
        _tabs.Size = new Size(2420, 1197);
        _tabs.SizeMode = TabSizeMode.Fixed;
        _tabs.TabIndex = 1;
        //
        // designerLobbyPage
        //
        designerLobbyPage.BackColor = Color.FromArgb(238, 224, 184);
        designerLobbyPage.Controls.Add(designerLobbySplit);
        designerLobbyPage.Location = new Point(8, 52);
        designerLobbyPage.Margin = new Padding(6, 5, 6, 5);
        designerLobbyPage.Name = "designerLobbyPage";
        designerLobbyPage.Padding = new Padding(20, 18, 20, 18);
        designerLobbyPage.Size = new Size(2404, 1137);
        designerLobbyPage.TabIndex = 0;
        designerLobbyPage.Text = "房间大厅";
        //
        // designerLobbySplit
        //
        designerLobbySplit.Dock = DockStyle.Fill;
        designerLobbySplit.FixedPanel = FixedPanel.Panel2;
        designerLobbySplit.Location = new Point(20, 18);
        designerLobbySplit.Margin = new Padding(6, 5, 6, 5);
        designerLobbySplit.Name = "designerLobbySplit";
        //
        // designerLobbySplit.Panel1
        //
        designerLobbySplit.Panel1.Controls.Add(designerLobbyLeft);
        //
        // designerLobbySplit.Panel2
        //
        designerLobbySplit.Panel2.Controls.Add(designerLobbyPreview);
        designerLobbySplit.Size = new Size(2364, 1101);
        designerLobbySplit.SplitterDistance = 2286;
        designerLobbySplit.SplitterWidth = 8;
        designerLobbySplit.TabIndex = 0;
        //
        // designerLobbyLeft
        //
        designerLobbyLeft.BackColor = Color.FromArgb(245, 236, 207);
        designerLobbyLeft.ColumnCount = 1;
        designerLobbyLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerLobbyLeft.Controls.Add(designerRoomActions, 0, 0);
        designerLobbyLeft.Controls.Add(designerRoomListPanel, 0, 1);
        designerLobbyLeft.Dock = DockStyle.Fill;
        designerLobbyLeft.Location = new Point(0, 0);
        designerLobbyLeft.Margin = new Padding(6, 5, 6, 5);
        designerLobbyLeft.Name = "designerLobbyLeft";
        designerLobbyLeft.Padding = new Padding(24, 22, 24, 22);
        designerLobbyLeft.RowCount = 2;
        designerLobbyLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 310F));
        designerLobbyLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerLobbyLeft.Size = new Size(2286, 1101);
        designerLobbyLeft.TabIndex = 0;
        //
        // designerRoomActions
        //
        designerRoomActions.BackColor = Color.FromArgb(252, 248, 235);
        designerRoomActions.ColumnCount = 1;
        designerRoomActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerRoomActions.Controls.Add(designerLobbyHint, 0, 0);
        designerRoomActions.Controls.Add(designerLobbyToolbar, 0, 1);
        designerRoomActions.Dock = DockStyle.Fill;
        designerRoomActions.Location = new Point(30, 27);
        designerRoomActions.Margin = new Padding(6, 5, 6, 5);
        designerRoomActions.Name = "designerRoomActions";
        designerRoomActions.Padding = new Padding(32, 22, 32, 18);
        designerRoomActions.RowCount = 2;
        designerRoomActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 113F));
        designerRoomActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
        designerRoomActions.Size = new Size(2226, 300);
        designerRoomActions.TabIndex = 0;
        //
        // designerLobbyHint
        //
        designerLobbyHint.Dock = DockStyle.Fill;
        designerLobbyHint.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
        designerLobbyHint.ForeColor = Color.FromArgb(92, 67, 38);
        designerLobbyHint.Location = new Point(38, 22);
        designerLobbyHint.Margin = new Padding(6, 0, 6, 0);
        designerLobbyHint.Name = "designerLobbyHint";
        designerLobbyHint.Size = new Size(2150, 113);
        designerLobbyHint.TabIndex = 0;
        designerLobbyHint.Text = "选择房间后加入，房主可准备开局。";
        designerLobbyHint.TextAlign = ContentAlignment.MiddleLeft;
        //
        // designerLobbyToolbar
        //
        designerLobbyToolbar.Controls.Add(designerCreateRoomButton);
        designerLobbyToolbar.Controls.Add(designerJoinRoomButton);
        designerLobbyToolbar.Controls.Add(designerLeaveRoomButton);
        designerLobbyToolbar.Controls.Add(designerReadyButton);
        designerLobbyToolbar.Dock = DockStyle.Fill;
        designerLobbyToolbar.Location = new Point(38, 140);
        designerLobbyToolbar.Margin = new Padding(6, 5, 6, 5);
        designerLobbyToolbar.Name = "designerLobbyToolbar";
        designerLobbyToolbar.Size = new Size(2150, 137);
        designerLobbyToolbar.TabIndex = 1;
        designerLobbyToolbar.WrapContents = false;
        //
        // designerCreateRoomButton
        //
        designerCreateRoomButton.BackColor = Color.FromArgb(125, 51, 27);
        designerCreateRoomButton.FlatStyle = FlatStyle.Flat;
        designerCreateRoomButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerCreateRoomButton.ForeColor = Color.White;
        designerCreateRoomButton.Location = new Point(6, 5);
        designerCreateRoomButton.Margin = new Padding(6, 5, 6, 5);
        designerCreateRoomButton.Name = "designerCreateRoomButton";
        designerCreateRoomButton.Size = new Size(150, 42);
        designerCreateRoomButton.TabIndex = 0;
        designerCreateRoomButton.Text = "创建房间";
        designerCreateRoomButton.UseVisualStyleBackColor = false;
        //
        // designerJoinRoomButton
        //
        designerJoinRoomButton.BackColor = Color.FromArgb(125, 51, 27);
        designerJoinRoomButton.FlatStyle = FlatStyle.Flat;
        designerJoinRoomButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerJoinRoomButton.ForeColor = Color.White;
        designerJoinRoomButton.Location = new Point(168, 5);
        designerJoinRoomButton.Margin = new Padding(6, 5, 6, 5);
        designerJoinRoomButton.Name = "designerJoinRoomButton";
        designerJoinRoomButton.Size = new Size(150, 42);
        designerJoinRoomButton.TabIndex = 1;
        designerJoinRoomButton.Text = "加入选中";
        designerJoinRoomButton.UseVisualStyleBackColor = false;
        //
        // designerLeaveRoomButton
        //
        designerLeaveRoomButton.BackColor = Color.FromArgb(125, 51, 27);
        designerLeaveRoomButton.FlatStyle = FlatStyle.Flat;
        designerLeaveRoomButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerLeaveRoomButton.ForeColor = Color.White;
        designerLeaveRoomButton.Location = new Point(330, 5);
        designerLeaveRoomButton.Margin = new Padding(6, 5, 6, 5);
        designerLeaveRoomButton.Name = "designerLeaveRoomButton";
        designerLeaveRoomButton.Size = new Size(150, 42);
        designerLeaveRoomButton.TabIndex = 2;
        designerLeaveRoomButton.Text = "离开房间";
        designerLeaveRoomButton.UseVisualStyleBackColor = false;
        //
        // designerReadyButton
        //
        designerReadyButton.BackColor = Color.FromArgb(125, 51, 27);
        designerReadyButton.FlatStyle = FlatStyle.Flat;
        designerReadyButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerReadyButton.ForeColor = Color.White;
        designerReadyButton.Location = new Point(492, 5);
        designerReadyButton.Margin = new Padding(6, 5, 6, 5);
        designerReadyButton.Name = "designerReadyButton";
        designerReadyButton.Size = new Size(150, 42);
        designerReadyButton.TabIndex = 3;
        designerReadyButton.Text = "准备";
        designerReadyButton.UseVisualStyleBackColor = false;
        //
        // designerRoomListPanel
        //
        designerRoomListPanel.BackColor = Color.FromArgb(252, 248, 235);
        designerRoomListPanel.Controls.Add(_roomGrid);
        designerRoomListPanel.Dock = DockStyle.Fill;
        designerRoomListPanel.Location = new Point(30, 337);
        designerRoomListPanel.Margin = new Padding(6, 5, 6, 5);
        designerRoomListPanel.Name = "designerRoomListPanel";
        designerRoomListPanel.Size = new Size(2226, 737);
        designerRoomListPanel.TabIndex = 1;
        //
        // _roomGrid
        //
        _roomGrid.AllowUserToAddRows = false;
        _roomGrid.AllowUserToDeleteRows = false;
        _roomGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _roomGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _roomGrid.BorderStyle = BorderStyle.None;
        _roomGrid.ColumnHeadersHeight = 46;
        _roomGrid.Dock = DockStyle.Fill;
        _roomGrid.EnableHeadersVisualStyles = false;
        _roomGrid.GridColor = Color.FromArgb(224, 210, 171);
        _roomGrid.Location = new Point(0, 0);
        _roomGrid.Margin = new Padding(6, 5, 6, 5);
        _roomGrid.MultiSelect = false;
        _roomGrid.Name = "_roomGrid";
        _roomGrid.ReadOnly = true;
        _roomGrid.RowHeadersVisible = false;
        _roomGrid.RowHeadersWidth = 82;
        _roomGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _roomGrid.Size = new Size(2226, 737);
        _roomGrid.TabIndex = 0;
        //
        // designerLobbyPreview
        //
        designerLobbyPreview.BackColor = Color.FromArgb(33, 67, 49);
        designerLobbyPreview.BackgroundImageLayout = ImageLayout.Zoom;
        designerLobbyPreview.Dock = DockStyle.Fill;
        designerLobbyPreview.Location = new Point(0, 0);
        designerLobbyPreview.Margin = new Padding(6, 5, 6, 5);
        designerLobbyPreview.Name = "designerLobbyPreview";
        designerLobbyPreview.Size = new Size(70, 1101);
        designerLobbyPreview.TabIndex = 0;
        //
        // designerGamePage
        //
        designerGamePage.BackColor = Color.FromArgb(238, 224, 184);
        designerGamePage.Controls.Add(designerGameLayout);
        designerGamePage.Location = new Point(8, 52);
        designerGamePage.Margin = new Padding(6, 5, 6, 5);
        designerGamePage.Name = "designerGamePage";
        designerGamePage.Padding = new Padding(20, 18, 20, 18);
        designerGamePage.Size = new Size(2404, 1137);
        designerGamePage.TabIndex = 1;
        designerGamePage.Text = "游戏棋盘";
        //
        // designerGameLayout
        //
        designerGameLayout.ColumnCount = 2;
        designerGameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerGameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1180F));
        designerGameLayout.Controls.Add(designerBoardHost, 0, 0);
        designerGameLayout.Controls.Add(designerSideScroll, 1, 0);
        designerGameLayout.Dock = DockStyle.Fill;
        designerGameLayout.Location = new Point(20, 18);
        designerGameLayout.Margin = new Padding(6, 5, 6, 5);
        designerGameLayout.Name = "designerGameLayout";
        designerGameLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerGameLayout.Size = new Size(2364, 1101);
        designerGameLayout.TabIndex = 0;
        //
        // designerBoardHost
        //
        designerBoardHost.BackColor = Color.FromArgb(29, 58, 44);
        designerBoardHost.Controls.Add(_boardView);
        designerBoardHost.Dock = DockStyle.Fill;
        designerBoardHost.Location = new Point(6, 5);
        designerBoardHost.Margin = new Padding(6, 5, 6, 5);
        designerBoardHost.Name = "designerBoardHost";
        designerBoardHost.Padding = new Padding(8, 7, 8, 7);
        designerBoardHost.Size = new Size(1172, 1091);
        designerBoardHost.TabIndex = 0;
        //
        // _boardView
        //
        _boardView.BackColor = Color.FromArgb(31, 54, 42);
        _boardView.Dock = DockStyle.Fill;
        _boardView.Location = new Point(8, 7);
        _boardView.Margin = new Padding(6, 5, 6, 5);
        _boardView.MinimumSize = new Size(1120, 1021);
        _boardView.Name = "_boardView";
        _boardView.Size = new Size(1156, 1077);
        _boardView.TabIndex = 0;
        //
        // designerSideScroll
        //
        designerSideScroll.AutoScroll = true;
        designerSideScroll.BackColor = Color.FromArgb(238, 224, 184);
        designerSideScroll.Controls.Add(designerSidePanel);
        designerSideScroll.Dock = DockStyle.Fill;
        designerSideScroll.Location = new Point(1190, 5);
        designerSideScroll.Margin = new Padding(6, 5, 6, 5);
        designerSideScroll.Name = "designerSideScroll";
        designerSideScroll.Size = new Size(1168, 1091);
        designerSideScroll.TabIndex = 1;
        //
        // designerSidePanel
        //
        designerSidePanel.BackColor = Color.Transparent;
        designerSidePanel.ColumnCount = 1;
        designerSidePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerSidePanel.Controls.Add(designerTurnPanel, 0, 0);
        designerSidePanel.Controls.Add(designerGameControlsPanel, 0, 1);
        designerSidePanel.Controls.Add(designerPlayersGroup, 0, 2);
        designerSidePanel.Controls.Add(designerChatGroup, 0, 3);
        designerSidePanel.Controls.Add(designerDetailsGroup, 0, 4);
        designerSidePanel.Dock = DockStyle.Top;
        designerSidePanel.Location = new Point(0, 0);
        designerSidePanel.Margin = new Padding(6, 5, 6, 5);
        designerSidePanel.Name = "designerSidePanel";
        designerSidePanel.Padding = new Padding(16, 15, 16, 15);
        designerSidePanel.RowCount = 5;
        designerSidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 241F));
        designerSidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 204F));
        designerSidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 434F));
        designerSidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 693F));
        designerSidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerSidePanel.Size = new Size(1134, 2097);
        designerSidePanel.TabIndex = 0;
        //
        // designerTurnPanel
        //
        designerTurnPanel.BackColor = Color.FromArgb(245, 236, 207);
        designerTurnPanel.ColumnCount = 1;
        designerTurnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerTurnPanel.Controls.Add(_lblTurn, 0, 0);
        designerTurnPanel.Controls.Add(_lblActionHint, 0, 1);
        designerTurnPanel.Dock = DockStyle.Fill;
        designerTurnPanel.Location = new Point(22, 20);
        designerTurnPanel.Margin = new Padding(6, 5, 6, 5);
        designerTurnPanel.Name = "designerTurnPanel";
        designerTurnPanel.Padding = new Padding(28, 22, 28, 22);
        designerTurnPanel.RowCount = 2;
        designerTurnPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 91F));
        designerTurnPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
        designerTurnPanel.Size = new Size(1090, 231);
        designerTurnPanel.TabIndex = 0;
        //
        // _lblTurn
        //
        _lblTurn.Dock = DockStyle.Fill;
        _lblTurn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblTurn.ForeColor = Color.FromArgb(64, 44, 25);
        _lblTurn.Location = new Point(34, 22);
        _lblTurn.Margin = new Padding(6, 0, 6, 0);
        _lblTurn.Name = "_lblTurn";
        _lblTurn.Size = new Size(1022, 91);
        _lblTurn.TabIndex = 0;
        _lblTurn.Text = "房间 1 | 第 1 回合";
        _lblTurn.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _lblActionHint
        //
        _lblActionHint.Dock = DockStyle.Fill;
        _lblActionHint.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        _lblActionHint.ForeColor = Color.FromArgb(92, 67, 38);
        _lblActionHint.Location = new Point(34, 113);
        _lblActionHint.Margin = new Padding(6, 0, 6, 0);
        _lblActionHint.Name = "_lblActionHint";
        _lblActionHint.Size = new Size(1022, 96);
        _lblActionHint.TabIndex = 1;
        _lblActionHint.Text = "当前：player1 | 轮到你行动";
        _lblActionHint.TextAlign = ContentAlignment.MiddleLeft;
        //
        // designerGameControlsPanel
        //
        designerGameControlsPanel.BackColor = Color.FromArgb(245, 236, 207);
        designerGameControlsPanel.ColumnCount = 2;
        designerGameControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 152F));
        designerGameControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerGameControlsPanel.Controls.Add(_dicePicture, 0, 0);
        designerGameControlsPanel.Controls.Add(designerGameActionButtons, 1, 0);
        designerGameControlsPanel.Dock = DockStyle.Fill;
        designerGameControlsPanel.Location = new Point(22, 261);
        designerGameControlsPanel.Margin = new Padding(6, 5, 6, 5);
        designerGameControlsPanel.Name = "designerGameControlsPanel";
        designerGameControlsPanel.Padding = new Padding(20, 15, 20, 15);
        designerGameControlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerGameControlsPanel.Size = new Size(1090, 194);
        designerGameControlsPanel.TabIndex = 1;
        //
        // _dicePicture
        //
        _dicePicture.BackColor = Color.FromArgb(252, 248, 235);
        _dicePicture.Dock = DockStyle.Fill;
        _dicePicture.Location = new Point(26, 20);
        _dicePicture.Margin = new Padding(6, 5, 6, 5);
        _dicePicture.Name = "_dicePicture";
        _dicePicture.Size = new Size(140, 154);
        _dicePicture.SizeMode = PictureBoxSizeMode.Zoom;
        _dicePicture.TabIndex = 0;
        _dicePicture.TabStop = false;
        //
        // designerGameActionButtons
        //
        designerGameActionButtons.ColumnCount = 3;
        designerGameActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        designerGameActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        designerGameActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        designerGameActionButtons.Controls.Add(_btnRoll, 0, 0);
        designerGameActionButtons.Controls.Add(_btnBuy, 1, 0);
        designerGameActionButtons.Controls.Add(_btnEnd, 2, 0);
        designerGameActionButtons.Dock = DockStyle.Fill;
        designerGameActionButtons.Location = new Point(178, 20);
        designerGameActionButtons.Margin = new Padding(6, 5, 6, 5);
        designerGameActionButtons.Name = "designerGameActionButtons";
        designerGameActionButtons.RowCount = 1;
        designerGameActionButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerGameActionButtons.Size = new Size(886, 154);
        designerGameActionButtons.TabIndex = 1;
        //
        // _btnRoll
        //
        _btnRoll.BackColor = Color.FromArgb(125, 51, 27);
        _btnRoll.Dock = DockStyle.Fill;
        _btnRoll.FlatStyle = FlatStyle.Flat;
        _btnRoll.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
        _btnRoll.ForeColor = Color.White;
        _btnRoll.Location = new Point(6, 5);
        _btnRoll.Margin = new Padding(6, 5, 6, 5);
        _btnRoll.Name = "_btnRoll";
        _btnRoll.Size = new Size(283, 144);
        _btnRoll.TabIndex = 0;
        _btnRoll.Text = "掷骰";
        _btnRoll.UseVisualStyleBackColor = false;
        //
        // _btnBuy
        //
        _btnBuy.BackColor = Color.FromArgb(125, 51, 27);
        _btnBuy.Dock = DockStyle.Fill;
        _btnBuy.FlatStyle = FlatStyle.Flat;
        _btnBuy.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
        _btnBuy.ForeColor = Color.White;
        _btnBuy.Location = new Point(301, 5);
        _btnBuy.Margin = new Padding(6, 5, 6, 5);
        _btnBuy.Name = "_btnBuy";
        _btnBuy.Size = new Size(283, 144);
        _btnBuy.TabIndex = 1;
        _btnBuy.Text = "购买";
        _btnBuy.UseVisualStyleBackColor = false;
        //
        // _btnEnd
        //
        _btnEnd.BackColor = Color.FromArgb(125, 51, 27);
        _btnEnd.Dock = DockStyle.Fill;
        _btnEnd.FlatStyle = FlatStyle.Flat;
        _btnEnd.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
        _btnEnd.ForeColor = Color.White;
        _btnEnd.Location = new Point(596, 5);
        _btnEnd.Margin = new Padding(6, 5, 6, 5);
        _btnEnd.Name = "_btnEnd";
        _btnEnd.Size = new Size(284, 144);
        _btnEnd.TabIndex = 2;
        _btnEnd.Text = "结束";
        _btnEnd.UseVisualStyleBackColor = false;
        //
        // designerPlayersGroup
        //
        designerPlayersGroup.Controls.Add(_playerCards);
        designerPlayersGroup.Dock = DockStyle.Fill;
        designerPlayersGroup.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerPlayersGroup.Location = new Point(22, 465);
        designerPlayersGroup.Margin = new Padding(6, 5, 6, 5);
        designerPlayersGroup.Name = "designerPlayersGroup";
        designerPlayersGroup.Padding = new Padding(6, 5, 6, 5);
        designerPlayersGroup.Size = new Size(1090, 424);
        designerPlayersGroup.TabIndex = 2;
        designerPlayersGroup.TabStop = false;
        designerPlayersGroup.Text = "玩家状态";
        //
        // _playerCards
        //
        _playerCards.AutoScroll = true;
        _playerCards.BackColor = Color.FromArgb(245, 236, 207);
        _playerCards.Dock = DockStyle.Fill;
        _playerCards.FlowDirection = FlowDirection.TopDown;
        _playerCards.Location = new Point(6, 46);
        _playerCards.Margin = new Padding(6, 5, 6, 5);
        _playerCards.Name = "_playerCards";
        _playerCards.Size = new Size(1078, 373);
        _playerCards.TabIndex = 0;
        _playerCards.WrapContents = false;
        //
        // designerChatGroup
        //
        designerChatGroup.Controls.Add(designerChatLayout);
        designerChatGroup.Dock = DockStyle.Fill;
        designerChatGroup.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerChatGroup.Location = new Point(22, 899);
        designerChatGroup.Margin = new Padding(6, 5, 6, 5);
        designerChatGroup.Name = "designerChatGroup";
        designerChatGroup.Padding = new Padding(6, 5, 6, 5);
        designerChatGroup.Size = new Size(1090, 683);
        designerChatGroup.TabIndex = 3;
        designerChatGroup.TabStop = false;
        designerChatGroup.Text = "聊天互动";
        //
        // designerChatLayout
        //
        designerChatLayout.BackColor = Color.FromArgb(245, 236, 207);
        designerChatLayout.ColumnCount = 1;
        designerChatLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerChatLayout.Controls.Add(_chatMessages, 0, 0);
        designerChatLayout.Controls.Add(designerChatInputRow, 0, 1);
        designerChatLayout.Controls.Add(designerQuickChatButtons, 0, 2);
        designerChatLayout.Dock = DockStyle.Fill;
        designerChatLayout.Location = new Point(6, 46);
        designerChatLayout.Margin = new Padding(6, 5, 6, 5);
        designerChatLayout.Name = "designerChatLayout";
        designerChatLayout.RowCount = 3;
        designerChatLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerChatLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 91F));
        designerChatLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 204F));
        designerChatLayout.Size = new Size(1078, 632);
        designerChatLayout.TabIndex = 0;
        //
        // _chatMessages
        //
        _chatMessages.AutoScroll = true;
        _chatMessages.BackColor = Color.FromArgb(252, 248, 235);
        _chatMessages.Dock = DockStyle.Fill;
        _chatMessages.FlowDirection = FlowDirection.TopDown;
        _chatMessages.Location = new Point(6, 5);
        _chatMessages.Margin = new Padding(6, 5, 6, 5);
        _chatMessages.Name = "_chatMessages";
        _chatMessages.Size = new Size(1066, 327);
        _chatMessages.TabIndex = 0;
        _chatMessages.WrapContents = false;
        //
        // designerChatInputRow
        //
        designerChatInputRow.ColumnCount = 2;
        designerChatInputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerChatInputRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 172F));
        designerChatInputRow.Controls.Add(_txtChat, 0, 0);
        designerChatInputRow.Controls.Add(designerSendChatButton, 1, 0);
        designerChatInputRow.Dock = DockStyle.Fill;
        designerChatInputRow.Location = new Point(6, 342);
        designerChatInputRow.Margin = new Padding(6, 5, 6, 5);
        designerChatInputRow.Name = "designerChatInputRow";
        designerChatInputRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerChatInputRow.Size = new Size(1066, 81);
        designerChatInputRow.TabIndex = 1;
        //
        // _txtChat
        //
        _txtChat.Dock = DockStyle.Fill;
        _txtChat.Location = new Point(6, 5);
        _txtChat.Margin = new Padding(6, 5, 6, 5);
        _txtChat.MaxLength = 80;
        _txtChat.Name = "_txtChat";
        _txtChat.Size = new Size(882, 48);
        _txtChat.TabIndex = 0;
        //
        // designerSendChatButton
        //
        designerSendChatButton.BackColor = Color.FromArgb(125, 51, 27);
        designerSendChatButton.Dock = DockStyle.Fill;
        designerSendChatButton.FlatStyle = FlatStyle.Flat;
        designerSendChatButton.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
        designerSendChatButton.ForeColor = Color.White;
        designerSendChatButton.Location = new Point(900, 5);
        designerSendChatButton.Margin = new Padding(6, 5, 6, 5);
        designerSendChatButton.Name = "designerSendChatButton";
        designerSendChatButton.Size = new Size(160, 71);
        designerSendChatButton.TabIndex = 1;
        designerSendChatButton.Text = "发送";
        designerSendChatButton.UseVisualStyleBackColor = false;
        //
        // designerQuickChatButtons
        //
        designerQuickChatButtons.Controls.Add(designerQuickWaitButton);
        designerQuickChatButtons.Controls.Add(designerQuickDuelButton);
        designerQuickChatButtons.Controls.Add(designerQuickFastButton);
        designerQuickChatButtons.Controls.Add(designerQuickSteadyButton);
        designerQuickChatButtons.Controls.Add(designerEggButton);
        designerQuickChatButtons.Controls.Add(designerFlowerButton);
        designerQuickChatButtons.Controls.Add(designerCheerButton);
        designerQuickChatButtons.Controls.Add(designerVoiceButton);
        designerQuickChatButtons.Dock = DockStyle.Fill;
        designerQuickChatButtons.Location = new Point(6, 433);
        designerQuickChatButtons.Margin = new Padding(6, 5, 6, 5);
        designerQuickChatButtons.Name = "designerQuickChatButtons";
        designerQuickChatButtons.Size = new Size(1066, 194);
        designerQuickChatButtons.TabIndex = 2;
        //
        // designerQuickWaitButton
        //
        designerQuickWaitButton.Location = new Point(6, 5);
        designerQuickWaitButton.Margin = new Padding(6, 5, 6, 5);
        designerQuickWaitButton.Name = "designerQuickWaitButton";
        designerQuickWaitButton.Size = new Size(150, 42);
        designerQuickWaitButton.TabIndex = 0;
        designerQuickWaitButton.Text = "等你";
        //
        // designerQuickDuelButton
        //
        designerQuickDuelButton.Location = new Point(168, 5);
        designerQuickDuelButton.Margin = new Padding(6, 5, 6, 5);
        designerQuickDuelButton.Name = "designerQuickDuelButton";
        designerQuickDuelButton.Size = new Size(150, 42);
        designerQuickDuelButton.TabIndex = 1;
        designerQuickDuelButton.Text = "决战";
        //
        // designerQuickFastButton
        //
        designerQuickFastButton.Location = new Point(330, 5);
        designerQuickFastButton.Margin = new Padding(6, 5, 6, 5);
        designerQuickFastButton.Name = "designerQuickFastButton";
        designerQuickFastButton.Size = new Size(150, 42);
        designerQuickFastButton.TabIndex = 2;
        designerQuickFastButton.Text = "快点吧";
        //
        // designerQuickSteadyButton
        //
        designerQuickSteadyButton.Location = new Point(492, 5);
        designerQuickSteadyButton.Margin = new Padding(6, 5, 6, 5);
        designerQuickSteadyButton.Name = "designerQuickSteadyButton";
        designerQuickSteadyButton.Size = new Size(150, 42);
        designerQuickSteadyButton.TabIndex = 3;
        designerQuickSteadyButton.Text = "稳了";
        //
        // designerEggButton
        //
        designerEggButton.Location = new Point(654, 5);
        designerEggButton.Margin = new Padding(6, 5, 6, 5);
        designerEggButton.Name = "designerEggButton";
        designerEggButton.Size = new Size(150, 42);
        designerEggButton.TabIndex = 4;
        designerEggButton.Text = "鸡蛋";
        //
        // designerFlowerButton
        //
        designerFlowerButton.Location = new Point(816, 5);
        designerFlowerButton.Margin = new Padding(6, 5, 6, 5);
        designerFlowerButton.Name = "designerFlowerButton";
        designerFlowerButton.Size = new Size(150, 42);
        designerFlowerButton.TabIndex = 5;
        designerFlowerButton.Text = "鲜花";
        //
        // designerCheerButton
        //
        designerCheerButton.Location = new Point(6, 57);
        designerCheerButton.Margin = new Padding(6, 5, 6, 5);
        designerCheerButton.Name = "designerCheerButton";
        designerCheerButton.Size = new Size(150, 42);
        designerCheerButton.TabIndex = 6;
        designerCheerButton.Text = "喝彩";
        //
        // designerVoiceButton
        //
        designerVoiceButton.Enabled = false;
        designerVoiceButton.Location = new Point(168, 57);
        designerVoiceButton.Margin = new Padding(6, 5, 6, 5);
        designerVoiceButton.Name = "designerVoiceButton";
        designerVoiceButton.Size = new Size(150, 42);
        designerVoiceButton.TabIndex = 7;
        designerVoiceButton.Text = "语音";
        //
        // designerDetailsGroup
        //
        designerDetailsGroup.Controls.Add(designerDetailsTabs);
        designerDetailsGroup.Dock = DockStyle.Fill;
        designerDetailsGroup.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        designerDetailsGroup.Location = new Point(22, 1592);
        designerDetailsGroup.Margin = new Padding(6, 5, 6, 5);
        designerDetailsGroup.Name = "designerDetailsGroup";
        designerDetailsGroup.Padding = new Padding(6, 5, 6, 5);
        designerDetailsGroup.Size = new Size(1090, 485);
        designerDetailsGroup.TabIndex = 4;
        designerDetailsGroup.TabStop = false;
        designerDetailsGroup.Text = "详情";
        //
        // designerDetailsTabs
        //
        designerDetailsTabs.Controls.Add(designerPropertyTab);
        designerDetailsTabs.Controls.Add(designerLogTab);
        designerDetailsTabs.Dock = DockStyle.Fill;
        designerDetailsTabs.Location = new Point(6, 46);
        designerDetailsTabs.Margin = new Padding(6, 5, 6, 5);
        designerDetailsTabs.Name = "designerDetailsTabs";
        designerDetailsTabs.SelectedIndex = 0;
        designerDetailsTabs.Size = new Size(1078, 434);
        designerDetailsTabs.TabIndex = 0;
        //
        // designerPropertyTab
        //
        designerPropertyTab.Controls.Add(_propertyGrid);
        designerPropertyTab.Location = new Point(8, 56);
        designerPropertyTab.Margin = new Padding(6, 5, 6, 5);
        designerPropertyTab.Name = "designerPropertyTab";
        designerPropertyTab.Size = new Size(1062, 370);
        designerPropertyTab.TabIndex = 0;
        designerPropertyTab.Text = "地产";
        //
        // _propertyGrid
        //
        _propertyGrid.AllowUserToAddRows = false;
        _propertyGrid.AllowUserToDeleteRows = false;
        _propertyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _propertyGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _propertyGrid.BorderStyle = BorderStyle.None;
        _propertyGrid.ColumnHeadersHeight = 46;
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.EnableHeadersVisualStyles = false;
        _propertyGrid.GridColor = Color.FromArgb(224, 210, 171);
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Margin = new Padding(6, 5, 6, 5);
        _propertyGrid.MultiSelect = false;
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.ReadOnly = true;
        _propertyGrid.RowHeadersVisible = false;
        _propertyGrid.RowHeadersWidth = 82;
        _propertyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _propertyGrid.Size = new Size(1062, 370);
        _propertyGrid.TabIndex = 0;
        //
        // designerLogTab
        //
        designerLogTab.Controls.Add(_logText);
        designerLogTab.Location = new Point(8, 56);
        designerLogTab.Margin = new Padding(6, 5, 6, 5);
        designerLogTab.Name = "designerLogTab";
        designerLogTab.Size = new Size(1062, 370);
        designerLogTab.TabIndex = 1;
        designerLogTab.Text = "日志";
        //
        // _logText
        //
        _logText.BackColor = Color.FromArgb(252, 248, 235);
        _logText.BorderStyle = BorderStyle.None;
        _logText.Dock = DockStyle.Fill;
        _logText.Location = new Point(0, 0);
        _logText.Margin = new Padding(6, 5, 6, 5);
        _logText.Multiline = true;
        _logText.Name = "_logText";
        _logText.ReadOnly = true;
        _logText.ScrollBars = ScrollBars.Vertical;
        _logText.Size = new Size(1062, 370);
        _logText.TabIndex = 0;
        //
        // designerManagePage
        //
        designerManagePage.BackColor = Color.FromArgb(238, 224, 184);
        designerManagePage.Controls.Add(designerManageTabs);
        designerManagePage.Location = new Point(8, 52);
        designerManagePage.Margin = new Padding(6, 5, 6, 5);
        designerManagePage.Name = "designerManagePage";
        designerManagePage.Padding = new Padding(20, 18, 20, 18);
        designerManagePage.Size = new Size(2404, 1137);
        designerManagePage.TabIndex = 2;
        designerManagePage.Text = "数据管理";
        //
        // designerManageTabs
        //
        designerManageTabs.Controls.Add(designerMapManageTab);
        designerManageTabs.Controls.Add(designerPropertyManageTab);
        designerManageTabs.Controls.Add(designerEventManageTab);
        designerManageTabs.Dock = DockStyle.Fill;
        designerManageTabs.Location = new Point(20, 18);
        designerManageTabs.Margin = new Padding(6, 5, 6, 5);
        designerManageTabs.Name = "designerManageTabs";
        designerManageTabs.SelectedIndex = 0;
        designerManageTabs.Size = new Size(2364, 1101);
        designerManageTabs.TabIndex = 0;
        //
        // designerMapManageTab
        //
        designerMapManageTab.Controls.Add(designerMapManageLayout);
        designerMapManageTab.Location = new Point(8, 56);
        designerMapManageTab.Margin = new Padding(6, 5, 6, 5);
        designerMapManageTab.Name = "designerMapManageTab";
        designerMapManageTab.Size = new Size(2348, 1037);
        designerMapManageTab.TabIndex = 0;
        designerMapManageTab.Text = "地图格子";
        //
        // designerMapManageLayout
        //
        designerMapManageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        designerMapManageLayout.Controls.Add(designerMapManageToolbar, 0, 0);
        designerMapManageLayout.Controls.Add(_mapGrid, 0, 1);
        designerMapManageLayout.Dock = DockStyle.Fill;
        designerMapManageLayout.Location = new Point(0, 0);
        designerMapManageLayout.Margin = new Padding(6, 5, 6, 5);
        designerMapManageLayout.Name = "designerMapManageLayout";
        designerMapManageLayout.RowCount = 2;
        designerMapManageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 109F));
        designerMapManageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerMapManageLayout.Size = new Size(2348, 1037);
        designerMapManageLayout.TabIndex = 0;
        //
        // designerMapManageToolbar
        //
        designerMapManageToolbar.Location = new Point(6, 5);
        designerMapManageToolbar.Margin = new Padding(6, 5, 6, 5);
        designerMapManageToolbar.Name = "designerMapManageToolbar";
        designerMapManageToolbar.Size = new Size(400, 98);
        designerMapManageToolbar.TabIndex = 0;
        //
        // _mapGrid
        //
        _mapGrid.AllowUserToAddRows = false;
        _mapGrid.AllowUserToDeleteRows = false;
        _mapGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _mapGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _mapGrid.BorderStyle = BorderStyle.None;
        _mapGrid.ColumnHeadersHeight = 46;
        _mapGrid.Dock = DockStyle.Fill;
        _mapGrid.EnableHeadersVisualStyles = false;
        _mapGrid.GridColor = Color.FromArgb(224, 210, 171);
        _mapGrid.Location = new Point(6, 114);
        _mapGrid.Margin = new Padding(6, 5, 6, 5);
        _mapGrid.MultiSelect = false;
        _mapGrid.Name = "_mapGrid";
        _mapGrid.RowHeadersVisible = false;
        _mapGrid.RowHeadersWidth = 82;
        _mapGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _mapGrid.Size = new Size(2336, 918);
        _mapGrid.TabIndex = 1;
        //
        // designerPropertyManageTab
        //
        designerPropertyManageTab.Controls.Add(designerPropertyManageLayout);
        designerPropertyManageTab.Location = new Point(8, 45);
        designerPropertyManageTab.Margin = new Padding(6, 5, 6, 5);
        designerPropertyManageTab.Name = "designerPropertyManageTab";
        designerPropertyManageTab.Size = new Size(2348, 1048);
        designerPropertyManageTab.TabIndex = 1;
        designerPropertyManageTab.Text = "地产配置";
        //
        // designerPropertyManageLayout
        //
        designerPropertyManageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        designerPropertyManageLayout.Controls.Add(designerPropertyManageToolbar, 0, 0);
        designerPropertyManageLayout.Controls.Add(_managePropertyGrid, 0, 1);
        designerPropertyManageLayout.Dock = DockStyle.Fill;
        designerPropertyManageLayout.Location = new Point(0, 0);
        designerPropertyManageLayout.Margin = new Padding(6, 5, 6, 5);
        designerPropertyManageLayout.Name = "designerPropertyManageLayout";
        designerPropertyManageLayout.RowCount = 2;
        designerPropertyManageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 109F));
        designerPropertyManageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerPropertyManageLayout.Size = new Size(2348, 1048);
        designerPropertyManageLayout.TabIndex = 0;
        //
        // designerPropertyManageToolbar
        //
        designerPropertyManageToolbar.Location = new Point(6, 5);
        designerPropertyManageToolbar.Margin = new Padding(6, 5, 6, 5);
        designerPropertyManageToolbar.Name = "designerPropertyManageToolbar";
        designerPropertyManageToolbar.Size = new Size(332, 98);
        designerPropertyManageToolbar.TabIndex = 0;
        //
        // _managePropertyGrid
        //
        _managePropertyGrid.AllowUserToAddRows = false;
        _managePropertyGrid.AllowUserToDeleteRows = false;
        _managePropertyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _managePropertyGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _managePropertyGrid.BorderStyle = BorderStyle.None;
        _managePropertyGrid.ColumnHeadersHeight = 46;
        _managePropertyGrid.Dock = DockStyle.Fill;
        _managePropertyGrid.EnableHeadersVisualStyles = false;
        _managePropertyGrid.GridColor = Color.FromArgb(224, 210, 171);
        _managePropertyGrid.Location = new Point(6, 114);
        _managePropertyGrid.Margin = new Padding(6, 5, 6, 5);
        _managePropertyGrid.MultiSelect = false;
        _managePropertyGrid.Name = "_managePropertyGrid";
        _managePropertyGrid.RowHeadersVisible = false;
        _managePropertyGrid.RowHeadersWidth = 82;
        _managePropertyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _managePropertyGrid.Size = new Size(2336, 929);
        _managePropertyGrid.TabIndex = 1;
        //
        // designerEventManageTab
        //
        designerEventManageTab.Controls.Add(designerEventManageLayout);
        designerEventManageTab.Location = new Point(8, 45);
        designerEventManageTab.Margin = new Padding(6, 5, 6, 5);
        designerEventManageTab.Name = "designerEventManageTab";
        designerEventManageTab.Size = new Size(2348, 1048);
        designerEventManageTab.TabIndex = 2;
        designerEventManageTab.Text = "事件卡";
        //
        // designerEventManageLayout
        //
        designerEventManageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        designerEventManageLayout.Controls.Add(designerEventManageToolbar, 0, 0);
        designerEventManageLayout.Controls.Add(_eventGrid, 0, 1);
        designerEventManageLayout.Dock = DockStyle.Fill;
        designerEventManageLayout.Location = new Point(0, 0);
        designerEventManageLayout.Margin = new Padding(6, 5, 6, 5);
        designerEventManageLayout.Name = "designerEventManageLayout";
        designerEventManageLayout.RowCount = 2;
        designerEventManageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 109F));
        designerEventManageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerEventManageLayout.Size = new Size(2348, 1048);
        designerEventManageLayout.TabIndex = 0;
        //
        // designerEventManageToolbar
        //
        designerEventManageToolbar.Location = new Point(6, 5);
        designerEventManageToolbar.Margin = new Padding(6, 5, 6, 5);
        designerEventManageToolbar.Name = "designerEventManageToolbar";
        designerEventManageToolbar.Size = new Size(332, 98);
        designerEventManageToolbar.TabIndex = 0;
        //
        // _eventGrid
        //
        _eventGrid.AllowUserToAddRows = false;
        _eventGrid.AllowUserToDeleteRows = false;
        _eventGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _eventGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _eventGrid.BorderStyle = BorderStyle.None;
        _eventGrid.ColumnHeadersHeight = 46;
        _eventGrid.Dock = DockStyle.Fill;
        _eventGrid.EnableHeadersVisualStyles = false;
        _eventGrid.GridColor = Color.FromArgb(224, 210, 171);
        _eventGrid.Location = new Point(6, 114);
        _eventGrid.Margin = new Padding(6, 5, 6, 5);
        _eventGrid.MultiSelect = false;
        _eventGrid.Name = "_eventGrid";
        _eventGrid.RowHeadersVisible = false;
        _eventGrid.RowHeadersWidth = 82;
        _eventGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _eventGrid.Size = new Size(2336, 929);
        _eventGrid.TabIndex = 1;
        //
        // designerRankPage
        //
        designerRankPage.BackColor = Color.FromArgb(238, 224, 184);
        designerRankPage.Controls.Add(designerRankLayout);
        designerRankPage.Location = new Point(8, 52);
        designerRankPage.Margin = new Padding(6, 5, 6, 5);
        designerRankPage.Name = "designerRankPage";
        designerRankPage.Padding = new Padding(20, 18, 20, 18);
        designerRankPage.Size = new Size(2404, 1137);
        designerRankPage.TabIndex = 3;
        designerRankPage.Text = "排行榜与历史";
        //
        // designerRankLayout
        //
        designerRankLayout.ColumnCount = 2;
        designerRankLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        designerRankLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        designerRankLayout.Controls.Add(designerRankToolbar, 0, 0);
        designerRankLayout.Controls.Add(designerRankGroup, 0, 1);
        designerRankLayout.Controls.Add(designerHistoryGroup, 1, 1);
        designerRankLayout.Dock = DockStyle.Fill;
        designerRankLayout.Location = new Point(20, 18);
        designerRankLayout.Margin = new Padding(6, 5, 6, 5);
        designerRankLayout.Name = "designerRankLayout";
        designerRankLayout.RowCount = 2;
        designerRankLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 109F));
        designerRankLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        designerRankLayout.Size = new Size(2364, 1101);
        designerRankLayout.TabIndex = 0;
        //
        // designerRankToolbar
        //
        designerRankLayout.SetColumnSpan(designerRankToolbar, 2);
        designerRankToolbar.Location = new Point(6, 5);
        designerRankToolbar.Margin = new Padding(6, 5, 6, 5);
        designerRankToolbar.Name = "designerRankToolbar";
        designerRankToolbar.Size = new Size(348, 98);
        designerRankToolbar.TabIndex = 0;
        //
        // designerRankGroup
        //
        designerRankGroup.Controls.Add(_rankGrid);
        designerRankGroup.Dock = DockStyle.Fill;
        designerRankGroup.Location = new Point(6, 114);
        designerRankGroup.Margin = new Padding(6, 5, 6, 5);
        designerRankGroup.Name = "designerRankGroup";
        designerRankGroup.Padding = new Padding(6, 5, 6, 5);
        designerRankGroup.Size = new Size(1170, 982);
        designerRankGroup.TabIndex = 1;
        designerRankGroup.TabStop = false;
        designerRankGroup.Text = "排行榜";
        //
        // _rankGrid
        //
        _rankGrid.AllowUserToAddRows = false;
        _rankGrid.AllowUserToDeleteRows = false;
        _rankGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _rankGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _rankGrid.BorderStyle = BorderStyle.None;
        _rankGrid.ColumnHeadersHeight = 46;
        _rankGrid.Dock = DockStyle.Fill;
        _rankGrid.EnableHeadersVisualStyles = false;
        _rankGrid.GridColor = Color.FromArgb(224, 210, 171);
        _rankGrid.Location = new Point(6, 46);
        _rankGrid.Margin = new Padding(6, 5, 6, 5);
        _rankGrid.MultiSelect = false;
        _rankGrid.Name = "_rankGrid";
        _rankGrid.ReadOnly = true;
        _rankGrid.RowHeadersVisible = false;
        _rankGrid.RowHeadersWidth = 82;
        _rankGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _rankGrid.Size = new Size(1158, 931);
        _rankGrid.TabIndex = 0;
        //
        // designerHistoryGroup
        //
        designerHistoryGroup.Controls.Add(_historyGrid);
        designerHistoryGroup.Dock = DockStyle.Fill;
        designerHistoryGroup.Location = new Point(1188, 114);
        designerHistoryGroup.Margin = new Padding(6, 5, 6, 5);
        designerHistoryGroup.Name = "designerHistoryGroup";
        designerHistoryGroup.Padding = new Padding(6, 5, 6, 5);
        designerHistoryGroup.Size = new Size(1170, 982);
        designerHistoryGroup.TabIndex = 2;
        designerHistoryGroup.TabStop = false;
        designerHistoryGroup.Text = "历史对局";
        //
        // _historyGrid
        //
        _historyGrid.AllowUserToAddRows = false;
        _historyGrid.AllowUserToDeleteRows = false;
        _historyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _historyGrid.BackgroundColor = Color.FromArgb(252, 248, 235);
        _historyGrid.BorderStyle = BorderStyle.None;
        _historyGrid.ColumnHeadersHeight = 46;
        _historyGrid.Dock = DockStyle.Fill;
        _historyGrid.EnableHeadersVisualStyles = false;
        _historyGrid.GridColor = Color.FromArgb(224, 210, 171);
        _historyGrid.Location = new Point(6, 46);
        _historyGrid.Margin = new Padding(6, 5, 6, 5);
        _historyGrid.MultiSelect = false;
        _historyGrid.Name = "_historyGrid";
        _historyGrid.ReadOnly = true;
        _historyGrid.RowHeadersVisible = false;
        _historyGrid.RowHeadersWidth = 82;
        _historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _historyGrid.Size = new Size(1158, 931);
        _historyGrid.TabIndex = 0;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(14F, 31F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(31, 54, 42);
        ClientSize = new Size(2480, 1495);
        Controls.Add(_mainShell);
        Controls.Add(_loginPage);
        Font = new Font("Microsoft YaHei UI", 9F);
        Margin = new Padding(6, 5, 6, 5);
        MinimumSize = new Size(2454, 1437);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "河南文旅大富翁";
        _loginPage.ResumeLayout(false);
        _loginCard.ResumeLayout(false);
        _loginLayout.ResumeLayout(false);
        _loginLayout.PerformLayout();
        designerServerRow.ResumeLayout(false);
        designerServerRow.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_numPort).EndInit();
        ((System.ComponentModel.ISupportInitialize)_numMaxPlayers).EndInit();
        designerLoginBottom.ResumeLayout(false);
        _loginButtons.ResumeLayout(false);
        _mainShell.ResumeLayout(false);
        designerMainRoot.ResumeLayout(false);
        designerHeader.ResumeLayout(false);
        designerIdentityRow.ResumeLayout(false);
        designerIdentityRow.PerformLayout();
        designerActionRow.ResumeLayout(false);
        _tabs.ResumeLayout(false);
        designerLobbyPage.ResumeLayout(false);
        designerLobbySplit.Panel1.ResumeLayout(false);
        designerLobbySplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)designerLobbySplit).EndInit();
        designerLobbySplit.ResumeLayout(false);
        designerLobbyLeft.ResumeLayout(false);
        designerRoomActions.ResumeLayout(false);
        designerLobbyToolbar.ResumeLayout(false);
        designerRoomListPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_roomGrid).EndInit();
        designerGamePage.ResumeLayout(false);
        designerGameLayout.ResumeLayout(false);
        designerBoardHost.ResumeLayout(false);
        designerSideScroll.ResumeLayout(false);
        designerSidePanel.ResumeLayout(false);
        designerTurnPanel.ResumeLayout(false);
        designerGameControlsPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_dicePicture).EndInit();
        designerGameActionButtons.ResumeLayout(false);
        designerPlayersGroup.ResumeLayout(false);
        designerChatGroup.ResumeLayout(false);
        designerChatLayout.ResumeLayout(false);
        designerChatInputRow.ResumeLayout(false);
        designerChatInputRow.PerformLayout();
        designerQuickChatButtons.ResumeLayout(false);
        designerDetailsGroup.ResumeLayout(false);
        designerDetailsTabs.ResumeLayout(false);
        designerPropertyTab.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_propertyGrid).EndInit();
        designerLogTab.ResumeLayout(false);
        designerLogTab.PerformLayout();
        designerManagePage.ResumeLayout(false);
        designerManageTabs.ResumeLayout(false);
        designerMapManageTab.ResumeLayout(false);
        designerMapManageLayout.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_mapGrid).EndInit();
        designerPropertyManageTab.ResumeLayout(false);
        designerPropertyManageLayout.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_managePropertyGrid).EndInit();
        designerEventManageTab.ResumeLayout(false);
        designerEventManageLayout.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_eventGrid).EndInit();
        designerRankPage.ResumeLayout(false);
        designerRankLayout.ResumeLayout(false);
        designerRankGroup.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_rankGrid).EndInit();
        designerHistoryGroup.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_historyGrid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private SolidTextButton _btnRoll;
    private SolidTextButton _btnBuy;
    private SolidTextButton _btnEnd;
    private SolidTextButton designerConnectButton;
    private SolidTextButton designerRegisterButton;
    private SolidTextButton designerEnterLobbyButton;
    private SolidTextButton designerSelectTokenButton;
    private SolidTextButton designerRefreshRoomsButton;
    private SolidTextButton designerBackLoginButton;
    private SolidTextButton designerCreateRoomButton;
    private SolidTextButton designerJoinRoomButton;
    private SolidTextButton designerLeaveRoomButton;
    private SolidTextButton designerReadyButton;
    private SolidTextButton designerSendChatButton;
    private SolidTextButton designerQuickWaitButton;
    private SolidTextButton designerQuickDuelButton;
    private SolidTextButton designerQuickFastButton;
    private SolidTextButton designerQuickSteadyButton;
    private SolidTextButton designerEggButton;
    private SolidTextButton designerFlowerButton;
    private SolidTextButton designerCheerButton;
    private SolidTextButton designerVoiceButton;
}
