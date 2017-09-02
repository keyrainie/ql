using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICommonSKUNumberDA))]
    public class CommonSKUNumberDA : ICommonSKUNumberDA
    {
        public CategorySeries GetCategorySeries(CategoryInfo categoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategorySeries");
            cmd.SetParameterValue("@C3SysNo", categoryInfo.SysNo);
            return cmd.ExecuteEntity<CategorySeries>();
        }

        public BrandSeries GetBrandSeries(BrandInfo brandInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandSeries");
            cmd.SetParameterValue("@BrandSysNo", brandInfo.SysNo);
            return cmd.ExecuteEntity<BrandSeries>();
        }

        public IList<ModelSeries> GetModelSeriesList(CategoryInfo categoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetModelSeriesList");
            cmd.SetParameterValue("@C2SysNo", categoryInfo.ParentSysNumber);
            return cmd.ExecuteEntityList<ModelSeries>();
        }

        public IList<PropertySeries> GetPropertySeriesList(int productGroupSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPropertySeriesList");
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            return cmd.ExecuteEntityList<PropertySeries>();
        }

        public IList<CategorySeries> GetAvailableCategorySeries(int start)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAvailableCategorySeries");
            cmd.SetParameterValue("@Start", start);
            return cmd.ExecuteEntityList<CategorySeries>();
        }

        public IList<BrandSeries> GetAvailableBrandSeries(int start)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAvailableBrandSeries");
            cmd.SetParameterValue("@Start", start);
            return cmd.ExecuteEntityList<BrandSeries>();
        }

        public void UpdateCategorySeries(CategorySeries categorySeries)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategorySeries");
            cmd.SetParameterValue("@SysNo", categorySeries.SysNo);
            cmd.SetParameterValue("@C3SysNo", categorySeries.CategorySysNo);
            cmd.ExecuteNonQuery();
        }

        public void UpdateBrandSeries(BrandSeries brandSeries)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandSeries");
            cmd.SetParameterValue("@SysNo", brandSeries.SysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSeries.BrandSysNo);
            cmd.ExecuteNonQuery();
        }

        public void InsertModelSeries(int seriesNo, CategoryInfo categoryInfo, BrandInfo brandInfo, string productGroupModel, string companyCode, string languageCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertModelSeries");
            cmd.SetParameterValue("@SeriesNo", seriesNo);
            cmd.SetParameterValue("@C3SysNo", categoryInfo.SysNo);
            cmd.SetParameterValue("@BrandSysNo", brandInfo.SysNo);
            cmd.SetParameterValue("@ProductMode", productGroupModel);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@StoreCompanyCode", companyCode);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.ExecuteNonQuery();
        }

        public void InsertPropertySeries(int seriesNo, int productGroupSysNo, string productModel, string companyCode, string languageCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertPropertySeries");
            cmd.SetParameterValue("@SeriesNo", seriesNo);
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.SetParameterValue("@GroupPropertyInfo", productModel);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@StoreCompanyCode", companyCode);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.ExecuteNonQuery();
        }
    }
}
