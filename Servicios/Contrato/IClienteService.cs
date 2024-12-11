using Microsoft.EntityFrameworkCore;
using Ultima.Models;


namespace Ultima.Servicios.Contrato
{
    public interface IClienteService
    {
        Task<Cliente> GetCliente(string correo, string contrasena);
        Task<Cliente> GetClienteByEmail(string correo);
        Task<Cliente> SaveCliente(Cliente modelo);
        Task<bool> UpdateCliente(Cliente usuario);
    }
}