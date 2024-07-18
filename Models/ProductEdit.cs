using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public class ProductEdit
    {
        public short PId { get; set; }

        public string EcId { get; set; } = null!;

        public string PName { get; set; } = null!;

        public int PPrice { get; set; }

        public string Pimage { get; set; } = null!;

        public int SaleQuantity { get; set; }

        public int PurchaseOuantity { get; set; }

        public int Stock { get; set; }

        public short? CategoryId { get; set; }

        public short? SupplierId { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ProductFile { get; set; }
    }
}
