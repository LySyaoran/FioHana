using System.Collections.Generic;

namespace Do_An.Models
{
    public class ShopSingleViewModel
    {
        public product Product { get; set; }
        public List<product> RelatedProducts { get; set; }
        public string Occasion { get; set; }
    }
}
