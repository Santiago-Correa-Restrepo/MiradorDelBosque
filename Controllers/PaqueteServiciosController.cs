using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ultima.Models;

namespace Glamping_Mirador.Controllers
{
    public class PaqueteServiciosController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public PaqueteServiciosController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        // GET: PaqueteServicios
        public async Task<IActionResult> Index()
        {
            var glampingMiradorContext = _context.PaqueteServicios.Include(p => p.oPaquete).Include(p => p.oServicio);
            return View(await glampingMiradorContext.ToListAsync());
        }

        // GET: PaqueteServicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paqueteServicio = await _context.PaqueteServicios
                .Include(p => p.oPaquete)
                .Include(p => p.oServicio)
                .FirstOrDefaultAsync(m => m.IdPaqueteServicio == id);
            if (paqueteServicio == null)
            {
                return NotFound();
            }

            return View(paqueteServicio);
        }

        // GET: PaqueteServicios/Create
        public IActionResult Create()
        {
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "NomPaquete");
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "NomServicio");
            return View();
        }

        // POST: PaqueteServicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaqueteServicio,IdPaquete,IdServicio,Precio")] PaqueteServicio paqueteServicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paqueteServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "NomPaquete", paqueteServicio.IdPaquete);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "NomServicio", paqueteServicio.IdServicio);
            return View(paqueteServicio);
        }

        // GET: PaqueteServicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paqueteServicio = await _context.PaqueteServicios.FindAsync(id);
            if (paqueteServicio == null)
            {
                return NotFound();
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "NomPaquete", paqueteServicio.IdPaquete);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "NomServicio", paqueteServicio.IdServicio);
            return View(paqueteServicio);
        }

        // POST: PaqueteServicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("IdPaqueteServicio,IdPaquete,IdServicio,Precio")] PaqueteServicio paqueteServicio)
        {
            if (id != paqueteServicio.IdPaqueteServicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paqueteServicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaqueteServicioExists(paqueteServicio.IdPaqueteServicio))
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
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "NomPaquete", paqueteServicio.IdPaquete);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "NomServicio", paqueteServicio.IdServicio);
            return View(paqueteServicio);
        }

        // GET: PaqueteServicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paqueteServicio = await _context.PaqueteServicios
                .Include(p => p.oPaquete)
                .Include(p => p.oServicio)
                .FirstOrDefaultAsync(m => m.IdPaqueteServicio == id);
            if (paqueteServicio == null)
            {
                return NotFound();
            }

            return View(paqueteServicio);
        }

        // POST: PaqueteServicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var paqueteServicio = await _context.PaqueteServicios.FindAsync(id);
            if (paqueteServicio != null)
            {
                _context.PaqueteServicios.Remove(paqueteServicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaqueteServicioExists(int? id)
        {
            return _context.PaqueteServicios.Any(e => e.IdPaqueteServicio == id);
        }
    }
}