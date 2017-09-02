using System;
using System.Linq;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.Portal.UI.Invoice.Views;
using ECCentral.Portal.UI.Invoice.Facades.RequestMsg;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class InvoiceFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public InvoiceFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        public void Query(InvoiceQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<InvoiceQueryResultVM> callback)
        {
            InvoiceQueryFilter filter = queryVM.ConvertVM<InvoiceQueryVM, InvoiceQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/Invoice/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                InvoiceQueryResultVM result = new InvoiceQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<InvoiceVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }
                if (args.Result[1] != null && args.Result[1].Rows != null && !(args.Result[1].Rows is DynamicXml.EmptyList))
                {
                    result.InvoiceAmt = DynamicConverter<InvoiceAmtVM>.ConvertToVM(args.Result[1].Rows[0]);
                }

                callback(result);
            });
        }

        public void ExportExcelFile(InvoiceQueryVM queryVM, ColumnSet[] columnSet)
        {
            InvoiceQueryFilter queryFilter = queryVM.ConvertVM<InvoiceQueryVM, InvoiceQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/Invoice/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        public void UpdateSOInvoice(int soSysNo, string invoiceNo, string warehouseNo, string companyCode)
        {

            string relativeUrl = string.Format("/InvoiceService/Invoice/UpdateSOInvoice/{0}/{1}/{2}/{3}", soSysNo, invoiceNo, warehouseNo, companyCode);
            restClient.Update(relativeUrl,null,(obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
            });
        }


        #region PriceChange
        /// <summary>
        /// 查询变价申请
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryPriceChange(PriceChangeQueryVM queryVM, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ChangePriceFilter filter = queryVM.ConvertVM<PriceChangeQueryVM, ChangePriceFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/Invoice/QueryPriceChange";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) => 
            {
                if (args.FaultsHandle())
                    return;

                callback(obj,args);
            });

        }
        /// <summary>
        /// 批量审核 
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchAuditPriceChange(Dictionary<int,string> sysNos, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/BatchAuditPriceChange";

            restClient.Create<string>(relativeUrl, sysNos, (obj, args) => 
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchVoidPriceChange(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/BatchVoidPriceChange";

            restClient.Create<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        public void SavePriceChange(PriceChangeVM vm, Action<int> callback)
        {
            PriceChangeMaster entity = vm.ConvertVM<PriceChangeVM, PriceChangeMaster>();

            string relativeUrl = "/InvoiceService/Invoice/SavePriceChange";

            restClient.Create<int>(relativeUrl, entity, (obj, args) => 
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        public void GetPriceChangeBySysNo(int sysNo, Action<PriceChangeVM> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/Invoice/GetPriceChangeBySysNo/{0}",sysNo);

            restClient.Query<PriceChangeMaster>(relativeUrl, (obj, args) => 
            {
                if (args.FaultsHandle())
                    return;

                PriceChangeVM vm = new PriceChangeVM();
                vm.ItemList = new System.Collections.ObjectModel.ObservableCollection<ChangeItemVM>();

                vm = args.Result.Convert<PriceChangeMaster, PriceChangeVM>();

                foreach(var item in vm.ItemList)
                {
                    item.RequestStatus = vm.Status;
                }

                callback(vm);
            });
        }

        public void UpdatePriceChange(PriceChangeVM vm, Action<PriceChangeVM> callback)
        {
            PriceChangeMaster entity = vm.ConvertVM<PriceChangeVM, PriceChangeMaster>();
   
            string relativeUrl = "/InvoiceService/Invoice/UpdatePriceChange";

            restClient.Create<PriceChangeMaster>(relativeUrl, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                PriceChangeVM temp = args.Result.Convert<PriceChangeMaster, PriceChangeVM>();

                callback(temp);
            });
        }

        /// <summary>
        /// 批量启动
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchRunPriceChange(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/BatchRunPriceChangeByManual";

            restClient.Create<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }


        /// <summary>
        /// 批量中止
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void BatchAbortPriceChange(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/BatchAbortedPriceChangeByManual";

            restClient.Create<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        /// <summary>
        /// 克隆数据
        /// </summary>
        /// <param name="sysNos"></param>
        /// <param name="callback"></param>
        public void ClonePriceChange(List<int> sysNos, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/ClonePriceChange";

            restClient.Create<string>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }

        public void GetVendorLastBasicInfo(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/InvoiceService/Invoice/QueryLastVendorSysNo";

            restClient.QueryDynamicData(relativeUrl,"", (obj, args) => 
            {
                callback(obj,args);
            });
        }
        
        #endregion

        public void QueryReconciliation(ReconciliationQueryVM queryVM, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ReconciliationQueryFilter filter = queryVM.ConvertVM<ReconciliationQueryVM, ReconciliationQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };
            string relativeUrl = "/InvoiceService/Invoice/QueryReconciliation";
            restClient.QueryDynamicData(relativeUrl, filter, callback);

            //restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            //{
            //    ReconciliationQueryResultVM result = new ReconciliationQueryResultVM();
                
            //    if (args.Result[0] != null && args.Result[0].Rows != null)
            //    {
            //        result.ResultList = DynamicConverter<ReconciliationVM>.ConvertToVMList(args.Result[0].Rows);
            //        result.TotalCount = args.Result[0].TotalCount;
            //    }
            //});
        }

    }
}