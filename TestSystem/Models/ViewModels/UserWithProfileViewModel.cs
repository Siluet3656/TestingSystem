// Models/ViewModels/UserWithProfileViewModel.cs
using System;

namespace TestSystem.Models.ViewModels
{
    public class UserWithProfileViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasProfile { get; set; }
    }
}