using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Infraestrutura.Db;

namespace Minimal_API.Dominio.Servicos;

public class VeiculoServico : iVeiculosServico
{
    private readonly DbContexto _context;
    public VeiculoServico(DbContexto context)
    {
        _context = context;
    }

    public Veiculo? ObterPorId(int id)
    {
        return _context.Veiculos.FirstOrDefault(v => v.Id == id);
    }

    public void Adicionar(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);
        _context.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo); 
        _context.SaveChanges();
    }

    public void Deletar(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
        _context.SaveChanges();
    }

 
    public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _context.Veiculos.AsQueryable();
        if(!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
        }

        int itemsPorPagina = 10;
        query = query.Skip((pagina - 1) * itemsPorPagina).Take(itemsPorPagina);

        return query.ToList();
    }
}
