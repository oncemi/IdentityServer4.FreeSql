using System.Collections.Generic;
using AutoMapper;

namespace IdentityServer4.FreeSql.Storage.Mappers
{
    /// <summary>
    /// Defines entity/model mapping for identity resources.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="IdentityResourceMapperProfile"/>
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            CreateMap<Entities.IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.IdentityResource())
                .ReverseMap();

            CreateMap<Entities.IdentityResourceClaim, string>()
               .ConstructUsing(x => x.Type)
               .ReverseMap()
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
