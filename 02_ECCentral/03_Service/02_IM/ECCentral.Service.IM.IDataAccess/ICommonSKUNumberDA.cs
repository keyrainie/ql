using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICommonSKUNumberDA
    {
        CategorySeries GetCategorySeries(CategoryInfo categoryInfo);

        BrandSeries GetBrandSeries(BrandInfo brandInfo);

        IList<ModelSeries> GetModelSeriesList(CategoryInfo categoryInfo);

        IList<PropertySeries> GetPropertySeriesList(int productGroupSysNo);

        IList<CategorySeries> GetAvailableCategorySeries(int start);

        IList<BrandSeries> GetAvailableBrandSeries(int start);

        void UpdateCategorySeries(CategorySeries categorySeries);

        void UpdateBrandSeries(BrandSeries categorySeries);

        void InsertModelSeries(int seriesNo, CategoryInfo categoryInfo, BrandInfo brandInfo,
                                     string productGroupModel, string companyCode, string languageCode);

        void InsertPropertySeries(int seriesNo, int productGroupSysNo, string productModel, string companyCode,
                                  string languageCode);


        //int GetC2SysNoByC3SysNo(int c3SysNo, string companyCode);

        //CommonSKUNumberCategory2SeriesEntity GetCommonSKUNumberCategory2SeriesEntity(int c2SysNo, string companyCode);

        //List<CommonSKUNumberCategory2SeriesEntity> GetAvailableCommonSKUNumberCategory2SeriesEntity(int startSysNo, string companyCode);

        //CommonSKUNumberCategory2SeriesEntity UpdateCommonSKUNumberCategory2SeriesEntity(int sysNo, int c2SysNo, string companyCode);

        //CommonSKUNumberBrandSeriesEntity GetCommonSKUNumberBrandSeriesEntity(int brandSysNo, string companyCode);

        //List<CommonSKUNumberBrandSeriesEntity> GetAvailableCommonSKUNumberBrandSeriesEntity(int startSysNo, string companyCode);

        //CommonSKUNumberBrandSeriesEntity UpdateCommonSKUNumberBrandSeriesEntity(int sysNo, int brandSysNo, string companyCode);

        //List<CommonSKUNumberModeSeriesEntity> GetCommonSKUNumberModeSeriesEntity(int c2SysNo, string companyCode);

        //CommonSKUNumberModeSeriesEntity InsertCommonSKUNumberModeSeriesEntity(int seriesNo, int c2SysNo, int c3SysNo, int brandSysNo, string productMode, string companyCode);

        //List<CommonSKUNumberPropertySeriesEntity> GetCommonSKUNumberPropertySeriesEntity(int productGroupSysNo, string companyCode);

        //CommonSKUNumberPropertySeriesEntity InsertCommonSKUNumberPropertySeriesEntity(int seriesNo, int productGroupSysNo, string groupPropertyInfoString, string companyCode);

        //string GetSellerID(int sellerSysNo, string companyCode);
    }
}
