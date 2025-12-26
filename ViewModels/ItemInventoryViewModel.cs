using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WmsCore.Models;

namespace WmsCore.ViewModels
{
    public class ItemInventoryViewModel
    {
        public Item Item { get; set; }

        [DisplayName("Ilość")]
        public int TotalQuantity { get; set; }
    }
}
