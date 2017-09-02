//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品信息管理
// 子系统名		        渠道商品信息管理Restful实现
// 作成者				John
// 改版日				2012.11.6
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
using ECCentral.Service.IM.IDataAccess;
using System;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region ProductChannelMemberInfo
        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductChannelMemberInfo/GetProductChannelMemberInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<ProductChannelMemberInfo> GetProductChannelMemberInfo()
        {
            return ObjectFactory<ProductChannelMemberInfoAppService>.Instance.GetChannelMemberInfoList();
        }
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询全部外部渠道会员价格
        [WebInvoke(UriTemplate = "/ProductChannelMemberInfo/GetProductChannelMemberPriceBySysNo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public ProductChannelMemberPriceInfo GetProductChannelMemberPriceBySysNo(Int32 SysNo)
        {
          return  ObjectFactory<ProductChannelMemberInfoAppService>.Instance
              .GetProductChannelMemberPriceBySysNo(SysNo);
        }

        //获取所有如何要求的渠道会员价信息
        [WebInvoke(UriTemplate = "/ProductChannelMemberInfo/GetProductChannelMemberPriceInfo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductChannelMemberPriceInfoUrl(ProductChannelInfoMemberQueryFilter request)
        {
            if (request == null)
            {
                //利用资源文件显示错误信息
                throw new BizException(ResouceManager.GetMessageString(
                    "IM.ProductChannelInfo", "ProductChannelInfoCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductChannelMemberInfoDA>.Instance
                .GetProductChannelMemberPriceInfoUrl(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        // 创建渠道商品信息
        [WebInvoke(UriTemplate = "ProductChannelMemberInfo/InsertProductChannelMemberPrices", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void InsertProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> productChannelMemberPrice)
        {
            ObjectFactory<ProductChannelMemberInfoAppService>.Instance
                .InsertProductChannelMemberPrices(productChannelMemberPrice);
        }


        //更新优惠价和优惠比例
        [WebInvoke(UriTemplate = "ProductChannelMemberInfo/UpdateProductChannelMemberPrice", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo ProductChannelMemberPrice)
        {
            ObjectFactory<ProductChannelMemberInfoAppService>.Instance
                .UpdateProductChannelMemberPrice(ProductChannelMemberPrice);
        }
        //成批更新优惠价和优惠比例
        [WebInvoke(UriTemplate = "ProductChannelMemberInfo/UpdateProductChannelMemberPrices", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> ProductChannelMemberPrice)
        {
            ObjectFactory<ProductChannelMemberInfoAppService>.Instance
                .UpdateProductChannelMemberPrices(ProductChannelMemberPrice);
        }

        //成批删除外部会员渠道信息
        [WebInvoke(UriTemplate = "ProductChannelMemberInfo/DeleteProductChannelMemberPrices", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> ProductChannelMemberPrice)
        {
            ObjectFactory<ProductChannelMemberInfoAppService>.Instance
              .DeleteProductChannelMemberPrices(ProductChannelMemberPrice);
        }
        #endregion

        #region ProductChannelMemberPriceLogInfo
        //成批删除外部会员渠道信息
        [WebInvoke(UriTemplate = "ProductChannelMemberInfo/GetProductChannelMemberPriceLogs", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetProductChannelMemberPriceLogs(ProductChannelInfoMemberQueryFilter request)
        {
            if (request == null)
                //利用资源文件显示错误信息
                throw new BizException(ResouceManager.GetMessageString(
                    "IM.ProductChannelInfo", "ProductChannelInfoCondtionIsNull"));
            request.ChannelName =
                request.ChannelSysNo > 0 ?
                ObjectFactory<ProductChannelMemberInfoAppService>
                    .Instance.ChannelMemberInfoByChannelNo[request.ChannelSysNo]
                    : String.Empty;
            int totalCount;
            var data = ObjectFactory<IProductChannelMemberInfoDA>.Instance
                .GetProductChannelMemberPriceLogs(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }
        #endregion

    }
}