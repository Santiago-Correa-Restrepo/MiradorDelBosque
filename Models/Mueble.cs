using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultima.Models;

public partial class Mueble
{
    public int IdMueble { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaRegistro { get; set; }

    public decimal? Valor { get; set; }

    public bool Estado { get; set; }

    public byte[]? Imagen { get; set; }

    public int Cantidad { get; set; }

    [NotMapped] // No se mapea a la base de datos
    public string? ImagenDataURL { get; set; }


    public virtual ICollection<HabitacionMueble> HabitacionMuebles { get; set; } = new List<HabitacionMueble>();
}
