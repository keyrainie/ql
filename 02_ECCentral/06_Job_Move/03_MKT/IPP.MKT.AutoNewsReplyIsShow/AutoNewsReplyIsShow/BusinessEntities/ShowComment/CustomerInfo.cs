using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities
{
    [Serializable]
    public class CustomerInfo 
    {
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSystemNumber
        {
            get;
            set;
        }

        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID
        {
            get;
            set;
        }

        [DataMapping("CustomerName", DbType.String)]
        public string CustomerName
        {
            get;
            set;
        }

        [DataMapping("Email", DbType.String)]
        public string EmailAddress
        {
            get;
            set;
        }

        [DataMapping("Rank", DbType.Int32)]
        public int CustomerRank
        {
            get;
            set;
        }       

        [DataMapping("CustomerGotPoint", DbType.Int32)]
        public int CustomerGotPoint
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
        [DataMapping("IsSubscribe", DbType.Int32)]
        public int IsSubscribe
        {
            get;
            set;
        }

    }
}
