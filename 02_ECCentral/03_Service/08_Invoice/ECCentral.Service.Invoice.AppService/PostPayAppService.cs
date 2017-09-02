using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 电汇邮局付款记录应用层服务
    /// </summary>
    [VersionExport(typeof(PostPayAppService))]
    public class PostPayAppService
    {
        #region [Properties]

        private PostIncomeProcessor m_PostIncomeBL;
        private PostIncomeProcessor PostIncomeBL
        {
            get
            {
                if (m_PostIncomeBL == null)
                {
                    m_PostIncomeBL = ObjectFactory<PostIncomeProcessor>.Instance;
                }
                return m_PostIncomeBL;
            }
        }

        private SOIncomeProcessor m_SOIncomeBL;
        private SOIncomeProcessor SOIncomeBL
        {
            get
            {
                if (m_SOIncomeBL == null)
                {
                    m_SOIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
                }
                return m_SOIncomeBL;
            }
        }

        private PostPayProcessor m_PostPayBL;
        private PostPayProcessor PostPayBL
        {
            get
            {
                if (m_PostPayBL == null)
                {
                    m_PostPayBL = ObjectFactory<PostPayProcessor>.Instance;
                }

                return m_PostPayBL;
            }
        }

        #endregion [Properties]

        /// <summary>
        /// 创建电汇邮局付款记录信息
        /// </summary>
        /// <param name="postpayInfo"></param>
        /// <param name="refundInfo"></param>
        /// <param name="isForceCheck"></param>
        /// <returns></returns>
        public PostPayInfo Create(PostPayInfo postpayInfo, SOIncomeRefundInfo refundInfo, bool isForceCheck)
        {
            return PostPayBL.Create(postpayInfo, refundInfo, isForceCheck);
        }

        /// <summary>
        /// 取得银行电汇-邮局付款支付方式列表
        /// </summary>
        /// <returns></returns>
        public virtual List<PayType> GetBankOrPostPayTypeList()
        {
            return PostPayBL.GetBankOrPostPayTypeList();
        }

        /// <summary>
        /// 通过订单编号加载和该订单号关联的电汇-邮局付款单关联信息，用于核对银行电汇-邮局汇款
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="soBaseInfo">订单基本信息</param>
        /// <param name="incomeAmt">实收金额</param>
        /// <param name="checkedOrderAmt">应收金额</param>
        /// <param name="refundAmt">已确认的多付金额</param>
        /// <returns>和该订单号关联的电汇-邮局付款单关联信息</returns>
        public virtual List<PostIncomeConfirmInfo> LoadForCreateBySOSysNo(int soSysNo, out SOBaseInfo soBaseInfo,
            out decimal remainAmt)
        {
            soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(soSysNo);
            if (soBaseInfo == null)
            {
                throw new BizException(ResouceManager.GetMessageString("Invoice.PostPay","PostPay_DeActiveOrderID"));
            }

            if (!PostPayBL.IsBankOrPostPayType(soBaseInfo.PayTypeSysNo.Value))
            {

                //throw new BizException("订单的付款类型不是银行电汇或邮局汇款");
                throw new BizException(ResouceManager.GetMessageString("Invoice.PostPay", "PostPay_OrderTypeError"));
            }

            var postIncomeList = PostIncomeBL.GetListBySOSysNoList(new List<int>() { soSysNo });
            if (postIncomeList == null || postIncomeList.Count == 0)
            {
                //throw new BizException("订单未找到有效的邮局电汇收款单");
                throw new BizException(ResouceManager.GetMessageString("Invoice.PostPay", "PostPay_DeActiveBill"));
            }
            var postIncomeInfo = postIncomeList[0];

            decimal incomeAmt = postIncomeInfo.IncomeAmt ?? 0;
            decimal checkedOrderAmt = 0;
            decimal refundAmt = 0;

            var confirmedOrderList = PostIncomeBL.GetConfirmedListByPostIncomeSysNo(postIncomeInfo.SysNo.Value);
            var soSysNoList = confirmedOrderList.Where(w => w.Status == PostIncomeConfirmStatus.Audit)
                                  .Select(s => s.ConfirmedSoSysNo.Value)
                                  .ToList();

            var soList = ExternalDomainBroker.GetSOBaseInfoList(soSysNoList);
            checkedOrderAmt = soList.Sum(s => s.ReceivableAmount);

            refundAmt = PostPayBL.GetRefundAmtByConfirmedSOSysNoList(soSysNoList);

            //计算收款单剩余金额
            remainAmt = incomeAmt - checkedOrderAmt + refundAmt;

            return confirmedOrderList;
        }
    }
}