using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers
{
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class IdentiyUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentiyUserEndpoints(this IEndpointRouteBuilder app, IConfiguration config)
        {
            app.MapPost("/signup", async (UserManager<AppUser> userManager,
                                              [FromBody] UserRegistrationModel userRegistrationModel) =>
            {
                // Create a new user
                AppUser user = new AppUser()
                {
                    UserName = userRegistrationModel.Email,
                    Email = userRegistrationModel.Email,
                    FullName = userRegistrationModel.FullName,
                };

                // Create the user asynchronously
                var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

                // Define a DTO for the response
                if (result.Succeeded)
                {
                    return Results.Ok(result);
                }
                else
                {
                    // Create a response DTO with the errors
                    var errors = result.Errors.Select(e => e.Description);
                    return Results.BadRequest(result);
                }
            });

            app.MapPost("/signin", async (UserManager<AppUser> userManager, [FromBody] LoginModel loginModel,
                IOptions<AppSettings> appSettings) =>
            {
                var user = await userManager.FindByEmailAsync(loginModel.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var signInKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));

                    var tokendescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("UserID",user.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(10),
                        SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokendescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Results.Ok(token);
                }
                else
                {
                    return Results.BadRequest(new { message = "Username or password is incorrect." });
                }

            });

            return app;
        }
    }
}
