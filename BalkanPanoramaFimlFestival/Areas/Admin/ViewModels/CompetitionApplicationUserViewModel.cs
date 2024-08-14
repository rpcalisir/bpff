namespace BalkanPanoramaFilmFestival.Areas.Admin.ViewModels
{
    public class CompetitionApplicationUserViewModel
    {
        public int Id { get; set; } // Primary key
        public required string CompetitionCategory { get; set; }
        public required string OriginalMovieName { get; set; }
        public required string EnglishMovieName { get; set; }
        public string? MovieWebsite { get; set; }
        public required string SelectedCountries { get; set; } 
        public required string SelectedMovieGenres { get; set; }
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


        // Signed In User
        public required string Applicant { get; set; }
        public required string ApplicantMail { get; set; }
        public required string ApplicantCountry { get; set; }


        // FILM WORK OPERATION CERTIFICATE
        public required string UploadedPdfFilePath { get; set; }

        // MEDIA
        public required string UploadedMoviePicturesFilePaths { get; set; }

    }
}
