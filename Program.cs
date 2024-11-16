using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentiyOptions()
                .AddIdentiyAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCORS(builder.Configuration)
    .AddIdentityAuthMiddelwares();

app.MapControllers();

app.MapGroup("/api")
   .MapIdentityApi<AppUser>();

app.MapGroup("/api")
   .MapIdentiyUserEndpoints(builder.Configuration);

app.Run();