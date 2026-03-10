using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Dtos;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.API;

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

       var url = new Url{
           Id= Guid.NewGuid(),
           OriginUrl = request.Url,
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
}