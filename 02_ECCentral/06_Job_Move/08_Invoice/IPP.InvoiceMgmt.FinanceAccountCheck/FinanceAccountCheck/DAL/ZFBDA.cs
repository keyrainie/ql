using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.DAL;
using System.Data;
using IPPOversea.Invoicemgmt.Model;

namespace IPPOversea.Invoicemgmt.ZFBAccountCheck.DAL
{
    class ZFBDA
    {  
        public bool IsFirst()
        {
            bool result;
            SqlCommand cmd = CommandManager.GetCommand("IsFirst");
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            if ((int)DBHelper.GetObject(cmd) == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public DateTime GetLastImportDate()
        {
            SqlCommand cmd = CommandManager.GetCommand("GetLastImportDate");
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            return Convert.ToDateTime(DBHelper.GetObject(cmd));
        }

        public void CreateLog(ZFBLogEntity entity)
        {
            SqlCommand cmd = CommandManager.GetCommand("CreateLog");
            cmd.Parameters.Add(new SqlParameter("@PayTermsNo", entity.PayTermsNo));
            cmd.Parameters.Add(new SqlParameter("@PayTerms", entity.PayTerms));
            cmd.Parameters.Add(new SqlParameter("@ContentSysNo", entity.ContentSysNo));
            cmd.Parameters.Add(new SqlParameter("@InUser", entity.InUser));
            cmd.Parameters.Add(new SqlParameter("@InDate", entity.InDate));
            
            DBHelper.ExecuteSql(cmd);
        }

        public ZFBLogEntity CreateLogContent(ZFBLogEntity entity)
        {
            SqlCommand cmd = CommandManager.GetCommand("CreateLogContent");
            cmd.Parameters.Add(new SqlParameter("@ImportData", entity.ImportData));

            entity.ContentSysNo = Convert.ToInt32(DBHelper.GetObject(cmd));

            return entity;
        }

        public void CreateData(List<ZFBDataEntity> entityList)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            SqlCommand cmd = CommandManager.GetCommand("CreateData");
            string sqlHeader = CommandManager.GetCommandText("CreateData");


            sqlBuilder.Append(sqlHeader);
            int countFlag = 0;
            foreach (ZFBDataEntity entity in entityList)
            {
                sqlBuilder.Append("\r\nSELECT ");
                sqlBuilder.Append(entity.SoSysNo.ToString());
                AppendNumeric(sqlBuilder, entity.PayTermsNo);
                AppendText(sqlBuilder, entity.PayTerms);
                AppendText(sqlBuilder, entity.PayedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                AppendNumeric(sqlBuilder, entity.PayedAmt);
                AppendText(sqlBuilder, entity.SerialNumber);
                AppendText(sqlBuilder, entity.OutOrderNo);
                AppendText(sqlBuilder, entity.PayedUserTag);
                AppendText(sqlBuilder, entity.PayedUserName);
                AppendText(sqlBuilder, entity.PayedUserNo);
                AppendText(sqlBuilder, entity.PartnerName);
                AppendText(sqlBuilder, entity.TradeType);
                AppendText(sqlBuilder, entity.AttachInfo);
                AppendText(sqlBuilder, entity.AttachInfo2);
                AppendText(sqlBuilder, "System");
                AppendText(sqlBuilder, entity.InDate.ToString("yyyy-MM-dd HH:mm:ss"));

                if (++countFlag != entityList.Count)
                    sqlBuilder.Append(" UNION ALL");
            }

            cmd.CommandText = sqlBuilder.ToString();
            DBHelper.ExecuteSql(cmd);
        }

        public List<ZFBDataEntity> GetLastTimeData(DateTime createStart)
        {
            SqlCommand cmd = CommandManager.GetCommand("GetLastTimeData");
            cmd.Parameters.Add(new SqlParameter("@PayedDate", createStart));
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            return DBHelper.GetData(cmd).ToList<ZFBDataEntity>();
        }

        public Int32 SysncPayedDate()
        {
            SqlCommand cmd1 = CommandManager.GetCommand("SysncPayedDateStep1");
            cmd1.CommandTimeout = 120;
            cmd1.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            int rowCount = DBHelper.ExecuteNoQuery(cmd1);
            

            SqlCommand cmd2 = CommandManager.GetCommand("SysncPayedDateStep2");
            cmd2.CommandTimeout = 120;
            cmd2.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            rowCount += DBHelper.ExecuteNoQuery(cmd2);

            return rowCount;
        }


        public bool ExistsUnSysnedData()
        {
            SqlCommand cmd = CommandManager.GetCommand("ExistsUnSysnedData");
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            var count = (int)DBHelper.GetObject(cmd);

            return count > 0;
        }


        #region HelpMethod

        private void AppendText(StringBuilder builder, object text)
        {
            if (text != null)
            {
                builder.Append("\r\n\t ,'");
                builder.Append(text.ToString());
                builder.Append("'");
            }
            else
            {
                builder.Append("\r\n\t, NULL");
            }
        }

        private void AppendNumeric(StringBuilder builder, object numeric)
        {
            if (numeric != null)
            {
                builder.Append("\r\n\t, ");
                builder.Append(numeric.ToString());
            }
            else
            {
                builder.Append("\r\n\t, NULL");
            }
        }

        public string ImportDataLength()
        {
            SqlCommand cmd = CommandManager.GetCommand("ImportDataLength");
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);

            return DBHelper.GetObject(cmd).ToString();
        }

        public List<PayTypeEntity> GetPayType(int soSysNo)
        {
            SqlCommand cmd = CommandManager.GetCommand("GetPayType");
            cmd.Parameters.Add(new SqlParameter("@SysNo", soSysNo));
            cmd.Parameters.Add(new SqlParameter("@CompanyCode", Settings.CompanyCode));

            return DBHelper.GetData(cmd).ToList<PayTypeEntity>();
        }

        #endregion
    }
}
