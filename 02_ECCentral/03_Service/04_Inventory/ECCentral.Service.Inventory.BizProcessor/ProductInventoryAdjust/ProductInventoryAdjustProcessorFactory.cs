using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ProductInventoryAdjustProcessorFactory))]
    public class ProductInventoryAdjustProcessorFactory
    {
        public IProductInventoryAdjustProcessor CreateProductInventoryAdjustProcessor(InventoryAdjustSourceBizFunction bizFunction)
        {
            switch (bizFunction)
            {                              
                case InventoryAdjustSourceBizFunction.PO_Order:    //先入先出SP实现
                    return ObjectFactory<ProductInventoryAdjustByPOOrderProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.RMA_OPC:     //先入先出SP实现
                    return ObjectFactory<ProductInventoryAdjustByRMAOPCProcessor>.Instance;                    
                case InventoryAdjustSourceBizFunction.Seller_Inventory:
                    return ObjectFactory<ProductInventoryAdjustBySellerProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.SO_Order:   //先入先出SP实现
                    return ObjectFactory<ProductInventoryAdjustBySOOrderProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.Channel_Inventory:
                    return ObjectFactory<ProductInventoryAdjustByChannelProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.Inventory_AdjustRequest:   //先入先出程序实现
                    return ObjectFactory<ProductInventoryAdjustByAdjustRequestProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.Inventory_ConvertRequest:  //先入先出程序实现
                    return ObjectFactory<ProductInventoryAdjustByConvertRequestProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.Inventory_LendRequest:     //先入先出程序实现
                    return ObjectFactory<ProductInventoryAdjustByLendRequestProcessor>.Instance;
                case InventoryAdjustSourceBizFunction.Inventory_ShiftRequest:    //先入先出程序实现
                    return ObjectFactory<ProductInventoryAdjustByShiftRequestProcessor>.Instance;                    
                case InventoryAdjustSourceBizFunction.Inventory_VirtualRequest:
                    return ObjectFactory<ProductInventoryAdjustByVirtualRequestProcessor>.Instance;                    
                default:
                    throw new NotSupportedException("Not Supported BizFunction");
                    
            }
        }
    }
}
