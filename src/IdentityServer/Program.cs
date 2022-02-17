using IdentityServer;
using IdentityServer4;
using IdentityServerHost.Quickstart.UI;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    // uncomment to write to Azure diagnostics stream
    //.WriteTo.File(
    //    @"D:\home\LogFiles\Application\identityserver.txt",
    //    fileSizeLimitBytes: 1_000_000,
    //    rollOnFileSizeLimit: true,
    //    shared: true,
    //    flushToDiskInterval: TimeSpan.FromSeconds(1))
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .CreateLogger();

try
{
    Log.Information("Starting host...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();


    builder.Services.AddControllersWithViews();

    builder.Services
        .AddIdentityServer()
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryClients(Config.Clients)
        .AddTestUsers(TestUsers.Users)
        // not recommended for production - you need to store your key material somewhere secure
        .AddDeveloperSigningCredential();

    //builder.Services.AddAuthentication()
    //    .AddGoogle("Google", options =>
    //    {
    //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

    //        options.ClientId = "<insert here>";
    //        options.ClientSecret = "<insert here>";
    //    })
    //    .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
    //    {
    //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
    //        options.SignOutScheme = IdentityServerConstants.SignoutScheme;
    //        options.SaveTokens = true;

    //        options.Authority = "https://demo.identityserver.io/";
    //        options.ClientId = "interactive.confidential";
    //        options.ClientSecret = "secret";
    //        options.ResponseType = "code";

    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            NameClaimType = "name",
    //            RoleClaimType = "role"
    //        };
    //    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    
    app.UseStaticFiles();
    app.UseRouting();

    app.UseIdentityServer();

    //app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}