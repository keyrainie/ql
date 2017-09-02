using System.ServiceModel.Web;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="entity">订单信息</param>
        /// <returns>创建成功后的订单信息</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetDownloadTemplateUrl/{c3SysNo}/{companyCode}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public string GetDownloadTemplateUrl(string c3SysNo, string companyCode)
        {
            int sysNo = int.TryParse(c3SysNo, out sysNo) ? sysNo : 0;
            string result = ObjectFactory<VendorPortalManagerAppService>.Instance.GetDownloadTemplateUrl(sysNo, companyCode);
            return result;
        }

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryVendorRole", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorRole(VendorRoleQueryFilter filter)
        {
            return QueryList<VendorRoleQueryFilter>(filter, ObjectFactory<IVendorRoleDA>.Instance.RoleQuery);
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryUser", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryUser(VendorUserQueryFilter filter)
        {
            return QueryList<VendorUserQueryFilter>(filter, ObjectFactory<IVendorUserDA>.Instance.UserQuery);
        }

        /// <summary>
        /// 生效用户
        /// </summary>
        /// <param name="sysNos">请求编号集合</param>
        [WebInvoke(UriTemplate = "/ExternalSYS/PassUser", Method = "PUT")]
        public void PassUser(List<int> sysNos)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.PassUser(sysNos);
        }

        /// <summary>
        /// 作废用户
        /// </summary>
        /// <param name="sysNos">请求编号集合</param>
        [WebInvoke(UriTemplate = "/ExternalSYS/InvalidUser", Method = "PUT")]
        public void InvalidUser(List<int> sysNos)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.InvalidUser(sysNos);
        }

        /// <summary>
        /// 获取用户账号信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns>账号信息</returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetUserInfo/{id}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public VendorUser GetUserInfo(string id)
        {
            int sysNo = int.TryParse(id, out sysNo) ? sysNo : 0;
            return ObjectFactory<VendorPortalManagerAppService>.Instance.GetUserInfo(sysNo);
        }

        /// <summary>
        /// 权限列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetPrivilegeList", Method = "GET")]
        public List<PrivilegeEntity> GetPrivilegeList()
        {
            return ObjectFactory<IVendorRoleDA>.Instance.GetPrivilegeList();
        }

        [WebInvoke(UriTemplate = "/ExternalSYS/GetPrivilegeListByRoleSysNo/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<PrivilegeEntity> GetPrivilegeListByRoleSysNo(string sysNo)
        {
            int id = int.TryParse(sysNo, out id) ? id : 0;
            return ObjectFactory<IVendorRoleDA>.Instance.GetPrivilegeListByRoleSysNo(id);
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/CreateRole", Method = "POST")]
        public void CreateRole(Role entity)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.CreateRole(entity);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateRole", Method = "PUT")]
        public void UpdateRole(Role entity)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateRole(entity);
        }
        /// <summary>
        /// 更新角色状态
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateRoleStatus", Method = "PUT")]
        public void UpdateRoleStatus(Role entity)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateRoleStatus(entity);
        }

        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateRoleStatusBatch", Method = "PUT")]
        public void UpdateRoleStatusBatch(List<Role> entity)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateRoleStatusBatch(entity);
        }

        /// <summary>
        /// 加载供应商代理信息
        /// </summary>
        /// <param name="vendorInfo">供应商实体</param>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetVendorAgentInfoList/{VendorSysNo}", Method = "GET")]
        public List<VendorAgentInfo> GetVendorAgentInfo(string vendorSysNo)
        {
            int sysNo = int.TryParse(vendorSysNo, out sysNo) ? sysNo : 0;
            VendorInfo vendorInfo = new VendorInfo()
            {
                SysNo = sysNo
            };
            return ObjectFactory<VendorPortalManagerAppService>.Instance.GetVendorAgentInfo(vendorInfo);
        }

        /// <summary>
        /// 根据厂家编号和用户编号获取角色列表
        /// </summary>
        /// <param name="ManufacturerSysNo">厂家编号</param>
        /// <param name="UserSysNo">用户编号</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetRoleListByVendorEx/{ManufactureSysNo}/{UserSysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public List<VendorUserMapping> GetRoleListByVendorEx(string ManufactureSysNo, string UserSysNo)
        {
            int manufacturerSysNo = int.TryParse(ManufactureSysNo, out manufacturerSysNo) ? manufacturerSysNo : 0;
            int userSysNo = int.TryParse(UserSysNo, out userSysNo) ? userSysNo : 0;
            return ObjectFactory<IVendorRoleDA>.Instance.GetRoleListByVendorEx(manufacturerSysNo, userSysNo);
        }

        /// <summary>
        /// 根据用户编号获取角色列表
        /// </summary>
        /// <param name="UserSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/GetRoleListByUserSysNo/{UserSysNo}", Method = "GET")]
        public List<VendorUserMapping> GetRoleListByUserSysNo(string UserSysNo)
        {
            int userSysNo = int.TryParse(UserSysNo, out userSysNo) ? userSysNo : 0;
            return ObjectFactory<IVendorRoleDA>.Instance.GetRoleListByUserSysNo(userSysNo);
        }

        /// <summary>
        /// 创建VendorUser
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/CreateVendorUser", Method = "POST")]
        public VendorUser CreateVendorUser(VendorUser entity)
        {
            return ObjectFactory<VendorPortalManagerAppService>.Instance.CreateVendorUser(entity);
        }

        /// <summary>
        /// 编辑VendorUser
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateVendorUser", Method = "PUT")]
        public bool UpdateVendorUser(VendorUser entity)
        {
            return ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateVendorUser(entity);
        }

        /// <summary>
        /// 更新VendorUserRole
        /// </summary>
        /// <param name="entityList"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateVendorUserRole", Method = "PUT")]
        public void UpdateVendorUserRole(VendorUserRoleList entityList)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateVendorUserRole(entityList);
        }

        /// <summary>
        /// 查询VendorProdcut
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExternalSYS/QueryVendorProduct", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendroProduct(VendorProductQueryFilter filter)
        {
            return QueryList<VendorProductQueryFilter>(filter, ObjectFactory<IVendorUserDA>.Instance.VendorProductQuery);
        }

        [WebInvoke(UriTemplate = "/ExternalSYS/GetIsAuto", Method = "POST")]
        public int GetIsAuto(VendorProductQueryFilter filter)
        {
            return ObjectFactory<IVendorUserDA>.Instance.GetIsAuto(filter);
        }

        [WebInvoke(UriTemplate = "/ExternalSYS/QueryByStockShippingeInvoice", Method = "POST")]
        public List<Vendor_ExInfo> QueryByStockShippingeInvoic(Vendor_ExInfo query)
        {
            return ObjectFactory<IVendorUserDA>.Instance.QueryByStockShippingeInvoic(query);
        }

        /// <summary>
        /// 更新VendorProdut
        /// </summary>
        /// <param name="entityList"></param>
        [WebInvoke(UriTemplate = "/ExternalSYS/UpdateVendorProduct", Method = "PUT")]
        public void UpdateVendorProcut(VendorProductList entityList)
        {
            ObjectFactory<VendorPortalManagerAppService>.Instance.UpdateVendorProdut(entityList);
        }
    }
}
