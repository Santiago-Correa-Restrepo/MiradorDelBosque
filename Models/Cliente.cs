using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ultima.Models
{
    public partial class Cliente
    {
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El número de documento debe contener exactamente 10 dígitos.")]
        [Display(Name = "Número de documento")]
        public int NroDocumento { get; set; }

        [Required(ErrorMessage = "El tipo de dócumento es obligatorio.")]
        [Display(Name = "Tipo de Documento")]
        public int IdTipoDocumento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El nombre solo debe contener letras.")]
        [StringLength(50, ErrorMessage = "El nombre debe tener un máximo de 50 caracteres.")]
        public string Nombres { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "El apellido solo debe contener letras.")]
        [StringLength(50, ErrorMessage = "El apellido debe tener un máximo de 50 caracteres.")]
        public string Apellidos { get; set; } = null!;

        [Required(ErrorMessage = "El número de celular es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El número de celular debe contener exactamente 10 dígitos.")]
        public string Celular { get; set; } = null!;

        [EmailAddress(ErrorMessage = "El formato del correo es inválido.")]
        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string Correo { get; set; } = null!;

        //[StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
        //[Required(ErrorMessage = "La contraseña es obligatoria.")]
        //[Display(Name = "Contraseña")]
        public string? Contrasena { get; set; } 

        public bool Estado { get; set; }

        //[Required(ErrorMessage = "El rol es obligatorio.")]
        //[Display(Name = "Rol")]
        public int? IdRol { get; set; }

        public virtual Role? IdRolNavigation { get; set; }

        //public virtual TipoDocumento IdTipoDocumentoNavigation { get; set; } = null!;

        [Display(Name = "Rol")]
        public virtual TipoDocumento? oTipoDocumento { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    }
}