# 多人回合制地产经营游戏系统

这是 `基于CS架构的多人回合制地产经营游戏系统_实现思路.pdf` 对应的 C# 课程设计实现，项目根目录为 `C:\fu`。

## 项目结构

- `MonopolyModels`：实体类、DTO、状态类、EF Core `DbContext`、SQLite 数据库初始化与种子数据。
- `MonopolyServer`：控制台 TCP 服务端，使用 `TcpListener`、`TcpClient`、`NetworkStream`、`StreamReader`、`StreamWriter` 和 `Task` 处理多客户端。
- `MonopolyClient`：WinForms 客户端，包含连接登录、大厅、游戏地图、DataGridView 数据管理、排行榜和历史记录界面。
- `MonopolyTests`：规则、协议、数据库初始化和对局记录持久化测试。

## 运行方式

1. 构建项目：

   ```powershell
   dotnet build C:\fu\MonopolyGameSolution.slnx
   ```

2. 启动服务端：

   ```powershell
   dotnet run --project C:\fu\MonopolyServer\MonopolyServer.csproj -- 127.0.0.1 9000
   ```

3. 启动两个客户端：

   ```powershell
   dotnet run --project C:\fu\MonopolyClient\MonopolyClient.csproj
   ```

4. 两个客户端使用不同用户名登录，一个创建房间，另一个加入房间，双方点击“准备”后自动开始游戏。

默认 SQLite 数据库位于 `%LOCALAPPDATA%\MonopolyGame\monopoly.db`。也可以通过环境变量 `MONOPOLY_DB_PATH` 指定数据库路径。

## 已覆盖功能

- 2-4 人 TCP 联机。
- 用户注册、登录。
- 创建房间、加入房间、离开房间、准备。
- 服务端统一掷骰、移动、经过起点奖励、购买地产、支付租金、机会事件、税收、破产、最大回合结算。
- 20 格地图与地产、事件卡种子数据。
- EF Core + LINQ + SQLite 数据库持久化。
- 地图格子、地产、事件卡 DataGridView 增删改查。
- 对局记录、玩家成绩、行动记录保存。
- 排行榜和历史对局查询。

## 课程知识点对应

- TCP 通信：`TcpListener`、`TcpClient`、`NetworkStream`。
- 数据流：`StreamReader`、`StreamWriter`，一行一条 JSON 消息解决 TCP 无消息边界问题。
- 多线程/多任务：服务端每个客户端一个后台接收任务，WinForms 客户端后台接收并使用 `BeginInvoke` 回到 UI 线程。
- 数据库：EF Core `DbContext`、实体类、LINQ 查询、`Add`、修改实体、`Remove`、`SaveChanges`。
- DataGridView：客户端管理地图、地产、事件卡，并显示排行榜、历史记录。
