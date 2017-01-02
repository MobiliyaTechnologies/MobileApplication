using CSU_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CSU_APP
{
    public class CustomMap : Map
    {
        public CustomMap()
        {
        }

        public CustomMap(MapSpan region) : base(region)
        {
        }

        public List<CustomCircle> CircleList { get; set; }
    }
}
