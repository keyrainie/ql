using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductStepPriceInfoVM : ModelBase
    {
        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }

        }

        private string _vendorName;
        /// <summary>
        /// 供应商
        /// </summary>
        public string VendorName
        {
            get { return _vendorName; }
            set { SetValue("VendorName", ref _vendorName, value); }

        }

        private int? _vendorSysNo;
        /// <summary>
        /// 商家编号
        /// </summary>
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set { SetValue("VendorSysNo", ref _vendorSysNo, value); }

        }

        private int? _productSysNo;
        /// <summary>
        /// 商品编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }

        }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }

        }

        private int _baseCount;
        /// <summary>
        /// 本段起始数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public int BaseCount
        {
            get { return _baseCount; }
            set { SetValue("BaseCount", ref _baseCount, value); }

        }

        private int _topCount;
        /// <summary>
        /// 本段截至数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public int TopCount
        {
            get { return _topCount; }
            set { SetValue("TopCount", ref _topCount, value); }

        }

        /// <summary>
        /// 本段最高价格
        /// </summary>
        private string _stepPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string StepPrice
        {
            get { return _stepPrice; }
            set { SetValue("StepPrice", ref _stepPrice, value); }
        }

        public string InUser { set; get; }


        public string Editdate { get; set; }

        public string Indate { get; set; }

        public string EditUser { get; set; }
    }
}
