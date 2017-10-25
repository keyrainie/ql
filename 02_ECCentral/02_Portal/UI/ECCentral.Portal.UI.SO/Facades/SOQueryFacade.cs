using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOQueryFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);

        public SOQueryFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public SOQueryFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void QuerySO(SOQueryVM queryVM, Action<List<SOQueryDataVM>, int> action)//EventHandler<RestClientEventArgs<List<SOQueryDataVM>>> handler
        {
            SORequestQueryFilter filter = queryVM == null ? null : EntityConverter<SOQueryVM, SORequestQueryFilter>.Convert(queryVM);
            restClient.QueryDynamicData("/SOService/SO/Query", filter, (sender, e) =>
                {
                    if (!e.FaultsHandle())
                    {
                        if (e.Result != null && action != null)
                        {
                            List<SOQueryDataVM> dataVMList = DynamicConverter<SOQueryDataVM>.ConvertToVMList(e.Result.Rows);
                            action(dataVMList, e.Result.TotalCount);
                        }
                    }
                });

            //restClient.Update("/SOService/SO/Job/ProcessFinishedAndInvalidGroupBuySO", null, (a, b) => { b.FaultsHandle(); });
        }

        public void ExportSO(SOQueryVM queryVM, ColumnSet[] columns)
        {
            SORequestQueryFilter filter = queryVM == null ? null : EntityConverter<SOQueryVM, SORequestQueryFilter>.Convert(queryVM);
            restClient.ExportFile("/SOService/SO/Query", filter, columns);
        }

        /// <summary>
        /// 发票日志查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QuerySOInvoiceChangeLog(SOInvoiceChangeLogQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            query.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QueryInvoiceChangeLog";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        /// <summary>
        /// 发票日志查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QuerySOSystemLog(SOLogQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/SOService/SO/QuerySystemLog";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        /// <summary>
        /// OPC查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryOPCMaster(OPCQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/SOService/SO/QueryOPC";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        /// <summary>
        /// 查询详细运营
        /// </summary>
        /// <param name="masterSysNo"></param>
        /// <param name="callback"></param>
        public void QueryOPCTransaction(int masterSysNo, EventHandler<RestClientEventArgs<List<OPCOfflineTransactionInfo>>> callback)
        {
            restClient.Query<List<OPCOfflineTransactionInfo>>(string.Format("/SOService/SO/QueryOPCTransactionByMasterID/{0}", masterSysNo), callback);
        }

        /// <summary>
        /// 查询所有投诉
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryComplainList(ComplainQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryComplain", request, callback);
        }

        /// <summary>
        /// 查询出货单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QuerySOOutStock(SOOutStockQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryOutStock", request, callback);
        }

        /// <summary>
        /// 查询手动更改仓库信息订单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QuerySOWHUpdate(SOWHUpdateQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryWHUpdate", request, callback);
        }

        public void ExportComplain(ComplainQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SOService/SO/QueryComplain", request, columns);
        }

        public void ExportOutStock(SOOutStockQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SOService/SO/QueryOutStock", request, columns);
        }

        public void ExportWHUpdate(SOWHUpdateQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SoService/SO/QueryWHUpdate", request, columns);
        }

        /// <summary>
        /// 查询运单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QuerySOPackageCoverSearch(SOPackageCoverSearchFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryPackageCoverSearch", request, callback);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="columns"></param>
        public void ExportSOPackageCoverSearch(SOPackageCoverSearchFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SOService/SO/QueryPackageCoverSearch", request, columns);
        }

        public void QueryThirdPartSOSearch(SOThirdPartSOSearchFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryThirdPartSOSearch", request, callback);
        }

        /// <summary>
        /// 查询订单Pending信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        public void QuerySOPending(SOPendingQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryPending", request, callback);
        }

        public void ExportSOPending(SOPendingQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SOService/SO/QueryPending", request, columns);
        }

        /// <summary>
        /// 查询订单跟进日志
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        public void QuerySOInternalMemo(SOInternalMemoQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            query.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/QueryInternalMemo", query, callback);
        }

        public void ExportSOInternalMemo(SOInternalMemoQueryFilter query, ColumnSet[] columns)
        {
            query.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/SOService/SO/QueryInternalMemo", query, columns);
        }

        /// <summary>
        /// 特殊分仓订单查询
        /// </summary>
        /// <param name="query">条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QuerySpecialSO(SpecialSOSearchQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData("/SOService/SO/QuerySpecialSO", query, callback);
        }

        /// <summary>
        /// 查询订单所有信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="callback"></param>
        public void QuerySOInfo(int soSysNo, Action<SOVM> callback)
        {
            restClient.Query<SOInfo>(string.Format("/SOService/SOInfo/Query/{0}", soSysNo), (obj, args) =>
            {
                if (!args.FaultsHandle() && callback != null)
                {
                    SOVM vm = null;
                    if (args.Result != null)
                    {
                        vm = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                    }
                    callback(vm);
                }
            });
        }

        /// <summary>
        /// 根据客户编号获取客户对应的增值税发票信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="callback"></param>
        public void QuerySOVATInvoiceInfo(int customerSysNo, EventHandler<RestClientEventArgs<List<SOVATInvoiceInfo>>> callback)
        {
            restClient.Query<List<SOVATInvoiceInfo>>(string.Format("/SOService/SOVATInvoiceInfo/Query/{0}", customerSysNo), callback);
        }

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="callback"></param>
        public void QuerySOGiftCardsInfo(int customerSysNo, EventHandler<RestClientEventArgs<List<GiftCardInfo>>> callback)
        {
            restClient.Query<List<GiftCardInfo>>(string.Format("/SOService/GiftCardListInfo/Query/{0}", customerSysNo), callback);
        }

        /// <summary>
        /// 根据 礼品卡编号 和密码 获取 对应的礼品卡信息
        /// </summary>
        /// <param name="code">礼品卡 卡号</param>
        /// <param name="password">礼品卡 密码</param>
        /// <param name="callback"></param>
        public void QueryGiftCardByCodeAndPassword(string code, string password, EventHandler<RestClientEventArgs<GiftCardInfo>> callback)
        {
            restClient.Query<GiftCardInfo>(string.Format("/SOService/GiftCardInfo/Query/{0}/{1}", code, password), callback);
        }

        /// <summary>
        /// 根据客户编号 获取客户所有信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="callback"></param>
        public void QuerySOCustomerInfo(int customerSysNo, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            string customerRestUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Customer, ConstValue.Key_ServiceBaseUrl);
            RestClient customeRestClient = new RestClient(customerRestUrl);
            customeRestClient.Query<CustomerInfo>(string.Format("/CustomerService/Customer/Load/{0}", customerSysNo), callback);
        }

        /// <summary>
        /// 根据优惠券编号获取相应优惠券信息
        /// </summary>
        /// <param name="couponCodeSysNo">优惠券编号</param>
        /// <param name="callback"></param>
        public void QueryMKTCouponsInfoByCouponCodeSysNo(int couponCodeSysNo, EventHandler<RestClientEventArgs<CouponsInfo>> callback)
        {
            string customerRestUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_MKT, ConstValue.Key_ServiceBaseUrl);
            RestClient customeRestClient = new RestClient(customerRestUrl);
            customeRestClient.Query<CouponsInfo>(string.Format("/MKTService/Coupons/GetCouponsInfoByCouponCodeSysNo/{0}", couponCodeSysNo), callback);
        }

        /// <summary>
        /// 根据支付方式判断是否为货到付款
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public void IsPayWhenReceived(int payTypeSysNo, string companyCode, EventHandler<RestClientEventArgs<Boolean>> callback)
        {
            restClient.Query<Boolean>(string.Format("/SOService/IsPayWhenReceived/{0}/{1}", payTypeSysNo, companyCode), callback);
        }

        /// <summary>
        /// 获取时间范围内的 商品 改价信息
        /// </summary>
        /// <param name="productSysNoList">商品编号</param>
        /// <param name="startTime">订单下单时间</param>
        /// <param name="endTime">客户收货时间</param>
        /// <param name="callback"></param>
        public void GetPriceChangeLogs(List<int> productSysNoList, DateTime startTime, DateTime endTime, EventHandler<RestClientEventArgs<List<PriceChangeLogInfo>>> callback)
        {
            restClient.Query<List<PriceChangeLogInfo>>(string.Format("/SOService/GetProductPriceChangeLogsInfo/{0}/{1}", startTime.ToString("yyyy-MM-dd"), endTime.ToString("yyyy-MM-dd")), productSysNoList, callback);
        }

        /// <summary>
        /// 分仓销售单据查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void WHSOOutStockQuery(WHSOOutStockSearchVM queryVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            WHSOOutStockQueryFilter filter = queryVM == null ? null : EntityConverter<WHSOOutStockSearchVM, WHSOOutStockQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/WHSOOutStockQuery";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 分仓销售单据导出Excel
        /// </summary>
        /// <param name="queryVM">过滤条件</param>
        /// <param name="columns">输入列</param>
        public void ExportWHSOOutStock(WHSOOutStockSearchVM queryVM, ColumnSet[] columns)
        {
            WHSOOutStockQueryFilter filter = queryVM == null ? null : EntityConverter<WHSOOutStockSearchVM, WHSOOutStockQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/WHSOOutStockQuery";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 查询配送历史
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void SODeliveryHistoryQuery(SODeliveryHistorySearchVM queryVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SODeliveryHistoryQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryHistorySearchVM, SODeliveryHistoryQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QueryDeliveryHistory";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询配送历史导出Excel
        /// </summary>
        /// <param name="queryVM">过滤条件</param>
        /// <param name="columns">输入列</param>
        public void ExportSODeliveryHistory(SODeliveryHistorySearchVM queryVM, ColumnSet[] columns)
        {
            SODeliveryHistoryQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryHistorySearchVM, SODeliveryHistoryQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QueryDeliveryHistory";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 查询配送任务
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void SODeliveryAssignTaskQuery(SODeliveryAssignTaskSearchVM queryVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SODeliveryAssignTaskQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryAssignTaskSearchVM, SODeliveryAssignTaskQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QuerySODeliveryAssignTask";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询配送任务导出Excel
        /// </summary>
        /// <param name="queryVM">过滤条件</param>
        /// <param name="columns">输入列</param>
        public void ExportSODeliveryAssignTask(SODeliveryAssignTaskSearchVM queryVM, ColumnSet[] columns)
        {
            SODeliveryAssignTaskQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryAssignTaskSearchVM, SODeliveryAssignTaskQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QuerySODeliveryAssignTask";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 配送服务评级
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void SODeliveryScoreQuery(SODeliveryScoreSearchVM queryVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SODeliveryScoreQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryScoreSearchVM, SODeliveryScoreQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QuerySODeliveryScore";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 配送服务评级导出Excel
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void ExportSODeliveryScore(SODeliveryScoreSearchVM queryVM, ColumnSet[] columns)
        {
            SODeliveryScoreQueryFilter filter = queryVM == null ? null : EntityConverter<SODeliveryScoreSearchVM, SODeliveryScoreQueryFilter>.Convert(queryVM);
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/SOService/SO/QuerySODeliveryScore";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 生成出库单查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryOutStock4Finance(SOOutStock4FinanceQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData("/SOService/SO/QueryOutStock4Finance", query, callback);
        }


        /// <summary>
        /// 根据客户系统编号获取客户对应的客户类型信息
        /// </summary>
        /// <param name="CustomerSysNo">客户系统编号</param>
        /// <param name="callback"></param>
        public void QueryKnownFraudCustomerInfo(int CustomerSysNo, EventHandler<RestClientEventArgs<KnownFraudCustomer>> callback)
        {
            restClient.Query<KnownFraudCustomer>(string.Format("/SOService/GetKnownFraudCustomerInfo/{0}", CustomerSysNo), callback);
        }

        #region 中蛋定制

        /// <summary>
        /// 待审核OZZO订单查询
        /// </summary>
        /// <param name="query">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void OZZOOriginNoteQuery(DefaultQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData("/SOService/SO/OZZOOriginNoteQuery", query, callback);
        }

        /// <summary>
        /// 订单拦截 信息设置查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        public void QuerySOIntercept(SOInterceptQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData("/SOService/SO/QuerySOIntercept", query, callback);
        }

        /// <summary>
        /// 获取配送方式列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetShipTypeList(EventHandler<RestClientEventArgs<List<ShippingType>>> callback)
        {
            string relativeUrl = string.Format("/CommonService/ShippingType/GetAll/{0}", CPApplication.Current.CompanyCode);
            string commonRestUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Common, ConstValue.Key_ServiceBaseUrl);
            new RestClient(commonRestUrl).Query<List<ShippingType>>(relativeUrl, callback);
        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QuerySO(SORequestQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/SOService/SO/Query", request, callback);
        }

        #endregion 中蛋定制

        #region 社团
        public void QuerySOSociety(SOQueryVM queryVM, Action<List<SOQueryDataVM>, int> action)//EventHandler<RestClientEventArgs<List<SOQueryDataVM>>> handler
        {
            SORequestQueryFilter filter = queryVM == null ? null : EntityConverter<SOQueryVM, SORequestQueryFilter>.Convert(queryVM);
            restClient.QueryDynamicData("/SOService/SO/QuerySocietyOrder", filter, (sender, e) =>
            {
                if (!e.FaultsHandle())
                {
                    if (e.Result != null && action != null)
                    {
                        List<SOQueryDataVM> dataVMList = DynamicConverter<SOQueryDataVM>.ConvertToVMList(e.Result.Rows);
                        action(dataVMList, e.Result.TotalCount);
                    }
                }
            });
        }
        #endregion
    }
}