//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品信息管理
// 子系统名		        渠道商品信息管理Restful实现
// 作成者				Kevin
// 改版日				2012.6.4
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
        #region 渠道商品信息

        /// <summary>
        /// 查询渠道商品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/QueryProductChannelInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductChannelInfo(ProductChannelInfoQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelInfoCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductChannelInfoQueryDA>.Instance.QueryProductChannelInfo(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取渠道商品信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/GetProductChannelInfoBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelInfo GetProductChannelInfoBySysNo(int sysNo)
        {
            var entity = ObjectFactory<ProductChannelInfoAppService>.Instance.GetProductChannelInfoBySysNo(sysNo);
            return entity;
        }


        /// <summary>
        /// 创建渠道商品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/CreateProductChannelInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreatetProductChannelInfo(List<ProductChannelInfo> request)
        {
            ObjectFactory<ProductChannelInfoAppService>.Instance.CreatetProductChannelInfo(request);            
        }

        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/UpdateProductChannelInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelInfo UpdateProductChannelInfo(ProductChannelInfo request)
        {
            var entity = ObjectFactory<ProductChannelInfoAppService>.Instance.UpdateProductChannelInfo(request);
            return entity;
        }

        /// <summary>
        /// 批量更新多渠道商品记录状态
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/BatchUpdateChannelProductInfoStatus", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchUpdateChannelProductInfoStatus(ProductChannelInfo entity)
        {
            ObjectFactory<ProductChannelInfoAppService>.Instance.BatchUpdateChannelProductInfoStatus(entity);
        }


        /// <summary>
        /// 批量修改渠道商品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/BatchUpdateProductChannelInfo", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchUpdateProductChannelInfo(List<ProductChannelInfo> request)
        {
            ObjectFactory<ProductChannelInfoAppService>.Instance.BatchUpdateProductChannelInfo(request);

        }

        #endregion

        #region 获取渠道信息
        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/GetChannelInfoList", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public List<ChannelInfo> GetChannelInfoList()
        {
            return ObjectFactory<ProductChannelInfoAppService>.Instance.GetChannelInfoList();;
        }

        #endregion

        #region 渠道商品价格信息

        /// <summary>
        /// 查询渠道商品价格信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/QueryProductChannelPeriodPrice", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductChannelPeriodPrice(ProductChannelPeriodPriceQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelInfoCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductChannelInfoQueryDA>.Instance.QueryProductChannelPeriodPrice(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取渠道商品价格信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/GetProductChannelPeriodPriceBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelPeriodPrice GetProductChannelPeriodPriceBySysNo(int sysNo)
        {
            var entity = ObjectFactory<ProductChannelInfoAppService>.Instance.GetProductChannelPeriodPriceBySysNo(sysNo);
            return entity;
        }


        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/CreateProductChannelPeriodPrice", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelPeriodPrice CreatetProductChannelPeriodPrice(ProductChannelPeriodPrice request)
        {
            var entity = ObjectFactory<ProductChannelInfoAppService>.Instance.CreatetProductChannelPeriodPrice(request);
            return entity;
        }

        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelInfo/UpdateProductChannelPeriodPrice", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelPeriodPrice UpdateProductChannelPeriodPrice(ProductChannelPeriodPrice request)
        {
            var entity = ObjectFactory<ProductChannelInfoAppService>.Instance.UpdateProductChannelPeriodPrice(request);
            return entity;
        }
        #endregion
    }
}