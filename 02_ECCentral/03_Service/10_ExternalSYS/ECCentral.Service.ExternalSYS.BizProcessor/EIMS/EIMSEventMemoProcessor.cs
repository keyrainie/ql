using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(EIMSEventMemoProcessor))]
    public class EIMSEventMemoProcessor
    {
        private IEIMSEventMemoQueryFilterDA m_EIMSInvoice = ObjectFactory<IEIMSEventMemoQueryFilterDA>.Instance;

        #region 录入发票信息
        /// <summary>
        /// 录入发票信息
        /// </summary>
        /// <param name="entities"></param>
        public virtual EIMSInvoiceResult CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            EIMSInvoiceResult result = new EIMSInvoiceResult();

            result = CheckInvoiceInputInfo(entities);
            if (result.IsSucceed)
            {
                m_EIMSInvoice.CreateEIMSInvoiceInput(entities);
            }
            return result;
        }
        #endregion

        #region 修改发票信息
        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="entities"></param>
        public virtual EIMSInvoiceResult UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            EIMSInvoiceResult result = new EIMSInvoiceResult();

            try
            {
                m_EIMSInvoice.UpdateEIMSInvoiceInput(entities);
                result.IsSucceed = true;
            }
            catch(BizException ex)
            {
                result.IsSucceed = false;
                result.Error = ex.Message;
            }
            return result;
        }
        #endregion

        #region 检查发票号不能重复录入
        private EIMSInvoiceResult CheckInvoiceInputInfo(List<EIMSInvoiceInfoEntity> entities)
        {
            EIMSInvoiceResult result = new EIMSInvoiceResult
            {
                Error = string.Empty,
                IsSucceed = true
            };
            //检测发票号不能重复录入
            List<string> InvoiceInputNoList = new List<string>();

            entities.ForEach(e =>
            {
                InvoiceInputNoList.Add(e.InvoiceInputNo);
            });

            List<EIMSInvoiceInfoEntity> invoiceList = m_EIMSInvoice.QueryEIMSInvoiceInputByInvoiceInputSysNo(InvoiceInputNoList);

            //如果发票已经存在，则抛出异常
            if (invoiceList.Count > 0)
            {
                string error = "发票号 {0} 已在单据({1})中录入，不能重复录入。";
                Dictionary<string, List<string>> errorDic = new Dictionary<string, List<string>>();
                string invocieNumber = string.Empty;
                int i = 0;
                //EIMSInvoiceInput item = invoiceList[0];
                foreach (EIMSInvoiceInfoEntity item in invoiceList)
                {
                    List<EIMSInvoiceInputExtendInfo> exList = m_EIMSInvoice.QueryEIMSInvoiceInputExtendByInvoiceInputSysNo(item.SysNo);

                    if (exList != null && exList.Count > 0)
                    {
                        if (!errorDic.ContainsKey(item.InvoiceInputNo))
                        {
                            errorDic.Add(item.InvoiceInputNo, new List<string>());
                        }
                        foreach (EIMSInvoiceInputExtendInfo ex in exList)
                        {
                            errorDic[item.InvoiceInputNo].Add(ex.InvoiceNumber);
                        }
                    }
                }
                if (errorDic.Count > 0)
                {
                    foreach (string key in errorDic.Keys)
                    {
                        i = 0;
                        invocieNumber = string.Empty;
                        foreach (string value in errorDic[key])
                        {
                            if (i > 0)
                            {
                                invocieNumber += "、";
                            }
                            invocieNumber += value;
                            i++;
                        }
                        result.IsSucceed = false;
                        result.Error += string.Format(error, key, invocieNumber);
                    }
                }
            }
            else
                result.IsSucceed = true;
            return result;
        }
        #endregion
    }
}
