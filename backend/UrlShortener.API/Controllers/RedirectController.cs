
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.API.Controllers;

[ApiController]
public class RedirectController : ControllerBase
{
    public readonly  UrlShortenerDbContext dbContext;
    public RedirectController(UrlShortenerDbContext dbContext )
    {
        this.dbContext = dbContext;
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string code)
    {
        var url = dbContext.Urls.FirstOrDefault(url => url.ShortCode == code);
        
        if(url == null) return NotFound();

        return Redirect(url.OriginUrl);

    }
}