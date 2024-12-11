using Microsoft.AspNetCore.Mvc.Rendering;
using Ultima.Models;

namespace Ultima.Models.ViewModels
{
    public class UsuarioVM
    {
        public Usuario oUsuario { get; set; }
        public List<SelectListItem> oListaTipoDocumento { get; set; }
    }
}