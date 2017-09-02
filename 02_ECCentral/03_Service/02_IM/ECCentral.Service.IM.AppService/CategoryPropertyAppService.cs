//************************************************************************
// 用户名				泰隆优选
// 系统名				分类属性管理
// 子系统名		        分类属性管理业务实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(CategoryPropertyAppService))]
    public class CategoryPropertyAppService
    {
        /// <summary>
        /// 根据SysNO获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public CategoryProperty GetCategoryPropertyBySysNo(int sysNo)
        {
            var result = ObjectFactory<CategoryPropertyProcessor>.Instance.GetCategoryPropertyBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据SysNO删除分类属性信息
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public void DelCategoryPropertyBySysNo(IList<int> sysNoList)
        {
            var oper = ObjectFactory<CategoryPropertyProcessor>.Instance;
            using (TransactionScope tran = new TransactionScope())
            {
                if (sysNoList == null || sysNoList.Count == 0) return;
                foreach (var sysNo in sysNoList)
                {
                    oper.DelCategoryPropertyBySysNo(sysNo);
                }
                tran.Complete();
            }

        }

        /// <summary>
        /// 创建分类属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryProperty CreateCategoryProperty(CategoryProperty entity)
        {
            var result = ObjectFactory<CategoryPropertyProcessor>.Instance.CreateCategoryProperty(entity);
            return result;
        }


        /// <summary>
        /// 修改分类属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CategoryProperty UpdateCategoryProperty(CategoryProperty entity)
        {
            var result = ObjectFactory<CategoryPropertyProcessor>.Instance.UpdateCategoryProperty(entity);
            return result;
        }

        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="categorySysNo"></param>
        /// <returns></returns>
        public IList<CategoryProperty> GetCategoryPropertyByCategorySysNo(int categorySysNo)
        {
            var result = ObjectFactory<CategoryPropertyProcessor>.Instance.GetCategoryPropertyByCategorySysNo(categorySysNo);
            return result;
        }
        public void CopyCategoryOutputTemplateProperty(CategoryProperty property)
        {
            ObjectFactory<CategoryPropertyProcessor>.Instance.CopyCategoryOutputTemplateProperty(property);
        }
        /// <summary>
        /// 批量更新CategoryProperty
        /// </summary>
        /// <param name="listCategoryProperty"></param>
        public void UpdateCategoryPropertyByList(List<CategoryProperty> listCategoryProperty)
        {
            ObjectFactory<CategoryPropertyProcessor>.Instance.UpdateCategoryPropertyByList(listCategoryProperty);
        }
    }
}
