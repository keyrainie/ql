using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    //public class BadWordFilterAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var manager = ComponentFactory.GetComponent<IBadWordsManager>();
    //        string word = "";
    //        if (value != null)
    //        {
    //            var flag = manager.HasBadWords(value.ToString(), out word);
    //            if (flag)
    //            {
    //                if (!string.IsNullOrEmpty(this.ErrorMessage))
    //                {
    //                    return new ValidationResult(this.ErrorMessage, new string[] { validationContext.MemberName });
    //                }
    //                if (!string.IsNullOrEmpty(this.ErrorMessageString))
    //                {
    //                    return new ValidationResult(this.ErrorMessageString, new string[] { validationContext.MemberName });
    //                }
    //                return new ValidationResult(word, new string[] { validationContext.MemberName });                    
    //            }
    //        }

    //        return ValidationResult.Success;    
    //    }      
    //}   
}
