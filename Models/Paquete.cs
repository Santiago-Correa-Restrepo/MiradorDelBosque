using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Ultima.Models;


public partial class Paquete
{
    public int IdPaquete { get; set; }

    [Required(ErrorMessage = "El nombre del paquete es obligatorio.")]
    [StringLength(20, ErrorMessage = "El nombre no debe tener más de 20 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\.,\-\s]+$", ErrorMessage = "Este campo solo puede contener letras.")]
    [Display(Name = "Nombre de Paquete")]
    public string? NomPaquete { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [RegularExpression(@"^(?!-).*", ErrorMessage = "El precio tiene que ser un número valido.")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public double? Costo { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    public int? IdHabitacion { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    public bool Estado { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    public string? Descripcion { get; set; }

    public virtual ICollection<DetalleReservaPaquete> DetalleReservaPaquetes { get; set; } = new List<DetalleReservaPaquete>();

    public virtual Habitacione oHabitacion { get; set; }

    public virtual ICollection<ImagenPaquete>? ImagenPaquetes { get; set; } = new List<ImagenPaquete>();

    public virtual ICollection<PaqueteServicio>? PaqueteServicios { get; set; } = new List<PaqueteServicio>();
}