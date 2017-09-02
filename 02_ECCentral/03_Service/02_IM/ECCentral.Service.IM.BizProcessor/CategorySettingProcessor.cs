
using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategorySettingProcessor))]
    public class CategorySettingProcessor
    {

        private readonly ICategorySettingDA _categoryDA = ObjectFactory<ICategorySettingDA>.Instance;

        #region 分类设置业务处理
        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CategorySetting GetCategorySettingBySysNo(int sysNo)
        {
            CheckCategorySettingProcessor.CheckCategorySettingSysNo(sysNo);
            var result = _categoryDA.GetCategorySettingBySysNo(sysNo);
            if (result != null && result.CategoryBasicInfo != null)
            {
                result.CategoryBasicInfo.VirtualRate = result.CategoryBasicInfo.VirtualRate * 100;
                result.CategoryBasicInfo.OOSRate = result.CategoryBasicInfo.OOSRate * 100;
            }
            if (result != null && result.CategoryMinMarginInfo != null
                && result.CategoryMinMarginInfo.Margin != null
                && result.CategoryMinMarginInfo.Margin.Count > 0)
            {
                result.CategoryMinMarginInfo.Margin.ForEach(v =>
                                                                {
                                                                    v.Value.MaxMargin = v.Value.MaxMargin * 100;
                                                                    v.Value.MinMargin = v.Value.MinMargin * 100;
                                                                });
            }
            return result;
        }
      
        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual CategorySetting GetCategorySettingBySysNo(int sysNo, int productSysNo)
        {
            CheckCategorySettingProcessor.CheckCategorySettingSysNo(sysNo);
            var eneity = _categoryDA.GetCategorySettingBySysNo(sysNo);
            CheckCategorySettingProcessor.CheckCategorySettingInfo(eneity);
            CheckCategorySettingProcessor.CheckProductSysNo(productSysNo);
            var inStockDays = ExternalDomainBroker.GetInStockDaysByProductSysNo(productSysNo);
            var days = GetMinMarginDays(inStockDays);
            if (eneity.CategoryMinMarginInfo.Margin.ContainsKey(days))
            {
                eneity.PrimaryMargin = eneity.CategoryMinMarginInfo.Margin[days].MinMargin;
                eneity.SeniorMargin = eneity.CategoryMinMarginInfo.Margin[days].MaxMargin;
            }
            else
            {
                if (inStockDays >= 0 && inStockDays <= 30)
                {
                    eneity.PrimaryMargin = 0.05m;
                    eneity.SeniorMargin = 0.05m;
                }

                if (inStockDays >= 31 && inStockDays <= 180)
                {
                    eneity.PrimaryMargin = 0.01m;
                    eneity.SeniorMargin = 0.01m;
                }

                if (inStockDays > 180)
                {
                    eneity.PrimaryMargin = -0.03m;
                    eneity.SeniorMargin = -0.03m;

                }
            }
            return eneity;
        }

        /// <summary>
        /// 根据滞销库存数获取等级
        /// </summary>
        /// <param name="inStockDays"></param>
        /// <returns></returns>
        private MinMarginDays GetMinMarginDays(int inStockDays)
        {
            MinMarginDays days = MinMarginDays.Thirty;
            if (inStockDays >= 0 && inStockDays <= 30)
            {
                days = MinMarginDays.Thirty;
            }

            if (inStockDays >= 31 && inStockDays <= 60)
            {
                days = MinMarginDays.Sixty;
            }

            if (inStockDays >= 61 && inStockDays <= 90)
            {
                days = MinMarginDays.Ninety;
            }

            if (inStockDays >= 91 && inStockDays <= 120)
            {
                days = MinMarginDays.OneHundredAndTwenty;
            }

            if (inStockDays >= 120 && inStockDays <= 180)
            {
                days = MinMarginDays.OneHundredAndEighty;
            }

            if (inStockDays > 180)
            {
                days = MinMarginDays.Other;
            }
            return days;
        }

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public virtual CategoryBasic UpdateCategoryBasic(CategoryBasic categoryBasicInfo)
        {
            CheckCategorySettingProcessor.CheckCategorySettingInfo(categoryBasicInfo);
            return _categoryDA.UpdateCategoryBasic(categoryBasicInfo);
        }

        /// <summary>
        /// 保存二级类别的基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public virtual void UpdateCategory2Basic(CategoryBasic categoryBasicInfo)
        {
            if (categoryBasicInfo != null)
            {
                CheckCategorySettingProcessor.CheckExistCategory2(categoryBasicInfo.CategorySysNo);
                _categoryDA.UpdateCategory2Basic(categoryBasicInfo);
            }
        
        }

        /// <summary>
        /// 保存RMA指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public virtual CategoryRMA UpdateCategoryRMA(CategoryRMA categoryBasicInfo)
        {
            if (categoryBasicInfo != null)
            {
                CheckCategorySettingProcessor.CheckCategorySettingSysNo(categoryBasicInfo.CategorySysNo);
            }
            CheckCategorySettingProcessor.CheckCategorySettingInfo(categoryBasicInfo);
            return _categoryDA.UpdateCategoryRMA(categoryBasicInfo);
        }

        /// <summary>
        /// 保存毛利率信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        public virtual CategoryMinMargin UpdateCategoryMinMargin(CategoryMinMargin categoryBasicInfo)
        {
            if (categoryBasicInfo != null)
            {
                CheckCategorySettingProcessor.CheckCategorySettingSysNo(categoryBasicInfo.CategorySysNo);
            }
            CheckCategorySettingProcessor.CheckCategorySettingInfo(categoryBasicInfo);
            return _categoryDA.UpdateCategoryMinMargin(categoryBasicInfo);
        }

       
        /// <summary>
        /// 更新类别2的毛利率信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public virtual void UpdateCategory2MinMargin(CategoryMinMargin categoryBasicInfo)
        {
            if (categoryBasicInfo != null)
            {
                CheckCategorySettingProcessor.CheckCategory2MinMargin(categoryBasicInfo);
                _categoryDA.UpdateCategory2MinMargin(categoryBasicInfo);
            }
        }

        /// <summary>
        /// 批量保存三级类别
        /// </summary>
        /// <param name="categoryMinMarginList"></param>
        public virtual void UpdateCategory3MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            foreach (var item in categoryMinMarginList)
            {
                UpdateCategoryMinMargin(item);
            }
        }
        /// <summary>
        /// 批量保存二级类别
        /// </summary>
        /// <param name="categoryMinMarginList"></param>
        public virtual void UpdateCategory2MinMarginBat(List<CategoryMinMargin> categoryMinMarginList)
        {
            foreach (var item in categoryMinMarginList)
            {
                UpdateCategory2MinMargin(item);
            }
        }
        /// <summary>
        /// 更新限额
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        public virtual void UpdateCategoryProductMinCommission(CategoryBasic categoryBasicInfo)
        {
            _categoryDA.UpdateCategoryProductMinCommission(categoryBasicInfo);
        }
        /// <summary>
        /// 批量更新限额 
        /// </summary>
        /// <param name="categoryBasicList"></param>
        public virtual void UpdateCategory2ProductMinCommission(List<CategoryBasic> categoryBasicList)
        {
            foreach (var item in categoryBasicList)
            {
                UpdateCategoryProductMinCommission(item);
            }
        }
        /// <summary>
        /// 根据类别2得到2级指标
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public CategorySetting GetCategorySettingByCategory2SysNo(int SysNo)
        {
            var result = _categoryDA.GetCategorySettingByCategory2SysNo(SysNo);
            if (result != null && result.CategoryBasicInfo != null)
            {
                result.CategoryBasicInfo.VirtualRate = result.CategoryBasicInfo.VirtualRate * 100;
                result.CategoryBasicInfo.OOSRate = result.CategoryBasicInfo.OOSRate * 100;
            }
            if (result != null && result.CategoryMinMarginInfo != null
                && result.CategoryMinMarginInfo.Margin != null
                && result.CategoryMinMarginInfo.Margin.Count > 0)
            {
                result.CategoryMinMarginInfo.Margin.ForEach(v =>
                {
                    v.Value.MaxMargin = v.Value.MaxMargin * 100;
                    v.Value.MinMargin = v.Value.MinMargin * 100;
                });
            }
            return result;
        }
        #endregion

        #region 检查品牌逻辑
        private static class CheckCategorySettingProcessor
        {
            /// <summary>
            /// 检查指标信息
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckCategorySettingInfo<T>(T entity) where T : class
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "InValidCategorySetting"));
                }
                var pro = entity.GetType().GetProperty("CategorySysNo");
                if (pro == null) return;
                var categorySysNo = Convert.ToInt32(entity.GetType().GetProperty("CategorySysNo").GetValue(entity, null));
                if (categorySysNo != 0)
                {
                    CheckCategorySettingSysNo(categorySysNo);
                    CheckExistCategory(categorySysNo);
                    if (entity.GetType().Name.Contains("CategoryMinMargin"))
                    {
                        object t = entity;
                        var tempEntity = (CategoryMinMargin)t;
                        CategoryMinMargin(tempEntity);
                    }
                }


            }

            /// <summary>
            /// 检查三级分类编号
            /// </summary>
            /// <param name="categorySettingSysNo"></param>
            public static void CheckCategorySettingSysNo(int? categorySettingSysNo)
            {

                if (categorySettingSysNo == null || categorySettingSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "InValidCategorySysNo"));
                }
            }

            /// <summary>
            /// 检查商品编号
            /// </summary>
            /// <param name="productSysNo"></param>
            public static void CheckProductSysNo(int? productSysNo)
            {

                if (productSysNo == null || productSysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "ProductSysNoIsNull"));
                }
            }

            /// <summary>
            /// 检查类别2的毛利率
            /// </summary>
            /// <param name="minMargin"></param>
            public static void CheckCategory2MinMargin(CategoryMinMargin minMargin)
            {
                CategoryMinMargin(minMargin);
            }

            /// <summary>
            /// 是否存在某个三级分类
            /// </summary>
            /// <param name="categorySettingSysNo"></param>
            /// <returns></returns>
            private static void CheckExistCategory(int categorySettingSysNo)
            {
                var categoryDA = ObjectFactory<ICategoryDA>.Instance;
                var result = categoryDA.IsExistsCategoryBySysNo(categorySettingSysNo);
                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "ExistsCategorySetting"));
                }

            }

            /// <summary>
            /// 是否存在某个二级分类
            /// </summary>
            /// <param name="SysNo"></param>
            public  static void CheckExistCategory2(int SysNo)
            {
                var categoryDA = ObjectFactory<ICategoryDA>.Instance;
                var result = categoryDA.IsExistsCategory2BySysNo(SysNo);
                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Category", "NotExistsCategory2"));
                }
            }

            /// <summary>
            /// 检查三级分类毛利率指标
            /// </summary>
            /// <param name="entity"></param>
            private static void CategoryMinMargin(CategoryMinMargin entity)
            {
                if (entity.Margin == null || entity.Margin.Count == 0) return;
                var minMarginValue =
                    (from p in entity.Margin
                     orderby p.Value.MinMargin descending
                     select (int)p.Key).ToList();

                var minMarginKey =
                  (from p in entity.Margin
                   orderby p.Key ascending
                   select (int)p.Key).ToList();

                var result = ListEquals(minMarginValue, minMarginKey);
                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "minMarginIsNull"));
                }

                var maxMarginValue =
                    (from p in entity.Margin
                     orderby p.Value.MaxMargin descending
                     select (int)p.Key).ToList();


                var maxMarginKey =
                  (from p in entity.Margin
                   orderby p.Key ascending
                   select (int)p.Key).ToList();

                //bool result = ListEquals(maxMarginValue, maxMarginKey);


                result = ListEquals(maxMarginValue, maxMarginKey);

                if (!result)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategorySetting", "MaxMarginIsNull"));
                }

                var source = (from p in entity.Margin
                              where p.Value.MinMargin < p.Value.MaxMargin
                              select p.Key).ToList();

                if (source.Count > 0)
                {
                    var desc = source[0].ToEnumDesc();
                    throw new BizException(desc + ResouceManager.GetMessageString("IM.CategorySetting", "MarginIsNull"));
                }

            }

            /// <summary>
            /// 比较两个list
            /// </summary>
            /// <param name="souceList"></param>
            /// <param name="targeList"></param>
            /// <returns></returns>
            private static bool ListEquals(List<int> souceList, List<int> targeList)
            {
                if (souceList == null && targeList == null) return true;
                if (souceList == null) return false;
                if (targeList == null) return false;
                if (souceList.Count == 0 && targeList.Count == 0) return true;
                if (souceList.Count != targeList.Count) return false;
                var count = souceList.Count;
                bool result = true;
                for (int i = 0; i < count; i++)
                {
                    if (souceList[i] != targeList[i])
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
        }
        #endregion


    }
}
