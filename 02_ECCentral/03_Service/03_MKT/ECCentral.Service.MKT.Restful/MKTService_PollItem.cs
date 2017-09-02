using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private PollItemAppService pollItemAppService = ObjectFactory<PollItemAppService>.Instance;

        #region  投票

        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/LoadPollMaster", Method = "POST")]
        public PollMaster LoadPollMaster(int itemID)
        {
            return pollItemAppService.LoadPollMaster(itemID);
        }

        /// <summary>
        /// 查询投票列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PollInfo/QueryPollList", Method = "POST")]//, ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryPollList(PollQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IPollItemQueryDA>.Instance.QueryPollList(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建投票，作为聚合根，只创建活动的基本信息，得到活动ID
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/CreatePollMaster", Method = "POST")]
        public int CreatePollMaster(PollMaster item)
        {
            return pollItemAppService.CreatePollMaster(item);
        }

        /// <summary>
        /// 编辑投票基本信息
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/UpdatePollMaster", Method = "PUT")]
        public void UpdatePollMaster(PollMaster item)
        {
            pollItemAppService.UpdatePollMaster(item);
        }

        /// <summary>
        /// 加载投票所有信息，本行为中会调用聚合根下所有聚合模型的加载行为        ??
        /// </summary>
        /// <param name="item"></param>
        //[WebInvoke(UriTemplate = "/PollItem/LoadPollInfo", Method = "POST")]
        //public PollItem LoadPollInfo(int itemID)
        //{
        //    return pollItemAppService.LoadPollInfo(itemID);
        //}

        #endregion

        #region 投票--问题组（PollItemGroup）
        /// <summary>
        /// 创建投票问题组，只创建问题组的基本信息
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/CreatePollItemGroup", Method = "POST")]
        public void CreatePollItemGroup(PollItemGroup item)
        {
            pollItemAppService.CreatePollItemGroup(item);
        }

        /// <summary>
        /// 编辑投票问题组基本信息
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/UpdatePollItemGroup", Method = "PUT")]
        public void UpdatePollItemGroup(PollItemGroup item)
        {
            pollItemAppService.UpdatePollItemGroup(item);
        }

        /// <summary>
        /// 删除投票问题组
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/PollInfo/DeletePollItemGroup", Method = "PUT")]
        public void DeletePollItemGroup(int sysNo)
        {
            pollItemAppService.DeletePollItemGroup(sysNo);
        }


        /// <summary>
        /// 加载投票问题组所有问题
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/PollInfo/GetPollItemGroupList", Method = "POST")]
        public List<PollItemGroup> GetPollItemGroupList(int sysNo)
        {
            return pollItemAppService.GetPollItemGroupList(sysNo);
        }


        /// <summary>
        /// 加载投票基本信息
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/PollItem/LoadPollItemGroup", Method = "POST")]
        //public PollItemGroup LoadPollItemGroup(int itemID)
        //{
        //    return pollItemAppService.LoadPollItemGroup(itemID);
        //}

        #endregion

        #region  投票问题组--选项（PollItem）
        /// <summary>
        /// 创建投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/CreatePollItem", Method = "POST")]
        public void CreatePollItem(PollItem item)
        {
            pollItemAppService.CreatePollItem(item);
        }


        /// <summary>
        /// 编辑投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/UpdatePollItem", Method = "PUT")]
        public void UpdatePollItem(PollItem item)
        {
            pollItemAppService.UpdatePollItem(item);
        }


        /// <summary>
        /// 删除投票问题组选项
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/PollInfo/DeletePollItem", Method = "PUT")]
        public void DeletePollItem(int itemID)
        {
            pollItemAppService.DeletePollItem(itemID);
        }


        /// <summary>
        /// 加载投票问题组选项
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PollInfo/GetPollItemList", Method = "POST")]
        public List<PollItem> GetPollItemList(int itemID)
        {
            return pollItemAppService.GetPollItemList(itemID);
        }

        /// <summary>
        /// 加载投票答案
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PollInfo/GetPollAnswer", Method = "POST")]
        public List<PollItemAnswer> GetPollAnswer(int itemID)
        {
            return pollItemAppService.GetPollAnswer(itemID);
        }
        #endregion
    }
}
