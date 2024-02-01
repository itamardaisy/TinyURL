namespace TinyUrl.Models.Interfaces 
{
    public interface ITinyUrlManager
    {
        public ShortenUrlResult ShortenUrl(UrlMapping urlMapping);
        public string RedirectUrl(string shortUrl);
    }
}