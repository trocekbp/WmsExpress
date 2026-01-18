using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class AtrDefinition
    {
        public int AtrDefinitionId { get; set; }
        [DisplayName("Cecha")]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public ICollection<Attribute> Attributes { get; set; }
    }
}
