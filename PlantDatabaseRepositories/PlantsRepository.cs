using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace PlantDatabaseRepositories
{
    public class PlantsRepository
    {
        public IEnumerable<Plant> GetPlants()
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                connection.Open();

                return connection.Query<Plant>(@"
                        SELECT                         
                            p.PlantId, p.PlantName, p.PlantTypeId, pt.PlantTypeLatinName
                        FROM Plants p 
                        INNER JOIN PlantTypes pt ON p.PlantTypeId = pt.PlantTypeId
                        ORDER BY pt.PlantTypeLatinName, p.PlantName
                        ");
            }
        }

        public IEnumerable<PlantType> GetPlantTypes()
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                connection.Open();

                return connection.Query<PlantType>(@"
                        SELECT                         
                            pt.PlantTypeId, pt.PlantTypeLatinName, pt.PlantTypeCommonName, pt.PlantFamilyId
                        FROM PlantTypes pt
                        ORDER BY pt.PlantTypeLatinName
                        ");
            }
        }

        public IEnumerable<PlantFamily> GetPlantFamilies()
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                connection.Open();

                return connection.Query<PlantFamily>(@"
                        SELECT                         
                            pf.PlantFamilyId, pf.PlantFamilyName
                        FROM PlantFamily pf
                        ORDER BY pf.PlantFamilyName
                        ");
            }
        }

        public void AddPlant(string plantName, int plantTypeId)
        {
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                connection.Open();

                connection.Execute(@"INSERT INTO 
                                        Plants(PlantName, PlantTypeId) 
                                        VALUES(@PlantName, @PlantTypeId)
                                    ", 
                                    new
                                    {
                                        PlantName = plantName,
                                        PlantTypeId = plantTypeId
                                    });
            }
        }
    }
}