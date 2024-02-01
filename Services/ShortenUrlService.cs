using TinyUrl.Models.Interfaces;

namespace TinyUrl.Services
{
    public class ShortenUrlService : IShortenUrlService
    {
        public string GenerateShortUrl()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
        }
    }
}