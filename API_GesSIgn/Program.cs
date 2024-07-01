using API_GesSIgn.Helpers;
using API_GesSIgn.Models;
using API_GesSIgn.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services;
using System.Text;
using System.Text.Json;

/* 
    Créé le : 24 April 2024
    Créé par : NicolasDebras
    Modifications :
        9f1adc7 - refactor response - NicolasDebras
    4182590 - app - NicolasDebras
    a9c6ab0 - AllowAllLocal - NicolasDebras
    1f8a838 - test - NicolasDebras
    fd354e3 - }); - NicolasDebras
    944b44e - accept all CORS - NicolasDebras
    46d3493 - test - NicolasDebras
    3d6d688 - allow new localhost - NicolasDebras
    94fa232 - update auth - NicolasDebras
    28f6c00 - add list - NicolasDebras
    a44dd9e - add auth work - NicolasDebras
    4471112 - add building controllers - NicolasDebras
    54d52e1 - change diff - NicolasDebras
    2e1ccd6 - push - NicolasDebras
    e692f48 - complete push - NicolasDebras
    ab3e302 - work on model - debrasnicolas
    f99f21e - change - debrasnicolas
    272427f - move file - NicolasDebras
*/


var builder = WebApplication.CreateBuilder(args);

// Ajoutez services au conteneur.
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API GESIGN",
        Description = ".NET 8 Web API"
    });
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    swagger.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});


var key = Encoding.ASCII.GetBytes("VotreCléSécrèteSuperSécuriséeDe32CaractèresOuPlus");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync("An error occurred processing your authentication.");
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated for user: {User}", context.Principal.Identity.Name);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { error = "You are not authorized" });
                return context.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddDbContext<MonDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("MYAPP_CONNECTION_STRING")));

builder.Services.AddHostedService<CodeSendingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowLocalhostOrigins");

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.StatusCode = 204;
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
    _ = endpoints.MapHub<PresenceHub>("/presenceHub");
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run("http://*:" + port);
