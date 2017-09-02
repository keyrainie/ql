using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Inventory;
using ECommerce.Service.Inventory;

namespace ECommerce.Web.Controllers
{
    public class InventoryController : WWWControllerBase
    {
        //
        // GET: /Inventory/
        
        public ActionResult ItemInventoryQuery(int productSysNo)
        {
            return View(productSysNo);
        }

        [HttpPost]
        public JsonResult QueryProductInventory()
        {
            InventoryQueryFilter qFilter = BuildQueryFilterEntity<InventoryQueryFilter>();
            qFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo.ToString();
            var result = InventoryService.QueryProductInventory(qFilter);

            return AjaxGridJson(result);
        }

        [HttpPost]
        public JsonResult QueryCardItemOrders()
        {
            InventoryItemCardQueryFilter qFilter = BuildQueryFilterEntity<InventoryItemCardQueryFilter>();
            qFilter.MerchantSysNo = UserAuthHelper.GetCurrentUser().SellerSysNo.ToString();
            var result = InventoryService.QueryCardItemOrders(qFilter);

            return AjaxGridJson(result);
        }

    }
}
