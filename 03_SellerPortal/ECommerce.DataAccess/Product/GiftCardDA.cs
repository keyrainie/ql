using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility.DataAccess.Database.DbProvider;

namespace ECommerce.DataAccess.Product
{
    public class GiftCardDA
    {
        public static string OperateGiftCard(string xmlMsg)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("OperateGiftCard");
            dc.SetParameterValue("@Msg", xmlMsg);

            try
            {
                dc.ExecuteNonQuery();
            }
            catch (DataAccessException e)
            {
                Regex reg = new Regex(@"(?<=((?<!Error[ ])Message:)).+(?=Error Severity)");
                throw new BusinessException(reg.Match(e.Message).Value);
            }

            object o = dc.GetParameterValue("@StatusCode");
            if (o != null && o != DBNull.Value)
            {
                return o.ToString().Trim();
            }
            return string.Empty;
        }
    }
}
