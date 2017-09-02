using System;
using System.Linq;
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
    public class AdjustRequestQueryFacade
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
        public AdjustRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryAdjustRequest(AdjustRequestQueryVM model, Action<int, List<dynamic>> callback)
        {
            AdjustRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<AdjustRequestQueryVM, AdjustRequestQueryFilter>();

            string relativeUrl = "/InventoryService/AdjustRequest/QueryAdjustRequest";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        List<dynamic> result = null;
                        int totalCount = 0;
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            result = args.Result.Rows.ToList();
                            totalCount = args.Result.TotalCount;
                        }
                        if (callback != null)
                        {
                            callback(totalCount, result);
                        }
                    }
                });
        }

        /// <summary>
        /// 导出查询结果
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForAdjustRequest(AdjustRequestQueryVM model, ColumnSet[] columns)
        {
            AdjustRequestQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<AdjustRequestQueryVM, AdjustRequestQueryFilter>();
            string relativeUrl = "/InventoryService/AdjustRequest/QueryAdjustRequest";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }
    }
}
