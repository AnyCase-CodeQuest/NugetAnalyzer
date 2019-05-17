using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public ICollection<ProfileDTO> Profiles { get; set; }
    }
}   