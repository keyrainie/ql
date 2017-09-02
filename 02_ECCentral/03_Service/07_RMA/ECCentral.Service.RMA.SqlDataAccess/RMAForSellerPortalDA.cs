using System;
using System.Data;

using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRMAForSellerPortalDA))]
    public class RMAForSellerPortalDA : IRMAForSellerPortalDA
    {
        #region IRMAForSellerPortalDA Members

        public bool ExistValidRMA(int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IfExistValidRMA");
            cmd.SetParameterValue("@SOSysNo", soSysNo);

            string result = Convert.ToString(cmd.ExecuteScalar());
            if (!string.IsNullOrEmpty(result))
            {
                return true;
            }
            return false;
        }

        public bool ExistAutoRMALog(int soSysNo)
        {
            SellerPortalAutoRMALog result = GetSellerPortalAutoRMALog(soSysNo);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public int CreateSequence(string tableName)
        {                    
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CreateSequence");
            cmd.CommandText = cmd.CommandText.Replace("#tableName#", tableName);            

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                return Convert.ToInt32(result.ToString());
            }
            return -1;
        }

        public void InsertSellerPortalAutoRMALog(int soSysNo, string inUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertSellerPortalAutoRMALog");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.SetParameterValue("@InUser", inUser);

            cmd.ExecuteNonQuery();
        }

        public void UpdateSellerPortalAutoRMALog(int soSysNo, string requestStatus, DateTime requestTime, string refundStatus, DateTime refundTime)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSellerPortalAutoRMALog");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.SetParameterValue("@RequestStatus", requestStatus);
            cmd.SetParameterValue("@RequestTime", requestTime);
            cmd.SetParameterValue("@RefundStatus", refundStatus);
            cmd.SetParameterValue("@RefundTime", refundTime);
            cmd.ExecuteNonQuery();
        }

        public SellerPortalAutoRMALog GetSellerPortalAutoRMALog(int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSellerPortalAutoRMALog");
            cmd.SetParameterValue("@SOSysNo", soSysNo);

            return cmd.ExecuteEntity<SellerPortalAutoRMALog>();            
        }

        public void SendVatSSB(string xmlMsg)
        {           
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SendVatSSB");

            command.SetParameterValue("@Msg",xmlMsg);

            command.ExecuteNonQuery();          
        }

        #endregion
    }
}
