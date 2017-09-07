using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public interface IMapPoints
    {
        int Id { get; set; }
        string Name { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        double MonthlyConsumption { get; set; }
        string Description { get; set; }
    }

    public class MapPoints
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MonthlyConsumption { get; set; }
        public string Description { get; set; }
    }
}
