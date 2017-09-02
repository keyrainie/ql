//************************************************************************
// 用户名				泰隆优选
// 系统名				渠道商品管理
// 子系统名		        渠道商品管理
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductChannelInfoDA))]
    internal class ProductChannelInfoDA : IProductChannelInfoDA
    {

        /// <summary>
        /// 根据SysNO获取渠道商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductChannelInfo GetProductChannelInfoBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductChannelInfoBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<ProductChannelInfo>();
        }

        /// <summary>
        /// 创建渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelInfo CreateProductChannelInfo(ProductChannelInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductChannelInfo");
            cmd.SetParameterValue("@ChannelSysNo", entity.ChannelInfo.SysNo);
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@SynProductID", entity.SynProductID);
            cmd.SetParameterValue("@InventoryPercent", entity.InventoryPercent);
            cmd.SetParameterValue("@ChannelSellCount", entity.ChannelSellCount);
            cmd.SetParameterValue("@SafeInventoryQty", entity.SafeInventoryQty);
            cmd.SetParameterValue("@IsAppointInventory", entity.IsAppointInventory);
            cmd.SetParameterValue("@ChannelPricePercent", entity.ChannelPricePercent);
            cmd.SetParameterValue("@IsUsePromotionPrice", entity.IsUsePromotionPrice);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CreateUser", entity.CreateUser.UserDisplayName);           

            cmd.ExecuteNonQuery();
            entity.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString()); 
            return entity;
        }

        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelInfo UpdateProductChannelInfo(ProductChannelInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductChannelInfo");
            cmd.SetParameterValue("@ChannelSysNo", entity.ChannelInfo.SysNo);
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@SynProductID", entity.SynProductID);
            cmd.SetParameterValue("@InventoryPercent", Convert.ToDecimal(entity.InventoryPercent) / 100);
            cmd.SetParameterValue("@ChannelSellCount", entity.ChannelSellCount);
            cmd.SetParameterValue("@SafeInventoryQty", entity.SafeInventoryQty);
            cmd.SetParameterValue("@IsAppointInventory", entity.IsAppointInventory);
            cmd.SetParameterValue("@ChannelPricePercent", Convert.ToDecimal(entity.ChannelPricePercent) / 100);
            cmd.SetParameterValue("@IsUsePromotionPrice", entity.IsUsePromotionPrice);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@EditUser", entity.EditUser.UserDisplayName);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 设置是否清库状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void SetClearInventoryStatus(ProductChannelInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetClearInventoryStatus");
            cmd.SetParameterValue("@IsClearInventory", entity.IsClearInventory);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Check渠道商品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CheckProductChannelInfo(ProductChannelInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckProductChannelInfo");
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@ChannelSysNo", entity.ChannelInfo.SysNo);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            cmd.ExecuteNonQuery();

            return int.Parse(cmd.GetParameterValue("@Flag").ToString());
        }

        /// <summary>
        /// 根据SysNO获取渠道商品价格信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice GetProductChannelPeriodPriceBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductChannelPeriodPriceBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<ProductChannelPeriodPrice>();
        }

        /// <summary>
        /// 创建渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice CreateProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductChannelPeriodPrice");
            cmd.SetParameterValue("@ChannelProductInfoSysNo", entity.ChannelProductInfo.SysNo);
            cmd.SetParameterValue("@PeriodPrice", entity.PeriodPrice);
            cmd.SetParameterValue("@BeginDate", entity.BeginDate);
            cmd.SetParameterValue("@EndDate", entity.EndDate);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@IsChangePrice", BooleanEnum.No);
            cmd.SetParameterValue("@Note", entity.Note);
            cmd.SetParameterValue("@CreateUser", entity.CreateUser.UserDisplayName);

            cmd.ExecuteNonQuery();
            entity.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            return entity;
        }


        /// <summary>
        /// 修改渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductChannelPeriodPrice UpdateProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductChannelPeriodPrice");
            cmd.SetParameterValue("@ChannelProductInfoSysNo", entity.ChannelProductInfo.SysNo);
            cmd.SetParameterValue("@PeriodPrice", entity.PeriodPrice);
            cmd.SetParameterValue("@BeginDate", entity.BeginDate);
            cmd.SetParameterValue("@EndDate", entity.EndDate);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@IsChangePrice", BooleanEnum.No);
            cmd.SetParameterValue("@Note", entity.Note);
            cmd.SetParameterValue("@EditUser", entity.EditUser.UserDisplayName);
            cmd.SetParameterValue("@AuditUser", entity.AuditUser.UserDisplayName);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// Check渠道商品价格信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CheckProductChannelPeriodPrice(ProductChannelPeriodPrice entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckProductChannelPeriodPrice");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@BeginDate", entity.BeginDate);
            cmd.SetParameterValue("@EndDate", entity.EndDate);
            cmd.SetParameterValue("@ChannelProductInfoSysNo", entity.ChannelProductInfo.SysNo);

            cmd.ExecuteNonQuery();

            return int.Parse(cmd.GetParameterValue("@Flag").ToString());
        }

        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        public List<ChannelInfo> GetChannelInfoList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetChannelInfoList");
            return command.ExecuteEntityList<ChannelInfo>();
        }

    }
}