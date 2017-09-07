using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Utils
{
    public class Common
    {
        public static string FunGetValuefromQueryString(string url, string param)
        {
            try
            {
                Dictionary<string, string> dicQueryString =
                      url.Split('?')[1].Split('&')
                           .ToDictionary(c => c.Split('=')[0],
                                         c => Uri.UnescapeDataString(c.Split('=')[1]));
                return dicQueryString[param];
            }
            catch (Exception)
            {
                Dictionary<string, string> dicQueryString =
                                      url.Split('#')[1].Split('&')
                                           .ToDictionary(c => c.Split('=')[0],
                                                         c => Uri.UnescapeDataString(c.Split('=')[1]));
                return dicQueryString[param];
            }

        }

      

    }
}
