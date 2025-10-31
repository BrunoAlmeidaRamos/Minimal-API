using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infraestrutura.Db;
using System.Diagnostics.Eventing.Reader;

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
    }).WithTags("Home");

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
}).WithTags("Administradores");


app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculosDTO, iVeiculosServico veiculosServico) =>
{
    var veiculo = new Veiculo { Nome = veiculosDTO.Nome, Marca = veiculosDTO.Marca, Ano = veiculosDTO.Ano, };
    veiculosServico.Adicionar(veiculo);

    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int pagina, iVeiculosServico veiculosServico) =>
{
    if(pagina <= 0) pagina = 1;
    {
        if (veiculosServico.Todos(pagina).Count == 0)
            return Results.NotFound($"A pagina que você escolheu está vazio no momento.");

        var veiculos = veiculosServico.Todos(pagina);
        return Results.Ok(veiculos);
    }

}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound($"Veículo com id: {id} não encontrado.");
    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, iVeiculosServico veiculosServico) =>
{
    var veiculoExistente = veiculosServico.ObterPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound($"Veículo com id: {id} não encontrado.");

    veiculoExistente.Nome = veiculoDTO.Nome;
    veiculoExistente.Marca = veiculoDTO.Marca;
    veiculoExistente.Ano = veiculoDTO.Ano;
    veiculosServico.Atualizar(veiculoExistente);
    return Results.Ok(veiculoExistente);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculosServico veiculosServico) =>
{
    var veiculoExistente = veiculosServico.ObterPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound($"Veículo com id: {id} não encontrado.");
    veiculosServico.Deletar(veiculoExistente);
    return Results.Ok($"Veículo com id: {id} deletado com sucesso.");
}).WithTags("Veiculos");

app.Run();



