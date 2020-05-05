using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using LibraryAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryAPI.Controllers
{

    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CoursDto>> GetCoursesForAuhtor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courses = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CoursDto>>(courses));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuhtor")]
        public ActionResult<CoursDto> GetCourseForAuhtor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course==null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CoursDto>(course));
        }

        [HttpPost]
        public ActionResult<CoursDto> CreateCourseForAthor(Guid authorId, CreateCourseDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Course>(course);

            _courseLibraryRepository.AddCourse(authorId, courseEntity);

            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CoursDto>(courseEntity);

            return CreatedAtRoute("GetCourseForAuhtor", new {authorId= authorId, courseId = courseToReturn.Id }, courseToReturn);
        }
    }
}