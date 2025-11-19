using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WmsCore.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Nazwa dostawcy jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa nie może mieć więcej niż 100 znaków.")]
        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        // Nawigacja
        public Address Address { get; set; }

        // Relacja 1:n — Dostawca ma wiele instrumentów
        [ValidateNever]
        public ICollection<Instrument> Instruments { get; set; }
    }
}
