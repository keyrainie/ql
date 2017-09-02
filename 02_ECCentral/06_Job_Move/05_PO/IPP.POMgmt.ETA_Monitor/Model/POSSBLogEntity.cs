using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using Newegg.Oversea.Framework.Biz;

namespace IPPOversea.POmgmt.ETA.Model
{
    public class POSSBLogEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("POSysNo", DbType.Int32)]
        public int POSysNo { get; set; }

        [DataMapping("Content", DbType.String)]
        public string Content { get; set; }

        [DataMapping("ActionType", DbType.StringFixedLength)]
        public string ActionType { get; set; }

        [DataMapping("InUser", DbType.Int32)]
        public int InUser { get; set; }

        [DataMapping("Indate", DbType.DateTime)]
        public DateTime Indate { get; set; }

        [DataMapping("ErrMSg", DbType.String)]
        public string ErrMSg { get; set; }

        [DataMapping("ErrMSgTime", DbType.DateTime)]
        public DateTime ErrMSgTime { get; set; }

        [DataMapping("SendErrMail", DbType.StringFixedLength)]
        public string SendErrMail { get; set; }

        [DataMapping("LanguageCode", DbType.StringFixedLength)]
        public string LanguageCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }
    }
}
