using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.QueryFilter;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IGiftCardQueryDA
    {
        /// <summary>
        /// 查询礼品卡
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryGiftCardInfo(ECCentral.QueryFilter.IM.GiftCardFilter filter, out int totalCount);

        /// <summary>
        /// 查询礼品卡制作单
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryGiftCardFabricationMaster(ECCentral.QueryFilter.IM.GiftCardFabricationFilter filter, out int totalCount);

        /// <summary>
        /// 查询礼品卡商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryGiftCardProductInfo(GiftCardProductFilter filter, out int totalCount);

        /// <summary>
        /// 查询礼品券关联商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryGiftVoucherProductRelation(GiftCardProductFilter filter, out int totalCount);

        /// <summary>
        /// 查询礼品券关联请求
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryGiftVoucherProductRelationReq(GiftCardProductFilter filter, out int totalCount);
    }
}
