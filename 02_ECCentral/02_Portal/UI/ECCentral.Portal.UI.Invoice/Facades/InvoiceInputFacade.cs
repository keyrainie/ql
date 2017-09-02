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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.Service.Invoice.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class InvoiceInputFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public InvoiceInputFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryInvoiceInput(InvoiceInputQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            InvoiceInputQueryFilter queryFilter = new InvoiceInputQueryFilter();
            queryFilter = queryVM.ConvertVM<InvoiceInputQueryVM, InvoiceInputQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/InvoiceService/InvoiceInput/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void QueryPaySettleCompany(int vendorSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/GetPaySettleCompany/{0}",vendorSysNo);

            restClient.Query(relativeUrl, callback);
        }

        public void ExportExcelFile(InvoiceInputQueryVM queryVM, ColumnSet[] columnSet)
        {
            InvoiceInputQueryFilter queryFilter = new InvoiceInputQueryFilter();
            queryFilter = queryVM.ConvertVM<InvoiceInputQueryVM, InvoiceInputQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = null
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/InvoiceInput/Query";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 批量拒绝
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void BatchRefuse(List<int> sysNoList, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/BatchRefuse";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void BatchAudit(List<int> sysNoList, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/BatchAudit";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量撤销
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void BatchCancel(List<int> sysNoList, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/BatchCancel";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void BatchSubmit(List<int> sysNoList, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/BatchSubmit";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 提交时创建或更新
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void SubmitWithSaveAPInvoice(APInvoiceInfo data, EventHandler<RestClientEventArgs<APInvoiceInfo>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/SubmitWithSaveAPInvoice");
            restClient.Create<APInvoiceInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量退回
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void BatchVPCancel(APInvoiceBatchUpdateReq request, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/BatchVPCancel";
            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 加载APInvoice的全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void LoadAPInvoiceWithItemsBySysNo(int sysNo, EventHandler<RestClientEventArgs<APInvoiceInfo>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/Load/{0}", sysNo);
            restClient.Query<APInvoiceInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        /// <summary>
        /// 录入PoItems
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void InputPoItem(APInvoiceItemInputEntity request, EventHandler<RestClientEventArgs<APInvoiceInfoResp>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/InputPoItem";
            restClient.Create<APInvoiceInfoResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
        /// <summary>
        /// 录入InvoiceItems
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void InputInvoiceItem(APInvoiceItemInputEntity request, EventHandler<RestClientEventArgs<APInvoiceInfoResp>> callback)
        {
            string relativeUrl = "/InvoiceService/InvoiceInput/InputInvoiceItem";
            restClient.Create<APInvoiceInfoResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
        /// <summary>
        /// 加载供应商未录入的Poitems
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void LoadNotInputPOItems(APInvoiceItemInputEntity request, EventHandler<RestClientEventArgs<List<APInvoicePOItemInfo>>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/LoadNotInputPOItems", request);
            restClient.Query<List<APInvoicePOItemInfo>>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }


        public void CreateAPInvoice(APInvoiceInfo data, EventHandler<RestClientEventArgs<APInvoiceInfo>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/CreateAPInvoice");
            restClient.Create<APInvoiceInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
        public void UpdateApInvoiceInfo(APInvoiceInfo data, EventHandler<RestClientEventArgs<APInvoiceInfo>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/InvoiceInput/UpdateAPInvoice");
            restClient.Update<APInvoiceInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
    }
}
