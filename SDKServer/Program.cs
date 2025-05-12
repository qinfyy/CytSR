using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace SDKServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                 .SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!)
                 .AddJsonFile("appsettings.json")
                 .Build();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.Host.UseNLog();
            builder.Services.AddControllers();
            builder.Services.Configure<SDKSettings>(config.GetSection("SDKSettings"));
            builder.Configuration.AddConfiguration(config);

            builder.Services.AddDbContext<SDKDbContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection")));

            WebApplication app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SDKDbContext>();
                db.Database.EnsureCreated();
            }

            app.MapControllers();
            app.Run();
        }
    }
}
