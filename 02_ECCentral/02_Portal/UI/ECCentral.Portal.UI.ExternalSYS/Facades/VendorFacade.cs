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
using ECCentral.QueryFilter.ExternalSYS;
using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class VendorFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public VendorFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public VendorFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 角色查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryVendorRole(VendorRoleQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryVendorRole";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        public void QueryVendorRoleVM(VendorRoleQueryFilter query, Action<List<RoleMgmtSearchResultVM>, int> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryVendorRole";


            restClient.QueryDynamicData(relativeUrl, query, (sender, e) =>
            {
                if (!e.FaultsHandle())
                {
                    if (e.Result != null && callback != null)
                    {
                        List<RoleMgmtSearchResultVM> dataVMList = DynamicConverter<RoleMgmtSearchResultVM>.ConvertToVMList(e.Result.Rows);
                        callback(dataVMList, e.Result.TotalCount);
                    }
                }

            });

        }

        /// <summary>
        /// 日志查询
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryLog(VendorSystemQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryVendorSystemLog";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        /// <summary>
        /// 查询账户用户
        /// </summary>
        /// <param name="query">过滤条件</param>
        /// <param name="callback">回调函数</param>
        public void QueryUser(VendorUserQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryUser";
            restClient.QueryDynamicData(relativeUrl, query, callback);
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="c3SysNo">第三级类别分类</param>
        /// <param name="companyCode">公司编码</param>
        /// <param name="callback">回调函数</param>
        public void DownTemplate(int c3SysNo, string companyCode)
        {
            restClient.Query<string>(string.Format("/ExternalSYSService/ExternalSYS/GetDownloadTemplateUrl/{0}/{1}", c3SysNo, companyCode)
                                    , (o, arg) =>
                                    {
                                        if (!arg.FaultsHandle())
                                        {
                                            //下载模板
                                            if (!string.IsNullOrEmpty(arg.Result))
                                            {
                                                string url = restClient.ServicePath.TrimEnd(new char[] { '/', '\\' }) + "/" + arg.Result.TrimStart(new char[] { '/', '\\' });
                                                UtilityHelper.OpenWebPage(url);
                                            }
                                        }
                                    });
        }

        public void BatchPassUser(List<int> sysNos, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/ExternalSYSService/ExternalSYS/PassUser", sysNos, callback);
        }

        public void BatchInvalidUser(List<int> sysNos, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.Update("/ExternalSYSService/ExternalSYS/InvalidUser", sysNos, callback);
        }

        public void QueryUserBySysNo(int sysNo, EventHandler<RestClientEventArgs<VendorUser>> callback)
        {
            restClient.Query<VendorUser>(string.Format("/ExternalSYSService/ExternalSYS/GetUserInfo/{0}", sysNo), callback);
        }

        public void CreateRole(Role entity, EventHandler<RestClientEventArgs<Role>> callback)
        {
            restClient.Create<Role>("/ExternalSYSService/ExternalSYS/CreateRole", entity, callback);

        }

        public void UpdateRole(Role entity, EventHandler<RestClientEventArgs<Role>> callback)
        {
            restClient.Update<Role>("/ExternalSYSService/ExternalSYS/UpdateRole", entity, callback);

        }

        public void UpdateRoleStatus(Role entity, EventHandler<RestClientEventArgs<Role>> callback)
        {
            restClient.Update<Role>("/ExternalSYSService/ExternalSYS/UpdateRoleStatus", entity, callback);

        }


        public void UpdateRoleStatusBatch(List<Role> list, EventHandler<RestClientEventArgs<List<Role>>> callback)
        {
            restClient.Update<List<Role>>("/ExternalSYSService/ExternalSYS/UpdateRoleStatusBatch", list, callback);
        }

        public void GetPrivilegeList(EventHandler<RestClientEventArgs<List<PrivilegeEntity>>> callback)
        {
            restClient.Query("/ExternalSYSService/ExternalSYS/GetPrivilegeList", callback);
        }

        public void GetPrivilegeListByRoleSysNo(int sysNo, EventHandler<RestClientEventArgs<List<PrivilegeEntity>>> callback)
        {
            restClient.Query(string.Format("/ExternalSYSService/ExternalSYS/GetPrivilegeListByRoleSysNo/{0}", sysNo), callback);
        }

        /// <summary>
        /// 创建UserVendor
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void CreateVendorUser(VendorUserVM entity, EventHandler<RestClientEventArgs<VendorUser>> callback)
        {
            entity.InUser = entity.EditUser = CPApplication.Current.LoginUser.LoginName;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            var data = entity.ConvertVM<VendorUserVM, VendorUser>();
            restClient.Create<VendorUser>("/ExternalSYSService/ExternalSYS/CreateVendorUser", data, callback);
        }

        /// <summary>
        /// 更新UserVendor
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void UpdateVendorUser(VendorUserVM entity, EventHandler<RestClientEventArgs<bool>> callback)
        {
            entity.EditUser = CPApplication.Current.LoginUser.LoginName;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            var data = entity.ConvertVM<VendorUserVM, VendorUser>();
            restClient.Update<bool>("/ExternalSYSService/ExternalSYS/UpdateVendorUser", data, callback);
        }

        /// <summary>
        /// 更新UserVendorRole
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="callback"></param>
        public void UpdateVendorUserRole(VendorUserRoleListVM entityList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = entityList.ConvertVM<VendorUserRoleListVM, VendorUserRoleList>();
            restClient.Update("/ExternalSYSService/ExternalSYS/UpdateVendorUserRole", data, callback);
        }

        /// <summary>
        /// 根据供应商编号获取代理信息
        /// </summary>
        /// <param name="vendorSysNo">供应编号</param>
        /// <param name="callback"></param>
        public void GetVendorAgentInfoList(string vendorSysNo, EventHandler<RestClientEventArgs<List<VendorAgentInfo>>> callback)
        {
            restClient.Query<List<VendorAgentInfo>>(string.Format("/ExternalSYSService/ExternalSYS/GetVendorAgentInfoList/{0}", vendorSysNo), callback);
        }

        /// <summary>
        /// 根据厂家编号和用户编号获取角色列表
        /// </summary>
        /// <param name="manufacturerSysNo">厂家编号</param>
        /// <param name="userSysNo">用户编号</param>
        /// <param name="callback"></param>
        public void GetRoleListByVendorEx(string manufacturerSysNo, string userSysNo, EventHandler<RestClientEventArgs<List<VendorUserMapping>>> callback)
        {
            restClient.Query(string.Format("/ExternalSYSService/ExternalSYS/GetRoleListByVendorEx/{0}/{1}", manufacturerSysNo, userSysNo), callback);
        }

        /// <summary>
        /// 根据用户编号获取角色列表
        /// </summary>
        /// <param name="userSysNo">用户编号</param>
        /// <param name="callback"></param>
        public void GetRoleListByUserSysNo(string userSysNo, EventHandler<RestClientEventArgs<List<VendorUserMapping>>> callback)
        {
            restClient.Query<List<VendorUserMapping>>(string.Format("/ExternalSYSService/ExternalSYS/GetRoleListByUserSysNo/{0}", userSysNo), callback);
        }

        /// <summary>
        /// 查询VendorProduct
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryVendorProduct(VendorProductQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryVendorProduct";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void GetIsAuto(VendorProductQueryFilter filter, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/ExternalSYSService/ExternalSYS/GetIsAuto";
            restClient.Query<int>(relativeUrl, filter, callback);
        }

        public void QueryByStockShippingeInvoic(Vendor_ExVM vm, EventHandler<RestClientEventArgs<List<Vendor_ExInfo>>> callback)
        {
            var data = vm.ConvertVM<Vendor_ExVM, Vendor_ExInfo>();
            string relativeUrl = "/ExternalSYSService/ExternalSYS/QueryByStockShippingeInvoice";
            restClient.Query<List<Vendor_ExInfo>>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 更新VendorProduct
        /// </summary>
        /// <param name="entityList"></param>
        /// <param name="callback"></param>
        public void UpdateVendorProduct(VendorProductListVM entityList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = entityList.ConvertVM<VendorProductListVM, VendorProductList>();
            restClient.Update("/ExternalSYSService/ExternalSYS/UpdateVendorProduct", data, callback);
        }
    }
}
