using LibraryAPI.ValidationTools;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Dtos
{
    [CourseValidationTools(ErrorMessage = "The description must be different from the title.")]
    public abstract class CourseManipulationDto
    {
        [Required(ErrorMessage = "The title is required.")]
        [MaxLength(100, ErrorMessage = "The title should not have more than 100 characters")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "The description should not have more than 1500 characters")]
        public virtual string Description { get; set; }
    }
}
