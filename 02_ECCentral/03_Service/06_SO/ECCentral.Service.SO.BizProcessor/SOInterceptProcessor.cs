using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOInterceptProcessor))]
    public class SOInterceptProcessor
    {
        private ISOInterceptDA m_da = ObjectFactory<ISOInterceptDA>.Instance;

        /// <summary>
        /// 添加订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="companyCode"></param> 
        public virtual void AddSOInterceptInfo(SOInterceptInfo info, string companyCode)
        {
            m_da.AddSOInterceptInfo(info, companyCode);
        }

        /// <summary>
        /// 批量修改订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        public void BatchUpdateSOInterceptInfo(SOInterceptInfo info)
        {
            m_da.BatchUpdateSOInterceptInfo(info);
        }

        /// <summary>
        /// 删除订单拦截设置信息
        /// </summary>
        /// <param name="info"></param>
        public void DeleteSOInterceptInfo(SOInterceptInfo info)
        {
            m_da.DeleteSOInterceptInfo(info);
        }

        /// <summary>
        /// 发送订单拦截邮件
        /// </summary>
        /// <param name="info"></param>
        public virtual void SendSOOrderInterceptEmail(SOInfo info,string Language)
        {
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("SOSysNo", info.BaseInfo.SysNo.Value);
            keyValueVariables.Add("CustomerSysNo", info.BaseInfo.CustomerSysNo);
            keyValueVariables.Add("OrderDate", info.BaseInfo.CreateTime);
            SOStatusChangeInfo statusAuditInfo = info.StatusChangeInfoList.Find(x => { return x.Status == SOStatus.WaitingOutStock; });
            if (statusAuditInfo == null)
            {
                keyValueVariables.Add("AuditTime", DBNull.Value);
            }
            else
            {
                keyValueVariables.Add("AuditTime", statusAuditInfo.ChangeTime.Value.ToString());
            }
            SOStatusChangeInfo statusOutStockInfo = info.StatusChangeInfoList.Find(x => { return x.Status == SOStatus.OutStock; });
            if (statusOutStockInfo == null)
            {
                keyValueVariables.Add("OutStockTime", DBNull.Value);
            }
            else
            {
                keyValueVariables.Add("OutStockTime", statusOutStockInfo.ChangeTime.Value.ToString());
            }
            keyValueVariables.Add("SOTotalAmount", info.BaseInfo.SOTotalAmount);
            keyValueVariables.Add("PayType", info.BaseInfo.PayTypeSysNo);
            keyValueVariables.Add("UserSysNo", ServiceContext.Current.UserSysNo);
            List<string> emailList = new List<string>();
            List<string> emailCCList = new List<string>();
            if( info.SOInterceptInfoList!=null&& info.SOInterceptInfoList.Count==1)
            {
                foreach (var item in info.SOInterceptInfoList)
                {
                    if (!string.IsNullOrEmpty(item.EmailAddress))
                    {
                        emailList.Add(item.EmailAddress);
                    }
                    if (!string.IsNullOrEmpty(item.CCEmailAddress))
                    {
                        emailCCList.Add(item.CCEmailAddress);
                    }                  
                }                
            }
            else if(info.SOInterceptInfoList!=null&& info.SOInterceptInfoList.Count==2)
            {
                if (info.SOInterceptInfoList[0].EmailAddress != info.SOInterceptInfoList[1].EmailAddress
                    || info.SOInterceptInfoList[0].CCEmailAddress != info.SOInterceptInfoList[1].CCEmailAddress)
                {
                    emailList.Add(info.SOInterceptInfoList[1].EmailAddress);
                    emailCCList.Add(info.SOInterceptInfoList[1].CCEmailAddress);
                }
            }
            else if (info.SOInterceptInfoList != null && info.SOInterceptInfoList.Count > 2)
            {
                if (info.SOInterceptInfoList[0].EmailAddress != info.SOInterceptInfoList[info.SOInterceptInfoList.Count-1].EmailAddress
                    || info.SOInterceptInfoList[0].CCEmailAddress != info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].CCEmailAddress)
                {
                    emailList.Add(info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].EmailAddress);
                    emailCCList.Add(info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].CCEmailAddress);
                }
                else
                {
                    foreach (var item in info.SOInterceptInfoList)
                    {
                        emailList.Add(item.EmailAddress);
                        emailCCList.Add(item.CCEmailAddress);
                    }
                }
            }
            if(emailList!=null&&emailList.Count>0)
            {
                ExternalDomainBroker.SendInternalEmail(emailList.Join(";"), emailCCList.Join(";"), "SO_OrderIntercept", keyValueVariables, Language);
            }                        
        }

        /// <summary>
        /// 发送增票拦截邮件
        /// </summary>
        /// <param name="info"></param>
        public virtual void SendSOFinanceInterceptEmail(SOInfo info, string Language)
        {
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            keyValueVariables.Add("SOSysNo", info.BaseInfo.SysNo.Value);
            keyValueVariables.Add("CustomerSysNo", info.BaseInfo.CustomerSysNo);
            keyValueVariables.Add("OrderDate", info.BaseInfo.CreateTime);

            SOStatusChangeInfo statusAuditInfo = info.StatusChangeInfoList.Find(x => { return x.Status == SOStatus.WaitingOutStock; });
            if (statusAuditInfo == null)
            {
                keyValueVariables.Add("AuditTime", DBNull.Value);
            }
            else
            {
                keyValueVariables.Add("AuditTime", statusAuditInfo.ChangeTime.Value.ToString());
            }
            SOStatusChangeInfo statusOutStockInfo = info.StatusChangeInfoList.Find(x => { return x.Status == SOStatus.OutStock; });
            if (statusOutStockInfo == null)
            {
                keyValueVariables.Add("OutStockTime", DBNull.Value);
            }
            else
            {
                keyValueVariables.Add("OutStockTime", statusOutStockInfo.ChangeTime.Value.ToString());
            }
                        
            keyValueVariables.Add("SOTotalAmount", info.BaseInfo.SOTotalAmount);
            keyValueVariables.Add("PayType", info.BaseInfo.PayTypeSysNo);
            keyValueVariables.Add("UserSysNo", ServiceContext.Current.UserSysNo);
            List<string> emailList = new List<string>();
            List<string> emailCCList = new List<string>();
            if (info.SOInterceptInfoList != null && info.SOInterceptInfoList.Count == 1)
            {
                foreach (var item in info.SOInterceptInfoList)
                {                   
                    if (!string.IsNullOrEmpty(item.FinanceEmailAddress))
                    {
                        emailList.Add(item.FinanceEmailAddress);
                    }
                    if (!string.IsNullOrEmpty(item.FinanceCCEmailAddress))
                    {
                        emailCCList.Add(item.FinanceCCEmailAddress);
                    }
                }
            }
            else if (info.SOInterceptInfoList != null && info.SOInterceptInfoList.Count == 2)
            {
                if (info.SOInterceptInfoList[0].FinanceEmailAddress != info.SOInterceptInfoList[1].FinanceEmailAddress
                    || info.SOInterceptInfoList[0].FinanceCCEmailAddress != info.SOInterceptInfoList[1].FinanceCCEmailAddress)
                {
                    emailList.Add(info.SOInterceptInfoList[1].FinanceEmailAddress);
                    emailCCList.Add(info.SOInterceptInfoList[1].FinanceCCEmailAddress);
                }
            }
            else if (info.SOInterceptInfoList != null && info.SOInterceptInfoList.Count > 2)
            {
                if (info.SOInterceptInfoList[0].EmailAddress != info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].FinanceEmailAddress
                    || info.SOInterceptInfoList[0].FinanceCCEmailAddress != info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].FinanceCCEmailAddress)
                {
                    emailList.Add(info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].FinanceEmailAddress);
                    emailCCList.Add(info.SOInterceptInfoList[info.SOInterceptInfoList.Count - 1].FinanceCCEmailAddress);
                }
                else
                {
                    foreach (var item in info.SOInterceptInfoList)
                    {
                        emailList.Add(item.FinanceEmailAddress);
                        emailCCList.Add(item.FinanceCCEmailAddress);
                    }
                }
            }
            if (emailList != null && emailList.Count > 0)
            {
                ExternalDomainBroker.SendInternalEmail(emailList.Join(";"),emailCCList.Join(";"),"SO_OrderIntercept", keyValueVariables, Language);
            }
        }
    }
}
