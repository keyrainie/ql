//************************************************************************
// 用户名				泰隆优选
// 系统名				类别指标管理
// 子系统名		        类别指标管理业务数据底层接口
// 作成者				Tom
// 改版日				2012.5.23
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICategorySettingDA
    {
        /// <summary>
        /// 根据三级分类获取三级指标
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CategorySetting GetCategorySettingBySysNo(int SysNo);

        /// <summary>
        /// 保存基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        CategoryBasic UpdateCategoryBasic(CategoryBasic categoryBasicInfo);

        /// <summary>
        /// 更新类别2的基本指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        void UpdateCategory2Basic(CategoryBasic categoryBasicInfo);

        /// <summary>
        /// 保存RMA指标信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        CategoryRMA UpdateCategoryRMA(CategoryRMA categoryBasicInfo);

        /// <summary>
        /// 保存毛利率信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        CategoryMinMargin UpdateCategoryMinMargin(CategoryMinMargin categoryBasicInfo);

        /// <summary>
        /// 保存类别2毛利率信息
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        /// <returns></returns>
        void UpdateCategory2MinMargin(CategoryMinMargin categoryBasicInfo);

        /// <summary>
        /// 修改最低佣金限额
        /// </summary>
        /// <param name="categoryBasicInfo"></param>
        void UpdateCategoryProductMinCommission(CategoryBasic categoryBasicInfo);

        /// <summary>
        /// 根据类别2得到2级指标
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        CategorySetting GetCategorySettingByCategory2SysNo(int SysNo);
    }
}
