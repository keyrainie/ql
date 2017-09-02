using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    public enum ValidateType : int
    {
        /// <summary>
        /// 验证邮箱
        /// 支持参数：
        ///             无
        /// </summary>
        Email = 0,
        /// <summary>
        /// 验证是否为整数
        /// 支持参数:
        ///             无
        /// </summary>
        Interger,
        /// <summary>
        /// 验证IP
        /// 支持参数:
        ///             无
        /// </summary>
        IP,
        /// <summary>
        /// 验证输入最大长度
        /// 支持参数:
        ///           args[0]  int    定义最大可输入长度
        /// </summary>
        MaxLength,
        /// <summary>
        /// 验证输入是否是数字，并验证输入的长度范围
        /// 支持参数:
        ///           args[0]  int   定义最小输入长度
        ///           args[1]  int   定义最大输入长度
        /// </summary>       
        Phone,
        /// <summary>
        /// 自定义正则表达式验证
        /// 支持参数:
        ///          args[0] string 定义正则表达式
        /// </summary>
        Regex,
        /// <summary>
        /// 验证输入是否为空
        /// 支持参数:
        ///          无
        /// </summary>
        Required,
        /// <summary>
        /// 验证URL
        /// 支持参数:
        ///          无
        /// </summary>
        URL,
        /// <summary>
        /// 验证邮编
        /// 支持参数:
        ///          无
        /// </summary>
        ZIP
        ///// <summary>
        ///// 验证脏词
        ///// 支持参数:
        /////          无
        ///// </summary>
        //BadWord
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidateAttribute : ValidationAttribute
    {
        public BaseValidationAttribute Validator { get; set; }

        public ValidateType ValidateType { get; private set; }

        public ValidateAttribute()
            : base()
        {
        }

        public ValidateAttribute(ValidateType type)
            : this()
        {
            this.Validator = GetValidator(type);
        }

        public ValidateAttribute(ValidateType type, params object[] args)
            : this(type)
        {
            this.Validator = GetValidator(type);
            this.Validator.Params = args;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (this.Validator == null)
            {
                this.Validator = GetValidator(this.ValidateType);
            }

            var result = this.Validator.GetValidationResult(value, validationContext);

            if (!string.IsNullOrEmpty(this.ErrorMessage) && result != null)
            {
                result.ErrorMessage = this.ErrorMessage;
            }
            if (!this.ErrorMessageResourceName.IsNullOrEmpty() && this.ErrorMessageResourceType != null && result != null)
            {
                var propertyInfo = this.ErrorMessageResourceType.GetProperty(this.ErrorMessageResourceName);
                if (propertyInfo != null)
                {
                    var valueInfo = propertyInfo.GetValue(null, null);
                    if (valueInfo != null)
                    {
                        result.ErrorMessage = valueInfo.ToString();
                    }
                }
            }
            return result;
        }


        private BaseValidationAttribute GetValidator(ValidateType type)
        {
            switch (type)
            {
                case Validation.ValidateType.Email:
                    return new EmailAttribute();
                case Validation.ValidateType.Interger:
                    return new IntegerAttribute();
                case Validation.ValidateType.IP:
                    return new IPAddressAttribute();
                case Validation.ValidateType.MaxLength:
                    return new MaxLengthAttribute();
                case Validation.ValidateType.Phone:
                    return new PhoneAttribute();
                case Validation.ValidateType.Regex:
                    return new RegexAttribute();
                case Validation.ValidateType.Required:
                    return new RequiredFieldAttribute();
                case Validation.ValidateType.URL:
                    return new URLAttribute();
                case Validation.ValidateType.ZIP:
                    return new ZipCodeAttribute();
                default:
                    return null;
            }
        }
    }
}
