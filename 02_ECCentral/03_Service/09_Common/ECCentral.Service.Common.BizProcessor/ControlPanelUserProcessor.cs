using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity;
using System.Transactions;
using ECCentral.Service.EventMessage.AuthCenter;
namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ControlPanelUserProcessor))]
    public class ControlPanelUserProcessor
    {
        public virtual ControlPanelUser GetControlPanelUserByLoginName(string loginName)
        {
            List<ControlPanelUser> resultList = ObjectFactory<IControlPanelUserDA>.Instance.GetUserByLoginName(loginName);
            if (resultList != null && resultList.Count == 0)
            {
                #region <如果在DB里没有根据LoginName找到数据，就到Keystone中查找>

                resultList = new List<ControlPanelUser>();

                var lookupUsers = GetKeyStoneUserByLoginName(loginName);

                if (lookupUsers != null && lookupUsers.Count > 0)
                {
                    foreach (var item in lookupUsers)
                    {
                        resultList.Add(new ControlPanelUser
                                       {
                                           LogicUserId = item.LogicUserId,
                                           SourceDirectory = item.SourceDirectory,
                                           PhysicalUserId = item.PhysicalUserId,
                                           LoginName = item.PhysicalUserName,
                                       });
                    }
                }
                #endregion
            }
            else
            {
                throw new BizException(string.Format("登陆名为:{0}的用户已存在", loginName));
            }

            if (resultList != null && resultList.Count == 1)
            {
                return resultList[0];
            }
            return null;
        }

        public virtual ControlPanelUser Create(BizEntity.Common.ControlPanelUser request)
        {
            ControlPanelUser result;
            TrimProperties(ref request);

            using (TransactionScope scope = new TransactionScope())
            {
                result = ObjectFactory<IControlPanelUserDA>.Instance.CreateUser(request);
                result.CompanyCode = request.CompanyCode;
                result.DepartmentName = request.DepartmentName;
                SynMappingAndSysUser(result);

                scope.Complete();
            }
            return result;
        }

        public virtual ControlPanelUser Update(BizEntity.Common.ControlPanelUser request)
        {
            ControlPanelUser result;
            TrimProperties(ref request);
            var keyStoneUser = GetKeyStoneUserByLoginName(request.LoginName);
            using (TransactionScope scope = new TransactionScope())
            {
                #region < 查询Keystone中数据，并进行赋值 >
                if (keyStoneUser != null && keyStoneUser.Count == 1)
                {
                    request.SourceDirectory = keyStoneUser[0].SourceDirectory;
                    request.LogicUserId = keyStoneUser[0].LogicUserId;
                    request.PhysicalUserId = keyStoneUser[0].PhysicalUserId;
                }
                #endregion

                result = ObjectFactory<IControlPanelUserDA>.Instance.UpdateUser(request);
                result.CompanyCode = request.CompanyCode;
                result.DepartmentName = request.DepartmentName;
                SynMappingAndSysUser(result);

                scope.Complete();
            }
            return result;
        }

        private dynamic GetKeyStoneUserByLoginName(string p)
        {
            KeystoneUserQueryMessage msg = new KeystoneUserQueryMessage()
            {
                 LoginName=p
            };
            EventPublisher.Publish<KeystoneUserQueryMessage>(msg);

            return msg.Result;
        }

        private void SynMappingAndSysUser(ControlPanelUser controlPanelUser)
        {
                int generateUserSysNo = 0;
                int mappingUserSysNo =ObjectFactory<ISynMappingAndSysUserDA>.Instance.GetExistUserSysNo(controlPanelUser);
                int sysUserNo = ObjectFactory<ISynMappingAndSysUserDA>.Instance.GetExistUserSysNoInOldData(controlPanelUser, mappingUserSysNo);

                if (sysUserNo == 0 && mappingUserSysNo == 0)
                {
                    generateUserSysNo = ObjectFactory<ISynMappingAndSysUserDA>.Instance.GenerateUserSysNo();
                }
                else
                {
                    generateUserSysNo = mappingUserSysNo == 0 ? sysUserNo : mappingUserSysNo;
                }

                ObjectFactory<ISynMappingAndSysUserDA>.Instance.SynUserMapping(controlPanelUser, mappingUserSysNo, generateUserSysNo);
                ObjectFactory<ISynMappingAndSysUserDA>.Instance.SynSysUser(controlPanelUser, sysUserNo, generateUserSysNo);
        }

        private void TrimProperties(ref ControlPanelUser entity)
        {
            entity.LoginName = entity.LoginName == null ? null : entity.LoginName.Trim();
            entity.SourceDirectory = entity.SourceDirectory == null ? null : entity.SourceDirectory.Trim();
            entity.DisplayName = entity.DisplayName == null ? null : entity.DisplayName.Trim();
            entity.DepartmentCode = entity.DepartmentCode == null ? null : entity.DepartmentCode.Trim();
            entity.PhoneNumber = entity.PhoneNumber == null ? null : entity.PhoneNumber.Trim();
            entity.EmailAddress = entity.EmailAddress == null ? null : entity.EmailAddress.Trim();
            entity.LogicUserId = entity.LogicUserId == null ? null : entity.LogicUserId.Trim();
            entity.PhysicalUserId = entity.PhysicalUserId == null ? null : entity.PhysicalUserId.Trim();
            entity.InUser = entity.InUser == null ? null : entity.InUser.Trim();
            entity.EditUser = entity.EditUser == null ? null : entity.EditUser.Trim();
            entity.CompanyCode = entity.CompanyCode == null ? null : entity.CompanyCode.Trim();
        }


        public ControlPanelUser GetUserBySysNo(int _sysNo)
        {
            return  ObjectFactory<IControlPanelUserDA>.Instance.GetUserBySysNo(_sysNo);
        }


        public int GetCPUsersLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<IControlPanelUserDA>.Instance.GetCPUsersLoginCount(request);
        }

        public virtual ControlPanelUser GetCPUsersLoginUser(string loginName)
        {
            List<ControlPanelUser> resultList = ObjectFactory<IControlPanelUserDA>.Instance.GetUserByLoginName(loginName);
            if (resultList != null && resultList.Count > 0)
            {
                return resultList[0];
            }
            return null;
        }
    }
}
