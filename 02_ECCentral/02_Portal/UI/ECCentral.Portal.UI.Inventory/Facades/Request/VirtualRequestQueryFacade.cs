using System;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class VirtualRequestQueryFacade
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

        public VirtualRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询虚库列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualRequest(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询此商品对应需要关闭的虚库申请单数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryNeedCloseRequestCount(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<int>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryNeedCloseRequestCount";
            restClient.Query<int>(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询虚库日志
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualRequestMemo(VirtualRequestQueryVM model, Action<int, List<dynamic>> callback)
        {
            VirtualRequestQueryFilter queryFilter;
            queryFilter = model.ConvertVM<VirtualRequestQueryVM, VirtualRequestQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestMemoList";
            restClient.QueryDynamicData(relativeUrl, queryFilter,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        int totalCount = 0;
                        List<dynamic> vmList = null;
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
        ///  查询虚库单关闭日志List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualRequestCloseLog(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestCloseLog";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询虚库单库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualInventoryInfoByStock(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualInventoryInfoByStock";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询虚库单 - 查询最后虚库变更List：
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryVirtualInventoryLastVerifiedRequest(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualInventoryLastVerifiedRequest";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询变更的虚库记录
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryModifiedVirtualRequestList(VirtualRequestQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryModifiedVirtualRequest";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryProducts(VirtualRequestQueryProductsVM filterVM, Action<int, List<VirtualRequestProductVM>> callback)
        {
            VirtualRequestQueryProductsFilter filter = filterVM.ConvertVM<VirtualRequestQueryProductsVM, VirtualRequestQueryProductsFilter>();
            string relativeUrl = "/InventoryService/VirtualRequest/QueryProducts";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<VirtualRequestProductVM> vmList = new List<VirtualRequestProductVM>();
                    int totalCount = 0;
                    if (args.Result != null && args.Result.Rows != null)
                    {
                        totalCount = args.Result.TotalCount;
                        vmList = DynamicConverter<VirtualRequestProductVM>.ConvertToVMList(args.Result.Rows);
                    }
                    if (callback != null)
                    {
                        callback(totalCount, vmList);
                    }
                }
            });
        }

        /// <summary>
        /// 查询虚库日志创建者列表
        /// </summary>        
        /// <param name="callback"></param>
        public void QueryVirtualRequestMemoCreateUserList(Action<int, List<UserInfoVM>> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestMemoCreateUserList";
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

        /// <summary>
        /// 查询虚库申请单创建者列表
        /// </summary>        
        /// <param name="callback"></param>
        public void QueryVirtualRequestCreateUserList(Action<int, List<UserInfoVM>> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestCreateUserList";
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

        /// <summary>
        /// 导出虚库日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForVirtualRequestMemo(VirtualRequestQueryVM model, ColumnSet[] columns)
        {
            VirtualRequestQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<VirtualRequestQueryVM, VirtualRequestQueryFilter>();
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestMemoList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 导出虚库设置信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForVirtualRequest(VirtualRequestQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/VirtualRequest/QueryVirtualRequestList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        /// <summary>
        /// 虚库单 - 批量审核同意
        /// </summary>
        /// <param name="viewVMList"></param>
        /// <param name="callback"></param>
        public void BatchApproveVirtualRequest(List<VirtualRequestVM> viewVMList, Action<string> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/BatchApproveVirtualRequest";

            var infoList = EntityConverter<List<VirtualRequestVM>, List<VirtualRequestInfo>>.Convert(viewVMList);

            restClient.Update<string>(relativeUrl, infoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                callback(args.Result);
            });
        }
    }
}
