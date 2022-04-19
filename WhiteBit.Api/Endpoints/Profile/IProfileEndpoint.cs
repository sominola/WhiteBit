using System.Threading.Tasks;

namespace WhiteBit.Api.Endpoints.Profile;

public interface IProfileEndpoint
{
    Task<string> WebsocketToken();
}