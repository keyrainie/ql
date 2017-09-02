using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        private GiftAppService _giftAppService = ObjectFactory<GiftAppService>.Instance;
        /// <summary>
        /// 创建顾客奖品信息
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/Create", Method = "POST")]
        public void CreateGift(List<CustomerGift> msg)
        {
            _giftAppService.CreateGift(msg);
        }

        /// <summary>
        /// 发送获奖通知
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/Notify", Method = "PUT")]
        public void NotifyGift(List<CustomerGift> msg)
        {
            _giftAppService.NotifyGift(msg);
        }

        /// <summary>
        /// 发送获奖提醒
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/Remind", Method = "PUT")]
        public void RemindGift(List<CustomerGift> msg)
        {
            _giftAppService.RemindGift(msg);
        }

        /// <summary>
        /// 发送获奖作废通知
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/Void", Method = "PUT")]
        public void VoidGift(List<CustomerGift> msg)
        {
            _giftAppService.VoidGift(msg);
        }

        /// <summary>
        /// 作废顾客奖品信息
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/Abandon", Method = "PUT")]
        public virtual List<int> AbandonGift(List<CustomerGift> msg)
        {
            return _giftAppService.AbandonGift(msg);
        }

        /// <summary>
        /// 取消作废顾客奖品信息
        /// </summary>
        [WebInvoke(UriTemplate = "/Gift/CancelAbandon", Method = "PUT")]
        public virtual List<int> CancelAbandonGift(List<CustomerGift> msg)
        {
            return _giftAppService.CancelAbandonGift(msg);
        }

        [WebInvoke(UriTemplate = "/Gift/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryGift(CustomerGiftQueryFilter msg)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
    }
}
