using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.ExternalSYS.BizProcessor.VendorPortal
{
    [VersionExport(typeof(RoleProcessor))]
    public class RoleProcessor
    {
        IVendorRoleDA m_da;

        public RoleProcessor()
        {
            this.m_da = ObjectFactory<IVendorRoleDA>.Instance;
        }


        public Role CreateRole(Role entity)
        {
            if (string.IsNullOrEmpty(entity.RoleName))
            {
                throw new BizException("RoleName is Empty!");
            }
            if (m_da.RoleNameIsExist(entity.RoleName, 0) > 0)
            {
                throw new BizException("RoleName is Exist!");
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required, options))
            {
                //entity.InUser= ExternalDomainBroker.GetUserBySysNo(ServiceContext.Current.UserSysNo);
                entity = m_da.CreateRole(entity);
                RolePrivilegeAdd(entity);

                scope.Complete();
            }
            return entity;
        }

        private void RolePrivilegeClearByRoleSysNo(int RoleSysNo)
        {
            m_da.DeleteRolePrivilege(RoleSysNo);
        }

        private void RolePrivilegeAdd(Role entity)
        {
            if (entity.PrivilegeSysNoList != null && entity.PrivilegeSysNoList.Count > 0)
            {
                foreach (var privilegeSysNo in entity.PrivilegeSysNoList)
                {
                    m_da.CreateRolePrivilege(new RolePrivilege()
                    {
                        RoleSysNo = entity.SysNo.Value,
                        PrivilegeSysNo = privilegeSysNo
                    });
                }
            }
        }

        public Role UpdateRole(Role entity)
        {
            if (string.IsNullOrEmpty(entity.RoleName))
            {
                throw new BizException("RoleName is Empty!");
            }
            if (m_da.RoleNameIsExist(entity.RoleName, entity.SysNo.Value) > 0)
            {
                throw new BizException("RoleName is Exist!");
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required, options))
            {
                entity = m_da.UpdateRole(entity);
                RolePrivilegeClearByRoleSysNo(entity.SysNo.Value);
                RolePrivilegeAdd(entity);

                scope.Complete();
            }
            return entity;
        }

        public Role UpdateStatus(Role entity)
        {
            return m_da.UpdateRole(entity);
        }

        /// <summary>
        /// 更新VendorUserRole
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateVendorUserRoleList(VendorUserRoleList entityList)
        {
            VendorUserRole entity = new VendorUserRole()
            {
                VendorSysNo = entityList.VendorSysNo,
                ManufacturerSysNo = entityList.ManufacturerSysNo,
                UserSysNo = entityList.UserSysNo
            };
            if (entity.ManufacturerSysNo.HasValue)
            {
                m_da.DeleteVendorUser_RoleMapping(entity);
            }
            else
            {
                m_da.DeleteVendorUser_User_Role(entity.UserSysNo.Value);
            }
            foreach (var item in entityList.RoleSysNoList)
            {
                VendorUserRole entityNew = new VendorUserRole()
                {
                    RoleSysNo = item,
                    ManufacturerSysNo = entity.ManufacturerSysNo,
                    UserSysNo = entity.UserSysNo,
                };
                try
                {
                    if (entity.ManufacturerSysNo.HasValue)
                    {
                        m_da.InsertVendorUser_RoleMapping(entityNew);
                    }
                    else
                    {
                        m_da.InsertVendorUser_User_Role(entityNew);
                    }
                }
                catch (Exception e)
                {
                    throw new BizException("UpdateVendorUserRoleList Error:" + e);
                }
            }
        }

    }
}
