using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ultima.Models.ViewModels
{
    public class PaqueteVM
    {
        public Paquete oPaquete { get; set; }
        public List<SelectListItem> oListaHabitaciones { get; set; }
        public List<Servicio> oListaServiciosSeleccionados { get; set; }
    }
}
