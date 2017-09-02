using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CustomerVisitProcessor))]
    public class CustomerVisitProcessor
    {
        ICustomerVisitDA CustomerVisitDA = null;
        public CustomerVisitProcessor()
        {
            CustomerVisitDA = ObjectFactory<ICustomerVisitDA>.Instance;
        }
        public virtual VisitLog AddCustomerVisitLog(VisitLog log)
        {
            if (log != null)
            {
                VisitCustomer visitCustomerInfo = CustomerVisitDA.GetVisitCustomerByCustomerSysNo(log.CustomerSysNo.Value);

                if (visitCustomerInfo == null)
                {
                    visitCustomerInfo = AddVisitCusotmer(log);
                }
                else
                {
                    if (!visitCustomerInfo.LastCallStatus.HasValue)
                    {
                        UpdateVisitCusotmer(visitCustomerInfo, log, false);
                    }
                    else
                    {
                        int VisitCustomerActiveNeedDays = 180;
                        //if (!int.TryParse(ConfigurationManager.AppSettings["VisitCustomerActiveNeedDays"], out VisitCustomerActiveNeedDays))
                        //{
                        //    VisitCustomerActiveNeedDays = 180;
                        //}

                        //-1:回访处理失败;1:最近一次回访成功
                        if (visitCustomerInfo.LastCallStatus.Value == VisitDealStatus.Failed)
                        {
                            throw new BizException(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddCustomerVisitLog_LastCallStatus_Failed"));
                        }
                        else if (visitCustomerInfo.LastCallStatus.Value == VisitDealStatus.Complete)
                        {
                            if (DateTime.Now.Subtract(visitCustomerInfo.LastCallTime.Value).Days < VisitCustomerActiveNeedDays)
                            {
                                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddCustomerVisitLog_LastCallStatus_Activating"));
                            }
                        }

                        if ((!visitCustomerInfo.LastCallTime.HasValue
                               || DateTime.Now.Subtract(visitCustomerInfo.LastCallTime.Value).Days >= VisitCustomerActiveNeedDays)
                             && (!visitCustomerInfo.LastBuyTime.HasValue || DateTime.Now.Subtract(visitCustomerInfo.LastBuyTime.Value).Days >= VisitCustomerActiveNeedDays))
                        {
                            //以前回访过，但是最后一次购物时间距离当前时间超过了规定的时间，需要再一次回访
                            visitCustomerInfo = AddVisitCusotmer(log);
                        }
                        else
                        {
                            UpdateVisitCusotmer(visitCustomerInfo, log, false);
                        }
                    }
                }
                log.VisitSysNo = visitCustomerInfo.SysNo;

                CustomerVisitDA.InsertCustomerVisitLog(log);

                if (log.DealStatus.Value == VisitDealStatus.Failed)
                {
                    //如果回访处理失败，则添加到黑名单中且从ToVisitCustomer 表中删除这个用户
                    CustomerVisitDA.InsertVisitBlackCustomer(log);
                }
            }
            return log;
        }

        private VisitCustomer AddVisitCusotmer(VisitLog log)
        {
            VisitCustomer info = new VisitCustomer
            { 
                ConsumeDesire = log.ConsumeDesire,
                ContactCount = 1,
                CustomerID = log.CustomerID,
                CustomerSysNo = log.CustomerSysNo,
                //EditDate = log.EditDate,
                //EditUserAcct = log.EditUserAcct,
                //EditUserSysNo = log.EditUserSysNo,
                //InDate = log.InDate,
                //InUserAcct = log.InUserAcct,
                //InUserSysNo = log.InUserSysNo,
                IsRMA = false, 
                LastCallResult = log.CallResult,
                LastCallStatus = log.DealStatus,
                LastCallTime = DateTime.Now,
                OrderAmount = 0,
                OrderCount = 0, 
            };
            info = CustomerVisitDA.InsertVisitCustomer(info);
            return info;
        }

        private VisitCustomer UpdateVisitCusotmer(VisitCustomer oldVisitCustomerInfo, VisitLog log, bool isOrderVisitLog)
        {
            VisitCustomer info = new VisitCustomer();
            info.LastCallStatus = isOrderVisitLog ? oldVisitCustomerInfo.LastCallStatus : log.DealStatus;
            info.LastMaintenanceStatus = isOrderVisitLog ? log.DealStatus : oldVisitCustomerInfo.LastMaintenanceStatus;
            info.LastCallResult = log.CallResult;
            info.ConsumeDesire = log.ConsumeDesire;
            info.LastCallTime = DateTime.Now;
            info.ContactCount = oldVisitCustomerInfo.ContactCount.HasValue ? (oldVisitCustomerInfo.ContactCount + 1) : 1;
            //info.EditDate = DateTime.Now;
            //info.EditUserSysNo = log.EditUserSysNo;
            info.SysNo = oldVisitCustomerInfo.SysNo;
            info.CompanyCode = log.CompanyCode;

            info = CustomerVisitDA.UpdateVisitCustomer(info);
            return info;
        }

        public virtual VisitLog AddOrderVisitLog(VisitLog log)
        {
            VisitCustomer visitCustomer = CustomerVisitDA.GetVisitCustomerByCustomerSysNo(log.CustomerSysNo.Value);

            if (visitCustomer == null)
            {
                throw new BizException( string.Format(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddOrderVisitLog_Customer_NoSelect"), 
                    log.CustomerSysNo));
            }
            if (!(visitCustomer.IsActive.HasValue && visitCustomer.IsActive.Value) || !(visitCustomer.IsRMA.HasValue && visitCustomer.IsRMA.Value))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddOrderVisitLog_Customer_NoActivation"));
            }
            if (visitCustomer.LastMaintenanceStatus != null && (visitCustomer.LastMaintenanceStatus == VisitDealStatus.Complete || visitCustomer.LastMaintenanceStatus == VisitDealStatus.Failed))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddOrderVisitLog_Maintenance_Repeat"));
            }

            log.VisitSysNo = visitCustomer.SysNo;
            try
            {
                CustomerVisitDA.InsertOrderVisitLog(log);
            }
            catch (System.Exception e)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("Customer.CustomerVisit", "AddOrderVisitLog_Err"),
                    e.Message));
            }


            UpdateVisitCusotmer(visitCustomer, log, true);

         
            return log;
        }

        #region Load

        public virtual List<VisitLog> GetCustomerVisitLogsByCustomerSysNo(int customerSysNo)
        {
            return CustomerVisitDA.GetCustomerVisitLogsByCustomerSysNo(customerSysNo);
        }
        public virtual List<VisitLog> GetOrderVisitLogsByCustomerSysNo(int customerSysNo)
        {
            return CustomerVisitDA.GetOrderVisitLogsByCustomerSysNo(customerSysNo);
        }
        public virtual List<VisitLog> GetCustomerVisitLogsByVisitSysNo(int visitSysNo)
        {
            return CustomerVisitDA.GetCustomerVisitLogsByVisitSysNo(visitSysNo);
        }
        public virtual List<VisitLog> GetOrderVisitLogsByVisitSysNo(int visitSysNo)
        {
            return CustomerVisitDA.GetOrderVisitLogsByVisitSysNo(visitSysNo);
        }

        #endregion

    }
}
