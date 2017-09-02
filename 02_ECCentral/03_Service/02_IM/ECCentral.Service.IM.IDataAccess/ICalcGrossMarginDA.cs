//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动申请单据
// 子系统名		        商品价格变动申请单据业务数据底层接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICalcGrossMarginDA
    {
        /// <summary>
        /// 获取毛利率
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="unitcost"></param>
        /// <param name="point"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal CalcGrossMarginRate(decimal currentPrice, decimal unitcost, decimal point, int productSysNo);

        /// <summary>
        /// 获取毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal CalcGrossMarginRate(int productSysNo);

        /// <summary>
        /// 获取最低毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        decimal GetMinMarginByItemSysNo(int productSysNo);

        /// <summary>
        /// 获取最高毛利率以及最低毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="minMarin"></param>
        /// <param name="minMarinH"></param>
        void GetMinMarginByProductSysNo(int productSysNo, ref decimal minMarin, ref decimal minMarinH);

    }
}
