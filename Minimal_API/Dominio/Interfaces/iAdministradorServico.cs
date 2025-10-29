using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Dominio.Interfaces;

public interface iAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}
