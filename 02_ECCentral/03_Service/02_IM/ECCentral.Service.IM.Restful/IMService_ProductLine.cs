//************************************************************************
// 用户名				泰隆优选
// 系统名				商品Domain管理
// 子系统名		        商品Domain管理Restful实现
// 作成者				Hax
// 改版日				2012.6.26
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.Restful.RequestMsg;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        [WebInvoke(UriTemplate = "/ProductLine/Load", Method = "POST")]
        public virtual ProductLineInfo LoadProductLine(int sysno)
        {
            return ObjectFactory<ProductLineAppService>.Instance.LoadBySysNo(sysno);
        }

        [WebInvoke(UriTemplate = "/ProductLine/Query", Method = "POST")]
        public QueryResult QueryProductLine(ProductLineFilter request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            int totalCount;
            var data = ObjectFactory<IProductLineQueryDA>.Instance.QueryProductLineList(request, out totalCount);

            return new QueryResult { Data = data, TotalCount = totalCount };
        }

        [WebInvoke(UriTemplate = "/ProductLine/Create", Method = "POST")]
        public virtual ProductLineInfo CreateProductLine(ProductLineInfo entity)
        {
            return ObjectFactory<ProductLineAppService>.Instance.Create(entity);
        }
        [WebInvoke(UriTemplate = "/ProductLine/Update", Method = "PUT")]
        public virtual ProductLineInfo UpdateProductLine(ProductLineInfo entity)
        {
            return ObjectFactory<ProductLineAppService>.Instance.Update(entity);
        }

        [WebInvoke(UriTemplate = "/ProductLine/Delete", Method = "DELETE")]
        public virtual void DeleteProductLine(int sysno)
        {
            ObjectFactory<ProductLineAppService>.Instance.Delete(sysno);
        }

        [WebInvoke(UriTemplate = "/ProductLine/HasRightByPMUser", Method = "POST")]
        public virtual bool HasRightByPMUser(ProductLineInfo entity) 
        {
            return ObjectFactory<ProductLineAppService>.Instance.HasRightByPMUser(entity);
        }

        [WebInvoke(UriTemplate = "/ProductLine/BatchUpdate", Method = "PUT")]
        public virtual void BatchUpdate(BatchUpdatePMEntity entity)
        {
            ObjectFactory<ProductLineAppService>.Instance.BatchUpdate(entity);
        }
    }
}
