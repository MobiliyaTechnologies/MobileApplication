using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class CampusModel
    {
        public int CampusID { get; set; }
        public string CampusName { get; set; }
        public string CampusDesc { get; set; }
        public int UniversityID { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MonthlyConsumption { get; set; }

    }
}
