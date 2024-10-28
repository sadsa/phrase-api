using Microsoft.EntityFrameworkCore;
using PhraseApi.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddDbContext<PhrasesDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add NpgsqlDataSource
builder.Services.AddNpgsqlDataSource(connectionString);

// Add Health Checks
builder.Services.AddHealthChecks().AddNpgSql();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();