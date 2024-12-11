using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Ultima.Models;

namespace Ultima.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BdGlampingFinalContext _context;
        public UsuariosController(BdGlampingFinalContext context)
        {
            _context = context;
        }



        // Método para hashear la contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.Estado = estado;
            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // GET: Usuarios
        public async Task<IActionResult> Index()
        {

            var bdGlampingFinalContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            return View(await bdGlampingFinalContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.NroDocumento == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento");
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol");
            ViewData["IdRol"] = new SelectList(_context.Roles.Where(r => r.Estado), "IdRol", "NomRol"); // Solo roles activos
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NroDocumento,IdTipoDocumento,Nombres,Apellidos,Celular,Correo,Contrasena,Estado,IdRol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verifica si ya existe un usuario con el mismo NroDocumento
                var existingUsuario = await _context.Usuarios.FindAsync(usuario.NroDocumento);
                if (existingUsuario != null)
                {
                    ModelState.AddModelError("NroDocumento", "Ya existe un usuario con este número de documento.");

                    // Recarga las listas de selección para evitar errores en la vista
                    ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", usuario.IdTipoDocumento);
                    ViewData["IdRol"] = new SelectList(await GetRolesParaUsuariosAsync(), "IdRol", "NomRol", usuario.IdRol);

                    return View(usuario); // Regresa a la vista Create con el error
                }

                // Verifica si el rol seleccionado está activo o es especial (ADMINISTRADOR o RECEPCIONISTA)
                var role = await _context.Roles.FirstOrDefaultAsync(r =>
                    r.IdRol == usuario.IdRol && (r.Estado || r.NomRol == "ADMINISTRADOR" || r.NomRol == "RECEPCIONISTA"));
                if (role == null)
                {
                    ModelState.AddModelError("IdRol", "El rol seleccionado no está activo.");

                    // Recarga las listas de selección para evitar errores en la vista
                    ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", usuario.IdTipoDocumento);
                    ViewData["IdRol"] = new SelectList(await GetRolesParaUsuariosAsync(), "IdRol", "NomRol", usuario.IdRol);

                    return View(usuario); // Regresa a la vista Create con el error
                }

                // Busca el tipo de documento y asigna el nombre
                var tipoDocumento = await _context.TipoDocumentos.FindAsync(usuario.IdTipoDocumento);
                if (tipoDocumento != null)
                {
                    usuario.TipoDocumento = tipoDocumento.NomTipoDcumento; // Asigna el nombre del tipo de documento
                }

                // Encripta la contraseña antes de guardar
                if (!string.IsNullOrEmpty(usuario.Contrasena))
                {
                    usuario.Contrasena = PasswordHelper.HashPassword(usuario.Contrasena);
                }

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirige al índice después de crear
            }

            // Si ocurre un error, recarga las listas de selección
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", usuario.IdTipoDocumento);
            ViewData["IdRol"] = new SelectList(await GetRolesParaUsuariosAsync(), "IdRol", "NomRol", usuario.IdRol);
            return View(usuario);
        }

        // Método auxiliar para filtrar roles en la vista de creación de usuarios
        private async Task<List<Role>> GetRolesParaUsuariosAsync()
        {
            return await _context.Roles
                .Where(r => r.Estado || r.NomRol == "ADMINISTRADOR" || r.NomRol == "RECEPCIONISTA")
                .ToListAsync();
        }



        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Carga de roles y tipos de documentos
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol", usuario.IdRol);
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", usuario.IdTipoDocumento);

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NroDocumento,TipoDocumento,IdTipoDocumento,Nombres,Apellidos,Celular,Correo,Contrasena,Estado,IdRol")] Usuario usuario)
        {
            if (id != usuario.NroDocumento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Busca el usuario original sin seguimiento para comparar la contraseña
                    var existingUsuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.NroDocumento == id);
                    if (existingUsuario == null)
                    {
                        return NotFound();
                    }

                    // Busca y asigna el nombre del tipo de documento
                    var tipoDocumento = await _context.TipoDocumentos.FindAsync(usuario.IdTipoDocumento);
                    if (tipoDocumento != null)
                    {
                        usuario.TipoDocumento = tipoDocumento.NomTipoDcumento;
                    }

                    // Si la contraseña fue modificada, encripta la nueva
                    if (!string.IsNullOrEmpty(usuario.Contrasena) && usuario.Contrasena != existingUsuario.Contrasena)
                    {
                        usuario.Contrasena = PasswordHelper.HashPassword(usuario.Contrasena);
                    }
                    else
                    {
                        // Mantén la contraseña existente si no se cambió
                        usuario.Contrasena = existingUsuario.Contrasena;
                    }

                    _context.Update(usuario); // Actualiza el usuario
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redirige al índice después de editar
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.NroDocumento))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si ocurre un error, recarga las listas de selección
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "IdRol", usuario.IdRol);
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", usuario.IdTipoDocumento);
            return View(usuario);
        }


        public static class PasswordHelper
        {
            public static string HashPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }

            public static bool VerifyPassword(string password, string hashedPassword)
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
        }


        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.NroDocumento == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al eliminar el usuario: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.NroDocumento == id);
        }


    }
}