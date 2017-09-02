using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor.VendorPortal;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.ExternalSYS
{
    [VersionExport(typeof(VendorPortalManagerAppService))]
    public class VendorPortalManagerAppService
    {
        /// <summary>
        /// 获取下载模板地址
        /// </summary>
        /// <param name="c3SysNo">类别3的编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>下载的模板地址</returns>
        public string GetDownloadTemplateUrl(int c3SysNo,string companyCode)
        {
            if (c3SysNo > 0)
            {
                return ObjectFactory<DownTemplateProcessor>.Instance.Download(c3SysNo, companyCode);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 生效用户
        /// </summary>
        /// <param name="userSysNos">编号集合</param>
        public void PassUser(List<int> userSysNos)
        {
            ObjectFactory<UserProcessor>.Instance.BatchPass(userSysNos);
        }

        /// <summary>
        /// 作废用户
        /// </summary>
        /// <param name="userSysNos">编号集合</param>
        public void InvalidUser(List<int> userSysNos)
        {
            ObjectFactory<UserProcessor>.Instance.BatchInvaild(userSysNos);
        }

        /// <summary>
        /// 获取用户账号信息
        /// </summary>
        /// <param name="userSysNo">账号编号</param>
        /// <returns>账号信息</returns>
        public VendorUser GetUserInfo(int userSysNo)
        {
            return ObjectFactory<UserProcessor>.Instance.GetUserInfo(userSysNo);
        }


        public Role CreateRole(Role entity)
        {

            return ObjectFactory<RoleProcessor>.Instance.CreateRole(entity);

        }


        public Role UpdateRole(Role entity)
        {
            return ObjectFactory<RoleProcessor>.Instance.UpdateRole(entity);
 
        }

        public Role UpdateRoleStatus(Role entity)
        {
            return ObjectFactory<RoleProcessor>.Instance.UpdateStatus(entity);
 
        }


        public List<Role> UpdateRoleStatusBatch(List<Role> list)
        {
            List<Role> results = new List<Role>();

            if (list != null)
            {
                foreach(Role role in list)
                {
                    Role result=this.UpdateRoleStatus(role);
                    if (result != null)
                        results.Add(result);
                }

            }
            return results;
        }

        /// <summary>
        /// 根据供应商编号获取对应的代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<VendorAgentInfo> GetVendorAgentInfo(VendorInfo vendorInfo)
        {
            return ObjectFactory<UserProcessor>.Instance.GetVendorAgentInfo(vendorInfo);   
        }

        /// <summary>
        /// 创建VendorUser
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VendorUser CreateVendorUser(VendorUser entity)
        {
            return ObjectFactory<UserProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新VendorUser
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateVendorUser(VendorUser entity)
        {
            return ObjectFactory<UserProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 更新VendorUserRole
        /// </summary>
        /// <param name="entityList"></param>
        public void UpdateVendorUserRole(VendorUserRoleList entityList)
        {
            ObjectFactory<RoleProcessor>.Instance.UpdateVendorUserRoleList(entityList);
        }

        /// <summary>
        /// 更新VendorProduct
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateVendorProdut(VendorProductList entity)
        {
            return ObjectFactory<UserProcessor>.Instance.UpdateVendorProdut(entity);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(VendorPortalLog log)
        {
            ObjectFactory<VendorPortalLogProcessor>.Instance.WriteLog(log);
        }
    }
}
