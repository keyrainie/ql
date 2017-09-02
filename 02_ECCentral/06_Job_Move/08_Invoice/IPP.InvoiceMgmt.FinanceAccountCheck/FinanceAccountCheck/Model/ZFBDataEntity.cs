/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author       :  Allan.K.Li
 * Create Date  :  2010-4-9 10:25:59 
 * Usage        :  
 * File         :  
 *
 * RevisionHistory
 * Date         Author               
 * Description  : 
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.Model
{
    public class ZFBDataEntity
    {
        public int SysNo { get; set; }

        public int SoSysNo { get; set; }

        public int PayTermsNo { get; set; }

        public string PayTerms { get; set; }

        public DateTime PayedDate { get; set; } 

        public decimal PayedAmt { get; set; }

        public string SerialNumber { get; set; }

        public string OutOrderNo { get; set; }

        public string PayedUserTag { get; set; }

        public string PayedUserName { get; set; }

        public string PayedUserNo { get; set; }

        public string PartnerName { get; set; }

        public string TradeType { get; set; }

        public string AttachInfo { get; set; }

        public string AttachInfo2 { get; set; }

        public string InUser { get; set; }

        public DateTime InDate { get; set; }

        public string Status { get; set; }

        public string CompanyCode { get; set; }

        public string CurrencyCode { get; set; }

        public string LanguageCode { get; set; }

        public string StoreCompanyCode { get; set; }
    }
}
