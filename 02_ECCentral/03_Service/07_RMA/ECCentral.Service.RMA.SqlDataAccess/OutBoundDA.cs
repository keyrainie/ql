using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IOutBoundDA))]
    public class OutBoundDA : IOutBoundDA
    {
        #region IOutBoundDA Members

        public virtual bool UpdateOutboundItemSendEmailCount(int outboundSysNo, int registerSysNo, int sendMailCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateOutboundItemSendEmailCount");

            cmd.SetParameterValue("@Count", sendMailCount);
            cmd.SetParameterValue("@OutboundSysNo", outboundSysNo);
            cmd.SetParameterValue("@RegisterSysNo", registerSysNo);
            return cmd.ExecuteNonQuery() == 1;
        }

        public virtual DataRow GetOutboundBySysNo(int OutboundSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetOutboundBySysNo");
            cmd.SetParameterValue("@OutboundSysNo", OutboundSysNo);
            return cmd.ExecuteDataRow();
        }
       
        public List<int> GetOutBoundSysNoListByRegisterSysNoList(string registerSysNoList)
        {
            List<int> list = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetOutBound");
            command.ReplaceParameterValue("#RegisterSysNo#", registerSysNoList);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                list = (from item in dt.AsEnumerable()
                        select item.Field<int>("OutBoundSysNo")).ToList();
            }
            return list;
        }       

        public bool UpdateOutBounds(string outBoundList)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateOutBound");
            command.ReplaceParameterValue("#OutBoundSysNos#", outBoundList);
            
            return command.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
