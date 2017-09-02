using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.CustomerMgmt.SendRMAEveryDayList
{
    [Serializable]
    public class DBModelPM
    {
        [DataMapping("pmusersysno", DbType.Int32)]
        public int pmusersysno
        {
            get;
            set;
        }

        [DataMapping("email", DbType.String)]
        public string email
        {
            get;
            set;
        }

        [DataMapping("username", DbType.String)]
        public string username
        {
            get;
            set;
        }

        [DataMapping("PMGroupSysNo",DbType.Int32)]
        public int PMGroupSysNo { get; set; }
    }

    [Serializable]
    public class DBModel
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

        [DataMapping("productid", DbType.String)]
        public String productid
        {
            get;
            set;
        }

        [DataMapping("productname", DbType.String)]
        public string productname
        {
            get;
            set;
        }

        [DataMapping("CustomerDesc", DbType.String)]
        public string CustomerDesc
        {
            get;
            set;
        }

        [DataMapping("status", DbType.Int32)]
        public int status
        {
            get;
            set;
        }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int? PMUserSysNo
        {
            get;
            set;
        }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName
        {
            get;
            set;
        }

        [DataMapping("LastVendorSysNo", DbType.Int32)]
        public int LastVendorSysNo
        {
            get;
            set;
        }

        [DataMapping("Cost", DbType.Decimal)]
        public decimal? Cost
        {
            get;
            set;
        }
    }

    [Serializable]
    public class DBModelPMInfo
    {
        [DataMapping("UserName", DbType.String)]
        public string UserName
        {
            get;
            set;
        }
    }
    
    public struct DBParam
    {
        public string reportName;
        public string CommandName;
        public int PMUserSysNo;
        public string PMUserName;
        public DateTime? today;
        public DateTime? yesterday;
        public DateTime? daybefore21;
        public int status;
        public string statusName;
        public int? status1;
    }
}
