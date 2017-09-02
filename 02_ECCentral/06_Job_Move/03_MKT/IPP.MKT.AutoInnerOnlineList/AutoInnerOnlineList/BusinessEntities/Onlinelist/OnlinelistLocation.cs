using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.Utilities;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities
{
    [Serializable]
    public class OnlineListLocation
    {
        [DataMapping("OnlineListlocationSysNo", DbType.Int32)]
        public int OnlinelocationSysNo
        {
            get;
            set;
        }

        [DataMapping("PageType", DbType.Int32)]
        public int PageType
        {
            get;
            set;
        }

        [DataMapping("PageID", DbType.Int32)]
        public int PageID
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

        [DataMapping("PositionID", DbType.Int32)]
        public int PositionID
        {
            get;
            set;
        }

        [DataMapping("Priority", DbType.Int32)]
        public int Priority
        {
            get;
            set;
        }

        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode
        {
            get;
            set;
        } 

        [DataMapping("Status", DbType.AnsiStringFixedLength)]
        public string Status
        {
            get;
            set;
        }



    }
}
