using System;

namespace UrlShortener.Application.Dtos;

public class CreateShortUrlRequestDto
{
    public string Url { get; set; } = String.Empty;
}