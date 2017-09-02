using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Data;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/TopItem/Query", Method = "POST")]
        public virtual QueryResult QueryTopItem(TopItemFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<ITopItemQuery>.Instance.QueryTopItem(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 添加置顶商品
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/TopItem/Create", Method = "POST")]
        public virtual void SetTopItem(TopItemInfo entity)
        {
            ObjectFactory<TopItemAppService>.Instance.SetTopItem(entity);
        }
        /// <summary>
        /// 批量取消商品置顶
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/TopItem/Cancle", Method = "PUT")]
        public virtual void CancleTopItem(List<TopItemInfo> list)
        {
            ObjectFactory<TopItemAppService>.Instance.CancleTopItem(list);
        }
        /// <summary>
        /// 更新置顶商品配置
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/TopItemConfig/Update", Method = "PUT")]
        public virtual void TopItemConfigUpdate(TopItemConfigInfo entity)
        {
            ObjectFactory<TopItemAppService>.Instance.TopItemConfigUpdate(entity);
        }
        /// <summary>
        /// 加载置顶商品配置
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/TopItemConfig/Load", Method = "POST")]
        public TopItemConfigInfo LoadTopItemConfig(TopItemFilter entity)
        {
            return ObjectFactory<TopItemAppService>.Instance.LoadTopItemConfig(entity.PageType.Value, entity.RefSysNo.Value);
        }
    }
}
