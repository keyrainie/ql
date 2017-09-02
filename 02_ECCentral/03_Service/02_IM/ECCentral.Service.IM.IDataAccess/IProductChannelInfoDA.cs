//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理NoBizQuery查询接口
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************


using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductChannelInfoDA
    {
        /// <summary>
        /// 根据SysNO获取渠道商品价格信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ProductChannelInfo GetProductChannelInfoBySysNo(int sysNo);

        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductChannelInfo CreateProductChannelInfo(ProductChannelInfo entity);

        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductChannelInfo UpdateProductChannelInfo(ProductChannelInfo entity);

        
        /// <summary>
        /// 设置是否清库状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void SetClearInventoryStatus(ProductChannelInfo entity);

        /// <summary>
        /// Check渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CheckProductChannelInfo(ProductChannelInfo entity);

        /// <summary>
        /// 根据SysNO获取渠道商品价格信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ProductChannelPeriodPrice GetProductChannelPeriodPriceBySysNo(int sysNo);

        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductChannelPeriodPrice CreateProductChannelPeriodPrice(ProductChannelPeriodPrice entity);

        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductChannelPeriodPrice UpdateProductChannelPeriodPrice(ProductChannelPeriodPrice entity);

        /// <summary>
        /// Check渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int CheckProductChannelPeriodPrice(ProductChannelPeriodPrice entity);

        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        List<ChannelInfo> GetChannelInfoList();

    }
}
