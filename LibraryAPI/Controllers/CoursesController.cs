using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using LibraryAPI.Dtos;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace LibraryAPI.Controllers
{

    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    // [ResponseCache(CacheProfileName = "240SecondsCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetCoursesForAuhtor")]
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
        //[ResponseCache(Duration =120)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate =false)]
        public ActionResult<CoursDto> GetCourseForAuhtor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CoursDto>(course));
        }

        [HttpPost(Name = "CreateCourseForAthor")]
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

            return CreatedAtRoute("GetCourseForAuhtor", new { authorId = authorId, courseId = courseToReturn.Id }, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAthor(Guid authorId, Guid courseId, UpdateCourseDto courseToUpdate)
        {

            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                var courseToCreate = _mapper.Map<Course>(courseToUpdate);

                courseToCreate.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToCreate);

                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CoursDto>(courseToCreate);

                return CreatedAtRoute("GetCourseForAuhtor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            _mapper.Map(courseToUpdate, course);

            _courseLibraryRepository.UpdateCourse(course);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public IActionResult PartiallyUpdateCourseForAthor(Guid authorId, Guid courseId,
            JsonPatchDocument<UpdateCourseDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                var courseDto = new UpdateCourseDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }


                var courseToCreate = _mapper.Map<Course>(courseDto);
                courseToCreate.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToCreate);

                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CoursDto>(courseToCreate);

                return CreatedAtRoute("GetCourseForAuhtor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            var courseToPatch = _mapper.Map<UpdateCourseDto>(course);

            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(courseToPatch, course);

            _courseLibraryRepository.UpdateCourse(course);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteCourse(course);

            _courseLibraryRepository.Save();

            return NoContent();
        }


        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}