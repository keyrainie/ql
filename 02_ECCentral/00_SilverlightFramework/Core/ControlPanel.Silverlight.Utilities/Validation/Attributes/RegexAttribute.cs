using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public class RegexAttribute : BaseValidationAttribute
    {       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                string input = value.ToString().Trim();
                Regex regex = null;
                if (this.Params != null && this.Params.Length > 0 && !string.IsNullOrEmpty(this.Params[0].ToString()))
                {
                    regex = new Regex(this.Params[0].ToString());

                    if (!regex.IsMatch(input))
                    {
                        if (!string.IsNullOrEmpty(this.ErrorMessage))
                        {
                            return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                        }
                        return new ValidationResult(ValidationResource.ValidationMessage_Regex, new string[] { validationContext.MemberName });
                    }
                }                
            }
            return ValidationResult.Success;
        }
    }        
}
