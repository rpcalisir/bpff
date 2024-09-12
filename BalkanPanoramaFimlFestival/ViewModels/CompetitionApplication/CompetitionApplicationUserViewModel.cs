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
                    //"Option1" => "SARI ŞEMSİYE ULUSAL UZUN METRAJ FİLM YARIŞMASI",
                    //"Option2" => "SARI ŞEMSİYE ULUSAL KISA METRAJ FİLM YARIŞMASI",
                    //"Option3" => "DARIO MORENO",
                    //"Option4" => "YARIŞMA DIŞI PROGRAM",
                    "Option1" => "INTERNATIONAL FEATURE FILM COMPETITION",
                    "Option2" => "INTERNATIONAL SHORT FILM COMPETITION",
                    "Option3" => "DARIO MORENO",
                    "Option4" => "OUT OF COMPETITION",
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
        public required string DirectorCompany { get; set; }
        public required string DirectorCountry { get; set; }
        public required string DirectorPhone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address!")]
        public required string DirectorEmail { get; set; }
        public string? DirectorBiographyTr { get; set; }
        public required string DirectorBiographyEn { get; set; }
        public string? DirectorFilmographyTr { get; set; }
        public required string DirectorFilmographyEn { get; set; }



        // Movie Tag
        public string? ScreenWriter1 { get; set; }
        public string? ScreenWriter2 { get; set; }
        public string? ScreenWriter3 { get; set; }
        public string? MovieScript { get; set; }
        public required string Cinematographer { get; set; }
        public string? MovieFiction1 { get; set; }
        public string? MovieFiction2 { get; set; }
        public string? MovieFiction { get; set; }


        // Producer
        public required string ProducerName { get; set; }
        public required string ProducerCompany { get; set; }
        public required string ProducerCountry { get; set; }
        public required string ProducerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address!")]
        public required string ProducerEmail { get; set; }

        // FILM WORK OPERATION CERTIFICATE
        //public IFormFile? UploadedPdfFile { get; set; }
        //public string? UploadedPdfFilePath { get; set; }

        // Sinopsis
        public string? SinopsisTr { get; set; }
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

        public IFormFile? UploadedBestActressPhoto { get; set; }
        public string? UploadedBestActressPhotoFilePath { get; set; }

        public IFormFile? UploadedBestActorPhoto { get; set; }
        public string? UploadedBestActorPhotoFilePath { get; set; }
        public string? UploadedBestActressPhotoName { get; set; }
        public string? UploadedBestActorPhotoName { get; set; }


        // DOWNLOADABLE SCREENING COPY OF THE FILM
        //[Required(ErrorMessage = "The Movie Link is required")]
        //[MaxLength(200, ErrorMessage = "The Movie Link cannot exceed 200 characters")]
        public required string MovieLink { get; set; }
        public required string MovieLinkPassword { get; set; }
        public required string TrailerLink { get; set; }
        public required string TrailerLinkPassword { get; set; }
        public required bool DownloadableCopyCheck { get; set; }

        // APPLICANT
        public required string ApplicantName { get; set; }

        public required string ApplicantCompany { get; set; }

        public required string ApplicantCountry { get; set; }

        public required string ApplicantPhone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address!")]
        [Required(ErrorMessage = "Applicant Email is required!")]
        public required string ApplicantEmail { get; set; }
    }
}
