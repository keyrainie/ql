using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public sealed class RequiredFieldAttribute : BaseValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
            {
                if (!string.IsNullOrEmpty(this.ErrorMessage))
                {
                    return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
                }

                return new ValidationResult(ValidationResource.ValidationMessage_Required, new string[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }       
    }
}
