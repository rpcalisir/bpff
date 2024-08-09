using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.CustomValidations
{
    public class CountrySelectionAttribute : ValidationAttribute
    {
        private readonly int _maxSelections;

        public CountrySelectionAttribute(int maxSelections)
        {
            _maxSelections = maxSelections;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var selectedCountries = value as List<string>;

            if (selectedCountries == null || !selectedCountries.Any())
            {
                return new ValidationResult("At least one country must be selected.");
            }

            if (selectedCountries.Count > _maxSelections)
            {
                return new ValidationResult($"Max {_maxSelections} countries can be selected.");
            }

            return ValidationResult.Success;
        }
    }
}
