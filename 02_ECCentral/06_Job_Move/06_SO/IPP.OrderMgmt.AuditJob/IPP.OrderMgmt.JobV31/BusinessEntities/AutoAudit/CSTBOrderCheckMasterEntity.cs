/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author       :  Ray.L.Xing
 * Create Date  :  2010-7-29 15:06:15 
 * Usage        :  
 * File         :  
 *
 * RevisionHistory
 * Date         Author               
 * Description  : 
 * 
*****************************************************************/
using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;

using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities;
namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    [Serializable]
    public class CSTBOrderCheckMasterEntity : EntityBase
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo 
        { 
            get;
            set;
        }

        [DataMapping("CheckType", DbType.String)]
        public string CheckType 
        {
            get; 
            set; 
        }

        [DataMapping("Description", DbType.String)]
        public string Description 
        {
            get; set;
        }

        [DataMapping("Status", DbType.Int32)]
        public int Status 
        { 
            get; set;
        }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int? CreateUserSysNo 
        { 
            get; set;
        }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime? CreateDate 
        { 
            get; set;
        }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime? LastEditDate 
        { 
            get; set; 
        }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int? LastEditUserSysNo 
        { 
            get; set; 
        }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode
        { 
            get; set;
        }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode 
        {
            get; set;
        }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode 
        { 
            get; set;
        }

    }
}
