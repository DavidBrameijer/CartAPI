namespace CartAPI.Models
{
    public class CartItem
    {
        public int Id {  get; set; }
        public string Product { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
