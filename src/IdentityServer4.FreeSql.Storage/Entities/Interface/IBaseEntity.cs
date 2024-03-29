﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace IdentityServer4.FreeSql.Storage.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IBaseEntity<TKey> : IEntity<TKey> where TKey : struct
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        [Description("是否删除")]
        [Column(Position = -8)]
        public virtual bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 创建者Id
        /// </summary>
        [Description("创建者Id")]
        [Column(Position = -7, CanUpdate = false)]
        public virtual TKey? CreatedUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        [Column(Position = -3, CanUpdate = false, ServerTime = DateTimeKind.Local)]
        public virtual DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 修改者Id
        /// </summary>
        [Description("修改者Id")]
        [Column(Position = -2, CanInsert = false)]
        public virtual TKey? UpdatedUserId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Description("修改时间")]
        [Column(Position = -1, CanInsert = false, ServerTime = DateTimeKind.Local)]
        public virtual DateTime? UpdatedTime { get; set; }
    }

    public abstract class IBaseEntity : IBaseEntity<long>
    {

    }
}
