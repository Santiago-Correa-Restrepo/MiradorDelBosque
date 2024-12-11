using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class TipoServicio
{
    [Display(Name = "Tipo Servicio")]
    public int? IdTipoServicio { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [MaxLength(20, ErrorMessage = "El campo no puede tener más de 20 caracteres.")]
    [MinLength(5, ErrorMessage = "El campo debe tener al menos 5 caracteres.")]
    [Display(Name = "Tipo Servicio")]
    public string? NombreTipoServicio { get; set; }

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
