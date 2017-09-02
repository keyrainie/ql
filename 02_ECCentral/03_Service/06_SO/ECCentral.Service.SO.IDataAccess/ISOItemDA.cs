using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOItemDA
    {
        string GetDisCountTypeByPromotionSysNo(int promotionSysNo);

        int GetRatioOfGift(int masterProductSysNo, int promotionSysNo, int giftProductSysNo);

        int ChangedGossLogStatus(int soSysNo, string status);
    }
}
