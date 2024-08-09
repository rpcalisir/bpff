using Microsoft.AspNetCore.Mvc.Rendering;

namespace BalkanPanoramaFilmFestival.Services
{
    public class CountryService : ICountryService
    {
        public List<string> GetAllCountries()
        {
            //// This method should return a list of SelectListItem, for example:
            //return new List<SelectListItem>
            //{
            //    new SelectListItem { Value = "Turkey", Text = "Turkey" },
            //    new SelectListItem { Value = "Germany", Text = "Germany" },
            //    new SelectListItem { Value = "France", Text = "France" },
            //    // Add more countries here
            //};
            // Example static list; replace with actual data fetching logic if needed
            return new List<string>
            {
                "ALBANIA",
                "AUSTRIA",
                "BOSNIA AND HERZEGOVIA",
                "BULGARIA",
                "CROTIA",
                "GREECE",
                "HUNGARY",
                "ITALY",
                "KOSOVO",
                "MOLDOVA",
                "MONTENEGRO",
                "MACEDONIA",
                "ROMANIA",
                "SERBIA",
                "SLOVENIA",
                "TURKIYE",
                "SPAIN",
                "GERMANY",
                "BELGIUM",
                "FRANCE",
                // Add other countries here
            };
        }
    }
}
