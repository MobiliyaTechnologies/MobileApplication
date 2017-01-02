using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_APP.Models
{
    class MonthlyConsumptionDetails
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Powerscout { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Ligne { get; set; }
        public double Monthly_KWH_Consumption { get; set; }
        public double Monthly_Electric_Cost { get; set; }
        public System.DateTime Current_Month { get; set; }
        public System.DateTime Last_Month { get; set; }
    }
}
