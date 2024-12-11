using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ultima.Models;

namespace Ultima.Inicializacion
{
    public class Dbinicializador : IDbinicializador
    {
        private readonly BdGlampingFinalContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Dbinicializador (BdGlampingFinalContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() >0 ) 
                {
                    _db.Database.Migrate(); // ejecuta las migraciones pendientes
                }
            }
            catch (Exception)
            {

                throw;
            }

            // Datos iniciales
            if (_db.Roles.Any(r => r.NomRol == DS.Role_Admin)) return
            {

            }
        }
    }
}
