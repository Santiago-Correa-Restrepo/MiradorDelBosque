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
    public class ServiciosController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public ServiciosController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            servicio.Estado = estado;
            _context.Update(servicio);
            await _context.SaveChangesAsync();

            return Ok();
        }
  


        // GET: Servicios
        public async Task<IActionResult> Index()
        {
            var glampingMiradorContext = _context.Servicios.Include(s => s.oTipoServicio);
            return View(await glampingMiradorContext.ToListAsync());
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .Include(s => s.oTipoServicio)
                .FirstOrDefaultAsync(m => m.IdServicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            ViewData["IdTipoServicio"] = new SelectList(_context.TipoServicios, "IdTipoServicio", "NombreTipoServicio");
            return View();
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdServicio,IdTipoServicio,NomServicio,Costo,Descripcion,Estado")] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoServicio"] = new SelectList(_context.TipoServicios, "IdTipoServicio", "NombreTipoServicio", servicio.IdTipoServicio);
            return View(servicio);
        }



        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit([Bind("IdServicio,IdTipoServicio,NomServicio,Costo,Descripcion,Estado")]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            ViewData["IdTipoServicio"] = new SelectList(_context.TipoServicios, "IdTipoServicio", "NombreTipoServicio", servicio.IdTipoServicio);
            return View(servicio);
        }

        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdServicio,IdTipoServicio,NomServicio,Costo,Descripcion,Estado")] Servicio servicio)
        {
            if (id != servicio.IdServicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists((int)servicio.IdServicio))
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
            ViewData["IdTipoServicio"] = new SelectList(_context.TipoServicios, "IdTipoServicio", "IdTipoServicio", servicio.IdTipoServicio);
            return View(servicio);
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.IdServicio == id);
        }
        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .Include(s => s.oTipoServicio)
                .FirstOrDefaultAsync(m => m.IdServicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio == null)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el servicio porque está asociado a uno o más paquetes.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si el servicio está asociado a algún paquete
            var paquetesAsociados = _context.PaqueteServicios.Any(ps => ps.IdServicio == id);
            if (paquetesAsociados)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el servicio porque está asociado a uno o más paquetes.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Servicios.Remove(servicio);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "El servicio se eliminó correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al intentar eliminar el servicio: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}