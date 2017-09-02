using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface IOPCDA
    {
        #region OPCOfflineInfo
        bool SOIsProcess(int soSysNo);

        int InsertOPCOfflineInfo(OPCOfflineInfo info);

        OPCOfflineInfo GetOPCOfflineBySysNo(int sysNo);

        void UpdateOPCOfflineBySysNo(OPCOfflineInfo info);

        #endregion

        #region OPCOfflineTransactionInfo
        int InsertOPCOfflineTransaction(OPCOfflineTransactionInfo info);

        OPCOfflineTransactionInfo GetOPCOfflineTransactionBySysNo(int sysNo);

        List<OPCOfflineTransactionInfo> GetOPCOfflineTransactionByMasterID(int masterID);

        void UpdateOPCOfflineTransactionBySysNo(OPCOfflineTransactionInfo info);
        #endregion

        #region OPCOfflineMessageInfo
        int InsertOPCOfflineMessage(OPCOfflineMessageInfo info);
        void UpdateOPCOfflineMessageBySysNo(OPCOfflineMessageInfo info);
        #endregion
    }
}
