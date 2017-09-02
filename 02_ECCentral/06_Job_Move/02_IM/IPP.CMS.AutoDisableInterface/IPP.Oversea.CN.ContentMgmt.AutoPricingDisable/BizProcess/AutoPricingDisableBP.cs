/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:      King.B.Wu
 * Create Date: 2010-01-11
 * Usage:  
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.IO;
using System.Web;
using System.Data;
using System.Text;
using System.Xml;
using System.Data.OleDb;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.Entities;
using IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.DataAccess;
using IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.ServiceAdapter;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.BizProcess
{
    public class AutoPricingDisableBP
    {
        public void Start()
        {
            // 配置文件统一
            string CompanyCode = "8601";
            // CompanyCode   
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["CompanyCode"].ToString()))
            {
                CompanyCode = ConfigurationManager.AppSettings["CompanyCode"].ToString();
            }

            DataSet dsResult = AutoPricingDisableDA.GetProductItemNoList(CompanyCode);

            if (dsResult.Tables != null && dsResult.Tables[0].Rows.Count == 0)
            {
                return;
            }

            int iCount = dsResult.Tables[0].Rows.Count;

            for (int i = 0; i < iCount; i++)
            {
                try
                {
                    AutoPricingDisableDA.UpdateAutoPricingDisableData(dsResult.Tables[0].Rows[i]["SysNo"].ToString(), CompanyCode);

                    SendAutoPricingDisableNotify(dsResult.Tables[0].Rows[i], CompanyCode);
                    Console.WriteLine("当前执行位置：" + (i + 1) + "/" + iCount); ;

                }catch(Exception ex)
                { Console.WriteLine(ex.Message); }
            }
        }

        /// <summary>
        /// 邮件PM
        /// </summary>
        /// <param name="entity"></param>
        private void SendAutoPricingDisableNotify(DataRow drData, string CompanyCode)
        {
            string SendEmailFlag = ConfigurationManager.AppSettings["SendEmailFlag"];

            if (String.IsNullOrEmpty(SendEmailFlag) || SendEmailFlag.Equals("0"))
            {
                return;
            }

            SalesMailEntity salesMailEntity = AutoPricingDisableDA.GetPMMails(drData["SysNo"].ToString(), drData["PMUserSysNo"].ToString(), CompanyCode);
            string toMailAddress = (salesMailEntity != null) ? salesMailEntity.toEmail : "";
            string ccMailAddress = (salesMailEntity != null) ? salesMailEntity.ccEmail : "";

            toMailAddress = String.IsNullOrEmpty(toMailAddress) ? ConfigurationManager.AppSettings["SLGroupEmail"] : toMailAddress;
            ccMailAddress = String.IsNullOrEmpty(ccMailAddress) ? ConfigurationManager.AppSettings["PMGroupEmail"] : ccMailAddress;

            if (String.IsNullOrEmpty(toMailAddress) && String.IsNullOrEmpty(ccMailAddress))
            {
                return;
            }

            if (String.IsNullOrEmpty(toMailAddress) && !String.IsNullOrEmpty(ccMailAddress))
            {
                toMailAddress = ccMailAddress;
                ccMailAddress = "";
            }

            ccMailAddress = (!String.IsNullOrEmpty(ccMailAddress)) ? ccMailAddress + ";" + ConfigurationManager.AppSettings["EmailOceo"].ToString() : ConfigurationManager.AppSettings["EmailOceo"].ToString();

            MailEntity mail = BuildEmail(drData);
            mail.From = ConfigurationManager.AppSettings["EmailFrom"];
            mail.To = toMailAddress;
            mail.CC = ccMailAddress;
            mail.Subject = "禁止自动调价到期失效通知";
            mail.CompanyCode = CompanyCode;
            EmailComparisonCNServiceFacade.SendProductEmail(mail);
        }
        private MailEntity BuildEmail(DataRow drData)
        {
            MailEntity mail = new MailEntity();
            mail.Body = new ItemAutoPricingDisableEmailBodyBuild().SetMailBody(drData);
            return mail;
        }

        private class ItemAutoPricingDisableEmailBodyBuild
        {
            public string SetMailBody(DataRow drData)
            {
                string assemblyPath = string.Empty;
                string text = string.Empty;
                assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                assemblyPath = assemblyPath.Substring(8, assemblyPath.Length - 8);
                assemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath), @"BizProcess\Template");
                string fullPath = Path.Combine(assemblyPath, "AutoDisableTemplate.txt");
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    text = sr.ReadToEnd();
                }

                text = text.Replace("_Reasons", "Success Reason: 禁止自动调价过期失效.");
                text = text.Replace("_Rule", "恢复自动调价通知");
                text = text.Replace("_Time", DateTime.Now.ToString());
                text = text.Replace("_ProductID", drData["ProductID"].ToString());
                text = text.Replace("_ProductName", drData["ProductName"].ToString());
                text = text.Replace("_JDPrice", drData["JDPrice"].ToString());
                text = text.Replace("_AMPrice", drData["AMPrice"].ToString());
                text = text.Replace("_OldPrice", drData["CurrentPrice"].ToString());
                text = text.Replace("_UnitCost", drData["UnitCost"].ToString());
                decimal MaxMargin = 0;
                decimal Point = String.IsNullOrEmpty(drData["Point"].ToString()) ? 0 : decimal.Parse(drData["Point"].ToString())/10;
                decimal CurrentPrice = String.IsNullOrEmpty(drData["CurrentPrice"].ToString()) ? 0 : decimal.Parse(drData["CurrentPrice"].ToString());
                decimal UnitCost = String.IsNullOrEmpty(drData["UnitCost"].ToString()) ? 0 : decimal.Parse(drData["UnitCost"].ToString());
                MaxMargin = (UnitCost == 0) ? 0 : (CurrentPrice - Point - UnitCost) / UnitCost;
                text = text.Replace("_Margin", (MaxMargin * 100).ToString("0.00"));

                return text;
            }
        }
    }
}