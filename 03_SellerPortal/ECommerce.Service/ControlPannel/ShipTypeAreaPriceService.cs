using ECommerce.DataAccess.ControlPannel;
using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.ControlPannel
{
    public class ShipTypeAreaPriceService
    {
        /// <summary>
        /// 查询配送方式-地区-价格
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ShipTypeAreaPriceInfoQueryResult> QueryShipTypeAreaPrice(ShipTypeAreaPriceQueryFilter queryFilter)
        {
            return ShipTypeAreaPriceDA.QueryShipTypeAreaPrice(queryFilter);
        }
        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool PreCheckShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            if (!entity.ShipTypeSysNo.HasValue || entity.ShipTypeSysNo.Value <= 0)
            {
                string msg = "配送方式不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.AreaSysNo.HasValue || entity.AreaSysNo.Value <= 0)
            {
                if (entity.AreaSysNoList == null || entity.AreaSysNoList.Count <= 0)
                {
                    string msg = "送货区域不能为空";
                    msg = LanguageHelper.GetText(msg);
                    throw new BusinessException(msg);
                }
                else if (entity.AreaSysNoList.Count == 1 && entity.AreaSysNoList[0] == 0)
                {
                    string msg = "送货区域不能为空";
                    msg = LanguageHelper.GetText(msg);
                    throw new BusinessException(msg);
                }
                else
                {

                }
            }
            else
            {
                entity.AreaSysNoList = new List<int>() { entity.AreaSysNo.Value };
            }
            if (!entity.BaseWeight.HasValue)
            {
                string msg = "本段起始重量不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.TopWeight.HasValue)
            {
                string msg = "本段截至重量不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (entity.TopWeight.Value < entity.BaseWeight.Value)
            {
                string msg = "本段截至重量不能小于本段起始重量";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.UnitWeight.HasValue)
            {
                string msg = "重量基本单位不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.UnitPrice.HasValue)
            {
                string msg = "价格基本单位不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.MaxPrice.HasValue)
            {
                string msg = "本段最高价格不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            return true;
        }
        /// <summary>
        /// 删除配送方式-地区-价格
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static bool VoidShipTypeAreaPrice(List<int> sysNoList, int merchantSysNo)
        {
            return ShipTypeAreaPriceDA.VoidShipTypeAreaPrice(sysNoList, merchantSysNo);
        }
        /// <summary>
        /// 创建配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Create(ShipTypeAreaPriceInfo entity)
        {
            if (!PreCheckShipTypeAreaPrice(entity))
            {
                return false;
            }
            entity.StoreCompanyCode = entity.CompanyCode;
            using (var trans = TransactionManager.Create())
            {
                //batch create
                foreach (int i in entity.AreaSysNoList)
                {
                    if (i < 1)
                    {
                        continue;
                    }
                    entity.AreaSysNo = i;
                    ShipTypeAreaPriceDA.Create(entity);
                }
                trans.Complete();
            }
            return true;
        }
        /// <summary>
        /// 更新配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(ShipTypeAreaPriceInfo entity)
        {
            //if (!PreCheckShipType(entity))
            //{
            //    return false;
            //}
            entity.StoreCompanyCode = entity.CompanyCode;
            return ShipTypeAreaPriceDA.Edit(entity);
        }
        /// <summary>
        /// 加载配送方式-地区-价格
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static ShipTypeAreaPriceInfo LoadShipTypeAreaPrice(int sysNo, int merchantSysNo)
        {
            return ShipTypeAreaPriceDA.LoadShipTypeAreaPrice(sysNo, merchantSysNo);
        }
    }
}
