using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgenciadeTours.Models
{
    public class Destino
    {
        [Key]
        public int DestinoID { get; set; }

        [Required, StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [ForeignKey(nameof(Pais))]
        public int PaisID { get; set; }
        public virtual Pais? Pais { get; set; } = null!;

        [Range(0, 365, ErrorMessage = "Días de duración inválidos.")]
        public int Dias_Duracion { get; set; }

        [Range(0, 23, ErrorMessage = "Horas de duración inválidas.")]
        public int Horas_Duracion { get; set; }
    }
}
