//************************************************************************
// 用户名				泰隆优选
// 系统名				分类配件管理
// 子系统名		        分类配件管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryAccessoriesProcessor))]
    public class CategoryAccessoriesProcessor
    {

        private readonly ICategoryAccessoriesDA _categoryAccessoriesDA = ObjectFactory<ICategoryAccessoriesDA>.Instance;

        #region 分类配件业务方法
        /// <summary>
        /// 根据SysNo获取分类属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CategoryAccessory GetCategoryAccessoryBySysNo(int sysNo)
        {
            CheckCategoryAccessoryProcessor.CheckCategoryAccessorySysNo(sysNo);
            return _categoryAccessoriesDA.GetCategoryAccessoriesBySysNo(sysNo);
        }

        /// <summary>
        /// 创建分类配件信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryAccessory CreatetCategoryAccessory(CategoryAccessory entity)
        {
            CheckCategoryAccessoryProcessor.CheckCategoryAccessoryInfo(entity);
            return _categoryAccessoriesDA.CreatetCategoryAccessories(entity);
        }

        /// <summary>
        /// 修改分类配件信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryAccessory UpdateCategoryAccessory(CategoryAccessory entity)
        {
            if (entity != null)
            {
                CheckCategoryAccessoryProcessor.CheckCategoryAccessorySysNo(entity.SysNo);
            }
            CheckCategoryAccessoryProcessor.CheckCategoryAccessoryInfo(entity);
            return _categoryAccessoriesDA.UpdateCategoryAccessories(entity);
        }


        #endregion

        #region 检查分类配件逻辑
        private static class CheckCategoryAccessoryProcessor
        {
            /// <summary>
            /// 检查品牌实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckCategoryAccessoryInfo(CategoryAccessory entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "CategoryAccessoryIsNull"));
                }
                if (entity.CategoryInfo == null || entity.CategoryInfo.SysNo == null || entity.CategoryInfo.SysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "CategorySysNoIsNull"));
                }
                if (entity.Accessory == null || entity.Accessory.AccessoryName == null || string.IsNullOrEmpty(entity.Accessory.AccessoryName.Content))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "AccessoryNameIsNull"));
                }
                var acessoriesProcessor = ObjectFactory<AccessoryProcessor>.Instance;
                var dataList = acessoriesProcessor.GetAccessoryInfoByName(entity.Accessory.AccessoryName.Content);
                if (dataList == null || dataList.Count <= 0 || dataList[0].SysNo == null || dataList[0].SysNo.Value <= 0)
                {
                    //throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "CategoryAccessoryIsNull"));
                    AccessoryInfo info = acessoriesProcessor.CreateAccessory(new AccessoryInfo() { AccessoryName = new LanguageContent(entity.Accessory.AccessoryName.Content), SysNo = 0 });
                    if (dataList==null)dataList=new List<AccessoryInfo>();
                    dataList.Add(info);
                }
                entity.Accessory.SysNo = dataList[0].SysNo;
                var categoryAccessoriesDA = ObjectFactory<ICategoryAccessoriesDA>.Instance;
                bool isExists;
                if (entity.SysNo == null || entity.SysNo.Value <= 0)
                {

                    isExists = categoryAccessoriesDA.IsExistCategoryAccessories(entity.Accessory.SysNo.Value,
                                                                             entity.CategoryInfo.SysNo.Value);
                }
                else
                {
                    isExists = categoryAccessoriesDA.IsExistCategoryAccessories(entity.Accessory.SysNo.Value,
                                                                              entity.CategoryInfo.SysNo.Value, entity.SysNo.Value);
                }
                if (isExists)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "ExistsCategoryAccessory"));
                }
            }

            /// <summary>
            /// 检查分类属性编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckCategoryAccessorySysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryAccessory", "CategoryAccessorySysNOIsNull"));
                }
            }

        }
        #endregion

    }
}
