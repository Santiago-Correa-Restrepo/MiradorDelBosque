using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class TipoHabitacione
{
    public int IdTipoHabitacion { get; set; }
    [Display(Name = "Tipo Habitacion")]
    public string? NomTipoHabitacion { get; set; }

    [Display(Name = "Numero Personas")]
    public int? NumeroPersonas { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Habitacione> Habitaciones { get; set; } = new List<Habitacione>();
}
