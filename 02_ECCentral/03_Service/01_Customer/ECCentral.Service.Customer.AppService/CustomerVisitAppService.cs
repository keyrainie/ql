using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CustomerVisitAppService))]
    public class CustomerVisitAppService
    {
        CustomerVisitProcessor VisitProcessor = ObjectFactory<CustomerVisitProcessor>.Instance;
        public virtual VisitLog AddCustomerVisitLog(VisitLog log)
        {
            log = VisitProcessor.AddCustomerVisitLog(log);
            if (log.DealStatus.Value == VisitDealStatus.FollowUp && log.RemindDate.HasValue)
            {
                //需要跟进
                //CreateRemindEmail(entity);
            }
            return log;
        }
        public virtual VisitLog AddOrderVisitLog(VisitLog log)
        {
            log = VisitProcessor.AddOrderVisitLog(log);
            if (log.DealStatus == VisitDealStatus.FollowUp && log.RemindDate.HasValue)
            {
                CreateRemindEmail(log);
            }
            return log;
        }
        private void CreateRemindEmail(VisitLog log)
        {
            throw new BizException("提醒邮件没有发送，请修改此处代码。");
            //IPPSystemUserInfo ippuserinfo = SystemUserHelper.GetUserInfoBy(log.CreateUserSysNumber);
            //UserInfo userinfo = null;
            //try
            //{
            //    userinfo = ControlPanelServiceAdapter.GetUserInfo(new UserInfo() { LoginName = ippuserinfo.UniqueUserName, CompanyCode = ippuserinfo.CompanyCode });
            //}
            //catch (System.Exception e)
            //{
            //    throw new BusinessException("获取回访操作者信息失败：" + e.Message);
            //}

            //if (userinfo != null)
            //{
            //    VisitEmailRemind entityVER = new VisitEmailRemind();
            //    entityVER.CSEmail = userinfo.EmailAddress;
            //    entityVER.VisitSysNo = visitCustomer.SystemNumber;
            //    entityVER.RemindDate = log.RemindDate;
            //    entityVER.VisitType = 1;
            //    entityVER.CreateDate = log.CreateDate;
            //    entityVER.CreateUserSysNumber = log.CreateUserSysNumber;
            //    entityVER.CustomerSysNo = log.CustomerSysNo;
            //    try
            //    {
            //        VisitEmailRemindDA.InsertVisitEmailRemind(entityVER);
            //    }
            //    catch (System.Exception e)
            //    {
            //        throw new BusinessException("添加维护邮件提醒失败：" + e.Message);
            //    }

            //}
        }
        #region Load

        public virtual List<VisitLog> GetCustomerVisitLogsByCustomerSysNo(int customerSysNo)
        {
            return VisitProcessor.GetCustomerVisitLogsByCustomerSysNo(customerSysNo);
        }
        public virtual List<VisitLog> GetOrderVisitLogsByCustomerSysNo(int customerSysNo)
        {
            return VisitProcessor.GetOrderVisitLogsByCustomerSysNo(customerSysNo);
        }
        public virtual List<VisitLog> GetCustomerVisitLogsByVisitSysNo(int visitSysNo)
        {
            return VisitProcessor.GetCustomerVisitLogsByVisitSysNo(visitSysNo);
        }
        public virtual List<VisitLog> GetOrderVisitLogsByVisitSysNo(int visitSysNo)
        {
            return VisitProcessor.GetOrderVisitLogsByVisitSysNo(visitSysNo);
        }

        #endregion
    }
}
