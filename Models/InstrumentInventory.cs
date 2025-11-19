using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class InstrumentInventory
    {
        public int InstrumentInventoryId { get; set; }
        [Required(ErrorMessage = "Instrument jest wymagany !")]
        public int InstrumentId { get; set; }
        public Instrument Instrument { get; set; }
        [DisplayName("Ilość")]

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    }
}
