using MonopolyModels.Data;
using MonopolyServer.Networking;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var ip = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 && int.TryParse(args[1], out var parsedPort) ? parsedPort : 9000;
var dbPath = args.Length > 2 ? args[2] : DbPaths.GetDefaultDatabasePath();

DatabaseBootstrapper.EnsureCreatedAndSeeded(dbPath);

var server = new GameTcpServer(dbPath);
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await server.StartAsync(ip, port, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("服务器已停止");
}
