//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		        PM管理业务接口
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************


using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductManagerDA
    {
        /// <summary>
        /// 根据SysNO获取PM信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ProductManagerInfo GetProductManagerInfoBySysNo(int sysNo);

        /// <summary>
        /// 根据UserSysNo获取PM信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        ProductManagerInfo GetProductManagerInfoByUserSysNo(int userSysNo);

        /// <summary>
        /// 创建PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductManagerInfo CreateProductManagerInfo(ProductManagerInfo entity);

        /// <summary>
        /// 修改PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductManagerInfo UpdateProductManagerInfo(ProductManagerInfo entity);

        /// <summary>
        /// 除本身之外是否存在某个PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        bool IsExistPMUserSysNo(int userSysNo, int productManagerInfoSysNo);

        /// <summary>
        /// PM表中是否存在某个PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        bool IsExistPMUserSysNo(int userSysNo);

        /// <summary>
        /// 是否存在PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        bool IsExistUserSysNo(int userSysNo);

        /// <summary>
        /// 是否存在PMUserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool IsExistUserID(string userID);

        /// <summary>
        /// 是否存在PM被ProductDomain中使用
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        bool IsPMInUsingByProductDomain(int userSysNo);

        /// <summary>
        /// 是否存在PM被Product中使用
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        bool IsPMInUsingByProduct(int userSysNo);

        IEnumerable<ProductManagerInfo> GetProductManager(CategoryInfo category, BrandInfo brand);


        /// <summary>
        /// 根据UserID获取UserInfo对象
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UserInfo GetUserInfoByUserID(string userID);

        /// <summary>
        /// 根据权限获取PM List (PM控件用)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="loginName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<ProductManagerInfo> GetPMListByType(PMQueryType queryType, string loginName, string companyCode);

        /// <summary>
        /// 获取PM Leader List
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<ProductManagerInfo> GetPMLeaderList(string companyCode);
    }
}
