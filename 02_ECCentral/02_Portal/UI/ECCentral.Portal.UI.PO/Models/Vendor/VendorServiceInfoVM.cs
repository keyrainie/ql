using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorServiceInfoVM : ModelBase
    {

        public VendorServiceInfoVM()
        {
            areaInfo = new AreaInfoVM();
        }

        /// <summary>
        /// 售后联系人
        /// </summary>
        private string contact;
        [Validate(ValidateType.Required)]
        public string Contact
        {
            get { return contact; }
            set { base.SetValue("Contact", ref contact, value); }
        }

        /// <summary>
        /// 电话
        /// </summary>
        private string contactPhone;
        [Validate(ValidateType.Required)]
        public string ContactPhone
        {
            get { return contactPhone; }
            set { base.SetValue("ContactPhone", ref contactPhone, value); }
        }

        /// <summary>
        /// 省市区
        /// </summary>
        private AreaInfoVM areaInfo;

        public AreaInfoVM AreaInfo
        {
            get { return areaInfo; }
            set { base.SetValue("AreaInfo", ref areaInfo, value); }
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string address;
        [Validate(ValidateType.Required)]
        public string Address
        {
            get { return address; }
            set { base.SetValue("Address", ref address, value); }
        }

        /// <summary>
        /// 邮编
        /// </summary>
        private string zipCode;
        [Validate(ValidateType.Regex, RegexHelper.ZIP, ErrorMessage = "请输入6位有效邮编")]
        public string ZipCode
        {
            get { return zipCode; }
            set { base.SetValue("ZipCode", ref zipCode, value); }
        }


        /// <summary>
        /// 售后服务范围
        /// </summary>
        private string rMAServiceArea;
        [Validate(ValidateType.Required)]
        public string RMAServiceArea
        {
            get { return rMAServiceArea; }
            set { base.SetValue("RMAServiceArea", ref rMAServiceArea, value); }
        }

        /// <summary>
        /// 退货策略
        /// </summary>
        private string rMAPolicy;
        [Validate(ValidateType.Required)]
        public string RMAPolicy
        {
            get { return rMAPolicy; }
            set { base.SetValue("RMAPolicy", ref rMAPolicy, value); }
        }
    }
}
