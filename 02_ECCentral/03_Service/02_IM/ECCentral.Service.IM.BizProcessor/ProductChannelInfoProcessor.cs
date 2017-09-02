//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductChannelInfoProcessor))]
    public class ProductChannelInfoProcessor
    {

        private readonly IProductChannelInfoDA _ProductChannelInfoDA = ObjectFactory<IProductChannelInfoDA>.Instance;

        #region 渠道商品信息业务方法
        /// <summary>
        /// 根据SysNo获取渠道商品信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductChannelInfo GetProductChannelInfoBySysNo(int sysNo)
        {
            CheckProductChannelInfoProcessor.CheckProductChannelInfoSysNo(sysNo);
            return _ProductChannelInfoDA.GetProductChannelInfoBySysNo(sysNo);;
        }

        /// <summary>
        /// 创建渠道商品信息信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductChannelInfo CreatetProductChannelInfo(ProductChannelInfo entity)
        {
            entity.SynProductID = "";
            entity.InventoryPercent = 1;
            entity.ChannelSellCount = 0;
            entity.SafeInventoryQty = 5;
            entity.IsAppointInventory = BooleanEnum.No;
            entity.ChannelPricePercent = 1;
            entity.IsUsePromotionPrice = BooleanEnum.No;
            entity.Status = ProductChannelInfoStatus.DeActive;
            entity.SysNo = 0;

            CheckProductChannelInfoProcessor.CheckProductChannelInfoInfo(entity);
            return _ProductChannelInfoDA.CreateProductChannelInfo(entity);
        }

        /// <summary>
        /// 修改渠道商品信息信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductChannelInfo UpdateProductChannelInfo(ProductChannelInfo entity, bool isBatchUpdate)
        {
            if (entity != null)
            {
                CheckProductChannelInfoProcessor.CheckProductChannelInfoSysNo(entity.SysNo);
            }

            CheckProductChannelInfoProcessor.CheckProductChannelInfoInfo(entity);

            //判断是否为批量更新
            if (!isBatchUpdate)
            {
                var oldEntity = _ProductChannelInfoDA.GetProductChannelInfoBySysNo(entity.SysNo.Value);


                if (oldEntity.Status == ProductChannelInfoStatus.DeActive &&
                    entity.Status == ProductChannelInfoStatus.Active &&
                    !string.IsNullOrEmpty(oldEntity.SynProductID))
                {
                    entity.IsClearInventory = BooleanEnum.No;
                    _ProductChannelInfoDA.SetClearInventoryStatus(entity);
                }

                if (oldEntity.Status == ProductChannelInfoStatus.DeActive &&
                    entity.Status == ProductChannelInfoStatus.Active)
                {
                    entity.IsClearInventory = BooleanEnum.Yes;
                    _ProductChannelInfoDA.SetClearInventoryStatus(entity);
                }

                if (entity.IsAppointInventory == BooleanEnum.No) entity.ChannelSellCount = 0;

                //如果指定库存，必须同步库存，如果由指定库粗修改为非指定库存需要清楚库存
                if (!entity.IsAppointInventory.Equals(oldEntity.IsAppointInventory) || entity.IsAppointInventory == BooleanEnum.Yes)
                {
                    bool result = true;
                    if (entity.IsAppointInventory == BooleanEnum.Yes)
                    {
                        //指定库存，需要设置渠道库存
                     result=   ExternalDomainBroker.SetChannelProductInventory(entity.ChannelInfo.SysNo.Value, entity.ProductSysNo.Value, entity.ChannelSellCount.Value);
                    }
                    else
                    {
                        //不指定库存，需要清除库存
                       result= ExternalDomainBroker.AbandonChannelProductInventory(entity.ChannelInfo.SysNo.Value, entity.ProductSysNo.Value, entity.ChannelSellCount.Value);
                    }

                    if (!result)
                    {
                         //同步库存失败,请稍后再试
                        throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelSetInventoryError"));
                    }
                }

            }

            return _ProductChannelInfoDA.UpdateProductChannelInfo(entity);
        }

          #endregion

        #region 渠道商品价格信息业务方法
        /// <summary>
        /// 根据SysNo获取渠道商品价格信息信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductChannelPeriodPrice GetProductChannelPeriodPriceBySysNo(int sysNo)
        {
            CheckProductChannelInfoProcessor.CheckProductChannelPeriodPriceSysNo(sysNo);
            return _ProductChannelInfoDA.GetProductChannelPeriodPriceBySysNo(sysNo); ;
        }

        /// <summary>
        /// 创建渠道商品价格信息信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductChannelPeriodPrice CreatetProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            CheckProductChannelInfoProcessor.CheckProductChannelPeriodPriceInfo(entity);
            return _ProductChannelInfoDA.CreateProductChannelPeriodPrice(entity);
        }

        /// <summary>
        /// 修改渠道商品价格信息信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductChannelPeriodPrice UpdateProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            if (entity != null)
            {
                CheckProductChannelInfoProcessor.CheckProductChannelPeriodPriceSysNo(entity.SysNo);
            }           

            CheckProductChannelInfoProcessor.CheckProductChannelPeriodPriceInfo(entity);

            var oldEntity = _ProductChannelInfoDA.GetProductChannelPeriodPriceBySysNo(entity.SysNo.Value);

            if (((entity.Operate == ProductChannelPeriodPriceOperate.CreateOrEdit ||
                 entity.Operate == ProductChannelPeriodPriceOperate.Submit) &&
                 oldEntity.Status != ProductChannelPeriodPriceStatus.Init) ||
                ((entity.Operate == ProductChannelPeriodPriceOperate.CancelSubmit ||
                 entity.Operate == ProductChannelPeriodPriceOperate.Approve ||
                 entity.Operate == ProductChannelPeriodPriceOperate.UnApprove) &&
                 oldEntity.Status != ProductChannelPeriodPriceStatus.WaitApproved) ||
               (entity.Operate == ProductChannelPeriodPriceOperate.Stop &&
                oldEntity.Status != ProductChannelPeriodPriceStatus.Ready &&
                oldEntity.Status != ProductChannelPeriodPriceStatus.Running))
            {
                //状态已变更请刷新再试
                throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelPeriodPriceStatusChanged"));
            }

            //根据操作类型填充数据
            switch (entity.Operate)
            {
                case ProductChannelPeriodPriceOperate.Stop:
                    entity.Note = oldEntity.Note;
                    entity.PeriodPrice = oldEntity.PeriodPrice;
                    entity.BeginDate = oldEntity.BeginDate;
                    entity.EndDate = oldEntity.EndDate;
                    entity.IsChangePrice = oldEntity.IsChangePrice;
                    entity.ChannelProductInfo = oldEntity.ChannelProductInfo;
                    entity.Status = ProductChannelPeriodPriceStatus.Abandon;
                    entity.EndDate = System.DateTime.Now;
                    entity.AuditUser.UserDisplayName = "";
                    break;
                case ProductChannelPeriodPriceOperate.Submit:
                    if (entity.PeriodPrice >= entity.ChannelProductInfo.CurrentPrice)
                    {
                        entity.Status = ProductChannelPeriodPriceStatus.Ready;
                    }
                    else
                    {
                        entity.Status = ProductChannelPeriodPriceStatus.WaitApproved;
                        entity.AuditUser.UserDisplayName = "";
                    }
                    break;
                case ProductChannelPeriodPriceOperate.CreateOrEdit:
                case ProductChannelPeriodPriceOperate.CancelSubmit:
                    entity.Status = ProductChannelPeriodPriceStatus.Init;
                    entity.AuditUser.UserDisplayName = "";
                    break;
                case ProductChannelPeriodPriceOperate.UnApprove:
                    entity.Status = ProductChannelPeriodPriceStatus.Init;
                    break;
                case ProductChannelPeriodPriceOperate.Approve:
                    entity.Status = ProductChannelPeriodPriceStatus.Ready;
                    break;
            }
         
            return _ProductChannelInfoDA.UpdateProductChannelPeriodPrice(entity);
        }


        #endregion

        #region 获取渠道信息
        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        public List<ChannelInfo> GetChannelInfoList()
        {
            return _ProductChannelInfoDA.GetChannelInfoList();
        }

        #endregion

        #region 检查渠道商品信息逻辑
        private static class CheckProductChannelInfoProcessor
        {
            private static readonly IProductChannelInfoDA _ProductChannelInfoDA = ObjectFactory<IProductChannelInfoDA>.Instance;

            /// <summary>
            /// 检查渠道商品信息实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductChannelInfoInfo(ProductChannelInfo entity)
            {
                int result = _ProductChannelInfoDA.CheckProductChannelInfo(entity);

                if (result == -1)
                {
                    //该渠道已存在该商品！
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelInfoIsExists"));
                }

                if (result == -2)
                {
                    //不存在该商品！
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductNotExists"));
                }

                if (entity.SysNo > 0 && entity.IsAppointInventory == BooleanEnum.Yes && entity.ChannelSellCount > entity.MaxStockQty)
                {
                    //指定库存必须小于等于最大分仓数！
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelSellCountError"));
                }

                if (entity.Status == ProductChannelInfoStatus.Active && string.IsNullOrEmpty(entity.SynProductID))
                {
                    //有效状态渠道商品编号不能为空！
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelActiveError"));
                }

            }

            /// <summary>
            /// 检查渠道商品价格信息实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductChannelPeriodPriceInfo(ProductChannelPeriodPrice entity)
            {
                if (entity.Operate != ProductChannelPeriodPriceOperate.Stop)
                {
                    int result = _ProductChannelInfoDA.CheckProductChannelPeriodPrice(entity);

                    if (result == -1)
                    {
                        //该时段价格已存在
                        throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelPeriodPriceIsExists"));
                    }
                }                   
            }

            /// <summary>
            /// 检查渠道商品信息编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckProductChannelInfoSysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelInfoSysNOIsNull"));
                }
            }

            /// <summary>
            /// 检查渠道商品信息编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckProductChannelPeriodPriceSysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductChannelInfo", "ProductChannelInfoPeriodPriceSysNoIsNull"));
                }
            }

        }
        #endregion

    }
}
