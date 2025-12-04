using System.ComponentModel.DataAnnotations;

namespace AgenciadeTours.Models
{
    public class Pais
    {
        [Key]
        public int PaisID { get; set; }

        [Required(ErrorMessage = "El nombre del país es obligatorio."), StringLength(100)]
        public required string Nombre { get; set; }

        public ICollection<Destino> Destinos { get; set; } = new List<Destino>();
    }
}
