using ECommerce.DataAccess.ControlPannel;
using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECommerce.Service.ControlPannel
{
    public class StockService
    {
        /// <summary>
        /// 查询商家的仓库列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<StockInfoQueryRestult> QueryStock(StockQueryFilter queryFilter)
        {
            return StockDA.QueryStock(queryFilter);
        }
        /// <summary>
        /// 获取某一个商家的有效仓库信息列表
        /// </summary>
        /// <param name="merchantSysNo"></param>
        /// <param name="vendorStockType"></param>
        /// <returns></returns>
        public static List<StockInfoQueryRestult> GetStockList(int merchantSysNo, VendorStockType vendorStockType)
        {
            StockQueryFilter queryFilter = new StockQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                Status = StockStatus.Actived,
                SortFields = "s.SysNo DESC"
            };
            queryFilter.MerchantSysNo = merchantSysNo;
            //所有商家都可以查看泰隆优选仓库
            queryFilter.ContainKJT = true;
            return StockService.QueryStock(queryFilter).ResultList;
        }
        /// <summary>
        /// 删除某一个商家的一个仓库
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static bool DelStock(int sysNo, int merchantSysNo)
        {
            return StockDA.DelStock(sysNo, merchantSysNo);
        }
        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool PreCheckStock(StockInfo entity)
        {
            if (string.IsNullOrEmpty(entity.StockID))
            {
                string msg = "仓库编号不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.StockName))
            {
                string msg = "仓库名称不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (entity.WarehouseRate < 0 || entity.WarehouseRate >= 10)
            {
                string msg = string.Format("移仓分仓系数值范围为：[0,10)");
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.Contact))
            {
                string msg = "发件联系人不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.Phone))
            {
                string msg = "发件联系电话不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            //if (string.IsNullOrEmpty(entity.CompanyName))
            //{
            //    string msg = "发件公司名称不能为空";
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            //if (!entity.AreaSysNo.HasValue || entity.AreaSysNo <= 0)
            //{
            //    string msg = "发件地区不能为空";
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            //if (string.IsNullOrEmpty(entity.Zip))
            //{
            //    string msg = "发件邮编不能为空";
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            if (string.IsNullOrEmpty(entity.Address))
            {
                string msg = "发件地址不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.ReceiveContact))
            {
                string msg = "收件联系人不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.ReceiveContactPhone))
            {
                string msg = "收件联系电话不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.ReceiveAddress))
            {
                string msg = "收件地址不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            return true;
        }
        /// <summary>
        /// 创建仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Create(StockInfo entity)
        {
            if (!PreCheckStock(entity))
            {
                return false;
            }
            entity.StoreCompanyCode = entity.CompanyCode;
            var data = StockDA.Create(entity);
            return true;
        }
        /// <summary>
        /// 编辑仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(StockInfo entity)
        {
            if (!PreCheckStock(entity))
            {
                return false;
            }
            entity.StoreCompanyCode = entity.CompanyCode;
            return StockDA.Edit(entity);
        }
        /// <summary>
        /// 加载仓库信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static StockInfo LoadStock(int sysNo)
        {
            return StockDA.LoadStock(sysNo);
        }

        public static QueryResult QueryStockShipType(StockShipTypeQueryFilter filter)
        {
            int count = 0;
            return new QueryResult(StockDA.QueryStockShipType(filter, out count), filter, count);
        }

        public static StockShipTypeInfo GetStockShipTypeInfo(int sysNo)
        {
            return StockDA.GetStockShipTypeInfo(sysNo);
        }

        public static void CreateStockShipType(StockShipTypeInfo info)
        {
            StockDA.CreateStockShipType(info);
        }

        public static void UpdateStockShipType(StockShipTypeInfo info)
        {
            StockDA.UpdateStockShipType(info);
        }
    }
}
