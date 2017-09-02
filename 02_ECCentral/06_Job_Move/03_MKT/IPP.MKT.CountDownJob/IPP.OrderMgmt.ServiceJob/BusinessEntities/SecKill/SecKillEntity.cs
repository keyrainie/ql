using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using System.ComponentModel;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill
{
    public class SecKillEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNo { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("StartTime", DbType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataMapping("EndTime", DbType.DateTime)]
        public DateTime EndTime { get; set; }

        [DataMapping("CountDownCurrentPrice", DbType.Decimal)]
        public decimal CountDownCurrentPrice { get; set; }

        [DataMapping("CountDownCashRebate", DbType.Int32)]
        public decimal CountDownCashRebate { get; set; }

        [DataMapping("CountDownPoint", DbType.Int32)]
        public int CountDownPoint { get; set; }

        [DataMapping("CountDownQty", DbType.Int32)]
        public int CountDownQty { get; set; }

        [DataMapping("SnapShotCurrentPrice", DbType.Decimal)]
        public decimal SnapShotCurrentPrice { get; set; }

        [DataMapping("SnapShotCashRebate", DbType.Decimal)]
        public decimal SnapShotCashRebate { get; set; }

        [DataMapping("SnapShotPoint", DbType.Int32)]
        public int SnapShotPoint { get; set; }

        [DataMapping("AffectedVirtualQty", DbType.Int32)]
        public int AffectedVirtualQty { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        private string m_AffectedStock;
        [DataMapping("AffectedStock", DbType.String)]
        public string AffectedStock
        {
            get
            {
                return m_AffectedStock;
            }
            set
            {
                m_AffectedStock = value;
                InitWareHouseList();
            }
        }
        [DataMapping("IsPromotionSchedule", DbType.Int32)]
        public int IsPromotionSchedule { get; set; }

        [DataMapping("SnapShotCurrentVirtualQty", DbType.String)]
        public string SnapShotCurrentVirtualQty { get; set; }

        [DataMapping("Reasons", DbType.String)]
        public string Reasons { get; set; }

        [DataMapping("IsCountDownAreaShow", DbType.Int32)]
        public int IsCountDownAreaShow { get; set; }

        [DataMapping("PromotionType", DbType.String)]
        public string PromotionType { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("IsLimitedQty", DbType.Int32)]
        public int IsLimitedQty { get; set; }

        [DataMapping("IsReservedQty", DbType.Int32)]
        public int IsReservedQty { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; }

        private List<InventoryStockEntity> m_WarehouseList;
        public List<InventoryStockEntity> WarehouseList
        {
            get
            {
                return m_WarehouseList;
            }
        }
        private void InitWareHouseList()
        {
            m_WarehouseList = new List<InventoryStockEntity>();
            if (!string.IsNullOrEmpty(AffectedStock))
            {
                string[] strWarehouse = AffectedStock.Split(';');
                string[] strItem;
                int warehouseSysNumber = 0;
                int warehouseCountdownQty = 0;
                foreach (string item in strWarehouse)
                {
                    warehouseSysNumber = 0;
                    warehouseCountdownQty = 0;

                    strItem = item.Split(':');
                    if (strItem.Length == 1 && int.TryParse(strItem[0], out warehouseSysNumber))
                    {
                        m_WarehouseList.Add(new InventoryStockEntity() { StockSysNo = warehouseSysNumber, SubCountdownQty = 0, ProductSysNo = this.ProductSysNo });
                    }
                    else if (strItem.Length >= 2 && int.TryParse(strItem[0], out warehouseSysNumber) && int.TryParse(strItem[1], out warehouseCountdownQty))
                    {
                        m_WarehouseList.Add(new InventoryStockEntity() { StockSysNo = warehouseSysNumber, SubCountdownQty = warehouseCountdownQty, ProductSysNo = this.ProductSysNo });
                    }
                }
            }
        }
    }

    public enum CountdownStatus
    {
        [Description("审核未通过")]
        VerifyFaild = -4,
        [Description("待审核")]
        WaitForVerify = -3,
        [Description("中止")]
        Interupt = -2,
        [Description("作废")]
        Abandon = -1,
        [Description("就绪")]
        Ready = 0,
        [Description("运行")]
        Running = 1,
        [Description("完成")]
        Finish = 2,
    }

}
