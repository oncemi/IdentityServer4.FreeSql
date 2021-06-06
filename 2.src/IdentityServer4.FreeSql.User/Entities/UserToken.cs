using FreeSql.DataAnnotations;
using IdentityServer4.FreeSql.Storage.Entities;

namespace IdentityServer4.FreeSql.User.Entities
{
    public class UserToken : IBaseEntity
    {
        public long UserId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Column(StringLength = 255, IsNullable = false)]
        public string Token { get; set; }
    }
}
