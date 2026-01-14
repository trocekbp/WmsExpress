using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WmsCore.Definitions;

namespace WmsCore.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class Article
    {
        public int ArticleId { get; set; }

        [DisplayName("Kod towaru")]
        [Required(ErrorMessage = "Kod artykułu jest wymagany")]
        [StringLength(30)]
        public string Code { get; set; }

        [DisplayName("Nazwa")]
        [Required(ErrorMessage = "Nazwa artykułu jest wymagana")]
        public string Name { get; set; }

        [DisplayName("Netto")]
        [Required(ErrorMessage = "Cena netto jest wymagana")]
        [Column(TypeName = "decimal(18, 2)")] //System samoistnie będzie cenę zaokrąglał od 5 w górę
        public decimal NetPrice { get; set; }

        [DisplayName("Brutto")]
        [Required(ErrorMessage = "Cena brutto jest wymagana")]
        [Column(TypeName = "decimal(18, 2)")] //System samoistnie będzie cenę zaokrąglał od 5 w górę
        public decimal GrossPrice { get; set; }
        [DisplayName("Stawka VAT")]
        public string VatRate { get; set; } = VatRates.Stawka23;
        [DisplayName("Opis")]
        public string? Description { get; set; }

        // Kod kreskowy – do skanowania lub importu z producenta
        [DisplayName("Kod ean")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "EAN musi zawierać dokładnie 13 znaków")]
        public string? EAN { get; set; }

        [Required(ErrorMessage = "Jednostka jest wymagana")]
        [DisplayName("Jednostka")]
        public string Unit { get; set; } = Units.Sztuka;

        public int CategoryId { get; set; }

        [DisplayName("Kategoria")]
        [ValidateNever]
        public Category Category { get; set; }

        // Nawigacja do przypisanych cech
        public ICollection<Attribute>? Attributes { get; set; }

        //Nawigacja do stanów na magazynie
        [ValidateNever]
        public ICollection<InventoryMovement>? InventoryMovements { get; set; }

    }

}
