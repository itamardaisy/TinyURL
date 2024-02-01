using Microsoft.AspNetCore.Mvc;
using TinyUrl.Models;
using TinyUrl.Logic;
using TinyUrl.Models.Interfaces;

public class UrlController : ControllerBase
{
    private readonly TinyUrlManager _tinyUrlManager;

    public UrlController(IDbContext dbContext, IShortenUrlService shortenUrlGenerator, ISizeLimitedCache<string, string> sizeLimitedCache)
    {
        _tinyUrlManager = new TinyUrlManager(dbContext, shortenUrlGenerator, sizeLimitedCache);
    }

    [HttpPost("shorten")]
    public IActionResult ShortenUrl([FromBody] UrlMapping urlMapping)
    {
        ShortenUrlResult res = _tinyUrlManager.ShortenUrl(urlMapping);
        if (!res.Status)
        {
            return BadRequest("Invalid input");
        }
        else
        {
            return Ok(res.ShortenUrl);
        }
    }

    [HttpGet("{shortUrl}")]
    public IActionResult RedirectUrl(string shortUrl)
    {
        string redirectUrl = _tinyUrlManager.RedirectUrl(shortUrl);

        if (redirectUrl != string.Empty)
        {
            return Redirect(redirectUrl);
        }

        return NotFound();
    }
}
