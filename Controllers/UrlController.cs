using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TinyUrl.Models;
using TinyUrl.Data;
using TinyUrl.Services;
using TinyUrl.Models.Interfaces;

[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly IDbContext _dbContext;
    private readonly IShortenUrlService _shortenUrlGenerator;

    public UrlController(IDbContext dbContext, IShortenUrlService shortenUrlGenerator)
    {
        _dbContext = dbContext;
        _shortenUrlGenerator = shortenUrlGenerator;
    }

    [HttpPost("shorten")]
    public IActionResult ShortenUrl([FromBody] UrlMapping urlMapping)
    {
        if (urlMapping == null || string.IsNullOrEmpty(urlMapping.LongUrl))
        {
            return BadRequest("Invalid input");
        }

        var existingMapping = _dbContext.UrlMappings.Find(x => x.LongUrl == urlMapping.LongUrl).FirstOrDefault();

        if (existingMapping != null)
        {
            return Ok(existingMapping.ShortUrl);
        }

        urlMapping.ShortUrl = _shortenUrlGenerator.GenerateShortUrl();
        _dbContext.UrlMappings.InsertOne(urlMapping);

        return Ok(urlMapping.ShortUrl);
    }

    [HttpGet("redirect/{shortUrl}")]
    public IActionResult RedirectUrl(string shortUrl)
    {
        var urlMapping = _dbContext.UrlMappings.Find(x => x.ShortUrl == shortUrl).FirstOrDefault();

        if (urlMapping != null)
        {
            return Redirect(urlMapping.LongUrl);
        }

        return NotFound();
    }
}
