using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.ThirdPart.Interfaces.Interface;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.ThirdPart.Interfaces.Contract;
using IPP.Oversea.CN.POASNMgmt.BusinessEntities;


namespace WMSWaitInStock.ServiceAdapter
{
    public class ThirdPartServiceAdapter
    {
        public static void InStock(POEntity po)
        {

            WMSRequest request=new WMSRequest()
            {
                 Header=ServiceAdapterHelper.ApplyMessageHeader()
                  , OrderInfo= new WMSOrderInfo()
                  { OrderSysNo= po.SysNo.Value
                    , OrderType="PO"
                    , WarehouseNumber=po.StockSysNo.ToString()
                  }
            };

            List<WMSRequestProduct> products=new List<WMSRequestProduct>();

            foreach(var item in po.POItems)
            {
                products.Add(
                    new WMSRequestProduct(){ 
                        Code=item.ProductID
                        , UnitQty=item.PurchaseQty
                    });
            }

            WMSRequestWaitInStock req = new WMSRequestWaitInStock()
            {
                Action = "ImportAsn"
                ,OrderCode = po.SysNo.ToString()
                ,WarehouseCode = po.StockSysNo.ToString()
                ,ProductList = products
            };
            request.WaitInStock = req;

            IWMSMaintain service = ServiceBroker.FindService<IWMSMaintain>();
            try 
            {
                WMSResponse response =  service.WaitInStock(request);
                ServiceAdapterHelper.DealServiceFault(response.Faults);
            }
            finally 
            {

                ServiceBroker.DisposeService<IWMSMaintain>(service);
            }
        }
    }
}
