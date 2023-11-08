using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Configuration;
using RecipeSocialMediaAPI.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureLogging();

builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwagger();

builder.ConfigureOptions();
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
