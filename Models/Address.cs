using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WmsCore.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Ulica jest wymagana.")]
        [StringLength(200, ErrorMessage = "Ulica nie może mieć więcej niż 200 znaków.")]
        [DisplayName("Ulica")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane.")]
        [StringLength(100, ErrorMessage = "Miasto nie może mieć więcej niż 100 znaków.")]
        [DisplayName("Miasto")]
        public string City { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
        [RegularExpression(@"^(?:\d{5}|\d{2}\D\d{3}|\d{3}\D\d{2})$",
        ErrorMessage = "Nieprawidłowy format kodu pocztowego (np. 00000, 00-001 PL lub 000 01 CZ).")]
        [DisplayName("Kod pocztowy")]
        public string PostalCode { get; set; }

        // → To jest FK do Supplier.SupplierId:
        [ValidateNever]
        public int SupplierId { get; set; }
        // Nawigacja odwrotna
        [ValidateNever]
        public Supplier Supplier { get; set; }
    }
}
