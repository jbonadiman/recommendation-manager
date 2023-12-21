using System.Reflection;
using FluentValidation;
using RecommendationManager.Application.Interfaces;
using RecommendationManager.Application.Parsers;
using RecommendationManager.Application.Services;
using RecommendationManager.Application.Validators;
using RecommendationManager.Infrastructure.Database;
using RecommendationManager.Infrastructure.Repositories;
using RecommendationManager.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logConfig) => logConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

builder.Logging
    .ClearProviders()
    .AddSerilog();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssembly(
    Assembly.GetAssembly(typeof(SaveBundleRequestValidator)));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IBookParser, BookParser>();
builder.Services.AddSingleton<INotificationService, LogNotification>();
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var context = new DataContext(config);
    context.Init();

    return context;
});

builder.Services.AddScoped<IBookService, BookService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
