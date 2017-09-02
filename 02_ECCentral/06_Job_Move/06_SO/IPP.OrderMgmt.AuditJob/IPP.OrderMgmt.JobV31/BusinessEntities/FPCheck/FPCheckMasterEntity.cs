using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;


namespace IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck
{
    [Serializable]
    public class FPCheckMasterEntity : EntityBase
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
            get; 
            set;
        }

        [DataMapping("Status", DbType.Int32)]
        public int Status 
        {
            get; 
            set;
        }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int? CreateUserSysNo
        { 
            get;
            set; 
        }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime? CreateDate 
        {   get;
            set;
        }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime? LastEditDate 
        { 
            get; 
            set; 
        }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int? LastEditUserSysNo 
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

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode 
        { 
            get;
            set;
        }
    }
}
