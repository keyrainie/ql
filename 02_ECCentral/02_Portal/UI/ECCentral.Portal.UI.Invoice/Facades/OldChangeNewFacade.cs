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
using ECCentral.QueryFilter.Invoice;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.BizEntity.Invoice;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class OldChangeNewFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Invoice, ConstValue.Key_ServiceBaseUrl);

        public OldChangeNewFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public OldChangeNewFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 以旧换新补贴款查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback"></param>
        public void QueryOldChangeNew(OldChangeNewQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/QueryOldChangeNew";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 获取以旧换新补贴款列表信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void GetOldChangeNewList(OldChangeNewQueryFilter filter, EventHandler<RestClientEventArgs<List<OldChangeNewInfo>>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/GetOldChangeNewList";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Query(relativeUrl, filter, callback);
        }

        /// <summary>
        /// Check以旧换新信息是否有效
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void IsOldChangeNewSO(OldChangeNewQueryFilter filter, EventHandler<RestClientEventArgs<bool>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/IsOldChangeNewSO";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Query(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void CrateOldChangeNew(OldChangeNewQueryVM vm, EventHandler<RestClientEventArgs<OldChangeNewInfo>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/Create";
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            vm.InUser = CPApplication.Current.LoginUser.UserSysNo.Value.ToString();
            var data = vm.ConvertVM<OldChangeNewQueryVM, OldChangeNewInfo>((t, s) =>
                {
                    t.SysNo = s.SysNo;
                });
            restClient.Create<OldChangeNewInfo>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void UpdateOldChangeNewRebate(OldChangeNewQueryVM vm, EventHandler<RestClientEventArgs<OldChangeNewInfo>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/UpdateOldChangeNewRebate";
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            var data = vm.ConvertVM<OldChangeNewQueryVM, OldChangeNewInfo>((t, s) =>
            {
                t.SysNo = s.SysNo;
            });
            restClient.Update<OldChangeNewInfo>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void UpdateOldChangeNewStatus(OldChangeNewQueryVM vm, EventHandler<RestClientEventArgs<OldChangeNewInfo>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/UpdateOldChangeNewStatus";
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            var data = vm.ConvertVM<OldChangeNewQueryVM, OldChangeNewInfo>((t, s) =>
            {
                t.SysNo = s.SysNo;
            });
            restClient.Update<OldChangeNewInfo>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 批量更新以旧换新状态信息
        /// </summary>
        /// <param name="viewList"></param>
        /// <param name="callback"></param>
        public void BatchUpdateOldChangeNewStatus(List<OldChangeNewInfo> infos, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/BtachUpdateOldChangeNewStatus";
            restClient.Update<string>(relativeUrl, infos, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void MaintainReferenceID(List<OldChangeNewInfo> infos, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/BtachMaintainReferenceID";
            restClient.Update<string>(relativeUrl, infos, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    callback(obj, args);
                });
        }

        /// <summary>
        /// 添加财务备注
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void MaintainStatusWithNote(OldChangeNewQueryVM vm, EventHandler<RestClientEventArgs<OldChangeNewInfo>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/MaintainStatusWithNote";
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            var data = vm.ConvertVM<OldChangeNewQueryVM, OldChangeNewInfo>((t, s) =>
            {
                t.SysNo = s.SysNo;
            });
            restClient.Update<OldChangeNewInfo>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 导出报表数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="columns"></param>
        public void ExportOldChangeNew(OldChangeNewQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/InvoiceService/Invoice/QueryOldChangeNew", filter, columns);
        }
    }
}
