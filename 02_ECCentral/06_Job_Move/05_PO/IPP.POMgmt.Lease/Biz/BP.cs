using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using AutoClose.Model;
using AutoClose.DAL;
using System.Xml;
using ECCentral.BizEntity.Invoice;
using ECCentral.Job.Utility;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;
using System.Transactions;


namespace AutoClose.Biz
{
    public static class BP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static ILog log = LogerManger.GetLoger();
        private static JobContext CurrentContext;
        public static DateTime Begin { get; set; }
        public static DateTime End { get; set; }
        public static PurchaseOrderType POType { get; set; }      
        #endregion
        public static void DoWork(JobContext currentContext)
        {
            CurrentContext = currentContext;
            Begin = DateTime.Today.AddDays(-1);
            End = DateTime.Today;
            POType = PurchaseOrderType.Normal;
            ProcessSO();
            POType = PurchaseOrderType.Negative;
            ProcessRO();
        }

        private static void ProcessRO()
        {
            var list = DA.GetLeaseRO(Begin,End);
            List<PurchaseOrderInfo> poList = MakePOList(list);
            using (TransactionScope tran = new TransactionScope())
            {
                foreach (var item in poList)
                {
                    foreach (var poitem in item.POItems)
                    {
                        var settled = list.Where(p => p.ProductSysNo == poitem.ProductSysNo);
                        foreach (var settleitem in settled)
                        {
                            DA.SettleLeaseRO(settleitem);
                        }
                    }
                }
                tran.Complete();
            }
        }

