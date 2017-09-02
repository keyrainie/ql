using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.Common
{
    [Serializable]
    public class HolidayEntity
    {

        [DataMapping("HolidayDate", DbType.DateTime)]
        public DateTime HolidayDate
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

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int? ShipTypeSysNo
        {
            get;
            set;
        }

        public int DeliveryType
        {
            get
            {
                if (BlockedService == "TwoTimeToOneTime")
                    return 1;
                else
                    return 0;
            }
        }



    }
}
