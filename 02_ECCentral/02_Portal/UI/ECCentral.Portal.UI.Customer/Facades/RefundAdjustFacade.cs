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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class RefundAdjustFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Customer, "ServiceBaseUrl");
            }
        }
        public RefundAdjustFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public RefundAdjustFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询补偿退款单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryRefundAdjust(RefundAdjustQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void CreateRefundAdjust(RefundAdjustInfo entity, EventHandler<RestClientEventArgs<RefundAdjustInfo>> callback)
        {
            string relativeUrl = "/CustomerService/RefundAdjust/Create";
            restClient.Create<RefundAdjustInfo>(relativeUrl, entity, callback);
        }

        /// <summary>
        /// 更新补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void UpdateRefundAdjust(RefundAdjustInfo entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/RefundAdjust/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        /// <summary>
        /// 更新补偿退款单状态
        /// </summary>
        /// <param name="refundSysNo">补偿退款单系统编号</param>
        /// <param name="callback"></param>
        public void UpdateRefundAdjustStatus(RefundAdjustInfo entity, EventHandler<RestClientEventArgs<Boolean>> callback)
        {
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/RefundAdjustUpdateStatus";
            restClient.Update<Boolean>(relativeUrl, entity, callback);
        }

        /// <summary>
        /// 批量更新补偿退款单状态
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="callback"></param>
        public void BatchUpdateRefundAdjustStatus(List<RefundAdjustInfo> entitys, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/RefundAdjust/BatchUpdateRefundAdjustStatus";
            restClient.Update(relativeUrl, entitys, callback);
        }

        /// <summary>
        /// 根据申请单号获取补偿退款单相关信息
        /// </summary>
        /// <param name="refundID"></param>
        /// <param name="callback"></param>
        public void GetRefundAdjustBySoSysNo(RefundAdjustMaintainVM vm, EventHandler<RestClientEventArgs<RefundAdjustInfo>> callback)
        {
            var data = vm.ConvertVM<RefundAdjustMaintainVM, RefundAdjustInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/GetRefundAdjustBySoSysNo";
            restClient.Query<RefundAdjustInfo>(relativeUrl, data, callback);
        }

        #region 节能补贴相关

        /// <summary>
        /// 查询节能补贴信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryEnergySubsidy(RefundAdjustQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/EnergySubsidyQuery";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 节能补贴导出查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EnergySubsidyExport(RefundAdjustQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/EnergySubsidyExport";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 获取节能补贴详细信息
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void GetEnergySubsidyDetails(EnergySubsidyVM vm, EventHandler<RestClientEventArgs<List<EnergySubsidyInfo>>> callback)
        {
            var data = vm.ConvertVM<EnergySubsidyVM, EnergySubsidyInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/RefundAdjust/GetEnergySubsidyDetails";
            restClient.Query<List<EnergySubsidyInfo>>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 节能补贴信息导出
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="columns"></param>
        public void ExportEnergySubsidy(RefundAdjustQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/CustomerService/RefundAdjust/EnergySubsidyExport", filter, columns);
        }

        #endregion
    }
}
