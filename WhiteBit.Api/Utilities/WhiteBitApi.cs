using System;
using System.Net;
using WhiteBit.Api.Endpoints.Profile;
using WhiteBit.Api.Endpoints.TradeAccount;

// ReSharper disable MemberCanBePrivate.Global

namespace WhiteBit.Api.Utilities;

public class WhiteBitApi: IDisposable
{
    private readonly RequestHandler _request;
    private readonly JsonFormatter _jsonFormatter;

    public WhiteBitApi(string apiKey,string apiSecretKey, IWebProxy? proxy = null)
    {
        _request = new RequestHandler(apiKey, apiSecretKey,proxy);
        _jsonFormatter = new JsonFormatter(_request);

        TradeTradeAccount = new TradeAccountEndpoint(ConstantPath.Balance, _jsonFormatter);
        Profile = new ProfileEndpoint(ConstantPath.WebsocketToken, _jsonFormatter);
    }


    public readonly ITradeAccountEndpoint TradeTradeAccount;
    public readonly IProfileEndpoint Profile;

    public void Dispose()
    {
        _request.Dispose();
        _jsonFormatter.Dispose();
    }
}