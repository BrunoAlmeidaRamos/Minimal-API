using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Dominio.Interfaces;

public interface iVeiculosServico
{
    List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
    Veiculo? ObterPorId(int id);

    void Adicionar(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Deletar(Veiculo veiculo);
}
