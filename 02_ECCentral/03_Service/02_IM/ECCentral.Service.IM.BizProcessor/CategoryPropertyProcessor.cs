//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryPropertyProcessor))]
    public class CategoryPropertyProcessor
    {

        private readonly ICategoryPropertyDA _categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
        private static readonly PropertyType[] PropertyTypes = new[] { PropertyType.Other };

        #region 分类属性业务方法
        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CategoryProperty GetCategoryPropertyBySysNo(int sysNo)
        {
            CheckCategoryPropertyProcessor.CheckCategoryPropertySysNo(sysNo);
            return _categoryPropertyDA.GetCategoryPropertyBySysNo(sysNo);
        }

        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public virtual List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            CheckCategoryPropertyProcessor.CheckCategoryPropertySysNo(categorySysNo);
            return _categoryPropertyDA.GetCategoryPropertyByCategorySysNo(categorySysNo);
        }

        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual void DelCategoryPropertyBySysNo(int sysNo)
        {
            CheckCategoryPropertyProcessor.CheckCategoryPropertyInUsing(sysNo);
            _categoryPropertyDA.DelCategoryPropertyBySysNo(sysNo);
        }

        /// <summary>
        /// 创建分类属性信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryProperty CreateCategoryProperty(CategoryProperty entity)
        {
            CheckCategoryPropertyProcessor.CheckCategoryPropertyInfo(entity);
            return _categoryPropertyDA.CreateCategoryProperty(entity);
        }

        /// <summary>
        /// 修改分类属性信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryProperty UpdateCategoryProperty(CategoryProperty entity)
        {
            if (entity != null)
            {
                CheckCategoryPropertyProcessor.CheckCategoryPropertySysNo(entity.SysNo);
            }
            CheckCategoryPropertyProcessor.CheckCategoryPropertyInfo(entity);
            return _categoryPropertyDA.UpdateCategoryProperty(entity);
        }

        public void CopyCategoryOutputTemplateProperty(CategoryProperty property)
        {
            int result = _categoryPropertyDA.CopyCategoryOutputTemplateProperty(property);
            if (result == -1)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Category", "CategoryPropertyTemplateIsNull"));
            }
        }
        /// <summary>
        /// 批量更新CategoryProperty
        /// </summary>
        /// <param name="listCategoryProperty"></param>
        public void UpdateCategoryPropertyByList(List<CategoryProperty> listCategoryProperty)
        {
            foreach (var item in listCategoryProperty)
            {
                if (item != null)
                {
                    _categoryPropertyDA.UpdateCategoryProperty(item);
                }
            }
        }
        #endregion

        #region 检查分类属性逻辑
        private static class CheckCategoryPropertyProcessor
        {
            /// <summary>
            /// 检查分类属性实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckCategoryPropertyInfo(CategoryProperty entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategoryPropertyIsNull"));
                }
                if (entity.Property == null || entity.Property.SysNo < 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "PropertySysNoIsNull"));
                }
                if (entity.PropertyGroup != null && entity.PropertyGroup.PropertyGroupName != null && !String.IsNullOrEmpty(entity.PropertyGroup.PropertyGroupName.Content))
                {
                    entity.PropertyGroup.PropertyGroupName.Content =
                        entity.PropertyGroup.PropertyGroupName.Content.Trim();
                }
                if (entity.PropertyGroup == null || entity.PropertyGroup.PropertyGroupName == null || String.IsNullOrEmpty(entity.PropertyGroup.PropertyGroupName.Content))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "PropertyGroupNameIsNull"));
                }
                if (entity.CategoryInfo == null || entity.CategoryInfo.SysNo == null || entity.CategoryInfo.SysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategorySysNoIsNull"));
                }

                if (entity.SysNo == null)
                {
                    CheckExistCategoryProperty(entity.Property.SysNo, entity.CategoryInfo.SysNo);
                }
                else if (entity.SysNo.Value > 0 && PropertyTypes.Contains(entity.PropertyType))
                {
                    var categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
                    var tempEnetity = categoryPropertyDA.GetCategoryPropertyBySysNo(entity.SysNo.Value);
                    if (tempEnetity == null)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "ExistsCategoryProperty"));
                    }
                    if (tempEnetity.PropertyType == PropertyType.Grouping)
                    {
                        CheckCategoryPropertyInUsing(entity.Property.SysNo, entity.CategoryInfo.SysNo);
                    }
                }

            }

            /// <summary>
            /// 检查分类属性编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckCategoryPropertySysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategoryPropertySysNOIsNull"));
                }
            }

            /// <summary>
            /// 是否存在某个相同属性
            /// </summary>
            /// <param name="propertySysNo"></param>
            /// <param name="categorySysNo"></param>
            /// <returns></returns>
            private static void CheckExistCategoryProperty(int? propertySysNo, int? categorySysNo)
            {
                if (propertySysNo == null || propertySysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "PropertySysNoIsNull"));
                }
                if (categorySysNo == null || categorySysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategorySysNoIsNull"));
                }
                var categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
                var isExist = categoryPropertyDA.IsExistProperty(propertySysNo.Value, categorySysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "IsExistCategoryProperty"));
                }
            }

            /// <summary>
            /// 在该分类中分组属性是否有商品使用
            /// </summary>
            /// <param name="propertySysNo"></param>
            /// <param name="categorySysNo"></param>
            /// <returns></returns>
            private static void CheckCategoryPropertyInUsing(int? propertySysNo, int? categorySysNo)
            {
                if (propertySysNo == null || propertySysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "PropertySysNoIsNull"));
                }
                if (categorySysNo == null || categorySysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategorySysNoIsNull"));
                }
                var categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
                var isExist = categoryPropertyDA.GetProductCommonInfoPropertyByPropertySysNo(propertySysNo.Value, categorySysNo.Value);
                if (isExist)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "UsingCategoryProperty"));
                }
            }

            /// <summary>
            /// 分组属性是否有使用
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckCategoryPropertyInUsing(int sysNo)
            {
                if (sysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategoryPropertySysNOIsNull"));
                }
                var categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
                var tempEnetity = categoryPropertyDA.GetCategoryPropertyBySysNo(sysNo);
                if (tempEnetity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "CategoryPropertyIsNull"));
                }
                if (tempEnetity.Property == null || tempEnetity.Property.SysNo == null || tempEnetity.Property.SysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "PropertySysNoIsNull"));
                }
                if (tempEnetity.PropertyType == PropertyType.Grouping)
                {
                    var isExist = categoryPropertyDA.IsCategoryPropertyForDGInUsing(tempEnetity.Property.SysNo.Value);
                    if (isExist)
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.CategoryProperty", "UsingCategoryProperty"));
                    }
                }

            }

        }
        #endregion

    }
}
