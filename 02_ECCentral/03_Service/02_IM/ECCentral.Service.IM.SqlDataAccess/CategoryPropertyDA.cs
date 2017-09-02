//************************************************************************
// 用户名				泰隆优选
// 系统名				类别属性管理
// 子系统名		        类别属性管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICategoryPropertyDA))]
    internal class CategoryPropertyDA : ICategoryPropertyDA
    {

        /// <summary>
        /// 根据SysNO获取三级属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryProperty GetCategoryPropertyBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCategoryPropertyBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<CategoryProperty>();
        }

        /// <summary>
        /// 根据SysNO获取三级属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCategoryPropertyByCategorySysNo");
            command.SetParameterValue("@CategorySysNo", categorySysNo);
            return command.ExecuteEntityList<CategoryProperty>();
        }

        /// <summary>
        /// 根据SysNO删除三级属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public void DelCategoryPropertyBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DelCategoryPropertyBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            command.ExecuteNonQuery();
        }


        ///// <summary>
        ///// 创建三级属性信息
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        public CategoryProperty CreateCategoryProperty(CategoryProperty entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatetCategoryProperty");
            command.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@PropertySysNo", entity.Property.SysNo);
            command.SetParameterValue("@IsMustInput", entity.IsMustInput);
            command.SetParameterValue("@IsInAdvSearch", entity.IsInAdvSearch);
            command.SetParameterValue("@IsItemSearch", entity.IsItemSearch);
            command.SetParameterValue("@GroupDescription", entity.PropertyGroup.PropertyGroupName.Content);
            command.SetParameterValue("@WebDisplayStyle", entity.DisplayStyle);
            command.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@LanguageCode", entity.LanguageCode);
            command.SetParameterValue("@Type", entity.PropertyType);
            command.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@flag"));
            return entity;
        }

        ///// <summary>
        ///// 修改三级属性信息
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        public CategoryProperty UpdateCategoryProperty(CategoryProperty entity)
        {
            //查询其原来信息

            if (entity.SysNo == null) return null;
            var tempEntity = GetCategoryPropertyBySysNo(entity.SysNo.Value);

            DataCommand command = DataCommandManager.GetDataCommand("UpdateCategoryProperty");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@CompanyCode", "8601");
            command.SetParameterValue("@PropertySysNo", entity.Property.SysNo);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@SearchPriority", entity.SearchPriority);
            command.SetParameterValue("@IsMustInput", entity.IsMustInput);
            command.SetParameterValue("@IsInAdvSearch", entity.IsInAdvSearch);
            command.SetParameterValue("@IsItemSearch", entity.IsItemSearch);
            command.SetParameterValue("@GroupDescription", entity.PropertyGroup.PropertyGroupName.Content);
            command.SetParameterValue("@WebDisplayStyle", entity.DisplayStyle);
            command.SetParameterValue("@LastEditUserSysNo", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@Type", entity.PropertyType);
            command.ExecuteNonQuery();

            //原来状态是O，现在修改为不是O状态，则执行删除操作
            //if (tempEntity.PropertyType == PropertyType.Basic && entity.PropertyType != PropertyType.Basic)
            //{
            //    if (entity.CategoryInfo.SysNo == null) return entity;
            //    DeleteProductGroupInfo_PropertyByGroupSysNo(entity.CategoryInfo.SysNo.Value);
            //}

            return entity;
        }

        /// <summary>
        /// 在某个三级分类下面是否存在某个属性
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public bool IsExistProperty(int propertySysNo, int categorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistProperty");
            cmd.SetParameterValue("@PropertySysNo", propertySysNo);
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }


        /// <summary>
        /// 当从O切换到G或者A时，需要查询三级分类下其组的对应的基本属性相关数据删除。
        /// </summary>
        /// <param name="c3SysNo"></param>
        private void DeleteProductGroupInfo_PropertyByGroupSysNo(int c3SysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteProductGroupInfo_PropertyByGroupSysNo");
            command.SetParameterValue("@C3SysNo", c3SysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 从分组属性切换到别的属性，
        /// 如果分组属性表(OverseaContentManagement.dbo.ProductCommonInfo_Property)中有该PropertySysno</summary>
        /// 则提示，不能更改属性类型
        /// <param name="propertySysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public bool GetProductCommonInfoPropertyByPropertySysNo(int propertySysNo, int categorySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductCommonInfoPropertyByPropertySysNo");
            command.SetParameterValue("@PropertySysNo", propertySysNo);
            command.SetParameterValue("C3SysNo", categorySysNo);
            command.SetParameterValue("@CompanyCode", "8601");
            var value = command.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 如果删除分组类型，需要check分组属性表是否有记录
        /// 如果有则说明已经生成CommonSKU，则不能删除
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <returns></returns>
        public bool IsCategoryPropertyForDGInUsing(int propertySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsCategoryPropertyForDGInUsing");
            command.SetParameterValue("@PropertySysNo", propertySysNo);

            command.ExecuteNonQuery();
            var count = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return count != 0;
        }


        /// <summary>
        /// 复制类别属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public int CopyCategoryOutputTemplateProperty(CategoryProperty property)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CopyCategoryOutputTemplateProperty");
            command.SetParameterValue("@CompanyCode", "8601");
            command.SetParameterValue("@SourceC3SysNo",property.SourceCategorySysNo);
            command.SetParameterValue("@TargetC3SysNo", property.TargetCategorySysNo);
            command.SetParameterValue("@LastEditUserSysNo",ServiceContext.Current.UserSysNo);
            command.ExecuteNonQuery();
            return (int)command.GetParameterValue("@Flag");
        }
    }
}