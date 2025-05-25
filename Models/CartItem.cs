using e_ticaret.Models;

namespace e_ticaret.Models
{
    public class CartItem
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}
