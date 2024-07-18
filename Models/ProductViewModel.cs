namespace StockManagement.Models
{
    public class ProductViewModel
    {

        public ProductEdit ProductEdit { get; set; } 
        public IEnumerable<ProdCategory>? ProdCategories { get; set; }
    }
}
