
namespace BalkanPanoramaFilmFestival.Models.CompetitionApplication
{
    public class CompetitionApplicationUser
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
        public required string DirectorCompany { get; set; }
        public required string DirectorCountry { get; set; }
        public required string DirectorPhone { get; set; }
        public required string DirectorEmail { get; set; }
        public string? DirectorWebsite { get; set; }
        public required string DirectorBiographyTr { get; set; }
        public required string DirectorBiographyEn { get; set; }
        public required string DirectorFilmographyTr { get; set; }
        public required string DirectorFilmographyEn { get; set; }

        // Movie Tag
        public required string MovieScript { get; set; }
        public required string Cinematographer { get; set; }
        public required string ArtDirector { get; set; }
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


        // Signed In User
        public required string Applicant { get; set; }
        public required string ApplicantMail { get; set; }
        public required string ApplicantCountry { get; set; }


        // FILM WORK OPERATION CERTIFICATE
        public string? UploadedFilePath { get; set; }
    }
}
