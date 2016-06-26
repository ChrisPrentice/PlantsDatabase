namespace PlantDatabaseRepositories
{
    public class Plant
    {
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string PlantTypeLatinName { get; set; }
        public string PlantTypeCommonName { get; set; }
        public string PlantFamilyName { get; set; }
        public int PlantTypeId { get; set; }
        public int PlantFamilyId { get; set; }
    }
}