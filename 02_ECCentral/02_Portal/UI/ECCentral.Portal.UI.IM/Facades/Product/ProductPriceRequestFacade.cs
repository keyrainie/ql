using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Models.Product;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductPriceRequestFacade
    {
        #region 字段以及构造函数
        private readonly IPage _viewPage;
        private readonly RestClient _restClient;
        const string UPdateRelativeUrl = "/IMService/Product/AuditProductPriceRequest";
        const string GetRelativeUrl = "/IMService/Product/GetProductPriceRequestInfoBySysNo";
        const string GetNeweggRelativeUrl = "/IMService/Product/GetNeweggProductPriceRequestInfoBySysNo";
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public ProductPriceRequestFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductPriceRequestFacade(IPage page)
        {
            _viewPage = page;
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public virtual void AuditProductPriceRequest(ProductPriceRequestInfo entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var source = new List<ProductPriceRequestInfo> { entity };
            _restClient.Update(UPdateRelativeUrl, source, callback);
        }


        /// <summary>
        /// 获取价格审核
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetProductPriceRequestBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductPriceRequestInfo>> callback)
        {
            _restClient.Query(GetRelativeUrl, sysNo, callback);
        }


        /// <summary>
        /// 获取价格审核
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetNeweggProductPriceRequestBySysNo(int sysNo, EventHandler<RestClientEventArgs<ProductPriceRequestMsg>> callback)
        {
            _restClient.Query(GetNeweggRelativeUrl, sysNo, callback);
        }

        #region 阶梯价格
        /// <summary>
        /// 创建阶梯价格
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void CreateProductStepPrice(ProductStepPriceInfoVM entity, EventHandler<RestClientEventArgs<int>> callback)
        {
            string url = "/IMService/Product/CreateProductStepPrice";
            _restClient.Query(url, entity, callback);
        }

        public void DeleteProductStepPrice(List<int> sysNos, EventHandler<RestClientEventArgs<int>> callback)
        {
            string url = "/IMService/Product/DeleteProductStepPrice";
            _restClient.Query(url, sysNos, callback);
        }

        public void GetProductStepPrice(ProductStepPriceQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string url = "/IMService/Product/GetProductStepPrice";
            _restClient.QueryDynamicData(url, filter, callback);// (obj, args) =>
            //{
            //    if (args.FaultsHandle())
            //    {
            //        return;
            //    }
            //    callback(this, new RestClientEventArgs<dynamic>(args.Result, this._viewPage));
            //});
        }

        public void GetProductStepPricebyProductSysNo(int ProductSysNo, EventHandler<RestClientEventArgs<List<ProductStepPriceInfoVM>>> callback)
        {
            string url = "/IMService/Product/GetProductStepPricebyProductSysNo";
            _restClient.Query<List<ProductStepPriceInfoVM>>(url, ProductSysNo, callback);
        }
        #endregion
    }
}
