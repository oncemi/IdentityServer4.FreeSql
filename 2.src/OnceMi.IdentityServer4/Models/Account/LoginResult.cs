using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnceMi.IdentityServer4.Models
{
    public class LoginResult
    {
        public bool IsRedirect { get; set; }

        public string RedirectUrl { get; set; }
    }
}
