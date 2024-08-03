using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.CompetitionApplication
{
    public class CompetitionApplicationUserViewModel
    {
        [Required(ErrorMessage = "Please select a competition category.")]
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
    }
}
