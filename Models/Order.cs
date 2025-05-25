namespace e_ticaret.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Hazırlanıyor";
        public DateTime EstimatedDeliveryDate { get; set; } = DateTime.Now.AddDays(3);
        public List<CartItem> Items { get; set; } = new();
        public DateTime? DeliveryDate { get; set; }
    }
}
