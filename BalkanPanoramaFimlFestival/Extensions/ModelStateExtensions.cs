using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BalkanPanoramaFilmFestival.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState, List<string> errors)
        {
            errors.ForEach(e => modelState.AddModelError(string.Empty, e));
        }
    }
}
