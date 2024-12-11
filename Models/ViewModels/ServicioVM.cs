using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ultima.Models.ViewModels
{
    public class ServicioVM
    {
        public Servicio oServicio { get; set; }
        public List<SelectListItem> oListaTipoServicio { get; set; }

        //public int ServicioId { get; set; }
        //public string NombreServicio { get; set; }


    }
}
