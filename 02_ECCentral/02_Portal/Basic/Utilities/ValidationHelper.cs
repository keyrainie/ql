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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls;
using System.Text.RegularExpressions;
using System.Linq;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class ValidationHelper
    {
        /// <summary>
        /// 设置验证
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="validList"></param>
        /// <returns></returns>
        public static bool Validation(FrameworkElement frameworkElement, List<ValidationEntity> validList)
        {
            bool isPass = true;
            TagObject tag = new TagObject() { Tag = frameworkElement.Tag, ValidList = validList };
            frameworkElement.Tag = tag;
            isPass = ValidateElement(frameworkElement);
            frameworkElement.LostFocus += new RoutedEventHandler(frameworkElement_LostFocus);
            return isPass;
        }

        /// <summary>
        /// 取消验证
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="validList"></param>
        /// <returns></returns>
        public static void ClearValidation(FrameworkElement frameworkElement)
        {
            TagObject tag = frameworkElement.Tag as TagObject;
            if (tag != null)
            {
                frameworkElement.Tag = tag.Tag;
            }
            frameworkElement.LostFocus -= new RoutedEventHandler(frameworkElement_LostFocus);
            frameworkElement.ClearValidationError();
        }

        private static bool ValidateElement(FrameworkElement frameworkElement)
        {
            frameworkElement.ClearValidationError();
            TagObject tag = frameworkElement.Tag as TagObject;
            List<ValidationEntity> validList = tag.ValidList;
            string elementValue = string.Empty;
            switch (frameworkElement.GetType().Name.ToUpper())
            {
                case "TEXTBOX":
                case "WATERMARKTEXTBOX":
                    elementValue = (frameworkElement as TextBox).Text;
                    break;
                case "COMBOX":
                    elementValue = ((frameworkElement as Combox).SelectedValue == null ? string.Empty : (frameworkElement as Combox).SelectedValue.ToString());
                    break;
                case "COMBOBOX":
                    elementValue = (frameworkElement as ComboBox).SelectedValue.ToString();
                    break;
                case "DATEPICKER":
                    elementValue = (frameworkElement as DatePicker).SelectedDate.ToString();
                    break;
            }


            foreach (ValidationEntity entity in validList)
            {
                switch (entity.ValidationType)
                {
                    case ValidationEnum.IsNotEmpty:
                        if (elementValue.Trim() == "")
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.IsInteger:
                        int itemp = 0;
                        if (string.IsNullOrEmpty(elementValue))
                        {
                            break;
                        }
                        if (!int.TryParse(elementValue, out itemp))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.IsDecimal:
                        decimal dtemp = 0.0M;
                        if (!decimal.TryParse(elementValue, out dtemp))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.MinLengthLimit:
                        if (elementValue.Length < Convert.ToInt32(entity.ValidationValue))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.MaxLengthLimit:
                        if (elementValue.Length > Convert.ToInt32(entity.ValidationValue))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.LessThanDateTime:
                        DateTime targetDateTime = Convert.ToDateTime(entity.ValidationValue);
                        DateTime currentDateTime = Convert.ToDateTime(elementValue);
                        if (currentDateTime > targetDateTime)
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.MoreThanDateTime:
                        DateTime targetDateTime2 = Convert.ToDateTime(entity.ValidationValue);
                        DateTime currentDateTime2 = Convert.ToDateTime(elementValue);
                        if (currentDateTime2 < targetDateTime2)
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;

                    case ValidationEnum.RegexCheck:
                        if (!Regex.IsMatch(elementValue, entity.ValidationValue.ToString()))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.ComboBoxNotOption:
                        string[] notOptions = entity.ValidationValue.ToString().Split(',');
                        if (notOptions.Contains<string>(elementValue))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.IntegerMin:
                        if (Convert.ToInt32(elementValue) < Convert.ToInt32(entity.ValidationValue))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                    case ValidationEnum.IntegerMax:
                        if (Convert.ToInt32(elementValue) > Convert.ToInt32(entity.ValidationValue))
                        {
                            frameworkElement.SetValidation(entity.ValidationMess);
                            frameworkElement.RaiseValidationError();
                            frameworkElement.Tag = tag;
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        static void frameworkElement_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            if (frameworkElement.Tag != null)
            {
                ValidateElement(frameworkElement);
            }
        }

    }

    public class TagObject
    {
        public TagObject() { }
        public object Tag { get; set; }
        public List<ValidationEntity> ValidList { get; set; }
    }

    public class ValidationEntity
    {
        public ValidationEntity(ValidationEnum validationEnum, object validationValue, string validatioMess)
        {
            ValidationType = validationEnum;
            ValidationValue = validationValue;
            ValidationMess = validatioMess;
        }
        public ValidationEnum ValidationType { get; set; }
        public object ValidationValue { get; set; }
        public string ValidationMess { get; set; }
    }

    public enum ValidationEnum
    {
        IsNotEmpty,
        IsInteger,
        IsDecimal,
        MinLengthLimit,
        MaxLengthLimit,
        LessThanDateTime,
        MoreThanDateTime,
        RegexCheck,
        ComboBoxNotOption,
        IntegerMin,
        IntegerMax
    }


}
