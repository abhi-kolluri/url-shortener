using System;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Application.Services;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private static readonly Random Random = new Random();

    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateShortCode(int length = 6)
    {
        var code = new char[length];
        for (var i = 0; i < length; i++)
        {
            code[i] = Chars[Random.Next(Chars.Length)];
        }
        return new string(code);
    }
}