var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.AzureBrasilCloudVaga_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.AzureBrasilCloudVaga_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
