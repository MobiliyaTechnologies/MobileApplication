using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
   public class ClassRoomModel
    {
        //public string ClassRoomId { get; set; }
        //public string ClassRoomDesc { get; set; }
        //public string SensorId { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string SensorId { get; set; }
        public string Building { get; set; }
        public string Breaker_details { get; set; }

    }
}
