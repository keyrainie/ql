using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using SqlHelper;

namespace CountDownPromotionTitle
{
    class Program
    {
        public static readonly string strConn = System.Configuration.ConfigurationSettings.AppSettings["ConnStr"];

        static void Main(string[] args)
        {            
            RunStep1();
            RunStep2();
        }
        /// <summary>
        /// 1.从Product_PromotionTitle 表取类型不为Normal, Status为等待,并且当前时间大于等于StartTime并且小于EndTime的数据,执行更新PromotionTitle, ProductName操作,把Status更新为有效;
        /// 即 if(UPPER([Type])='COUNTDOWN' AND [Status]='O' AND GETDATE()>=BeginDate AND GETDATE()〈Enddate)
        /// {  更新PromotionTitle;更新ProductName; Status=A; }
        /// </summary>
        private static void RunStep1()
        {
            try
            {
                string strWhere = "UPPER([Type])='COUNTDOWN' AND [Status]='O' AND GETDATE()>=BeginDate AND GETDATE()<Enddate";
                DataTable dt = null;
                dt = GetPromotionTitleList(strWhere);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UpdateStatus(Convert.ToInt32(dr["SysNo"]), "A");
                        UpdatePromotionTitle(Convert.ToInt32(dr["ProductSysNo"]), dr["PromotionTitle"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }
        }
        /// <summary>
        /// 2.从Product_PromotionTitle 表取取类型不为Normal, Status为有效中或者等待,并且当前时间大于等于EndTime的数据,执行更新PromotionTitle, ProductName操作,把Status更新为失效;
        /// 即 if(Type!= Normal && (Status==O|| Status==A) && CurrentTime>= EndTime)
        /// {  更新PromotionTitle;更新ProductName; Status=D; }
        /// </summary>
        private static void RunStep2()
        {
            try
            {
                string strWhere = "UPPER([Type])='COUNTDOWN' AND [Status] in ('A','O') AND GETDATE()>=Enddate";
                DataTable dt = null;
                string strNormalPromotionTitle = null;
                dt = GetPromotionTitleList(strWhere);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UpdateStatus(Convert.ToInt32(dr["SysNo"]), "D");
                        strNormalPromotionTitle = GetNormalPromotionTitle(Convert.ToInt32(dr["ProductSysNo"]));
                        if (strNormalPromotionTitle != null)
                        {
                            UpdatePromotionTitle(Convert.ToInt32(dr["ProductSysNo"]), strNormalPromotionTitle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }
        }
        private static void log(string text)
        {
            Console.WriteLine(text);
            File.AppendAllText(string.Format(@"{1}log\{0}.log", DateTime.Now.ToString("yyyyMMdd"), AppDomain.CurrentDomain.BaseDirectory), string.Format("{0} {1}", DateTime.Now.ToString("hh:mm:ss"), text + '\r' + '\n'));
        }
        #region DB操作
        private static DataTable GetPromotionTitleList(string strWhere)
        {
            DataTable dt = null;
            try
            {
                string strSql = "SELECT SysNo,ProductSysNo,PromotionTitle,[Type],BeginDate,Enddate,[Status] FROM OverseaContentManagement.dbo.Product_PromotionTitle ";
                if (strWhere != null && strWhere.Trim() != "")
                    strSql += " WHERE " + strWhere + " AND CompanyCode='8601'";
                else
                    strSql += " WHERE CompanyCode='8601'";
                dt = SqlHelper.SqlHelper.GetDataSet(strConn, CommandType.Text, strSql, null).Tables[0];
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
            return dt;
        }
        /// <summary>
        /// 根据ProductSysNo取Normal的PromotionTitle
        /// </summary>
        /// <param name="intProductSysNo"></param>
        /// <returns></returns>
        private static string GetNormalPromotionTitle(int intProductSysNo)
        {
            string result = null;
            try
            {
                string strSql = "SELECT PromotionTitle FROM OverseaContentManagement.dbo.Product_PromotionTitle WHERE UPPER([Type])='NORMAL' AND ProductSysNo=@ProductSysNo";
                SqlParameter[] parameters = {
					new SqlParameter("@ProductSysNo", SqlDbType.Int)
                                             };
                parameters[0].Value = intProductSysNo;
                DataTable dt = SqlHelper.SqlHelper.GetDataSet(strConn, CommandType.Text, strSql, parameters).Tables[0];
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["PromotionTitle"] != null)
                    result = dt.Rows[0]["PromotionTitle"].ToString();
                else
                    result = "";
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return null;
            }
            return result;
        }
        /// <summary>
        /// 更新Product_PromotionTitle表状态
        /// </summary>
        /// <param name="intSysNo"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        private static int UpdateStatus(int intSysNo,string strStatus)
        {
            int result = 0;
            try
            {
                string strSql = "UPDATE OverseaContentManagement.dbo.Product_PromotionTitle SET [Status]=@Status where SysNo=@SysNo";
                SqlParameter[] parameters = {
					new SqlParameter("@SysNo", SqlDbType.Int),
                    new SqlParameter("@Status",SqlDbType.VarChar)
                                           };
                parameters[0].Value = intSysNo;
                parameters[1].Value = strStatus;
                result = SqlHelper.SqlHelper.ExecuteNonQuery(strConn, CommandType.Text, strSql, parameters);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return 0;
            }
            return result;
        }
        /// <summary>
        /// 更新Product表的ProductName、ProductTitle
        /// </summary>
        /// <param name="intProductSysNo"></param>
        /// <param name="strPromotionTitle"></param>
        /// <returns></returns>
        private static int UpdatePromotionTitle(int intProductSysNo, string strPromotionTitle)
        {
            int result = 0;
            try
            {
                if (strPromotionTitle.Trim() != "")
                    strPromotionTitle = "<font color='red'>" + strPromotionTitle + "</font>";
                string strSql = "UPDATE ipp3.dbo.Product SET PromotionTitle=@PromotionTitle,ProductName=ProductTitle+@PromotionTitle WHERE SysNo=@ProductSysNo";
                SqlParameter[] parameters = {
					new SqlParameter("@ProductSysNo", SqlDbType.Int),
                    new SqlParameter("@PromotionTitle",SqlDbType.VarChar)
                                           };
                parameters[0].Value = intProductSysNo;
                parameters[1].Value = strPromotionTitle;
                result = SqlHelper.SqlHelper.ExecuteNonQuery(strConn, CommandType.Text, strSql, parameters);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                return 0;
            }
            return result;
        }
        #endregion DB操作
    }
}
