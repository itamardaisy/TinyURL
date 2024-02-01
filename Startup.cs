using Microsoft.AspNetCore.Mvc;
using TinyUrl.Data;

namespace TinyUrl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(); // Add this line to enable controllers for Web API
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0); // Adjust the version based on your needs

            services.AddSingleton(new MongoDbContext(Configuration.GetConnectionString("MongoDb"), "TinyUrlDb"));
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
