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
    public class HabitacionesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public HabitacionesController(BdGlampingFinalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCreateData()
        {
            var tiposHabitacion = _context.TipoHabitaciones
                .Select(th => new { idTipoHabitacion = th.IdTipoHabitacion, nomTipoHabitacion = th.NomTipoHabitacion })
                .ToList();

            var muebles = _context.Muebles
                .Where(m => m.Estado == true)
                .Select(m => new { idMueble = m.IdMueble, nombre = m.Nombre, valor = m.Valor ?? 0 }) // Asegúrate de manejar valores nulos aquí
                .ToList();

            return Json(new { tiposHabitacion, muebles });
        }
        [HttpGet]
        public async Task<IActionResult> GetHabitacionDetails(int id)
        {
            try
            {
                var habitacion = await _context.Habitaciones
                    .Include(h => h.oTipoHabitacion) // Incluir el tipo de habitación
                    .FirstOrDefaultAsync(h => h.IdHabitacion == id);

                if (habitacion == null)
                {
                    return NotFound(new { message = "Habitación no encontrada." });
                }

                // Devuelve los datos en formato JSON
                return Json(new
                {
                    habitacion.Nombre,
                    habitacion.Descripcion,
                    habitacion.Cantidad,
                    habitacion.FechaRegistro,
                    habitacion.Costo,
                    habitacion.Estado,
                    TipoHabitacion = habitacion.oTipoHabitacion?.NomTipoHabitacion ?? "Sin tipo asignado"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles de la habitación: {ex.Message}");
                return StatusCode(500, new { message = "Error interno en el servidor." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            var cliente = await _context.Habitaciones.FindAsync(id);
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

        // GET: Habitaciones
        public async Task<IActionResult> Index()
        {
            var bdMiradorContext = _context.Habitaciones.Include(h => h.oTipoHabitacion);
            return View(await bdMiradorContext.ToListAsync());
        }

        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.oTipoHabitacion)
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            return View(habitacione);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "NomTipoHabitacion");
            ViewData["Muebles"] = _context.Muebles.Where(s => s.Estado == true).Select(m => new { m.IdMueble, m.Nombre, m.Cantidad }).ToList();
            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHabitacion,Nombre,IdTipoHabitacion,Estado,Cantidad,Descripcion,Costo")] Habitacione habitacione, int[] mueblesSeleccionados, int[] cantidadMuebles)
        {
            if (ModelState.IsValid)
            {
                // Guardar la habitación
                _context.Add(habitacione);
                await _context.SaveChangesAsync();

                // Guardar la relación con los muebles y actualizar las cantidades
                if (mueblesSeleccionados != null)
                {
                    for (int i = 0; i < mueblesSeleccionados.Length; i++)
                    {
                        var muebleId = mueblesSeleccionados[i];
                        var cantidad = cantidadMuebles != null && i < cantidadMuebles.Length ? cantidadMuebles[i] : 0; // Default cantidad

                        var habitacionMueble = new HabitacionMueble
                        {
                            IdHabitacion = habitacione.IdHabitacion,
                            IdMueble = muebleId,
                            Cantidad = cantidad // Cantidad asignada a la habitación
                        };

                        _context.HabitacionMuebles.Add(habitacionMueble);

                        // Actualizar la cantidad de muebles en la tabla Muebles
                        var mueble = await _context.Muebles.FindAsync(muebleId);
                        if (mueble != null)
                        {
                            mueble.Cantidad -= cantidad; // Restar la cantidad seleccionada
                            _context.Update(mueble); // Marcar el mueble como modificado
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "NomTipoHabitacion", habitacione.IdTipoHabitacion);
            ViewData["Muebles"] = new SelectList(_context.Muebles.Where(s => s.Estado == true), "IdMueble", "Nombre");
            return View(habitacione);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation) // Incluir información de muebles
                .FirstOrDefaultAsync(h => h.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "NomTipoHabitacion", habitacione.IdTipoHabitacion);

            // Obtener todos los muebles y su estado de selección
            var muebles = await _context.Muebles.ToListAsync();
            var mueblesSeleccionados = habitacione.HabitacionMuebles.Select(hm => hm.IdMueble).ToHashSet();

            ViewData["Muebles"] = muebles.Select(m => new
            {
                Id = m.IdMueble,
                Nombre = m.Nombre,
                Seleccionado = mueblesSeleccionados.Contains(m.IdMueble) // Si el mueble está asignado
            }).ToList();

            // Convertir DateOnly a DateTime para la vista
            ViewBag.FechaRegistro = habitacione.FechaRegistro.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd");

            return View(habitacione);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdHabitacion,Nombre,IdTipoHabitacion,Estado,Descripcion,Costo,Cantidad")] Habitacione habitacione,
            int[] mueblesSeleccionados,
            string fechaRegistro)
        {
            if (id != habitacione.IdHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir la fecha del formato string a DateOnly
                    if (DateTime.TryParse(fechaRegistro, out var fecha))
                    {
                        habitacione.FechaRegistro = DateOnly.FromDateTime(fecha);
                    }
                    else
                    {
                        ModelState.AddModelError("FechaRegistro", "La fecha no es válida.");
                        ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre", habitacione.IdTipoHabitacion);
                        ViewData["Muebles"] = _context.Muebles
                            .Select(m => new
                            {
                                m.IdMueble,
                                m.Nombre,
                                Seleccionado = mueblesSeleccionados.Contains(m.IdMueble)
                            }).ToList();
                        return View(habitacione);
                    }

                    _context.Update(habitacione);

                    // Obtener los muebles actuales asignados a la habitación
                    var mueblesAnteriores = _context.HabitacionMuebles.Where(hm => hm.IdHabitacion == id).ToList();

                    // Eliminar relaciones de muebles no seleccionados
                    foreach (var muebleAnterior in mueblesAnteriores)
                    {
                        if (!mueblesSeleccionados.Contains(muebleAnterior.IdMueble))
                        {
                            _context.HabitacionMuebles.Remove(muebleAnterior);
                        }
                    }

                    // Agregar nuevos muebles seleccionados
                    foreach (var muebleId in mueblesSeleccionados)
                    {
                        if (!mueblesAnteriores.Any(hm => hm.IdMueble == muebleId))
                        {
                            _context.HabitacionMuebles.Add(new HabitacionMueble
                            {
                                IdHabitacion = habitacione.IdHabitacion,
                                IdMueble = muebleId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacioneExists(habitacione.IdHabitacion))
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

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre", habitacione.IdTipoHabitacion);
            ViewData["Muebles"] = _context.Muebles
                .Select(m => new
                {
                    m.IdMueble,
                    m.Nombre,
                    Seleccionado = mueblesSeleccionados.Contains(m.IdMueble)
                }).ToList();

            return View(habitacione);
        }


        private bool HabitacioneExists(int id)
        {
            return _context.Habitaciones.Any(e => e.IdHabitacion == id);
        }

        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.oTipoHabitacion)
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            return View(habitacione);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitaciones
                .Include(h => h.oTipoHabitacion)
                .FirstOrDefaultAsync(h => h.IdHabitacion == id);

            if (habitacion == null)
            {
                TempData["ErrorMessage"] = "No se puede eliminar porque está asociada a un tipo de habitación.";
                return RedirectToAction(nameof(Index));
            }

            if (habitacion.oTipoHabitacion != null)
            {
                TempData["ErrorMessage"] = "No se puede eliminar porque está asociada a un tipo de habitación.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Habitaciones.Remove(habitacion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "La habitación se eliminó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
    