        private static void ProcessSO()
        {
            var list = DA.GetLeaseSO(Begin, End);
            List<PurchaseOrderInfo> poList = MakePOList(list);
            CallRestful(poList);
            using (TransactionScope tran = new TransactionScope())
            {
                foreach (var item in poList)
                {
                    foreach (var poitem in item.POItems)
                    {
                        var settled = list.Where(p => p.ProductSysNo == poitem.ProductSysNo);
                        foreach (var settleitem in settled)
                        {
                            DA.SettleLeaseSO(settleitem);
                        }
                    }
                }
                tran.Complete();
            }
        }
        private static void CallRestful(List<PurchaseOrderInfo> poList)
        {
            RestClient client = new RestClient(Settings.PORestFulBaseUrl, Settings.LanguageCode);
            RestServiceError error;
            string errorMsg = string.Empty;
            foreach (var item in poList)
            {
                client.UserSysNo = item.PurchaseOrderBasicInfo.ProductManager.SysNo.Value;
                var ar = client.Update("/PurchaseOrder/CreatePurchaseOrderInfo", item, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                  
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }                                 
                }
            }          
            OnShowInfo(errorMsg);
           
        }
        private static List<PurchaseOrderInfo> MakePOList(List<LeaseProduct> prds)
        {
            List<PurchaseOrderInfo> result=new List<PurchaseOrderInfo>();
            List<LeaseProduct> OneCost = new List<LeaseProduct>();
            List<LeaseProduct> muliCost = new List<LeaseProduct>();
            var produstgp = prds.GroupBy(p => p.ProductSysNo);
            //记录多成本商品
            foreach (var item in produstgp)
            {
                var costgp = item.GroupBy(p => p.Cost);
                if (costgp.Count() >1)
                {
                    muliCost.AddRange(item);
                }
               
            }
            //排除多成本的商品
            OneCost = prds.Except(muliCost,new LeaseProductComparer()).ToList();
            //处理单成本
            ProcessPOInfo(result, OneCost, false);
            //处理多成本
            ProcessPOInfo(result, muliCost,true);
            return result;
        }

        private static void ProcessPOInfo(List<PurchaseOrderInfo> result, List<LeaseProduct> CostList,bool IsMuli)
        {
            if (IsMuli)
            {
                var muligppo = CostList.GroupBy(p => new { p.VendorSysNo, p.IsConsign, p.PMUserSysNo, p.TaxRate, p.ProductSysNo, p.Cost });
                foreach (var gp in muligppo)
                {
                    PurchaseOrderInfo po = new PurchaseOrderInfo();
                    var basic = po.PurchaseOrderBasicInfo;
                    basic.ConsignFlag = (PurchaseOrderConsignFlag)gp.Key.IsConsign;
                    basic.TaxRate = gp.Key.TaxRate;
                    basic.ProductManager = new ECCentral.BizEntity.IM.ProductManagerInfo
                    {
                        SysNo = gp.Key.PMUserSysNo
                    };
                    basic.PurchaseOrderLeaseFlag = PurchaseOrderLeaseFlag.Lease;
                    basic.StockInfo = new StockInfo { SysNo = 51 };
                    basic.PurchaseOrderType = POType;
                    po.VendorInfo.SysNo = gp.Key.VendorSysNo;
                    po.VendorInfo.VendorFinanceInfo = new VendorFinanceInfo
                    {
                        PayPeriodType = new VendorPayTermsItemInfo
                        {
                            PayTermsNo = 1
                        }
                    };
                    po.VendorInfo.VendorBasicInfo = new VendorBasicInfo
                    {
                        PaySettleCompany = PaySettleCompany.BJ
                    };
                    po.CompanyCode = Settings.CompanyCode;
                    basic.PurchaseOrderStatus = PurchaseOrderStatus.InStocked;
                    basic.CurrencyCode = 1;
                    basic.ShippingType = new ShippingType { SysNo = 13 };
                    basic.MemoInfo = new PurchaseOrderMemoInfo();
                    basic.ETATimeInfo = new PurchaseOrderETATimeInfo
                    {

                        HalfDay = PurchaseOrderETAHalfDayType.PM
                    };
                    var sameprd = gp.GroupBy(p => p.ProductSysNo);
                    foreach (var item in sameprd)
                    {

                        PurchaseOrderItemInfo poitem = new PurchaseOrderItemInfo
                        {
                            ProductSysNo = item.Key,
                            PurchaseQty = item.Sum(p=>p.Quantity),
                            OrderPrice = item.First().Cost,
                            UnitCost = item.First().Cost,
                            Weight = 0,
                            ReturnCost = 0
                        };
                        po.POItems.Add(poitem);
                    }
                    result.Add(po);
                }
            }
            else
            {
                var gppo = CostList.GroupBy(p => new { p.VendorSysNo, p.IsConsign, p.PMUserSysNo, p.TaxRate});
                foreach (var gp in gppo)
                {
                    PurchaseOrderInfo po = new PurchaseOrderInfo();
                    var basic = po.PurchaseOrderBasicInfo;
                    basic.ConsignFlag = (PurchaseOrderConsignFlag)gp.Key.IsConsign;
                    basic.TaxRate = gp.Key.TaxRate;
                    basic.ProductManager = new ECCentral.BizEntity.IM.ProductManagerInfo
                    {
                        SysNo = gp.Key.PMUserSysNo
                    };
                    basic.PurchaseOrderLeaseFlag = PurchaseOrderLeaseFlag.Lease;
                    basic.StockInfo = new StockInfo { SysNo = 51 };
                    basic.PurchaseOrderType = POType;
                    po.VendorInfo.SysNo = gp.Key.VendorSysNo;
                    po.VendorInfo.VendorFinanceInfo = new VendorFinanceInfo
                    {
                        PayPeriodType = new VendorPayTermsItemInfo
                        {
                            PayTermsNo = 1
                        }
                    };
                    po.VendorInfo.VendorBasicInfo = new VendorBasicInfo
                    {
                        PaySettleCompany = PaySettleCompany.BJ
                    };
                    po.CompanyCode = Settings.CompanyCode;
                    basic.PurchaseOrderStatus = PurchaseOrderStatus.InStocked;
                    basic.CurrencyCode = 1;
                    basic.ShippingType = new ShippingType { SysNo = 13 };
                    basic.MemoInfo = new PurchaseOrderMemoInfo();
                    basic.ETATimeInfo = new PurchaseOrderETATimeInfo
                    {
                       
                        HalfDay = PurchaseOrderETAHalfDayType.PM
                    };

                    var sameprd = gp.GroupBy(p => p.ProductSysNo);
                    foreach (var item in sameprd)
                    {
                      
                        PurchaseOrderItemInfo poitem = new PurchaseOrderItemInfo
                        {
                            ProductSysNo = item.Key,
                            PurchaseQty = item.Sum(p => p.Quantity),
                            OrderPrice = item.First().Cost,
                            UnitCost = item.First().Cost,
                            Weight = 0,
                            ReturnCost = 0
                        };
                        po.POItems.Add(poitem);
                    }
                    result.Add(po);
                }
            }
           
        }

        private static void OnShowInfo(string info)
        {
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            Console.WriteLine(info);
            log.WriteLog(info);
            if (CurrentContext != null)
            {
                CurrentContext.Message += info + Environment.NewLine;
            }
        }      
       
    }
}
