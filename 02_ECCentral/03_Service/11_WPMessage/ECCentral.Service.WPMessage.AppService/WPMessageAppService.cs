using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using WPM = ECCentral.WPMessage.BizEntity;
using ECCentral.WPMessage.BizEntity;
using ECCentral.Service.WPMessage.BizProcessor;

namespace ECCentral.Service.WPMessage.AppService
{
    [VersionExport(typeof(WPMessageAppService))]
    public class WPMessageAppService
    {
        private WPMessageProcessor _wpMessageProcessor;
        private WPMessageProcessor WPMessageProcessor
        {
            get
            {
                _wpMessageProcessor = _wpMessageProcessor ?? ObjectFactory<WPMessageProcessor>.Instance;
                return _wpMessageProcessor;
            }
        }

        public void DoESBMessage(ESBMessage msg)
        {
            WPMessageProcessor.DoESBMessage(msg);
        }
        #region WPMessage相关处理

        public WPM.WPMessage AddWPMessage(int categorySysNo, string bizSysNo, string parameters, int userSysNo, string memo)
        {
            return WPMessageProcessor.AddWPMessage(categorySysNo, bizSysNo, parameters, userSysNo, memo);
        }

        public void ProcessWPMessage(int msgSysNo, int userSysNo)
        {
            WPMessageProcessor.ProcessWPMessage(msgSysNo, userSysNo);
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo, string memo)
        {
            WPMessageProcessor.CompleteWPMessage(categorySysNo, bizSysNo, userSysNo, memo);
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo)
        {
            WPMessageProcessor.CompleteWPMessage(categorySysNo, bizSysNo, userSysNo);
        }
        public bool HasWPMessageByUserSysNo(int userSysNo)
        {
            return WPMessageProcessor.HasWPMessageByUserSysNo(userSysNo);
        }

        #endregion

        #region WPMessageCategory相关处理

        public List<WPMessageCategory> GetAllCategory()
        {
            return WPMessageProcessor.GetAllCategory();
        }
        public List<WPMessageCategory> GetCategoryByUserSysNo(int userSysNo)
        {
            return WPMessageProcessor.GetCategoryByUserSysNo(userSysNo);
        }

        public void UpdateCategoryRoleByCategorySysNo(int categorySysNo, List<int> roleSysNo)
        {
            WPMessageProcessor.UpdateCategoryRoleByCategorySysNo(categorySysNo, roleSysNo);
        }

        public List<int> GetRoleSysNoByCategorySysNo(int categorySysNo)
        {
            return WPMessageProcessor.GetRoleSysNoByCategorySysNo(categorySysNo);
        }

        public void UpdateCategoryRoleByRoleSysNo(int roleSysNo, List<int> categorySysNoList)
        {
            WPMessageProcessor.UpdateCategoryRoleByRoleSysNo(roleSysNo, categorySysNoList);
        }

        public List<int> GetCategorySysNoByRoleSysNo(int roleSysNo)
        {
            return WPMessageProcessor.GetCategorySysNoByRoleSysNo(roleSysNo);
        }
        #endregion

    }
}
