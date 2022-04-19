using System.Collections.Generic;

namespace WhiteBit.WebSocket.Models;

public class ListenPrice
{
    public int? Id { get; set; }
    public string? Method { get; set; }
    public List<string> Params { get; set; }
}