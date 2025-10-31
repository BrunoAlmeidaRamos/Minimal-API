using Minimal_API.Dominio.Enus;

namespace Minimal_API.Dominio.DTOs;

public class AdministradorDTO
{
    public string Senha { get; set; } = default!;
    public string Email { get; set; } = default!;
    public Perfil Perfil { get; set; } = default!;
}
