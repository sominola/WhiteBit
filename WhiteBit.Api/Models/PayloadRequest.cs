namespace WhiteBit.Api.Models;

internal record struct PayloadRequest(string Request, string Nonce, object? Data);