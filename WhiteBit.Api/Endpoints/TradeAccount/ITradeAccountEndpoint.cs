using System.Threading.Tasks;
using WhiteBit.Api.Enums;
using WhiteBit.Api.Models;

namespace WhiteBit.Api.Endpoints.TradeAccount;

public interface ITradeAccountEndpoint
{
    Task<BalanceResponse> Balance(Tickers tickers);
    Task<BalanceResponse> Balance(string tickers);
}