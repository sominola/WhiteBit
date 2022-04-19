using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhiteBit.WebSocket.Enums;
using WhiteBit.WebSocket.Models;

namespace WhiteBit.WebSocket;

public class WhiteBitWebSocket : IDisposable
{
    private int CountRequest => _webSocket.CountRequest;
    private readonly WebSocketWrapper _webSocket;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<Tickers, Action<decimal>> _lastPriceCallbacks;

    public WhiteBitWebSocket()
    {
        var action = new Action<string>(OnMessageReceived);
        _webSocket = new WebSocketWrapper(new Uri("wss://api.whitebit.com/ws"),action);
        _lastPriceCallbacks = new Dictionary<Tickers, Action<decimal>>();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private void OnMessageReceived(string message)
    {
        if (message.Contains("lastprice_update"))
            LastPriceReceived(message);
    }
   
    private void LastPriceReceived(string message)
    {
        var json = JsonSerializer.Deserialize<ListenPrice>(message, _jsonOptions);
        var prices = GetPrices(json);
        foreach (var value in _lastPriceCallbacks)
        {
            foreach (var price in prices)
            {
                if (value.Key.ToString() == price.Key)
                {
                    value.Value.Invoke(price.Value);
                }
            }
        }
    }

    public async Task StartListen()
    {
        await _webSocket.StartListenAsync();
    }

    private async Task Authorize(string token)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));

        var test = new
        {
            id = CountRequest,
            method = "authorize",
            @params = new[]
            {
                token,
                "public"
            }
        };
        await SendJson(test);
    }

    public async Task ListenLastPrice(Tickers ticker, Action<decimal> callback)
    {
        if (_lastPriceCallbacks.ContainsKey(ticker))
            return;

        _lastPriceCallbacks.Add(ticker, callback);
        var array = _lastPriceCallbacks.Keys.Select(x => x.ToString()).ToArray();
        var json = new
        {
            id = CountRequest,
            method = "lastprice_subscribe",
            @params = array
        };

        await SendJson(json);
    }

    public async Task UnListenLastPrice(Tickers ticker)
    {
        _lastPriceCallbacks.Remove(ticker);
        var json = new
        {
            id = CountRequest,
            method = "lastprice_unsubscribe",
            @params = new[]
            {
                ""
            }
        };
        await SendJson(json);
        await ResubscribePrices();
    }

    private async Task ResubscribePrices()
    {
        var array = _lastPriceCallbacks.Keys.Select(x => x.ToString()).ToArray();
        var json = new
        {
            id = CountRequest,
            method = "lastprice_subscribe",
            @params = array
        };
        await SendJson(json);
    }
    private async Task SendJson(object data)
    {
        var json = JsonSerializer.Serialize(data);
        await _webSocket.SendMessageAsync(json);
    }

    private static Dictionary<string, decimal> GetPrices(ListenPrice? model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        var dictionary = new Dictionary<string, decimal>(model.Params.Count / 2);
        for (var i = 0; i < model.Params.Count; i += 2)
        {
            var currency = model.Params[i] ?? "";
            decimal.TryParse(model.Params[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
            dictionary.Add(currency, value);
        }

        return dictionary;
    }

    public void Dispose()
    {
        _webSocket.Dispose();
    }
}