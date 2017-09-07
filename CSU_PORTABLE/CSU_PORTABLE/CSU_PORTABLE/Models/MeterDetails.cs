using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public class MeterDetails : IMapPoints
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public string Description { get; set; }
        public double MonthlyConsumption { get; set; }
    }
}
