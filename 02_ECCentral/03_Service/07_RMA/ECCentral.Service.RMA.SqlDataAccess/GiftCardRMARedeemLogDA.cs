using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IGiftCardRMARedeemLogDA))]
    public class GiftCardRMARedeemLogDA : IGiftCardRMARedeemLogDA
    {
        #region IGiftCardRMARedeemLogDA Members

        public GiftCardRMARedeemLog Create(GiftCardRMARedeemLog entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertGiftCardRMARedeemLog");

            dc.SetParameterValue<GiftCardRMARedeemLog>(entity);

            dc.ExecuteNonQuery();

            entity.TransactionNumber = Convert.ToInt32(dc.GetParameterValue("@TransactionNumber"));
            return entity;
        }

        public List<GiftCardRMARedeemLog> GetGiftCardRMARedeemLogBySOSysNo(int soSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetGiftCardRMARedeemLogBySOSysNo");
            dc.SetParameterValue("@SOSysNo", soSysNo);            

            return dc.ExecuteEntityList<GiftCardRMARedeemLog>();            
        }

        #endregion
    }
}
