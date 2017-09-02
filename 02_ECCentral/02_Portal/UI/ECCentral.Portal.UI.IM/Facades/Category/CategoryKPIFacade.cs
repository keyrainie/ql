using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models.Category;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryKPIFacade
    {
        #region 构造函数和字段
        private readonly RestClient _restClient;
        const string UPdateeCategoryBasicRelativeUrl = "/IMService/CategoryKPI/UpdateCategoryBasic";
        const string UPdateeCategoryRMARelativeUrl = "/IMService/CategoryKPI/UpdateCategoryRMA";
        const string UPdateeCategoryMinMarginRelativeUrl = "/IMService/CategoryKPI/UpdateCategoryMinMargin";
        const string UPdateeCategory2MinMarginRelativeUrl = "/IMService/CategoryKPI/UpdateCategory2MinMargin";
        const string GetRelativeUrl = "/IMService/CategoryKPI/GetCategorySettingBySysNo";
        const string UpdateCategoryProductMinCommissionUrl = "/IMService/CategoryKPI/UpdateCategoryProductMinCommission";
        const string UpdateCategory2ProductMinCommissionUrl = "/IMService/CategoryKPI/UpdateCategory2ProductMinCommission";
        const string GetCategorySettingByCategory2SysNoUrl = "/IMService/CategoryKPI/GetCategorySettingByCategory2SysNo";
        const string UpdateCategory2BasicUrl = "/IMService/CategoryKPI/UpdateCategory2Basic";
        const string UpdateCategory3MinMarginBatUrl = "/IMService/CategoryKPI/UpdateCategory3MinMarginBat";
        const string UpdateCategory2MinMarginBatUrl = "/IMService/CategoryKPI/UpdateCategory2MinMarginBat";
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public CategoryKPIFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryKPIFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        #region 函数
        /// <summary>
        /// 转换品牌视图和品牌实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private T CovertVMtoEntity<TV, T>(TV data) where TV : ModelBase
        {
            T msg = data.ConvertVM<TV, T>();

            return msg;
        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryBasic(CategoryKPIBasicInfoVM data, CategoryType type, EventHandler<RestClientEventArgs<CategoryBasic>> callback)
        {
            if (type == CategoryType.CategoryType3)
            {
                _restClient.Update(UPdateeCategoryBasicRelativeUrl, CovertVMtoEntity<CategoryKPIBasicInfoVM, CategoryBasic>(data), callback);
            }
            else
            {
                _restClient.Update(UpdateCategory2BasicUrl, CovertVMtoEntity<CategoryKPIBasicInfoVM, CategoryBasic>(data), callback);
            }

        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryRMA(CategoryKPIRMAInfoVM data, EventHandler<RestClientEventArgs<CategoryRMA>> callback)
        {

            _restClient.Update(UPdateeCategoryRMARelativeUrl, CovertVMtoEntity<CategoryKPIRMAInfoVM, CategoryRMA>(data), callback);
        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryMinMargin(CategoryKPIMinMarginVM data, CategoryType type, EventHandler<RestClientEventArgs<CategoryMinMargin>> callback)
        {
            if (type == CategoryType.CategoryType3)
            {
                _restClient.Update(UPdateeCategoryMinMarginRelativeUrl, CovertVMtoEntity<CategoryKPIMinMarginVM, CategoryMinMargin>(data), callback);
            }
            else
            {
                _restClient.Update(UPdateeCategory2MinMarginRelativeUrl, CovertVMtoEntity<CategoryKPIMinMarginVM, CategoryMinMargin>(data), callback);
            }

        }

        /// <summary>
        /// 根据二级分类获取二级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategorySettingByCategory2SysNo(int SysNo, EventHandler<RestClientEventArgs<CategorySetting>> callback)
        {

            _restClient.Query(GetCategorySettingByCategory2SysNoUrl, SysNo, callback);
        }
        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetCategorySettingBySysNo(int SysNo, CategoryType type, EventHandler<RestClientEventArgs<CategorySetting>> callback)
        {
            if (type == CategoryType.CategoryType3)
            {
                _restClient.Query(GetRelativeUrl, SysNo, callback);
            }
            else
            {
                _restClient.Query(GetCategorySettingByCategory2SysNoUrl, SysNo, callback);
            }
        }
        public void UpdateCategoryProductMinCommission(ProductCommissionQuotaVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryBasic info = new CategoryBasic()
            {
                Category1SysNo = (int)model.Category1SysNo,
                Category2SysNo = (int)model.Category2SysNo,
                CategorySysNo = model.Category3SysNo == null ? 0 : (int)model.Category3SysNo,
                CommissionInfo = new CommissionInfo() { Comparison = model.Comparison, CategoryType = model.CategoryType, ManufacturerSysNo = Convert.ToInt32(model.ManufacturerSysNo), PMSysNo = model.PMSysNo, ProductStatus = model.ProductStatus },
                MinCommission = Convert.ToDecimal(model.CommissionMin)
            };
            _restClient.Update(UpdateCategoryProductMinCommissionUrl, info, callback);
        }

        public void UpdateCategory2ProductMinCommission(List<ProductCommissionQuotaVM> data, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            List<CategoryBasic> list = new List<CategoryBasic>();
            foreach (var model in data)
            {
                list.Add(new CategoryBasic() 
                {
                    Category1SysNo = (int)model.Category1SysNo,
                    Category2SysNo = (int)model.Category2SysNo,
                    CategorySysNo = model.Category3SysNo == null ? 0 : (int)model.Category3SysNo,
                    CommissionInfo = new CommissionInfo() { Comparison = model.Comparison, CategoryType = model.CategoryType, ManufacturerSysNo = Convert.ToInt32(model.ManufacturerSysNo), PMSysNo = model.PMSysNo, ProductStatus = model.ProductStatus },
                    MinCommission = Convert.ToDecimal(model.CommissionMin)
                });
            }
            _restClient.Update(UpdateCategory2ProductMinCommissionUrl, list, callback);
        }
        /// <summary>
        /// 根据类型批量保存类别的毛利率
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void UpdateCategoryMinMarginByType(List<CategoryKPIMinMarginVM> list,CategoryType type,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (type == CategoryType.CategoryType3)
            {
                _restClient.Update(UpdateCategory3MinMarginBatUrl, list, callback);
            }
            else
            {
                _restClient.Update(UpdateCategory2MinMarginBatUrl, list, callback);
            }

        }
        #endregion
    }
}