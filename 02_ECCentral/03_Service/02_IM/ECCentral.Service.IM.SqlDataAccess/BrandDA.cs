//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理业务数据底层接口实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.IM;
using System.Data;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IBrandDA))]
    public class BrandDA : IBrandDA
    {

        /// <summary>
        /// 根据SysNO获取品牌信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public virtual BrandInfo GetBrandInfoBySysNo(int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandInfoBySysNo");
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            var sourceEntity = cmd.ExecuteEntity<BrandInfo>();
            return sourceEntity;
        }

        /// <summary>
        ///  获取品牌列表
        /// </summary>
        /// <returns></returns>
        public List<BrandInfo> GetBrandInfoList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandInfoList");
            var sourceEntity = cmd.ExecuteEntityList<BrandInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 创建品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BrandInfo CreateBrand(BrandInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateBrand");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@BrandName_Ch", entity.BrandNameLocal.Content);
            cmd.SetParameterValue("@BrandName_En", entity.BrandNameGlobal);
            cmd.SetParameterValue("@Note", entity.BrandDescription != null ? entity.BrandDescription.Content : null);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@ManufacturerSysNo", entity.Manufacturer.SysNo);
            cmd.SetParameterValue("@ServiceEmail", entity.BrandSupportInfo.ServiceEmail);
            cmd.SetParameterValue("@ServicePhone", entity.BrandSupportInfo.ServicePhone);
            cmd.SetParameterValue("@ServiceUrl", entity.BrandSupportInfo.ServiceUrl);
            cmd.SetParameterValue("@WebSite", entity.BrandSupportInfo.ManufacturerUrl);
            cmd.SetParameterValue("@Type", "0");
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.SetParameterValue("@InUser", entity.User.UserName);
            cmd.SetParameterValue("@BrandCode", entity.BrandCode);
            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return entity;
        }

        /// <summary>
        /// 修改品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BrandInfo UpdateBrand(BrandInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandMaster");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@BrandName_Ch", entity.BrandNameLocal.Content);
            cmd.SetParameterValue("@BrandName_En", entity.BrandNameGlobal);
            cmd.SetParameterValue("@Note", entity.BrandDescription != null ? entity.BrandDescription.Content : null);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@ManufacturerSysNo", entity.Manufacturer.SysNo);
            cmd.SetParameterValue("@ServiceEmail", entity.BrandSupportInfo.ServiceEmail);
            cmd.SetParameterValue("@ServicePhone", entity.BrandSupportInfo.ServicePhone);
            cmd.SetParameterValue("@ServiceUrl", entity.BrandSupportInfo.ServiceUrl);
            cmd.SetParameterValue("@WebSite", entity.BrandSupportInfo.ManufacturerUrl);
            cmd.SetParameterValue("@Type", entity.BrandStoreType);
            cmd.SetParameterValue("@HasLogo", entity.IsLogo);
            cmd.SetParameterValue("@NeweggUrl", entity.Manufacturer.ShowUrl);
            cmd.SetParameterValue("@IsShowInZone", entity.Manufacturer.IsShowZone);
            cmd.SetParameterValue("@ADImage", entity.Manufacturer.BrandImage);
            cmd.SetParameterValue("@BrandStory", entity.BrandStory);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.SetParameterValue("@EditUser", entity.User.UserName);
            cmd.SetParameterValue("@BrandCode", entity.BrandCode);
            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 是否存在除本身之外相同名称的名牌
        /// </summary>
        /// <param name="name"></param>
        /// <param name="brandSysNo"></param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public bool IsExistBrandName(string name, int brandSysNo, int manufacturerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistBrandName");
            cmd.SetParameterValue("@BrandName_En", name.Trim());
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
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
        /// 是否存在相同名称的名牌
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public bool IsExistBrandName(string name, int manufacturerSysNo)
        {
            var result = IsExistBrandName(name, 0, manufacturerSysNo);
            return result;
        }

        /// <summary>
        /// 是否有正在被商品使用的品牌
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        public bool IsBrandInUsing(int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsBrandInUsing");
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);

            return cmd.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 根据生产商修改该生产商下所有的品牌
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateBrandMasterByManufacturerSysNo(BrandInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandMasterByManufacturerSysNo");
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@ManufacturerSysNo", entity.Manufacturer.SysNo);
            cmd.SetParameterValue("@ServiceEmail", entity.BrandSupportInfo.ServiceEmail);
            cmd.SetParameterValue("@ServicePhone", entity.BrandSupportInfo.ServicePhone);
            cmd.SetParameterValue("@ServiceUrl", entity.BrandSupportInfo.ServiceUrl);
            cmd.SetParameterValue("@WebSite", entity.BrandSupportInfo.ManufacturerUrl);
            cmd.SetParameterValue("@Type", entity.BrandStoreType);
            cmd.SetParameterValue("@HasLogo", entity.IsLogo);
            cmd.SetParameterValue("@NeweggUrl", entity.Manufacturer.ShowUrl);
            cmd.SetParameterValue("@IsShowInZone", entity.Manufacturer.IsShowZone);
            cmd.SetParameterValue("@ADImage", entity.Manufacturer.BrandImage);
            cmd.SetParameterValue("@EditUser", entity.User.UserName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量设置置顶
        /// </summary>
        /// <param name="sysNos">SysNo拼接字符串逗号隔开</param>
        public void SetTopBrands(string sysNos)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetTopBrands");
            cmd.SetParameterValue("@SysNos", sysNos);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 根据品牌SysNo得到该品牌的所有授权状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public DataTable GetBrandAuthorizedByBrandSysNo(BrandAuthorizedFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandAuthorizedByBrandSysNo");
            cmd.SetParameterValue("@BrandSysNo", query.BrandSysNo);
            cmd.SetParameterValue("@pageSize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", query.PageInfo.PageIndex);
            cmd.SetParameterValue("@SortField", query.PageInfo.SortBy);

            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(6, typeof(AuthorizedStatus));
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;

        }

        /// <summary>
        /// 删除授权牌
        /// </summary>
        /// <param name="sysNo"></param>
        public void DeleteBrandAuthorized(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteBrandAuthorized");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新授权牌的状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBrandAuthorized(BrandAuthorizedInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateBrandAuthorized");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@Status", info.AuthorizedStatus);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandAuthorized(BrandAuthorizedInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertBrandAuthorized");
            cmd.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", info.ReferenceSysNo);
            cmd.SetParameterValue("@Type", info.Type);
            cmd.SetParameterValue("@ImageName", info.ImageName);
            cmd.SetParameterValue("@EndTime", info.EndActiveTime);
            cmd.SetParameterValue("@BeginTime", info.StartActiveTime);
            cmd.SetParameterValue("@Status", info.AuthorizedStatus);
            cmd.SetParameterValue("@InUser", info.User.UserName);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", info.CompanyCode);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsExistBrandAuthorized(BrandAuthorizedInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistBrandAuthorized");
            cmd.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", info.ReferenceSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }

        /// <summary>
        /// 根据品牌SysNo和类别SysNo删除
        /// </summary>
        /// <param name="info"></param>
        public void DeleteBrandAuthorizeBySysNoAndBrandSysNo(BrandAuthorizedInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteBrandAuthorizeBySysNoAndBrandSysNo");
            cmd.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", info.ReferenceSysNo);
            cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// 检测授权牌
        /// </summary>
        /// <param name="info"></param>
        public bool CheckAuthorized(BrandAuthorizedInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckAuthorized");
            cmd.SetParameterValue("@ImageName", info.ImageName);
            cmd.SetParameterValue("@ReferenceSysno", info.ReferenceSysNo);
            cmd.SetParameterValue("@BrandSysNo", info.BrandSysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@Flag") < 0;

        }



        /// <summary>
        /// 检测授权牌
        /// </summary>
        /// <param name="info"></param>
        public bool CheckBrandCodeIsExit(string brandCode, int? brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckBrandCodeIsExit");
            cmd.SetParameterValue("@BrandCode", brandCode);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            return cmd.ExecuteScalar<int>() > 0;


        }

        /// <summary>
        /// 自动生成品牌Code
        /// </summary>
        /// <returns>BrandCode</returns>
        public virtual string GetBrandCode()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetBrandCode");
            var code = cmd.ExecuteScalar<string>();
            var temp = StringUtility.Convert36To10(code) + 1;
            var newCode = StringUtility.ConvertTo36(temp);
            switch (newCode.Length)
            {
                case 1:
                    return "00" + newCode;
                case 2:
                    return "0" + newCode;
            }
            return newCode;
        }
    }
}
