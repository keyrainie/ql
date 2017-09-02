using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    public class ProductVirtualInfoEntity
    {
      
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("ProductLink", DbType.String)]
        public string ProductLink { get; set; }

        [DataMapping("VirtualQty", DbType.Int32)]
        public int VirtualQty { get; set; }

        [DataMapping("PMRequestNote", DbType.String)]
        public string PMRequestNote { get; set; }

        [DataMapping("AvailableQty", DbType.Int32)]
        public int AvailableQty { get; set; }

        [DataMapping("ConsignQty", DbType.DateTime)]
        public int ConsignQty { get; set; }

        [DataMapping("StartTime", DbType.DateTime)]
        public DateTime? StartTime { get; set; }

        [DataMapping("EndTime", DbType.DateTime)]
        public DateTime? EndTime { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        public string StatusDescription
        {
            get 
            {
                string str = "";

                switch (Status)
                {
                    case 0:
                        str = "待审核";
                        break;
                    case 1:
                        str = "已审核";
                        break;
                    case 2:
                        str = "运行中";
                        break;
                    case 3:
                        str = "已完成";
                        break;
                    case 4:
                        str = "关闭中";
                        break;
                    case -1:
                        str = "审核未通过";
                        break;
                }

                return str;
            }
        }


        public int HoldVirtualQty
        {
            get 
            {
                if (AvailableQty + ConsignQty < 0)
                {
                    return -AvailableQty;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
