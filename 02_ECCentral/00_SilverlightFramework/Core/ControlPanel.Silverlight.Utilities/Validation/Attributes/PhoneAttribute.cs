using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public sealed class PhoneAttribute : BaseValidationAttribute
    {
        private static readonly Regex m_regex = new Regex(ValidationResource.ValidationPattern_Phone);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                string input = value.ToString().Trim();

                if (!m_regex.IsMatch(input))
                {
                    if (!string.IsNullOrEmpty(this.ErrorMessage))
                    {
                        return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                   
                    return new ValidationResult(ValidationResource.ValidationMessage_Phone, new string[] { validationContext.MemberName });                    
                }
            }

            return ValidationResult.Success;
        }       
    }
}
