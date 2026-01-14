using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class InventoryMovement
    {
        public int InventoryMovementId { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal QuantityChange { get; set; } // + / -

        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
