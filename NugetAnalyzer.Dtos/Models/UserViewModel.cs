using System.Collections.Generic;

namespace NugetAnalyzer.Dtos.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public ICollection<ProfileViewModel> Profiles { get; set; }
    }
}   