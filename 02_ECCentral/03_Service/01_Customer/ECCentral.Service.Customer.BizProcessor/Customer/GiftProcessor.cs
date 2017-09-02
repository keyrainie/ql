using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Customer.BizProcessor
{
    /// <summary>
    /// 奖品信息业务处理器
    /// </summary>
    [VersionExport(typeof(GiftProcessor))]
    public class GiftProcessor
    {
        private IGiftDA _giftDA = ObjectFactory<IGiftDA>.Instance;
        private ICustomerInfoDA _customerDA = ObjectFactory<ICustomerInfoDA>.Instance;

        private string GetString(string key)
        {
            return ResouceManager.GetMessageString("Customer.CustomerGift", key);
        }

        /// <summary>
        /// 创建顾客奖品信息
        /// </summary>
        public virtual void CreateGift(List<CustomerGift> msg)
        {
            int successCount = 0;
            StringBuilder sb = new StringBuilder();
            if (msg.Count <= 0)
            {
                throw new BizException(GetString("GiftQtyLimit1"));
            }
            //验证产品数量必须大于0
            if (msg.First().Quantity <= 0)
            {
                throw new BizException(GetString("GiftQtyLimit1"));
            }
            //TODO
            //验证商品是否存在
            ProductInfo product = ExternalDomainBroker.GetProductInfo(msg.First().ProductSysNo.Value);
            if (product == null)
            {
                throw new BizException(GetString("ProductAsGiftNotExists"));
            }
            if (product.ProductStatus != ProductStatus.InActive_UnShow)
            {
                throw new BizException(GetString("ProductStatusInvalid"));
            }
            //验证商品的状态必须是0

            foreach (var item in msg)
            {
                //验证顾客是否存在
                //原 依据SysNo判断
                //var customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(item.CustomerSysNo.Value);
                //if (customer == null)
                //{
                //    sb.AppendLine(string.Format(GetString("CustomerNotExists"), item.CustomerSysNo));
                //    continue;
                //}
                //现 依据CustomerID判断
                var customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoByID(item.CustomerID);
                if (customer == null)
                {
                    sb.AppendLine(string.Format(GetString("CustomerNotExists"), item.CustomerID));
                    continue;
                }
                else
                {
                    item.CustomerSysNo = customer.CustomerSysNo;
                }
                //验证顾客不能存在重复的奖品信息
                var existsGift = _giftDA.Load(item.CustomerSysNo.Value, item.ProductSysNo.Value, CustomerGiftStatus.Origin);
                if (existsGift != null)
                {
                    sb.AppendLine(string.Format(GetString("CustomerAlreadHaveGift"), customer.CustomerID, item.ProductID));
                    continue;
                }
                _giftDA.Insert(item);
                successCount++;
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("AddSuccessPartial", successCount, msg.Count));
                throw new BizException(sb.ToString());
            }
        }

        private string BuildErrorTitle(string errorMsgKey, int successCount, int totalCount)
        {
            return string.Format(GetString(errorMsgKey), successCount, totalCount - successCount) + Environment.NewLine;
        }

        /// <summary>
        ///作废顾客奖品 
        /// </summary>
        public virtual List<int> AbandonGift(List<CustomerGift> msg)
        {
            //只有原始状态的单据可以执行作废操作
            List<int> successSysNoList = new List<int>(msg.Count);
            StringBuilder sb = new StringBuilder();
            foreach (var item in msg)
            {
                var target = _giftDA.Load(item.SysNo.Value);
                if (target == null)
                {
                    sb.AppendFormat(GetString("GiftInfoNotExists"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                if (target.Status != CustomerGiftStatus.Origin)
                {
                    sb.AppendFormat(GetString("GiftStatusNotOrigin"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                //将单据状态置为已作废
                item.Status = CustomerGiftStatus.Voided;
                _giftDA.UpdateStatus(item);
                successSysNoList.Add(item.SysNo.Value);
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("VoidSuccessPartial", successSysNoList.Count, msg.Count));
                throw new BizException(sb.ToString());
            }

            return successSysNoList;
        }

        /// <summary>
        ///取消作废顾客奖品 
        /// </summary>
        public virtual List<int> CancelAbandonGift(List<CustomerGift> msg)
        {
            //只有作废状态的单据可以执行取消作废操作
            List<int> successSysNoList = new List<int>(msg.Count);
            StringBuilder sb = new StringBuilder();
            foreach (var item in msg)
            {
                var target = _giftDA.Load(item.SysNo.Value);
                if (target == null)
                {
                    sb.AppendFormat(GetString("GiftInfoNotExists"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                if (target.Status != CustomerGiftStatus.Voided)
                {
                    sb.AppendFormat(GetString("CanNotCancelVoid"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }

                //将单据状态置为原始的
                item.Status = CustomerGiftStatus.Origin;
                _giftDA.UpdateStatus(item);
                successSysNoList.Add(item.SysNo.Value);
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("CancelVoidSuccessPartial", successSysNoList.Count, msg.Count));
                throw new BizException(sb.ToString());
            }

            return successSysNoList;
        }

        /// <summary>
        /// 发送获奖通知
        /// </summary>
        public virtual void NotifyWinGift(List<CustomerGift> msg)
        {
            //只有状态为原始的单据可以执行发送获取通知
            int successCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in msg)
            {
                var target = _giftDA.Load(item.SysNo.Value);
                if (target == null)
                {
                    sb.AppendFormat(GetString("GiftInfoNotExists"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                if (target.Status != CustomerGiftStatus.Origin)
                {
                    sb.AppendFormat(GetString("CanNotSendWin"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                //TODO
                //验证商品是否存在

                //验证顾客是否存在
                var customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(item.CustomerSysNo.Value);
                if (customer == null)
                {
                    sb.AppendFormat(GetString("CustomerNotExists@SendWin")
                        , item.SysNo.ToString(), customer.CustomerID);
                    sb.AppendLine();
                    continue;
                }
                SendNotifyMail(customer.Email, item.ProductID,customer.CustomerID);
                successCount++;
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("SendWinSuccessPartial", successCount, msg.Count));
                throw new BizException(sb.ToString());
            }
        }

        private void SendNotifyMail(string customerEmail, string productName,string customerId)
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("GiftProductName", productName);
            vars.Add("Year", DateTime.Now.Year);
            if (string.IsNullOrEmpty(customerEmail))
            {
                throw new BizException("顾客帐号：" + customerId + " : 邮件收件人不能为空!");
            }
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(customerEmail, "CustomerGift_Notify", vars, false);
        }

        /// <summary>
        /// 发送获奖过期通知
        /// </summary>
        public virtual void RemindExpiringGift(List<CustomerGift> msg)
        {
            //只有状态为原始的单据可以执行发送即将过期提醒通知
            int successCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in msg)
            {
                var target = _giftDA.Load(item.SysNo.Value);
                if (target == null)
                {
                    sb.AppendFormat(GetString("GiftInfoNotExists"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                if (target.Status != CustomerGiftStatus.Origin)
                {
                    sb.AppendFormat(GetString("CanNotSendAlert"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                //验证顾客是否存在
                var customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(item.CustomerSysNo.Value);
                if (customer == null)
                {
                    sb.AppendFormat(GetString("CustomerNotExists@SendAlert")
                        , item.SysNo.ToString(), customer.CustomerID);
                    sb.AppendLine();
                    continue;
                }
                SendExpiringMail(customer.Email);
                successCount++;
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("SendAlertSuccessPartial", successCount, msg.Count));
                throw new BizException(sb.ToString());
            }
        }

        private void SendExpiringMail(string customerEmail)
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("Year", DateTime.Now.Year);
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(customerEmail, "CustomerGift_Remind", vars, false);
        }

        /// <summary>
        /// 发送获奖作废通知
        /// </summary>
        public virtual void VoidGift(List<CustomerGift> msg)
        {
            //只有状态为作废的单据可以执行发送即将过期提醒通知
            int successCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in msg)
            {
                var target = _giftDA.Load(item.SysNo.Value);
                if (target == null)
                {
                    sb.AppendFormat(GetString("GiftInfoNotExists"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                if (target.Status != CustomerGiftStatus.Voided)
                {
                    sb.AppendFormat(GetString("CanNotSendVoid"), item.SysNo.ToString());
                    sb.AppendLine();
                    continue;
                }
                //验证顾客是否存在
                var customer = ObjectFactory<CustomerProcessor>.Instance.GetCustomerBasicInfoBySysNo(item.CustomerSysNo.Value);
                if (customer == null)
                {
                    sb.AppendFormat(GetString("CustomerNotExists@SendVoid")
                        , item.SysNo.ToString(), customer.CustomerID);
                    sb.AppendLine();
                    continue;
                }
                if (string.IsNullOrEmpty(customer.Email))
                {
                    sb.AppendFormat(GetString("CustomerEmailNotExists@SendVoid")
                        , item.SysNo.ToString(), customer.CustomerID);
                    sb.AppendLine();
                    continue;
                }
                SendVoidMail(customer.Email);
                successCount++;
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, BuildErrorTitle("SendVoidSuccessPartial", successCount, msg.Count));
                throw new BizException(sb.ToString());
            }
        }

        private void SendVoidMail(string customerEmail)
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("Year", DateTime.Now.Year);
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(customerEmail, "CustomerGift_Void", vars, false);
        }
        /// <summary>
        /// 领取奖品
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="soSysNo"></param>
        public virtual void GetGift(int customerSysNo, int productSysNo, int soSysNo)
        {
            CustomerGift entity = _giftDA.Load(customerSysNo, productSysNo, CustomerGiftStatus.Origin);
            if (entity == null)
                throw new BizException(GetString("GiftCantGet"));
            entity.Status = CustomerGiftStatus.Assigned;
            entity.SOSysNo = soSysNo;
            _giftDA.Update(entity);
        }

        public void ReturnGiftForSO(int soSysNo)
        {
            _giftDA.ReturnGiftForSO(soSysNo);
        }
    }
}
