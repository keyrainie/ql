using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.WPMessage.IDataAccess;
using WPM = ECCentral.WPMessage.BizEntity;
using ECCentral.WPMessage.BizEntity;

namespace ECCentral.Service.WPMessage.BizProcessor
{
    [VersionExport(typeof(WPMessageProcessor))]
    public class WPMessageProcessor
    {
        private WPMessage.IDataAccess.IWPMessageDA _wpMessageDA;
        private WPMessage.IDataAccess.IWPMessageDA WPMessageDA
        {
            get
            {
                _wpMessageDA = _wpMessageDA ?? ObjectFactory<IWPMessageDA>.Instance;
                return _wpMessageDA;
            }
        }
        public void DoESBMessage(ESBMessage msg)
        {
            List<IESBMessageProcessor> processorList = WPMessageProcessorFactory.CreateProcessorList(msg);
            if (processorList != null && processorList.Count > 0)
            {
                foreach (IESBMessageProcessor processor in processorList)
                {
                    processor.Process(msg);
                }
            }
        }

        #region WPMessage相关处理

        public WPM.WPMessage AddWPMessage(int categorySysNo, string bizSysNo, string parameters, int userSysNo, string memo)
        {
            WPM.WPMessage msg = new WPM.WPMessage
            {
                CategorySysNo = categorySysNo,
                BizSysNo = bizSysNo,
                Parameters = parameters,
                CreateUserSysNo = userSysNo,
                Memo = memo
            };
            return WPMessageDA.AddWPMessage(msg);
        }
        public List<ECCentral.WPMessage.BizEntity.WPMessage> GetWPMessage(int categorySysNo, string bizSysNo)
        {
            return WPMessageDA.GetWPMessage(categorySysNo, bizSysNo);
        }
        public void ProcessWPMessage(int msgSysNo, int userSysNo)
        {
            WPMessageDA.UpdateWPMessageToProcessing(msgSysNo, userSysNo);
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo, string memo)
        {
            WPMessageDA.CompleteWPMessage(categorySysNo, bizSysNo, userSysNo, memo);
        }

        public void CompleteWPMessage(int categorySysNo, string bizSysNo, int userSysNo)
        {
            WPMessageDA.CompleteWPMessage(categorySysNo, bizSysNo, userSysNo);
        }

        public bool HasWPMessageByUserSysNo(int userSysNo)
        {
            return WPMessageDA.HasWPMessageByUserSysNo(userSysNo);
        }

        #endregion

        #region WPMessageCategory相关处理

        public List<WPMessageCategory> GetAllCategory()
        {
            return WPMessageDA.GetAllCategory();
        }
        public List<WPMessageCategory> GetCategoryByUserSysNo(int userSysNo)
        {
            return WPMessageDA.GetCategoryByUserSysNo(userSysNo);
        }

        public void UpdateCategoryRoleByCategorySysNo(int categorySysNo, List<int> roleSysNoList)
        {
            WPMessageDA.UpdateCategoryRoleByCategorySysNo(categorySysNo, roleSysNoList);
        }

        public List<int> GetRoleSysNoByCategorySysNo(int categorySysNo)
        {
            return WPMessageDA.GetRoleSysNoByCategorySysNo(categorySysNo);
        }

        public void UpdateCategoryRoleByRoleSysNo(int roleSysNo, List<int> categorySysNoList)
        {
            WPMessageDA.UpdateCategoryRoleByRoleSysNo(roleSysNo, categorySysNoList);
        }

        public List<int> GetCategorySysNoByRoleSysNo(int roleSysNo)
        {
            return WPMessageDA.GetCategorySysNoByRoleSysNo(roleSysNo);
        }
        #endregion

    }
}
