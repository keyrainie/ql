//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理业务接口实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IManufacturerDA))]
    public class ManufacturerDA : IManufacturerDA
    {
        /// <summary>
        /// 根据SysNO查询厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo GetManufacturerInfoBySysNo(int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetManufacturerInfoBySysNo");
            cmd.SetParameterValue("@SysNo", manufacturerSysNo);
            var sourceEntity = cmd.ExecuteEntity<ManufacturerInfo>() ??
                              new ManufacturerInfo { SysNo = manufacturerSysNo };
            return sourceEntity;
        }

        /// <summary>
        /// 创建厂商信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo CreateManufacturer(ManufacturerInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateManufacturer");
            cmd.SetParameterValue("@ManufacturerName", entity.ManufacturerNameLocal != null ? entity.ManufacturerNameLocal.Content : null);
            cmd.SetParameterValue("@BriefName", entity.ManufacturerNameGlobal);
            cmd.SetParameterValue("@Note", entity.ManufacturerDescription != null ? entity.ManufacturerDescription.Content : null);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@ManufacturerWebSite", entity.SupportInfo.ManufacturerUrl);
            cmd.SetParameterValue("@SupportEmail", entity.SupportInfo.ServiceEmail);
            cmd.SetParameterValue("@SupportURL", entity.SupportInfo.ServiceUrl);
            cmd.SetParameterValue("@Type", 0);
            cmd.SetParameterValue("@CustomerServicePhone", entity.SupportInfo.ServicePhone);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.ExecuteNonQuery();
            if (entity.SysNo == 0 || entity.SysNo == null)
            {
                entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            }
            return entity;
        }

        /// <summary>
        /// 修改厂商信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo UpdateManufacturer(ManufacturerInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateManufacturer");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@ManufacturerID", entity.ManufacturerID);
            cmd.SetParameterValue("@ManufacturerName", entity.ManufacturerNameLocal.Content);
            cmd.SetParameterValue("@BriefName", entity.ManufacturerNameGlobal);
            cmd.SetParameterValue("@Note", entity.ManufacturerDescription != null ? entity.ManufacturerDescription.Content : null);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@ManufacturerWebSite", entity.SupportInfo.ManufacturerUrl);
            cmd.SetParameterValue("@SupportEmail", entity.SupportInfo.ServiceEmail);
            cmd.SetParameterValue("@SupportURL", entity.SupportInfo.ServiceUrl);
            cmd.SetParameterValue("@Type", entity.BrandStoreType);
            cmd.SetParameterValue("@CustomerServicePhone", entity.SupportInfo.ServicePhone);
            cmd.SetParameterValue("@IsLogo", entity.IsLogo);
            cmd.SetParameterValue("@IsShowZone", entity.IsShowZone);
            cmd.SetParameterValue("@AdImage", entity.BrandImage);
            cmd.SetParameterValue("@NeweggUrl", entity.ShowUrl);
            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 是否存在除本身之外具有相同国际化名称
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public int IsExistManufacturerName(string name, int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistManufacturerName");
            cmd.SetParameterValue("@SysNo", manufacturerSysNo);
            cmd.SetParameterValue("@BriefName", name);
            cmd.SetParameterValue("@CompanyCode", "8601");
            var value = cmd.ExecuteScalar();
            int count = Convert.ToInt32(value);
            return count;
        }

 
        /// <summary>
        /// 具是否存在相同名称的国际化名称
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <returns></returns>
        public bool IsExistManufacturerName(string name)
        {
            var isExist = IsExistManufacturerName(name, 0)>0;
            return isExist;
        }

        /// <summary>
        /// 是否有正在被商品使用的厂商
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public bool IsManufacturerInUsing(int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsManufacturerInUsing");
            cmd.SetParameterValue("@ManufacturerSysNo", manufacturerSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 是否存在除本身之外具有相同生产商ID
        /// </summary>
        /// <param name="manufacturerID">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public bool IsExistManufacturerID(string manufacturerID, int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistManufacturerID");
            cmd.SetParameterValue("@SysNo", manufacturerSysNo);
            cmd.SetParameterValue("@ManufacturerID", manufacturerID);
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
        /// 具是否存在相同生产商ID
        /// </summary>
        /// <param name="manufacturerID">国际化名称</param>
        /// <returns></returns>
        public bool IsExistManufacturerID(string manufacturerID)
        {
            var isExist = IsExistManufacturerName(manufacturerID, 0)>0;
            return isExist;
        }

        /// <summary>
        /// 根据BrandSysNo获取厂商信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public virtual ManufacturerInfo GetManufacturerInfoByBrandSysNo(int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetManufacturerInfoByBrandSysNo");
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            var sourceEntity = cmd.ExecuteEntity<ManufacturerInfo>();
            return sourceEntity;
        }

        #region IManufacturerDA Members


        public List<ManufacturerInfo> GetAllManufacturer(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllManufacturerInfo");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<ManufacturerInfo>();
        }

        #endregion

        /// <summary>
        /// 更新时检查是否存在生产商
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistManufacturerByUpdate(ManufacturerInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistManufacturerByUpdate");
            cmd.SetParameterValue("@ManufacturerName", info.ManufacturerNameLocal.Content);
            cmd.SetParameterValue("@ManufacturerBriefName", info.ManufacturerNameGlobal);
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        public void DeleteBrandShipCategory(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteBrandShipCategory");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建旗舰店首页分类
        /// </summary>
        /// <param name="brandShipCategory"></param>
        public int CreateBrandShipCategory(BrandShipCategory brandShipCategory)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AddBrandShipCategory");
            cmd.SetParameterValue("@ECC3SysNo", brandShipCategory.BrandShipCategoryID);
            cmd.SetParameterValue("@ManufacturerSysNo", brandShipCategory.ManufacturerSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag");
        }


    }
}
