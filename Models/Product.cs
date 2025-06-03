using System.ComponentModel.DataAnnotations;

namespace e_ticaret.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün açıklaması zorunludur")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün fiyatı zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Ürün görseli zorunludur")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori zorunludur")]
        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Range(0, 5, ErrorMessage = "Puan 0-5 arasında olmalıdır")]
        public decimal Rating { get; set; }
    }
}
