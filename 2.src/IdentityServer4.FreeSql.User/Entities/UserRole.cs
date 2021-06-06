using FreeSql.DataAnnotations;
using IdentityServer4.FreeSql.Storage.Entities;

namespace IdentityServer4.FreeSql.User.Entities
{
    public class UserRole : IBaseEntity
    {
        [Column(IsNullable = false)]
        public long UserId { get; set; }

        [Column(IsNullable = false)]
        public long RoleId { get; set; }

        [Navigate(nameof(Roles.Id))]
        public Roles Role { get; set; }

        [Navigate(nameof(Users.Id))]
        public Users User { get; set; }
    }
}
