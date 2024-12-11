using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class Permiso
{

    public int IdPermiso { get; set; }

    public string? NomPermiso { get; set; }

    [Required(ErrorMessage = "Tiene que seleccionar un permiso como mínimo.")]
    [Display(Name = "Permisos seleccionados")]
    public string PermisosSeleccionados { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<PermisosRole> PermisosRoles { get; set; } = new List<PermisosRole>();
}
