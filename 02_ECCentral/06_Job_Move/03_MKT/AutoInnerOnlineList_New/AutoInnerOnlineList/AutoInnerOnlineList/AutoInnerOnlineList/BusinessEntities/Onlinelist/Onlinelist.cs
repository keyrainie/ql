using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities
{
    [Serializable]
    public class OnlineList
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

        [DataMapping("OnlineListlocationSysNo", DbType.Int32)]
        public int OnlineLocationSysNo
        {
            get;
            set;
        }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo
        {
            get;
            set;
        }

        [DataMapping("ProductGroupSysNo", DbType.Int32)]
        public int ProductGroupSysNo
        {
            get;
            set;
        }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID
        {
            get;
            set;
        }


        [DataMapping("ProductName", DbType.String)]
        public string ProductName
        {
            get;
            set;
        }

        [DataMapping("ProductStatus", DbType.Int32)]
        public int ProductStatus
        {
            get;
            set;
        }

        [DataMapping("FirstOnlineTime", DbType.DateTime)]
        public DateTime FirstOnlineTime
        {
            get;
            set;
        }


        [DataMapping("BrandSysNo", DbType.Int32)]
        public int BrandSysNo
        {
            get;
            set;
        }

        [DataMapping("Days", DbType.Int32)]
        public int Days
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

        /// <summary>
        /// 失效开始日期
        /// </summary>
        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime? BeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// 失效结束日期
        /// </summary>
        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime? EndDate
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

        [DataMapping("CreateUserName", DbType.String)]
        public string CreateUserName
        {
            get;
            set;
        }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime LastEditDate
        {
            get;
            set;
        }

        [DataMapping("LastEditUserName", DbType.String)]
        public string LastEditUserName
        {
            get;
            set;
        }

        [DataMapping("OnlineQty", DbType.Int32)]
        public int OnlineQty
        {
            get;
            set;
        }

        [DataMapping("ShiftQty", DbType.Int32)]
        public int ShiftQty
        {
            get;
            set;
        }

        [DataMapping("adjustmentprice", DbType.Decimal)]
        public decimal AdjustmentPrice
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

        [ReferencedEntity(typeof(OnlineListLocation))]
        public OnlineListLocation locationInfo
        {
            get;
            set;
        }

        [DataMapping("W1", DbType.Int32)]
        public int W1
        {
            get;
            set;
        }

        [DataMapping("W4", DbType.Int32)]
        public int W4
        {
            get;
            set;
        }
        [DataMapping("M1", DbType.Int32)]
        public int M1
        {
            get;
            set;
        }

        [DataMapping("M3", DbType.Int32)]
        public int M3
        {
            get;
            set;
        }

        [DataMapping("C1SysNo", DbType.Int32)]
        public int C1SysNo { get; set; }

        [DataMapping("C1Name", DbType.String)]
        public string C1Name { get; set; }

        [DataMapping("C2SysNo", DbType.Int32)]
        public int C2SysNo { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal CurrentPrice { get; set; }

        public override string ToString()
        {
            return ProductSysNo.ToString();
        }
    }
}
