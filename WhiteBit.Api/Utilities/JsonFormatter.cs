using System;
using System.Text.Json;
using System.Threading.Tasks;
using WhiteBit.Api.Utilities.Converters;

namespace WhiteBit.Api.Utilities;

internal sealed class JsonFormatter : IDisposable
{
    private readonly RequestHandler _request;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFormatter(RequestHandler request)
    {
        _request = request;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _jsonOptions.Converters.Add(new DecimalConverter());
    }

    public async Task<T?> DeserializeAsync<T>(string url, object data)
    {
        var response = await _request.GetResponseAsync(url, data);

        if (response == null)
            throw new NullReferenceException(nameof(response));

        return await JsonSerializer.DeserializeAsync<T>(response, _jsonOptions);
    }

    public async Task<string> GetStringAsync(string url, object data)
    {
        return await _request.GetStringAsync(url, data);
    }

    public void Dispose()
    {
        _request?.Dispose();
    }
}