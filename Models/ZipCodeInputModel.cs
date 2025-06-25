using System.ComponentModel.DataAnnotations;

namespace HellowWebAppSR.Models
{
    public class ZipCodeInputModel
    {
        [Required(ErrorMessage = "Please enter a valid US zip code.")]
        [RegularExpression(@"^\d{5}(?:-\d{4})?$", ErrorMessage = "Invalid zip code format.")]
        public string? ZipCode { get; set; } // Make nullable
    }
}