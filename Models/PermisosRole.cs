using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models;

public partial class PermisosRole
{
    [Display(Name = "Id permisos roles")]
    public int IdPermisosRoles { get; set; }

    [Display(Name = "Id Rol")]
    public int? IdRol { get; set; }

    [Display(Name = "Id permiso")]
    public int? IdPermiso { get; set; }

    [Display(Name = "Id navegación")]
    public virtual Permiso? IdPermisoNavigation { get; set; }

    [Display(Name = "Rol de navegación")]
    public virtual Role? IdRolNavigation { get; set; }
}

