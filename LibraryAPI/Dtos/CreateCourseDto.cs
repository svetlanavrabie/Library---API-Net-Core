using LibraryAPI.ValidationTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace LibraryAPI.Dtos
{
    [CourseValidationTools(ErrorMessage = "The description must be different from the title.")]
    public class CreateCourseDto /*: IValidatableObject*/
    {
        [Required(ErrorMessage ="The title is required.")]
        [MaxLength(100, ErrorMessage = "The title should not have more than 100 characters")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "The description should not have more than 1500 characters")]
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title== Description)
        //    {
        //        yield return new ValidationResult("The description must be different from the title!",
        //        new[] { "CreateCourseDto" });
        //    }
        //}
    }
}