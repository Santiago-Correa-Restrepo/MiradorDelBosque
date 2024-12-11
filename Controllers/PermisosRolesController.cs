using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ultima.Models;

namespace Ultima.Controllers
{
    public class PermisosRolesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public PermisosRolesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        // GET: PermisosRoles
        public async Task<IActionResult> Index()
        {
            var bdGlampingFinalContext = _context.PermisosRoles.Include(p => p.IdPermisoNavigation).Include(p => p.IdRolNavigation);
            return View(await bdGlampingFinalContext.ToListAsync());
        }

        // GET: PermisosRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisosRole = await _context.PermisosRoles
                .Include(p => p.IdPermisoNavigation)
                .Include(p => p.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdPermisosRoles == id);
            if (permisosRole == null)
            {
                return NotFound();
            }

            return View(permisosRole);
        }

        // GET: PermisosRoles/Create
        public IActionResult Create()
        {
            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "NomPermiso");
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol");
            return View();
        }

        // POST: PermisosRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPermisosRoles,IdRol,IdPermiso")] PermisosRole permisosRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permisosRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "IdPermiso", permisosRole.IdPermiso);
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "IdRol", permisosRole.IdRol);
            return View(permisosRole);
        }

        // GET: PermisosRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisosRole = await _context.PermisosRoles.FindAsync(id);
            if (permisosRole == null)
            {
                return NotFound();
            }
            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "IdPermiso", permisosRole.IdPermiso);
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "IdRol", permisosRole.IdRol);
            return View(permisosRole);
        }

        // POST: PermisosRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPermisosRoles,IdRol,IdPermiso")] PermisosRole permisosRole)
        {
            if (id != permisosRole.IdPermisosRoles)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permisosRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisosRoleExists(permisosRole.IdPermisosRoles))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "IdPermiso", permisosRole.IdPermiso);
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "IdRol", permisosRole.IdRol);
            return View(permisosRole);
        }

        // GET: PermisosRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisosRole = await _context.PermisosRoles
                .Include(p => p.IdPermisoNavigation)
                .Include(p => p.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdPermisosRoles == id);
            if (permisosRole == null)
            {
                return NotFound();
            }

            return View(permisosRole);
        }

        // POST: PermisosRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Carga el permiso junto con las asociaciones necesarias
            var permiso = await _context.Permisos
                .Include(p => p.PermisosRoles)
                .FirstOrDefaultAsync(p => p.IdPermiso == id);

            if (permiso == null)
            {
                return RedirectToAction(nameof(Index)); // Si no existe, redirige al índice
            }

            // Verifica si tiene roles asociados
            if (permiso.PermisosRoles != null && permiso.PermisosRoles.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar un permiso ya que está asociado a un rol.";
                return RedirectToAction(nameof(Index)); // Redirige con mensaje de error
            }

            // Elimina el permiso si no tiene asociaciones
            _context.Permisos.Remove(permiso);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Permiso eliminado correctamente.";
            return RedirectToAction(nameof(Index)); // Redirige con mensaje de éxito
        }


        private bool PermisosRoleExists(int id)
        {
            return _context.PermisosRoles.Any(e => e.IdPermisosRoles == id);
        }
    }
}