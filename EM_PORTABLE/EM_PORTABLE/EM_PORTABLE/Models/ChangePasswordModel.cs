﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class ChangePasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string New_Password { get; set; }
    }
}
