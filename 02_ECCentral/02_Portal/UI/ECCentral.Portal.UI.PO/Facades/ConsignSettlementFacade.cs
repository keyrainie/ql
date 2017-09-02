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
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.BizEntity.PO.Settlement;


namespace ECCentral.Portal.UI.PO.Facades
{

    public class ConsignSettlementFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public ConsignSettlementFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询代销结算单列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryConsignSettlements(ConsignSettleQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/QueryConsignSettlementList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelFroConsignSettlementList(ConsignSettleQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/QueryConsignSettlementList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// TODO：查询返点列表(调用 EIMS接口)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void QueryConsignSettlementEIMSList(ConsignSettlementEIMSQueryRsq request, EventHandler<RestClientEventArgs<CosignSettlementEIMSQueryrRsp>> callback)
        {
            string relativeUrl = "POService/ConsignSettlement/QueryConsignSettlmentEIMSList";
            restClient.Query<CosignSettlementEIMSQueryrRsp>(relativeUrl, request, callback);
        }

        /// <summary>
        /// 加载代销结算单详细信息
        /// </summary>
        /// <param name="consignSettleSysNo"></param>
        /// <param name="callback"></param>
        public void GetConsignSettlementInfo(string consignSettleSysNo, EventHandler<RestClientEventArgs<ConsignSettlementInfo>> callback)
        {
            string relativeUrl = string.Format("POService/ConsignSettlement/LoadConsignSettlement/{0}", consignSettleSysNo);
            restClient.Query<ConsignSettlementInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 更新代销结算单信息
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void UpdateConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/UpdateConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 作废代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void AbandonConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/AbandonConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消作废 - 代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelAbandonConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/CancelAbandonConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 结算代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void SettleConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/SettleConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消结算 - 代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelSettleConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/CancelSettleConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 审核代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void AuditConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/AuditConsignSettlement";
            restClient.Update(relativeUrl, consignInfo, callback);
        }

        /// <summary>
        /// 取消审核代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <param name="callback"></param>
        public void CancelAuditConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/CancelAuditConsignSettlement";
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
        public void CreateConsignSettlement(ConsignSettlementInfo consignInfo, EventHandler<RestClientEventArgs<ConsignSettlementInfo>> callback)
        {
            consignInfo.CompanyCode = CPApplication.Current.CompanyCode;
            consignInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/CreateConsignSettlement";
            restClient.Create<ConsignSettlementInfo>(relativeUrl, consignInfo, callback);
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

        #region 经销商品结算单

        /// <summary>
        /// 经销商品结算单
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QuerySettleAccountWithOrigin(SettleOrderCreateQueryVM queryVM, List<int> OrderSysNoList, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SettleOrderCreateQueryFilter queryFilter = queryVM.ConvertVM<SettleOrderCreateQueryVM, SettleOrderCreateQueryFilter>();
            queryFilter.OrderSysNoList = OrderSysNoList;
            queryFilter.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = PageIndex, PageSize = PageSize, SortBy = SortField };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/QuerySettleAccountWithOrigin";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }


        /// <summary>
        ///  创建经销商品结算单
        /// </summary>
        /// <param name="settleInfo"></param>
        /// <param name="callback"></param>
        public void CreateSettleAccount(SettleInfo settleInfo, EventHandler<RestClientEventArgs<SettleInfo>> callback)
        {
            settleInfo.CreateUserSysNo = RestClient.UserSysNo;
            settleInfo.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "POService/ConsignSettlement/CreateSettleAccountBil";
            restClient.Create(relativeUrl, settleInfo, callback);
        }

        /// <summary>
        /// 查询经销商品详细信息(基本信息和个子项税率信息)
        /// </summary>
        /// <param name="settleInfo"></param>
        /// <param name="callback"></param>
        public void GetSettleAccount(SettleInfo settleInfo, EventHandler<RestClientEventArgs<SettleInfo>> callback)
        {
            string relativeUrl = "POService/ConsignSettlement/GetSettleAccountBil";
            restClient.Query<SettleInfo>(relativeUrl, settleInfo, callback);
        }

        /// <summary>
        /// 审核经销商品结算单
        /// </summary>
        /// <param name="settleInfo"></param>
        /// <param name="callback"></param>
        public void AuditSettleAccount(SettleInfo settleInfo, EventHandler<RestClientEventArgs<object>> callback)
        {
            settleInfo.AuditUserSysNo = RestClient.UserSysNo;
            string relativeUrl = "POService/ConsignSettlement/AuditSettleAccount";
            restClient.Query(relativeUrl, settleInfo, callback);
        }

        #endregion
    }
}
