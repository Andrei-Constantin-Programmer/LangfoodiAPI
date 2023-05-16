using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Utilities;
using RecipeSocialMediaAPI.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog(SerilogConfiguration.ConfigureSerilog);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Setup Endpoints
app.MapUserEndpoints();

app.MapPost("/logtest", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World");
});


app.Run();
