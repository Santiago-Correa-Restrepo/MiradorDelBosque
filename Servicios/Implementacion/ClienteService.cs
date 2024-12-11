using Microsoft.EntityFrameworkCore;
using Ultima.Models;
using Ultima.Servicios.Contrato;
using System.Threading.Tasks;
using System.Linq;

namespace Ultima.Servicios.Implementacion
{
    public class ClienteService : IClienteService
    {
        private readonly BdGlampingFinalContext _bdContext;

        public ClienteService(BdGlampingFinalContext bdContext)
        {
            _bdContext = bdContext;
        }

        public async Task<Cliente> GetCliente(string correo, string contrasena)
        {
            Cliente cliente_encontrado = await _bdContext.Clientes
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Contrasena == contrasena);

            return cliente_encontrado;
        }

        public async Task<Cliente> SaveCliente(Cliente modelo)
        {
            try
            {
                _bdContext.Clientes.Add(modelo);
                await _bdContext.SaveChangesAsync();
                return modelo;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateCliente(Cliente cliente)
        {
            var clienteExistente = await _bdContext.Clientes.FirstOrDefaultAsync(c => c.Correo == cliente.Correo);

            if (clienteExistente == null)
            {
                return false;
            }

            clienteExistente.Contrasena = cliente.Contrasena;

            try
            {
                _bdContext.Clientes.Update(clienteExistente);
                await _bdContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Cliente> GetClienteByEmail(string correo)
        {
            return await _bdContext.Clientes.FirstOrDefaultAsync(u => u.Correo == correo);
        }
    }
}