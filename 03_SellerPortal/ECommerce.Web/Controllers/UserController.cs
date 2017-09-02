using ECommerce.Entity.ControlPannel;
using ECommerce.Enums;
using ECommerce.Service.ControlPannel;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Service.ControlPannel;
using ECommerce.Service.RMA;
using System.Text;
using System.Security.Cryptography;

namespace ECommerce.Web.Controllers
{
    public class UserController : SSLControllerBase
    {
        //
        // GET: /User/


        #region 角色管理
        
        public ActionResult Role()
        {
            return View();
        }

        
        public ActionResult RoleMgt()
        {
            int roleSysNo = int.Parse(Request["roleSysNo"]);
            string Name = Request["name"];
            if (roleSysNo > 0)
            {
                ViewBag.RoleInfo = UserService.GetRoleInfo(roleSysNo);
            }
            if (ViewBag.RoleInfo == null)
            {
                ViewBag.RoleInfo = new RoleInfo();
            }
            List<PrivilegeInfo> all = UserService.GetPrivilegeList();
            List<PrivilegeInfo> curentRoles = UserService.GetPrivilegeListByRoleSysNo(roleSysNo);
            ViewBag.All = TransferToNodes(all);
            foreach (var item in curentRoles)
            {
                ViewBag.RolesPrivilege += item.SysNo + ",";
            }
            if (ViewBag.RolesPrivilege != null)
            {
                ViewBag.RolesPrivilege = ViewBag.RolesPrivilege.TrimEnd(',');
            }
            return View();
        }

        private List<PrivilegeInfo> TransferToNodes(List<PrivilegeInfo> all)
        {
            if (all != null && all.Count > 0)
            {
                int parentsysno = all.Min(p => p.ParentSysNo.Value);
                List<PrivilegeInfo> result = new List<Entity.ControlPannel.PrivilegeInfo>();
                foreach (var item in all.Where(p => p.ParentSysNo == parentsysno))
                {
                    GetChild(item, all);
                    result.Add(item);
                }
                return result;
            }
            else
            {
                return new List<Entity.ControlPannel.PrivilegeInfo>();
            }
        }
        private void GetChild(PrivilegeInfo current, List<PrivilegeInfo> all)
        {
            var children = all.Where(p => p.ParentSysNo == current.SysNo).ToList();
            foreach (var item in children)
            {
                if (current.Children == null)
                {
                    current.Children = new List<Entity.ControlPannel.PrivilegeInfo>();
                }
                current.Children.Add(item);
                GetChild(item, all);
            }
        }

        public JsonResult CreateRole(RoleInfo info)
        {

            foreach (var item in Request["PrivilegeSysNoList"].Split(','))
            {
                info.PrivilegeSysNoList.Add(int.Parse(item));
            }
            UserAuthHelper.SetBizEntityUserInfo(info, true);
            UserAuthHelper.SetBizEntityUserInfo(info, false);//ecc 搞笑得行，edituser必填
            UserService.CreateRole(info);
            return new JsonResult() { };
        }

        public ActionResult UpdateRole(RoleInfo info)
        {
            foreach (var item in Request["PrivilegeSysNoList"].Split(','))
            {
                info.PrivilegeSysNoList.Add(int.Parse(item));
            }
            UserAuthHelper.SetBizEntityUserInfo(info, false);
            UserService.UpdateRole(info);
            return new JsonResult() { };
        }


        public ActionResult UpdateRoleStatus(List<RoleInfo> list)
        {
            list = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<RoleInfo>>(Request.Params[0]);
            list.ForEach((item) => { UserAuthHelper.SetBizEntityUserInfo(item, false); });
            UserService.BathUpdateRoleStatus(list);
            return new JsonResult();
        }

        public ActionResult QueryRoleData()
        {
            RoleQueryFilter filter = BuildQueryFilterEntity<RoleQueryFilter>();
            filter.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = UserService.QueryRole(filter);
            return AjaxGridJson(result);
        }

        #endregion

        #region 用户管理
        
