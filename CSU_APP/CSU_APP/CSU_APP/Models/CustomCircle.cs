using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CSU_APP.Models
{
    public class CustomCircle
    {
        public Position Position { get; set; }
        public double Radius { get; set; }
        public string Color { get; set; }
    }
}
