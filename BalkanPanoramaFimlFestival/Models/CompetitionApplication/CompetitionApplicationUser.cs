using Microsoft.AspNetCore.Identity;

namespace BalkanPanoramaFilmFestival.Models.CompetitionApplication
{
    public class CompetitionApplicationUser
    {
        public int Id { get; set; } // Primary key
        public required string CompetitionCategory { get; set; }
        public required string ProductionYear { get; set; }
        public required string MovieName { get; set; }
        public required string DirectorName { get; set; }

    }
}
