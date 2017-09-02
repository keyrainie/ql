//************************************************************************
// 用户名				泰隆优选
// 系统名				类别配件管理
// 子系统名		        类别配件管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategoryAccessoriesDA))]
    internal class CategoryAccessoriesDA : ICategoryAccessoriesDA
    {

        /// <summary>
        /// 根据SysNO获取三级分类配件信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryAccessory GetCategoryAccessoriesBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCategoryAccessoriesBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<CategoryAccessory>();
        }


        /// <summary>
        /// 创建三级属性信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryAccessory CreatetCategoryAccessories(CategoryAccessory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreatetCategoryAccessories");
            cmd.SetParameterValue("@AccessoryName", entity.Accessory.AccessoryName.Content);
            cmd.SetParameterValue("@AccessoryOrder", entity.Priority);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);
            cmd.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@IsDefault", entity.IsDefault);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return entity;
        }

        /// <summary>
        /// 修改三级属性信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryAccessory UpdateCategoryAccessories(CategoryAccessory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryAccessories");
            cmd.SetParameterValue("@AccessoryName", entity.Accessory.AccessoryName.Content);
            cmd.SetParameterValue("@AccessoryOrder", entity.Priority);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);
            cmd.SetParameterValue("@UpdateUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@IsDefault", entity.IsDefault);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 在某个三级分类下面是否存在某个配件
        /// </summary>
        /// <param name="accessoriesSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public bool IsExistCategoryAccessories(int accessoriesSysNo, int categorySysNo)
        {
            return IsExistCategoryAccessories(accessoriesSysNo, categorySysNo, 0);
        }

        /// <summary>
        /// 在某个三级分类下面是否存在某个配件
        /// </summary>
        /// <param name="accessoriesSysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool IsExistCategoryAccessories(int accessoriesSysNo, int categorySysNo, int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistCategoryAccessories");
            cmd.SetParameterValue("@AccessoriesSysNo", accessoriesSysNo);
            cmd.SetParameterValue("@C3SysNo", categorySysNo);
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

    }
}