using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class PostIncomeFacade
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

        public PostIncomeFacade()
            : this(null)
        {
        }

        public PostIncomeFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(PostIncomeQueryVM query, int pageSize, int pageIndex, string sortField, Action<PostIncomeQueryResultVM> callback)
        {
            PostIncomeQueryFilter queryFilter = query.ConvertVM<PostIncomeQueryVM, PostIncomeQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/PostIncome/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var result = new PostIncomeQueryResultVM();
                result.ResultList = DynamicConverter<PostIncomeVM>.ConvertToVMList(args.Result.Rows);
                result.TotalCount = args.Result.TotalCount;

                callback(result);
            });
        }

        public void ExportExcelFile(PostIncomeQueryVM queryVM, ColumnSet[] columnSet)
        {
            PostIncomeQueryFilter queryFilter = queryVM.ConvertVM<PostIncomeQueryVM, PostIncomeQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "a.SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/PostIncome/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        #region Action

        /// <summary>
        /// 创建
        /// </summary>
        public void Create(PostIncomeVM vm, Action callback)
        {
            var data = vm.ConvertVM<PostIncomeVM, PostIncomeInfo>();
            string relativeUrl = "/InvoiceService/PostIncome/Create";
            data.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<PostIncomeInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        public void Update(PostIncomeVM vm, Action callback)
        {
            var req = new UpdatePostIncomeReq()
            {
                PostIncome = vm.ConvertVM<PostIncomeVM, PostIncomeInfo>(),
                ConfirmedSOSysNo = vm.ConfirmedSOSysNoList
            };
            string relativeUrl = "/InvoiceService/PostIncome/Update";

            restClient.Update(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void Confirm(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/Confirm";

            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 批量确认
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/BatchConfirm";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 取消确认
        /// </summary>
        public void CancelConfrim(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/CancelConfirm";

            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 作废
        /// </summary>
        public void Abandon(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/Abandon";

            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        public void CancelAbandon(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/CancelAbandon";

            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        #endregion Action

        public void ImportPostIncome(string fileIdentity, Action<ImportPostIncomeResp> callback)
        {
            string relativeUrl = "/InvoiceService/PostIncome/Import";

            ImportPostIncomeReq request = new ImportPostIncomeReq();
            request.FileIdentity = fileIdentity;
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<ImportPostIncomeResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }
    }
}