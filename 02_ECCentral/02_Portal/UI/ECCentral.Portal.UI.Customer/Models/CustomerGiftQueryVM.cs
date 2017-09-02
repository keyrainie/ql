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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerGiftQueryVM : ModelBase
    {
        public CustomerGiftQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }

        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }

        private string _customerID;
        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID
        {
            get
            {
                return _customerID;
            }
            set
            {
                base.SetValue("CustomerID", ref _customerID, value);
            }
        }

        private int? _productSysNo;
        /// <summary>
        /// 商品ID
        /// </summary>
        public int? ProductSysNo
        {
            get
            {
                return _productSysNo;
            }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }

        private string   _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string  ProductID
        {
            get
            {
                return _productID;
            }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        

        private CustomerGiftStatus? _status;
        /// <summary>
        /// 奖品信息状态
        /// </summary>
        public CustomerGiftStatus? Status
        {
            get
            {
                return _status;
            }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private int? _productQty;
        /// <summary>
        /// 奖品数量
        /// </summary>
        public int? ProductQty
        {
            get
            {
                return _productQty;
            }
            set
            {
                base.SetValue("ProductQty", ref _productQty, value);
            }
        }

        private DateTime? _createDateFrom;
        /// <summary>
        /// 发布时间起始范围
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get
            {
                return _createDateFrom;
            }
            set
            {
                base.SetValue("CreateDateFrom", ref _createDateFrom, value);
            }
        }

        private DateTime? _createDateTo;
        /// <summary>
        /// 发布时间终止范围
        /// </summary>
        public DateTime? CreateDateTo
        {
            get
            {
                return _createDateTo;
            }
            set
            {
                base.SetValue("CreateDateTo", ref _createDateTo, value);
            }
        }
    }
}
