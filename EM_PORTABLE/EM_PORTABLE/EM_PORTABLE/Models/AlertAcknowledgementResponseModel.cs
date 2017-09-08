using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class AlertAcknowledgementResponseModel
    {
        public int Status_Code { get; set; }
        public string Message { get; set; }
        public int Alert_Id { get; set; }

    }
}
