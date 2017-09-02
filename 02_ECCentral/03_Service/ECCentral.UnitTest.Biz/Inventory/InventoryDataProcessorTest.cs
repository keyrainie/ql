using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.BizEntity.Inventory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECCentral.UnitTest.BizProcessor.Inventory
{
    [TestClass]    
    public class InventoryDataProcessorTest
    {
        [TestMethod]
        public void Test_Inventory_GetStockInfo()
        {
            
        }
         [TestMethod]
        public void Test_AdjustProductInventory()
        {
            InventoryAdjustContractProcessor process = new InventoryAdjustContractProcessor();
            InventoryAdjustContractInfo entity = new InventoryAdjustContractInfo();
            entity.SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order;
            entity.SourceActionName = InventoryAdjustSourceAction.Audit;         
            process.ProcessAdjustContract(entity);
        }

        [TestMethod]
         public void Test_ProductRing()
         {
             ProductInventoryProcessor process = new ProductInventoryProcessor();
             process.GetProductRingAndSendEmail();
         }
        
    }
}
