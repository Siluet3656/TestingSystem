using System.Collections.Generic;

namespace TestSystem.Models.ViewModels
{
    public class UserDetailViewModel : UserWithProfileViewModel
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
}