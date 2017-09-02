using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PriceChangeVM : ModelBase
    {
        public PriceChangeVM()
        {
            this.PriceTypeList = EnumConverter.GetKeyValuePairs<RequestPriceType>(EnumConverter.EnumAppendItemType.Select);
        }

        private bool? isCheck;
        public bool? IsChecked
        {
            get { return isCheck ?? false; }
            set { SetValue("IsChecked", ref isCheck, value); }
        }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        private DateTime? beginDate;
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { SetValue("BeginDate", ref beginDate, value); }
        }

        private DateTime? endDate;
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return endDate; }
            set { SetValue("EndDate", ref endDate, value); }
        }

        private DateTime? realBeginDate;
        public DateTime? RealBeginDate
        {
            get { return realBeginDate; }
            set { SetValue("RealBeginDate", ref realBeginDate, value); }
        }

        private RequestPriceStatus? status;
        public RequestPriceStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }

        private string memo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 500)]
        public string Memo
        {
            get { return memo; }
            set { SetValue("Memo", ref memo, value); }
        }

        private string audtiMemo;
        public string AudtiMemo
        {
            get { return audtiMemo; }
            set { SetValue("AudtiMemo", ref audtiMemo, value); }
        }

        private RequestPriceType? type;
        [Validate(ValidateType.Required)]
        public RequestPriceType? PriceType
        {
            get
            {
                return type;
            }
            set
            {
                SetValue("PriceType", ref type, value);
            }
        }

        private string rate;
        public string ChangeRate
        {
            get
            {
                return rate;
            }
            set
            {
                SetValue("ChangeRate", ref rate, value);
            }
        }

        #region Visibility

        public Visibility EditModelVisibility
        {
            get
            {
                return this.SysNo.HasValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool ReadyOnlyModel
        {
            get
            {
                return (Status.Value == RequestPriceStatus.Auditting
                    || Status.Value == RequestPriceStatus.Audited) ? true : false;
            }
        }



        public Visibility AddProductVisibility
        {
            get
            {
                return (this.Status.HasValue && this.Status != RequestPriceStatus.Auditting) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility AuditedVisibility
        {
            get
            {
                return (this.Status.HasValue && this.Status == RequestPriceStatus.Auditting) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility RunVisibility
        {
            get
            {
                return this.Status == RequestPriceStatus.Audited ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AbortedVisibility
        {
            get
            {
                return this.Status == RequestPriceStatus.Running ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool AuditedIsEnable
        {
            get
            {
                return (this.Status.HasValue && this.Status != RequestPriceStatus.Auditting) ? false : true;
            }
        }

        public Visibility AddVisibility
        {
            get
            {
                return (this.Status.HasValue && this.Status != RequestPriceStatus.Auditting) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility PrintVisibility
        {
            get { return this.sysNo.HasValue ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool EditModelIsEnable
        {
            get
            {
                return this.SysNo.HasValue ? false : true;
            }
        }

        // 用于查询页面
        public Visibility DetailVisibility
        {
            get
            {
                if ((Status.Value == RequestPriceStatus.Aborted || Status.Value == RequestPriceStatus.Finished))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }

            }
        }

        public Visibility EditVisibility
        {
            get
            {
                return DetailVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        // 用于UC
        public Visibility IsAuditMemoVisibility
        {
            get;
            set;
        }

        public Visibility IsChangeRateVisibility
        {
            get;
            set;
        }

        public Visibility IsBatchAuditVisibility
        {
            get;
            set;
        }

        public int TabSelectedIndex
        { get; set; }

        #endregion


        private string inUser;
        public string InUser
        {
            get { return inUser; }
            set { SetValue("InUser", ref inUser, value); }
        }

        private DateTime? inDate;
        public DateTime? InDate
        {
            get { return inDate; }
            set { SetValue("InDate", ref inDate, value); }
        }


        private string auditUser;
        public string AuditUser
        {
            get { return auditUser; }
            set { SetValue("AuditUser", ref auditUser, value); }
        }

        private DateTime? auditDate;
        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { SetValue("AuditDate", ref auditDate, value); }
        }



        public List<KeyValuePair<RequestPriceType?, string>> PriceTypeList { get; set; }

        public ObservableCollection<ChangeItemVM> ItemList { get; set; }

        #region ProductInfo

        public int ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }

        #endregion
    }

    public partial class ChangeItemVM : ModelBase
    {

        public bool SaleTypeModel
        {
            get
            {
                if (!AuditedModel)
                {
                    return false;
                }
                else if (PriceType == RequestPriceType.SalePrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool PurchaseType
        {
            get { return !SaleTypeModel; }
        }

        private RequestPriceType? priceType;
        public RequestPriceType? PriceType
        {
            get
            {
                return priceType;
            }
            set
            {
                priceType = value;
                RaisePropertyChanged("SaleTypeModel");
                RaisePropertyChanged("PurchaseType");
            }
        }

        private bool? isCheck;
        public bool? IsChecked
        {
            get { return isCheck ?? false; }
            set { SetValue("IsChecked", ref isCheck, value); }
        }

        private RequestPriceStatus? status;
        public RequestPriceStatus? RequestStatus
        {
            get { return status; }
            set { SetValue("RequestStatus", ref status, value); }
        }

        #region Price Info
        public decimal OldShowPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal OldInstockPrice { get; set; }


        private string newShowPrice;
        [Validate(ValidateType.Regex, @"^(-)?\d+(\.\d\d)?$", ErrorMessageResourceName = "Msg_JustEnterTwoSignificantPositive",ErrorMessageResourceType=typeof(ResPriceChangeMaintain))]
        public string NewShowPrice
        {
            get { return newShowPrice; }
            set { SetValue("NewShowPrice", ref newShowPrice, value); }
        }

        private string newPrice;
        [Validate(ValidateType.Regex, @"^(-)?\d+(\.\d\d)?$", ErrorMessageResourceName="Msg_JustEnterTwoSignificantPositive",ErrorMessageResourceType=typeof(ResPriceChangeMaintain))]
        public string NewPrice
        {
            get { return newPrice; }
            set { SetValue("NewPrice", ref newPrice, value); }
        }

        private string newInstockPrice;
        [Validate(ValidateType.Regex, @"^(-)?\d+(\.\d\d)?$", ErrorMessageResourceName="Msg_JustEnterTwoSignificantPositive",ErrorMessageResourceType=typeof(ResPriceChangeMaintain))]
        public string NewInstockPrice
        {
            get { return newInstockPrice; }
            set { SetValue("NewInstockPrice", ref newInstockPrice, value); }
        }

        private string grossProfit;
        public string LessThanGrossProfit
        {
            get
            {
                if (UnitCost != null && UnitCost > 0)
                {
                    if (NewPrice != null && NewPrice.Trim().Length > 0)
                    {
                        if (((Convert.ToDecimal(NewPrice) - UnitCost) / UnitCost) < MinMargin)
                        {
                            grossProfit = "是";
                        }
                        else
                        {
                            grossProfit = "否";
                        }
                    }
                    else
                    {

                        if (((CurrentPrice - UnitCost) / UnitCost) < MinMargin)
                        {
                            grossProfit = "是";
                        }
                        else
                        {
                            grossProfit = "否";
                        }
                    }
                }
                else
                {
                    grossProfit = "否";
                }

                return grossProfit;
            }
            set
            {
                SetValue("LessThanGrossProfit", ref grossProfit, value);
            }
        }

        public decimal CurrentPrice { get; set; }

        public decimal UnitCost { get; set; }

        public decimal MinMargin { get; set; }

        #endregion

        #region ProductInfo

        public int ProductSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }

        #endregion


        public bool EditModelIsEnable
        {
            get
            {
                return this.RequestStatus.HasValue ? false : true;
            }
        }

        public bool AuditedModel
        {
            get
            {
                return (this.RequestStatus.HasValue && this.RequestStatus != RequestPriceStatus.Auditting) ? false : true;
            }
        }
    }
}
