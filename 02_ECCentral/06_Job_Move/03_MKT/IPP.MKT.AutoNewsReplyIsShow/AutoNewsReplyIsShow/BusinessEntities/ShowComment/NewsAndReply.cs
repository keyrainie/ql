using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{
    [Serializable]
    public class NewsAndReply
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo
        {
            get;
            set;
        }

        [DataMapping("ReplyContent", DbType.String)]
        public string ReplyContent
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

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime CreateDate
        {
            get;
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

        [DataMapping("HolidayDate", DbType.DateTime)]
        public DateTime? HolidayDate
        {
            get;
            set;
        }

        public DateTime? HolidayDateFrom
        {
            get;
            set;
        }

        public DateTime? HolidayDateTo
        {
            get;
            set;
        }

        [DataMapping("BlockedService", DbType.String)]
        public string BlockedService
        {
            get;
            set;
        }
    }
}
