using MongoDB.Driver;

namespace TinyUrl.Models.Interfaces
{
    public interface IDbContext
    {
        IMongoCollection<UrlMapping> UrlMappings { get; }
    }
}
