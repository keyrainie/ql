using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.DataAccess;

using Newegg.Oversea.Framework.Entity;
using IPP.ContentMgmt.SendMailForPicture.BusinessEntities;
using System.Configuration;

namespace IPP.ContentMgmt.SendMailForPicture.DA
{
    public class Product_StatusDA
    {
        /// <summary>
        /// 获取图片没有打勾Item
        /// </summary>
        /// <returns>返回productList</returns>
        public static List<ProductList> GetNotTickPicture()
        {
            List<ProductList> productList = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetNotTickPicture");

            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);

            productList = command.ExecuteEntityList<ProductList>();
            return productList;
        }

        /// <summary>
        /// 没有上传图片Item
        /// </summary>
        /// <returns>返回productList</returns>
        public static List<ProductList> GetNotUploadPicture()
        {
            List<ProductList> productList = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetNotUploadPicture");         
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);

            productList = command.ExecuteEntityList<ProductList>();
            return productList;
        }


        public static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["ErroMailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " 实库图片拍摄Item 的Newegg Support 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 邮件插入表
        /// </summary>
        /// <param name="ErrorMsg"></param>
        public static void InsertMailDB (string MailBody)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " 实库图片拍摄Item 的Newegg Support 通知邮件";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");

            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);
            command.ExecuteNonQuery();
        }
    }
}
