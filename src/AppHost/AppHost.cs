using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var backend = builder.AddProject<Projects.Todo_WebAPI>("backend");

var frontend = builder.AddNpmApp("frontend", "../Frontend", "start")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(backend)
    .WaitFor(backend);

builder.Build().Run();
