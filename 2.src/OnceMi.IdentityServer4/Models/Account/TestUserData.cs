using IdentityServer4.FreeSql.Storage.Entities;
using IdentityServer4.FreeSql.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnceMi.IdentityServer4.Models
{
    public class TestUserData
    {
        public List<Roles> Roles { get; set; }

        public List<Users> Users { get; set; }

        public List<IdentityResource> Resources { get; set; }

        public List<ApiScope> ApiScopes { get; set; }

        public List<ApiResource> ApiResources { get; set; }

        public List<Client> Clients { get; set; }
    }
}
