using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Enus;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Servicos;
using Minimal_API.Infraestrutura.Db;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var Key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(Key)) Key = "0123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui em baixo "
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


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
    }).AllowAnonymous().WithTags("Home");

    // Mantenha as configurações do Swagger para evitar conflitos na raiz
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

string GeradorTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(Key)) return string.Empty;

    var security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    var credentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),

    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
        );

    return new JwtSecurityTokenHandler().WriteToken(token);

}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null) 
    { 

        string token = GeradorTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, iAdministradorServico administradorServico) =>
{
    var novoAdministrador = new Administrador { Email = administradorDTO.Email, Senha = administradorDTO.Senha, Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()};
    administradorServico.Incluir(novoAdministrador);

    if (novoAdministrador == null)
        return Results.Conflict("Administrador com esse email já existe.");
    return Results.Created($"/administradores/{novoAdministrador.Id}", novoAdministrador);
}).RequireAuthorization().WithTags("Administradores");


app.MapGet("/administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.ObterPorId(id);
    if (administrador == null)
        return Results.NotFound($"Administrador com id: {id} não encontrado.");
    return Results.Ok(administrador);
}).RequireAuthorization().WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int pagina, iAdministradorServico administradorServico) =>
{
    if (pagina <= 0) pagina = 1;
    {
        if (administradorServico.Todos(pagina).Count == 0)
            return Results.NotFound($"A pagina que você escolheu está vazio no momento.");
        var administradores = administradorServico.Todos(pagina);
        return Results.Ok(administradores);
    }
}).RequireAuthorization().WithTags("Administradores");



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

}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound($"Veículo com id: {id} não encontrado.");
    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags("Veiculos");

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

}).RequireAuthorization().WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculosServico veiculosServico) =>
{
    var veiculoExistente = veiculosServico.ObterPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound($"Veículo com id: {id} não encontrado.");
    veiculosServico.Deletar(veiculoExistente);
    return Results.Ok($"Veículo com id: {id} deletado com sucesso.");
}).RequireAuthorization().WithTags("Veiculos");

app.Run();



