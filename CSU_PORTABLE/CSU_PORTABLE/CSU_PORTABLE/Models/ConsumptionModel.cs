using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public class ConsumptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Expected { get; set; }
        public string Consumed { get; set; }
        public string Overused { get; set; }
    }

    public class Premise
    {
        public int PremiseID { get; set; }
        public string PremiseName { get; set; }
        public string PremiseDesc { get; set; }
        public int OrganizationID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
    }

    public class Building
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDesc { get; set; }
        public int PremiseID { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class Meter
    {
        public int Id { get; set; }
        public string PowerScout { get; set; }
        public string Name { get; set; }
        public double MonthlyConsumption { get; set; }
        public double MonthlyPrediction { get; set; }
    }
}
