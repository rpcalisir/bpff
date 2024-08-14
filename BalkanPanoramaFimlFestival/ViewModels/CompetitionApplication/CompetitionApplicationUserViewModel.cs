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
        public required string OriginalMovieName { get; set; }
        public required string EnglishMovieName { get; set; }
        public string? MovieWebsite { get; set; }

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

        public List<string> AllMovieGenres { get; set; } = new List<string>();
        public required List<string> SelectedMovieGenres { get; set; } = new List<string>();

        public required string ProductionYear { get; set; }
        public required string MovieTimeLength { get; set; }
        public required string MovieLanguage { get; set; }


        // Director Section
        public required string DirectorName { get; set; }
        public required string DirectorCountry { get; set; }
        public required string DirectorPhone { get; set; }
        public required string DirectorEmail { get; set; }
        public required string DirectorBiographyTr { get; set; }
        public required string DirectorBiographyEn { get; set; }
        public required string DirectorFilmographyTr { get; set; }
        public required string DirectorFilmographyEn { get; set; }



        // Movie Tag
        public required string MovieScript { get; set; }
        public required string Cinematographer { get; set; }
        public required string MovieFiction { get; set; }
        public required string MovieActors { get; set; }
        public required string BestActress { get; set; }
        public required string BestActor { get; set; }


        // Producer
        public required string ProducerName { get; set; }
        public required string ProducerCompany { get; set; }
        public required string ProducerCountry { get; set; }
        public required string ProducerPhone { get; set; }
        public required string ProducerEmail { get; set; }
        public string? ProducerWebsite { get; set; }


        // FILM WORK OPERATION CERTIFICATE
        public IFormFile? UploadedPdfFile { get; set; }
        public string? UploadedPdfFilePath { get; set; }

        // Sinopsis
        public required string SinopsisTr { get; set; }
        public required string SinopsisEn { get; set; }
        public required string FestivalsAttended { get; set; }
        public required string AwardsReceived { get; set; }
        public required string PremierStatus { get; set; }
        public required string FirstScreening { get; set; }

        // Movie Technical Information
        public required string MovieTechInfoColor { get; set; }
        public required string ScreenSize { get; set; }
        public required string MovieTechInfoAudio { get; set; }

        // MEDIA
        public List<IFormFile>? UploadedMoviePictures { get; set; }
        public string? UploadedMoviePicturesFilePaths { get; set; }
        public IFormFile? UploadedMoviePoster { get; set; }
        public string? UploadedMoviePosterFilePath { get; set; }
        public IFormFile? UploadedMovieSubtitle { get; set; }
        public string? UploadedMovieSubtitleFilePath { get; set; }
        public IFormFile? UploadedDirectorPhoto { get; set; }
        public string? UploadedDirectorPhotoFilePath { get; set; }
    }
}
