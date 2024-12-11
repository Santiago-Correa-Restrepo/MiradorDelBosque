using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Ultima.Models;

public partial class Abono
{
    public int IdAbono { get; set; }

    public int IdReserva { get; set; }

    public DateOnly? FechaAbono { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public double? ValorDeuda { get; set; }

    public double? Porcentaje { get; set; }

    public double? Pendiente { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public double? SubTotal { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]

    public double? Iva { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public double? CantAbono { get; set; }

    public bool? Estado { get; set; }

    public virtual Reserva? oReserva { get; set; }

    public virtual ICollection<ImagenAbono> ImagenAbonos { get; set; } = new List<ImagenAbono>();
    
}