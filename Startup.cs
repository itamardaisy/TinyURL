using Microsoft.AspNetCore.Mvc;
using TinyUrl.Data;
using TinyUrl.Models.Interfaces;
using TinyUrl.Services;

namespace TinyUrl
{
    public class Startup
    {
        private const int CACHE_SIZE = 1000;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(); // Add this line to enable controllers for Web API
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0); // Adjust the version based on your needs

            services.AddTransient<IShortenUrlService>(provider => new ShortenUrlService());
            services.AddSingleton<ISizeLimitedCache<string, string>>(pro => new SizeLimitedCache<string, string>(CACHE_SIZE));
            services.AddSingleton<IDbContext>(provider =>
            {
                var connectionString = Configuration.GetConnectionString("MongoDb");
                var databaseName = "TinyUrlDb";

                return new MongoDbContext(connectionString, databaseName);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map controllers for Web API
            });
        }
    }
}
