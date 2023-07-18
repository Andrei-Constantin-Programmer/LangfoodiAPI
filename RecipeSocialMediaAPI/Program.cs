using RecipeSocialMediaAPI.Utilities;
using System.Runtime.CompilerServices;
using RecipeSocialMediaAPI.Configuration;
using RecipeSocialMediaAPI;

[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Tests.Unit")]
[assembly: InternalsVisibleTo("RecipeSocialMediaAPI.Tests.Integration")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureLogging();

builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwagger();

builder.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMappingMiddleware>();

app.MapEndpoints();
app.Run();


public partial class Program { }
