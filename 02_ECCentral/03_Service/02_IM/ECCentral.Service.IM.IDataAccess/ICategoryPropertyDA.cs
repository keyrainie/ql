//************************************************************************
// 用户名				泰隆优选
// 系统名				类别属性管理
// 子系统名		        类别属性管理业务数据底层接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************


using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ICategoryPropertyDA
    {
        /// <summary>
        /// 根据SysNO获取三级属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        CategoryProperty GetCategoryPropertyBySysNo(int categorySysNo);


        /// <summary>
        /// 根据SysNO删除三级属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        void DelCategoryPropertyBySysNo(int categorySysNo);

        ///// <summary>
        ///// 创建三级属性信息
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        CategoryProperty CreateCategoryProperty(CategoryProperty entity);

        ///// <summary>
        ///// 修改三级属性信息
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        CategoryProperty UpdateCategoryProperty(CategoryProperty entity);

        /// <summary>
        /// 在某个三级分类下面是否存在某个属性
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        bool IsExistProperty(int propertySysNo, int categorySysNo);

        /// <summary>
        /// 从分组属性切换到别的属性，
        /// 如果分组属性表(OverseaContentManagement.dbo.ProductCommonInfo_Property)中有该PropertySysno</summary>
        /// 则提示，不能更改属性类型
        /// <param name="propertySysNo"></param>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        bool GetProductCommonInfoPropertyByPropertySysNo(int propertySysNo, int categorySysNo);

        /// <summary>
        /// 如果删除分组类型，需要check分组属性表是否有记录
        /// 如果有则说明已经生成CommonSKU，则不能删除
        /// </summary>
        /// <param name="propertySysNo"></param>
        /// <returns></returns>
        bool IsCategoryPropertyForDGInUsing(int propertySysNo);


        /// <summary>
        /// 根据三级分类获取属性
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        List<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo);

        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        int CopyCategoryOutputTemplateProperty(CategoryProperty property);


    }
}
