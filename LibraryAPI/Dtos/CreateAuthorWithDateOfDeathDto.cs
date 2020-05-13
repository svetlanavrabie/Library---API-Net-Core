using System;

namespace LibraryAPI.Dtos
{
    public class CreateAuthorWithDateOfDeathDto : CreateAuthorDto
    {
        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
