var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    //.WithDataVolume() // Persist data across restarts
    .AddDatabase("sqldata");

builder.AddProject<Projects.EfCoreBP_ApiService>("apiservice")
    .WithUrl("/scalar", "Scalar UI")
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar (HTTPS)";
        url.Url = "/scalar";
    })
    .WithHttpHealthCheck("/health")
    .WithReference(sql)
    .WaitFor(sql);

builder.Build().Run();
