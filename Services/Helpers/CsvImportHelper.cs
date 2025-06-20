using System.Globalization;
using System.Reflection;

namespace MeterChangeApi.Services.Helpers

{
    public static class CsvImportHelper
    {
        /// <summary>
        /// Validates that the specified string properties on the record are not null or whitespace.
        /// Returns a list of property names that are missing or invalid.
        /// </summary>
        public static List<string> ValidateRequiredStringFields<T>(T record, params string[] requiredProperties)
        {
            var missingFields = new List<string>();
            var type = typeof(T);

            foreach (var propName in requiredProperties)
            {
                var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    // Property not found on type, skip or optionally log
                    continue;
                }

                if (prop.PropertyType != typeof(string))
                {
                    // Only validate string properties here
                    continue;
                }

                var value = prop.GetValue(record) as string;
                if (string.IsNullOrWhiteSpace(value))
                {
                    missingFields.Add(propName);
                }
            }

            return missingFields;
        }

        /// <summary>
        /// Parses a string to nullable int, returns null if input is null/whitespace or invalid.
        /// </summary>
        public static int? ParseIntOrNull(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : int.TryParse(value, out var result) ? (int?)result : null;

        /// <summary>
        /// Parses a string to nullable decimal, returns null if input is null/whitespace or invalid.
        /// </summary>
        public static decimal? ParseDecimalOrNull(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? (decimal?)result : null;

        /// <summary>
        /// Parses a string to nullable DateTime, returns null if input is null/whitespace or invalid.
        /// </summary>
        public static DateTime? ParseDateTimeOrNull(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : DateTime.TryParse(value, out var result) ? (DateTime?)result : null;
    }
}

