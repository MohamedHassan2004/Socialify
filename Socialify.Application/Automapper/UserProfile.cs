using AutoMapper;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Automapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.JoinedOn, opt => opt.MapFrom(src => src.CreatedAt.Date));

            CreateMap<ApplicationUser, UpdateProfileInfoDto>().ReverseMap();

            CreateMap<ApplicationUser, ProfileBasicInfoDto>()
                .ForMember(dest => dest.RelationshipStatus, opt=> opt.Ignore());
        }
    }
}
