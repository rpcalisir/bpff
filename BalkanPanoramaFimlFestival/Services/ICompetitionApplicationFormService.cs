using Microsoft.AspNetCore.Mvc.Rendering;

namespace BalkanPanoramaFilmFestival.Services
{
    public interface ICompetitionApplicationFormService
    {
        List<string> GetAllCountries();
        List<string> GetAllGenres();
    }
}
