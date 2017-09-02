using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(FinanceProcessor))]
    public class FinanceProcessor
    {
        private IFinanceDA m_FinanceDA = ObjectFactory<IFinanceDA>.Instance;

        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="payableInfo"></param>
        public virtual PayableInfo AddMemo(PayableInfo info)
        {
            if (string.IsNullOrEmpty(info.NewMemo))
            {
                //throw new BizException("新建备注不能为空！");
                ThrowBizException("Finance_MemoNotNull");
            }

            PayableInfo oldInfo = null;
            var oldInfoList = m_FinanceDA.PayableQuery(new PayableCriteriaInfo() { SysNo = info.SysNo });
            if (null == oldInfoList || oldInfoList.Count == 0)
            {
                //throw new BizException("未找到此单据！");
                ThrowBizException("Finance_NotFoundBill");
            }
            else
            {
                oldInfo = oldInfoList[0];
            }

            string newMemo = string.Format("\r\n{0},[{1},({2})];", info.NewMemo,info.OperationUserFullName, DateTime.Now.ToString("yy-MM-dd,HH:mm"));

            if (newMemo.Length + ((string.IsNullOrEmpty(oldInfo.Memo)) ? 0 : oldInfo.Memo.Length) <= 500)
            {
                info.Memo += newMemo;
                m_FinanceDA.AddMemo(info.SysNo.Value,info.Memo,info.CompanyCode);
            }

            return info;
        }

        /// <summary>
        /// 查询备注
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual List<PayableInfo> PayableQuery(PayableCriteriaInfo info)
        {
            return m_FinanceDA.PayableQuery(info);
        }


        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Finance, msgKeyName), args);
        }
    }
}
