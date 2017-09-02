using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategorySettingDA))]
    public class CategorySettingDA : ICategorySettingDA
    {
        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategorySetting GetCategorySettingBySysNo(int SysNo)
        {
              DataCommand cmd=DataCommandManager.GetDataCommand("GetCategorySettingBySysNo");

              cmd.SetParameterValue("@SysNo", SysNo);
            var entity = cmd.ExecuteEntity<CategorySetting>();
            if (entity != null)
            {
                var categoryMinMarginInfo = GetCategoryMargin(SysNo);
                entity.CategoryMinMarginInfo = new CategoryMinMargin { Margin = categoryMinMarginInfo };
                var categoryProperty = ObjectFactory<ICategoryPropertyDA>.Instance;
                entity.CategoryProperties = categoryProperty.GetCategoryPropertyByCategorySysNo(SysNo);

            }
            return entity;

        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryBasic UpdateCategoryBasic(CategoryBasic categoryBasicInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryBasic");
            cmd.SetParameterValue("@SysNo", categoryBasicInfo.CategorySysNo);
            cmd.SetParameterValue("@IsValuableProduct", categoryBasicInfo.IsValuableProduct);
            cmd.SetParameterValue("@DMSRate", 0);
            cmd.SetParameterValue("@IsMemberArea", 0);
            categoryBasicInfo.VirtualRate = categoryBasicInfo.VirtualRate / 100;
            cmd.SetParameterValue("@VirtualRate", categoryBasicInfo.VirtualRate);
            cmd.SetParameterValue("@VirtualCount", categoryBasicInfo.VirtualCount);
            cmd.SetParameterValue("@CheapenRisk", categoryBasicInfo.CheapenRiskInfo);
            cmd.SetParameterValue("@OOSQty", categoryBasicInfo.OOSQty);
            categoryBasicInfo.OOSRate = categoryBasicInfo.OOSRate / 100;
            cmd.SetParameterValue("@OOSRate", categoryBasicInfo.OOSRate);
            cmd.SetParameterValue("@PayPeriodType", categoryBasicInfo.PayPeriodTypeInfo);
            cmd.SetParameterValue("@SafetyInventoryDay", categoryBasicInfo.SafetyInventoryDay);
            cmd.SetParameterValue("@SafetyInventoryQty", categoryBasicInfo.SafetyInventoryQty);
            cmd.SetParameterValue("@IsRequired", categoryBasicInfo.IsRequired);
            cmd.SetParameterValue("@LargeFlag", categoryBasicInfo.IsLargeInfo);
            cmd.SetParameterValue("@Quota", categoryBasicInfo.Quota);
            cmd.SetParameterValue("@MinCommission", categoryBasicInfo.MinCommission);

            cmd.ExecuteNonQuery();
            return categoryBasicInfo;
        }

        /// <summary>
        /// 保存RMA指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryRMA UpdateCategoryRMA(CategoryRMA categoryBasicInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryRMA");
            cmd.SetParameterValue("@SysNo", categoryBasicInfo.CategorySysNo);
            cmd.SetParameterValue("@WarrantyDays", categoryBasicInfo.WarrantyDays);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.SetParameterValue("@RMARateStandard", categoryBasicInfo.RMARateStandard);
            cmd.ExecuteNonQuery();
            return categoryBasicInfo;
        }

        /// <summary>
        /// 保存毛利率指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public CategoryMinMargin UpdateCategoryMinMargin(CategoryMinMargin categoryBasicInfo)
        {
            
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryMinMargin");
            cmd.SetParameterValue("SysNo", categoryBasicInfo.CategorySysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Thirty))
            {
                cmd.SetParameterValue("@M1", categoryBasicInfo.Margin[MinMarginDays.Thirty].MinMargin / 100);
                cmd.SetParameterValue("@M1H", categoryBasicInfo.Margin[MinMarginDays.Thirty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M1", 0);
                cmd.SetParameterValue("@M1H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Sixty))
            {
                cmd.SetParameterValue("@M2", categoryBasicInfo.Margin[MinMarginDays.Sixty].MinMargin / 100);
                cmd.SetParameterValue("@M2H", categoryBasicInfo.Margin[MinMarginDays.Sixty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M2", 0);
                cmd.SetParameterValue("@M2H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Ninety))
            {
                cmd.SetParameterValue("@M3", categoryBasicInfo.Margin[MinMarginDays.Ninety].MinMargin / 100);
                cmd.SetParameterValue("@M3H", categoryBasicInfo.Margin[MinMarginDays.Ninety].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M3", 0);
                cmd.SetParameterValue("@M3H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.OneHundredAndTwenty))
            {
                cmd.SetParameterValue("@M4", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndTwenty].MinMargin / 100);
                cmd.SetParameterValue("@M4H", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndTwenty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M4", 0);
                cmd.SetParameterValue("@M4H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.OneHundredAndEighty))
            {
                cmd.SetParameterValue("@M5", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndEighty].MinMargin / 100);
                cmd.SetParameterValue("@M5H", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndEighty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M5", 0);
                cmd.SetParameterValue("@M5H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Other))
            {
                cmd.SetParameterValue("@M6", categoryBasicInfo.Margin[MinMarginDays.Other].MinMargin / 100);
                cmd.SetParameterValue("@M6H", categoryBasicInfo.Margin[MinMarginDays.Other].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@6", 0);
                cmd.SetParameterValue("@M6H", 0);
            }
            cmd.ExecuteNonQuery();
            return categoryBasicInfo;
        }

        /// <summary>
        /// 获取毛利率数组
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        private Dictionary<MinMarginDays, MinMarginKPI> GetCategoryMargin(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategoryMargin");
            cmd.SetParameterValue("@SysNo", sysNo);
            var source = cmd.ExecuteEntityList<MinMarginKPI>();
            if (source == null || source.Count == 0) return new Dictionary<MinMarginDays, MinMarginKPI>();
            var dataList = source.Select(p => new { p.MinMarginDays, Margin = new MinMarginKPI { MaxMargin = p.MaxMargin, MinMargin = p.MinMargin } });
            var templist = dataList.AsEnumerable().ToDictionary(p => p.MinMarginDays, k => k.Margin);
            return templist;
        }


        /// <summary>
        /// 更新最低限额
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public void UpdateCategoryProductMinCommission(CategoryBasic categoryBasicInfo)
        {
            DataCommand cmd;
            if (categoryBasicInfo.CommissionInfo.Comparison == Comparison.Equal)
            {
                cmd = DataCommandManager.GetDataCommand("UpdateCategoryProductMinCommission");
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("UpdateCategoryProductMinCommission2");
            }
            cmd.SetParameterValue("@MinCommission",categoryBasicInfo.MinCommission);
            cmd.SetParameterValue("@Category1SysNo", categoryBasicInfo.Category1SysNo);
            cmd.SetParameterValue("@Category2SysNo", categoryBasicInfo.Category2SysNo);
            cmd.SetParameterValue("@Category3SysNo", categoryBasicInfo.CategorySysNo == null ? 0 : categoryBasicInfo.CategorySysNo);
            cmd.SetParameterValue("@PMID", categoryBasicInfo.CommissionInfo.PMSysNo == null ? 0 : categoryBasicInfo.CommissionInfo.PMSysNo);
            cmd.SetParameterValue("@ManufacturerSysNo", categoryBasicInfo.CommissionInfo.ManufacturerSysNo);
            cmd.SetParameterValue("@ProductStatus", categoryBasicInfo.CommissionInfo.ProductStatus );
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据类别2得到2级指标
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public CategorySetting GetCategorySettingByCategory2SysNo(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategorySettingBySysNo2");

            cmd.SetParameterValue("@SysNo", SysNo);
            var entity = cmd.ExecuteEntity<CategorySetting>();
            if (entity != null)
            {
                var categoryMinMarginInfo = GetCategoryMargin(SysNo);
                entity.CategoryMinMarginInfo = new CategoryMinMargin { Margin = categoryMinMarginInfo };
                var categoryProperty = ObjectFactory<ICategoryPropertyDA>.Instance;
                entity.CategoryProperties = categoryProperty.GetCategoryPropertyByCategorySysNo(SysNo);

            }
            return entity;
        }

        /// <summary>
        /// 更新类别2的基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public void UpdateCategory2Basic(CategoryBasic categoryBasicInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryKpi2");
            cmd.SetParameterValue("@SysNo", categoryBasicInfo.PropertySysNO);
            cmd.SetParameterValue("@AvgDailySalesCycle", categoryBasicInfo.AvgDailySalesCycle);
            cmd.SetParameterValue("@IsValuableProduct", categoryBasicInfo.IsValuableProduct);
             cmd.SetParameterValue("@CategorySysNo", categoryBasicInfo.Category2SysNo);
            cmd.SetParameterValue("@InStockDays", categoryBasicInfo.InStockDays);
            categoryBasicInfo.VirtualRate = categoryBasicInfo.VirtualRate / 100;
            cmd.SetParameterValue("@VirtualRate", categoryBasicInfo.VirtualRate);
            cmd.SetParameterValue("@VirtualCount", categoryBasicInfo.VirtualCount);
            cmd.SetParameterValue("@CheapenRisk", categoryBasicInfo.CheapenRiskInfo);
            cmd.SetParameterValue("@OOSQty", categoryBasicInfo.OOSQty);
            categoryBasicInfo.OOSRate = categoryBasicInfo.OOSRate / 100;
            cmd.SetParameterValue("@OOSRate", categoryBasicInfo.OOSRate);
            cmd.SetParameterValue("@LogUserName", "IPPSystemAdmin");
            cmd.SetParameterValue("@Quota", categoryBasicInfo.Quota);
            cmd.SetParameterValue("@MinCommission", categoryBasicInfo.MinCommission);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新类别2的毛利率
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public void UpdateCategory2MinMargin(CategoryMinMargin categoryBasicInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryMinMargin2");
            cmd.SetParameterValue("SysNo", categoryBasicInfo.CategorySysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Thirty))
            {
                cmd.SetParameterValue("@M1", categoryBasicInfo.Margin[MinMarginDays.Thirty].MinMargin / 100);
                cmd.SetParameterValue("@M1H", categoryBasicInfo.Margin[MinMarginDays.Thirty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M1", 0);
                cmd.SetParameterValue("@M1H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Sixty))
            {
                cmd.SetParameterValue("@M2", categoryBasicInfo.Margin[MinMarginDays.Sixty].MinMargin / 100);
                cmd.SetParameterValue("@M2H", categoryBasicInfo.Margin[MinMarginDays.Sixty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M2", 0);
                cmd.SetParameterValue("@M2H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Ninety))
            {
                cmd.SetParameterValue("@M3", categoryBasicInfo.Margin[MinMarginDays.Ninety].MinMargin / 100);
                cmd.SetParameterValue("@M3H", categoryBasicInfo.Margin[MinMarginDays.Ninety].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M3", 0);
                cmd.SetParameterValue("@M3H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.OneHundredAndTwenty))
            {
                cmd.SetParameterValue("@M4", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndTwenty].MinMargin / 100);
                cmd.SetParameterValue("@M4H", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndTwenty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M4", 0);
                cmd.SetParameterValue("@M4H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.OneHundredAndEighty))
            {
                cmd.SetParameterValue("@M5", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndEighty].MinMargin / 100);
                cmd.SetParameterValue("@M5H", categoryBasicInfo.Margin[MinMarginDays.OneHundredAndEighty].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@M5", 0);
                cmd.SetParameterValue("@M5H", 0);
            }
            if (categoryBasicInfo.Margin.ContainsKey(MinMarginDays.Other))
            {
                cmd.SetParameterValue("@M6", categoryBasicInfo.Margin[MinMarginDays.Other].MinMargin / 100);
                cmd.SetParameterValue("@M6H", categoryBasicInfo.Margin[MinMarginDays.Other].MaxMargin / 100);
            }
            else
            {
                cmd.SetParameterValue("@6", 0);
                cmd.SetParameterValue("@M6H", 0);
            }
            cmd.ExecuteNonQuery();
        }
    }
}
