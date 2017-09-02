using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductChannelMemberFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string GetProductChannelMemberInfoUrl = "/IMService/ProductChannelMemberInfo/GetProductChannelMemberInfo";
        const string GetProductChannelMemberPriceInfoUrl = "/IMService/ProductChannelMemberInfo/GetProductChannelMemberPriceInfo";
        const string InsertProductChannelMemberPricesUrl = "/IMService/ProductChannelMemberInfo/InsertProductChannelMemberPrices";
        const string GetProductChannelMemberPriceBySysNoUrl = "/IMService/ProductChannelMemberInfo/GetProductChannelMemberPriceBySysNo";
        const string UpdateProductChannelMemberPriceUrl = "/IMService/ProductChannelMemberInfo/UpdateProductChannelMemberPrice";
        const string UpdateProductChannelMemberPricesUrl = "/IMService/ProductChannelMemberInfo/UpdateProductChannelMemberPrices";
        const string DeleteProductChannelMemberPricesUrl = "/IMService/ProductChannelMemberInfo/DeleteProductChannelMemberPrices";
        const string GetProductChannelMemberPriceLogsUrl = "/IMService/ProductChannelMemberInfo/GetProductChannelMemberPriceLogs";
        #endregion

        #region Method
        // CustomerService服务基地址
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }
        public ProductChannelMemberFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }
        public ProductChannelMemberFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region ProductChannelMemberInfo
        // 获取渠道商品信息
        public void GetProductChannelMemberInfoList(EventHandler<RestClientEventArgs<List<ProductChannelMemberInfo>>> callback)
        {
            _restClient.Query<List<ProductChannelMemberInfo>>(GetProductChannelMemberInfoUrl,null,callback);
        }
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询指定渠道会员信息
        public void GetProductChannelMemberPriceBySysNo(Int32 sysNo
            , EventHandler<RestClientEventArgs<ProductChannelMemberPriceInfo>> callback)
        {
            _restClient.Query<ProductChannelMemberPriceInfo>(GetProductChannelMemberPriceBySysNoUrl, sysNo, callback);
        }
        // 查询渠道会员价格表
        public void GetProductChannelMemberPriceInfo(ProductChannelMemberQueryVM model
            , int PageSize, int PageIndex, string SortField
            , EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            //过滤器转换成服务层的过滤器
            ProductChannelInfoMemberQueryFilter filter = 
                model.ConvertVM<ProductChannelMemberQueryVM, ProductChannelInfoMemberQueryFilter>();
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            _restClient.QueryDynamicData(GetProductChannelMemberPriceInfoUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (!(args.Result == null || args.Result.Rows == null))
                {
                    foreach (var item in args.Result.Rows)
                    {
                        item.IsChecked = false;
                        item.DisplayMemberPricePercent = item.MemberPricePercent != null && item.MemberPricePercent > 0
                           ? String.Format("{0}%", item.MemberPricePercent.ToString().Split('.')[0]) : String.Empty;
                        item.Discount = (item.MemberPrice != null && item.MemberPrice > 0)
                            ? (item.CurrentPrice - item.MemberPrice).ToString("0.00")
                            : (item.CurrentPrice * (1 - item.MemberPricePercent / 100)).ToString("0.00");
                        item.CurrentPrice = item.CurrentPrice.ToString("0.00");
                    }
                }
                callback(obj, args);
            });
        }
        // 插入会员渠道价格
        public void InsertProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> data
            , EventHandler<RestClientEventArgs<ProductChannelMemberPriceInfo>> callback)
        {
            _restClient.Create(InsertProductChannelMemberPricesUrl, data, callback);
        }
        //更新优惠价和优惠比例
        public void UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo data,
            EventHandler<RestClientEventArgs<ProductChannelMemberPriceInfo>> callback)
        {
            _restClient.Update(UpdateProductChannelMemberPriceUrl, data, callback);
        }
        //成批更新优惠价和优惠比例
        public void UpdateProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> data,
            EventHandler<RestClientEventArgs<ProductChannelMemberPriceInfo>> callback)
        {
            _restClient.Update(UpdateProductChannelMemberPricesUrl,data, callback);
        }
        //成批删除外部渠道信息
        public void DeleteProductChannelMemberPrices(List<ProductChannelMemberPriceInfo> data,
            EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.Delete(DeleteProductChannelMemberPricesUrl, data, callback);
        }
        #endregion

        #region ProductChannelMemberPriceLogInfo
        //获取日志查询
        public void GetProductChannelMemberPriceLogs(
            ProductChannelMemberQueryVM model, int PageSize, int PageIndex, string SortField
            , EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductChannelInfoMemberQueryFilter filter =
            model.ConvertVM<ProductChannelMemberQueryVM, ProductChannelInfoMemberQueryFilter>();
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            _restClient.QueryDynamicData(GetProductChannelMemberPriceLogsUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (!(args.Result == null || args.Result.Rows == null))
                {
                    foreach (var item in args.Result.Rows)
                    {
                        item.DisplayMemberPricePercent = item.MemberPricePercent != null && item.MemberPricePercent > 0
                            ? String.Format("{0}%", item.MemberPricePercent.ToString().Split('.')[0]) : String.Empty;
                        item.InDate = item.InDate.ToString("yyyy-MM-dd HH:mm:ss");
                        item.DisplayOperationType = item.OperationType == "A" ? "新增"
                            : item.OperationType == "E" ? "编辑" : "作废";
                    }
                }
                callback(obj, args);
            });
        }
        #endregion
    }
}
