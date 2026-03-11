using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.BackgroundJobs;

public class ExpiredUrlCleanUpJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredUrlCleanUpJob> _logger;
    private readonly IConfiguration _configuration;
    public ExpiredUrlCleanUpJob(IServiceScopeFactory scopeFactory, ILogger<ExpiredUrlCleanUpJob> logger, IConfiguration configuration)
    {
        this._scopeFactory = scopeFactory;
        this._logger = logger; 
        this._configuration = configuration;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var bjflag = _configuration.GetValue<Boolean>("BackGroundJobFlag");
            if (bjflag == false)
                return;
           _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await CleanUpUrl();
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task CleanUpUrl()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();

        var expiredUrl = await db.Urls.Where(x => x.ExpiredAt < DateTime.UtcNow).ToListAsync();
        
        _logger.LogInformation("Removing expired urls" + expiredUrl.Count);
        if (expiredUrl.Count > 0)
        {
            db.RemoveRange(expiredUrl);
            await db.SaveChangesAsync();
        }
    }
}