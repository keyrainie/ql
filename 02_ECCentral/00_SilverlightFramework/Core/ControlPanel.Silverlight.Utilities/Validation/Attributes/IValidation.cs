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

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public interface IValidation
    {
        object[] Params { get; set; }
    }

    public abstract class BaseValidationAttribute : ValidationAttribute, IValidation
    {
        #region IValidator Members

        public object[] Params { get; set; }

        #endregion
    }
}
