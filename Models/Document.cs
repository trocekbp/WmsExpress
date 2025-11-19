using WmsCore.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WmsCore.Models
{
    public class Document
    {
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Typ dokumentu jest wymagany !")]
        [DisplayName("Typ")]
        public DocumentType Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Data wystawienia")]
        public DateTime Date { get; set; } = DateTime.Now; //Dana wystawienia

        public ICollection<DocumentInstrument> DocumentInstruments { get; set; }
           = new List<DocumentInstrument>();
    }
}
