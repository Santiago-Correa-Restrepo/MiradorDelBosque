using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ultima.Models;

namespace Ultima.Models;

public partial class Role
{
    [Display(Name = "ID Rol")]
    public int IdRol { get; set; }


    [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder los 50 caracteres.")]
    [Display(Name = "Nombre Rol")]
    public string? NomRol { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public bool Estado { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<PermisosRole> PermisosRoles { get; set; } = new List<PermisosRole>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}