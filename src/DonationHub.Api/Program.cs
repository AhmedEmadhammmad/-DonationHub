using System.Text;
using DonationHub.Api.Extensions;
using DonationHub.Api.Infrastructure;
using DonationHub.Api.Transformers;
using DonationHub.Application;
using DonationHub.Application.Common.Options;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register TimeProvider for deterministic time calculation and unit testing (Sprint 2 Task 2)
builder.Services.AddSingleton(TimeProvider.System);

// Register Global Exception Handling and RFC 9457 ProblemDetails (Sprint 2 Task 6)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Register strongly-typed options with fail-fast startup validation (Sprint 1 Task 6)
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<SeedAdminOptions>()
    .BindConfiguration(SeedAdminOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<DonationOptions>()
    .BindConfiguration(DonationOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add layer dependencies cleanly via Extension Methods (Task 1)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure JWT Bearer Authentication (Task 7: IDN-03)
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var secretKey = jwtSection[nameof(JwtOptions.SecretKey)]
    ?? throw new InvalidOperationException("JwtOptions:SecretKey is not configured.");
var issuer = jwtSection[nameof(JwtOptions.Issuer)];
var audience = jwtSection[nameof(JwtOptions.Audience)];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Configure OpenAPI 3.1 with Bearer Security Scheme Transformer
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Global Exception Handler Middleware (RFC 9457 ProblemDetails)
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DonationHub API - Sprint 2")
               .EnablePersistentAuthentication()
               .AddHttpAuthentication("Bearer", auth => auth.WithToken(string.Empty));
    });
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default Administrator account if not present (Task 7: IDN-11)
await app.SeedAdminUserAsync();
await app.SeedDefaultCategoriesAsync();
await app.SeedDevelopmentDataAsync();

app.Run();
