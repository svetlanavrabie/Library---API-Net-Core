using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Dtos
{
    public class UpdateCourseDto : CourseManipulationDto
    {
        [Required(ErrorMessage = "The description is required.")]
        public override string Description { get=>base.Description; set=>base.Description=value; }
    }
}
