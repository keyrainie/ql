using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity;

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 备货中心
    /// </summary>
    [VersionExport(typeof(ProductCenterProcessor))]
    public class ProductCenterProcessor
    {
        public int CreateBasketItemsForPrepare(List<ProductCenterItemInfo> list)
        {
            List<BasketItemsInfo> returnList = new List<BasketItemsInfo>();
            list.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    if (y.PurchaseQty != 0)
                    {
                        BasketItemsInfo info = new BasketItemsInfo();
                        info.OrderPrice = Convert.ToDecimal(y.Price);
                        info.ProductSysNo = x.ItemSysNumber;
                        info.ProductID = x.ItemCode;
                        info.Quantity = y.PurchaseQty;
                        info.StockSysNo = Convert.ToInt32(y.WareHouseNumber);
                        info.IsTransfer = y.NeedBufferEnable == ECCentral.BizEntity.Inventory.YNStatus.Yes ? 1 : 0;
                        info.LastVendorSysNo = null;
                        info.ReadyQuantity = x.SuggestQtyAll;
                        info.CompanyCode = x.CompanyCode;
                        returnList.Add(info);
                    }
                });

            });
            if (returnList.Count <= 0)
            {
                throw new BizException("你没有要选择的商品,或者商品信息 （采购数量，采购价格）设置不正确!");
            }
            return ObjectFactory<IPOBizInteract>.Instance.CreateBasketItemsForPrepare(returnList);
        }

    }
}
