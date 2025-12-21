 using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WmsCore.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [StringLength(200, ErrorMessage = "Ulica nie może mieć więcej niż 200 znaków.")]
        [DisplayName("Ulica")]
        public string? Street { get; set; }

        [StringLength(100, ErrorMessage = "Miasto nie może mieć więcej niż 100 znaków.")]
        [DisplayName("Miasto")]
        public string? City { get; set; }

        [RegularExpression(@"^(?:\d{5}|\d{2}\D\d{3}|\d{3}\D\d{2})$",
        ErrorMessage = "Nieprawidłowy format kodu pocztowego (np. 00000, 00-001 PL lub 000 01 CZ).")]
        [DisplayName("Kod pocztowy")]
        public string? PostalCode { get; set; }

        [ValidateNever]
        public int ContractorId { get; set; }
        // Nawigacja odwrotna
        [ValidateNever]
        public Contractor Contractor { get; set; }

        public bool IsEmpty() => string.IsNullOrWhiteSpace(Street)
                          || string.IsNullOrWhiteSpace(City)
                          || string.IsNullOrWhiteSpace(PostalCode);
    }
}
