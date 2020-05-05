using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using LibraryAPI.Dtos;
using LibraryAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection([FromRoute] 
        [ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids==null)
            {
                return BadRequest();
            }

            var authorEntities = _courseLibraryRepository.GetAuthors(ids);

            if (ids.Count()!= authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<CreateAuthorDto> authors)
        {
            if (authors == null)
            {
                return BadRequest();
            }

            var authorEntities= _mapper.Map<IEnumerable<Author>>(authors);

            foreach (var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }
                        
            _courseLibraryRepository.Save();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            var idsAsString = string.Join(",", authorsToReturn.Select(a=>a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorsToReturn);
        }
    }
}