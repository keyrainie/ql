/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:      King.B.Wu
 * Create Date: 2010-05-05
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
using System.Data.OleDb;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using IPP.Oversea.CN.ContentMgmt.GiftCard.Entities;
using IPP.Oversea.CN.ContentMgmt.GiftCard.DataAccess;
using IPP.Oversea.CN.ContentMgmt.GiftCard.ServiceAdapter;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.GiftCard.BizProcess
{
    public class GiftCardBP
    {
        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;

        string StoreCompanyCode = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["StoreCompanyCode"].ToString())) ? "8601" : ConfigurationManager.AppSettings["StoreCompanyCode"].ToString();
        string companyCode = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["CompanyCode"].ToString())) ? "8601" : ConfigurationManager.AppSettings["CompanyCode"].ToString();
        decimal ItemBatchPageSize = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ItemBatchPageSize"].ToString())) ? 1000 : decimal.Parse(ConfigurationManager.AppSettings["ItemBatchPageSize"].ToString());

        int iManufacturerSysNo = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ManufacturerSysNo"])) ? 1 : int.Parse(ConfigurationManager.AppSettings["ManufacturerSysNo"]);
        string gcC3Number = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["GCC3Number"].ToString())) ? "11295" : ConfigurationManager.AppSettings["GCC3Number"].ToString(); 

        #region 主程序
        public void Starter()
        {
           
            DataSet dsMaster = GiftCardDA.GetGiftCardFabricationMaster(companyCode);
           

            if (dsMaster == null || dsMaster.Tables.Count == 0 || dsMaster.Tables[0].Rows.Count == 0)
            {
                WriteLog("无审核通过制作卡单,JOB运行结束...");
                return;
            }
        
            foreach (DataRow drRow in dsMaster.Tables[0].Rows)
            {              
                DataSet dsPassItem = GiftCardDA.GetPassGiftCardInfo(drRow["GiftCardFabricationSysNo"].ToString(), companyCode);
                if (dsPassItem != null && dsPassItem.Tables.Count > 0 && dsPassItem.Tables[0].Rows.Count > 0 && decimal.Parse(dsPassItem.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    continue;
                }
             
                DataSet dsItem = GiftCardDA.GetGiftCardFabricationItem(drRow["GiftCardFabricationSysNo"].ToString(), companyCode, gcC3Number, iManufacturerSysNo);
                if (dsItem == null || dsItem.Tables.Count == 0 || dsItem.Tables[0].Rows.Count == 0)
                {
                    WriteLog("GiftCardFabricationSysNo:" + drRow["GiftCardFabricationSysNo"].ToString() + "无有效的礼品卡信息.");
                    continue;
                }

                WriteLog("GiftCardFabricationSysNo:" + drRow["GiftCardFabricationSysNo"].ToString() + "礼品卡信息SP调用开始...");

                decimal decTotalCount = 0;
                decTotalCount = (String.IsNullOrEmpty(drRow["TotalCount"].ToString())) ? 0 : decimal.Parse(drRow["TotalCount"].ToString());

          
                DataSet dsInfoResult = GiftCardDA.CheckGiftCardInfo();

                if (dsInfoResult != null && dsInfoResult.Tables.Count > 0 && dsInfoResult.Tables[0].Rows.Count > 0)
                {
                    decimal TotalPass = String.IsNullOrEmpty(dsInfoResult.Tables[0].Rows[0]["TotalPass"].ToString()) ? 0 : decimal.Parse(dsInfoResult.Tables[0].Rows[0]["TotalPass"].ToString());

                    if (decTotalCount > TotalPass)
                    {
                        WriteLog("GiftCardFabricationSysNo:" + drRow["GiftCardFabricationSysNo"].ToString() + "无法生成礼品卡信息,原因:有效卡号不足.");
                        //Console.WriteLine("有效卡号不足,JOB运行结束...");
                        try
                        {
                          
                            SendFailEmail("GiftCardFabricationSysNo:" + drRow["GiftCardFabricationSysNo"].ToString() + " JOB无法生成礼品卡信息,原因:有效卡号不足.");
                        }
                        catch
                        { }
                        continue;
                    }
                }

                if (decTotalCount <= ItemBatchPageSize)
                {
                   
                    CreateGiftCardInfoList(dsItem, companyCode, StoreCompanyCode, drRow["GiftCardFabricationSysNo"].ToString());
                }
                else
                {
                  
                    BatchCreateGiftCardInfoList(dsItem, companyCode, StoreCompanyCode, drRow["GiftCardFabricationSysNo"].ToString(), ItemBatchPageSize);
                }

                WriteLog("GiftCardFabricationSysNo:" + drRow["GiftCardFabricationSysNo"].ToString() + "礼品卡信息SP调用结束...");
            }

            WriteLog("审核通过制作卡单生成卡号成功,JOB运行结束...");

        }
        #endregion

        #region
        private void CreateGiftCardInfoList(DataSet dsItem, string companyCode, string StoreCompanyCode, string GiftCardFabricationSysNo)
        {
            StringBuilder messageBuilder = new StringBuilder();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = Encoding.Unicode;
            using (XmlWriter xmlWriter = XmlWriter.Create(messageBuilder, setting))
            {
                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteStartElement("Header");
                #region header
                xmlWriter.WriteElementString("Action", "Fabrication");
                xmlWriter.WriteElementString("Version", "V1");
                xmlWriter.WriteElementString("From", "IPP.Content");
                xmlWriter.WriteElementString("CurrencySysNo", "1");
                xmlWriter.WriteElementString("Language", "zh-CN");
                xmlWriter.WriteElementString("CompanyCode", companyCode);
                xmlWriter.WriteElementString("StoreCompanyCode", StoreCompanyCode);
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Body");
                xmlWriter.WriteElementString("EditUser", "System");
                #region giftcard list
                xmlWriter.WriteStartElement("GiftCard");
                xmlWriter.WriteElementString("ReferenceSysno", GiftCardFabricationSysNo);
                xmlWriter.WriteStartElement("ItemInfo");
                foreach (DataRow item in dsItem.Tables[0].Rows)
                {
                    xmlWriter.WriteStartElement("Item");
                    xmlWriter.WriteElementString("TotalAmount", item["CurrentPrice"].ToString());
                    xmlWriter.WriteElementString("Quantity", item["Quantity"].ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            WriteLog("GiftCardFabricationSysNo:" + GiftCardFabricationSysNo + "SP调用生成卡号....");
            GiftCardDA.GiftCardFabricationECOperateGiftCard(messageBuilder.ToString());
        }

        private void BatchCreateGiftCardInfoList(DataSet dsItem,string companyCode, string StoreCompanyCode, string GiftCardFabricationSysNo, decimal ItemBatchPageSize)
        {
            foreach (DataRow item in dsItem.Tables[0].Rows)
            {
                decimal quantity = decimal.Parse(item["Quantity"].ToString());
                if (quantity > ItemBatchPageSize)
                {

                    while (quantity > ItemBatchPageSize)
                    {
                        quantity = quantity - ItemBatchPageSize;
                        CreateGiftCardInfoList(item, companyCode, StoreCompanyCode, GiftCardFabricationSysNo, ItemBatchPageSize.ToString()); 
                    }

                    if (quantity > 0)
                    {
                        CreateGiftCardInfoList(item, companyCode, StoreCompanyCode, GiftCardFabricationSysNo, quantity.ToString());
                    }
                }
                else
                {
                    CreateGiftCardInfoList(item, companyCode, StoreCompanyCode, GiftCardFabricationSysNo, quantity.ToString());
                }
            }
        }


        private void CreateGiftCardInfoList(DataRow drItem, string companyCode, string StoreCompanyCode, string GiftCardFabricationSysNo, string Quantity)
        {
            StringBuilder messageBuilder = new StringBuilder();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = Encoding.Unicode;
            using (XmlWriter xmlWriter = XmlWriter.Create(messageBuilder, setting))
            {
                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteStartElement("Header");
                #region header
                xmlWriter.WriteElementString("Action", "Fabrication");
                xmlWriter.WriteElementString("Version", "V1");
                xmlWriter.WriteElementString("From", "IPP.Content");
                xmlWriter.WriteElementString("CurrencySysNo", "1");
                xmlWriter.WriteElementString("Language", "zh-CN");
                xmlWriter.WriteElementString("CompanyCode", companyCode);
                xmlWriter.WriteElementString("StoreCompanyCode", StoreCompanyCode);
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Body");
                xmlWriter.WriteElementString("EditUser", "System");
                #region giftcard list
                xmlWriter.WriteStartElement("GiftCard");
                xmlWriter.WriteElementString("ReferenceSysno", GiftCardFabricationSysNo);
                xmlWriter.WriteStartElement("ItemInfo");

                xmlWriter.WriteStartElement("Item");
                xmlWriter.WriteElementString("TotalAmount", drItem["CurrentPrice"].ToString());
                xmlWriter.WriteElementString("Quantity", Quantity);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                #endregion
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            WriteLog("GiftCardFabricationSysNo:" + GiftCardFabricationSysNo + "SP调用生成卡号....");
            GiftCardDA.GiftCardFabricationECOperateGiftCard(messageBuilder.ToString());
        }
        #endregion

        #region Email通知
        /// <summary>
        /// Email通知
        /// </summary>
        /// <param name="strMailBody"></param>
        private void SendFailEmail(string body)
        {
            MailEntity mail = new MailEntity();
            mail.Body = body;
            mail.From = ConfigurationManager.AppSettings["EmailFrom"];
            mail.To = ConfigurationManager.AppSettings["EmailTo"];
            mail.CC = ConfigurationManager.AppSettings["EmailCC"];

            if (String.IsNullOrEmpty(mail.To))
            {
                return;
            }
            mail.Subject = "GC卡号生成失败";

            EmailComparisonCNServiceFacade.SendProductEmail(mail);
        }
        #endregion


        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);           
        }
    }
}