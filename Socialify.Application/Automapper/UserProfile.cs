using AutoMapper;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Socialify.Application.DTOs.Comment;

namespace Socialify.Application.Automapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // profile
            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.JoinedOn, opt => opt.MapFrom(src => src.CreatedAt.Date));

            CreateMap<ApplicationUser, UpdateProfileInfoDto>().ReverseMap();

            CreateMap<ApplicationUser, ProfileBasicInfoDto>()
                .ForMember(dest => dest.RelationshipStatus, opt => opt.Ignore());


            // post
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count()))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserProfilePicUrl, opt => opt.MapFrom(src => src.User.ProfilePicUrl))
                .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore())
                .ForMember(dest => dest.IsSavedByCurrentUser, opt => opt.Ignore());

            CreateMap<Post, PostWithDetailsDto>()
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<UpdatePostDto, Post>().ReverseMap();

            CreateMap<CommentDto, Comment>().ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserProfilePictureUrl, opt => opt.MapFrom(src => src.User.ProfilePicUrl));
        }

    }
}
