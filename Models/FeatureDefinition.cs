using System.ComponentModel;

namespace WmsCore.Models
{
    public enum FType //Feature Type - Typ na podstawie jego dobieramy właściwości dla odpowiednich instrumentów
    {
        Gitary,
        Perkusje,
        Pianina,
        Skrzypce,
        Dęte,
        Inne        //narazie inne są nieobsłużone
    }
    public class FeatureDefinition
    {
        public int FeatureDefinitionId { get; set; }

        public FType Type { get; set; } //Typ 
        [DisplayName("Cecha")]
        public string Name { get; set; } // np. "Ilość strun"
       
        public ICollection<InstrumentFeature> InstrumentFeatures { get; set; }
    }
}
