using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WmsCore.Definitions;

namespace WmsCore.Models
{

    [Index(nameof(Number), IsUnique = true)]
    public class Document
    {
        public int DocumentId { get; set; }

        [DisplayName("Numer")]
        [ValidateNever]
        public string? Number { get; set; } //generowany za pomocą funkcji

        [DisplayName("Numer faktury kosztowej")]
        [ValidateNever]
        public string? CostInvoiceNumber { get; set; } 

        [Required(ErrorMessage = "Typ dokumentu jest wymagany")]
        [DisplayName("Typ")]
        public DocumentTypes Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Data wystawienia")]
        public DateTime Date { get; set; } = DateTime.Now; //Data wystawienia

        [DataType(DataType.Date)]
        [DisplayName("Data obowiązywania")]
        public DateTime OperationDate { get; set; } = DateTime.Now; //Data obowiązywania

        public DateTime CreationDate { get; set; } //Data utworzenia informacyjnie, wstawiana przez trigger

        [DisplayName("Wartość")]
        public decimal TotalValue { get; set; }

        [DisplayName("Opis")]
        public string? Description { get; set; }

        public ICollection<DocumentItem> DocumentItems { get; set; }
           = new List<DocumentItem>();

        [DisplayName("Kontrahent")]
        public int ContractorId { get; set; }

        [ValidateNever]
        public Contractor Contractor { get; set; } 

    }
}
