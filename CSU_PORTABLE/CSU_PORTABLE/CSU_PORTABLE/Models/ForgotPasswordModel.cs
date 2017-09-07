using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public class ForgotPasswordModel
    {
        public ForgotPasswordModel(string Email)
        {
            this.Email = Email;
        }

        public string Email { get; set; }
    }
}
