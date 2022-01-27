using FreeSql;
using OnceMi.Framework.Entity.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnceMi.IdentityServer4.User
{
    public class UserDbContext : DbContext
    {
        private readonly IFreeSql<UserDbContext> _freeSql;

        public UserDbContext(IFreeSql<UserDbContext> freeSql) : base(freeSql, null)
        {
            this._freeSql = freeSql ?? throw new ArgumentNullException(nameof(IFreeSql<UserDbContext>));
        }

        public DbSet<UserInfo> Users { get; set; }

        public DbSet<UserToken> UserToken { get; set; }

        public DbSet<UserRole> UserRole { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<LoginHistory> LoginHistory { get; set; }

        public DbSet<Organize> Organizes { get; set; }

        public DbSet<OrganizeManager> OrganizeManagers { get; set; }

        public DbSet<UserOrganize> UserOrganize { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseFreeSql(orm: _freeSql);
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ICodeFirst codefirst)
        {
            if (codefirst.IsAutoSyncStructure)
            {
                var items = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.FullName.StartsWith(typeof(DbSet<object>).FullName.Split('`')[0])).ToList();
                List<Type> entityTypes = new List<Type>();
                foreach(var item in items)
                {
                    if(item.PropertyType.GenericTypeArguments  == null 
                        || item.PropertyType.GenericTypeArguments.Length != 1)
                        continue;
                    entityTypes.Add(item.PropertyType.GenericTypeArguments[0]);
                }
                codefirst.SyncStructure(entityTypes.ToArray());
            }
            base.OnModelCreating(codefirst);
        }
    }
}
