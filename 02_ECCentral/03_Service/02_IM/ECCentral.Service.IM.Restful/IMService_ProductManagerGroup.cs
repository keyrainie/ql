//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员管理
// 子系统名		        商品管理员管理Restful实现
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.ProductManager;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 公共方法

        /// <summary>
        /// 查询PM组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/QueryProductManagerGroupInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductManagerGroupInfo(ProductManagerGroupQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductManagerGroup", "ProductManagerGroupCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductManagerGroupQueryDA>.Instance.QueryProductManagerGroupInfo(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 获取所有PM信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/LoadPMList/{sysNo}", Method = "GET")]
        //[DataTableSerializeOperationBehavior]
        public List<ProductManagerInfo> QueryAllProductManagerInfoByPMGroupSysNo(string sysno)
        {
            int pmGroupSysNo = int.Parse(sysno);
            var source = ObjectFactory<IProductManagerGroupQueryDA>.Instance.QueryAllProductManagerInfoByPMGroupSysNo(pmGroupSysNo);
            return source;
        }

        /// <summary>
        /// 查询不在其他PM组的PM集合
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/QueryOtherProductManagerGroupInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<KeyValuePair<string, string>> QueryOtherProductManagerGroupInfo(int productManagerGroupInfoSysNo)
        {
            var source = ObjectFactory<IProductManagerGroupQueryDA>.Instance.QueryAllProductManagerInfo(productManagerGroupInfoSysNo);
            return source;
        }

        /// <summary>
        /// 根据SysNO获取PM组信息
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/LoadPMGroup/{sysNo}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public ProductManagerGroupInfo GetProductManagerGroupInfoBySysNo(string sysNo)
        {
            int productManagerGroupInfoSysNo = int.Parse(sysNo);
            var entity = ObjectFactory<ProductManagerGroupAppService>.Instance.GetProductManagerGroupInfoBySysNo(productManagerGroupInfoSysNo);
            return entity;
        }

        /// <summary>
        /// 创建PM组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/CreateProductManagerGroupInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductManagerGroupInfo CreateProductManagerGroupInfo(ProductManagerGroupInfo request)
        {
            var entity = ObjectFactory<ProductManagerGroupAppService>.Instance.CreateProductManagerGroupInfo(request);
            return entity;
        }

        /// <summary>
        /// 修改PM组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/UpdateProductManagerGroupInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ProductManagerGroupInfo UpdateProductManagerGroupInfo(ProductManagerGroupInfo request)
        {
            var entity = ObjectFactory<ProductManagerGroupAppService>.Instance.UpdateProductManagerGroupInfo(request);
            return entity;
        }

        /// <summary>
        /// 得到所有PM
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductManagerGroup/QueryAllProductManagerInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<ProductManagerInfo> QueryAllProductManagerInfo()
        {
          return   ObjectFactory<IProductManagerGroupQueryDA>.Instance.QueryAllProductManagerInfo();
        }
        #endregion
    }
}
