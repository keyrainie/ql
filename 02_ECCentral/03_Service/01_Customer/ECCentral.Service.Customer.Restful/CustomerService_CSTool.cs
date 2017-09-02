using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.ServiceModel.Web;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.AppService;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        #region OrderCheckMaster
        #region Action
        /// <summary>
        /// 创建OrderCheckMaster信息
        /// </summary>
        [WebInvoke(UriTemplate = "/OrderCheckMaster/Create", Method = "POST")]
        public OrderCheckMaster CreateOrderCheckMaster(OrderCheckMaster msg)
        {
            return ObjectFactory<CSToolAppService>.Instance.CreateOrderCheckMaster(msg);
        }

        /// <summary>
        /// 批量更新OrderCheckMaster状态
        /// </summary>
        /// <param name="request"></param>
        [WebInvoke(UriTemplate = "/OrderCheckMaster/Update", Method = "PUT")]
        public void BatchUpdateOrderCheckMasterStatus(OrderCheckMasterReq request)
        {
            ObjectFactory<CSToolAppService>.Instance.BatchUpdateOrderCheckMasterStatus(request.orderCheckMaster, request.SysNoList);
        }

        #endregion

        #region Query
        /// <summary>
        /// 查询OrderCheckMaster
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/OrderCheckMaster/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOrderCheckMasterList(OrderCheckMasterQueryFilter request)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IOrderCheckMasterQueryDA>.Instance.Query(request, out totalCount),
                TotalCount = totalCount
            };
        }
        #endregion

        #endregion

        #region OrderCheckItem

        #region Action
        /// <summary>
        /// 创建OrderCheckItem信息
        /// </summary>
        [WebInvoke(UriTemplate = "/OrderCheckItem/Create", Method = "POST")]
        public OrderCheckItem CreateOrderCheckItem(OrderCheckItem msg)
        {
            return ObjectFactory<CSToolAppService>.Instance.CreateOrderCheckItem(msg);
        }

        /// <summary>
        /// 批量创建OrderCheckItem信息
        /// </summary>
        [WebInvoke(UriTemplate = "/OrderCheckItem/BatchCreate", Method = "POST")]
        public void BatchCreateOrderCheckItem(BatchCreatOrderCheckItemReq request)
        {
            ObjectFactory<CSToolAppService>.Instance.BatchCreateOrderCheckItem(request.orderCheckItemList, request.ReferenceType);
        }
        /// <summary>
        /// 更新OrderCheckItem
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/OrderCheckItem/Update", Method = "PUT")]
        public void UpdateOrderCheckItem(OrderCheckItem msg)
        {
            ObjectFactory<CSToolAppService>.Instance.UpdateOrderCheckItem(msg);
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询OrderCheckItem
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/OrderCheckItem/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOrderCheckItemList(OrderCheckItemQueryFilter request)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IOrderCheckItemQueryDA>.Instance.Query(request, out totalCount),
                TotalCount = totalCount
            };
        }
        #endregion
        #endregion
 

       
 
    }

}
