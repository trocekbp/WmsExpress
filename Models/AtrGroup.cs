namespace WmsCore.Models
{
    public class AtrGroup
    {
        public int Id { get; set; }             
        public string Name { get; set; }           
        public string? Description { get; set; }   

        public ICollection<AtrDefinition> AtrDefinitions { get; set; } = new List<AtrDefinition>();
    }

}
