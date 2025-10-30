using Auth.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Auth.Data;
using Microsoft.EntityFrameworkCore;
using Auth.Repository;
using Auth.Services;
using Auth.Filters;
using Auth.Startup;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();


builder.Services.AddScoped<IPasswordHasher<string>, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var applyMigrations = builder.Configuration["Database:ApplyMigrations"] == "true";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var claimsPrincipal = context.Principal!;
            Guid userId;
            if (!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out userId))
            {
                context.Fail("Token is invalid");
            }
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<DataContext>();
            var user = await dbContext.Users.FindAsync(userId);

            if (user == null || user.LastPasswordChangeDate > context.SecurityToken.ValidFrom)
            {
                context.Fail("Token is invalid due to password change");
            }
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORS_CONFIG",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                      });
});

builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionHandler>();
});

var port = builder.Configuration["Port"] ?? "5156";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();
if (applyMigrations) app.ApplyMigrations();


app.UseCors("CORS_CONFIG");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
