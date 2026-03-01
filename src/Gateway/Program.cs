using LettuceEncrypt;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

if (!builder.Environment.IsDevelopment())
{
    var domainNames = builder.Configuration.GetSection("LettuceEncrypt:DomainNames").Get<string[]>() ?? [];
    var hasRealDomain = domainNames.Length > 0 && domainNames[0] is not ("" or "localhost");

    if (hasRealDomain)
        builder.Services.AddLettuceEncrypt()
            .PersistDataToDirectory(new DirectoryInfo("/app/certs"), null);
}

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapReverseProxy();

app.MapFallbackToFile("index.html");

app.MapDefaultEndpoints();

app.Run();
