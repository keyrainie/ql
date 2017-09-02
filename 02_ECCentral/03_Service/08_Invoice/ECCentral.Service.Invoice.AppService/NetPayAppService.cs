using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 网上支付应用层服务
    /// </summary>
    [VersionExport(typeof(NetPayAppService))]
    public class NetPayAppService
    {
        /// <summary>
        /// 创建网上支付信息
        /// </summary>
        /// <param name="input">待创建的网上支付信息</param>
        /// <returns>创建后的网上支付信息</returns>
        public virtual NetPayInfo Create(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity, bool isForceCheck)
        {
            if (netpayEntity.SOSysNo == null)
            {
                throw new ArgumentNullException("netpayEntity.SOSysNo");
            }
            return ObjectFactory<NetPayProcessor>.Instance.Create(netpayEntity, refundEntity, isForceCheck);
        }

        /// <summary>
        /// 批量审核网上支付记录
        /// </summary>
        /// <param name="netpaySysNoList">netpay系统编号列表</param>
        public virtual string BatchAudit(List<int> netpaySysNoList)
        {
            List<BatchActionItem<NetPayAuditInfo>> items = netpaySysNoList.Select(x => new BatchActionItem<NetPayAuditInfo>()
            {
                ID = x.ToString()
                ,Data = new NetPayAuditInfo { AuditUserSysNo=ServiceContext.Current.UserSysNo, SysNo=x}
            }).ToList();

            NetPayProcessor netpayBL = ObjectFactory<NetPayProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (info) =>
            {
                //审核和创建人不能相同 仅限页面
                //NetPayInfo netpayInfo = netpayBL.LoadBySysNo(info.SysNo.Value);

                //if (netpayInfo.InputUserSysNo == info.AuditUserSysNo)
                //{
                //    throw new BizException(string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_InputAndAuditUserCannotSame"), info.SysNo));
                //}

                netpayBL.Audit(info.SysNo.Value);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 作废网上支付
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        public virtual void Abandon(int netpaySysNo)
        {
            ObjectFactory<NetPayProcessor>.Instance.Abandon(netpaySysNo);
        }

        /// <summary>
        /// 检查网上支付
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public virtual void Review(int soSysNo)
        {
            ObjectFactory<NetPayProcessor>.Instance.Review(soSysNo);
        }

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public virtual bool IsExistOriginal(int soSysNo)
        {
            return ObjectFactory<NetPayProcessor>.Instance.IsExistOriginalBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 审核netpay时加载netpay相关信息
        /// </summary>
        /// <param name="netpaySysNo"></param>
        /// <param name="refundInfo"></param>
        /// <param name="soBaseInfo"></param>
        /// <returns></returns>
        public virtual NetPayInfo LoadForAudit(int netpaySysNo
            , out SOIncomeRefundInfo refundInfo, out SOBaseInfo soBaseInfo)
        {
            var netpayInfo = ObjectFactory<NetPayProcessor>.Instance.LoadBySysNo(netpaySysNo);
            if (netpayInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_NeyPayRecordNotExist"));
            }

            soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(netpayInfo.SOSysNo.Value);
            if (soBaseInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_SORecordNotExist"));
            }

            var query = new SOIncomeRefundInfo();
            query.OrderType = RefundOrderType.OverPayment;
            query.OrderSysNo = netpayInfo.SOSysNo;
            query.Status = RefundStatus.Origin;

            refundInfo = null;
            var refundList = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(query);
            if (refundList != null && refundList.Count > 0)
            {
                refundInfo = refundList[0];
            }

            return netpayInfo;
        }
    }
}