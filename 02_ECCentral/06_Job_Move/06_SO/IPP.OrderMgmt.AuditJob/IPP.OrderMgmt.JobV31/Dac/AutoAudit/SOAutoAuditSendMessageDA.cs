using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;

namespace IPP.OrderMgmt.JobV31.Dac.AutoAudit
{
    public class SOAutoAuditSendMessageDA
    {

        public static List<int> GetSOList4Audit2SendMessage(int topCount,string companyCode)
        {
            List<int> result = new List<int>();

            List<SingleValueEntity> queryResult = new List<SingleValueEntity>();

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOList4Audit2SendMessage");
            command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());
            command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, companyCode);
            queryResult = command.ExecuteEntityList<SingleValueEntity>();

            if (queryResult != null
                && queryResult.Count > 0)
            {
                queryResult.ForEach(x => result.Add(x.Int32Value));
            }

            queryResult = null;
            GC.Collect();

            if (result == null)
                result = new List<int>();
            return result;
        }

        public static int UpdateSO4PassAutoAuditSendMessage(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSO4PassAutoAuditSendMessage");

            command.SetParameterValue("@SOSysNo", sysNo);

            return command.ExecuteNonQuery();
        }
    }
}