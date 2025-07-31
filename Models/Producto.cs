using System.ComponentModel.DataAnnotations;

namespace ECommerceArtesanos.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        public string ImagenUrl { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un artesano")]
        public int ArtesanoId { get; set; }

        // ❌ NUNCA pongas [Required] aquí
        public Artesano Artesano { get; set; }
    }


}
