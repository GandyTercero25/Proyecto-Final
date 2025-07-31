namespace ECommerceArtesanos.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public string UserId { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public List<PedidoItem> Items { get; set; } = new();
    }
}
