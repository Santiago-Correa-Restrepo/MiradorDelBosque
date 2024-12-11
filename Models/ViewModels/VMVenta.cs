namespace Ultima.Models.ViewModels
{
    public class VMVenta
    {

        public int IdReserva { get; set; }

        public int NroDocumentoCliente { get; set; }

        public int NroDocumentoUsuario { get; set; }

        public DateTime FechaReserva { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFinalizacion { get; set; }

        public double SubTotal { get; set; }

        public double Descuento { get; set; }

        public double Iva { get; set; }

        public double MontoTotal { get; set; }

        public int MetodoPago { get; set; }

        public int NroPersonas { get; set; }

        public int IdEstadoReserva { get; set; }
    }
}
