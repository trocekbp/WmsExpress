using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WmsCore.Models
{
    public class Contractor
    {
        public int ContractorId { get; set; }

        [Required(ErrorMessage = "Nazwa dostawcy jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa nie może mieć więcej niż 100 znaków.")]
        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [RegularExpression(@"^\d{10}$|^$", ErrorMessage = "NIP musi być pusty lub musi mieć dokładnie 10 cyfr.")]

        public string? NIP { get; set; }

        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [ValidateNever]
        public Address Address { get; set; }

        [DisplayName("Dostawca")]
        public bool IsContractor { get; set; }

        [DisplayName("Odbiorca")]
        public bool IsCustomer { get; set; }

        [ValidateNever]
        public ICollection<Document> Documents { get; set; }
    }
}
