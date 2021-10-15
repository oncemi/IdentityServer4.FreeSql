using FreeSql.DataAnnotations;
using System.ComponentModel;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    public abstract class IEntity<TKey> where TKey : struct
    {
        /// <summary>
        /// Id
        /// </summary>
        [Description("Id")]
        [Column(IsPrimary = true, Position = 1)]
        public virtual TKey Id { get; set; }
    }

    public abstract class IEntity : IEntity<long>
    {

    }
}
