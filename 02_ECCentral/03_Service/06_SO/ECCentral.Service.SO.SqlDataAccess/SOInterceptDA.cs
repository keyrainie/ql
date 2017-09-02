using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOInterceptDA))]
    public class SOInterceptDA : ISOInterceptDA
    {
        #region ISOInterceptDA Members

        public List<SOInterceptInfo> GetSOInterceptInfoListBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOInterceptInfoListBySOSysNo");
            command.SetParameterValue("@Sysno", soSysNo);
            return command.ExecuteEntityList<SOInterceptInfo>();    
        }

        public void AddSOInterceptInfo(SOInterceptInfo info, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AddSOInterceptInfo");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@ShipCategory", info.ShipTypeCategory);
            command.SetParameterValue("@WareHouseNumber", info.StockSysNo);
            command.SetParameterValue("@ShipTypeSysNo", info.ShippingTypeID);
            command.SetParameterValue("@HasTrackingNumber", string.IsNullOrEmpty(info.HasTrackingNumber) ? null : info.HasTrackingNumber);
            command.SetParameterValue("@ShipTimeType", string.IsNullOrEmpty(info.ShipTimeType) ? null : info.ShipTimeType);
            command.SetParameterValue("@EmailAddress", info.EmailAddress);
            command.SetParameterValue("@CCEmailAddress", info.CCEmailAddress);
            command.SetParameterValue("@FinanceEmailAddress", info.FinanceEmailAddress);
            command.SetParameterValue("@FinanceCCEmailAddress", info.FinanceCCEmailAddress);
            command.ExecuteNonQuery();
        }

        public void BatchUpdateSOInterceptInfo(SOInterceptInfo info)
        {
            DataCommand command = null;
            StringBuilder updateSB = new StringBuilder();

            if (!string.IsNullOrEmpty(info.EmailAddress))
            {
                updateSB.Append(",h.[EmailAddress] = '" + info.EmailAddress + "'");
            }

            if (!string.IsNullOrEmpty(info.CCEmailAddress))
            {
                updateSB.Append(",h.[CCEmailAddress] = '" + info.CCEmailAddress + "'");
            }

            if (!string.IsNullOrEmpty(info.FinanceEmailAddress))
            {
                updateSB.Append(",h.[FinanceEmailAddress] = '" + info.FinanceEmailAddress + "'");
            }

            if (!string.IsNullOrEmpty(info.FinanceCCEmailAddress))
            {
                updateSB.Append(",h.[FinanceCCEmailAddress] = '" + info.FinanceCCEmailAddress + "'");
            }

            string updateString = updateSB.ToString();
            if (updateString.IndexOf(',') == 0)
            {
                updateString = updateString.Substring(1);
            }

            if (!string.IsNullOrEmpty(info.Sysnolist))
            {
                command = DataCommandManager.GetDataCommand("BatchUpdateSOInterceptInfo");

                command.ReplaceParameterValue("#UpdateString#", updateString);
                command.ReplaceParameterValue("#SysNoString#", " h.SysNo IN (" + info.Sysnolist + ")");
            }      
            command.ExecuteNonQuery();
        }

        public void DeleteSOInterceptInfo(SOInterceptInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("BatchDeleteSOIntercept");
            command.ReplaceParameterValue("#SysNoString#", " SysNo IN (" + info.Sysnolist + ")");
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
