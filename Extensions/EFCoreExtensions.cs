using AuthECAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
        {
            return services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DB")));
        }
    }
}
