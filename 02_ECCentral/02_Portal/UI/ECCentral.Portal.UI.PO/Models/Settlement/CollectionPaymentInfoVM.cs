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
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CollectionPaymentInfoVM :ModelBase
    {

        //private List<CollectionPaymentItemInfoVM> consignSettlementItemInfoList;
        public CollectionPaymentInfoVM()
        {
            TaxRateData = PurchaseOrderTaxRate.Percent017;
           // consignSettlementItemInfoList = new List<CollectionPaymentItemInfoVM>();
            vendorInfo = new VendorInfoVM();
            settleItems = new List<CollectionPaymentItemInfoVM>();
        }


        private int? m_VendorSysNo;
        public int? VendorSysNo {

            get
            {
                return m_VendorSysNo;
            }
            set
            {
                base.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }

        private string m_VendorName;
        public string VendorName {
            get
            {
                return m_VendorName;
            }
            set
            {
                base.SetValue("VendorName", ref m_VendorName, value);
            }
        }

        //public int? CurrencySysNo { get; set; }

        //public DateTime? CreateTime { get; set; }

        //public int? CreateUserSysNo { get; set; }
        //public string CreateUser { get; set; }

        //public DateTime? AuditTime { get; set; }


        //public int? AuditUser { get; set; }

        //public DateTime? SettleTime { get; set; }


        //public int? SettleUser { get; set; }


     
        //public decimal? TaxRate { get; set; }
    

        //public int? ReturnPointPM { get; set; }


        //public decimal? UsingReturnPoint { get; set; }

     
        //public int? PayPeriodType { get; set; }



       

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        private string referenceSysNo;

        public string ReferenceSysNo
        {
            get { return referenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref referenceSysNo, value); }
        }

        /// <summary>
        /// 供应商信息
        /// </summary>
        private VendorInfoVM vendorInfo;

        public VendorInfoVM VendorInfo
        {
            get { return vendorInfo; }
            set { base.SetValue("VendorInfo", ref vendorInfo, value); }
        }

        /// <summary>
        /// 源渠道仓库 -编号
        /// </summary>
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }


        /// <summary>
        /// 源渠道仓库 -编号
        /// </summary>
        private string stockID;

        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }

        /// <summary>
        /// 源渠道仓库 -名称
        /// </summary>
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        /// <summary>
        /// 税率
        /// </summary>
        private PurchaseOrderTaxRate? taxRateData;
        [Validate(ValidateType.Required)]
        public PurchaseOrderTaxRate? TaxRateData
        {
            get { return taxRateData; }
            set { base.SetValue("TaxRateData", ref taxRateData, value); }
        }

        /// <summary>
        /// 货币类型
        /// </summary>
        private string currencyCode;
        [Validate(ValidateType.Required)]
        public string CurrencyCode
        {
            get { return currencyCode; }
            set { base.SetValue("CurrencyCode", ref currencyCode, value); }
        }

        /// <summary>
        /// 结算总金额
        /// </summary>
        private decimal? totalAmt;

        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { base.SetValue("TotalAmt", ref totalAmt, value); }
        }


        private POCollectionPaymentSettleStatus? status;
        public POCollectionPaymentSettleStatus? Status
        {
            get
            {
                return status;
            }
            set
            { 
                base.SetValue("Status", ref status, value);
            }

        }

        /// <summary>
        /// 调整单编号
        /// </summary>
        private string settleBalanceSysNo;

        public string SettleBalanceSysNo
        {
            get { return settleBalanceSysNo; }
            set { base.SetValue("SettleBalanceSysNo", ref settleBalanceSysNo, value); }
        }

        /// <summary>
        /// 创建时成本
        /// </summary>
        private decimal? createCostTotalAmt;

        public decimal? CreateCostTotalAmt
        {
            get { return createCostTotalAmt; }
            set { base.SetValue("CreateCostTotalAmt", ref createCostTotalAmt, value); }
        }

        /// <summary>
        /// 差额
        /// </summary>
        private decimal? difference;

        public decimal? Difference
        {
            get { return difference; }
            set { base.SetValue("Difference", ref difference, value); }
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        private DateTime? createTime;
        public DateTime? CreateTime
        {
            get { return createTime; }
            set { base.SetValue("CreateTime", ref createTime, value); }
        }

        

        private DateTime? outStockRefundDateFrom;
        public DateTime? OutStockRefundDateFrom
        {
            get { return outStockRefundDateFrom; }
            set { base.SetValue("OutStockRefundDateFrom", ref outStockRefundDateFrom, value); }
        }

        private DateTime? outStockRefundDateTo;
        public DateTime? OutStockRefundDateTo
        {
            get { return outStockRefundDateTo; }
            set { base.SetValue("OutStockRefundDateTo", ref outStockRefundDateTo, value); }
        }

        /// <summary>
        /// 结算所属PM信息 -编号
        /// </summary>
        private string pMSysNo;
        [Validate(ValidateType.Required)]
        public string PMSysNo
        {
            get { return pMSysNo; }
            set { base.SetValue("PMSysNo", ref pMSysNo, value); }
        }

        /// <summary>
        /// 结算所属PM信息 - 名称
        /// </summary>
        private int? pMDisplayName;

        public int? PMDisplayName
        {
            get { return pMDisplayName; }
            set { base.SetValue("PMDisplayName", ref pMDisplayName, value); }
        }

        /// <summary>
        /// 毛利总金额
        /// </summary>
        private decimal? rateMarginCount;

        public decimal? RateMarginCount
        {
            get { return rateMarginCount; }
            set { base.SetValue("RateMarginCount", ref rateMarginCount, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

        /// <summary>
        /// 记录
        /// </summary>
        private string note;

        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        /// <summary>
        /// 审核人编号
        /// </summary>
        private int? auditUserSysNo;

        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { base.SetValue("AuditUserSysNo", ref auditUserSysNo, value); }
        }
        /// <summary>
        /// 审核人名称
        /// </summary>
        private int? auditUserName;

        public int? AuditUserName
        {
            get { return auditUserName; }
            set { base.SetValue("AuditUserName", ref auditUserName, value); }
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        private DateTime? auditDate;

        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { base.SetValue("AuditDate", ref auditDate, value); }
        }

  

        /// <summary>
        /// 结算单编号
        /// </summary>
        private string settleID;

        public string SettleID
        {
            get { return settleID; }
            set { base.SetValue("SettleID", ref settleID, value); }
        }

        /// <summary>
        /// 结算人
        /// </summary>
        private int? settleUserSysNo;

        public int? SettleUserSysNo
        {
            get { return settleUserSysNo; }
            set { base.SetValue("SettleUserSysNo", ref settleUserSysNo, value); }
        }
        private string settleUserName;

        public string SettleUserName
        {
            get { return settleUserName; }
            set { base.SetValue("SettleUserName", ref settleUserName, value); }
        }

        /// <summary>
        /// 结算日期
        /// </summary>
        private DateTime? settleDate;

        public DateTime? SettleDate
        {
            get { return settleDate; }
            set { base.SetValue("SettleDate", ref settleDate, value); }
        }
        /// <summary>
        /// 代销结算商品财务记录列表
        /// </summary>
        private List<CollectionPaymentItemInfoVM> settleItems;

        public List<CollectionPaymentItemInfoVM> SettleItems
        {
            get { return settleItems; }
            set { base.SetValue("SettleItems", ref settleItems, value); }
        }

        public List<CollectionPaymentItemInfoVM> MergedSettleItems
        {
            get
            {
                List<CollectionPaymentItemInfoVM> listItemsCopy = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<List<CollectionPaymentItemInfoVM>>(this.settleItems);
                List<CollectionPaymentItemInfoVM> dataRows = new List<CollectionPaymentItemInfoVM>();
                if (listItemsCopy != null)
                {
                    //var deleteIndexs = 0;
                    for (var n = 0; n < listItemsCopy.Count; n++)
                    {
                        //与返点没有关系Point

                        CollectionPaymentItemInfoVM currentItem = dataRows.SingleOrDefault
                          (p => p.ProductID == settleItems[n].ProductID
                          && p.ConsignToAccLogInfo.CreateCost == settleItems[n].ConsignToAccLogInfo.CreateCost
                          && p.SettleType == settleItems[n].SettleType
                          && p.ConsignToAccLogInfo.SalePrice== settleItems[n].ConsignToAccLogInfo.SalePrice
                         
                          && p.ConsignToAccLogInfo.StockSysNo == settleItems[n].ConsignToAccLogInfo.StockSysNo
                          && p.SettleRuleSysNo == settleItems[n].SettleRuleSysNo
                          && p.SettleSysNo!=-1
                          );
                        if (currentItem == null)
                        {
                            if (settleItems[n].SettleType == SettleType.P && settleItems[n].SettlePercentage != currentItem.SettlePercentage)
                            {
                                continue;
                            }
                            listItemsCopy[n].AllOrderSysNoFormatString = (listItemsCopy[n].ConsignToAccLogInfo.LogSysNo.HasValue ? listItemsCopy[n].ConsignToAccLogInfo.LogSysNo.Value.ToString() : string.Empty);
                            listItemsCopy[n].OrderCount = 1;

                            listItemsCopy[n].Cost = listItemsCopy[n].SettleRuleSysNo.HasValue ? (listItemsCopy[n].SettlePrice ?? 0m) : listItemsCopy[n].Cost;
                            //settleItems[n].Cost = listItemsCopy[n].Cost;
                            listItemsCopy[n].ConsignToAccLogInfo.FoldCost = listItemsCopy[n].ConsignToAccLogInfo.CreateCost - listItemsCopy[n].Cost;//折扣
                            listItemsCopy[n].ConsignToAccLogInfo.RateMargin = listItemsCopy[n].ConsignToAccLogInfo.SalePrice - listItemsCopy[n].Cost;//毛利
                            dataRows.Add(listItemsCopy[n]);
                        }
                        else
                        {

                            //如果存在，则进行累加:
                            currentItem.ConsignToAccLogInfo.ProductQuantity += listItemsCopy[n].ConsignToAccLogInfo.ProductQuantity;//商品总数
                            if ((currentItem.Cost ?? 0m) >= (listItemsCopy[n].Cost ?? 0m))
                            {
                                currentItem.Cost = listItemsCopy[n].SettleRuleSysNo.HasValue ? listItemsCopy[n].SettlePrice : listItemsCopy[n].Cost;
                                //settleItems[n].Cost = listItemsCopy[n].Cost;
                            }
                            currentItem.OrderCount += 1;//订单数量+1
                            currentItem.AllOrderSysNoFormatString += string.Format("-{0}", listItemsCopy[n].ConsignToAccLogInfo.LogSysNo);
                        }

                    }
                }
                foreach (var item in dataRows)
                {
                    //计算总金额
                    item.ConsignToAccLogInfo.CountMany = item.ConsignToAccLogInfo.ProductQuantity.Value * item.Cost.Value;
                    //计算毛利总额
                    item.ConsignToAccLogInfo.RateMarginTotal = item.ConsignToAccLogInfo.ProductQuantity.Value * item.ConsignToAccLogInfo.RateMargin.Value;
                
                }
                
                
                return dataRows;
            }
        }

    }
}
