using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class LogoutModel
    {
        public LogoutModel(string Email)
        {
            this.Email = Email;
        }

        public string Email { get; set; }
    }
}
