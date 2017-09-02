using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategoryDA))]
    public class CategoryDA : ICategoryDA
    {
        public CategoryInfo AddCategory(CategoryInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddCategory");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@C3ID", entity.CategoryID);
            cmd.SetParameterValue("@C3Name", entity.CategoryName.Content);
            cmd.SetParameterValue("@C2SysNo", 2);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@MinMargin", 0);
            cmd.SetParameterValue("@MinMarginPMD", 0);
            cmd.SetParameterValue("@CompanyCode", "8601");

            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return entity;
        }

        public void UpdateCategory(CategoryInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategory");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@C3Name", entity.CategoryName.Content);
            cmd.SetParameterValue("@C2SysNo", 2);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.SetParameterValue("@C3Code", entity.C3Code);

            cmd.ExecuteNonQuery();

        }

        public List<CategoryInfo> GetAllCategory()
        {
            throw new NotImplementedException();
        }

        public List<CategoryInfo> GetCategory1List()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFloorUseCategory1List");
            return cmd.ExecuteEntityList<CategoryInfo>();
        }

        public CategoryInfo GetCategoryBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory3BySysNo");
            cmd.SetParameterValue("@C3SysNo", sysNo);
            var sourceEntity = cmd.ExecuteEntity<CategoryInfo>();
            return sourceEntity;
        }

        public bool IsExistsCategoryByCategoryName(string categoryName)
        {
            var result = IsExistsCategoryByCategoryName(categoryName, 0);
            return result;
        }

        public bool IsExistsCategoryByCategoryName(string categoryName, int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategoryByCategoryName");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CategoryName", categoryName);
            cmd.SetParameterValue("@CompanyCode", "8601");
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        public CategoryInfo GetCategoryByCategoryName(string categoryName)
        {
            throw new NotImplementedException();
        }

        public CategoryInfo GetCategory1BySysNo(int c1SysNo)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryCategory1ByC1SysNo");
            dataCommand.SetParameterValue("@C1SysNo", c1SysNo);
            dataCommand.SetParameterValue("@CompanyCode", "8601");
            return dataCommand.ExecuteEntity<CategoryInfo>();
        }

        public CategoryInfo GetCategory2BySysNo(int c2SysNo)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryCategory2ByC2SysNo");
            dataCommand.SetParameterValue("@C2SysNo", c2SysNo);
            dataCommand.SetParameterValue("@CompanyCode", "8601");
            return dataCommand.ExecuteEntity<CategoryInfo>();
        }

        /// <summary>
        /// 根据c3SysNO获取三级类信息
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public CategoryInfo GetCategory3BySysNo(int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory3BySysNo");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            var sourceEntity = cmd.ExecuteEntity<CategoryInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 根据C2,c3编号，获取类名称
        /// </summary>
        /// <param name="c2SysNo"></param>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public string GetCategoryC2AndC3NameBySysNo(int? c2SysNo, int? c3SysNo)
        {
            if (c3SysNo.HasValue)
            {
                DataCommand command = DataCommandManager.GetDataCommand("GetC2AndC3Name");
                command.SetParameterValue("@C2SysNo", c2SysNo);
                command.SetParameterValue("@C3SysNo", c3SysNo);
                command.SetParameterValue("@CompanyCode", 8601);
                return command.ExecuteScalar<string>();
            }
            else
            {
                DataCommand command = DataCommandManager.GetDataCommand("GetCategory2Name");
                command.SetParameterValue("@C2SysNo", c2SysNo);
                command.SetParameterValue("@CompanyCode", 8601);
                return command.ExecuteScalar<string>();
            }
        }

        public bool IsExistsCategoryByCategoryID(string categoryID)
        {
            var result = IsExistsCategoryByCategoryID(categoryID, 0);
            return result;
        }

        public bool IsExistsCategoryByCategoryID(string categoryID, int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategoryByCategoryID");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CategoryID", categoryID);
            cmd.SetParameterValue("@CompanyCode", "8601");
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        public bool IsCategoryInUsing(int categorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsCategoryInUsing");
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            return cmd.ExecuteScalar<int>() > 0;
        }

        public bool IsExistsCategoryBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategoryBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        ///根据CategoryType 添加CeateCategory
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        public void CeateCategoryByType(CategoryType type, CategoryInfo entity)
        {
            DataCommand cmd = null;
            switch (type)
            {
                case CategoryType.CategoryType1:
                    cmd = DataCommandManager.GetDataCommand("CreateCategory1");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C1ID", entity.CategoryID);
                    cmd.SetParameterValue("@C1Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@Status", entity.Status);
                    cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
                    cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
                    break;
                case CategoryType.CategoryType2:
                    cmd = DataCommandManager.GetDataCommand("CreateCategory2");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C2ID", entity.CategoryID);
                    cmd.SetParameterValue("@C2Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@C1SysNo", entity.ParentSysNumber);
                    cmd.SetParameterValue("@Status", entity.Status);
                     cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
                    cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
                    break;
                case CategoryType.CategoryType3:
                    cmd = DataCommandManager.GetDataCommand("CreateCategory3");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C3ID", entity.CategoryID);
                    cmd.SetParameterValue("@C3Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@C2SysNo", entity.ParentSysNumber);
                    cmd.SetParameterValue("@Status", entity.Status);
                    cmd.SetParameterValue("@MinMargin", 0);
                    cmd.SetParameterValue("@MinMarginPMD", 0);
                     cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
                    cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
                    cmd.SetParameterValue("@C3Code", entity.C3Code);
                    break;
                default:
                    cmd = DataCommandManager.GetDataCommand("CreateCategory1");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C1ID", entity.CategoryID);
                    cmd.SetParameterValue("@C1Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@Status", entity.Status);
                     cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
                    cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
                    break;
            }
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据CategoryType 更新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        public void UpdateCategoryByType(CategoryType type, CategoryInfo entity)
        {
            DataCommand cmd = null;
            switch (type)
            {
                case CategoryType.CategoryType1:
                    cmd = DataCommandManager.GetDataCommand("UpdateCategory1");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C1Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@Status", entity.Status);
                    break;
                case CategoryType.CategoryType2:
                    cmd = DataCommandManager.GetDataCommand("UpdateCategory2");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C2Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@C1SysNo", entity.ParentSysNumber);
                    cmd.SetParameterValue("@Status", entity.Status);
                    break;
                case CategoryType.CategoryType3:
                    cmd = DataCommandManager.GetDataCommand("UpdateCategory3");
                    cmd.SetParameterValue("@SysNo", entity.SysNo);
                    cmd.SetParameterValue("@C3Name", entity.CategoryName.Content);
                    cmd.SetParameterValue("@C2SysNo", entity.ParentSysNumber);
                    cmd.SetParameterValue("@Status", entity.Status);
                    cmd.SetParameterValue("@C3Code", entity.C3Code);
                    break;
                default:
                    break;
            }
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建Category的SysNO
        /// </summary>
        /// <returns></returns>
        public int CreateCategorySequence()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCategorySequence");
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@SysNo");
        }

        /// <summary>
        /// 根据类别检测是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsIsExistCategoryByType(CategoryType type, CategoryInfo entity)
        {
            DataCommand cmd = null;
            switch (type)
            {
                case CategoryType.CategoryType1:
                    cmd = DataCommandManager.GetDataCommand("IsExistCategory1");
                    break;
                case CategoryType.CategoryType2:
                    cmd = DataCommandManager.GetDataCommand("IsExistCategory2");
                    cmd.SetParameterValue("@ParentCategorySysNo", entity.ParentSysNumber);
                    break;
                case CategoryType.CategoryType3:
                    cmd = DataCommandManager.GetDataCommand("IsExistCategory3");
                    cmd.SetParameterValue("@ParentCategorySysNo", entity.ParentSysNumber);
                    break;
                default:
                    cmd = DataCommandManager.GetDataCommand("IsExistCategory1");
                    break;
            }
            cmd.SetParameterValue("@CategoryName", entity.CategoryName.Content);
            cmd.SetParameterValue("@CategorySysNo", entity.SysNo);
            cmd.SetParameterValue("@OperationType", entity.OperationType);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag")<0;
        }


        public DataTable GetCategoryListByType(CategoryQueryFilter query, out int totalCount)
        {
            DataCommand cmd = null;
            switch (query.Type)
            {
                case CategoryType.CategoryType1:
                    cmd = DataCommandManager.GetDataCommand("GetCategory1List");
                    break;
                case CategoryType.CategoryType2:
                    cmd = DataCommandManager.GetDataCommand("GetCategory2List");
                    cmd.SetParameterValue("@Category1SysNo", query.Category1SysNo);
                    break;
                case CategoryType.CategoryType3:
                    cmd = DataCommandManager.GetDataCommand("GetCategory3List");
                    cmd.SetParameterValue("@Category1SysNo", query.Category1SysNo);
                    cmd.SetParameterValue("@Category2SysNo", query.Category2SysNo);
                    break;
                default:
                    cmd = DataCommandManager.GetDataCommand("GetCategory1List");
                    break;
            }
                
            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            cmd.SetParameterValue("@Status", query.Status);
            cmd.SetParameterValue("@CategoryName", query.CategoryName);
             EnumColumnList enumList = new EnumColumnList
                {
                    { "CategoryStatus", typeof(CategoryStatus) },
                  
                   
                };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount =(int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }

        /// <summary>
        /// 根据SysNo检查类别2是否存在
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool IsExistsCategory2BySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsCategory2BySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 根据C3SysNo得到C1CategoryInfo
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public CategoryInfo GetCategory1ByCategory3SysNo(int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCategory1ByCategory3SysNo");
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            var sourceEntity = cmd.ExecuteEntity<CategoryInfo>();
            return sourceEntity;
        }
    }
}
