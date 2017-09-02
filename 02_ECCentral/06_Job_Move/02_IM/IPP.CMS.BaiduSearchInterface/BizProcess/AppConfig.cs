/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Phoebe Zhang (Phoebe.F.Zhang@Newegg.com)
 *  Date:    2009-05-20 17:28:55
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System.Configuration;
using System.Collections;
using System;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{    /// <summary>
    /// Summary description for AppConfig.
    /// </summary>
    public static class AppConfig
    {
        static AppConfig()
        {

        }

        public static string ErrorLogFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ErrorLogFolder"];
            }
        }

        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"] ?? "8601";
            }
        }

        public static string IndexFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["IndexFileName"];
            }
        }

        public static string ItemFileName
        {
            get
            {
                string itemFileName = ConfigurationManager.AppSettings["ItemFileName"];
                int length = itemFileName.LastIndexOf(".xml");

                if (length > -1)
                {
                    itemFileName = ItemFileName.Substring(0, length);
                }

                return itemFileName;
            }
        }

        public static int ItemCountPerFile
        {
            get
            {
                int itemCountPerFile = 0;

                if (!int.TryParse(ConfigurationManager.AppSettings["ItemCountPerFile"], out itemCountPerFile))
                {
                    itemCountPerFile = 2000;
                }

                return itemCountPerFile;
            }
        }

        public static string StoreName
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreName"] ?? "新蛋商城";
            }
        }

        public static string StorePic
        {
            get
            {
                return ConfigurationManager.AppSettings["StorePic"] ?? @"http://c1.neweggimages.com.cn/WebResources/2009/Default/Nest/common/logo.gif";
            }
        }

        public static string Service
        {
            get
            {
                return ConfigurationManager.AppSettings["Service"] ?? @"正规发票\全国联保\七天退换货";
            }
        }

        public static string ChangeFrequency
        {
            get
            {
                return ConfigurationManager.AppSettings["ChangeFrequency"] ?? @"daily";
            }
        }

        public static string LastModifiedDateTime
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        public static string Priority
        {
            get
            {
                return ConfigurationManager.AppSettings["Priority"] ?? "0.8";
            }
        }

        public static string Category2Address
        {
            get
            {
                return ConfigurationManager.AppSettings["Category2Address"] ?? @"http://www.newegg.com.cn/Category/{0}.htm";
            }
        }

        public static string Category3Address
        {
            get
            {
                return ConfigurationManager.AppSettings["Category3Address"] ?? @"http://www.newegg.com.cn/SubCategory/{0}.htm";
            }
        }


        public static string FlagshipBrandAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["FlagshipBrandAddress"] ?? @"http://www.newegg.com.cn/FlagshipStore/{0}-{1}.htm";
            }
        }


        public static string CommonBrandAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["CommonBrandAddress"] ?? @"http://www.newegg.com.cn/Search.aspx?N={0}";
            }
        }

        public static string CategoryBrandAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["CategoryBrandAddress"] ?? @"http://www.newegg.com.cn/SubCategory/{0}.htm?N={1}+{2}";
            }
        }

    }
}
