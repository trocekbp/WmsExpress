using WmsCore.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WmsCore.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Typ dokumentu jest wymagany")]
        [DisplayName("Typ")]
        public DocumentType Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Data dokumentu")]
        public DateTime Date { get; set; } //Dana wystawienia

        [DataType(DataType.Date)]
        [DisplayName("Data wystawienia")]
        public DateTime IssueDate { get; set; } = DateTime.Now; //Dana wystawienia

        [DisplayName("Wartość")]
        public decimal TotalValue { get; set; }

        [DisplayName("Opis")]
        public string Description { get; set; }

        public ICollection<DocumentItem> DocumentItems { get; set; }
           = new List<DocumentItem>();

        [DisplayName("Kontrahent")]
        public int? ContractorId { get; set; }

        [ValidateNever]
        public Contractor Contractor { get; set; }

    }
}
