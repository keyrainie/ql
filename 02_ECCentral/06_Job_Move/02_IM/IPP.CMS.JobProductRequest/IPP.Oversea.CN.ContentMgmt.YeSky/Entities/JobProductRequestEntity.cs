/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2009-12-08
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
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.Entity;

using IPP.Oversea.CN.ContentManagement.BusinessEntities.Common;

namespace IPP.Oversea.CN.ContentMgmt.JobProductRequest.Entities
{
    public class JobProductRequestEntity : DefaultQueryEntity
    {
        public int? SysNo { get; set; }
        public int? VP_ProductSysno { get; set; }
        public string ProductName { get; set; }
        public string ProductMode { get; set; }
        public string ProductID { get; set; }
        public int? ManufacturerSysno { get; set; }
        public int? C3SysNo { get; set; }
        public string ProductLink { get; set; }
        public string AccessoriesMemo { get; set; }
        public int? HostWarrantyDay { get; set; }
        public int? PartWarrantyDay { get; set; }
        public string Warranty { get; set; }
        public string ServicePhone { get; set; }
        public string ServiceInfo { get; set; }
        public string Note { get; set; }
        public int? Weight { get; set; }
        public int? IsLarge { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MinPackNumber { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PromotionTitle { get; set; }
        public string Color { get; set; }
        public string Auditor { get; set; }
        public DateTime? AuditDate { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public DateTime? InDate { get; set; }
        public string InUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditUser { get; set; }
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
    }

}