/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Harry.H.Ning
 *  Date:    2010-5-10
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System.Collections.Generic;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess
{
    public static class BaiduDA
    {
        public static List<ItemEntity> GetProductList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<ItemEntity>();
        }

        public static List<CategoryEntity> GetCategoryList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<CategoryEntity>();
        }

        public static ItemScoreEntity GetProductScore(int sysNo, string companyCode)
        {
            try
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetProductScore");
                cmd.SetParameterValue("@SysNo", sysNo);
                cmd.SetParameterValue("@CompanyCode", companyCode);
                return cmd.ExecuteEntity<ItemScoreEntity>();
            }
            catch
            {
                return new ItemScoreEntity();
            }
        }

        public static List<ItemPropertyEntity> GetProductAdvPropertyListByProduct(ItemPropertyEntity query)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductAdvPropertyListByProduct");
            cmd.SetParameterValue("@CompanyCode", query.CompanyCode);
            cmd.SetParameterValue("@ProductSysNo", query.ProductSysNo);
            return cmd.ExecuteEntityList<ItemPropertyEntity>();
        }

        public static List<BaiduCategoryEntity> GetBaiduCategoryList()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "DataAccess\\Configuration\\CategoryConfig.xml";
            return SerializeHelper.LoadFromXml<CategoryConfigEntity>(path).BaiduCategoryList;
        }

        public static List<Category1ConfigurationEntity> GetCategory1ConfigList()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "DataAccess\\Configuration\\BaiduPlatformCategory1Data.xml";
            return SerializeHelper.LoadFromXml<BaiduPlatformCategory1DataConfigurationEntity>(path).BaiduCategory1List;
        }

        public static List<Category2or3ConfigurationEntity> GetCategory2or3List(string level, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory2or3List");
            cmd.SetParameterValue("@Level", level);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntityList<Category2or3ConfigurationEntity>();
        }

        public static ProductNumberAndMinPriceEntity GetProductCountByFrontEndCategory1SysNo(int category1SysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCountByFrontEndCategory1SysNo");
            cmd.SetParameterValue("@Category1SysNo", category1SysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntity<ProductNumberAndMinPriceEntity>();

        }

        public static ProductNumberAndMinPriceEntity GetProductCountByFrontEndCategory2SysNo(int category2SysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCountByFrontEndCategory2SysNo");
            cmd.SetParameterValue("@Category2SysNo", category2SysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntity<ProductNumberAndMinPriceEntity>();
        }

        public static ProductNumberAndMinPriceEntity GetProductCountByFrontEndCategory3SysNo(int category3SysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCountByFrontEndCategory3SysNo");
            cmd.SetParameterValue("@Category3SysNo", category3SysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntity<ProductNumberAndMinPriceEntity>();
        }

        public static ProductNumberAndMinPriceEntity GetProductCountByFrontEndCategory3SysNoAndManufacturerSysNo(int category3SysNo, int manufacturerSysNo, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductCountByFrontEndCategory3SysNoAndManufacturerSysNo");
            cmd.SetParameterValue("@Category3SysNo", category3SysNo);
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturerSysNo);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntity<ProductNumberAndMinPriceEntity>();
        }

        

        public static List<BaiduManufacturerEntity> GetBrandList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetManufacturerList");
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntityList<BaiduManufacturerEntity>();
        }

        public static CategoryManufacturerPathEntity GetCategoryManufacturerPath(int manufacturerSysNo, int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryManufacturerPath");
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturerSysNo);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);

            return cmd.ExecuteEntity<CategoryManufacturerPathEntity>();
        }
    }
}