using EfCoreBP.ApiService.Data;
using EfCoreBP.ApiService.Endpoints;
using EfCoreBP.ApiService.Services;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<BookStoreContext>("sqldata");

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddScoped<DatabaseSeeder>();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ => _.Servers = []);
}

app.MapDefaultEndpoints();

app.MapBookEndpoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<BookStoreContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

    context.Database.EnsureCreated();
    await seeder.SeedAsync();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();
