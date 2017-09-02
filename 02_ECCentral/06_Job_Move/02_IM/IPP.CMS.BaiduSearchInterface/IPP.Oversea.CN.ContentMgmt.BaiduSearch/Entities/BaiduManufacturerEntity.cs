using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class BaiduManufacturerEntity
    {
        [DataMapping("ManufacturerSysNo", DbType.Int32)]
        public int ManufacturerSysNo
        {
            get;
            set;
        }

        [DataMapping("ManufacturerName", DbType.String)]
        public string ManufacturerName
        {
            get;
            set;
        }

        [DataMapping("Type", DbType.Int32)]
        public int Type
        {
            get;
            set;
        }

        [DataMapping("ProductCount", DbType.Int32)]
        public int ProductCount
        {
            get;
            set;
        }

        [DataMapping("MinPrice", DbType.Decimal)]
        public decimal MinPrice
        {
            get;
            set;
        }

        [DataMapping("InitialPingYin", DbType.String)]
        public string InitialPingYin
        {
            get;
            set;
        }
    }
}
