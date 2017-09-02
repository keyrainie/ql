using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class ResSalesCouponUsedReportQueryVM : ModelBase
    {

        /// <summary>
        ///系统编号
        /// </summary>
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { sysNo = value; }
        }

        private DateTime? sodateFrom;
        public DateTime? SODateFrom
        {
            get { return sodateFrom; }
            set { SetValue<DateTime?>("SODateFrom", ref sodateFrom, value); }
        }

        private DateTime? sodateTo;
        public DateTime? SODateTo
        {
            get { return sodateTo; }
            set { SetValue<DateTime?>("SODateTo", ref sodateTo, value); }
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        private int? soSysNo;
        public int? SoSysNo
        {
            get { return soSysNo; }
            set{soSysNo=value;}
        }
        /// <summary>
        /// 优惠券编码
        /// </summary>
        private int couponCodeSysNo;
        public int CouponCodeSysNo
        {
            get { return couponCodeSysNo; }
            set { couponCodeSysNo = value; }
        }
        /// <summary>
        /// 优惠券码
        /// </summary>
        private string couponCode;
        public string CouponCode
        {
            get { return couponCode; }
            set { couponCode = value; }
        }
        /// <summary>
        /// 优惠券活动系统编码
        /// </summary>
        private int? couponSysNo;
        public int? CouponSysNo
        {
            get { return couponSysNo; }
            set { couponSysNo = value; }
        }
        /// <summary>
        /// 优惠券活动
        /// </summary>
        private string couponName;
        public string CouponName
        {
            get { return couponName; }
            set { couponName = value; }
        }
        /// <summary>
        /// 折扣价
        /// </summary>
        private decimal originalPrice;
        public decimal OriginalPrice
        {
            get { return originalPrice; }
            set { originalPrice = value; }
        }
        /// <summary>
        /// 供应商编码
        /// </summary>
        private int? merchantSysNo;
        public int? MerchantSysNo
        {
            get { return merchantSysNo; }
            set { merchantSysNo = value; }
        }
        /// <summary>
        /// 供应商
        /// </summary>
        private string merchantName;
        public string MerchantName
        {
            get { return merchantName; }
            set { merchantName = value; }
        }
        /// <summary>
        /// 支付编码
        /// </summary>
        private int? payTypeSysNo;
        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { payTypeSysNo = value; }
        }
        /// <summary>
        /// 支付方式
        /// </summary>
        private string payTypeName;
        public string PayTypeName
        {
            get { return payTypeName; }
            set { payTypeName = value; }
        }
        private SOStatus? status;
        /// <summary>
        /// 系统状态
        /// </summary>
        public SOStatus? Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? SOPayStatus
        {
            get;
            set;
        }

        #region 页面绑定源
        private List<KeyValuePair<SOStatus?, string>> soStatusList;
        /// <summary>
        /// 订单状态列表
        /// </summary>
        public List<KeyValuePair<SOStatus?, string>> SOStatusList
        {
            get
            {
                soStatusList = soStatusList ?? EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
                return soStatusList;
            }
        }

        public List<KeyValuePair<SOIncomeStatus?, string>> soIncomeStatusList;
        /// <summary>
        /// 销售收款单状态
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> SOIncomeStatusList
        {
            get
            {
                soIncomeStatusList = soIncomeStatusList ?? EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
                soIncomeStatusList.RemoveAll(item => { return item.Key == SOIncomeStatus.Abandon; });
                soIncomeStatusList.Insert(1, new KeyValuePair<SOIncomeStatus?, string>((SOIncomeStatus)(-999), ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied));
                soIncomeStatusList.Add(new KeyValuePair<SOIncomeStatus?, string>(SOIncomeStatus.Abandon, ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay));
                return soIncomeStatusList;
            }
        }

        #endregion
    }
    
    public class SalesCouponUsedReportDataVM: ModelBase
    {
        ///// <summary>
        /////系统编号
        ///// </summary>
        //private int sysNo;
        //public int SysNo
        //{
        //    get { return sysNo; }
        //    set { sysNo = value; }
        //}

        private DateTime? orderDate;
        public DateTime? OrderDate
        {
            get { return orderDate; }
            set { SetValue<DateTime?>("OrderDate", ref orderDate, value); }
        }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        private int? soSysNo;
        public int? SoSysNo
        {
            get { return soSysNo; }
            set { soSysNo = value; }
        }
        /// <summary>
        /// 优惠券编码
        /// </summary>
        private int couponCodeSysNo;
        public int CouponCodeSysNo
        {
            get { return couponCodeSysNo; }
            set { couponCodeSysNo = value; }
        }
        /// <summary>
        /// 优惠券码
        /// </summary>
        private string couponCode;
        public string CouponCode
        {
            get { return couponCode; }
            set { couponCode = value; }
        }
        /// <summary>
        /// 优惠券活动系统编码
        /// </summary>
        private int? couponSysNo;
        public int? CouponSysNo
        {
            get { return couponSysNo; }
            set { couponSysNo = value; }
        }
        /// <summary>
        /// 优惠券活动
        /// </summary>
        private string couponName;
        public string CouponName
        {
            get { return couponName; }
            set { couponName = value; }
        }
        /// <summary>
        /// 折扣价
        /// </summary>
        private decimal originalPrice;
        public decimal OriginalPrice
        {
            get { return originalPrice; }
            set { originalPrice = value; }
        }
        /// <summary>
        /// 供应商编码
        /// </summary>
        private int? merchantSysNo;
        public int? MerchantSysNo
        {
            get { return merchantSysNo; }
            set { merchantSysNo = value; }
        }
        /// <summary>
        /// 供应商
        /// </summary>
        private string merchantName;
        public string MerchantName
        {
            get { return merchantName; }
            set { merchantName = value; }
        }
        /// <summary>
        /// 支付编码
        /// </summary>
        private int? payTypeSysNo;
        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { payTypeSysNo = value; }
        }
        /// <summary>
        /// 支付方式
        /// </summary>
        private string payTypeName;
        public string PayTypeName
        {
            get { return payTypeName; }
            set { payTypeName = value; }
        }
        private SOStatus? status;
        /// <summary>
        /// 系统状态
        /// </summary>
        public SOStatus? Status
        {
            get { return status; }
            set { status = value; }
        }
        private SOIncomeStatus? soPayStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? SOPayStatus
        {
            get{return soPayStatus;}
            set { soPayStatus = value; }
        }
        private ECCentral.BizEntity.Invoice.NetPayStatus? netPayStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public ECCentral.BizEntity.Invoice.NetPayStatus? NetPayStatus
        {
            get { return netPayStatus; }
            set { netPayStatus = value; }
        }

        private SOIncomeStatus? soSOIncomeStatus;
        public SOIncomeStatus? SOIncomeStatus
        {
            get { return soSOIncomeStatus; }
            set { soSOIncomeStatus = value; }
        }

        public string SOIncomeStatusText
        {
            get;
            set;
            //get
            //{
            //    if (SOPayStatus == null || SOPayStatus == BizEntity.Invoice.SOIncomeStatus.Abandon)
            //    {
            //        if (NetPayStatus.HasValue && NetPayStatus == BizEntity.Invoice.NetPayStatus.Origin)
            //        {
            //            return ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied;
            //        }
            //        else
            //        {
            //            return ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay;
            //        }
            //    }
            //    return SOPayStatus.ToDescription();
            //}
        }

    }

    public class StaticsticInfo : ModelBase
    {
        /// <summary>
        /// 当前页统计
        /// </summary>
        private string currentPageAmount;
        public string CurrentPageAmount
        {
            get { return currentPageAmount; }
            set { currentPageAmount = value; }
        }
        /// <summary>
        /// 所有数据统计
        /// </summary>
        private string totalAmount;
        public string TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
    }
    public class SalesCouponUsedReportQueryView : ModelBase
    {
        //数据结果
        private List<SalesCouponUsedReportDataVM> result;
        public List<SalesCouponUsedReportDataVM> Result
        {
            get { return result; }
            set { SetValue<List<SalesCouponUsedReportDataVM>>("Result", ref result, value); }
        }
        //记录总数
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set { SetValue<int>("TotalCount", ref totalCount, value); }
        }
        private  List<StaticsticInfo> staticsticinfo;
        public List<StaticsticInfo> Staticsticinfo
        {
            get { return staticsticinfo; }
            set { SetValue<List<StaticsticInfo>>("Staticsticinfo", ref staticsticinfo, value); }
        }
    }

}
