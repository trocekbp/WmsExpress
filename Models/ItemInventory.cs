using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class ItemInventory
    {
        public int ItemInventoryId { get; set; }
        [Required(ErrorMessage = "Item jest wymagany !")]
        public int ItemId { get; set; }
        public Item Item { get; set; }
        [DisplayName("Ilość")]

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    }
}
