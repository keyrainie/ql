//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理
// 子系统名		        厂商管理N业务接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IManufacturerDA
    {
        /// <summary>
        /// 根据SysNo获取厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        ManufacturerInfo GetManufacturerInfoBySysNo(int manufacturerSysNo);

        /// <summary>
        /// 创建厂商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ManufacturerInfo CreateManufacturer(ManufacturerInfo entity);

        /// <summary>
        /// 修改厂商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ManufacturerInfo UpdateManufacturer(ManufacturerInfo entity);

        /// <summary>
        /// 是否存在除本身之外具有相同国际化名称
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        int IsExistManufacturerName(string name, int manufacturerSysNo);

        /// <summary>
        /// 具是否存在相同名称的国际化名称
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <returns></returns>
        bool IsExistManufacturerName(string name);

        /// <summary>
        /// 是否有正在被商品使用的厂商
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        bool IsManufacturerInUsing(int manufacturerSysNo);

        /// <summary>
        /// 是否存在除本身之外具有相同生产商ID
        /// </summary>
        /// <param name="manufacturerID">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        bool IsExistManufacturerID(string manufacturerID, int manufacturerSysNo);

        /// <summary>
        /// 具是否存在相同生产商ID
        /// </summary>
        /// <param name="manufacturerID">国际化名称</param>
        /// <returns></returns>
        bool IsExistManufacturerID(string manufacturerID);

        /// <summary>
        /// 根据BrandSysNo获取厂商信息
        /// </summary>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        ManufacturerInfo GetManufacturerInfoByBrandSysNo(int brandSysNo);

        System.Collections.Generic.List<ManufacturerInfo> GetAllManufacturer(string companyCode);

        /// <summary>
        /// 更新时检查是否存在生产商
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool IsExistManufacturerByUpdate(ManufacturerInfo info);

        /// <summary>
        /// 删除旗舰店首页分类
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        void DeleteBrandShipCategory(int sysNo);

        /// <summary>
        /// 创建旗舰店首页分类
        /// </summary>
        /// <param name="brandShipCategory"></param>
        /// <returns></returns>
        int CreateBrandShipCategory(BrandShipCategory brandShipCategory);
    }
}
