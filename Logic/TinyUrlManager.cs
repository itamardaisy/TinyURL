using MongoDB.Driver;
using TinyUrl.Models;
using TinyUrl.Models.Interfaces;

namespace TinyUrl.Logic
{
    public class TinyUrlManager : ITinyUrlManager
    {
        private readonly IDbContext _dbContext;
        private readonly IShortenUrlService _shortenUrlGenerator;
        private readonly ISizeLimitedCache<string, string> _sizeLimitedCache;

        public TinyUrlManager(IDbContext dbContext, IShortenUrlService shortenUrlGenerator, ISizeLimitedCache<string, string> sizeLimitedCache)
        {
            _dbContext = dbContext;
            _shortenUrlGenerator = shortenUrlGenerator;
            _sizeLimitedCache = sizeLimitedCache;
        }

        public ShortenUrlResult ShortenUrl(UrlMapping urlMapping)
        {
            if (urlMapping == null || string.IsNullOrEmpty(urlMapping.LongUrl))
            {
                return new ShortenUrlResult { Status = true, ShortenUrl = string.Empty };
            }

            var existingMapping = _dbContext.UrlMappings.Find(x => x.LongUrl == urlMapping.LongUrl).FirstOrDefault();

            if (existingMapping != null)
            {
                return new ShortenUrlResult { Status = true, ShortenUrl = existingMapping.ShortUrl };
            }

            urlMapping.ShortUrl ??= _shortenUrlGenerator.GenerateShortUrl();
            _dbContext.UrlMappings.InsertOne(urlMapping);

            return new ShortenUrlResult { Status = true, ShortenUrl = urlMapping.ShortUrl };
        }

        public string RedirectUrl(string shortUrl)
        {
            string shortenUrlInCache = _sizeLimitedCache.Get(shortUrl, null);

            if (shortenUrlInCache != null)
            {
                return shortenUrlInCache;
            }
            else
            {
                var urlMapping = _dbContext.UrlMappings.Find(x => x.ShortUrl == shortUrl).FirstOrDefault();
                if (urlMapping != null)
                {
                    _sizeLimitedCache.Add(urlMapping.ShortUrl, urlMapping.LongUrl);
                    return urlMapping.LongUrl;
                }

                return string.Empty;
            }
        }
    }
}