using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ultima.Models;

public partial class DetalleReservaServicio
{
    public int? IdDetalleReservaServicio { get; set; }

    public int? IdServicio { get; set; }

    public int? IdReserva { get; set; }

    public int? Cantidad { get; set; }

    public double? Costo { get; set; }

    [JsonIgnore]
    public virtual Reserva? oReserva { get; set; }

    public virtual Servicio? oServicio { get; set; }
}
