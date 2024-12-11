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
    public class MueblesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public MueblesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        // GET: Muebles
        public async Task<IActionResult> Index()
        {
            var muebles = await _context.Muebles.ToListAsync();

            foreach (var mueble in muebles)
            {
                if (mueble.Imagen != null)
                {
                    string imagenBase64Data = Convert.ToBase64String(mueble.Imagen);
                    string imagenDataURL = string.Format("data:imagen/png;base64,{0}", imagenBase64Data);

                    // Crear una propiedad adicional en el objeto para almacenar la URL
                    mueble.ImagenDataURL = imagenDataURL;
                }
            }
            return View(muebles);
        }

        // GET: Muebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles
                .FirstOrDefaultAsync(m => m.IdMueble == id);
            if (mueble == null)
            {
                return NotFound();
            }

            return View(mueble);
        }

        // GET: Muebles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Muebles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMueble,Nombre,FechaRegistro,Valor,Estado,Imagen,Cantidad")] Mueble mueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mueble);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mueble);

        }


        // GET: Muebles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles.FindAsync(id);
            if (mueble == null)
            {
                return NotFound();
            }
            return View(mueble);
        }

        // POST: Muebles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMueble,Nombre,FechaRegistro,Valor,Estado,Imagen")] Mueble mueble)
        {
            if (id != mueble.IdMueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuebleExists(mueble.IdMueble))
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

            return View(mueble);
        }

        // GET: Muebles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles
                .FirstOrDefaultAsync(m => m.IdMueble == id);
            if (mueble == null)
            {
                return NotFound();
            }

            return View(mueble);
        }

        // POST: Muebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mueble = await _context.Muebles.FindAsync(id);
            if (mueble != null)
            {
                _context.Muebles.Remove(mueble);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ActualizarEstado(int id, bool estado)
        {
            var paquete = _context.Paquetes.Find(id);
            if (paquete != null)
            {
                paquete.Estado = estado;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Mueble no encontrado." });
        }

        private bool MuebleExists(int id)
        {
            return _context.Muebles.Any(e => e.IdMueble == id);
        }
    }
}