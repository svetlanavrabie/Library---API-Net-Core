using AutoMapper;
using CourseLibrary.API.Entities;
using LibraryAPI.Dtos;

namespace LibraryAPI.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CoursDto>();

            CreateMap<CreateCourseDto, Course>();
        }
    }
}
