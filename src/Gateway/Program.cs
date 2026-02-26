var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapReverseProxy();

app.MapFallbackToFile("index.html");

app.MapDefaultEndpoints();

app.Run();
