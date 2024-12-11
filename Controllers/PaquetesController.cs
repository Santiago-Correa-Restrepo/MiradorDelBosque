using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Ultima.Models.ViewModels;
using Ultima.Models;

namespace Ultima.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly BdGlampingFinalContext _context;

        public PaquetesController(BdGlampingFinalContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetById(int id)
        {
            var paquete = _context.Paquetes
                .Include(p => p.PaqueteServicios)
                    .ThenInclude(ps => ps.oServicio) // Incluye los servicios relacionados
                .FirstOrDefault(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            var result = new
            {
                IdPaquete = paquete.IdPaquete,
                NomPaquete = paquete.NomPaquete,
                Descripcion = paquete.Descripcion,
                Costo = paquete.Costo,
                IdHabitacion = paquete.IdHabitacion,
                Servicios = paquete.PaqueteServicios.Select(ps => new
                {
                    ps.IdServicio,
                    ps.oServicio.NomServicio,
                    ps.oServicio.Costo
                }).ToList()
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaqueteDetails(int id)
        {
            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteServicios)
                    .ThenInclude(ps => ps.oServicio)
                    .Include(p => p.oHabitacion)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            // Devuelve los datos en formato JSON
            return Json(new
            {
                paquete.NomPaquete,
                paquete.Descripcion,
                paquete.Costo,
                Habitacion = new
                {
                    paquete.oHabitacion.Nombre,
                    
                },
                Servicios = paquete.PaqueteServicios.Select(ps => new
                {
                    ps.oServicio.NomServicio,
                    ps.oServicio.Costo
                }).ToList()
            });
        }

        [HttpGet]
        public IActionResult GetPaqueteById(int id)
        {
            var paquete = _context.Paquetes
                .Include(p => p.PaqueteServicios) // Incluye los servicios relacionados
                .Include(p => p.oHabitacion) // Incluye la habitación relacionada
                .FirstOrDefault(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            // Retorna el paquete y la lista completa de habitaciones y servicios
            var result = new
            {
                paquete.IdPaquete,
                paquete.NomPaquete,
                paquete.Descripcion,
                paquete.Costo,
                paquete.IdHabitacion,
                ServiciosSeleccionados = paquete.PaqueteServicios.Select(s => s.IdServicio).ToList(),
                Habitaciones = _context.Habitaciones.Select(h => new
                {
                    h.IdHabitacion,
                    h.Nombre
                }).ToList(),
                TodosLosServicios = _context.Servicios.Select(s => new
                {
                    s.IdServicio,
                    s.NomServicio,
                    s.Costo
                }).ToList()
            };

            return Json(result);
        }
        public async Task<IActionResult> GetCreateData()
        {
            var habitaciones = await _context.Habitaciones
                .Select(h => new { h.IdHabitacion, h.Nombre })
                .ToListAsync();

            var servicios = await _context.Servicios
                .Select(s => new { s.IdServicio, s.NomServicio, s.Costo })
                .ToListAsync();

            return Json(new { habitaciones, servicios });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEstado(int id, bool estado)
        {
            // Buscar el paquete en la base de datos
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete == null)
            {
                return NotFound(); // Retorna 404 si el paquete no existe
            }

            // Actualizar el estado del paquete
            paquete.Estado = estado;

            // Guardar los cambios en la base de datos
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

        // GET: Paquetes
        public async Task<IActionResult> Index()
        {
            var ultimaMiradorContext = _context.Paquetes.Include(p => p.oHabitacion);
            return View(await ultimaMiradorContext.ToListAsync());


        }

        // GET: Paquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteServicios)
                    .ThenInclude(ps => ps.oServicio) // Incluye el servicio relacionado
                .Include(p => p.oHabitacion) // Incluye el nombre de la habitación
                .FirstOrDefaultAsync(m => m.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }



        // GET: Paquetes/Create
        public IActionResult Create()
        {
            // Filtrar habitaciones activas
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == true), "IdHabitacion", "Nombre");
            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == false).Select(s => new { s.IdServicio, s.NomServicio, s.Costo }).ToList();
            return View();
        }


        // POST: Paquetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paquete paquete, int[] IdServicios)
        {
            if (ModelState.IsValid)
            {
                // Primero, guarda el paquete
                _context.Add(paquete);
                await _context.SaveChangesAsync();

                // Asociar los servicios seleccionados al paquete con sus precios
                if (IdServicios != null)
                {
                    foreach (var servicioId in IdServicios)
                    {
                        // Obtener el servicio para acceder a su precio
                        var servicio = await _context.Servicios.FindAsync(servicioId);
                        if (servicio != null)
                        {
                            var paqueteServicio = new PaqueteServicio
                            {
                                IdPaquete = paquete.IdPaquete,
                                IdServicio = servicioId,
                                Costo = servicio.Costo // Agregar el precio del servicio
                            };
                            _context.PaqueteServicios.Add(paqueteServicio);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar las listas de habitaciones y servicios
            ViewData["Servicios"] = new SelectList(_context.Servicios.Where(s => s.Estado == false), "IdServicio", "NomServicio");
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Where(s => s.Estado == false), "IdHabitacion", "Nombre");
            return View(paquete);
        }


        // GET: Paquetes/Edit/5
        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var paquete = await _context.Paquetes
                .Include(p => p.PaqueteServicios)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            var servicios = _context.Servicios
                .Where(s => s.Estado == false)
                .AsEnumerable()
                .Select(s => new
                {
                    s.IdServicio,
                    s.NomServicio,
                    s.Costo,
                    Seleccionado = paquete.PaqueteServicios.Any(ps => ps.IdServicio == s.IdServicio)
                })
                .ToList();

            // Filtrar habitaciones activas
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == true), "IdHabitacion", "Nombre", paquete.IdHabitacion);
            ViewData["Servicios"] = servicios;
            ViewData["Precio"] = paquete.Costo;

            return PartialView("Edit", paquete);
        }

        // POST: Paquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Paquetes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaquete,NomPaquete,Descripcion,Costo,Estado,IdHabitacion")] Paquete paquete, int?[] ServiciosSeleccionados)
        {
            if (id != paquete.IdPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calcular el precio total sumando los precios de los servicios seleccionados
                    decimal precioTotal = 0;

                    if (ServiciosSeleccionados != null && ServiciosSeleccionados.Any())
                    {
                        var serviciosSeleccionados = await _context.Servicios
                            .Where(s => ServiciosSeleccionados.Contains(s.IdServicio))
                            .ToListAsync();

                        precioTotal = (decimal)serviciosSeleccionados.Sum(s => s.Costo);

                        // Actualizar la tabla intermedia PaqueteServicios
                        var serviciosExistentes = _context.PaqueteServicios
                            .Where(ps => ps.IdPaquete == id)
                            .ToList();

                        _context.PaqueteServicios.RemoveRange(serviciosExistentes);

                        foreach (var servicio in serviciosSeleccionados)
                        {
                            _context.PaqueteServicios.Add(new PaqueteServicio
                            {
                                IdPaquete = id,
                                IdServicio = servicio.IdServicio,
                                Costo = servicio.Costo // Asignar el precio del servicio
                            });
                        }
                    }

                    // Actualizar el precio total del paquete
                    paquete.Costo = (double?)precioTotal;

                    // Actualizar el paquete en la base de datos
                    _context.Update(paquete);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaqueteExists((int)paquete.IdPaquete))
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

            // Recargar servicios y habitaciones si el modelo no es válido
            ViewData["Servicios"] = _context.Servicios
                .Where(s => s.Estado == false)
                .Select(s => new
                {
                    s.IdServicio,
                    s.NomServicio,
                    s.Costo,
                    Seleccionado = ServiciosSeleccionados.Contains(s.IdServicio)
                })
                .ToList();

            ViewBag.IdHabitacion = new SelectList(await _context.Habitaciones.ToListAsync(), "IdHabitacion", "Nombre", paquete.IdHabitacion);

            return PartialView("Edit", paquete);
        }



        // GET: Paquetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .Include(p => p.oHabitacion)
                .Include(p => p.PaqueteServicios)

                .FirstOrDefaultAsync(m => m.IdPaquete == id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }

        private bool PaqueteExists(int id)
        {
            return _context.Paquetes.Any(e => e.IdPaquete == id);
        }

        // POST: Paquetes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var paquete = _context.Paquetes
                .Include(p => p.PaqueteServicios) // Incluir la relación con los servicios
                .FirstOrDefault(p => p.IdPaquete == id);

            if (paquete == null)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el paquetes si esta asociado a una reserva.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si el paquete tiene servicios asociados
            if (paquete.PaqueteServicios.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el paquete porque tiene servicios asociados.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Eliminar el paquete de la base de datos
                _context.Paquetes.Remove(paquete);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "El paquete se eliminó correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al intentar eliminar el paquete: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}