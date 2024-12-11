using System;
using System.Collections.Generic;

namespace Ultima.Models;

public partial class ImagenHabitacion
{
    public int IdImagenHabi { get; set; }

    public int IdImagen { get; set; }

    public int IdHabitacion { get; set; }

    public virtual Habitacione IdHabitacionNavigation { get; set; } = null!;

    public virtual Imagene IdImagenNavigation { get; set; } = null!;
}
