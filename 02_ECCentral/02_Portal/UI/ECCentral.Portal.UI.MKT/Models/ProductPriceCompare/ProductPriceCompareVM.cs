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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductPriceCompareVM : ModelBase
    {
        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }

        private int? _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        private string _productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set
            {
                base.SetValue("ProductName", ref _productName, value);
            }
        }
        private decimal _sellPrice;
        /// <summary>
        /// 商品卖价
        /// </summary>
        public decimal SellPrice
        {
            get { return _sellPrice; }
            set
            {
                base.SetValue("SellPrice", ref _sellPrice, value);
            }
        }
        private decimal _userSubmittedPrice;
        /// <summary>
        /// 举报价格
        /// </summary>
        public decimal UserSubmittedPrice
        {
            get { return _userSubmittedPrice; }
            set
            {
                base.SetValue("UserSubmittedPrice", ref _userSubmittedPrice, value);
            }
        }
        private string _internetURL;
        /// <summary>
        /// 举报网址
        /// </summary>
        public string InternetURL
        {
            get { return _internetURL; }
            set
            {
                base.SetValue("InternetURL", ref _internetURL, value);
            }
        }
        private string _note;
        /// <summary>
        /// 客户说明
        /// </summary>
        public string Note
        {
            get { return _note; }
            set
            {
                base.SetValue("Note", ref _note, value);
            }
        }
    }
}
