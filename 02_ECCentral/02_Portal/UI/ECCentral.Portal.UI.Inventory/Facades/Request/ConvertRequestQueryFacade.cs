using System;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using System.Collections.Generic;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ConvertRequestQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public ConvertRequestQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ConvertRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryConvertRequest(ConvertRequestQueryVM model, Action<int, List<dynamic>> callback)
        {
            ConvertRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<ConvertRequestQueryVM, ConvertRequestQueryFilter>();

            string relativeUrl = "/InventoryService/ConvertRequest/QueryConvertRequest";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    int totalCount = 0;
                    List<dynamic> vmList = null;
                    if (!args.FaultsHandle())
                    {
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList();
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        /// <summary>
        /// 导出查询结果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForConvertRequest(ConvertRequestQueryVM model, ColumnSet[] columns)
        {
            ConvertRequestQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<ConvertRequestQueryVM, ConvertRequestQueryFilter>();
            string relativeUrl = "/InventoryService/ConvertRequest/QueryConvertRequest";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
