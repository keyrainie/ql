using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public class IPAddressAttribute : BaseValidationAttribute
    {
        private static readonly Regex m_regex = new Regex(ValidationResource.ValidationPattern_IP);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                if (!m_regex.IsMatch(value.ToString().Trim()))
                {
                    if (!string.IsNullOrEmpty(this.ErrorMessage))
                    {
                        return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                   
                    return new ValidationResult(ValidationResource.ValidationMessage_IP, new string[] { validationContext.MemberName });                                           
                }
            }

            return ValidationResult.Success;
        }        
    }
}
