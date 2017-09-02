using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Data;
using System.Text;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOPendingDA))]
    public class SOPendingDA : ISOPendingDA
    {

        #region ISOPendingDA Members

        public void UpdateSOPendingStatus(int soSysNo, SOPendingStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_UpdateStatus_Pending");
            command.SetParameterValue("@Status", (int)status);
            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@LastEditUserSysNumber", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@LastEditDate", DateTime.Now);
            command.ExecuteNonQuery();
        }

        public string GetOutStockString(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_GetOutStockString_Pending");
            command.SetParameterValue("@SOSysNo", soSysNo);
            DataSet queryResult = command.ExecuteDataSet();
            StringBuilder result = new StringBuilder();
            foreach (DataRow dr in queryResult.Tables[0].Rows)
            {
                result.Append(dr["WarehouseNumber"].ToString() + ";");
            }
            return result.ToString();
        }

        /// <summary>
        /// 根据订单号获取对象
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>对象实体</returns>
        public SOPending GetBySysNo(int soSysNo)
        {
            SOPending result = null;
            SOPendingQueryFilter query = new SOPendingQueryFilter() { SOSysNo = soSysNo };
            int count = 0;
            var dt = ObjectFactory<ISOQueryDA>.Instance.PendingListQuery(query, out count, false);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                result = DataMapper.GetEntity<SOPending>(row);
            }
            return result;
        }

        #endregion
    }
}
