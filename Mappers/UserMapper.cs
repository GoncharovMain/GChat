using AutoMapper;
using GChat.Models;
using GChat.ViewModels;

namespace GChat.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterModel, User>()
                .ForMember(
                    user => user.Birthday,
                    user => user.MapFrom(registerModel => new DateTimeOffset(registerModel.Birthday).ToUnixTimeSeconds()));
        }
    }
}