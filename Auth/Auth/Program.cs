using Auth.Security;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Auth.Data;
using Microsoft.EntityFrameworkCore;
using Auth.Repository;
using Auth.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IPasswordHasher<string>, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis")!;
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));


var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CORS_CONFIG");

app.MapControllers();

app.Run();
