using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace WmsCore.Models
{
    public class DocumentItem
    {
        public int DocumentItemId { get; set; }

        public int DocumentId { get; set; }

        [ValidateNever]
        public Document Document { get; set; }

        public int ArticleId { get; set; }

        [ValidateNever]
        public Article Article { get; set; }

        [DisplayName("Ilość")]

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(0.0001, (double)decimal.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }   // ilość przyjęta / wydana

        [DisplayName("Netto")]
        [Required(ErrorMessage = "Cena netto jest wymagana")]
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal NetPrice { get; set; }

        [DisplayName("Brutto")]
        [Required(ErrorMessage = "Cena brutto jest wymagana")]
        [Column(TypeName = "decimal(18, 2)")] 

        public decimal GrossPrice { get; set; }



    }
}
