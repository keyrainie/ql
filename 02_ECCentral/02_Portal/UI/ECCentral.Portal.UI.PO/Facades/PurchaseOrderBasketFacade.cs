using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class PurchaseOrderBasketFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public PurchaseOrderBasketFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询采购篮列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryBasketList(PurchaseOrderBasketQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Basket/QueryPurchaseOrderBasketList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 批量创建PO单
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void BatchCreatePO(List<BasketItemsInfo> list, EventHandler<RestClientEventArgs<BatchCreateBasketResultInfo>> callback)
        {
            string relativeUrl = "/POService/Basket/BatchCreatePurchaseOrder";
            list.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            });
            restClient.Update<BatchCreateBasketResultInfo>(relativeUrl, list, callback);
        }

        /// <summary>
        /// 批量添加赠品
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void BatchAddGiftForBasket(List<BasketItemsInfo> list, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/Basket/BatchAddGift";
            list.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            });
            restClient.Update<string>(relativeUrl, list, callback);
        }

        /// <summary>
        /// 批量更新采购篮商品
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void BatchUpdateBasketItems(List<BasketItemsInfo> list, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/Basket/BatchUpdateBasketItems";
            list.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            });
            restClient.Update<string>(relativeUrl, list, callback);
        }

        /// <summary>
        /// 批量删除采购篮商品
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void BatchDeleteBasketItems(List<BasketItemsInfo> list, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/Basket/BatchDeleteBasketItems";
            list.ForEach(x =>
            {
                x.CompanyCode = CPApplication.Current.CompanyCode;
                x.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            });
            restClient.Update<string>(relativeUrl, list, callback);
        }

        /// <summary>
        /// 解析上传的采购篮模板，并转换为List:
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void ConvertBasketTemplateFileToEntityList(string fileIdentity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/Basket/ConvertBasketTemplateFileToEntityList";
            restClient.QueryDynamicData(relativeUrl, fileIdentity, callback);
        }

        public void QueryBasketTargetWarehouseList(EventHandler<RestClientEventArgs<List<WarehouseInfo>>> callback)
        {
            string relativeUrl = "/POService/PurchaseOrder/GetPurchaseOrderWarehouseList";
            restClient.Query<List<WarehouseInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        public void GetGiftBasketItems(List<int> productSysNoList, EventHandler<RestClientEventArgs<List<BasketItemsInfo>>> callback)
        {
            string relativeUrl = "/POService/Basket/GetGiftBasketItems";
            restClient.Query<List<BasketItemsInfo>>(relativeUrl, productSysNoList, callback);
        }
    }
}