        public ActionResult Query()
        {
            return View();
        }

        
        public ActionResult UserMgt()
        {
            int usersysno = int.Parse(Request["usersysno"]);
            RoleQueryFilter filter = new Entity.ControlPannel.RoleQueryFilter();
            filter.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            filter.PageIndex = 0;
            filter.PageSize = 1000;
            filter.Status = RoleStatus.Active;
            ViewBag.AllRoles = UserService.QueryRoleDatatable(filter);
            if (usersysno > 0)
            {
                ViewBag.UserInfo = UserService.GetUserInfo(usersysno, filter.VendorSysNo);
            }
            else
            {
                ViewBag.UserInfo = new UserInfo() { SysNo = 0 };
            }
            return View();
        }

        public JsonResult CreateUser(UserInfo info)
        {
            info.Roles = new List<Entity.ControlPannel.UsersRoleInfo>();
            if (Request["Roles"] != "")
            {
                foreach (var item in Request["Roles"].Split(','))
                {
                    info.Roles.Add(new UsersRoleInfo() { RoleSysNo = int.Parse(item) });
                }
            }
            info.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            UserAuthHelper.SetBizEntityUserInfo(info, true);
            UserAuthHelper.SetBizEntityUserInfo(info, false);
            info.Pwd = UserAuthHelper.EncryptPassword(info.InputPwd);
            UserService.CreateUser(info);
            return new JsonResult() { };
        }

        public ActionResult UpdateUser(UserInfo info)
        {
            info.Roles = new List<Entity.ControlPannel.UsersRoleInfo>();
            if (Request["Roles"] != "")
            {
                foreach (var item in Request["Roles"].Split(','))
                {
                    info.Roles.Add(new UsersRoleInfo() { RoleSysNo = int.Parse(item), UserSysNo = info.SysNo });
                }
            }
            info.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            UserAuthHelper.SetBizEntityUserInfo(info, false);
            info.Pwd = UserAuthHelper.EncryptPassword(info.InputPwd);
            UserService.UpdateUser(info);
            return new JsonResult() { };
        }


        public ActionResult UpdateUserStatus(List<UserInfo> list)
        {
            list = ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<UserInfo>>(Request.Params[0]);
            list.ForEach((item) => { UserAuthHelper.SetBizEntityUserInfo(item, false); });
            UserService.UpdateUserStatus(list.Select(p => p.SysNo.Value).ToList(), list[0].Status, UserAuthHelper.GetCurrentUser().UserDisplayName);
            return new JsonResult();
        }

        public ActionResult QueryUserData()
        {
            UserQueryFilter filter = BuildQueryFilterEntity<UserQueryFilter>();
            filter.ManufacturerSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var result = UserService.QueryUser(filter);
            return AjaxGridJson(result);
        }
        #endregion

        #region 修改密码
        
        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult ChangePasswordData(string pwd, string old)
        {
            if (pwd == old)
            {
                throw new BusinessException("新密码和原密码相同，请重新输入!");
            }

            UserInfo info = UserService.GetUserInfo(UserAuthHelper.GetCurrentUser().UserSysNo, UserAuthHelper.GetCurrentUser().SellerSysNo);
            if (info.Pwd != UserAuthHelper.EncryptPassword(old))
            {
                throw new BusinessException("原密码不正确!");
            }
            info.Pwd = UserAuthHelper.EncryptPassword(pwd);
            info.VendorSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            UserAuthHelper.SetBizEntityUserInfo(info, false);
            UserService.UpdateUserPassword(info);
            return new JsonResult() { };
        }



        #endregion

        #region 二级域名管理
        
        public ActionResult SecondDomain()
        {
            return View();
        }

        public ActionResult SetSecondDomain()
        {
            SecondDomainInfo secondDomain = SerializationUtility.JsonDeserialize<SecondDomainInfo>(Request.Form["data"]);
            secondDomain.SecondDomainStatus = SecondDomainStatus.ToAudit;
            secondDomain.SysNo = UserAuthHelper.GetCurrentUser().SellerSysNo;
            var success = UserService.SetSecondDomain(secondDomain);
            return Json(new { Success = success, Msg = success ? LanguageHelper.GetText("操作成功") : LanguageHelper.GetText("操作失败，请稍候再试") });
        }

        #endregion
    }
}
