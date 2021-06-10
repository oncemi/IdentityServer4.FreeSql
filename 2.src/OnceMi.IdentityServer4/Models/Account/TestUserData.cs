using IdentityServer4.FreeSql.Storage.Entities;
using OnceMi.IdentityServer4.User.Entities;
using System.Collections.Generic;

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
