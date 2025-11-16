using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestSystem.Models.ViewModels
{
    public class CreateSubmissionViewModel
    {
        [Required(ErrorMessage = "Выберите задачу")]
        [Display(Name = "Задача")]
        public int? ObjectiveId { get; set; }
        
        [BindNever]
        public List<SelectListItem> Objectives { get; set; }

        [Required(ErrorMessage = "Выберите язык")]
        [Display(Name = "Язык")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Введите код")]
        [Display(Name = "Код")]
        public string Code { get; set; }
    }
}