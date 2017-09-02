using System;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities;

namespace IPP.Oversea.CN.CustomerMgmt.SplitPhone
{
    [Serializable]
    public class Customer
    {
        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID
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

        [DataMapping("Phone", DbType.String)]
        public string Phone
        {
            get;
            set;
        }

        [DataMapping("CellPhone", DbType.String)]
        public string CellPhone
        {
            get;
            set;
        }
    }
}