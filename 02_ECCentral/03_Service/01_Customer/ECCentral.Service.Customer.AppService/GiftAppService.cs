using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(GiftAppService))]
    public class GiftAppService
    {
        private GiftProcessor _giftProcessor = ObjectFactory<GiftProcessor>.Instance;
        /// <summary>
        /// 创建顾客奖品信息
        /// </summary>
        public virtual void CreateGift(List<CustomerGift> msg)
        {
            _giftProcessor.CreateGift(msg);
        }

        /// <summary>
        /// 发送获奖通知
        /// </summary>
        public virtual void NotifyGift(List<CustomerGift> msg)
        {
            _giftProcessor.NotifyWinGift(msg);
        }

        /// <summary>
        /// 发送获奖提醒
        /// </summary>
        public virtual void RemindGift(List<CustomerGift> msg)
        {
            _giftProcessor.RemindExpiringGift(msg);
        }

        /// <summary>
        /// 发送获奖作废通知
        /// </summary>
        public virtual void VoidGift(List<CustomerGift> msg)
        {
            _giftProcessor.VoidGift(msg);
        }

        /// <summary>
        /// 作废顾客奖品信息
        /// </summary>
        public virtual List<int> AbandonGift(List<CustomerGift> msg)
        {
            return _giftProcessor.AbandonGift(msg);
        }

        /// <summary>
        /// 取消作废顾客奖品信息
        /// </summary>
        public virtual List<int> CancelAbandonGift(List<CustomerGift> msg)
        {
            return _giftProcessor.CancelAbandonGift(msg);
        }

        /// <summary>
        /// 领取奖品
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="soSysNo"></param>
        public virtual void GetGift(int customerSysNo, int productSysNo, int soSysNo)
        {
            _giftProcessor.GetGift(customerSysNo, productSysNo, soSysNo);
        }
    }
}
