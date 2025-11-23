using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WmsCore.Models
{
    [Index(nameof(Acronym), IsUnique = true)]
    public class Item
    {
        public int ItemId { get; set; }

        [DisplayName("Kod towaru")]
        [Required]
        [StringLength(30)]
        public string Code { get; set; }


        [DisplayName("Akronim")]
        [StringLength(30)]
        public string Acronym { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Cena")]
        [Required(ErrorMessage = "Cena jest wymagana")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$",
            ErrorMessage = "Podaj cenę w formacie walutowym")]
        [DataType(DataType.Currency, ErrorMessage = "Podaj cenę w formacie walutowym")]
        public decimal Price { get; set; }
        [DisplayName("Opis")]
        public string Description { get; set; }

        // Kod kreskowy – do skanowania lub importu z producenta
        [DisplayName("Kod ean")]
        // [RegularExpression(@"^\d{13}$", ErrorMessage = "EAN musi zawierać dokładnie 13 znaków")]
        public string? EAN { get; set; }

        // Relacja do kategorii 
        // [Required(ErrorMessage ="Kategoria jest wymagana")]
        public int CategoryId { get; set; }

        [DisplayName("Kategoria")]
        [ValidateNever]
        public Category Category { get; set; }

        // Nawigacja do przypisanych cech
        public ICollection<Attribute>? Attributes { get; set; }

        //Nawigacja do stanu na magazynie
        [ValidateNever]
        public ItemInventory ItemInventory { get; set; }

    }

}
