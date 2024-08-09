using BalkanPanoramaFilmFestival.CustomValidations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.CompetitionApplication
{
    public class CompetitionApplicationUserViewModel
    {
        [Required(ErrorMessage = "At least one competition category must be selected.")]
        public required string CompetitionCategory { get; set; }

        public string CompetitionCategoryDescription
        {
            get
            {
                return CompetitionCategory switch
                {
                    "Option1" => "SARI ŞEMSİYE ULUSAL UZUN METRAJ FİLM YARIŞMASI",
                    "Option2" => "SARI ŞEMSİYE ULUSAL KISA METRAJ FİLM YARIŞMASI",
                    "Option3" => "DARIO MORENO",
                    _ => "Unknown"
                };
            }
        }
        public required string ProductionYear { get; set; }
        public required string MovieName { get; set; }
        public required string DirectorName { get; set; }

        // The list of selected countries
        //[Required(ErrorMessage = "Please select between 1 and 3 countries.")]
        //[MinLength(1, ErrorMessage = "Please select at least 1 country.")]
        //[MaxLength(3, ErrorMessage = "Please select no more than 3 countries.")]
        //[CountrySelection(3, ErrorMessage = "Max 3 countries can be selected.")]
        public List<string> SelectedCountries { get; set; } = new List<string>();

        // Add this property to hold the list of all countries
        //public List<string> AllCountries { get; set; } = new List<string>();

        // The list of all countries to display in the dropdown
        public List<string> AllCountries { get; set; } = new List<string>();
    }
}
