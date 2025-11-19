using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WmsCore.Models
{
    public class Instrument
    {
        public int InstrumentId { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Cena")]
        [Required(ErrorMessage = "Cena jest wymagana")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$",
            ErrorMessage = "Podaj cenę w formacie liczby całkowitej lub z maksymalnie dwoma cyframi po przecinku (np. 123,45)")]
        [DataType(DataType.Currency, ErrorMessage = "Podaj w formacie waluty")]
        public decimal Price { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; }

        // Kod kreskowy – do skanowania lub importu z producenta
        [DisplayName("Kod ean")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "EAN musi zawierać dokładnie 13 znaków")]
        public string EAN { get; set; }

        // Numer magazynowy – do zarządzania wewnętrznego
        [DisplayName("Numer magazynowy")]
        [StringLength(20)]
        public string SKU { get; set; } //Stock Keeping Unit

        // Relacja do dostawcy, może być null we wstępnej fazie projektu
        public int? SupplierId { get; set; }

        [DisplayName("Dostawca")]
        [ValidateNever] //Dzięki temu nie będzie problemu z tworzeniem klasy, WAŻNE
        public Supplier? Supplier { get; set; }

        // Relacja do kategorii 
        [Required(ErrorMessage ="Kategoria jest wymagana")]
        public int CategoryId { get; set; }

        [DisplayName("Kategoria")]
        [ValidateNever]
        public Category Category { get; set; }

        // Nawigacja do przypisanych cech
        public ICollection<InstrumentFeature>? InstrumentFeatures { get; set; }

        //Nawigacja do stanu na magazynie
        [ValidateNever]
        public InstrumentInventory Inventory { get; set; }
    }

}
