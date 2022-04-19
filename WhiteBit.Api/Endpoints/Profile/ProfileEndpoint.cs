using System.Threading.Tasks;
using WhiteBit.Api.Utilities;

namespace WhiteBit.Api.Endpoints.Profile;

internal class ProfileEndpoint : IProfileEndpoint
{
    private readonly JsonFormatter _formatter;
    private readonly string _path;

    public ProfileEndpoint(string path, JsonFormatter formatter)
    {
        _formatter = formatter;
        _path = path;
    }

    public async Task<string> WebsocketToken()
    {
        return await _formatter.GetStringAsync(_path, null);
    }
}