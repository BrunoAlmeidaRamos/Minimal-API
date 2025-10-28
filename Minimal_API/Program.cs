using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
  options.UseMySql(
          builder.Configuration.GetConnectionString("mysqlConnection"),
           ServerVersion.AutoDetect(
               builder.Configuration.GetConnectionString("mysqlConnection")
           )
  );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "0123456")
        return Results.Ok("Login com sucesso");

    else
        return Results.Unauthorized();
});

app.Run();


