using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOItemDA))]
    public class SOItemDA:ISOItemDA
    {

        public string GetDisCountTypeByPromotionSysNo(int promotionSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetDisCountTypeByPromotionSysNo");

            command.SetParameterValue("@PromotionSysNo", promotionSysNo);

            object o = command.ExecuteScalar();

            if (o == null || Convert.IsDBNull(o))
                return null;
            else
                return o.ToString();
        }


        public int GetRatioOfGift(int masterProductSysNo, int promotionSysNo, int giftProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRatioOfGift");

            command.SetParameterValue("@ProductSysNo", masterProductSysNo);
            command.SetParameterValue("@PromotionSysNo", promotionSysNo);
            command.SetParameterValue("@GiftSysNo", giftProductSysNo);

            object o = command.ExecuteScalar();

            if (o == null)
                return 0;
            else
                return (int)o;
        }


        public int ChangedGossLogStatus(int soSysNo, string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangedGossLogStatus");

            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);

            return command.ExecuteNonQuery();
        }

    }
}
