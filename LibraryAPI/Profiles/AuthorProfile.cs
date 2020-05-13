using AutoMapper;
using CourseLibrary.API.Entities;
using LibraryAPI.Dtos;
using LibraryAPI.Helpers;

namespace LibraryAPI.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

            CreateMap<CreateAuthorDto, Author>();

            CreateMap<CreateAuthorWithDateOfDeathDto, Author>();

            CreateMap<Author, AuthorFullDto>();
        }
    }
}
