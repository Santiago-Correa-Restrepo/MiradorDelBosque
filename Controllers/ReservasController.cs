using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Ultima.Models;
using Ultima.Models.ViewModels;
using RazorPDF;


namespace Ultima.Controllers
{
    public class ReservasController : Controller
    {
        private readonly BdGlampingFinalContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private object document;

        public ReservasController(BdGlampingFinalContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        internal class PdfActionResult : ActionResult
        {
            private ReservaVM reserva;

            public PdfActionResult(ReservaVM reserva)
            {
                this.reserva = reserva;
            }
        }

        // GET: Reservas
        //[Authorize(Policy = "ListarReservasPermiso")]
        public async Task<IActionResult> Index()
        {
            var reservas = ObtenerTodasLasReservas();

            return View(reservas);
        }

        //[Authorize(Policy = "CrearReservasPermiso")]
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.PaquetesDisponibles = ObtenerPaquetesDisponibles();
            ViewBag.ServiciosDisponibles = _context.Servicios.Where(s => s.Estado == true && (s.IdServicio != 1 && s.IdServicio != 2 && s.IdServicio != 3))
                .ToList();

            return View(CargarDatosIniciales());

        }

        //[Authorize(Policy = "CrearReservasPermiso")]
        [HttpPost]
        public IActionResult Crear(Reserva oReserva, string paqueteSeleccionado, string serviciosSeleccionados)
        {
            ViewBag.PaquetesDisponibles = _context.Paquetes.Where(s => s.Estado == true)
                     .ToList(); ;

            ViewBag.ServiciosDisponibles = _context.Servicios
                .Where(s => s.Estado == true && (s.IdServicio != 1 && s.IdServicio != 2 && s.IdServicio != 3))
                .ToList();

            ViewData["Error"] = "True";

            if (string.IsNullOrEmpty(paqueteSeleccionado))
            {
                ModelState.AddModelError("paqueteSeleccionados", "Seleccione un paquete");
                return View(CargarDatosIniciales());
            }

            if (string.IsNullOrEmpty(serviciosSeleccionados) || serviciosSeleccionados == "[]")
            {
                ViewData["ErrorServicio"] = "True";
                return View(CargarDatosIniciales());
            }

            if (!ModelState.IsValid)
            {
                return View(CargarDatosIniciales());
            }

            if (!Existe(oReserva.NroDocumentoCliente))
            {
                ModelState.AddModelError("oReserva.NroDocumentoCliente", "El cliente no existe");
                return View(CargarDatosIniciales());
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.NroDocumento == oReserva.NroDocumentoCliente);

            if (cliente.Estado == false)
            {
                ModelState.AddModelError("oReserva.NroDocumentoCliente", "El cliente está inhabilitado");
                return View(CargarDatosIniciales());
            }

            if (!ValidarFechas(oReserva))
            {
                return View(CargarDatosIniciales());
            }

            if (oReserva.Descuento == null)
            {
                oReserva.Descuento = 0;
            }

            // Guardar la reserva
            _context.Reservas.Add(oReserva);
            _context.SaveChanges();

            // Procesar paquetes seleccionados
            var listaPaqueteSeleccionado = JsonConvert.DeserializeObject<List<dynamic>>(paqueteSeleccionado.ToString());
            if (listaPaqueteSeleccionado != null && listaPaqueteSeleccionado.Any())
            {
                var paquetes = listaPaqueteSeleccionado.Select(paquete => new Paquete
                {
                    IdPaquete = Convert.ToInt32(paquete.id),
                    Costo = Convert.ToDouble(paquete.costo)
                }).ToList();

                foreach (var paquete in paquetes)
                {
                    var DetalleReservaPaquete = new DetalleReservaPaquete
                    {
                        IdReserva = oReserva.IdReserva,
                        IdPaquete = paquete.IdPaquete,
                        Costo = paquete.Costo
                    };
                    _context.DetalleReservaPaquetes.Add(DetalleReservaPaquete);

                    // Actualizar el estado del paquete a "no disponible"
                    var paqueteDb = _context.Paquetes.Find(paquete.IdPaquete);
                    if (paqueteDb != null)
                    {
                        paqueteDb.Estado = false; // Cambiar el estado
                        _context.Entry(paqueteDb).State = EntityState.Modified;
                    }
                }
            }

            if (!Existe(oReserva.NroDocumentoCliente))
            {
                ViewBag.ClienteNoExiste = true; // Indica que el cliente no existe
                ModelState.AddModelError("oReserva.NroDocumentoCliente", "El cliente no existe");
                return View(CargarDatosIniciales());
            }

            // Procesar servicios seleccionados
            if (!string.IsNullOrEmpty(serviciosSeleccionados))
            {
                var listaServiciosSeleccionados = JsonConvert.DeserializeObject<List<dynamic>>(serviciosSeleccionados.ToString());
                if (listaServiciosSeleccionados != null && listaServiciosSeleccionados.Any())
                {
                    foreach (var servicio in listaServiciosSeleccionados)
                    {
                        var DetalleReservaServicio = new DetalleReservaServicio
                        {
                            IdReserva = oReserva.IdReserva,
                            IdServicio = Convert.ToInt32(servicio.id),
                            Costo = Convert.ToDouble(servicio.costo),
                            Cantidad = servicio.cantidad ?? 1
                        };
                        _context.DetalleReservaServicios.Add(DetalleReservaServicio);
                    }
                }
            }

            // Guardar cambios finales
            _context.SaveChanges();

            return RedirectToAction("Index", "Reservas");
        }
        
