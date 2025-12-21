using System.ComponentModel;

namespace WmsCore.Models
{
    public class AtrDefinition
    {
        public int AtrDefinitionId { get; set; }

        public int AttributeGroupId { get; set; }
        public AtrGroup AttributeGroup { get; set; } = null!;

        [DisplayName("Cecha")]
        public string Name { get; set; } 
       
        public ICollection<Attribute> Attributes { get; set; }
    }
}
