using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.EntityFramework.Demo
{
    public class Config
    {
        public static List<TestUser> GetUsers()
        {
            List<TestUser> user = new List<TestUser>()
            {
                new TestUser()
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "123456",
                    IsActive = true,
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Role,"admin"),
                        new Claim(JwtClaimTypes.BirthDate,"2015-06-01"),
                    }
                }
            };
            return user;
        }
    }
}
