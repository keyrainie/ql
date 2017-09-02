using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace System.Windows
{
    public static class ValidationExtension
    {
        public static void SetValidation(this FrameworkElement frameworkElement, string message)
        {
            CustomizeValidation customValidation = new CustomizeValidation(message);

            Binding binding = new Binding("ValidationError")
            {
                Mode = System.Windows.Data.BindingMode.TwoWay,
                NotifyOnValidationError = true,
                ValidatesOnExceptions = true,
                Source = customValidation
            };
            frameworkElement.SetBinding(Control.TagProperty, binding);
        }

        public static void RaiseValidationError(this FrameworkElement frameworkElement)
        {
            BindingExpression be = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (be != null)
            {
                ((CustomizeValidation)be.DataItem).ShowErrorMessage = true;
                be.UpdateSource();
            }
        }

        public static void Validation(this FrameworkElement frameworkElement, string message)
        {
            Validation(frameworkElement, message, null, null, false);
        }

        /// <summary>
        /// 自定义设置抛出验证不通过的信息
        /// </summary>
        /// <param name="frameworkElement">控件自身</param>
        /// <param name="message">验证不通过的信息</param>
        /// <param name="viewmodel">当前控件所属的ViewModel</param>
        /// <param name="container">当前控件所属的容器：Grid，StackPanel</param>
        public static void Validation(this FrameworkElement frameworkElement, string message, ModelBase viewmodel,
            FrameworkElement container)
        {
            Validation(frameworkElement, message, viewmodel, container, false);
        }
        /// <summary>
        /// 自定义设置抛出验证不通过的信息
        /// </summary>
        /// <param name="frameworkElement">控件自身</param>
        /// <param name="message">验证不通过的信息</param>
        /// <param name="viewmodel">当前控件所属的ViewModel</param>
        /// <param name="container">当前控件所属的容器：Grid，StackPanel</param>
        /// <param name="needClearErrorWhenLostFocus">当再一次失去焦点时是否需要清除掉验证错误信息</param>
        public static void Validation(this FrameworkElement frameworkElement, string message, ModelBase viewmodel,
            FrameworkElement container, bool needClearErrorWhenLostFocus)
        {
            frameworkElement.SetValidation(message);
            frameworkElement.RaiseValidationError();
            if (needClearErrorWhenLostFocus)
            {
                TagObject tag = new TagObject();
                tag.ViewModel = viewmodel;
                tag.Container = container;
                frameworkElement.Tag = tag;
                frameworkElement.LostFocus += new RoutedEventHandler(frameworkElement_LostFocus);
            }
        }

        static void frameworkElement_LostFocus(object sender, RoutedEventArgs e)
        {
            TagObject tag = null;
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement.Tag != null)
            {
                tag = frameworkElement.Tag as TagObject;
            }
            if (tag == null)
            {
                return;
            }

            ValidationManager.Validate(tag.Container);
            if (!tag.ViewModel.HasValidationErrors)
            {
                frameworkElement.ClearValidationError();
            }
        }


        public static void ClearValidationError(this FrameworkElement frameworkElement)
        {
            BindingExpression be = frameworkElement.GetBindingExpression(Control.TagProperty);

            if (be != null)
            {
                ((CustomizeValidation)be.DataItem).ShowErrorMessage = false;
                be.UpdateSource();
            }
        }
    }

    public class TagObject
    {
        public ModelBase ViewModel { get; set; }
        public FrameworkElement Container { get; set; }
    }

    #region CustomizeValidation

    public class CustomizeValidation : ValidationAttribute
    {
        #region Private memebers
        private string message;
        #endregion

        #region Public Property
        public bool ShowErrorMessage
        {
            get;
            set;
        }

        public object ValidationError
        {
            get
            {
                return null;
            }
            set
            {
                if (ShowErrorMessage)
                {
                    throw new ValidationException(message);
                }
            }
        }
        #endregion

        #region Constructor
        public CustomizeValidation()
        {
        }

        public CustomizeValidation(string message)
        {
            this.message = message;
        }
        #endregion
    }

    #endregion

    public class Int32ValiAttribute : BaseValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //return base.IsValid(value, validationContext);

            int va;

            if (((value != null) && !string.IsNullOrWhiteSpace(Convert.ToString(value).Trim())) && !int.TryParse(Convert.ToString(value).Trim(), out va))
            {
                if (!string.IsNullOrEmpty(base.ErrorMessage))
                {
                    return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
                }
                return new ValidationResult(ValidationResource.ValidationMessage_Integer, new string[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// 判断数据是范围的整数
    /// </summary>
    public class IntRangeCustomValidation : ValidateAttribute
    {
        private int _minValue;
        private int _maxValue;
        public IntRangeCustomValidation(int minValue, int maxValue)
            : base()
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
        // Fields
        private static readonly Regex m_regex = new Regex(@"^-?\d+$");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (m_regex.IsMatch(value.ToString().Trim()))
            {
                int currentValue = Convert.ToInt32(value);
                if (currentValue >= _minValue && currentValue <= _maxValue)
                {
                    return ValidationResult.Success;
                }
            }
            if (!string.IsNullOrEmpty(base.ErrorMessage))
            {
                return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            if (base.ErrorMessageResourceType != null && !String.IsNullOrEmpty(base.ErrorMessageResourceName))
            {
                var property = base.ErrorMessageResourceType.GetProperty(base.ErrorMessageResourceName);
                if (property != null)
                {
                    object obj2 = property.GetValue(null, null);
                    if (obj2 != null)
                    {

                        var failed = new ValidationResult(obj2.ToString(), new string[] { validationContext.MemberName });
                        return failed;
                    }
                }

            }
            return new ValidationResult("必须为范围是" + _minValue + "到" + _maxValue + "整数!");
        }

    }

    /// <summary>
    /// 判断数据是正整数
    /// </summary>
    public class UintValidation : ValidateAttribute
    {
        public UintValidation()
            : base()
        {

        }
        // Fields
        private static readonly Regex m_regex = new Regex("^[1-9]*[1-9][0-9]*$");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (m_regex.IsMatch(Convert.ToString(value)))
            {
                return ValidationResult.Success;
            }
            if (!string.IsNullOrEmpty(base.ErrorMessage))
            {
                return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            if (base.ErrorMessageResourceType != null && !String.IsNullOrEmpty(base.ErrorMessageResourceName))
            {
                var property = base.ErrorMessageResourceType.GetProperty(base.ErrorMessageResourceName);
                if (property != null)
                {
                    object obj2 = property.GetValue(null, null);
                    if (obj2 != null)
                    {
                        return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                }

            }
            return new ValidationResult("必须为大于零正整数!");
        }

    }

    /// <summary>
    /// 判断数据是货币
    /// </summary>
    public class CurrencyValidation : ValidateAttribute
    {
        public CurrencyValidation()
            : base()
        {

        }
        // Fields
        private static readonly Regex m_regex = new Regex(@"(-)?\d+(\.\d\d)?");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (m_regex.IsMatch(Convert.ToString(value)))
            {
                return ValidationResult.Success;
            }
            if (!string.IsNullOrEmpty(base.ErrorMessage))
            {
                return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            if (base.ErrorMessageResourceType != null && !String.IsNullOrEmpty(base.ErrorMessageResourceName))
            {
                var property = base.ErrorMessageResourceType.GetProperty(base.ErrorMessageResourceName);
                if (property != null)
                {
                    object obj2 = property.GetValue(null, null);
                    if (obj2 != null)
                    {
                        return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                }

            }
            return new ValidationResult("必须为是货币类型!");
        }

    }

    /// <summary>
    /// 数据不能包含全角或尖括号
    /// </summary>
    public class SpecialCHarValidation : ValidateAttribute
    {
        public SpecialCHarValidation()
            : base()
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = Convert.ToString(value);
            bool rtn = str.IndexOf('<') > 0 || str.IndexOf('>') > 0;
            rtn = rtn || str.Any(t => (t > 65248) || t == 12288);
            if (!rtn)
            {
                return ValidationResult.Success;
            }
            if (!string.IsNullOrEmpty(base.ErrorMessage))
            {
                return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            if (base.ErrorMessageResourceType != null && !String.IsNullOrEmpty(base.ErrorMessageResourceName))
            {
                var property = base.ErrorMessageResourceType.GetProperty(base.ErrorMessageResourceName);
                if (property != null)
                {
                    object obj2 = property.GetValue(null, null);
                    if (obj2 != null)
                    {
                        return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
                    }
                }

            }
            return new ValidationResult("不能为全角字符或尖括号!");
        }

    }

    /// <summary>
    /// 判断数据是范围的整数
    /// </summary>
    public class DoubleRangeCustomValidation : ValidateAttribute
    {
        private double _minValue;
        private double _maxValue;
        public DoubleRangeCustomValidation(double minValue, double maxValue)
            : base()
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
        // Fields
        private static readonly Regex m_regex = new Regex(@"^/d+(/./d+)?$");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (m_regex.IsMatch(value.ToString().Trim()))
            {
                var currentValue = Convert.ToDouble(value);
                if (currentValue > _minValue && currentValue <= _maxValue)
                {
                    return ValidationResult.Success;
                }
            }
            if (!string.IsNullOrEmpty(base.ErrorMessage))
            {
                return new ValidationResult(base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            if (base.ErrorMessageResourceType != null && !String.IsNullOrEmpty(base.ErrorMessageResourceName))
            {
                var property = base.ErrorMessageResourceType.GetProperty(base.ErrorMessageResourceName);
                if (property != null)
                {
                    object obj2 = property.GetValue(null, null);
                    if (obj2 != null)
                    {

                        var failed = new ValidationResult(obj2.ToString(), new string[] { validationContext.MemberName });
                        return failed;
                    }
                }

            }
            return new ValidationResult("必须为范围是" + _minValue + "到" + _maxValue + "数!");
        }

    }


}
