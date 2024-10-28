using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
              .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>().ReverseMap(); //if we want to go from AppUser to memberUpdateDto then we can go ahead and use reverseMap()
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<Message, MessageDto>()
                   .ForMember(dest => dest.SenderPhotoUrl, 
                   opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                   .ForMember(dest => dest.RecipientPhotoUrl,
                   opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));




        }
    }
}