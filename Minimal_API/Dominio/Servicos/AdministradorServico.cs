using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Infraestrutura.Db;
using System.Linq;

namespace Minimal_API.Dominio.Servicos;

public class AdministradorServico : iAdministradorServico
{
    private readonly DbContexto _context;
    public AdministradorServico(DbContexto context)
    {
        _context = context;
    }

    public Administrador? Incluir(Administrador administrador)
    {
            _context.Administradores.Add(administrador);
            _context.SaveChanges();

            return administrador;
    }
    

    public Administrador? Login(LoginDTO loginDTO)
    {
           return _context.Administradores
          .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
    }

    public Administrador? ObterPorId(int id)
    {
       return _context.Administradores.FirstOrDefault(a => a.Id == id);
    }

    public List<Administrador?> Todos(int? pagina)
    {
        var query = _context.Administradores.AsQueryable();
        int itemsPorPagina = 10;

        if(pagina != null)
            query = query.Skip(((int)pagina - 1) * itemsPorPagina).Take(itemsPorPagina);

        return query.ToList();
    }
}
