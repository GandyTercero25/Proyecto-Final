using System.ComponentModel.DataAnnotations;

namespace ECommerceArtesanos.Models
{
    public class Artesano
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public string Ubicacion { get; set; }
    }


}
