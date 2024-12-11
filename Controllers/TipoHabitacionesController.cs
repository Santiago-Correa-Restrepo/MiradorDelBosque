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
    public class TipoHabitacionesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public TipoHabitacionesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var cliente = await _context.TipoHabitaciones.FindAsync(id);
            if (cliente == null)
            {
                return Json(new { success = false, message = "Tipo de habitacion no encontrado" });
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

        // GET: TipoHabitaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoHabitaciones.ToListAsync());
        }

        // GET: TipoHabitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones
                .FirstOrDefaultAsync(m => m.IdTipoHabitacion == id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }

            return View(tipoHabitacione);
        }

        // GET: TipoHabitaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoHabitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoHabitacion,NomTipoHabitacion,NumeroPersonas,Estado")] TipoHabitacione tipoHabitacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoHabitacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacione);
        }

        // GET: TipoHabitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }
            return View(tipoHabitacione);
        }

        // POST: TipoHabitaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoHabitacion,NomTipoHabitacion,NumeroPersonas,Estado")] TipoHabitacione tipoHabitacione)
        {
            if (id != tipoHabitacione.IdTipoHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoHabitacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoHabitacioneExists(tipoHabitacione.IdTipoHabitacion))
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
            return View(tipoHabitacione);
        }
        private bool TipoHabitacioneExists(int id)
        {
            return _context.TipoHabitaciones.Any(e => e.IdTipoHabitacion == id);
        }
        // GET: TipoHabitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones
                .FirstOrDefaultAsync(m => m.IdTipoHabitacion == id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }

            return View(tipoHabitacione);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Buscar el tipo de habitación por ID
                var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);

                if (tipoHabitacion == null)
                {
                    return NotFound();  // Si no se encuentra, devolver NotFound
                }

                // Eliminar el tipo de habitación (la eliminación en cascada eliminará los registros relacionados en DetalleReservaPaquete)
                _context.TipoHabitaciones.Remove(tipoHabitacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "El tipo de habitación y sus dependencias fueron eliminados correctamente.";
                return RedirectToAction(nameof(Index));  // Redirigir a la vista principal
            }
            catch (Exception ex)
            {
                // Manejar cualquier error
                TempData["Error"] = $"Ocurrió un error al intentar eliminar: {ex.Message}";
                return RedirectToAction(nameof(Index));  // Redirigir con mensaje de error
            }
        }
    }
}