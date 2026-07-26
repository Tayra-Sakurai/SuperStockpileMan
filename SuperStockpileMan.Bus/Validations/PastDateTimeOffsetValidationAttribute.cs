using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Validations
{
    public class PastDateTimeOffsetValidationAttribute : ValidationAttribute
    {
        public PastDateTimeOffsetValidationAttribute()
        {
            ErrorMessage ??= "The date and time must be a past one.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                if (dateTimeOffset < DateTimeOffset.Now)
                    return ValidationResult.Success;

                return new ValidationResult(ErrorMessageString ?? ErrorMessage);
            }

            return new ValidationResult(ErrorMessageString ?? ErrorMessage);
        }
    }
}
