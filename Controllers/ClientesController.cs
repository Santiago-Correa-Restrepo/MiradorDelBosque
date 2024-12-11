using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ultima.Models;

namespace Ultima.Controllers
{
    public class ClientesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public ClientesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var paquete = await _context.Paquetes
                .Include(p => p.oHabitacion)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            return Json(new
            {
                idPaquete = paquete.IdPaquete,
                nomPaquete = paquete.NomPaquete,
                descripcion = paquete.Descripcion,
                costo = paquete.Costo,
                idHabitacion = paquete.IdHabitacion,
                habitacionNombre = paquete.oHabitacion?.Nombre
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return Json(new { success = false, message = "Cliente no encontrado" });
            }

            cliente.Estado = estado;
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

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var bdMiradorContext = _context.Clientes.Include(c => c.IdRolNavigation).Include(c => c.oTipoDocumento);
            return View(await bdMiradorContext.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdRolNavigation)
                .Include(c => c.oTipoDocumento)
                .FirstOrDefaultAsync(m => m.NroDocumento == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol");
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento");
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NroDocumento,IdTipoDocumento,Nombres,Apellidos,Celular,Correo,Contrasena,Estado,IdRol")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Asignar valores predeterminados si no se proporcionan
                if (string.IsNullOrEmpty(cliente.Contrasena))
                {
                    cliente.Contrasena = "defaultPassword"; // Cambia a un valor más seguro si es necesario
                }

                if (!cliente.IdRol.HasValue)
                {
                    cliente.IdRol = 1; // Asigna un valor de rol por defecto
                }

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol", cliente.IdRol);
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", cliente.IdTipoDocumento);
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Datos inválidos" });
        }
        [HttpGet]
        public IActionResult ObtenerTiposDocumento()
        {
            var tiposDocumento = _context.TipoDocumentos
                .Select(td => new
                {
                    IdTipoDocumento = td.IdTipoDocumento,
                    NomTipoDcumento = td.NomTipoDcumento
                })
                .ToList();

            return Json(tiposDocumento);
        }
        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol", cliente.IdRol);
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", cliente.IdTipoDocumento);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NroDocumento,IdTipoDocumento,Nombres,Apellidos,Celular,Correo,Contrasena,Estado,IdRol")] Cliente cliente)
        {
            if (id != cliente.NroDocumento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtén la entidad original sin rastrearla para evitar la duplicación
                    var existingClient = await _context.Clientes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.NroDocumento == id);

                    if (existingClient != null && string.IsNullOrEmpty(cliente.Contrasena))
                    {
                        // Mantener la contraseña original si no se proporciona una nueva
                        cliente.Contrasena = existingClient.Contrasena;
                    }

                    _context.Update(cliente); // Actualiza la entidad con los valores modificados
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.NroDocumento))
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

            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NomRol", cliente.IdRol);
            ViewData["IdTipoDocumento"] = new SelectList(_context.TipoDocumentos, "IdTipoDocumento", "NomTipoDcumento", cliente.IdTipoDocumento);
            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.NroDocumento == id);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdRolNavigation)
                .Include(c => c.oTipoDocumento)
                .FirstOrDefaultAsync(m => m.NroDocumento == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            var cliente = await _context.Clientes
                .Include(c => c.Reservas)
                .FirstOrDefaultAsync(c => c.NroDocumento == id);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (cliente.Reservas != null && cliente.Reservas.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el cliente porque tiene reservas asociadas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}