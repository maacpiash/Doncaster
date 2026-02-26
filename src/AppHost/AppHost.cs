using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var backend = builder.AddProject<Projects.Todo_WebAPI>("backend");

var gateway = builder.AddProject<Projects.Todo_Gateway>("gateway")
    .WithReference(backend)
    .WaitFor(backend)
    .WithExternalHttpEndpoints();

var frontend = builder.AddNpmApp("frontend", "../Frontend", "start")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(backend)
    .WaitFor(backend);

builder.Build().Run();
