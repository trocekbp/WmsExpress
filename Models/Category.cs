using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace WmsCore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [DisplayName("Nazwa kategorii")]
        public string Name { get; set; }

        [ValidateNever]
        public ICollection<Article> Articles { get; set; }

        [ValidateNever]
        public ICollection<AtrDefinition> AtrDefinitions { get; set; }
    }

}
