namespace ECommerceArtesanos.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public string UserId { get; set; } // FK al usuario (IdentityUser.Id)
        public List<CarritoItem> Items { get; set; } = new();
    }
}
