using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ultima.Models.ViewModels
{
    public class ReservaVM
    {
        public Reserva oReserva { get; set; }
        public Cliente oCliente { get; set; }
        public List<SelectListItem> oListaEstados { get; set; }
        public List<SelectListItem> oListaMetodoPagos { get; set; }
    }
}


