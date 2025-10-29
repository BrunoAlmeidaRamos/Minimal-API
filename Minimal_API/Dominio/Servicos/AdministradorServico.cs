using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Infraestrutura.Db;

namespace Minimal_API.Dominio.Servicos;

public class AdministradorServico : iAdministradorServico
{
    private readonly DbContexto _context;
    public AdministradorServico(DbContexto context)
    {
        _context = context;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
           return _context.Administradores
          .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
    }
}
