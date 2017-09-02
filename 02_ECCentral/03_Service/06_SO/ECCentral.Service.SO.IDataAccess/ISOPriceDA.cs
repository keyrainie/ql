using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOPriceDA
    {

        /// <summary>
        /// 根据订单编号取得订单拆分的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo);


        /// <summary>
        /// 根据订单编号删除订单的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void DeleteSOPriceBySOSysNo(int soSysNo);


        /// <summary>
        /// 添加订单价格信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="companyCode">订单对应的CompanyCode</param>
        void InsertSOPrice(SOPriceMasterInfo info, string companyCode);

        /// <summary>
        /// 添加订单商品价格信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="soSysNo">对应的订单编号</param>
        /// <param name="companyCode">订单对应的CompanyCode</param>
        void InsertSOPriceItem(SOPriceItemInfo info, int soSysNo, string companyCode);

        /// <summary>
        /// 作废订单价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void AbandonSOPriceBySOSysNo(int soSysNo);
    }
}
