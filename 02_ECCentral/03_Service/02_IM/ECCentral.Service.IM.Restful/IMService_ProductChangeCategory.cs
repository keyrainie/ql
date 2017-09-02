//************************************************************************
// 用户名				泰隆优选
// 系统名				商品批量移类
// 子系统名		        商品批量移类Restful实现
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************

using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 更新

        /// <summary>
        /// 更新商品请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductBatchChangeCategory/BatchChangeCategory", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchChangeCategory(ProductBatchChangeCategoryInfo request)
        {
            ObjectFactory<ProductBatchChangeCategoryAppService>.Instance.BatchChangeCategory(request);          
        }
       
        #endregion
    }
}