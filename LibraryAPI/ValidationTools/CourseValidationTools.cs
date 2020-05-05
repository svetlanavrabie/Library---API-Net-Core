using LibraryAPI.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.ValidationTools
{
    public class CourseValidationTools : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CreateCourseDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
              return new ValidationResult(ErrorMessage,
                new[] { nameof(CreateCourseDto) });
            }

            return ValidationResult.Success;
        }
    }
}
