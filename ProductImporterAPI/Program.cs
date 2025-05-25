using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductImporterAPI.Data;
using ProductImporterAPI.Repositories;
using ProductImporterAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja serwisów (wstrzykiwanie zależności)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Rejestrujemy nasz kontekst Dappera oraz repozytoria
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();