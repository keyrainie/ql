using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public sealed class MaxLengthAttribute : BaseValidationAttribute
    {              
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                int length = value.ToString().Trim().Length;
                if (this.Params != null 
                    && this.Params.Length > 0 
                    && !string.IsNullOrEmpty(this.Params[0].ToString()))
                {
                    var maxLength = int.Parse(this.Params[0].ToString());
                    if (length > maxLength)
                    {
                        if (!string.IsNullOrEmpty(this.ErrorMessage))
                        {
                            return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                        }
                        return new ValidationResult(string.Format(ValidationResource.ValidationMessage_MaxLength, maxLength, length), new string[] { validationContext.MemberName });
                    }
                }
            }
            
            return ValidationResult.Success;
        }
    }
}
