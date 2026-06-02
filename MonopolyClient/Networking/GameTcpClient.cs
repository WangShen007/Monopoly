using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MonopolyModels.Dtos;

namespace MonopolyClient.Networking;

public class GameTcpClient : IDisposable
{
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private CancellationTokenSource? _cts;

    public event Action<NetMessage>? MessageReceived;
    public event Action<string>? Disconnected;

    public bool IsConnected => _client?.Connected == true;

    public async Task ConnectAsync(string ip, int port)
    {
        Disconnect();
        _client = new TcpClient();
        await _client.ConnectAsync(ip, port);
        var stream = _client.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: false);
        _writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: false) { AutoFlush = true };
        _cts = new CancellationTokenSource();
        _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));
    }

    public async Task SendAsync<T>(string type, T data)
    {
        if (_writer is null)
        {
            throw new InvalidOperationException("尚未连接服务器");
        }

        var message = NetMessage.Create(type, data);
        var json = JsonSerializer.Serialize(message, JsonProtocol.Options);
        await _sendLock.WaitAsync();
        try
        {
            await _writer.WriteLineAsync(json);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public Task SendAsync(string type)
    {
        return SendAsync(type, new EmptyDto());
    }

    public void Disconnect()
    {
        try
        {
            _cts?.Cancel();
            _client?.Close();
        }
        catch
        {
            // Best effort network cleanup.
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && _reader is not null)
            {
                var line = await _reader.ReadLineAsync(token);
                if (line is null)
                {
                    break;
                }

                var message = JsonSerializer.Deserialize<NetMessage>(line, JsonProtocol.Options);
                if (message is not null)
                {
                    MessageReceived?.Invoke(message);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Disconnected?.Invoke(ex.Message);
        }
    }

    public void Dispose()
    {
        Disconnect();
        _sendLock.Dispose();
        _cts?.Dispose();
    }
}
