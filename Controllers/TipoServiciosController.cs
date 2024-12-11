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
    public class TipoServiciosController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public TipoServiciosController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        // GET: TipoServicios

        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoServicios.ToListAsync());
        }

        // GET: TipoServicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicio = await _context.TipoServicios
                .FirstOrDefaultAsync(m => m.IdTipoServicio == id);
            if (tipoServicio == null)
            {
                return NotFound();
            }

            return View(tipoServicio);
        }

        // GET: TipoServicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoServicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoServicio,NombreTipoServicio")] TipoServicio tipoServicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoServicio);
        }

        // GET: TipoServicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicio = await _context.TipoServicios.FindAsync(id);
            if (tipoServicio == null)
            {
                return NotFound();
            }
            return View(tipoServicio);
        }

        // POST: TipoServicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoServicio,NombreTipoServicio")] TipoServicio tipoServicio)
        {
            if (id != tipoServicio.IdTipoServicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoServicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoServicioExists((int)tipoServicio.IdTipoServicio))
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
            return View(tipoServicio);
        }
        private bool TipoServicioExists(int id)
        {
            return _context.TipoServicios.Any(e => e.IdTipoServicio == id);
        }

        // GET: TipoServicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicio = await _context.TipoServicios
                .FirstOrDefaultAsync(m => m.IdTipoServicio == id);
            if (tipoServicio == null)
            {
                return NotFound();
            }

            return View(tipoServicio); // Esto muestra el tipo de servicio en la vista, en caso de que quieras confirmación antes de borrar
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoServicio = await _context.TipoServicios.FindAsync(id);
            if (tipoServicio == null)
            {
                return NotFound();
            }

            try
            {
                _context.TipoServicios.Remove(tipoServicio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se puede eliminar este tipo de servicio porque está asociado con un servicio.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}