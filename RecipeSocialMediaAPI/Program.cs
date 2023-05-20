using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Configuration;
using Serilog;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfiguration.ConfigureSerilog);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<IFakeRecipeRepository, FakeRecipeRepository>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

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
app.MapRecipeEndpoints();

app.MapPost("/logtest", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World");
});


app.Run();
