using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Inventory
{
    public class InventoryAdjustContractInfo
    {
        public InventoryAdjustSourceBizFunction SourceBizFunctionName { get; set; }
        public InventoryAdjustSourceAction SourceActionName { get; set; }
        public bool IsOutStockAbandon { get; set; }
        public string ReferenceSysNo { get; set; }
        public List<InventoryAdjustItemInfo> AdjustItemList { get; set; }
        public CostBillType CostType { get { return GetCostType(); } }

        public enum CostBillType
        {
            UnKnown = 0,
            //库存增加单据，记CostIn           
            IT = 1,
            RT = 2,
            AutoRMA = 3,
            LendReturn = 4,
            //库存减少单据，记CostOut          
            SO = 20,
            Lend = 21,
            OR = 22,
            //以下三种为混合单据，由于调整item是个集合，类型不再分In和Out
            PO = 40,
            Adjust = 41,
            Shift = 42,
            Transfer = 43,
            CostAdjust = 44
        }
        /// <summary>
        /// 根据单据类型和调整类型确定成本类型，不处理的为CostBillType.UnKnown
        /// </summary>
        /// <returns></returns>
        private CostBillType GetCostType()
        {
            switch (SourceBizFunctionName)
            {
                case InventoryAdjustSourceBizFunction.Inventory_AdjustRequest:
                    return CostBillType.Adjust;
                case InventoryAdjustSourceBizFunction.Inventory_ConvertRequest:
                    return CostBillType.Transfer;
                case InventoryAdjustSourceBizFunction.Inventory_ShiftRequest:
                    return CostBillType.Shift;
                case InventoryAdjustSourceBizFunction.Inventory_LendRequest:
                    if (SourceActionName == InventoryAdjustSourceAction.Return)
                    {
                        return CostBillType.LendReturn;
                    }
                    else
                    {
                        return CostBillType.Lend;
                    }
                case InventoryAdjustSourceBizFunction.Inventory_VirtualRequest://不影响实物库存，不处理
                    break;
                case InventoryAdjustSourceBizFunction.SO_Order://出库由SP处理
                    break;
                case InventoryAdjustSourceBizFunction.PO_Order://出库由SP处理
                    break;
                case InventoryAdjustSourceBizFunction.RMA_OPC://出库由SP处理
                    break;
                case InventoryAdjustSourceBizFunction.Seller_Inventory://暂不实现先进先出
                    break;
                case InventoryAdjustSourceBizFunction.Channel_Inventory://暂不实现先进先出
                    break;
                default:
                    break;
            }
            return CostBillType.UnKnown;
        }
    }
}
