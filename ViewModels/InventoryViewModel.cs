using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WmsCore.Models;

namespace WmsCore.ViewModels
{
    public class InventoryViewModel
    {
        public Article Article{ get; set; }

        [DisplayName("Ilość")]
        public decimal TotalQuantity { get; set; }
    }
}
