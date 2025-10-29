using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    
    public DbContexto(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = - 1,
                Email = "administrador@teste.com",
                Senha = "0123456",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configurationAppSettings.GetConnectionString("mysqlConnection")?.ToString();
        if (connectionString != null)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        else        
        {
            throw new InvalidOperationException("Connection string 'mysqlConnection' not found.");
        }


    }
}
