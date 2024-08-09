using Microsoft.AspNetCore.Mvc.Rendering;

namespace BalkanPanoramaFilmFestival.Services
{
    public interface ICountryService
    {
        List<string> GetAllCountries();
    }
}
