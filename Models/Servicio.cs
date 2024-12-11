using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [Display(Name = "Tipo de Servicio")]
    public int? IdTipoServicio { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [MaxLength(20, ErrorMessage = "El campo no puede tener más de 20 caracteres.")]
    [MinLength(5, ErrorMessage = "El campo debe tener al menos 5 caracteres.")]
    [Display(Name = "Nombre Servicio")]
    public string? NomServicio { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public double? Costo { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [MaxLength(100, ErrorMessage = "El campo no puede tener más de 100 caracteres.")]
    [MinLength(5, ErrorMessage = "El campo debe tener al menos 5 caracteres.")]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<DetalleReservaServicio> DetalleReservaServicios { get; set; } = new List<DetalleReservaServicio>();

    public virtual TipoServicio? oTipoServicio { get; set; }

    public virtual ICollection<ImagenServicio> ImagenServicios { get; set; } = new List<ImagenServicio>();

    public virtual ICollection<PaqueteServicio> PaqueteServicios { get; set; } = new List<PaqueteServicio>();
}