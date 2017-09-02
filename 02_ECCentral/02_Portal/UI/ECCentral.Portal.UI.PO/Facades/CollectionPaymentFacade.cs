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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;
using ECCentral.Service.PO.Restful.ResponseMsg;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.PO.Facades
{
    /// <summary>
    /// 代收代付
    /// </summary>
    public class CollectionPaymentFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public CollectionPaymentFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询代收代付结算单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryConsignSettlements(CollectionPaymentFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/CollectionPayment/QueryCollectionPaymentList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 加载代销结算单详细信息
        /// </summary>
        /// <param name="consignSettleSysNo"></param>
        /// <param name="callback"></param>
        public void GetConsignSettlementInfo(string consignSettleSysNo, EventHandler<RestClientEventArgs<CollectionPaymentInfo>> callback)
        {
            string relativeUrl = string.Format("POService/CollectionPayment/Load/{0}", consignSettleSysNo);
            restClient.Query<CollectionPaymentInfo>(relativeUrl, callback);
        }

        //public void ExportExcelFroConsignSettlementList(CollectionPaymentFilter queryFilter, ColumnSet[] columns)
        //{
        //    queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
        //    string relativeUrl = "POService/ConsignSettlement/QueryConsignSettlementList";
        //    restClient.ExportFile(relativeUrl, queryFilter, columns);
        //}

        
       
       

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void Update(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/Update";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void Abandon(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/Abandon";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelAbandon(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/CancelAbandon";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void Settle(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/Settle";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消结算 
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelSettle(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/CancelSettled";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 审核代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void Audit(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/CollectionPayment/Audit";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消审核代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelAudited(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/CollectionPayment/CancelAudited";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        ///查询代销结算单商品(新建)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetConsignSettlmentProductList(ConsignSettlementProductsQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/GetConsignSettlmentProductList";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 创建代销结算单
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void CreateConsignSettlement(CollectionPaymentInfo consignInfo, EventHandler<RestClientEventArgs<CollectionPaymentInfo>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/CollectionPayment/Create";
            restClient.Create<CollectionPaymentInfo>(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 根据不同权限获取PMList:
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void GetPMSysNoListByType(ConsignSettlementBizInfo consignInfo,EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            string relativeUrl = "POService/ConsignSettlement/GetPMSysNoListByType";
            restClient.Create<List<int>>(relativeUrl,consignInfo, callback);
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        public void GetProductLineSysNoByProductList(int[] productSysNo, EventHandler<RestClientEventArgs<List<ProductPMLine>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetProductLineSysNoByProductList";
            restClient.Query<List<ProductPMLine>>(relativeUrl, productSysNo, callback);
        }
        /// <summary>
        /// 根据PM，获取其全部产品线
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        public void GetProductLineInfoByPM(int pmSysNo, EventHandler<RestClientEventArgs<List<ProductPMLine>>> callback)
        {
            string relativeUrl = "POService/PurchaseOrder/GetProductLineInfoByPM";
            restClient.Query<List<ProductPMLine>>(relativeUrl, pmSysNo, callback);
        }
        
    }
}
