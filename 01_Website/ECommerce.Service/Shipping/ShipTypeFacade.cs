using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Shipping;
using ECommerce.Enums;
using ECommerce.SOPipeline;

namespace ECommerce.Facade.Shipping
{
    public  class ShipTypeFacade
    {
        /// <summary>
        /// 根据配送地区编号和支付方式编号取得所有支持的配送方式列表
        /// </summary>
        /// <param name="addressAreaID">配送地区编号</param>
        /// <param name="paymentCategory">支付类别</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> GetSupportedShipTypeList(int addressAreaID, PaymentCategory? paymentCategory)
        {
            return SOPipelineProcessor.GetSupportedShipTypeList(addressAreaID, paymentCategory);
        }

        /// <summary>
        /// 获取商家仓库的所有配送方式
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> Checkout_GetStockShippingType(int sellerSysNo)
        {
            return SOPipelineProcessor.Checkout_GetStockShippingType(sellerSysNo);
        }
    }
}
