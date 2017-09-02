using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ThirdPart.Interface;
using ECCentral.Service.Utility;
using ECCentral.Service.ThirdPart.SqlDataAccess;

namespace ECCentral.Service.ThirdPart.Adapter
{
    [VersionExport(typeof(IAdjustERPInventory))]
    public class ERPInventoryProcessor : IAdjustERPInventory
    {
        private ERPItemInfoDA erpDA = new ERPItemInfoDA();

        public string AdjustERPInventory(ERPInventoryAdjustInfo adjustInfo)
        {
            string errMsg = string.Empty;
           
            erpDA.AdjustERPItemInventory(adjustInfo);
            erpDA.AddERPInventoryAdjustLog(adjustInfo);
            
            return errMsg;
        }

        public ERPItemInventoryInfo GetERPItemInventoryInfoByProductSysNo(int productSysNo)
        {
            ERPItemInventoryInfo item = erpDA.GetERPItemInventoryByProductSysNo(productSysNo);

            if(item == null)
            {
                item = new ERPItemInventoryInfo
                            { 
                                ProductSysNo = productSysNo,
                                HQQuantity = 0,
                                DeptQuantity = 0,
                                B2CSalesQuantity = 0
                            };
            }           

            return item;
        }

        public ERPItemInventoryInfo GetERPItemInventoryInfoByERPProductId(int erpProductId)
        {
            ERPItemInventoryInfo item = erpDA.GetERPItemInventoryByERPProductID(erpProductId);

            if (item == null)
            {
                item = new ERPItemInventoryInfo
                {
                    ERPProductId = erpProductId,
                    HQQuantity = 0,
                    DeptQuantity = 0,
                    B2CSalesQuantity = 0
                };
            }   

            return item;
        }
    }

    [VersionExport(typeof(ISyncBusiness))]
    public class ERPBusinessProcessor : ISyncBusiness
    {
       
    }
    
}
