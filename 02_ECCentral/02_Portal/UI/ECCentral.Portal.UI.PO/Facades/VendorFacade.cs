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
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.BizEntity.PO.Vendor;
using ECCentral.Portal.UI.PO.Models.Vendor;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class VendorFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VendorFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询供应商列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryVendors(VendorQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryVendorList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        /// <summary>
        /// 获取Vendor下可以锁定的PM列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryCanLockPMListVendorSysNo(VendorQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryCanLockPMListVendorSysNo";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        /// <summary>
        ///  获取Vendor下已经锁定的PM列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryVendorPMHoldInfoByVendorSysNo(VendorQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryVendorPMHoldInfoByVendorSysNo";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void ExportExcelForVendors(VendorQueryFilter request, ColumnSet[] columns)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryVendorList";
            restClient.ExportFile(relativeUrl, request, columns);
        }

        /// <summary>
        /// 加载单个供应商信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void GetVendorBySysNo(string vendorSysNo, EventHandler<RestClientEventArgs<VendorInfo>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/GetVendorInfo/{0}", vendorSysNo);
            restClient.Query<VendorInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 查询供应商历史日志信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void GetVendorHistoryLogBySysNo(string vendorSysNo, EventHandler<RestClientEventArgs<List<VendorHistoryLog>>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/GetVendorHistoryLogs/{0}", vendorSysNo);
            restClient.Query<List<VendorHistoryLog>>(relativeUrl, callback);
        }

        //创建供应商信息
        public void CreateVendor(VendorInfoVM newVendorInfoVM, EventHandler<RestClientEventArgs<VendorInfo>> callback)
        {
            VendorInfo vendorInfo = EntityConverter<VendorInfoVM, VendorInfo>.Convert(newVendorInfoVM);
            vendorInfo.CompanyCode = CPApplication.Current.CompanyCode;
            vendorInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            vendorInfo.CreateUserName = CPApplication.Current.LoginUser.DisplayName;

            string relativeUrl = "/POService/Vendor/CreateVendor";
            restClient.Create<VendorInfo>(relativeUrl, vendorInfo, callback);
        }
        /// <summary>
        /// 手动创建供应商历史信息
        /// </summary>
        /// <param name="log"></param>
        /// <param name="callback"></param>
        public void CreateVendorHistoryLog(VendorHistoryLog log, EventHandler<RestClientEventArgs<VendorHistoryLog>> callback)
        {
            log.CompanyCode = CPApplication.Current.CompanyCode;
            log.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;

            string relativeUrl = "/POService/Vendor/CreateVendorHistoryLog";
            restClient.Create<VendorHistoryLog>(relativeUrl, log, callback);
        }

        /// <summary>
        /// 撤销(等级申请/下单日期/代理信息)
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="callback"></param>
        public void CancelVendorModifyRequest(VendorModifyRequestInfo requestInfo, EventHandler<RestClientEventArgs<VendorModifyRequestInfo>> callback)
        {
            requestInfo.CompanyCode = CPApplication.Current.CompanyCode;
            requestInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/CancelVendorModifyRequest";
            restClient.Update<VendorModifyRequestInfo>(relativeUrl, requestInfo, callback);
        }

        /// <summary>
        /// 申请通过(下单日期/等级申请/代理信息)
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="callback"></param>
        public void PassVendorModifyRequest(VendorModifyRequestInfo requestInfo, EventHandler<RestClientEventArgs<VendorModifyRequestInfo>> callback)
        {
            requestInfo.CompanyCode = CPApplication.Current.CompanyCode;
            requestInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/PassVendorModifyRequest";
            restClient.Update<VendorModifyRequestInfo>(relativeUrl, requestInfo, callback);
        }

        public void ApproveVendor(VendorApproveInfo vendorApproveInfo, EventHandler<RestClientEventArgs<bool>> callback)
        {
            vendorApproveInfo.CompanyCode = CPApplication.Current.CompanyCode;
            vendorApproveInfo.UserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            vendorApproveInfo.UserName = CPApplication.Current.LoginUser.DisplayName;
            const string relativeUrl = "/POService/Vendor/ApproveVendor";
            restClient.Update(relativeUrl, vendorApproveInfo, callback);
        }

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <param name="callback"></param>
        public void EditVendorInfo(VendorInfo vendorInfo, EventHandler<RestClientEventArgs<VendorInfo>> callback)
        {
            vendorInfo.CompanyCode = CPApplication.Current.CompanyCode;
            vendorInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            vendorInfo.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            if (vendorInfo.VendorServiceInfo.AreaInfo.CitySysNo != null)
            {
                vendorInfo.VendorServiceInfo.AreaInfo.SysNo = vendorInfo.VendorServiceInfo.AreaInfo.CitySysNo;
            }
            string relativeUrl = "/POService/Vendor/EditVendorInfo";
            restClient.Update<VendorInfo>(relativeUrl, vendorInfo, callback);
        }

        /// <summary>
        /// 锁定供应商信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <param name="callback"></param>
        public void HoldVendor(VendorInfo vendorInfo, EventHandler<RestClientEventArgs<int>> callback)
        {
            vendorInfo.CompanyCode = CPApplication.Current.CompanyCode;
            vendorInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            vendorInfo.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/Vendor/HoldVendorInfo";
            restClient.Update<int>(relativeUrl, vendorInfo, callback);
        }

        /// <summary>
        /// 锁定，解锁供应商关联PM：
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void HoldOrUnholdVendorPM(VendorHoldPMReq request, EventHandler<RestClientEventArgs<List<int>>> callback)
        {
            request.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/HoldOrUnHoldVendorPM";
            restClient.Update<List<int>>(relativeUrl, request, callback);
        }

        /// <summary>
        /// 解锁供应商信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <param name="callback"></param>
        public void UnHoldVendor(VendorInfo vendorInfo, EventHandler<RestClientEventArgs<int>> callback)
        {
            vendorInfo.CompanyCode = CPApplication.Current.CompanyCode;
            vendorInfo.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            vendorInfo.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/Vendor/UnHoldVendorInfo";
            restClient.Update<int>(relativeUrl, vendorInfo, callback);
        }

        /// <summary>
        /// 提交申请 - 供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void RequestForApprovalVendorFinanceInfo(VendorModifyRequestInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/RequestForApprovalVendorFinanceInfo";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核通过 - 供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void ApproveVendorFinanceInfo(VendorModifyRequestInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/ApproveVendorFinanceInfo";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 审核拒绝 - 供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void DeclineVendorFinanceInfo(VendorModifyRequestInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/DeclineVendorFinanceInfo";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 取消审核 - 供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void WithDrawVendorFinanceInfo(VendorModifyRequestInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            string relativeUrl = "/POService/Vendor/WithdrawVendorFinanceInfo";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        ///  更新供应商邮件地址(采购单-邮件地址维护用到.)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void UpdateVendorMailAddress(VendorInfo info, EventHandler<RestClientEventArgs<object>> callback)
        {
            info.CompanyCode = CPApplication.Current.CompanyCode;
            info.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            info.CreateUserName = CPApplication.Current.LoginUser.DisplayName;
            string relativeUrl = "/POService/Vendor/UpdateVendorMailAddress";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 查询供应商应付款余额与负PO金额
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void QueryVendorPayBalanceByVendorSysNo(string vendorSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/GetVendorPayBalanceByVendorSysNo/{0}", vendorSysNo);
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        /// <summary>
        /// 移动上传的附件(从临时文件夹移动)
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="callback"></param>
        public void ModeVendorAttchmentFile(string fileIdentity, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/Vendor/MoveVendorAttachmentFile";
            restClient.Update(relativeUrl, fileIdentity, callback);
        }

        public void GetWarehouseInfo(EventHandler<RestClientEventArgs<List<WarehouseInfo>>> callback)
        {
            string relativeUrl = "/POService/Vendor/LoadWarehouseInfo";
            restClient.Query<List<WarehouseInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="callback"></param>
        public void GetStockList(EventHandler<RestClientEventArgs<List<WarehouseInfo>>> callback)
        {
            string relativeUrl = "/POService/Vendor/GetStockList";
            restClient.Query<List<WarehouseInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        public void CreateVendorStoreInfo(VendorStoreInfoVM vm, EventHandler<RestClientEventArgs<VendorStoreInfo>> callback)
        {
            var info = EntityConverter<VendorStoreInfoVM, VendorStoreInfo>.Convert(vm);
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/POService/VendorStore/Create";
            restClient.Create(relativeUrl, info, callback);
        }

        public void UpdateVendorStoreInfo(VendorStoreInfoVM vm, EventHandler<RestClientEventArgs<VendorStoreInfo>> callback)
        {
            var info = EntityConverter<VendorStoreInfoVM, VendorStoreInfo>.Convert(vm);
            info.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/POService/VendorStore/Update";
            restClient.Update(relativeUrl, info, callback);
        }

        /// <summary>
        /// 查询商家页面类型
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void QueryStorePageType(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/QueryStorePageType");
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        /// <summary>
        /// 查询商家页面
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void QueryStorePageInfo(StorePageQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/QueryStorePageInfo");
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 批量删除商家页面
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void BatchDeleteStorePageInfo(List<int> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/BatchDeleteStorePageInfo");
            restClient.QueryDynamicData(relativeUrl, list, callback);
        }

        public void getPreviewPath(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/getPreviewPath");
            restClient.Update(relativeUrl,null, callback);
        }


        public void QueryCommissionRuleTemplateInfo(CommissionRuleTemplateQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/VendorStore/QueryCommissionRuleTemplateInfo";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void BatchActiveCommissionRuleTemplate(List<int> request, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/POService/VendorStore/BatchSetCommissionRuleActive";
            string ids = string.Empty;
            request.ForEach((item) => { ids += item + ","; });
            restClient.Update(relativeUrl, ids.TrimEnd(','), callback);
        }


        public void BatchDEActiveCommissionRuleTemplate(List<int> request, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/POService/VendorStore/BatchSetCommissionRuleDEActive";
            string ids = string.Empty;
            request.ForEach((item) => { ids += item + ","; });
            restClient.Update(relativeUrl, ids.TrimEnd(','), callback);
        }

        public void UpdateCommissionRuleTemplate(CommissionRuleTemplateInfo request, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/POService/VendorStore/UpdateCommissionRuleTemplate";
            restClient.Update(relativeUrl, request, callback);
        }



        public void LoadCommissionRuleTemplate(int syssno, EventHandler<RestClientEventArgs<CommissionRuleTemplateInfo>> callback)
        {
            string relativeUrl = "/POService/VendorStore/GetCommissionRuleTemplateInfo/" + syssno;
            restClient.Query<CommissionRuleTemplateInfo>(relativeUrl, callback);
        }

        public void ExportExcelForVendorBrandFiling(int vendorId, ColumnSet[] columns)
        {
            string relativeUrl = "/POService/VendorStore/ExportExcelForVendorBrandFiling/" + vendorId;
            restClient.ExportFile(relativeUrl, columns);
        }

        public void QuerySecondDomain(SecondDomainQueryVM model, int pageIndex, int pageSize, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SecondDomainQueryFilter filter = model.ConvertVM<SecondDomainQueryVM, SecondDomainQueryFilter>();

            filter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/POService/VendorStore/QuerySecondDomain";

            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void SecondDomainAuditThrough(int SysNo, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/VendorStore/SecondDomainAuditThrough";
            restClient.Update(relativeUrl, SysNo, callback);
        }

        public void SecondDomainAuditThroughNot(int SysNo, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/VendorStore/SecondDomainAuditThroughNot";
            restClient.Update(relativeUrl, SysNo, callback);
        }

        public void CheckStorePageInfo(int SysNo, int status, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/POService/Vendor/CheckStorePageInfo";
            restClient.Update(relativeUrl, SysNo + "," + status, callback);
        }
    }
}
