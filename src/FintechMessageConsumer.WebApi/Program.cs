using System.Diagnostics.CodeAnalysis;
using FintechMessageConsumer.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DependencyInjections
ConfigureBindingsDependencyInjection.RegisterBindings(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Program Partial Class
/// </summary>
[ExcludeFromCodeCoverage]
public static partial class Program;
