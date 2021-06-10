using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace OnceMi.IdentityServer4.User.Entities
{
    [Index("index_{TableName}_" + nameof(Name), nameof(Name), false)]
    public class Roles : IBaseEntity<long>
    {
        [Column(Position = 2, IsNullable = true)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(StringLength = 64, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(StringLength = 100, IsNullable = false)]
        public string RoleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(StringLength = 300)]
        public string Description { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public int Sort { get; set; } = 0;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnabled { get; set; }

        [Navigate(nameof(UserRole.RoleId))]
        public List<UserRole> UserRoles { get; set; }
    }
}
