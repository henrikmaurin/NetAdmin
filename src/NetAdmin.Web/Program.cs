using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;
using NetAdmin.Infrastructure.Data;
using NetAdmin.Infrastructure.Modules;
using NetAdmin.Infrastructure.Services;
using NetAdmin.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "NetAdmin.Auth";
        options.LoginPath = "/login";
        options.LogoutPath = "/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// SQLite-databas (lagras i /app/data i Docker)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=data/netadmin.db";

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Registrera korreleringsmotorn och SQLite-lagringen för switchportar
builder.Services.AddScoped<NetworkTopologyService>();
builder.Services.AddScoped<ISwitchPortStore, SqliteSwitchPortStore>();
builder.Services.AddScoped<IEncryptedSettingsService, EncryptedSettingsService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IShellyStore, SqliteShellyStore>();
builder.Services.AddScoped<IPatchPanelProvider, SqlitePatchPanelStore>();
builder.Services.AddScoped<IUnmanagedSwitchProvider, SqliteUnmanagedSwitchStore>();
builder.Services.AddScoped<IPhysicalLinkProvider, SqlitePhysicalLinkStore>();
builder.Services.AddScoped<IHardwareInventoryProvider, SqliteHardwareInventoryStore>();
builder.Services.AddScoped<IRoomProvider, SqliteRoomStore>();
builder.Services.AddScoped<NetworkTraceService>();

// Registrera basmoduler och upptäck alla tillverkare/nätverks-providers automatiskt via reflection (plugin-arkitektur)
builder.Services.AddInfrastructureHardwareModules();
builder.Services.AddNetAdminProviderPlugins(builder.Configuration);
// Service provider registry to map service types to registered IServiceIntegration implementations
builder.Services.AddScoped<NetAdmin.Web.Services.ServiceProviderRegistry>();

var app = builder.Build();

// Apply EF Core migrations on startup
Directory.CreateDirectory("data");
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = await dbFactory.CreateDbContextAsync();

    // Baseline __EFMigrationsHistory if the SQLite database was previously created with EnsureCreated()
    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();
    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='HardwareDevices';";
        var hasHardwareTable = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';";
        var hasHistoryTable = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;

        if (hasHardwareTable && !hasHistoryTable)
        {
            cmd.CommandText = """
                CREATE TABLE "__EFMigrationsHistory" (
                    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                    "ProductVersion" TEXT NOT NULL
                );
                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('20260405120000_InitialSchema', '10.0.13');
                """;
            await cmd.ExecuteNonQueryAsync();
        }
    }

    await db.Database.MigrateAsync();

    var seedFilePath = app.Configuration["Seed:FilePath"];
    await LocalDatabaseSeeder.SeedFromFileIfEmptyAsync(db, seedFilePath);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

// Auth & Onboarding HTTP gate for page requests
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "/";

    var isFrameworkOrStatic =
        path.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/auth/", StringComparison.OrdinalIgnoreCase) ||
        Path.HasExtension(path);

    if (!isFrameworkOrStatic && HttpMethods.IsGet(context.Request.Method))
    {
        var authService = context.RequestServices.GetRequiredService<IAdminAuthService>();
        var isOnboarded = await authService.IsOnboardingCompletedAsync();
        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;

        if (!isOnboarded)
        {
            if (!path.StartsWith("/onboarding", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect("/onboarding");
                return;
            }
        }
        else if (!isAuthenticated)
        {
            if (!path.StartsWith("/login", StringComparison.OrdinalIgnoreCase))
            {
                var returnUrl = Uri.EscapeDataString(path + context.Request.QueryString);
                context.Response.Redirect($"/login?returnUrl={returnUrl}");
                return;
            }
        }
    }

    await next();
});

app.MapPost("/auth/login", async (HttpContext httpContext, IAdminAuthService authService) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();

    if (await authService.ValidateCredentialsAsync(username, password))
    {
        var canonicalUser = await authService.GetAdminUsernameAsync() ?? username.Trim();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, canonicalUser),
            new(ClaimTypes.Role, "Administrator")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        var target = !string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//")
            ? returnUrl
            : "/";
        return Results.Redirect(target);
    }

    var retryReturn = !string.IsNullOrWhiteSpace(returnUrl) ? $"&returnUrl={Uri.EscapeDataString(returnUrl)}" : "";
    return Results.Redirect($"/login?error=1{retryReturn}");
}).DisableAntiforgery();

app.MapGet("/auth/token-signin", async (
    HttpContext httpContext,
    IDataProtectionProvider protectionProvider,
    string? token,
    string? redirect) =>
{
    if (string.IsNullOrWhiteSpace(token))
        return Results.Redirect("/login");

    try
    {
        var protector = protectionProvider.CreateProtector("NetAdmin.Onboarding.SignIn.v1");
        var payload = protector.Unprotect(token);
        var parts = payload.Split('|', 2);
        if (parts.Length == 2 &&
            long.TryParse(parts[1], out var issuedUnix) &&
            Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - issuedUnix) <= 120)
        {
            var username = parts[0];
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, "Administrator")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            var target = !string.IsNullOrWhiteSpace(redirect) && redirect.StartsWith('/') && !redirect.StartsWith("//")
                ? redirect
                : "/";
            return Results.Redirect(target);
        }
    }
    catch
    {
        // Invalid or expired token
    }

    return Results.Redirect("/login");
});

app.MapGet("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
