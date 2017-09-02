//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理业务逻辑实现
// 作成者				John
// 改版日				2012.11.5
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Linq;
using System;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductChannelMemberInfoProcessor))]
    public class ProductChannelMemberInfoProcessor
    {
        #region Const
        private readonly IProductChannelMemberInfoDA _ProductChannelInfoDA = 
            ObjectFactory<IProductChannelMemberInfoDA>.Instance;
        #endregion

        #region ProductChannelMemberInfo
        // 获取渠道列表
        public List<ProductChannelMemberInfo> GetProductChannelMemberInfoListBiz()
        {
            return _ProductChannelInfoDA.GetProductChannelMemberInfoList();
        }
        #endregion

        #region ProductChannelMemberPriceInfo
        //查询全部外部渠道会员价格
        public List<ProductChannelMemberPriceInfo> GetProductChannelMemberPriceByAll()
        {
            return _ProductChannelInfoDA.GetProductChannelMemberPriceByAll();

        }
        // 插入会员渠道价格表
        public Int32 InsertProductChannelMemberPricesBiz(ProductChannelMemberPriceInfo entity)
        {
            //找出当前渠道下的所有记录
            IList<ProductChannelMemberPriceInfo> _old =
                _ProductChannelInfoDA.GetProductChannelMemberPriceByChannelSysNo(entity.ChannelSysNO);
            if (_old.Select(p => p.ProductSysNo).Contains(entity.ProductSysNo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "TheChannelExistsThePorduct"));
            }
            return _ProductChannelInfoDA.InsertProductChannelMemberPrices(entity);
        }
        //更新优惠价和优惠比例
        public Int32 UpdateProductChannelMemberPrice(ProductChannelMemberPriceInfo entity)
        {
            return _ProductChannelInfoDA.UpdateProductChannelMemberPrice(entity);
        }
        //删除外部渠道会员信息
        public Int32 DeleteProductChannelMemberPrice(Int32 sysNo)
        {
            return _ProductChannelInfoDA.DeleteProductChannelMemberPrice(sysNo);
        }
        #endregion

        #region ProductChannelMemberPriceLogInfo
        //插入日志
        public Int32 InsertProductChannelMemberPriceLogBiz(ProductChannelMemberPriceLogInfo entity)
        {
            return _ProductChannelInfoDA.InsertProductChannelMemberPriceLog(entity);
        }
        #endregion
    }
}
