using System.ComponentModel;

namespace WmsCore.Models
{
    public class AtrDefinition
    {
        public int AtrDefinitionId { get; set; }
        [DisplayName("Cecha")]
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;      
        public ICollection<Attribute> Attributes { get; set; }
    }
}
