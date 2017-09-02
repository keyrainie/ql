/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2009-11-11
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;

using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.AutoPricingDisable.Entities
{
    public class UpdateAutoPricingDisableEntity : DefaultQueryEntity
    {
        [DataMapping("ItemNo", DbType.String)]
        public string ItemNo
        {
            get;
            set;
        }

        [DataMapping("CompetitorWebSite", DbType.String)]
        public string CompetitorWebSite
        {
            get;
            set;
        }

        [DataMapping("UnitPrice", DbType.String)]
        public string UnitPrice
        {
            get;
            set;
        }

        [DataMapping("StockStatus", DbType.String)]
        public string StockStatus
        {
            get;
            set;
        }

        [DataMapping("Stock", DbType.String)]
        public string Stock
        {
            get;
            set;
        }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMapping("Type", DbType.Int32)]
        public int Type
        {
            get;
            set;
        }
    }

    public class SalesMailEntity : DefaultDataEntity
    {
        [DataMapping("UserID", DbType.String)]
        public string UserID
        {
            get;
            set;
        }
        [DataMapping("UserName", DbType.String)]
        public string UserName
        {
            get;
            set;
        }
        [DataMapping("toEmail", DbType.String)]
        public string toEmail
        {
            get;
            set;
        }

        [DataMapping("ccEmail", DbType.String)]
        public string ccEmail
        {
            get;
            set;
        }
    }

    public class MailEntity
    {
        public string BCC { get; set; }

        public string Body { get; set; }

        public string CC { get; set; }

        public string From { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }

        public string CompanyCode { get; set; }
    }
}