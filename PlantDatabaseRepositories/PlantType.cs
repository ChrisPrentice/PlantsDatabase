namespace PlantDatabaseRepositories
{
    public class PlantType
    {
        public int PlantTypeId { get; set; }
        public string PlantTypeLatinName { get; set; }
        public string PlantTypeCommonName { get; set; }
        public int PlantFamilyId { get; set; }
    }
}