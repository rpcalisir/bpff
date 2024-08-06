namespace BalkanPanoramaFilmFestival.Areas.Admin.ViewModels
{
    public class CompetitionApplicationUserViewModel
    {
        public int Id { get; set; } // Primary key
        public required string CompetitionCategory { get; set; }
        public required string ProductionYear { get; set; }
        public required string Applicant { get; set; }
        public required string ApplicantMail { get; set; }
        public required string ApplicantCountry { get; set; }
        public required string MovieName { get; set; }
        public required string DirectorName { get; set; }
    }
}
