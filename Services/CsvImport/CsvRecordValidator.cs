using MeterChangeApi.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace MeterChangeApi.Services.CsvImport
{
    /// <summary>
    /// Provides validation functionality for CSV records.
    /// </summary>
    public static class CsvRecordValidator
    {
        /// <summary>
        /// Validates a CSV record using data annotations.
        /// </summary>
        /// <param name="record">The record to validate.</param>
        /// <returns>A collection of validation error messages, if any.</returns>
        public static IEnumerable<string> ValidateCsvRecord(CsvDataDto record)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(record, serviceProvider: null, items: null);
            Validator.TryValidateObject(record, context, validationResults, validateAllProperties: true);
            return validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error");
        }
    }
}
