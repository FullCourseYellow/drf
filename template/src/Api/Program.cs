using Company.ProjectName.Api.Features.WeatherForecasts;
using Company.ProjectName.Api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
#if (includeAuth)
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
#if (includeOpenTelemetry)
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
#endif

// Bootstrap Serilog so startup errors are captured
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ──────────────────────────────────────────────────────────────
    // Sinks are owned in code below. ReadFrom.Configuration reads MinimumLevel
    // overrides from appsettings.json — do not add WriteTo entries there to
    // avoid duplicate output.
    builder.Host.UseSerilog((context, services, config) => config
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14));

    // ── EF Core ───────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=app.db"));

    // ── Authentication ────────────────────────────────────────────────────────
#if (includeAuth)
    var authSection = builder.Configuration.GetSection("Authentication");
    var jwtSection = authSection.GetSection("JwtBearer");
    var useAuth = jwtSection.Exists() && !string.IsNullOrEmpty(jwtSection["Authority"]);
    if (useAuth)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtSection["Authority"];
                options.Audience = jwtSection["Audience"];
            });
        builder.Services.AddAuthorization();
    }
#endif

    // ── OpenTelemetry ─────────────────────────────────────────────────────────
#if (includeOpenTelemetry)
    var otelSection = builder.Configuration.GetSection("OpenTelemetry");
    if (otelSection.Exists())
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());
    }
#endif

    // ── OpenAPI ───────────────────────────────────────────────────────────────
    builder.Services.AddOpenApi("v1");

    // ── Application services ──────────────────────────────────────────────────
    builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

    // ─────────────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // Global exception handler → RFC 9457 ProblemDetails
    app.UseExceptionHandler(exApp => exApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "An unexpected error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        });
    }));

#if (includeAuth)
    if (useAuth)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
#endif

    app.UseSerilogRequestLogging();

    // OpenAPI JSON endpoint + Scalar UI
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
        options.WithTitle("Company.ProjectName API").WithTheme(ScalarTheme.Default));

    // API routes
    var api = app.MapGroup("/api/v1");
    api.MapWeatherForecasts();

#if (includeFrontend)
    // In production: serve React SPA from wwwroot/
    if (app.Environment.IsProduction())
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapFallbackToFile("index.html");
    }
#endif

    // Apply pending migrations on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Expose Program for WebApplicationFactory in tests
public partial class Program { }
