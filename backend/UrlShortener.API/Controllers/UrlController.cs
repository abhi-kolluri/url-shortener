using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Dtos;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/url")]
public class UrlController : ControllerBase
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly IShortCodeGenerator _generator;
    public UrlController(UrlShortenerDbContext dbContext, IShortCodeGenerator generator)
    {
        this._dbContext = dbContext;
        this._generator = generator;
    }
    [HttpPost]
    public async Task<ActionResult<CreateShortUrlResponseDto>> CreateShortUrl(CreateShortUrlRequestDto request)
    {

       var code = _generator.GenerateShortCode();

       var normalizeUrl = NormalizeUrl(request.Url);
       var isValidUrl = CheckIfValid(normalizeUrl);
       if (!isValidUrl)
       {
           await Console.Error.WriteLineAsync("Invalid url:" + normalizeUrl);
           return BadRequest();
       }
       
       
       var url = new Url{
           Id= Guid.NewGuid(),
           OriginUrl = normalizeUrl,
           ShortCode = code,
           CreatedAt = DateTime.UtcNow,
           ExpiredAt = DateTime.UtcNow.AddMinutes(10),
       };
       
       _dbContext.Urls.Add(url);
       await _dbContext.SaveChangesAsync();

       var shortUrl = $"{Request.Scheme}://{Request.Host}/{code}";

       return Ok(new CreateShortUrlResponseDto
       {
           Url = shortUrl
       });
    }

    private string NormalizeUrl(string url)
    {
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "http://" + url;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            string host = uri.Host;

            // 2. Strip "www." from the start of the host
            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                host = host.Substring(4);
            }

            // 3. Reconstruct with http scheme
            // uri.PathAndQuery ensures we keep the rest of the URL (/search, etc.)
            return $"http://{host}{uri.PathAndQuery}";
        }
        return url;
    }

    private bool CheckIfValid(string requestUrl)
    {
        return !string.IsNullOrWhiteSpace(requestUrl) && Uri.IsWellFormedUriString(requestUrl, UriKind.Absolute);
    }
}