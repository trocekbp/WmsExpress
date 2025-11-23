using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WmsCore.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace WmsCore.Models
{
    public class DocumentItem
    {
        public int DocumentItemId { get; set; }

        public int DocumentId { get; set; }

        [ValidateNever]
        public Document Document { get; set; }

        public int ItemId { get; set; }

        [ValidateNever]
        public Item Item { get; set; }

        [DisplayName("Ilość")]

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
        public int Quantity { get; set; }           // ilość przyjęta / wydana

        [Required(ErrorMessage = "Jednostka jest wymagana")]
        public string UnitOfMeasure { get; set; } = "szt.";

    }
}
