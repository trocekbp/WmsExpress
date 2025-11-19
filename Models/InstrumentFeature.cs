using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace WmsCore.Models
{
    public class InstrumentFeature
    {
        public int InstrumentFeatureId { get; set; }

        public int InstrumentId { get; set; }
        [ValidateNever]
        public Instrument Instrument { get; set; }

        public int FeatureDefinitionId { get; set; }
        [ValidateNever]
        public FeatureDefinition FeatureDefinition { get; set; }

        public string Value { get; set; } // np. "Olcha"
    }

}
