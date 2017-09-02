//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理业务数据底层接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IBrandDA
    {
        /// <summary>
        /// 根据SysNO获取品牌信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        BrandInfo GetBrandInfoBySysNo(int brandSysNo);

        /// <summary>
        /// 创建品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        BrandInfo CreateBrand(BrandInfo entity);

        /// <summary>
        /// 修改品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        BrandInfo UpdateBrand(BrandInfo entity);

        /// <summary>
        /// 某个厂商是否存在除本身之外具有相同名称的名牌
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <param name="brandSysNo"></param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        bool IsExistBrandName(string name, int brandSysNo, int manufacturerSysNo);

        /// <summary>
        /// 某个厂商具是否存在相同名称的名牌
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        bool IsExistBrandName(string name, int manufacturerSysNo);

        /// <summary>
        /// 是否有正在被商品使用的品牌
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        bool IsBrandInUsing(int brandSysNo);

        List<BrandInfo> GetBrandInfoList();

        /// <summary>
        /// 批量设置品牌的置顶
        /// </summary>
        /// <param name="sysNos"></param>
        void SetTopBrands(string sysNos);

        /// <summary>
        /// 跟据生产商更新品牌
        /// </summary>
        /// <param name="entity"></param>
        void UpdateBrandMasterByManufacturerSysNo(BrandInfo entity);

        /// <summary>
        /// 检查品牌code是否已被使用
        /// </summary>
        /// <param name="brandCode"></param>
        /// <returns></returns>
        bool CheckBrandCodeIsExit(string brandCode, int? brandSysNo);

        /// <summary>
        /// 自动生成品牌Code
        /// </summary>
        /// <returns>品牌Code</returns>
        string GetBrandCode();

        #region "品牌的授权操作"

        /// <summary>
        /// 根据品牌SysNo得到该品牌的所有授权信息
        /// </summary>
        /// <returns></returns>
        DataTable GetBrandAuthorizedByBrandSysNo(BrandAuthorizedFilter query, out int totalCount);

        /// <summary>
        ///删除授权信息
        /// </summary>
        /// <param name="sysNo"></param>
        void DeleteBrandAuthorized(int sysNo);

        /// <summary>
        /// 修改授权信息
        /// </summary>
        /// <param name="info"></param>
        void UpdateBrandAuthorized(BrandAuthorizedInfo info);

        /// <summary>
        /// 插入授权信息
        /// </summary>
        /// <param name="info"></param>
        void InsertBrandAuthorized(BrandAuthorizedInfo info);

        /// <summary>
        /// 是否存在该授权信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool IsExistBrandAuthorized(BrandAuthorizedInfo info);

        /// <summary>
        /// 根据品牌SysNo和类别SysNo删除
        /// </summary>
        /// <param name="info"></param>
        void DeleteBrandAuthorizeBySysNoAndBrandSysNo(BrandAuthorizedInfo info);

        /// <summary>
        /// 检测授权牌
        /// </summary>
        /// <param name="info"></param>
        bool CheckAuthorized(BrandAuthorizedInfo info);
        #endregion
    }
}
