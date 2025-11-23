using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace WmsCore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [DisplayName("Nazwa kategorii")]
        public string Name { get; set; }

        public ICollection<Item> Items { get; set; }
    }

}
