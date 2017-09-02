using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using System.Transactions;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CustomerAppService))]
    public class CustomerAppService
    {

        #region 用户处理
        public virtual bool IsExists(int customerSysNo)
        {
            return ObjectFactory<CustomerProcessor>.Instance.IsExists(customerSysNo);
        }
        /// <summary>
        /// 根据顾客的SysNo获取顾客的全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CustomerInfo GetCustomerBySysNo(int sysNo)
        {
            return ObjectFactory<CustomerProcessor>.Instance.GetCustomerBySysNo(sysNo);
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CustomerInfo CreateCustomer(CustomerInfo entity)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                CustomerInfo info = ObjectFactory<CustomerProcessor>.Instance.CreateCustomer(entity);
                ExternalDomainBroker.CreateOperationLog("Insert Customer", BizLogType.Basic_Customer_Add, entity.SysNo.Value, entity.CompanyCode);
                tran.Complete();
                return info;
            }
        }

        public virtual void UpdateCustomer(CustomerInfo entity)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                ObjectFactory<CustomerProcessor>.Instance.UpdateCustomer(entity);
                ExternalDomainBroker.CreateOperationLog("Update Customer", BizLogType.Basic_Customer_Update, entity.SysNo.Value, entity.CompanyCode);
                tran.Complete();
            }
        }

        /// <summary>
        /// 设置恶意用户
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="isMalice">是否恶意用户</param>
        /// <param name="memo">备注</param>
        /// <param name="SoSysNo">对应的订单编号</param>
        public virtual void SetMaliceCustomer(int customerSysNo, bool isMalice, string memo, int? SoSysNo)
        {
            CustomerOperateLog log = new CustomerOperateLog();
            log.Memo = memo;
            log.SOSysNo = SoSysNo;
            ObjectFactory<CustomerProcessor>.Instance.MaintainMaliceUser(customerSysNo, isMalice, log);
        }

        /// <summary>
        /// 批量设置顾客头像状态
        /// </summary>
        public virtual void BatchUpdateAvatarStatus(List<int> CustomerSysNoList, AvtarShowStatus AvtarImageStatus)
        {
            CustomerProcessor processor = ObjectFactory<CustomerProcessor>.Instance;
            foreach (int sysNo in CustomerSysNoList)
            {
                processor.UpdateAvatarStatus(sysNo, AvtarImageStatus);
            }
        }



        public virtual void AbandonCustomer(CustomerInfo customer)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                ObjectFactory<CustomerProcessor>.Instance.AbandonCustomer(customer.BasicInfo);
                ExternalDomainBroker.CreateOperationLog("Update CustomerStatus", BizLogType.Basic_Customer_Invalid, customer.BasicInfo.CustomerSysNo.Value, customer.CompanyCode);
                tran.Complete();
            }
        }

        public virtual void CancelConfirmEmail(CustomerInfo customer)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                ObjectFactory<CustomerProcessor>.Instance.CancelConfirmEmail(customer.BasicInfo);
                ExternalDomainBroker.CreateOperationLog("Cancel confirm customer email", BizLogType.Basic_Customer_CancelConfirmEmail, customer.BasicInfo.CustomerSysNo.Value, customer.CompanyCode);
                tran.Complete();
            }
        }

        public virtual void CancelConfirmPhone(CustomerInfo customer)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                ObjectFactory<CustomerProcessor>.Instance.CancelConfirmPhone(customer.BasicInfo);
                ExternalDomainBroker.CreateOperationLog("Cancel confirm customer phone", BizLogType.Basic_Customer_CancelConfirmPhone, customer.BasicInfo.CustomerSysNo.Value, customer.CompanyCode);
                tran.Complete();
            }
        }
        /// <summary>
        /// 手动调整经验值
        /// </summary>
        /// <param name="CustomerSysNo"></param>
        /// <param name="Amount"></param>
        /// <param name="Memo"></param>
        /// <returns></returns>
        public virtual void AdjustExperience(CustomerExperienceLog adjustInfo)
        {
            ObjectFactory<ExperienceProcessor>.Instance.Adjust(adjustInfo);
        }

        public virtual void ManaulSetVipRank(int customerSysNo, VIPRank rank, string companyCode)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                ObjectFactory<RankProcessor>.Instance.SetVIPRank(customerSysNo, rank);
                ExternalDomainBroker.CreateOperationLog("Update Customer", BizLogType.Basic_Customer_ManualSetVIPRank, customerSysNo, companyCode);
                tran.Complete();
            }
        }
        public virtual void MaintainMaliceUser(CustomerInfo entity)
        {
            ObjectFactory<CustomerProcessor>.Instance.MaintainMaliceUser(entity.SysNo.Value, entity.IsBadCustomer.Value, entity.OperateLog);
        }
        #endregion

        #region 代理
        public virtual AgentInfo CreateAgent(AgentInfo entity)
        {
            return ObjectFactory<AgentProcessor>.Instance.CreateAgent(entity);
        }

        public virtual AgentInfo UpdateAgent(AgentInfo entity)
        {
            return ObjectFactory<AgentProcessor>.Instance.UpdateAgent(entity);
        }

        public virtual AgentInfo GetAgentByCustomerSysNo(int customerSysNo)
        {
            return ObjectFactory<AgentProcessor>.Instance.GetAgentByCustomerSysNo(customerSysNo);
        }
        #endregion

        #region 收货地址信息

        /// <summary>
        /// 创建收货地址信息
        /// </summary>
        /// <param name="shippingAddress"></param>
        /// <returns></returns>
        public virtual ShippingAddressInfo CreateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            return ObjectFactory<ShippingAddressProcessor>.Instance.CreateShippingAddress(shippingAddress);
        }

        /// <summary>
        /// 更新收货地址信息
        /// </summary>
        /// <param name="shippingAddress"></param>
        /// <returns></returns>
        public virtual void UpdateShippingAddress(ShippingAddressInfo shippingAddress)
        {
            ObjectFactory<ShippingAddressProcessor>.Instance.UpdateShippingAddress(shippingAddress);
        }

        /// <summary>
        /// 查询收货地址列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual List<ShippingAddressInfo> QueryShippingAddress(int customerSysNo)
        {
            return ObjectFactory<ShippingAddressProcessor>.Instance.QueryShippingAddress(customerSysNo);
        }

        /// <summary>
        /// 获取默认收货地址
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual ShippingAddressInfo GetDefaultShippingAddress(int customerSysNo)
        {
            return ObjectFactory<ShippingAddressProcessor>.Instance.GetDefaultShippingAddress(customerSysNo);
        }

        #endregion 收货地址信息

        #region 增值税信息

        /// <summary>
        /// 创建增值税信息
        /// </summary>
        /// <param name="vat"></param>
        /// <returns></returns>
        public virtual ValueAddedTaxInfo CreateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            CertificateFileHandel(vat);
            return ObjectFactory<ValueAddedTaxProcessor>.Instance.CreateValueAddedTaxInfo(vat);
        }

        /// <summary>
        /// 更新增值税信息
        /// </summary>
        /// <param name="vat"></param>
        /// <returns></returns>
        public virtual void UpdateValueAddedTaxInfo(ValueAddedTaxInfo vat)
        {
            CertificateFileHandel(vat);
            ObjectFactory<ValueAddedTaxProcessor>.Instance.UpdateValueAddedTaxInfo(vat);
        }
        /// <summary>
        /// 处理增值税业务中的证书
        /// </summary>
        /// <param name="vat"></param>
        private void CertificateFileHandel(ValueAddedTaxInfo vat)
        {
            string fileIdentity = vat.CertificateFileName;
            string getConfigPath = AppSettingManager.GetSetting("Customer", "CertificateFilesPath");
            if (!string.IsNullOrEmpty(fileIdentity))
            {
                if (!fileIdentity.Contains(getConfigPath))
                {
                    string fileName = Path.GetFileName(Encoding.UTF8.GetString(Convert.FromBase64String(fileIdentity)));
                    vat.CertificateFileName = getConfigPath + "\\" + fileName;
                    if (!Path.IsPathRooted(getConfigPath))
                    {
                        //是相对路径:
                        getConfigPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, getConfigPath);
                    }
                    string getDestinationPath = Path.Combine(getConfigPath, fileName);
                    string getFolder = Path.GetDirectoryName(getDestinationPath);
                    if (!Directory.Exists(getFolder))
                    {
                        Directory.CreateDirectory(getFolder);
                    }
                    //将上传的文件从临时文件夹剪切到目标文件夹:
                    FileUploadManager.MoveFile(fileIdentity, getDestinationPath);

                    FileUploadManager.DeleteFile(fileIdentity);
                }
            }
        }
        /// <summary>
        /// 查询增值税列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual List<ValueAddedTaxInfo> QueryValueAddedTaxInfo(int customerSysNo)
        {
            return ObjectFactory<ValueAddedTaxProcessor>.Instance.QueryValueAddedTaxInfo(customerSysNo);
        }

        /// <summary>
        /// 查询默认增值税信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public virtual ValueAddedTaxInfo GetDefaultValueAddedTaxInfo(int customerSysNo)
        {
            return ObjectFactory<ValueAddedTaxProcessor>.Instance.GetDefaultValueAddedTaxInfo(customerSysNo);
        }

        #endregion 增值税信息

        #region 权限
        /// <summary>
        /// 更新用户权限
        /// </summary>
        /// <param name="right"></param>
        public virtual void UpdateCustomerRight(int customerSysNo, List<CustomerRight> right)
        {
            ObjectFactory<CustomerRightProcessor>.Instance.UpdateCustomerRight(customerSysNo, right);
        }

        /// <summary>
        /// 获取用户所有的权限
        /// </summary>
        /// <param name="right"></param>
        public virtual List<CustomerRight> LoadAllCustomerRight(int customerSysNo)
        {
            return ObjectFactory<CustomerRightProcessor>.Instance.LoadAllCustomerRight(customerSysNo);
        }
        #endregion

        #region 账期
        /// <summary>
        /// 更新账期信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateAccountPeriodInfo(AccountPeriodInfo entity)
        {
            ObjectFactory<AccountPeridProcessor>.Instance.UpdateAccountPeriodInfo(entity);
        }
        #endregion



    }
}