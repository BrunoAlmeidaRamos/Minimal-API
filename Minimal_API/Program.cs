using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<iVeiculosServico, VeiculoServico>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddDbContext<DbContexto>(options =>
{
  options.UseMySql(
          builder.Configuration.GetConnectionString("mysqlConnection"),
           ServerVersion.AutoDetect(
               builder.Configuration.GetConnectionString("mysqlConnection")
           )
  );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure o HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // O SEU NOVO ENDPOINT DE HOME (MUDE O Results.Json PARA Results.Content)
    app.MapGet("", () =>
    {
        // 1. Cria o conteúdo HTML com o link
        var htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>API de Veiculos</title>
                <style>
                    body {{ font-family: sans-serif; text-align: center; padding-top: 50px; background-color: black; color: white }}
                    a {{ font-size: 1.2em; }}
                </style>
            </head>
            <body>
                <h1>Bem-vindo(a) API de Veiculos - Minimal API.</h1>
                <p>Acesse a doc interativa:</p>
                <a href=""/swagger"">Acessar Swagger</a>
            </body>
            </html>";

        // 2. Retorna o conteúdo com o Content-Type: text/html
        return Results.Content(htmlContent, "text/html");
    });

    // Mantenha as configurações do Swagger para evitar conflitos na raiz
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapPost("/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");

    else
        return Results.Unauthorized();
});

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculosDTO, iVeiculosServico veiculosServico) =>
{
    var veiculo = new Veiculo { Nome = veiculosDTO.Nome, Marca = veiculosDTO.Marca, Ano = veiculosDTO.Ano, };
    veiculosServico.Adicionar(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
});

app.Run();



