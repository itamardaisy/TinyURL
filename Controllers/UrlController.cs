using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TinyUrl.Models;
using TinyUrl.Data;

[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly MongoDbContext _dbContext;

    public UrlController(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("shorten")]
    public IActionResult ShortenUrl([FromBody] UrlMapping urlMapping)
    {
        Console.WriteLine("kakakaka");
        if (urlMapping == null || string.IsNullOrEmpty(urlMapping.LongUrl))
        {
            return BadRequest("Invalid input");
        }

        var existingMapping = _dbContext.UrlMappings.Find(x => x.LongUrl == urlMapping.LongUrl).FirstOrDefault();

        if (existingMapping != null)
        {
            // URL already exists, return the existing short URL
            return Ok(existingMapping.ShortUrl);
        }

        // Generate a short URL (you can use a custom algorithm to make collisions extremely unlikely)
        urlMapping.ShortUrl = GenerateShortUrl();
        _dbContext.UrlMappings.InsertOne(urlMapping);

        return Ok(urlMapping.ShortUrl);
    }

    [HttpGet("redirect/{shortUrl}")]
    public IActionResult RedirectUrl(string shortUrl)
    {
        var urlMapping = _dbContext.UrlMappings.Find(x => x.ShortUrl == shortUrl).FirstOrDefault();

        if (urlMapping != null)
        {
            // Redirect to the original long URL
            return Redirect(urlMapping.LongUrl);
        }

        return NotFound();
    }

    // Custom short URL generation algorithm
    private string GenerateShortUrl()
    {
        // Implement your custom logic here
        // For simplicity, you can use a library like Base64 encoding
        // Make sure to handle collisions if needed
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
    }
}
