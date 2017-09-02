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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Facades
{
    public class NeweggAmbassadorFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }



        public NeweggAmbassadorFacade(IPage page)
        {

            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询泰隆优选大使基本信息。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void QueryAmbassadorBasicInfo(NeweggAmbassadorQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorQueryVM, NeweggAmbassadorQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/NeweggAmbassador/QueryAmbassadorBasicInfo";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        /// <summary>
        /// 查询泰隆优选大使代购订单信息。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void QueryAmbassadorPurchaseOrderInfo(NeweggAmbassadorOrderQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/NeweggAmbassador/QueryAmbassadorPurchaseOrderInfo";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        /// <summary>
        /// 查询泰隆优选大使推荐订单信息。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void QueryAmbassadorRecommendOrderInfo(NeweggAmbassadorOrderQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/NeweggAmbassador/QueryAmbassadorRecommendOrderInfo";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        /// <summary>
        /// 查询泰隆优选大使推荐订单信息。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void QueryPointInfo(NeweggAmbassadorOrderQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/NeweggAmbassador/QueryPointInfo";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        /// <summary>
        /// 导出泰隆优选大使基本信息到Excel
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void ExportAllBasicInfoToExcel(NeweggAmbassadorQueryVM queryVM, ColumnSet[] columnSet)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorQueryVM, NeweggAmbassadorQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            queryFilter.PagingInfo=new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            queryFilter.PagingInfo.PageSize = -1;
            string relativeUrl = "/MKTService/NeweggAmbassador/QueryAmbassadorBasicInfo";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);

        }

        /// <summary>
        /// 导出泰隆优选大使代购订单信息到Excel
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void ExportAllPurchaseOrderInfoToExcel(NeweggAmbassadorOrderQueryVM queryVM, ColumnSet[] columnSet)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            queryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            queryFilter.PagingInfo.PageSize = -1;
            string relativeUrl = "/MKTService/NeweggAmbassador/ExportAmbassadorPurchaseOrderInfo";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);

        }

        /// <summary>
        /// 导出泰隆优选大使推荐订单信息到Excel
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void ExportAllRecommendOrderInfoToExcel(NeweggAmbassadorOrderQueryVM queryVM, ColumnSet[] columnSet)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            queryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            queryFilter.PagingInfo.PageSize = -1;
            string relativeUrl = "/MKTService/NeweggAmbassador/ExportAmbassadorRecommendOrderInfo";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);

        }

        /// <summary>
        /// 导出泰隆优选大使积分发放信息到Excel
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void ExportAllPointInfoToExcel(NeweggAmbassadorOrderQueryVM queryVM, ColumnSet[] columnSet)
        {
            var queryFilter = queryVM.ConvertVM<NeweggAmbassadorOrderQueryVM, NeweggAmbassadorOrderQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            queryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            queryFilter.PagingInfo.PageSize = -1;
            string relativeUrl = "/MKTService/NeweggAmbassador/ExportPointInfo";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);

        }

        /// <summary>
        /// 尝试更新泰隆优选大使的状态，返回需要确认的泰隆优选大使。
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="status"></param>
        /// <param name="callback"></param>
        public void TryUpdateAmbassadorStatus(List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos, EventHandler<RestClientEventArgs<NeweggAmbassadorBatchInfo>> callback)
        {
            NeweggAmbassadorBatchInfo batchInfo = new NeweggAmbassadorBatchInfo();
            batchInfo.NeweggAmbassadors = neweggAmbassadorStatusInfos;
            batchInfo.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/MKTService/NeweggAmbassador/TryUpdateAmbassadorStatus";
            restClient.Update(relativeUrl, batchInfo, callback);

        }

        /// <summary>
        /// 更新泰隆优选大使的状态信息，即激活或取消泰隆优选大使。
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="status"></param>
        /// <param name="callback"></param>
        public void MaintainNeweggAmbassadorStatus(List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos, EventHandler<RestClientEventArgs<NeweggAmbassadorBatchInfo>> callback)
        {
            NeweggAmbassadorBatchInfo batchInfo = new NeweggAmbassadorBatchInfo();
            batchInfo.NeweggAmbassadors = neweggAmbassadorStatusInfos;
            batchInfo.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/MKTService/NeweggAmbassador/MaintainNeweggAmbassadorStatus";
            restClient.Update(relativeUrl, batchInfo, callback);

        }

        /// <summary>
        /// 更新泰隆优选大使的状态信息，即激活或取消泰隆优选大使。
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="status"></param>
        /// <param name="callback"></param>
        public void CancelRequestNeweggAmbassadorStatus(List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos, EventHandler<RestClientEventArgs<NeweggAmbassadorBatchInfo>> callback)
        {
            NeweggAmbassadorBatchInfo batchInfo = new NeweggAmbassadorBatchInfo();
            batchInfo.NeweggAmbassadors = neweggAmbassadorStatusInfos;
            batchInfo.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/MKTService/NeweggAmbassador/CancelRequestNeweggAmbassadorStatus";
            restClient.Update(relativeUrl, batchInfo, callback);

        }
    }
}
