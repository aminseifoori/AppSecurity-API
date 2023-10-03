using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AutoMapper;

namespace AppSecurity_API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
