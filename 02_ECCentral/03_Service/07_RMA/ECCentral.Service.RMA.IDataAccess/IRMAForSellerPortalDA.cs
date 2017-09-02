using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRMAForSellerPortalDA
    {
        bool ExistValidRMA(int soSysNo);

        bool ExistAutoRMALog(int soSysNo);

        int CreateSequence(string tableName);

        void InsertSellerPortalAutoRMALog(int soSysNo, string inUser);

        void UpdateSellerPortalAutoRMALog(int soSysNo, string requestStatus, DateTime requestTime, string refundStatus, DateTime refundTime);

        SellerPortalAutoRMALog GetSellerPortalAutoRMALog(int soSysNo);

        void SendVatSSB(string xmlMsg);
    }
}
