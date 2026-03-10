using System;

namespace UrlShortener.Application.Dtos;

public class CreateShortUrlResponseDto
{
    public string Url { get; set; } = String.Empty;
}