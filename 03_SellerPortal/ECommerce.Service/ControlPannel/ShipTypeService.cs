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
    public class ShipTypeService
    {
        /// <summary>
        /// 查询商家的配送方式
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ShipTypeInfoQueryResult> QueryShipType(ShipTypeQueryFilter queryFilter)
        {
            return ShipTypeDA.QueryShipType(queryFilter);
        }
        public static List<ShipTypeInfoQueryResult> GetShipTypeList(int merchantSysNo)
        {
            ShipTypeQueryFilter queryFilter = new ShipTypeQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
            };
            queryFilter.MerchantSysNo = merchantSysNo;
            return ShipTypeService.QueryShipType(queryFilter).ResultList;
        }
        /// <summary>
        /// 删除某一个商家的一个配送方式
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool DelShipType(int sysNo, int merchantSysNo)
        {
            return ShipTypeDA.DelShipType(sysNo, merchantSysNo);
        }
        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool PreCheckShipType(ShipTypeInfo entity)
        {
            //if (string.IsNullOrEmpty(entity.ShipTypeID))
            //{
            //    string msg = "配送方式编号不能为空";
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            if (string.IsNullOrEmpty(entity.ShipTypeName))
            {
                string msg = "配送方式名称不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.Period))
            {
                string msg = "配送周期不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.ShipTypeDesc))
            {
                string msg = "配送方式描述不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.Provider))
            {
                string msg = "提供方不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.PremiumRate.HasValue)
            {
                string msg = "运费费率不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (!entity.PremiumBase.HasValue)
            {
                string msg = "免保价费金额不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.OrderNumber))
            {
                string msg = "优先级不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            if (string.IsNullOrEmpty(entity.DisplayShipName))
            {
                string msg = "前台显示名称不能为空";
                msg = LanguageHelper.GetText(msg);
                throw new BusinessException(msg);
            }
            //if (string.IsNullOrEmpty(entity.ShortName))
            //{
            //    string msg = "配送方式简称不能为空";
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            if (!entity.AreaSysNo.HasValue)
            {
                entity.AreaSysNo = 0;
            }
            return true;
        }
        /// <summary>
        /// 创建配送方式信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Create(ShipTypeInfo entity)
        {
            if (!PreCheckShipType(entity))
            {
                return false;
            }
            entity.StoreCompanyCode = entity.CompanyCode;
            if (string.IsNullOrWhiteSpace(entity.ShortName))
                entity.ShortName = entity.ShipTypeName.Trim().Substring(0, 2);
            //if (ShipTypeDA.GetShipTypeforCreate(entity))
            //{
            //    string msg = string.Format("配送方式ID为{0}的数据已存在！", entity.ShipTypeID);
            //    msg = LanguageHelper.GetText(msg);
            //    throw new BusinessException(msg);
            //}
            var data = ShipTypeDA.Create(entity);
            return true;
        }
        /// <summary>
        /// 编辑配送方式信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(ShipTypeInfo entity)
        {
            if (!PreCheckShipType(entity))
            {
                return false;
            }
            entity.StoreCompanyCode = entity.CompanyCode;
            return ShipTypeDA.Edit(entity);
        }
        /// <summary>
        /// 加载配送方式信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static ShipTypeInfo LoadShipType(int sysNo)
        {
            return ShipTypeDA.LoadShipType(sysNo);
        }
    }
}
