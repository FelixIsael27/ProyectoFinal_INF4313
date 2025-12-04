using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciadeTours.Models
{
    public class Tour
    {
        [Key]
        public int TourID { get; set; }

        [Required, StringLength(200)]
        public required string Nombre { get; set; }

        [Required]
        [ForeignKey(nameof(Pais))]
        public int PaisID { get; set; }
        public virtual Pais Pais { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Destino))]
        public int DestinoID { get; set; }
        public virtual Destino Destino { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [NotMapped]
        public decimal ITBIS
        {
            get
            {
                return Math.Round(Precio * 0.18m, 2);
            }
            set { }
        }

        [NotMapped]
        public int Duracion_Dias
        {
            get
            {
                return Destino != null ? Destino.Dias_Duracion : 0;
            }
            set { }
        }

        [NotMapped]
        public int Duracion_Horas
        {
            get
            {
                return Destino != null ? Destino.Horas_Duracion : 0;
            }
            set { }
        }

        [NotMapped]
        public DateTime FechaHoraInicio
        {
            get
            {
                return CalcularFechaHoraInicio();
            }
            set { }
        }

        [NotMapped]
        public DateTime FechaHoraFin
        {
            get
            {
                return CalcularFechaHoraFin();
            }
            set { }
        }

        [NotMapped]
        public string Estado
        {
            get
            {
                return CalcularEstado();
            }
            set { }
        }

        private DateTime CalcularFechaHoraInicio()
        {
            var start = Fecha.Date + Hora;
            return start;
        }

        private DateTime CalcularFechaHoraFin()
        {
            var duracion = TimeSpan.FromDays(Duracion_Dias) + TimeSpan.FromHours(Duracion_Horas);
            return FechaHoraInicio.Add(duracion);
        }

        private string CalcularEstado()
        {
            var now = DateTime.Now;
            var start = Fecha.Date + Hora;
            return (start >= now) ? "Vigente" : "No vigente";
        }
    }
}