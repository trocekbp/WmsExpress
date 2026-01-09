using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class Attribute
    {
        public int AttributeId { get; set; }

        public int ArticleId { get; set; }

        [ValidateNever]
        public Article Article { get; set; }

        public int AtrDefinitionId { get; set; }
        [ValidateNever]
        public AtrDefinition AtrDefinition { get; set; }

        public string Value { get; set; } 

    }

}
