using System;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ShiftRequestQueryFacade
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

        public ShiftRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryShiftRequest(ShiftRequestQueryVM model, Action<int, List<dynamic>> callback)
        {
            ShiftRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<ShiftRequestQueryVM, ShiftRequestQueryFilter>();

            string relativeUrl = "/InventoryService/ShiftRequest/QueryShiftRequest";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        int totalCount = 0;
                        List<dynamic> vmList = null;
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = args.Result.Rows.ToList("IsChecked", false);
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        public void QueryShiftDataCount(ShiftRequestQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ShiftRequestQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<ShiftRequestQueryVM, ShiftRequestQueryFilter>();

            restClient.QueryDynamicData("/InventoryService/ShiftRequest/QueryShiftCountData", filter, callback);
        }

        public void QueryShiftRequestCreateUserList(Action<int, List<UserInfoVM>> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/QueryShiftRequestCreateUserList";
            string companyCode = CPApplication.Current.CompanyCode;            
            restClient.QueryDynamicData(relativeUrl, companyCode,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        int totalCount = 0;
                        List<UserInfoVM> vmList = null;
                        if (!(args.Result == null || args.Result.Rows == null))
                        {
                            totalCount = args.Result.TotalCount;
                            vmList = DynamicConverter<UserInfoVM>.ConvertToVMList(args.Result.Rows);
                        }
                        callback(totalCount, vmList);
                    }
                });
        }

        public void Export(ShiftRequestQueryVM queryVM, ColumnSet[] columns)
        {
            ShiftRequestQueryFilter filter;
            filter = queryVM.ConvertVM<ShiftRequestQueryVM, ShiftRequestQueryFilter>();
            restClient.ExportFile("/InventoryService/ShiftRequest/QueryShiftRequest", filter, columns);
        }


        public void GetRMAShift(int shiftSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ShiftRequest/GetRMAShift/{0}", shiftSysNo);
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        #region 移仓单日志
        /// <summary>
        /// 查询移仓单日志
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>     

        public void QueryShiftRequestMemo(ShiftRequestMemoQueryVM model, Action<int, List<dynamic>> callback)
        {
            ShiftRequestMemoQueryFilter filter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            filter = model.ConvertVM<ShiftRequestMemoQueryVM, ShiftRequestMemoQueryFilter>();

            string relativeUrl = "/InventoryService/ShiftRequest/QueryShiftRequestMemo";
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

        public void ExportRequestMemo(ShiftRequestMemoQueryVM queryVM, ColumnSet[] columns)
        {
            ShiftRequestMemoQueryFilter filter;
            filter = queryVM.ConvertVM<ShiftRequestMemoQueryVM, ShiftRequestMemoQueryFilter>();
            restClient.ExportFile("/InventoryService/ShiftRequest/QueryShiftRequestMemo", filter, columns);
        }

        #endregion 移仓单日志
    }
}
