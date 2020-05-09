using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using LibraryAPI.Dtos;
using LibraryAPI.Helpers;
using LibraryAPI.ResourcesParameters;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]

    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
                                 IPropertyCheckerService propertyCheckerService)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors([FromQuery] AuthorResourcesParameters resourcesParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(resourcesParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(resourcesParameters.Fields))
            {
                return BadRequest();
            }

            var authors = _courseLibraryRepository.GetAuthors(resourcesParameters);

            var previousPageLink = authors.HasPrevious ? CreateAuthorsResourceUri(resourcesParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = authors.HasNext ? CreateAuthorsResourceUri(resourcesParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors).ShapeData(resourcesParameters.Fields));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }

            var author = _courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(author).ShapeData(fields));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(CreateAuthorDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = _mapper.Map<Author>(author);

            _courseLibraryRepository.AddAuthor(authorEntity);

            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {

            Response.Headers.Add("Allow", "GET,OPTIONS,POST");

            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteAuthor(author);

            _courseLibraryRepository.Save();

            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorResourcesParameters authorResourcesParameters,
                                                 ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorResourcesParameters.Fields,
                            orderBy = authorResourcesParameters.OrderBy,
                            pageNumber = authorResourcesParameters.PageNumber - 1,
                            pageSize = authorResourcesParameters.PageSize,
                            mainCategory = authorResourcesParameters.MainCategory,
                            searchQuery = authorResourcesParameters.Search
                        });

                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorResourcesParameters.Fields,
                            orderBy = authorResourcesParameters.OrderBy,
                            pageNumber = authorResourcesParameters.PageNumber + 1,
                            pageSize = authorResourcesParameters.PageSize,
                            mainCategory = authorResourcesParameters.MainCategory,
                            searchQuery = authorResourcesParameters.Search
                        });
                default:
                    return Url.Link("GetAuthors",
                         new
                         {
                             fields = authorResourcesParameters.Fields,
                             orderBy = authorResourcesParameters.OrderBy,
                             pageNumber = authorResourcesParameters.PageNumber,
                             pageSize = authorResourcesParameters.PageSize,
                             mainCategory = authorResourcesParameters.MainCategory,
                             searchQuery = authorResourcesParameters.Search
                         });
            }
        }
    }
}