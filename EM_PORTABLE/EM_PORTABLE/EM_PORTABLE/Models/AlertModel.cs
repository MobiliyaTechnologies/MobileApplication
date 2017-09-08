using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class AlertModel
    {
        public int Alert_Id { get; set; }
        public int Sensor_Log_Id { get; set; }
        public string Sensor_Id { get; set; }
        public string Alert_Type { get; set; }
        public string Alert_Desc { get; set; }
        public string Class_Id { get; set; }
        public string Class_Desc { get; set; }
        public string Timestamp { get; set; }
        public bool Is_Acknowledged { get; set; }
        public string Acknowledged_By { get; set; }
        public string Acknowledged_Timestamp { get; set; }
    }
}
