using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthECAPI.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<AppUser>()
                    .AddEntityFrameworkStores<AppDbContext>();
            return services;
        }
        public static IServiceCollection ConfigureIdentiyOptions(this IServiceCollection services)
        {
            return services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            });
        }
        //Auth = Authentication + Authorization
        public static IServiceCollection AddIdentiyAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(x =>
              {
                  x.DefaultScheme =
                  x.DefaultChallengeScheme =
                  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
              }).AddJwtBearer(y =>
              {
                  y.SaveToken = false;
                  y.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!))
                  };
              });

            return services;
        }
        public static WebApplication AddIdentityAuthMiddelwares(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
