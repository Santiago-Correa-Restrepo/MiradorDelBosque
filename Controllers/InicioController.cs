// InicioController.cs
using Microsoft.AspNetCore.Mvc;
using Ultima.Models;
using Ultima.Recursos;
using Ultima.Servicios.Contrato;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Ultima.Controllers
{
    public class InicioController : Controller
    {
        private readonly IClienteService _clienteServicio;
        private readonly EmailSender _emailSender;
        private static Dictionary<string, string> _recoveryCodes = new Dictionary<string, string>();

        public InicioController(IClienteService clienteServicio)
        {
            _clienteServicio = clienteServicio;
            _emailSender = new EmailSender();
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(Cliente modelo)
        {
            modelo.Contrasena = Utilidades.EncriptarClave(modelo.Contrasena);

            Cliente cliente_creado = await _clienteServicio.SaveCliente(modelo);

            if (cliente_creado != null)
            {
                string subject = "Bienvenido a Mirador Del Bosque";
                string message = "<h1>Registro exitoso</h1><p>Gracias por registrarte en nuestra aplicación.</p>";
                await _emailSender.SendEmailAsync(cliente_creado.Correo, subject, message);
                return RedirectToAction("IniciarSesion", "Inicio");
            }

            ViewData["mensaje"] = "No se pudo crear el cliente";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewData["mensaje"] = "El correo y la contraseña son obligatorios.";
                return View();
            }

            Cliente cliente_encontrado = await _clienteServicio.GetCliente(correo, Utilidades.EncriptarClave(contrasena));

            if (cliente_encontrado == null)
            {
                ViewData["mensaje"] = "Credenciales incorrectas.";
                return View();
            }

            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, cliente_encontrado.Nombres) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties { AllowRefresh = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            string subject = "Inicio de sesión exitoso";
            string message = "<h1>¡Bienvenido de nuevo!</h1><p>Has iniciado sesión correctamente.</p>";
            await _emailSender.SendEmailAsync(correo, subject, message);

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContrasena(string correo)
        {
            Cliente cliente = await _clienteServicio.GetClienteByEmail(correo);

            if (cliente == null)
            {
                ViewData["mensaje"] = "Correo no encontrado";
                return View();
            }

            var code = new Random().Next(100000, 999999).ToString();
            _recoveryCodes[correo] = code;
            await _emailSender.SendRecoveryCodeAsync(correo, code);
            TempData["correo"] = correo;
            return RedirectToAction("VerificarCodigo", "Inicio");
        }

        public IActionResult VerificarCodigo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerificarCodigo(string correo, string codigo)
        {
            if (_recoveryCodes.ContainsKey(correo) && _recoveryCodes[correo] == codigo)
            {
                _recoveryCodes.Remove(correo);
                TempData["correo"] = correo;
                return RedirectToAction("RestablecerContrasena", "Inicio");
            }

            ViewData["mensaje"] = "Código incorrecto";
            return View();
        }

        public IActionResult RestablecerContrasena()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContrasena(string correo, string nuevaContrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(nuevaContrasena))
            {
                ViewData["mensaje"] = "Correo o contraseña no válidos.";
                return View();
            }

            Cliente cliente = await _clienteServicio.GetClienteByEmail(correo);

            if (cliente == null)
            {
                ViewData["mensaje"] = "No se encontró el cliente.";
                return View();
            }

            cliente.Contrasena = Utilidades.EncriptarClave(nuevaContrasena);
            bool actualizado = await _clienteServicio.UpdateCliente(cliente);

            if (actualizado)
            {
                TempData["mensaje"] = "Contraseña restablecida con éxito.";
                return RedirectToAction("IniciarSesion", "Inicio");
            }

            ViewData["mensaje"] = "No se pudo restablecer la contraseña.";
            return View();
        }
    }
}


