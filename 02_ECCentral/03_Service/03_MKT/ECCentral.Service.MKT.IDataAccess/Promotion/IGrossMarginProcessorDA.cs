using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess 
{

    public interface IGrossMarginProcessorDA
    {
        /// <summary>
        /// 取得指定商品所有的有效赠品及赠品数量 
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DataTable GetSaleGiftCurrentGiftProductsForVendor(int productSysNo);

        /// <summary>
        /// 获取指定商品，指定赠品活动的非“买满即赠”的赠品成本总和
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DataTable GetSaleGiftGiftProductsExcludeFull(int productSysNo, int saleGiftSysNo);

        /// <summary>
        /// 获取指定商品当前 时间生效,优惠券范围=“PM-产品优惠券”,商品范围=指定商品，系统编号列表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<int> GetCurrentCouponsForPM(int productSysNo);

        /// <summary>
        /// 某个活动，某个商品的“PM-产品优惠券”的折扣金额
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        decimal GetCouponAmountForPM(int productSysNo, int couponSysNo);
    }
}
