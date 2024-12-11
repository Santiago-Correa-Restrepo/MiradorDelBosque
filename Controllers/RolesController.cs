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
    public class RolesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public RolesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.Estado = estado;
            _context.Update(role);
            await _context.SaveChangesAsync();

            // Opcional: Si necesitas alguna acción adicional tras el cambio de estado
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Index()
        {
            // Carga todos los roles, incluyendo ADMINISTRADOR y RECEPCIONISTA aunque estén desactivados
            var roles = await _context.Roles
                .Where(r => r.NomRol == "ADMINISTRADOR" || r.NomRol == "RECEPCIONISTA" || r.Estado)
                .ToListAsync();

            return View(roles);
        }



        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.PermisosRoles) // Incluye la relación con PermisosRoles
                    .ThenInclude(pr => pr.IdPermisoNavigation) // Incluye la navegación hacia Permisos
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }


        // GET: Roles/Create
        public IActionResult Create()
        {
            ViewData["Permisos"] = _context.Permisos.Select(p => new SelectListItem
            {
                Value = p.IdPermiso.ToString(),
                Text = p.NomPermiso
            }).ToList();

            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role, int[] permisosSeleccionados)
        {
            // Verifica si ya existe un rol con el mismo nombre
            if (_context.Roles.Any(r => r.NomRol == role.NomRol))
            {
                ModelState.AddModelError("NomRol", "Ya existe un rol con este nombre.");
            }

            // Verifica que se hayan seleccionado al menos un permiso
            if (permisosSeleccionados == null || !permisosSeleccionados.Any())
            {
                ModelState.AddModelError("Permisos", "Debe seleccionar al menos un permiso para este rol.");
            }

            if (ModelState.IsValid)
            {
                // Agregar el rol
                _context.Add(role);
                await _context.SaveChangesAsync();

                // Asignar permisos seleccionados al rol creado
                foreach (var permisoId in permisosSeleccionados)
                {
                    var permisoRol = new PermisosRole
                    {
                        IdRol = role.IdRol,
                        IdPermiso = permisoId
                    };
                    _context.PermisosRoles.Add(permisoRol);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si la validación falla, vuelve a cargar los datos para la vista
            ViewData["Permisos"] = _context.Permisos.Select(p => new SelectListItem
            {
                Value = p.IdPermiso.ToString(),
                Text = p.NomPermiso
            }).ToList();

            return View(role);
        }



        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            ViewData["Permisos"] = _context.Permisos.Select(p => new SelectListItem
            {
                Value = p.IdPermiso.ToString(),
                Text = p.NomPermiso,
                Selected = _context.PermisosRoles.Any(pr => pr.IdRol == id && pr.IdPermiso == p.IdPermiso)
            }).ToList();

            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role, int[] permisosSeleccionados)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualiza el rol en la base de datos
                    _context.Update(role);
                    await _context.SaveChangesAsync();

                    // Elimina los permisos actuales asignados al rol
                    var permisosActuales = _context.PermisosRoles.Where(pr => pr.IdRol == id).ToList();
                    _context.PermisosRoles.RemoveRange(permisosActuales);

                    // Asigna los nuevos permisos seleccionados
                    foreach (var permisoId in permisosSeleccionados)
                    {
                        var permisoRol = new PermisosRole
                        {
                            IdRol = role.IdRol,
                            IdPermiso = permisoId
                        };
                        _context.PermisosRoles.Add(permisoRol);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.IdRol))
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

            // Prepara los permisos nuevamente para mostrarlos en la vista si falla la validación
            ViewData["Permisos"] = _context.Permisos.Select(p => new SelectListItem
            {
                Value = p.IdPermiso.ToString(),
                Text = p.NomPermiso,
                Selected = permisosSeleccionados.Contains(p.IdPermiso)
            }).ToList();

            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (role == null)
            {
                return NotFound();
            }
            {
                TempData["ErrorMessage"] = "La eliminación de roles no está permitida.";
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles
                .Include(r => r.PermisosRoles)
                .FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return RedirectToAction("Index");
            }

            if (role.PermisosRoles != null && role.PermisosRoles.Any())
            {
                TempData["ErrorMessage"] = "La eliminación de roles no está permitida.";
                return RedirectToAction("Index");
            }

            {
                TempData["ErrorMessage"] = "La eliminación de roles no está permitida.";
                return RedirectToAction(nameof(Index));
            }
        }


        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }
    }
}