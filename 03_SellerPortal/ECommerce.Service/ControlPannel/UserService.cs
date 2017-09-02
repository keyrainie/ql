using ECommerce.DataAccess.ControlPannel;
using ECommerce.Entity.ControlPannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Enums;
using ECommerce.Entity.Common;
using System.Security.Cryptography;

namespace ECommerce.Service.ControlPannel
{
    public class UserService
    {
        /// <summary>
        /// 获取商家二级域名
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static SecondDomainInfo LoadSecondDomain(int sellerSysNo)
        {
            return UserDA.LoadSecondDomain(sellerSysNo);
        }
        /// <summary>
        /// 设置商家二级域名
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <param name="secondDomain"></param>
        /// <returns></returns>
        public static bool SetSecondDomain(SecondDomainInfo secondDomain)
        {
            //判断是否存在，并且是待审核状态，才能设置
            SecondDomainInfo old = LoadSecondDomain(secondDomain.SysNo);
            if (old != null && old.SecondDomainStatus.HasValue && (old.SecondDomainStatus.Value == SecondDomainStatus.PassAudit || old.SecondDomainStatus.Value == SecondDomainStatus.Online))
            {
                string msg = string.Format("操作失败，当前状态为{0}", old.SecondDomainStatus.Value.GetDescription());
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(secondDomain.SecondDomain))
            {
                string msg = LanguageHelper.GetText("二级域名不能为空!");
                throw new BusinessException(msg);
            }
            var forbiddenSD= AppSettingManager.GetSetting("Store", "ForbiddenSecondDomain").Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            if (forbiddenSD.ToList<string>().Exists(f => f.Trim().ToLower() == secondDomain.SecondDomain))
            {
                string msg = LanguageHelper.GetText("该二级域名为平台专用，不可申请!");
                throw new BusinessException(msg);
            }


            secondDomain.SecondDomainStatus = SecondDomainStatus.ToAudit;
            secondDomain.SecondDomain = secondDomain.SecondDomain.ToLower();
            var status = UserDA.SetSecondDomain(secondDomain);
            if (status == -1)
            {
                string msg = LanguageHelper.GetText("二级域名已被使用!");
                throw new BusinessException(msg);
            }
            return status == 1;
        }

        public static QueryResult QueryUser(UserQueryFilter filter)
        {
            int count = 0;
            return new QueryResult(UserDA.UserQuery(filter, out count), filter, count);
        }

        public static QueryResult QueryRole(RoleQueryFilter filter)
        {
            int count = 0;
            return new QueryResult(UserDA.QueryRole(filter, out count), filter, count);
        }

        public static dynamic QueryRoleDatatable(RoleQueryFilter filter)
        {
            int count = 0;
            return UserDA.QueryRole(filter, out count);
        }

        public static List<PrivilegeInfo> GetPrivilegeList()
        {
            return UserDA.GetPrivilegeList();
        }

        public static List<PrivilegeInfo> GetPrivilegeListByRoleSysNo(int roleSysNo)
        {
            return UserDA.GetPrivilegeListByRoleSysNo(roleSysNo);
        }

        public static void CreateRole(RoleInfo info)
        {
            if (string.IsNullOrEmpty(info.RoleName))
            {
                throw new BusinessException("角色名为空");
            }
            if (UserDA.RoleNameIsExist(info.RoleName, 0, info.SellerSysNo.Value) == 0)
            {
                using (ITransaction transaction = TransactionManager.Create())
                {

                    info = UserDA.CreateRole(info);
                    foreach (var item in info.PrivilegeSysNoList)
                    {
                        UserDA.InsertRolePrivilege(info.SysNo, item);
                    }
                    transaction.Complete();
                }
            }
            else
            {
                throw new BusinessException("已存在相同的角色名");
            }
        }

        public static void UpdateRole(RoleInfo info)
        {
            if (string.IsNullOrEmpty(info.RoleName))
            {
                throw new BusinessException("角色名为空");
            }
            if (UserDA.RoleNameIsExist(info.RoleName, info.SysNo, info.SellerSysNo.Value) == 0)
            {
                using (ITransaction transaction = TransactionManager.Create())
                {
                    UserDA.DeleteRolePrivilege(info.SysNo);
                    UserDA.UpdateRole(info);
                    foreach (var item in info.PrivilegeSysNoList)
                    {
                        UserDA.InsertRolePrivilege(info.SysNo, item);
                    }
                    transaction.Complete();
                }
            }
            else
            {
                throw new BusinessException("已存在相同的角色名");
            }

        }

        public static void BathUpdateRoleStatus(List<RoleInfo> list)
        {
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (var item in list)
                {
                    UserDA.UpdateRoleStatus(item);
                }
                transaction.Complete();
            }
        }

        public static RoleInfo GetRoleInfo(int roleSysNo)
        {
            return UserDA.GetRoleInfo(roleSysNo);
        }

        public static void CreateUser(UserInfo user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserID))
            {
                throw new BusinessException("输入不正确");
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                throw new BusinessException("用户名不能为空");
            }
            if (UserDA.CountUserID(user.UserID, 0) > 0)
            {
                throw new BusinessException("登录用户名重复，请重新输入");
            }


            user.UserNum = UserDA.CountVendorNum(user.VendorSysNo.Value) + 1;
            using (ITransaction transaction = TransactionManager.Create())
            {
                UserDA.InsertVendorUser(user);
                foreach (var item in user.Roles)
                {
                    item.UserSysNo = user.SysNo;
                    UserDA.InsertVendorUser_User_Role(item);
                }
                transaction.Complete();
            }
        }

        public static void UpdateUser(UserInfo user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserID))
            {
                throw new BusinessException("输入不正确");
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                throw new BusinessException("用户名不能为空");
            }
            if (UserDA.CountUserID(user.UserID, user.SysNo.Value) > 0)
            {
                throw new BusinessException("登录用户名重复，请重新输入");
            }

            using (ITransaction transaction = TransactionManager.Create())
            {
                UserDA.UpdateVendorUser(user);
                UserDA.DeleteVendorUser_User_Role(user.SysNo.Value);
                foreach (var item in user.Roles)
                {
                    item.UserSysNo = user.SysNo;
                    UserDA.InsertVendorUser_User_Role(item);
                }
                transaction.Complete();
            }
        }
        public static void UpdateUserPassword(UserInfo user)
        {
            UserDA.UpdateUserPassword(user);
        }
        public static void UpdateUserStatus(List<int> sysNos, UserStatus status, string editUser)
        {
            UserDA.UpdateVendorUserStatus(sysNos, status, editUser);
        }

        public static UserInfo GetUserInfo(int sysno, int sellersysno)
        {
            UserInfo userinfo = UserDA.GetUserBySysNo(sysno, sellersysno);
            userinfo.Roles = UserDA.GetRolesByUserSysNo(sysno);
            return userinfo;
        }
    }
}
