using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using System.Transactions;
using System.Security.Cryptography;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CustomerProcessor))]
    public class CustomerProcessor
    {
        private ICustomerInfoDA customerDA = ObjectFactory<ICustomerInfoDA>.Instance;

        /// <summary>
        /// 判断用户是否存在的方法，如果适应新的应用场景，需要重写该方法
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CheckCustomerIDIsExists(CustomerInfo entity)
        {

            List<CustomerBasicInfo> existCustomers = ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerByCustomerIdList(entity.BasicInfo.CustomerID);
            if (existCustomers != null && existCustomers.Count > 0)
            {
                string msg = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "CreateCustomer_CustomerID_Repeat"),
                    entity.BasicInfo.CustomerID);
                throw new BizException(msg);
            }


        }

        public virtual void CheckCustomerEmailIsExists(CustomerInfo entity)
        {
            List<CustomerBasicInfo> existCustomers = ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerByEmailList(entity.BasicInfo.Email);
            if (entity.SysNo == null)//添加
            {
                if (existCustomers != null && existCustomers.Count > 0)
                {
                    string msg = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "CreateCustomer_Email_Repeat"),
                        entity.BasicInfo.Email);
                    throw new BizException(msg);
                }
            }
            else //update
            {
                if (existCustomers != null && existCustomers.Exists(p => p.CustomerSysNo != entity.SysNo))
                {
                    string msg = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "UpdateCustomer_Email_Repeat"),
                        entity.BasicInfo.Email);
                    throw new BizException(msg);
                }
            }
        }

        /// <summary>
        /// 检查顾客手机号码的唯一性
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CheckCustomerCellPhoneIsExists(CustomerInfo entity)
        {
            //未输入的不检查
            if (string.IsNullOrEmpty(entity.BasicInfo.CellPhone)) return;

            CustomerBasicInfo existCustomer = ObjectFactory<ICustomerBasicDA>.Instance.CheckSameCellPhone(entity.BasicInfo, entity.CompanyCode);
            if (entity.SysNo == null)//Add
            {
                if (existCustomer != null)
                {
                    string msg = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "CreateCustomer_CellPhone_Repeat"), entity.BasicInfo.CellPhone);
                    throw new BizException(msg);
                }
            }
            else //Update
            {
                if (existCustomer != null)
                {
                    string msg = string.Format(ResouceManager.GetMessageString("Customer.CustomerInfo", "UpdateCustomer_CellPhone_Repeat"), entity.BasicInfo.CellPhone);
                    throw new BizException(msg);
                }
            }
        }
        private void CheckCustomerIdentityCardIsExists(CustomerInfo entity)
        {
            if (string.IsNullOrEmpty(entity.BasicInfo.IdentityCard)) return;
            List<CustomerBasicInfo> existCustomers = ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerByIdentityCard(entity.BasicInfo.IdentityCard);
            if (entity.SysNo == null)//添加
            {
                if (existCustomers != null && existCustomers.Count > 0)
                {
                    string msg = string.Format("创建顾客失败，原因：已有此身份证号码：{0}", entity.BasicInfo.IdentityCard);
                    throw new BizException(msg);
                }
            }
            else //update
            {
                if (existCustomers != null && existCustomers.Exists(p => p.CustomerSysNo != entity.SysNo))
                {
                    string msg = string.Format("编辑顾客失败，原因：已有此身份证号码：{0}", entity.BasicInfo.IdentityCard);
                    throw new BizException(msg);
                }
            }
        }
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CustomerInfo CreateCustomer(CustomerInfo entity)
        {
            CheckCustomerIDIsExists(entity);
            CheckCustomerEmailIsExists(entity);
            CheckCustomerCellPhoneIsExists(entity);
            CheckCustomerIdentityCardIsExists(entity);
            using (TransactionScope tran = new TransactionScope())
            {
                //1.create cusotmer basic info
                ObjectFactory<ICustomerBasicDA>.Instance.CreateBasicInfo(entity.BasicInfo);
                entity.SysNo = entity.BasicInfo.CustomerSysNo;
                //2.create password info
                EncryptionPassword(entity);
                entity.PasswordInfo.CustomerSysNo = entity.SysNo;
                ObjectFactory<ICustomerBasicDA>.Instance.UpdatePassword(entity.PasswordInfo);
                //3.create cusotmerinfo
                ObjectFactory<ICustomerInfoDA>.Instance.CreateDetailInfo(entity);
                //4.create agentinfo
                entity.AgentInfo.CustomerSysNo = entity.SysNo;
                ObjectFactory<AgentProcessor>.Instance.CreateAgent(entity.AgentInfo);
                //5.create accountperiodinfo
                entity.AccountPeriodInfo.CustomerSysNo = entity.SysNo;
                ObjectFactory<AccountPeridProcessor>.Instance.CreateAccountPeriodInfo(entity.AccountPeriodInfo);
                tran.Complete();
            }
            return entity;
        }

        public virtual void UpdateCustomer(CustomerInfo entity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //1.邮件地址重复性检查
                CheckCustomerEmailIsExists(entity);
                //2.如果用户是否恶意用户有调整的话需要记录日志
                if (GetCsutomerDeatilInfo(entity.SysNo.Value).IsBadCustomer != entity.IsBadCustomer)
                    MaintainMaliceUser(entity.SysNo.Value, entity.IsBadCustomer.Value, new CustomerOperateLog() { CustomerSysNo = entity.SysNo.Value });
                //3 check customer cellphone repeat
                CheckCustomerCellPhoneIsExists(entity);
                CheckCustomerIdentityCardIsExists(entity);
                //4.update cusotmer basic info
                ObjectFactory<ICustomerBasicDA>.Instance.UpdateBasicInfo(entity.BasicInfo);
                //5.update cusotmerinfo
                ObjectFactory<ICustomerInfoDA>.Instance.UpdateDetailInfo(entity);
                scope.Complete();
            }
        }
        /// <summary> 
        /// 设置顾客头像状态
        /// </summary>
        public virtual void UpdateAvatarStatus(int CustomerSysNo, AvtarShowStatus AvtarImageStatus)
        {
            customerDA.UpdateAvatarStatus(CustomerSysNo, AvtarImageStatus);
        }


        public virtual CustomerInfo GetCustomerBySysNo(int sysNo)
        {
            CustomerInfo customer = GetCsutomerDeatilInfo(sysNo);
            if (customer != null)
            {
                customer.BasicInfo = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(sysNo);
                customer.AgentInfo = ObjectFactory<AgentProcessor>.Instance.GetAgentByCustomerSysNo(sysNo); ;
                customer.ShippingAddressList = ObjectFactory<ShippingAddressProcessor>.Instance.QueryShippingAddress(sysNo);
                customer.ValueAddedTaxInfoList = ObjectFactory<ValueAddedTaxProcessor>.Instance.QueryValueAddedTaxInfo(sysNo);
                customer.AccountPeriodInfo = ObjectFactory<AccountPeridProcessor>.Instance.GetCustomerAccountPeriod(sysNo);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CustomerInfo", "Customer_CustomerNoFound"));
            }
            return customer;
        }
        public virtual CustomerInfo GetCsutomerDeatilInfo(int sysNo)
        {
            return customerDA.GetCustomerBySysNo(sysNo);
        }

        public virtual void MaintainMaliceUser(int customerSysNo, bool isMalice, CustomerOperateLog entity)
        {
            if (isMalice)
                entity.EventType = ResouceManager.GetMessageString("Customer.CustomerInfo", "UpdateToBadUser");
            else
                entity.EventType = ResouceManager.GetMessageString("Customer.CustomerInfo", "UpdateToGoodUser");
            customerDA.InsertCustomerInfoOperateLog(entity);
            customerDA.UpdateIsBadCustomer(customerSysNo, isMalice);

        }

        public virtual void AbandonCustomer(CustomerBasicInfo entity)
        {
            entity.Status = CustomerStatus.InValid;
            customerDA.UpdateCustomerStatus(entity.CustomerSysNo.Value, entity.Status.Value);
        }

        public virtual void CancelConfirmEmail(CustomerBasicInfo entity)
        {
            entity.IsEmailConfirmed = false;
            customerDA.CancelConfirmEmail(entity.CustomerSysNo.Value, entity.IsEmailConfirmed.Value);
        }

        public virtual void CancelConfirmPhone(CustomerBasicInfo entity)
        {
            entity.CellPhoneConfirmed = false;
            customerDA.CancelConfirmPhone(entity.CustomerSysNo.Value, entity.CellPhoneConfirmed.Value);
        }

        #region CustomerBasicInfo

        public virtual CustomerBasicInfo GetCustomerBasicInfoBySysNo(int sysNo)
        {
            return ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerBasicInfoBySysNo(sysNo);
        }
        public virtual CustomerBasicInfo GetCustomerBasicInfoByID(string customerID)
        {
            return ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerBasicInfoByID(customerID);
        }
        public virtual List<CustomerBasicInfo> GetCustomerBasicInfoBySysNos(List<int> sysNos)
        {
            string ids = string.Empty;
            sysNos.ForEach(item => ids += item + ",");
            return ObjectFactory<ICustomerBasicDA>.Instance.GetCustomerBasicInfoBySysNoList(ids.TrimEnd(','));
        }
        #endregion



        #region customerpasswordInfo
        private void EncryptionPassword(CustomerInfo entity)
        {
            //IPP有个配置可以保留明文(EnableCustomerEncryption)，这样不好，去掉保留明文功能，全部存密文
            string guid = Guid.NewGuid().ToString("N").Trim();
            entity.PasswordInfo.PasswordSalt = guid;
            entity.PasswordInfo.Password = GetEncryptedString(entity.PasswordInfo.Password.Trim() + guid);
            
        }
        public static string GetEncryptedString(string oldstring)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] source = Encoding.UTF8.GetBytes(oldstring);
            byte[] destination = sha1.ComputeHash(source);
            sha1.Clear();
            (sha1 as IDisposable).Dispose();
            return Convert.ToBase64String(destination);
        }
        #endregion

        public virtual void InsertCustomerInfoOperateLog(CustomerOperateLog entity)
        {
            customerDA.InsertCustomerInfoOperateLog(entity);
        }


        public virtual List<CustomerBasicInfo> GetSystemAccount(string webChannelID)
        {
            return ObjectFactory<ICustomerBasicDA>.Instance.GetSystemAccount(webChannelID);
        }

        public virtual void UpdateOrderedAmount(int customerSysNo, decimal orderedAmt)
        {
            customerDA.AdjustOrderedAmount(customerSysNo, orderedAmt);
        }

        public virtual bool IsExists(int customerSysNo)
        {
            return ObjectFactory<ICustomerBasicDA>.Instance.IsExists(customerSysNo);
        }

        /// <summary>
        /// 获取所有恶意用户
        /// </summary>
        /// <param name="companyCode"></param>
        public virtual List<CustomerInfo> GetMalevolenceCustomerList(string companyCode)
        {
            return ObjectFactory<ICustomerInfoDA>.Instance.GetMalevolenceCustomerList(companyCode);
        }
    }
}
