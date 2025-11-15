using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TestSystem.Models.ViewModels
{
    public class CreateSubmissionViewModel
    {
        [Required]
        [Display(Name = "Задача")]
        public int ObjectiveId { get; set; }

        public List<SelectListItem> Objectives { get; set; }

        [Required]
        [Display(Name = "Язык")]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
    }
}