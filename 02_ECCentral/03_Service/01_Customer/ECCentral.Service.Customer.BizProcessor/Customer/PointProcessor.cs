using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.Service.IBizInteract;
using System.Transactions;
using ECCentral.Service.Customer.BizProcessor;
using System.Collections;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(PointProcessor))]

    public class PointProcessor
    {
        private IPointDA pointDA = ObjectFactory<IPointDA>.Instance;

        #region for so
        public virtual void Adjust(AdjustPointRequest adujstInfo)
        {
            //1.做检查
            if (!adujstInfo.CustomerSysNo.HasValue || adujstInfo.CustomerSysNo.Value == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CustomerSysNoIsNull"));
            }
            if (string.IsNullOrEmpty(adujstInfo.Source))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SourceIsNull"));
            }
            if (string.IsNullOrEmpty(adujstInfo.Memo))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "MemoIsNull"));
            }
            //2.执行调整操作
            //GetOperResult(ObjectFactory<IPointDA>.Instance.Adjust(adujstInfo));
            GetOperResult(ExternalDomainBroker.AdjustPoint(adujstInfo));
        }

        public virtual void UpateExpiringDate(int obtainSysNO, DateTime exprireDate)
        {
            //DB私有化  修改非法Write其他Domain数据表
            ExternalDomainBroker.UpdatePointExpiringDate(obtainSysNO, exprireDate);
            //ObjectFactory<IPointDA>.Instance.UpateExpiringDate(obtainSysNO, exprireDate);
        }
        /// <summary>
        /// 母订单作废，拆分成多个子订单
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="master"></param>
        /// <param name="subSoList"></param>
        public virtual void SplitSOPointLog(int customerSysNo, SOBaseInfo master, List<SOBaseInfo> subSoList)
        {
            if (master == null || master.SysNo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOSplitPoint_MasterSoIsNull"));
            }
            if (subSoList == null || subSoList.Count <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOSplitPoint_SubSoIsNull"));
            }
            int sumSubPoint = 0;
            subSoList.ForEach(item =>
            {
                sumSubPoint += Convert.ToInt32(item.PointPay.Value);
            });
            if (master.PointPay.HasValue
                && sumSubPoint != master.PointPay.Value)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SONotEqual"));
            }
            //GetOperResult(ObjectFactory<IPointDA>.Instance.SplitSOPointLog(customerSysNo, master, subSoList));
            GetOperResult(ExternalDomainBroker.SplitSOPointLog(customerSysNo, master, subSoList));
        }
        /// <summary>
        /// 子订单作废，合并为原来的母单，原来的母订单启用
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="master"></param>
        /// <param name="subSoList"></param>
        public virtual void CancelSplitSOPointLog(int customerSysNo, SOBaseInfo master, List<SOBaseInfo> subSoList)
        {
            //需验证子单的积分之和是否和主订单积分相等
            if (master == null || master.SysNo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOSplitPoint_MasterSoIsNull"));
            }

            if (subSoList == null || subSoList.Count <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOSplitPoint_SubSoIsNull"));
            }

            int sumSubPoint = 0;
            subSoList.ForEach(item =>
            {
                sumSubPoint += Convert.ToInt32(item.PointPay.Value);
            });
            if (master.PointPay.HasValue
                && sumSubPoint != master.PointPay.Value)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SONotEqual"));
            }
            //GetOperResult(ObjectFactory<IPointDA>.Instance.CancelSplitSOPointLog(customerSysNo, master, subSoList));
            GetOperResult(ExternalDomainBroker.CancelSplitSOPointLog(customerSysNo, master, subSoList));
        }
        #endregion
        /// <summary>
        /// 积分在渠道上的共享模式发生变化重写该方法
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual int GetValidPoint(int customerSysNo)
        {
            var customer = ObjectFactory<CustomerProcessor>.Instance.GetCsutomerDeatilInfo(customerSysNo);
            if (customer != null)
            {
                return customer.ValidScore.Value;
            }
            else
                return 0;
        }
        /// <summary>
        ///  积分在渠道上的共享模式发生变化重写该方法
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual int GetTotalPoint(int customerSysNo)
        {
            var customer = ObjectFactory<CustomerProcessor>.Instance.GetCsutomerDeatilInfo(customerSysNo);
            if (customer != null)
            {
                return customer.TotalScore.Value;
            }
            else
                return 0;
        }
        #region for request
        public virtual CustomerPointsAddRequest CreateCustomerAddPointRequset(CustomerPointsAddRequest requset)
        {
            //检查输入的SO单存不存在:
            if (requset.PointType == (int)AdjustPointType.RMAPostageManuToPoints)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "AuditRequest_RMASONotFound"), requset.SOSysNo.Value.ToString()));
            }
            else
            {
                if (requset.SOSysNo != null && !ExternalDomainBroker.ExistSO(requset.SOSysNo.Value))
                    throw new BizException(string.Format(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CreateRequest_SONotFound"), requset.SOSysNo.Value.ToString()));

            }
            using (TransactionScope scope = new TransactionScope())
            {
                ObjectFactory<IPointDA>.Instance.CreateRequest(requset);
                if (0 < requset.SysNo)
                {
                    requset.PointsItemList.ForEach(delegate(CustomerPointsAddRequestItem item)
                    {
                        item.PointsAddRequestSysNo = requset.SysNo.Value;
                        ObjectFactory<IPointDA>.Instance.CreateRequestItem(item);
                    });
                    scope.Complete();
                    return requset;
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CreateRequest_CreateFailed"));
                }
            }
        }
        public virtual void VerifyCustomerAddPointRequset(CustomerPointsAddRequest requset)
        {
            if (requset.SysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "AuditRequest_SONotFound"));
            }
            //审核人和申请人不能相同
            //if (requset.ConfirmUserSysNo == requset.CreateUserSysNo)
            //{
            //    throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "AuditRequest_ComfirmEqualCreate"));
            //}
            if (requset.Status == CustomerPointsAddRequestStatus.AuditDenied)
            {
                string to = ExternalDomainBroker.GetUserInfoBySysNo(requset.CreateUserSysNo).EmailAddress;
                KeyValueVariables param = new KeyValueVariables();
                param.Add("RequestSysNo", requset.SysNo);
                param.Add("Reasion", requset.ConfirmNote);
                using (TransactionScope scope = new TransactionScope())
                {
                    ObjectFactory<IPointDA>.Instance.ConfirmRequest(requset);
                    ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(to, "CustomerPointAddRequest_AuditDenied", param, true);
                    scope.Complete();
                }
                return;
            }
            else if (requset.Status == CustomerPointsAddRequestStatus.AuditPassed)
            {

                if (requset.PointsItemList != null && requset.PointsItemList.Count > 0)
                {
                    SOBaseInfo soInfo = ExternalDomainBroker.GetSOBaseInfo(requset.SOSysNo.Value);
                    if (soInfo == null)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOStatusNotFound"));
                    }

                    if (ExternalDomainBroker.GetAutoRMARefundCountBySOSysNo(requset.SOSysNo.Value) > 0)//出库后被物流拒收也视为未出库
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOStatusNotOutStock"));
                    }
                    SOIncomeInfo soincomeinfo = ExternalDomainBroker.GetConfirmedSOIncomeInfo(requset.SOSysNo.Value, SOIncomeOrderType.SO);
                    if (soincomeinfo == null)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOIncomeNotComfirmed"));
                    }
                    if (soInfo.Status != SOStatus.OutStock && SOIncomeStatus.Confirmed != soincomeinfo.Status)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOStatusNotOutStockAndSOIncomeNotComfirmed"));
                    }
                    else if (soInfo.Status != SOStatus.OutStock)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOStatusNotOutStock"));
                    }
                    else if (soincomeinfo.Status != SOIncomeStatus.Confirmed)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "SOIncomeNotComfirmed"));
                    }

                }
                using (TransactionScope scope = new TransactionScope())
                {
                    //1.为顾客加积分
                    AdjustPointRequest pointAdjust = new AdjustPointRequest();
                    pointAdjust.CustomerSysNo = requset.CustomerSysNo;
                    pointAdjust.Memo = ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "PointAddMemo_ManulAdjust");
                    pointAdjust.OperationType = AdjustPointOperationType.AddOrReduce;
                    pointAdjust.Point = requset.Point;
                    pointAdjust.PointType = requset.PointType;
                    pointAdjust.SOSysNo = requset.SOSysNo;
                    pointAdjust.Source = "CustomerMgmt";
                    Adjust(pointAdjust);
                    //1.将申请单审核通过 
                    ObjectFactory<IPointDA>.Instance.ConfirmRequest(requset);
                    scope.Complete();
                }
            }

        }
        #endregion
        /// <summary>
        /// 根据数据库的执行结果抛出相应的异常
        /// </summary>
        /// <param name="result"></param>
        private void GetOperResult(object result)
        {
            if (Convert.ToInt32(result) != 1000099)//成功
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "PointOperateResult_" + result));
            }
        }

        public int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList)
        {
            return ObjectFactory<IPointDA>.Instance.GetPriceprotectPoint(soSysNo, productSysNoList);
        }

        public SOInfo GetSoDetail(int SOSysNo, string sysAccount)
        {
            SOInfo soInfo = ExternalDomainBroker.GetSOInfo(SOSysNo);
            if (soInfo != null)
            {
                switch (sysAccount)
                {
                    case "PM-价保积分":
                        if (!((soInfo.ShippingInfo.StockType == StockType.SELF && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF && soInfo.InvoiceInfo.InvoiceType == InvoiceType.SELF ||
                                (soInfo.ShippingInfo.StockType == StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF && soInfo.InvoiceInfo.InvoiceType == InvoiceType.SELF ||
                                (soInfo.ShippingInfo.StockType == StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.MET && soInfo.InvoiceInfo.InvoiceType == InvoiceType.SELF)))))
                        {
                            throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CanNotOpreateByCurrentUser"));
                        }
                        break;
                    case "Seller-Depreciation":
                        if (!((soInfo.ShippingInfo.StockType == StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF && soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET)
                            || soInfo.ShippingInfo.StockType == StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.MET && soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET))
                        {
                            throw new BizException(ResouceManager.GetMessageString("Customer.CustomerPointsAddRequest", "CanNotOpreateByCurrentUser"));
                        }
                        break;
                }
            }
            return soInfo;
        }
    }
}