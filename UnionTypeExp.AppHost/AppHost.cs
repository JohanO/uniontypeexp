using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.UnionTypeExp_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var scalar = builder.AddScalarApiReference();

scalar.WithApiReference(server);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
