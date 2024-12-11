using System;
using System.Collections.Generic;

namespace Ultima.Models;

public partial class ImagenAbono
{
    public int? IdImagenAbono { get; set; }

    public int? IdImagen { get; set; }

    public int? IdAbono { get; set; }

    public virtual Abono? oAbono { get; set; }

    public virtual Imagene? oImagen { get; set; }
}
