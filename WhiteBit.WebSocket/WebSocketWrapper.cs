using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteBit.WebSocket;

public class WebSocketWrapper : IDisposable
{
    private readonly ClientWebSocket _webSocket;
    public Action<string>? Notify;
    private readonly Uri _uri;
    public int CountRequest { get; private set; }

    public WebSocketWrapper(Uri uri)
    {
        _uri = uri;
        _webSocket = new ClientWebSocket();
    }
    public WebSocketWrapper(Uri uri,Action<string> action):this(uri)
    {
        Notify = action;
    }

    public async Task StartListenAsync()
    {
        await _webSocket.ConnectAsync(_uri, CancellationToken.None);
        await Task.Run(() => RunListenerAsync().ConfigureAwait(false));
    }

    public async Task SendMessageAsync(string message)
    {
        CountRequest++;
        var encoded = Encoding.UTF8.GetBytes(message);
        var requestBuffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
        await _webSocket.SendAsync(requestBuffer, WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private async Task RunListenerAsync()
    {
        var buffer = new byte[8048];
        var receiveBuffer = new Memory<byte>(buffer);
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing",
                    CancellationToken.None);
            }
            else
            {
                var message = GetMessage(buffer, result.Count);
                Notify?.Invoke(message);
                var test = Notify?.GetInvocationList();
            }
        }
    }

    private static string GetMessage(byte[] buffer, int count)
    {
        var message = Encoding.UTF8.GetString(buffer);
        return message.Remove(count, buffer.Length - count);
    }

    public void Dispose()
    {
        var list = Notify?.GetInvocationList();
        if (list != null)
            foreach (var handler in list)
            {
                Notify -= (handler as Action<string>);
            }

        _webSocket.Dispose();
    }
}