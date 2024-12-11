using System;
using System.Collections.Generic;

namespace Ultima.Models;

public partial class ImagenPaquete
{
    public int IdImagenPaque { get; set; }

    public int IdImagen { get; set; }

    public int IdPaquete { get; set; }

    public virtual Imagene IdImagenNavigation { get; set; } = null!;

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
}
