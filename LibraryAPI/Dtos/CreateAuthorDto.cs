using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Dtos
{
    public class CreateAuthorDto
    {

        [Required(ErrorMessage = "The FirstName is required.")]
        [MaxLength(50, ErrorMessage = "The FirstName should not have more than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The LastName is required.")]
        [MaxLength(50, ErrorMessage = "The LastName should not have more than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The DateOfBirth is required.")]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required(ErrorMessage = "The MainCategory is required.")]
        [MaxLength(50, ErrorMessage = "The MainCategory should not have more than 50 characters")]
        public string MainCategory { get; set; }

        public ICollection<CreateCourseDto> Courses { get; set; }
          = new List<CreateCourseDto>();
    }
}
