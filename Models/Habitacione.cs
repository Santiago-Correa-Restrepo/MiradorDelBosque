using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class Habitacione
{
    public int IdHabitacion { get; set; }

    public string Nombre { get; set; } = null!;

    [Display(Name = "Tipo Habitacion")]
    public int IdTipoHabitacion { get; set; }

    public string? Descripcion { get; set; }

    [Display(Name = "Cantidad de habitaciones")]
    public int? Cantidad { get; set; }

    [Display(Name = "Fecha Registro")]
    public DateOnly FechaRegistro { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal Costo { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();

    public virtual TipoHabitacione? oTipoHabitacion { get; set; }

    public virtual ICollection<ImagenHabitacion> ImagenHabitacions { get; set; } = new List<ImagenHabitacion>();

    public virtual ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
}
