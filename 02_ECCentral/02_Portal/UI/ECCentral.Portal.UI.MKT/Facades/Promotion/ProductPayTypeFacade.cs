using System;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT.Promotion;

namespace ECCentral.Portal.UI.MKT.Facades.Promotion
{
    public class ProductPayTypeFacade
    {
        private const string QueryProductPayTypeRelativeUrl = "/MKTService/ProductPayType/QueryProductPayType";
        private const string QueryPayTypeListRelativeUrl = "/MKTService/ProductPayType/QueryPayTypeList";
        private const string BatchCreatePayTypeRelativeUrl = "/MKTService/ProductPayType/BatchCreateProductPayType";
        private const string BatchAbortPayTypeRelativeUrl = "/MKTService/ProductPayType/BathAbortProductPayType";
        private readonly RestClient _restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public ProductPayTypeFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductPayTypeFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询商品支付方式
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryProductPayType(ProductPayTypeQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            _restClient.QueryDynamicData(QueryProductPayTypeRelativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询支付方式列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetProductPayTypeList(EventHandler<RestClientEventArgs<List<PayTypeInfo>>> callback)
        {
            _restClient.Query(QueryPayTypeListRelativeUrl, null, callback);
        }

        /// <summary>
        /// 批量创建支付方式
        /// </summary>
        /// <param name="vm"> </param>
        /// <param name="callback"></param>
        public void BatchCreateProductPayType(ProductPayTypeVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var entity = vm.ConvertVM<ProductPayTypeVM, ProductPayTypeInfo>();
            _restClient.Create(BatchCreatePayTypeRelativeUrl, entity, callback);
        }

        /// <summary>
        /// 批量中止支付方式
        /// </summary>
        /// <param name="payTypeIds"></param>
        /// <param name="editUser"> </param>
        /// <param name="callback"></param>
        public void BathAbortProductPayType(string payTypeIds, string editUser, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var productPayTypeInfo = new ProductPayTypeInfo {EditUser = editUser, ProductPayTypeIds = payTypeIds};
            _restClient.Update(BatchAbortPayTypeRelativeUrl, productPayTypeInfo, callback);
        }
    }
}
