using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhiteBit.Api.Models;

namespace WhiteBit.Api.Utilities;

internal sealed class RequestHandler : IDisposable
{
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _client;

    private readonly string _apiKey;
    private readonly string _apiSecretKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public RequestHandler(string apiKey, string apiSecretKey, IWebProxy? proxy = null)
    {
        _apiKey = apiKey;
        _apiSecretKey = apiSecretKey;

        _handler = new HttpClientHandler {UseProxy = proxy != null, Proxy = proxy};
        _client = new System.Net.Http.HttpClient(_handler);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    private async Task<HttpResponseMessage> GetResponse(string path, object? data)
    {
        var payloadRequest = new PayloadRequest
        {
            Nonce = GetNonce(),
            Request = path,
            Data = data
        };

        var dataJson = SerializePayload(payloadRequest);
        var payload = Base64Encode(dataJson);
        var signature = CalcSignature(payload, _apiSecretKey);
        var content = new StringContent(dataJson, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{Constants.HostName}{path}")
        {
            Content = content,
        };

        requestMessage.Headers.Add("X-TXC-APIKEY", _apiKey);
        requestMessage.Headers.Add("X-TXC-PAYLOAD", payload);
        requestMessage.Headers.Add("X-TXC-SIGNATURE", signature);

        return await _client.SendAsync(requestMessage);
    }
    public async Task<Stream> GetResponseAsync(string path, object? data)
    {
        var response = await GetResponse(path, data);
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<string> GetStringAsync(string path, object? data)
    {
        var response = await GetResponse(path,data);
        return await response.Content.ReadAsStringAsync();
    }

    private string CalcSignature(string text, string apiSecret)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var stream = new MemoryStream(bytes);

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(apiSecret));
        {
            var hash = hmac.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }

    private string Base64Encode(string text)
    {
        var plaintTextBytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(plaintTextBytes);
    }

    private string GetNonce()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
    }

    private string SerializePayload(PayloadRequest payload)
    {
        var dataJsonStr = JsonSerializer.Serialize(payload, _jsonOptions);
        var str = new StringBuilder(dataJsonStr);
        
        if (str.ToString().Contains("\"data\":{"))
            str.Replace("\"data\":{", string.Empty).Remove(str.Length - 1, 1);
        
        return str.ToString();
    }

    #region Disposable

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _handler?.Dispose();
            _client?.Dispose();
        }

        _disposed = true;
    }

    ~RequestHandler()
    {
        Dispose(false);
    }

    #endregion
}