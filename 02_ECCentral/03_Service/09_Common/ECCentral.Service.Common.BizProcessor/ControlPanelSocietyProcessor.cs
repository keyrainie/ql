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
    [VersionExport(typeof(ControlPanelSocietyProcessor))]
    public class ControlPanelSocietyProcessor
    {
        public virtual ControlPanelSociety GetControlPanelSocietyByLoginName(string loginName)
        {
            List<ControlPanelSociety> resultList = ObjectFactory<IControlPanelSocietyDA>.Instance.GetSocietyByLoginName(loginName);
            if (resultList != null && resultList.Count == 0)
            {
                #region <如果在DB里没有根据LoginName找到数据，就到Keystone中查找>

                resultList = new List<ControlPanelSociety>();

                var lookupUsers = GetKeyStoneUserByLoginName(loginName);

                if (lookupUsers != null && lookupUsers.Count > 0)
                {
                    foreach (var item in lookupUsers)
                    {
                        resultList.Add(new ControlPanelSociety
                        {
                            //LogicUserId = item.LogicUserId,
                            //SourceDirectory = item.SourceDirectory,
                            //PhysicalUserId = item.PhysicalUserId,
                            //LoginName = item.PhysicalUserName,
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

        public virtual ControlPanelSociety Create(BizEntity.Common.ControlPanelSociety request)
        {
            ControlPanelSociety result;
            TrimProperties(ref request);

            using (TransactionScope scope = new TransactionScope())
            {
                result = ObjectFactory<IControlPanelSocietyDA>.Instance.CreateSociety(request);
                //result.CompanyCode = request.CompanyCode;
                //result.DepartmentName = request.DepartmentName;
                //SynMappingAndSysUser(result);
                scope.Complete();
            }
            return result;
        }

        public virtual ControlPanelSociety Update(BizEntity.Common.ControlPanelSociety request)
        {
            ControlPanelSociety result;
            TrimProperties(ref request);
            var keyStoneUser = GetKeyStoneUserByLoginName(request.OrganizationName);
            using (TransactionScope scope = new TransactionScope())
            {
                #region < 查询Keystone中数据，并进行赋值 >
                if (keyStoneUser != null && keyStoneUser.Count == 1)
                {
                    //request.SourceDirectory = keyStoneUser[0].SourceDirectory;
                    //request.LogicUserId = keyStoneUser[0].LogicUserId;
                    //request.PhysicalUserId = keyStoneUser[0].PhysicalUserId;
                }
                #endregion

                result = ObjectFactory<IControlPanelSocietyDA>.Instance.UpdateSociety(request);
                //result.CompanyCode = request.CompanyCode;
                //result.DepartmentName = request.DepartmentName;
                SynMappingAndSysUser(result);

                scope.Complete();
            }
            return result;
        }

        private dynamic GetKeyStoneUserByLoginName(string p)
        {
            KeystoneUserQueryMessage msg = new KeystoneUserQueryMessage()
            {
                LoginName = p
            };
            EventPublisher.Publish<KeystoneUserQueryMessage>(msg);

            return msg.Result;
        }

        private void SynMappingAndSysUser(ControlPanelSociety controlPanelSociety)
        {
            int generateSocietySysNo = 0;
            int mappingSocietySysNo = ObjectFactory<ISynMappingAndSysSocietyDA>.Instance.GetExistSocietySysNo(controlPanelSociety);
            int sysUserNo = ObjectFactory<ISynMappingAndSysSocietyDA>.Instance.GetExistSocietySysNoInOldData(controlPanelSociety, mappingSocietySysNo);

            if (sysUserNo == 0 && mappingSocietySysNo == 0)
            {
                generateSocietySysNo = ObjectFactory<ISynMappingAndSysSocietyDA>.Instance.GenerateSocietySysNo();
            }
            else
            {
                generateSocietySysNo = mappingSocietySysNo == 0 ? sysUserNo : mappingSocietySysNo;
            }

            ObjectFactory<ISynMappingAndSysSocietyDA>.Instance.SynSocietyMapping(controlPanelSociety, mappingSocietySysNo, generateSocietySysNo);
            ObjectFactory<ISynMappingAndSysSocietyDA>.Instance.SynSysSociety(controlPanelSociety, sysUserNo, generateSocietySysNo);
        }

        private void TrimProperties(ref ControlPanelSociety entity)
        {
            //entity.LoginName = entity.LoginName == null ? null : entity.LoginName.Trim();
            //entity.SourceDirectory = entity.SourceDirectory == null ? null : entity.SourceDirectory.Trim();
            //entity.DisplayName = entity.DisplayName == null ? null : entity.DisplayName.Trim();
            //entity.DepartmentCode = entity.DepartmentCode == null ? null : entity.DepartmentCode.Trim();
            //entity.PhoneNumber = entity.PhoneNumber == null ? null : entity.PhoneNumber.Trim();
            //entity.EmailAddress = entity.EmailAddress == null ? null : entity.EmailAddress.Trim();
            //entity.LogicUserId = entity.LogicUserId == null ? null : entity.LogicUserId.Trim();
            //entity.PhysicalUserId = entity.PhysicalUserId == null ? null : entity.PhysicalUserId.Trim();
            //entity.InUser = entity.InUser == null ? null : entity.InUser.Trim();
            //entity.EditUser = entity.EditUser == null ? null : entity.EditUser.Trim();
            //entity.CompanyCode = entity.CompanyCode == null ? null : entity.CompanyCode.Trim();

            entity.Province = string.IsNullOrWhiteSpace(entity.Province) ? null : entity.Province.Trim();
            entity.OrganizationName = string.IsNullOrWhiteSpace(entity.OrganizationName) ? null : entity.OrganizationName.Trim();
        }


        public ControlPanelSociety GetSocietyBySysNo(int _sysNo)
        {
            return ObjectFactory<IControlPanelSocietyDA>.Instance.GetSocietyBySysNo(_sysNo);
        }


        public int GetCPSocietysLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<IControlPanelSocietyDA>.Instance.GetCPSocietysLoginCount(request);
        }

        public virtual ControlPanelSociety GetCPSocietysLoginSociety(string loginName)
        {
            List<ControlPanelSociety> resultList = ObjectFactory<IControlPanelSocietyDA>.Instance.GetSocietyByLoginName(loginName);
            if (resultList != null && resultList.Count > 0)
            {
                return resultList[0];
            }
            return null;
        }

        public virtual List<ComBoxData> GetSocietyProvince_ComBox(string loginName)
        {
            List<ComBoxData> resultList = ObjectFactory<IControlPanelSocietyDA>.Instance.GetSocietyProvince_ComBox(loginName);
            if (resultList != null && resultList.Count > 0)
            {
                return resultList;
            }
            return null;
        }
        public virtual List<ComBoxData> GetSocietyCommissionType_ComBox(string loginName)
        {
            List<ComBoxData> resultList = ObjectFactory<IControlPanelSocietyDA>.Instance.GetSocietyCommissionType_ComBox(loginName);
            if (resultList != null && resultList.Count > 0)
            {
                return resultList;
            }
            return null;
        }
    }
}
