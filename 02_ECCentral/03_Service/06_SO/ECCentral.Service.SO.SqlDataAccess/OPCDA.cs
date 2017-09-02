using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(IOPCDA))]
    public class OPCDA : IOPCDA
    {
        #region OPCOfflineInfo
        public bool SOIsProcess(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_GetOPCMasterLast");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<int>() > 0;
        }

        public int InsertOPCOfflineInfo(OPCOfflineInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_OPCOfflineMaster");
            command.SetParameterValue(info);
            command.SetParameterValue("@CallBackService", ((int)info.CallBackService).ToString());
            command.SetParameterValue("@NeedResponse", info.NeedResponse ? "Y" : "N");
            return command.ExecuteScalar<int>();
        }

        public OPCOfflineInfo GetOPCOfflineBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_OPCOfflineMaster");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<OPCOfflineInfo>();
        }

        public void UpdateOPCOfflineBySysNo(OPCOfflineInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_OPCOfflineMaster");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }
        #endregion

        #region OPCOfflineTransactionInfo
        public int InsertOPCOfflineTransaction(OPCOfflineTransactionInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_OPCOfflineTransaction");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
            int result = command.ExecuteScalar<int>();
            return result;
        }

        public OPCOfflineTransactionInfo GetOPCOfflineTransactionBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_OPCOfflineTransaction");
            command.SetParameterValue("@SysNo", sysNo);
            OPCOfflineTransactionInfo OPCTransactionInfo = command.ExecuteEntity<OPCOfflineTransactionInfo>();
            return OPCTransactionInfo;
        }

        public List<OPCOfflineTransactionInfo> GetOPCOfflineTransactionByMasterID(int masterID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_OPCOfflineTransactionByMasterID");
            command.SetParameterValue("@MasterID", masterID);
            return command.ExecuteEntityList<OPCOfflineTransactionInfo>();
        }

        public void UpdateOPCOfflineTransactionBySysNo(OPCOfflineTransactionInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_OPCOfflineTransaction");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }
        #endregion

        #region OPCOfflineMessageInfo
        public int InsertOPCOfflineMessage(OPCOfflineMessageInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_OPCOfflineMessage");
            command.SetParameterValue(info);
            return command.ExecuteScalar<int>();
        }

        public void UpdateOPCOfflineMessageBySysNo(OPCOfflineMessageInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_OPCResponseMessage");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }
        #endregion
    }
}
