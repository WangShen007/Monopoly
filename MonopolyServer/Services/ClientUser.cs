using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MonopolyModels.Dtos;

namespace MonopolyServer.Services;

public class ClientUser
{
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    public ClientUser(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        var stream = tcpClient.GetStream();
        Reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: false);
        Writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: false) { AutoFlush = true };
    }

    public TcpClient TcpClient { get; }
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string TokenImageFile { get; set; } = "棋子-红.png";
    public int? RoomId { get; set; }
    public bool IsLoggedIn => UserId > 0;

    public async Task SendAsync(NetMessage message)
    {
        var json = JsonSerializer.Serialize(message, JsonProtocol.Options);
        await _sendLock.WaitAsync();
        try
        {
            await Writer.WriteLineAsync(json);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public void Close()
    {
        try
        {
            TcpClient.Close();
        }
        catch
        {
            // Best effort cleanup during disconnect.
        }
    }
}
