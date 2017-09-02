//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理NoBizQuery查询接口
// 作成者				John
// 改版日				2012.11.5
// 改版内容				新建
//************************************************************************


using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using System.Data;
using ECCentral.QueryFilter.IM;
using System;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductChannelMemberInfoDA
    {

        #region ProductChannelMemberInfo
        // 获取渠道列表
        List<ProductChannelMemberInfo> GetProductChannelMemberInfoList();
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询全部外部渠道会员价格
        List<ProductChannelMemberPriceInfo> GetProductChannelMemberPriceByAll();
        // 插入会员渠道信息
        Int32 InsertProductChannelMemberPrices(ProductChannelMemberPriceInfo entity);
        //查询指定外部渠道会员价格
        IList<ProductChannelMemberPriceInfo> GetProductChannelMemberPriceByChannelSysNo(int ChannelSysNo);
        // 查询会员渠道信息
        DataTable GetProductChannelMemberPriceInfoUrl(ProductChannelInfoMemberQueryFilter queryCriteria
            , out int totalCount);
        //更新优惠价和优惠比例
        Int32 UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo entity);
        //删除外部渠道会员信息
        Int32 DeleteProductChannelMemberPrice(Int32 sysNo);
        #endregion
        
        #region ProductChannelMemberPriceLogInfo
        //添加日志记录
        Int32 InsertProductChannelMemberPriceLog(ProductChannelMemberPriceLogInfo entity);
        //查询日志信息
        DataTable GetProductChannelMemberPriceLogs(
            ProductChannelInfoMemberQueryFilter queryCriteria, out int totalCount);
        #endregion
    }
}
