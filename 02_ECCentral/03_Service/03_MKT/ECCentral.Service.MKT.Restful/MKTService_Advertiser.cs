using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private AdvertisersAppService advAppService = ObjectFactory<AdvertisersAppService>.Instance;

        /// <summary>
        /// 获取广告商查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/QueryAdvertiser", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryAdvertiser(AdvertiserQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAdvertiserQueryDA>.Instance.QueryAdvertiser(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/AdvInfo/SetAdvertiserValid", Method = "PUT")]
        public virtual void SetAdvertiserValid(List<int> item)
        {
            advAppService.SetAdvertiserValid(item);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/AdvInfo/SetAdvertiserInvalid", Method = "PUT")]
        public virtual void SetAdvertiserInvalid(List<int> item)
        {
            advAppService.SetAdvertiserInvalid(item);
        }

        /// <summary>
        /// 获取广告效果查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/QueryAdvEffect", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryAdvEffect(AdvEffectQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAdvertiserQueryDA>.Instance.QueryAdvEffect(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取广告效果所涉及的总价钱
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/AdvInfo/QueryAdvEffectToltalPrice", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        //[DataTableSerializeOperationBehavior]
        //public virtual QueryResult QueryAdvEffectToltalPrice(AdvEffectQueryFilter filter)
        //{
        //    int totalCount;
        //    var dataTable = ObjectFactory<IAdvertiserQueryDA>.Instance.QueryAdvEffect(filter, out totalCount);
        //    return new QueryResult()
        //    {
        //        Data = dataTable,
        //        TotalCount = totalCount
        //    };
        //}

        /// <summary>
        /// 获取广告效果BBS查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/QueryAdvEffectBBS", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryAdvEffectBBS(AdvEffectBBSQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAdvertiserQueryDA>.Instance.QueryAdvEffectBBS(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/AdvInfo/LoadAdvertiser", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual Advertisers LoadAdvertiser(int sysNo)
        {
            return advAppService.LoadAdvertiser(sysNo);
        }

        /// <summary>
        /// 更新广告商
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/UpdateAdvertisers", Method = "PUT")]
        public virtual void UpdateAdvertisers(Advertisers item)
        {
            advAppService.UpdateAdvertisers(item);
        }

        /// <summary>
        /// 添加广告商
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/CreateAdvertisers", Method = "POST")]
        public virtual void CreateAdvertisers(Advertisers item)
        {
            advAppService.CreateAdvertisers(item);
        }

        /// <summary>
        /// 获取订阅维护查询列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/QuerySubscription", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySubscription(SubscriptionQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAdvertiserQueryDA>.Instance.QuerySubscription(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取订阅分类列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdvInfo/QuerySubscriptionCategory", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<SubscriptionCategory> QuerySubscriptionCategory()
        {
            return ObjectFactory<IAdvertiserQueryDA>.Instance.QuerySubscriptionCategory();
        }
    }
}
