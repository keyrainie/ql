//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICategoryExtendWarrantyDA))]
    internal class CategoryExtendWarrantyDA : ICategoryExtendWarrantyDA
    {

        /// <summary>
        /// 根据SysNO获取三级分类延保信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryExtendWarranty GetCategoryExtendWarrantyBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCategoryExtendWarrantyBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<CategoryExtendWarranty>();
        }

        /// <summary>
        /// 创建三级延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryExtendWarranty CreateCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreatetCategoryExtendWarranty");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo ?? -1);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);            
            cmd.SetParameterValue("@ProductCode", entity.ProductCode);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@Years", entity.Years);
            cmd.SetParameterValue("@MinUnitPrice", entity.MinUnitPrice);
            cmd.SetParameterValue("@MaxUnitPrice", entity.MaxUnitPrice);
            cmd.SetParameterValue("@UnitPrice", entity.UnitPrice);
            cmd.SetParameterValue("@Cost", entity.Cost);
            cmd.SetParameterValue("@InUser", entity.InUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@IsECSelected", entity.IsECSelected);

            cmd.ExecuteNonQuery();
            entity.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString()); 
            return entity;
        }

        /// <summary>
        /// 修改三级延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryExtendWarranty UpdateCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCategoryExtendWarranty");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo ?? -1);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);
            cmd.SetParameterValue("@EditUser", entity.EditUser.UserDisplayName);
            cmd.SetParameterValue("@ProductCode", entity.ProductCode);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@Years", entity.Years);
            cmd.SetParameterValue("@MinUnitPrice", entity.MinUnitPrice);
            cmd.SetParameterValue("@MaxUnitPrice", entity.MaxUnitPrice);
            cmd.SetParameterValue("@UnitPrice", entity.UnitPrice);
            cmd.SetParameterValue("@Cost", entity.Cost);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@IsECSelected", entity.IsECSelected);

            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// Check三级类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CheckCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckCategoryExtendWarranty");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo ?? -1);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo);
            cmd.SetParameterValue("@Years", entity.Years);
            cmd.SetParameterValue("@MinUnitPrice", entity.MinUnitPrice);
            cmd.SetParameterValue("@MaxUnitPrice", entity.MaxUnitPrice);
            cmd.SetParameterValue("@UnitPrice", entity.UnitPrice);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            cmd.ExecuteNonQuery();

            return int.Parse(cmd.GetParameterValue("@Flag").ToString());
        }

        /// <summary>
        /// 根据SysNO获取三级分类延保排除品牌信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryExtendWarrantyDisuseBrand GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCategoryExtendWarrantyDisuseBrandBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<CategoryExtendWarrantyDisuseBrand>();
        }

        /// <summary>
        /// 创建三级延保排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryExtendWarrantyDisuseBrand CreateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateExtendWarrantyDisuseBrand");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo??-1);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@InUser", entity.InUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.ExecuteNonQuery();
            entity.SysNo = int.Parse(cmd.GetParameterValue("@SysNo").ToString());
            return entity;
        }


        /// <summary>
        /// 修改三级延保信息排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryExtendWarrantyDisuseBrand UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateExtendWarrantyDisuseBrand");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo??-1);
            cmd.SetParameterValue("@EditUser", entity.EditUser.UserDisplayName);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);

            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// Check三级类延保排除品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CheckCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckCategoryExtendWarrantyDisuseBrand");
            cmd.SetParameterValue("@BrandSysNo", entity.Brand.SysNo);
            cmd.SetParameterValue("@C3SysNo", entity.CategoryInfo.SysNo??-1);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            cmd.ExecuteNonQuery();

            return int.Parse(cmd.GetParameterValue("@Flag").ToString());
        }

    }
}