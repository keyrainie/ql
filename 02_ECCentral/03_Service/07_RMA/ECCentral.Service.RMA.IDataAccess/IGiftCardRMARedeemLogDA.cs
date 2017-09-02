using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IGiftCardRMARedeemLogDA
    {
        GiftCardRMARedeemLog Create(GiftCardRMARedeemLog entity);

        List<GiftCardRMARedeemLog> GetGiftCardRMARedeemLogBySOSysNo(int soSysNo);
    }
}
