namespace Ultima.Models.ViewModels
{
    public class ReservaDetallesVM
    {
        public Reserva Reserva { get; set; }
        public Cliente Cliente { get; set; }
        public Paquete PaqueteAsociado { get; set; }
        public List<PaqueteServicio> ServiciosPaquete { get; set; }
        public List<DetalleReservaServicio> ServiciosReserva { get; set; }
    }
}
