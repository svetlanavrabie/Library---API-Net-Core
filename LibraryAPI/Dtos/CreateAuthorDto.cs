

using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraryAPI.Dtos
{
    public class CreateAuthorDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string MainCategory { get; set; }

        public ICollection<CreateCourseDto> Courses { get; set; }
          = new List<CreateCourseDto>();

    }
}
