/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:      King.B.Wu
 * Create Date: 2009-12-08
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

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip; 
 
using IPP.Oversea.CN.ContentMgmt.GoogleSearch.Entities;
using IPP.Oversea.CN.ContentMgmt.GoogleSearch.DataAccess;
using IPP.Oversea.CN.ContentMgmt.GoogleSearch.ServiceAdapter;
using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.GoogleSearch.BizProcess
{
    public class GoogleSearchBP
    {
        #region 主程序
        public void Starter()
        {
            string companyCode = ConfigurationManager.AppSettings["CompanyCode"].ToString();
            string unZippedPath = ConfigurationManager.AppSettings["UnzippedPath"].ToString();
            string zipPath = ConfigurationManager.AppSettings["ZipPath"].ToString();
            string zipFileName = ConfigurationManager.AppSettings["ZipFileName"].ToString();
            string sleepTime = ConfigurationManager.AppSettings["SleepTime"].ToString();
            sleepTime = (String.IsNullOrEmpty(sleepTime)) ? "3" : sleepTime;

            if (String.IsNullOrEmpty(unZippedPath) || String.IsNullOrEmpty(zipPath) || String.IsNullOrEmpty(zipFileName))
            {
                return;
            }

            DataSet dsResult = GetGoogleSearchProductList(companyCode);

            if (dsResult == null || dsResult.Tables[0].Rows.Count == 0)
            {
                return;
            }

            try
            {
                if (!Directory.Exists(unZippedPath))
                {
                    DirectoryInfo diUnZipped = Directory.CreateDirectory(unZippedPath);
                }
            }
            catch
            {
                SendFailEmail("GoogleSearch文件打包,指定的生成TXT文件目录不存在,生成失败!");
                return;
            }

            try
            {
                string zipPaths = zipPath.Substring(0, zipPath.LastIndexOf(@"\"));
                if (!Directory.Exists(zipPaths))
                {
                    DirectoryInfo diZip = Directory.CreateDirectory(zipPaths);
                }
            }
            catch
            {
                SendFailEmail("GoogleSearch文件打包,指定的压缩文件目录不存在,生成失败!");
                return;
            }

            int iRound = 1;
            while (iRound <= 3)
            {
                try
                {
                    if (File.Exists(unZippedPath + zipFileName))     //清空输出列表
                    {
                        File.Delete(unZippedPath + zipFileName);
                    }

                    if (File.Exists(zipPath))     //清空输出列表
                    {
                        File.Delete(zipPath);
                    }

                    break;
                }
                catch
                {
                    if (iRound == 3)
                    {
                        SendFailEmail("GoogleSearch文件打包,文件被占用,重复三次失败!");
                        return;
                    }
                    else
                    {
                        iRound = iRound + 1;
                        Thread.Sleep(int.Parse(sleepTime) * 60000);
                        continue;
                    }                    
                }
            }

            FileStream fs = new FileStream(unZippedPath + zipFileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            Regex regex = new Regex(@"<.+?>");  
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < 13; i++)
            {
                if (i == 12)
                {
                    sw.Write(dsResult.Tables[0].Columns[i].ToString().Replace("1", ""));  
                }
                else
                {
                    sw.Write(dsResult.Tables[0].Columns[i].ToString().Replace("1", "") + "\t");  
                }
            }

            sw.Write("\r\n");

            for (int j = 0; j < dsResult.Tables[0].Rows.Count; j++)
            {
                for (int k = 0; k < 13; k++)
                {
                    if (k == 0)
                    {
                        sw.Write(regex.Replace(dsResult.Tables[0].Rows[j][k].ToString(), "").Replace("\r", "").Replace("\n", " ").Replace("\t", " ") + "\t");     
                    }
                    else if (k == 3)
                    {
                        string categoryName = dsResult.Tables[0].Rows[j]["类型"].ToString();
                        //categoryName = categoryName.Substring(categoryName.LastIndexOf('>') + 1);
                        string bandName = dsResult.Tables[0].Rows[j]["品牌"].ToString();
                        sw.Write(dsResult.Tables[0].Rows[j][k].ToString().Replace("@CategoryName", HttpUtility.UrlEncode(categoryName, System.Text.Encoding.GetEncoding("gb2312")))
                            .Replace("@Brand", HttpUtility.UrlEncode(bandName, System.Text.Encoding.GetEncoding("gb2312"))).Replace("\t", " ") + "\t");
                    }
                    else if (k == 6)
                    {
                        sw.Write(dsResult.Tables[0].Rows[j][k].ToString().Replace("\t", " ").Replace("\"","") + "\t"); 
                    }
                    else if (k == 12)
                    {
                        string strScore = GetProductScore(dsResult.Tables[0].Rows[j]["SysNo"].ToString(), companyCode);
                        sw.Write(strScore);
                    }
                    else if (k == 2)
                    {
                        string strPerDec = GetPerformanceList(dsResult.Tables[0].Rows[j]["规格参数"].ToString()) + regex.Replace(dsResult.Tables[0].Rows[j]["产品描述"].ToString(), "");
                        strPerDec = strPerDec + " " + regex.Replace(dsResult.Tables[0].Rows[j][k].ToString(), "").Replace("\r", "").Replace("\n", " ").Replace("\t", " "); 
                        sw.Write(strPerDec.Replace("\r", "").Replace("\n", " ").Replace("\t", " ") + "\t");
                    }
                    else
                    {
                        sw.Write(dsResult.Tables[0].Rows[j][k].ToString().Replace("\t", " ") + "\t");
                    }
                }
                Console.WriteLine("当前处理：" + (j + 1) + "/" + dsResult.Tables[0].Rows.Count);
                sw.Write("\r\n");
            }

            sw.Flush();
            sw.Close();

            string []FileProperties=new string[3]; 
            //待压缩文件目录
            FileProperties[0] = unZippedPath;
            //压缩后的目标文件 
            FileProperties[1] = zipPath;
            //待压缩文件
            FileProperties[2] = zipFileName;
            ZipFileMain(FileProperties);

            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
        #endregion

        #region 处理数据取得
        /// <summary>
        /// 处理数据取得
        /// </summary>
        /// <returns></returns>
        private DataSet GetGoogleSearchProductList(string companyCode)
        {
            DataSet dsResult = GoogleSearchDA.GetGoogleSearchProductList(companyCode);
            return dsResult;
        }
        #endregion


        #region 压缩文件
        public void ZipFileMain(string[] args)
        {
            string[] filenames = Directory.GetFiles(args[0]);
            using (ZipOutputStream s = new ZipOutputStream(File.Create(args[1])))
            {
                s.SetLevel(9); // 0 - store only to 9 - means best compression
                foreach (string file in filenames) //String[] filename为待压缩文件列表
                {
                    FileInfo fi = new FileInfo(file);

                    if (!fi.Name.Equals(args[2]))
                    {
                        continue;                        
                    }

                    FileStream fsd = File.OpenRead(file);
                    byte[] buffer = new byte[fsd.Length];
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fi.Length; ;
                    s.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }
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
            mail.Subject = "GoogleSearch生成文件或打包失败";

            EmailComparisonCNServiceFacade.SendProductEmail(mail);
        }
        #endregion


        #region 产品简介
        private string GetPerformanceList(string strXml)
        {
            if (String.IsNullOrEmpty(strXml))
            {
                return "";
            }
            string strResult = "";

            XmlDocument xmls = new XmlDocument();

            try
            {
                xmls.LoadXml(strXml);
            }
            catch
            {
                return "";
            }

            if (xmls.GetElementsByTagName("Property").Count == 0)
            {
                return "";
            }

            for (int i = 0; i < xmls.GetElementsByTagName("Property").Count; i++)
            {
                string Key = (xmls.GetElementsByTagName("Property")[0].Attributes["Key"] == null) ? "" : xmls.GetElementsByTagName("Property")[i].Attributes["Key"].Value.ToString();
                string Value = (xmls.GetElementsByTagName("Property")[0].Attributes["Value"] == null) ? "" : xmls.GetElementsByTagName("Property")[i].Attributes["Value"].Value.ToString();

                if (String.IsNullOrEmpty(Key) || String.IsNullOrEmpty(Value))
                {
                    continue;
                }

                Key = (Key.Contains("_")) ? Key.Substring(Key.LastIndexOf('_') + 1) : Key;

                strResult = strResult + Key + ":" + Value + ",";
            }

            strResult = (strResult.Length > 1) ? strResult.Substring(0, strResult.Length - 1) + " " : strResult;

            return strResult;
        }
        #endregion

        #region 评分
        private string GetProductScore(string sysNo, string companyCode)
        {
            string strScore = "";
            DataSet dsResult = GoogleSearchDA.GetGoogleSearchScore(sysNo, companyCode);

            if (dsResult.Tables.Count == 0 || dsResult.Tables[0].Rows.Count == 0)
            {
                strScore = "0";
            }
            else
            {
                decimal ReviewCount = (String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["ReviewCount"].ToString())) ? 0 : decimal.Parse(dsResult.Tables[0].Rows[0]["ReviewCount"].ToString());
                decimal RatingScore = (String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["RatingScore"].ToString())) ? 0 : decimal.Parse(dsResult.Tables[0].Rows[0]["RatingScore"].ToString());

                strScore = (ReviewCount == 0) ? "0" : (RatingScore / ReviewCount).ToString("f1");
            }
            return strScore;
        }
        #endregion
    }
}