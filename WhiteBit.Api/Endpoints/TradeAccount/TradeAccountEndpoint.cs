using System.Threading.Tasks;
using WhiteBit.Api.Enums;
using WhiteBit.Api.Models;
using WhiteBit.Api.Utilities;

namespace WhiteBit.Api.Endpoints.TradeAccount;

internal class TradeAccountEndpoint:ITradeAccountEndpoint
{
    private readonly JsonFormatter _formatter;
    private readonly string _path;
    
    public TradeAccountEndpoint(string path, JsonFormatter formatter)
    {
        _formatter = formatter;
        _path = path;
    }

    public async Task<BalanceResponse> Balance(Tickers ticker)
    {
        var model = new {Ticker=ticker.ToString()};
        return await _formatter.DeserializeAsync<BalanceResponse>(_path,model);
    }
    public async Task<BalanceResponse> Balance(string ticker)
    {
        var model = new {Ticker = ticker};
        return await _formatter.DeserializeAsync<BalanceResponse>(_path,model);
    }
}