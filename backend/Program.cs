using System.Text;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailConfirmationService>();
builder.Services.AddScoped<MailingService>();
builder.Services.AddScoped<RegisterService>();

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<PasswordHasherService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(_builder =>
    {
        _builder
            .WithOrigins(
                builder.Configuration["ApplicationUrls:Frontent:Http"]!,
                builder.Configuration["ApplicationUrls:Frontent:Https"]!
            )
            .AllowAnyHeader()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Authentication:Key"],
            ValidAudience = builder.Configuration["Jwt:Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Authentication:Key"]!)
            ),
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("Authentication", out var cookieToken))
                {
                    context.Token = cookieToken;
                    context.HttpContext.Items["AuthenticationToken"] = cookieToken;
                }
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                using var scope = context.HttpContext.RequestServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(
        "api-routes",
        options =>
        {
            options.Theme = ScalarTheme.BluePlanet;
        }
    );
}
else
{
    app.UseExceptionHandler("/home/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
