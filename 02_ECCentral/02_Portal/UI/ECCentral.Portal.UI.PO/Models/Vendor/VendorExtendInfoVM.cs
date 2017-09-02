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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorExtendInfoVM : ModelBase
    {

        public VendorExtendInfoVM()
        {
            this.invoiceType = VendorInvoiceType.NEG;
            this.shippingType = VendorShippingType.MET;
            this.stockType = VendorStockType.NEG;
            this.defaultStock = (int)VendorDefaultShippingStock.SH_BaoShan;
            this.merchantRate = 0m;
            this.isShowStore = false;
        }

        /// <summary>
        /// 公告
        /// </summary>
        private string bulletin;

        public string Bulletin
        {
            get { return bulletin; }
            set { base.SetValue("Bulletin", ref bulletin, value); }
        }

        /// <summary>
        /// 商家简介
        /// </summary>
        private string briefInfo;

        public string BriefInfo
        {
            get { return briefInfo; }
            set { base.SetValue("BriefInfo", ref briefInfo, value); }
        }
        /// <summary>
        /// 是否前台展示
        /// </summary>
        private bool isShowStore;

        public bool IsShowStore
        {
            get { return isShowStore; }
            set { base.SetValue("IsShowStore", ref isShowStore, value); }
        }

        /// <summary>
        /// 开票方式
        /// </summary>
        private VendorInvoiceType? invoiceType;

        public VendorInvoiceType? InvoiceType
        {
            get { return invoiceType; }
            set { base.SetValue("InvoiceType", ref invoiceType, value); }
        }

        /// <summary>
        /// 仓储方式
        /// </summary>
        private VendorStockType? stockType;

        public VendorStockType? StockType
        {
            get { return stockType; }
            set { base.SetValue("StockType", ref stockType, value); }
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        private VendorShippingType? shippingType;

        public VendorShippingType? ShippingType
        {
            get { return shippingType; }
            set { base.SetValue("ShippingType", ref shippingType, value); }
        }


        private decimal merchantRate;

        public decimal MerchantRate
        {
            get { return merchantRate; }
            set { base.SetValue("MerchantRate", ref merchantRate, value); }
        }

        /// <summary>
        /// 默认送货分仓号
        /// </summary>
        private int? defaultStock;

        public int? DefaultStock
        {
            get { return defaultStock; }
            set { base.SetValue("DefaultStock", ref defaultStock, value); }
        }

        /// <summary>
        /// 货币编号
        /// </summary>
        private string currencyCode;

        public string CurrencyCode
        {
            get { return currencyCode; }
            set { base.SetValue("CurrencyCode", ref currencyCode, value); }
        }

        private decimal? rentFee;

        public decimal? RentFee
        {
            get { return rentFee; }
            set { base.SetValue("RentFee", ref rentFee, value); }
        }


        private string qqNumber;
        [Validate(ValidateType.Regex, @"^[1-9]([0-9]+(\d{4,20}))$", ErrorMessage = "格式不正确,号码不能小于100000")]
        public string QQNumber
        {
            get { return qqNumber; }
            set { base.SetValue("QQNumber",ref qqNumber,value); }
        }

        private bool im_Enabled;
        public bool IM_Enabled
        {
            get { return im_Enabled; }
            set { base.SetValue("IM_Enabled", ref im_Enabled, value); }
        }


    }
}
