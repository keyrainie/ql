using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ISaleGiftQueryDA
    {
        DataTable QuerySaleGift(SaleGiftQueryFilter filter, out int totalCount);

        DataTable GetValidVenderGifts(int productSysNo, out int totalCount);

        /// <summary>
        /// 根据主商品列表取得赠品列表
        /// </summary>
        /// <param name="giftBeginDate">赠品活动开始时间</param>
        /// <param name="giftEndDate">赠品活动结束时间</param>
        /// <param name="masterProductSysNo">赠品活动主商品列表</param>
        /// <returns></returns>
        DataTable GetGiftItemByMasterProducts(DateTime giftBeginDate, DateTime giftEndDate, List<int> masterProductSysNo);

        List<ProductItemInfo> GetSaleRules(int promotionSysNo, string companyCode);

        DataTable QuerySaleGiftLog(SaleGiftLogQueryFilter filter, out int totalCount);

    }
}
