namespace UrlShortener.Domain.Entities;

public class Url
{
    public Guid Id { get; set; }
    
    public string OriginUrl { get; set; } = String.Empty;
    
    public string ShortCode { get; set; } = String.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ExpiredAt { get; set; }
    
    public int ClickCount { get; set; }
}