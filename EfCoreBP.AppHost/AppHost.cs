var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    //.WithDataVolume() // Persist data across restarts
    .AddDatabase("sqldata");

builder.AddProject<Projects.EfCoreBP_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(sql)
    .WaitFor(sql);

builder.Build().Run();
