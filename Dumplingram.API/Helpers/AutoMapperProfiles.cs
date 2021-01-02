using AutoMapper;
using System.Linq;
using Dumplingram.API.Dtos;
using Dumplingram.API.Models;
using System;

namespace Dumplingram.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForRegisterDto>().ReverseMap();

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));


            CreateMap<PhotoLike, PhotoLikeDto>()
                .ForMember(dest => dest.UserId, opt =>
                    opt.MapFrom(src => src.Liker.ID))
                .ForMember(dest => dest.Username, opt =>
                    opt.MapFrom(src => src.Liker.Username))
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Liker.Name))
                .ForMember(dest => dest.Surname, opt =>
                    opt.MapFrom(src => src.Liker.Surname))
                .ForMember(dest => dest.UserPhotoUrl, opt =>
                    opt.MapFrom(src => src.Liker.Photos.FirstOrDefault(x => x.IsMain).Url));


            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForUpdateDto, User>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt =>
                    opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt =>
                    opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

            CreateMap<DateTime, DateTime>()
                .ConstructUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            CreateMap<Follow, FollowerForReturn>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Follower.ID))
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Follower.Name))
                .ForMember(dest => dest.Surname, opt =>
                    opt.MapFrom(src => src.Follower.Surname))
                .ForMember(dest => dest.Username, opt =>
                    opt.MapFrom(src => src.Follower.Username))
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.Follower.Photos.FirstOrDefault(x => x.IsMain == true).Url));

            CreateMap<Follow, FolloweeToReturn>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Followee.ID))
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Followee.Name))
                .ForMember(dest => dest.Surname, opt =>
                    opt.MapFrom(src => src.Followee.Surname))
                .ForMember(dest => dest.Username, opt =>
                    opt.MapFrom(src => src.Followee.Username))
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.Followee.Photos.FirstOrDefault(x => x.IsMain == true).Url));

            CreateMap<Photo, PhotoForDashboardDto>()
                .ForMember(dest => dest.UserId, opt =>
                    opt.MapFrom(src => src.User.ID))
                .ForMember(dest => dest.UserPhotoUrl, opt =>
                    opt.MapFrom(src => src.User.Photos.FirstOrDefault(x => x.IsMain == true).Url))
                .ForMember(dest => dest.Username, opt =>
                    opt.MapFrom(src => src.User.Username));


            CreateMap<CommentForAddDto, PhotoComment>();
            CreateMap<PhotoComment, CommentForReturnDto>()
                .ForMember(dest => dest.CommenterUsername, opt =>
                opt.MapFrom(src => src.Commenter.Username));
        }


    }
}