        //[Authorize(Policy = "EditarReservasPermiso")]
        [HttpGet]
        public IActionResult Editar(int ReservaId)
        {
            ViewBag.PaqueteAsociado = ObtenerPaqueteAsociado(ReservaId);

            var paqueteAsociado = _context.DetalleReservaPaquetes
                .Where(p => p.IdReserva == ReservaId)
                .Select(p => p.IdPaquete)
                .FirstOrDefault();

            ViewBag.ServiciosAsociados = ObtenerServiciosAsociados(ReservaId);

            ViewBag.CantidadesServiciosAsociados = CantidadesServicios(ReservaId);

            ViewBag.PaquetesDisponibles = _context.Paquetes
                .Where(p => p.IdPaquete != paqueteAsociado && p.Estado == true)
                .ToList();

            ViewBag.ServiciosDisponibles = _context.Servicios.Where(s => s.Estado == true && (s.IdServicio != 1 && s.IdServicio != 2 && s.IdServicio != 3))
                .ToList();


            return View(CargarDatosReserva(ReservaId));

        }



        //[Authorize(Policy = "EditarReservasPermiso")]
        [HttpPost]
        public IActionResult Editar(Reserva oReserva, string paqueteSeleccionado, string serviciosSeleccionados)
        {
            ViewBag.PaqueteAsociado = ObtenerPaqueteAsociado(oReserva.IdReserva);

            ViewBag.ServiciosAsociados = ObtenerServiciosAsociados(oReserva.IdReserva);

            ViewBag.CantidadesServiciosAsociados = CantidadesServicios(oReserva.IdReserva);

            var paqueteAsociado = ObtenerPaqueteAsociado(oReserva.IdReserva);

            ViewBag.PaquetesDisponibles = _context.Paquetes
                .Where(p => p.IdPaquete != paqueteAsociado.IdPaquete && p.Estado == true)
                .ToList();

            ViewBag.ServiciosDisponibles = _context.Servicios.Where(s => s.Estado == true && (s.IdServicio != 1 && s.IdServicio != 2 && s.IdServicio != 3))
                .ToList();

            if (!ModelState.IsValid)
            {
                return View(CargarDatosReserva(oReserva.IdReserva));
            }

            if (!Existe(oReserva.NroDocumentoCliente))
            {
                ModelState.AddModelError("oReserva.NroDocumentoCliente", "El cliente no existe");
                return View(CargarDatosReserva(oReserva.IdReserva));
            }

            if (string.IsNullOrEmpty(paqueteSeleccionado))
            {
                ModelState.AddModelError("paqueteSeleccionados", "Este campo es obligatorio");
                return View(CargarDatosReserva(oReserva.IdReserva));
            }

            if (string.IsNullOrEmpty(serviciosSeleccionados) || serviciosSeleccionados == "[]")
            {
                ViewData["ErrorServicio"] = "True";
                return View(CargarDatosReserva(oReserva.IdReserva));
            }

            if (!ValidarFechas(oReserva))
            {
                return View(CargarDatosReserva(oReserva.IdReserva));
            }

            if (oReserva.Descuento == null)
            {
                oReserva.Descuento = 0;
            }

            var listaAbonos = _context.Abonos
                .Where(a => a.IdReserva == oReserva.IdReserva)
                .ToList();

            var costoOriginalReserva = _context.Reservas
                .Where(r => r.IdReserva == oReserva.IdReserva)
                .Select(r => r.MontoTotal)
                .FirstOrDefault();

            if (oReserva.MontoTotal != costoOriginalReserva && listaAbonos.Count != 0)
            {
                foreach (var abono in listaAbonos)
                {
                    var valorDescuento = oReserva.SubTotal * (1 - (oReserva.Descuento / 100));
                    var nuevoPorcentaje = (abono.SubTotal / valorDescuento) * 100;
                    var porcentajeRedondeado = Math.Floor(nuevoPorcentaje.Value);

                    abono.ValorDeuda = valorDescuento;
                    abono.Porcentaje = porcentajeRedondeado;
                }
            }

            if (oReserva.IdEstadoReserva == 6)
            {
                var oAbono = new AbonoController(_context, _webHostEnvironment);
                bool validacion = oAbono.ObtenerPendiente(oReserva.IdReserva) == 0;

                if (!validacion)
                {
                    ViewBag.ErrorFinalizacion = "True";
                    return View(CargarDatosReserva(oReserva.IdReserva));
                }

            }

            guardarPaqueteSeleccionado(oReserva, paqueteSeleccionado);
            guardarServiciosSeleccionados(oReserva, serviciosSeleccionados);

            _context.Reservas.Update(oReserva);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //[Authorize(Policy = "VerDetallesReservasPermiso")]
        public IActionResult Detalles(int ReservaId)
        {
            ViewBag.PaqueteAsociado = ObtenerPaqueteAsociado(ReservaId);

            var paqueteAsociado = _context.DetalleReservaPaquetes
                .Where(p => p.IdReserva == ReservaId)
                .Select(p => p.IdPaquete)
                .FirstOrDefault();

            ViewBag.ServiciosAsociadospaquete = ObtenerServiciosAsociadosPaquete(paqueteAsociado);

            ViewBag.ServiciosAsociados = ObtenerServiciosAsociados(ReservaId);

            ViewBag.CantidadesServiciosAsociados = CantidadesServicios(ReservaId);

            return View(CargarDatosReserva(ReservaId));
        }

        //[Authorize(Policy = "AnularReservaPermiso")]
        public IActionResult AnularReserva(int idReserva)
        {
            // Buscar la reserva con sus detalles relacionados
            var reserva = _context.Reservas
                .Include(r => r.DetalleReservaPaquetes)
                .ThenInclude(drp => drp.oPaquete) // Incluir los paquetes asociados
                .FirstOrDefault(r => r.IdReserva == idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            // Cambiar el estado de la reserva a "Anulada"
            reserva.IdEstadoReserva = 5; // 5 representa "Anulado"

            // Liberar los paquetes asociados
            foreach (var detalle in reserva.DetalleReservaPaquetes)
            {
                var paquete = detalle.oPaquete;
                if (paquete != null)
                {
                    paquete.Estado = true; // Marcar el paquete como disponible
                }
            }

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Metodos 

        public ReservaVM CargarDatosReserva(int? idReserva)
        {

            ReservaVM oReservaVM = new ReservaVM()
            {
                oReserva = _context.Reservas
                .Where(r => r.IdReserva == idReserva)
                .FirstOrDefault(),
                oListaEstados = _context.EstadosReservas.Select(reservas => new SelectListItem()
                {
                    Text = reservas.NombreEstadoReserva,
                    Value = reservas.IdEstadoReserva.ToString()
                }).ToList(),
                oListaMetodoPagos = _context.MetodoPagos.Select(reservas => new SelectListItem()
                {
                    Text = reservas.NomMetodoPago,
                    Value = reservas.IdMetodoPago.ToString()
                }).ToList()
            };

            return oReservaVM;
        }

        public ReservaVM CargarDatosIniciales()
        {

            ReservaVM oReservaVM = new ReservaVM()
            {
                oReserva = new Reserva(),
                oListaMetodoPagos = _context.MetodoPagos.Select(reservas => new SelectListItem()
                {
                    Text = reservas.NomMetodoPago,
                    Value = reservas.IdMetodoPago.ToString()
                }).ToList()
            };

            return oReservaVM;
        }

        public bool ValidarFechas(Reserva oReserva)
        {

            if (oReserva.FechaFinalizacion < oReserva.FechaInicio)
            {
                ModelState.AddModelError("oReserva.FechaFinalizacion", "La fecha de finalizacion debe ser posterior a la fecha de inicio");
                return false;
            }
            else if (oReserva.FechaReserva >= oReserva.FechaInicio)
            {
                ModelState.AddModelError("oReserva.FechaInicio", "La fecha de inicio debe ser mayor que la fecha de la reserva");
                return false;
            }

            return true;
        }

        public IActionResult BuscarCliente(int searchTerm)
        {
            var informacionCliente = _context.Clientes
                .Include(s => s.oTipoDocumento)
                .Where(s => s.NroDocumento == searchTerm)
                .FirstOrDefault();

            if (informacionCliente == null)
            {
                return Json(new
                {
                    nombre = "",
                    apellido = "",
                    tipoDocumento = "",
                    correo = "",
                    celular = "",
                });
            }
            else if (informacionCliente.Estado == false)
            {
                return Json(new
                {
                    estado = "false",
                });
            }
            else
            {
                return Json(new
                {
                    nombre = informacionCliente.Nombres,
                    apellido = informacionCliente.Apellidos,
                    tipoDocumento = informacionCliente.oTipoDocumento.NomTipoDcumento,
                    correo = informacionCliente.Correo,
                    celular = informacionCliente.Celular
                });
            }
        }

        public bool Existe(int cliente)
        {
            return _context.Clientes.Any(c => c.NroDocumento == cliente);
        }

        public IActionResult Buscar(int searchTerm)
        {
            List<Reserva> resultados;

            if (string.IsNullOrEmpty(searchTerm.ToString()))
            {
                resultados = ObtenerTodasLasReservas();
            }
            else
            {
                resultados = ObtenerResultadosDeLaBaseDeDatos(searchTerm);
            }

            return PartialView("_ResultadoBusquedaParcial", resultados);
        }

        public List<int?> CantidadesServicios(int? reservaId)
        {
            var serviciosAsociados = ObtenerServiciosAsociados(reservaId);

            List<int?> listaCantidades = new List<int?>();

            foreach (var servicio in serviciosAsociados)
            {
                var cantidad = _context.DetalleReservaServicios
                    .Where(drs => drs.IdServicio == servicio.IdServicio && drs.IdReserva == reservaId)
                    .Select(drs => drs.Cantidad)
                    .FirstOrDefault();

                listaCantidades.Add(cantidad);
            }

            return listaCantidades;

        }

        public List<Reserva> ObtenerTodasLasReservas()
        {
            var todasLasReservas = _context.Reservas
                .Include(e => e.oEstadoReserva)
                .Include(e => e.oMetodoPago)
                .Include(e => e.oCliente)
                    .ThenInclude(e => e.oTipoDocumento)
                .Include(e => e.oUsuario)
                .Include(e => e.DetalleReservaPaquetes)
                    .ThenInclude(drp => drp.oPaquete)
                        .ThenInclude(p => p.oHabitacion)
                            .ThenInclude(h => h.oTipoHabitacion)
                .ToList();

            return todasLasReservas;
        }

        public List<Reserva> ObtenerResultadosDeLaBaseDeDatos(int? searchTerm)
        {
            if (searchTerm.HasValue && DateOnly.TryParse(searchTerm.Value.ToString(), out DateOnly fecha))
            {
                DateTime fechaComoDateTime = fecha.ToDateTime(TimeOnly.MinValue); // Convierte DateOnly a DateTime.
                var resultados = _context.Reservas
                    .Include(e => e.oEstadoReserva)
                    .Include(e => e.oMetodoPago)
                    .Include(e => e.oCliente)
                    .Include(e => e.oUsuario)
                    .Where(e => e.FechaReserva.HasValue && e.FechaReserva.Value.Date == fechaComoDateTime.Date) // Compara las fechas.
                    .ToList();

                return resultados;
            }
            else
            {
                var resultados = _context.Reservas
                    .Include(e => e.oEstadoReserva)
                    .Include(e => e.oMetodoPago)
                    .Include(e => e.oCliente)
                    .Include(e => e.oUsuario)
                    .Where(e => e.NroDocumentoCliente.ToString().Contains(searchTerm.ToString()))
                    .ToList();

                return resultados;
            }
        }

        public Paquete ObtenerPaqueteAsociado(int? reservaId)
        {
            return _context.DetalleReservaPaquetes
                .Where(p => p.IdReserva == reservaId)
                .Select(p => p.oPaquete)
                .FirstOrDefault();
        }

        public List<DetalleReservaServicio> ObtenerServiciosAsociados(int? ReservaId)
        {
            return _context.DetalleReservaServicios
                .Where(drs => drs.IdReserva == ReservaId)
                .Include(drs => drs.oServicio)
                .ToList();
        }

        public List<Servicio> ObtenerServiciosAsociadosPaquete(int? idPaquete)
        {
            return _context.PaqueteServicios
                .Where(p => p.IdPaquete == idPaquete)
                .Select(p => p.oServicio)
                .ToList();
        }

        public List<Paquete> ObtenerPaquetesDisponibles()
        {
            // Obtener habitaciones asociadas a reservas activas (no anuladas ni finalizadas)
            var habitacionesReservadas = _context.DetalleReservaPaquetes
                .Where(drp => drp.oReserva.IdEstadoReserva != 5 && drp.oReserva.IdEstadoReserva != 6) // Excluir anuladas y finalizadas
                .Select(drp => drp.oPaquete.IdHabitacion)
                .Distinct()
                .ToList();

            // Filtrar paquetes disponibles
            var paquetesDisponibles = _context.Paquetes
                .Where(p => !habitacionesReservadas.Contains(p.IdHabitacion) && p.Estado == true) // Excluir habitaciones reservadas y paquetes inactivos
                .ToList();

            return paquetesDisponibles;
        }

        public IActionResult ObtenerInfoBasicaPaquete(int reservaId)
        {
            var detalle = _context.DetalleReservaPaquetes
                .Where(p => p.IdReserva == reservaId)
                .FirstOrDefault();

            return Json(new
            {
                costo = detalle.Costo
            });
        }

        public IActionResult ObtenerCostoServicio(int servicioId)
        {
            var costoServicio = _context.Servicios
                                        .Where(s => s.IdServicio == servicioId)
                                        .Select(s => s.Costo)
                                        .FirstOrDefault();

            return Json(new { costo = costoServicio });
        }

        public IActionResult ObtenerInfoPaquete(int paqueteId)
        {
            var Informacionpaquete = _context.Paquetes
                                  .Include(s => s.oHabitacion)
                                    .ThenInclude(s => s.oTipoHabitacion)
                                  .Where(s => s.IdPaquete == paqueteId)
                                  .FirstOrDefault();

            return Json(new
            {
                nombre = Informacionpaquete.NomPaquete,
                costo = Informacionpaquete.Costo,
                habitacion = Informacionpaquete.oHabitacion.Nombre,
                nroPersonas = Informacionpaquete.oHabitacion.oTipoHabitacion.NumeroPersonas,
            });

        }

        public IActionResult ObtenerServiciosPaquete(int paqueteId)
        {
            var serviciosPaquete = _context.PaqueteServicios
                .Where(e => e.IdPaquete == paqueteId)
                .Include(e => e.oServicio)
                .ToList();

            var servicioData = serviciosPaquete.Select(e => new
            {
                nombre = e.oServicio.NomServicio,
                costo = e.Costo
            });

            return Json(servicioData);
        }

        public bool guardarPaqueteSeleccionado(Reserva oReserva, string paqueteSeleccionado)
        {
            var PaqueteSeleccionadoObj = JsonConvert.DeserializeObject<List<dynamic>>(paqueteSeleccionado.ToString());

            if (PaqueteSeleccionadoObj != null && PaqueteSeleccionadoObj.Count > 0)
            {

                var primerPaquete = PaqueteSeleccionadoObj[0];

                var nuevoPaquete = new Paquete()
                {
                    IdPaquete = primerPaquete.id,
                    Costo = primerPaquete.costo
                };

                var paqueteOriginal = _context.DetalleReservaPaquetes
                    .Where(drp => drp.IdReserva == oReserva.IdReserva)
                    .Select(drp => drp.oPaquete)
                    .FirstOrDefault();

                var detallePaqueteExistente = _context.DetalleReservaPaquetes
                   .FirstOrDefault(d => d.IdReserva == oReserva.IdReserva && d.IdPaquete == paqueteOriginal.IdPaquete);


                if (nuevoPaquete.IdPaquete != paqueteOriginal.IdPaquete)
                {

                    detallePaqueteExistente.IdPaquete = nuevoPaquete.IdPaquete;
                    detallePaqueteExistente.Costo = nuevoPaquete.Costo;

                    _context.DetalleReservaPaquetes.Update(detallePaqueteExistente);

                }

                return true;

            }

            return false;
        }

        public bool guardarServiciosSeleccionados(Reserva oReserva, string serviciosSeleccionados)
        {
            var ServiciosSeleccionadosObj = JsonConvert.DeserializeObject<List<dynamic>>(serviciosSeleccionados.ToString());

            if (ServiciosSeleccionadosObj != null && ServiciosSeleccionadosObj.Any())
            {

                var serviciosNuevos = new List<Servicio>();
                var cantidadeServiciosNuevos = new List<int>();

                for (var i = 0; i < ServiciosSeleccionadosObj.Count; i++)
                {
                    var servicio = new Servicio()
                    {
                        IdServicio = Convert.ToInt32(ServiciosSeleccionadosObj[i].id),
                        NomServicio = ServiciosSeleccionadosObj[i].nombre.ToString(),
                        Costo = Convert.ToDouble(ServiciosSeleccionadosObj[i].costo)
                    };

                    var cantidad = ServiciosSeleccionadosObj[i].cantidad;

                    serviciosNuevos.Add(servicio);

                    if (cantidad == null)
                    {
                        cantidad = 1;
                    }

                    cantidadeServiciosNuevos.Add((int)cantidad);
                }

                var serviciosOriginales = _context.DetalleReservaServicios
                    .Where(drs => drs.IdReserva == oReserva.IdReserva)
                    .Select(drs => drs.oServicio.IdServicio)
                    .ToList();

                var serviciosAEliminar = serviciosOriginales.Except(serviciosNuevos.Select(s => s.IdServicio)).ToList();
                var serviciosAAgregar = serviciosNuevos.Select(s => s.IdServicio).Except(serviciosOriginales).ToList();

                if (serviciosAEliminar.Count != 0)
                {

                    foreach (var servicioEliminar in serviciosAEliminar)
                    {

                        var detalle = _context.DetalleReservaServicios
                            .Where(drs => drs.IdReserva == oReserva.IdReserva && drs.IdServicio == servicioEliminar)
                            .FirstOrDefault();

                        _context.DetalleReservaServicios.Remove(detalle);

                    }

                }

                if (serviciosAAgregar.Count != 0)
                {

                    for (var i = 0; i < serviciosAAgregar.Count; i++)
                    {
                        var detalle = new DetalleReservaServicio()
                        {
                            IdReserva = oReserva.IdReserva,
                            IdServicio = serviciosAAgregar[i],
                            Costo = _context.Servicios
                                        .Where(s => s.IdServicio == serviciosAAgregar[i])
                                        .Select(s => s.Costo)
                                        .FirstOrDefault(),
                            Cantidad = cantidadeServiciosNuevos[i]
                        };

                        _context.DetalleReservaServicios.Add(detalle);
                    }

                }

                _context.SaveChanges();

                for (var i = 0; i < serviciosNuevos.Count; i++)
                {
                    var detalle = new DetalleReservaServicio()
                    {

                        IdReserva = oReserva.IdReserva,
                        IdServicio = serviciosNuevos[i].IdServicio,
                        Costo = serviciosNuevos[i].Costo,
                        Cantidad = cantidadeServiciosNuevos[i]

                    };

                    var detalleExistente = _context.DetalleReservaServicios
                        .FirstOrDefault(drs => drs.IdReserva == oReserva.IdReserva && drs.IdServicio == detalle.IdServicio);

                    if (detalleExistente != null)
                    {
                        detalleExistente.Cantidad = detalle.Cantidad;

                        _context.SaveChanges();
                    }
                }

                return true;

            }
            else
            {
                var serviciosAdicionales = _context.DetalleReservaServicios
                    .Where(d => d.IdReserva == oReserva.IdReserva)
                    .ToList();

                _context.DetalleReservaServicios.RemoveRange(serviciosAdicionales);

                return true;
            }
        }

        


        // Exportar PDF

        //    // Método GET
        //    [HttpGet]
        //    public IActionResult DescargarPDF(int idReserva)
        //    {
        //        return GenerarPDF(idReserva);
        //    }

        //    // Método POST
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public IActionResult DescargarPDFPost(int idReserva)
        //    {
        //        return GenerarPDF(idReserva);

        //    }

        //    private IActionResult GenerarPDF(int idReserva)
        //    {
        //        var reserva = _context.Reservas
        //            .Include(r => r.oCliente)
        //            .FirstOrDefault(r => r.IdReserva == idReserva);

        //        if (reserva == null)
        //        {
        //            return NotFound("Reserva no encontrada.");
        //        }

        //        var cliente = reserva.oCliente;
        //        var detallePaquete = _context.DetalleReservaPaquetes
        //            .Where(drp => drp.IdReserva == idReserva)
        //            .Include(drp => drp.oPaquete)
        //                .ThenInclude(p => p.oHabitacion)
        //            .FirstOrDefault();

        //        var servicios = _context.DetalleReservaServicios
        //            .Where(drs => drs.IdReserva == idReserva)
        //            .Include(drs => drs.oServicio)
        //                .ThenInclude(s => s.oTipoServicio)
        //            .ToList();

        //        using var memoryStream = new MemoryStream();

        //        // Crear el documento PDF
        //        using (var writer = new PdfWriter(memoryStream))
        //        {


        //            // Crear el documento PDF
        //            PdfWriter pdfWriter = new PdfWriter(memoryStream);
        //            using (var writer = pdfWriter)
        //            {
        //                using (var pdf = new PdfDocument(writer))
        //                {
        //                    var document = new iText.Layout.Document(pdf);

        //                    // Fuente en negrita
        //                    PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
        //                    PdfFont regularFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        //                    // Añadir logo
        //                    string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "Imagenes/Default/Logo.png");
        //                    if (System.IO.File.Exists(logoPath))
        //                    {
        //                        var image = new Image(iText.IO.Image.ImageDataFactory.Create(logoPath));
        //                        image.ScaleToFit(100, 50);
        //                        document.Add(image);
        //                    }

        //                    // Encabezado
        //                    document.Add(new Paragraph("EcoGlam")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(20));

        //                    document.Add(new Paragraph("Crr 67D cll 83-23").SetFont(regularFont));
        //                    document.Add(new Paragraph("3113234546").SetFont(regularFont));
        //                    document.Add(new Paragraph("ecoglam7@gmail.com").SetFont(regularFont));

        //                    // Datos del cliente
        //                    document.Add(new Paragraph("\nDatos del Cliente")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(14));

        //                    document.Add(new Paragraph($"Nombre: {cliente.Nombres} {cliente.Apellidos}").SetFont(regularFont));
        //                    document.Add(new Paragraph($"Documento: {cliente.NroDocumento}").SetFont(regularFont));
        //                    document.Add(new Paragraph($"Correo: {cliente.Correo}").SetFont(regularFont));

        //                    // Datos de la reserva
        //                    document.Add(new Paragraph("\nDatos de la Reserva")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(14));

        //                    document.Add(new Paragraph($"Fecha de Reserva: {reserva.FechaReserva:dd/MM/yyyy}").SetFont(regularFont));
        //                    document.Add(new Paragraph($"Fecha de Inicio: {reserva.FechaInicio:dd/MM/yyyy}").SetFont(regularFont));
        //                    document.Add(new Paragraph($"Fecha de Finalización: {reserva.FechaFinalizacion:dd/MM/yyyy}").SetFont(regularFont));
        //                    document.Add(new Paragraph($"Monto Total: {reserva.MontoTotal:C}").SetFont(regularFont));
        //                }
        //            }

        //            memoryStream.Seek(0, SeekOrigin.Begin);
        //        return File(memoryStream.ToArray(), "application/pdf", "ReservaDetalle.pdf");
        //    }
        //}

    }


}