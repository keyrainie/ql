//************************************************************************
// 用户名				泰隆优选
// 系统名				分类配件管理
// 子系统名		        分类配件管理业务逻辑实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;


namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryExtendWarrantyProcessor))]
    public class CategoryExtendWarrantyProcessor
    {

        private readonly ICategoryExtendWarrantyDA _CategoryExtendWarrantyDA = ObjectFactory<ICategoryExtendWarrantyDA>.Instance;

        #region 分类延保业务方法
        /// <summary>
        /// 根据SysNo获取分类延保信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarranty GetCategoryExtendWarrantyBySysNo(int sysNo)
        {
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantySysNo(sysNo);
            return _CategoryExtendWarrantyDA.GetCategoryExtendWarrantyBySysNo(sysNo);;
        }

        /// <summary>
        /// 创建分类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarranty CreatetCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyInfo(entity);
            return _CategoryExtendWarrantyDA.CreateCategoryExtendWarranty(entity);
        }

        /// <summary>
        /// 修改分类延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarranty UpdateCategoryExtendWarranty(CategoryExtendWarranty entity)
        {
            if (entity != null)
            {
                CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantySysNo(entity.SysNo);
            }
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyInfo(entity);
            return _CategoryExtendWarrantyDA.UpdateCategoryExtendWarranty(entity);
        }


        #endregion


        #region 分类延保排出品牌业务方法
        /// <summary>
        /// 根据SysNo获取分类延保排出品牌信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarrantyDisuseBrand GetCategoryExtendWarrantyDisuseBrandBySysNo(int sysNo)
        {
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyDisuseBrandSysNo(sysNo);
            return _CategoryExtendWarrantyDA.GetCategoryExtendWarrantyDisuseBrandBySysNo(sysNo); ;
        }

        /// <summary>
        /// 创建分类延保排出品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarrantyDisuseBrand CreatetCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyDisuseBrandInfo(entity);
            return _CategoryExtendWarrantyDA.CreateCategoryExtendWarrantyDisuseBrand(entity);
        }

        /// <summary>
        /// 修改分类延保排出品牌信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CategoryExtendWarrantyDisuseBrand UpdateCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrand entity)
        {
            if (entity != null)
            {
                CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyDisuseBrandSysNo(entity.SysNo);
            }
            CheckCategoryExtendWarrantyProcessor.CheckCategoryExtendWarrantyDisuseBrandInfo(entity);
            return _CategoryExtendWarrantyDA.UpdateCategoryExtendWarrantyDisuseBrand(entity);
        }


        #endregion

        #region 检查分类延保逻辑
        private static class CheckCategoryExtendWarrantyProcessor
        {
            private static readonly ICategoryExtendWarrantyDA _CategoryExtendWarrantyDA = ObjectFactory<ICategoryExtendWarrantyDA>.Instance;

            /// <summary>
            /// 检查分类延保实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckCategoryExtendWarrantyInfo(CategoryExtendWarranty entity)
            {
                if (entity.MinUnitPrice >= entity.MaxUnitPrice)
                {
                    //价格上限必须大于价格下限！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyUnitPriceError"));
                }

                int result = _CategoryExtendWarrantyDA.CheckCategoryExtendWarranty(entity);

                if (result == -1)
                {
                    //该类别延保已存在！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyIsExists"));
                }

                if (result == -2)
                {
                    //统一价格区间的长期延保价格不能小于短期延保价格！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyPriceCheck"));
                }

                if (result == -3)
                {
                    //同一品牌的延保区间不能重复！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyRangeExists"));
                }

                if (result == -4)
                {
                    //该品牌不存在！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyBrandNoExists"));
                }


            }

            /// <summary>
            /// 检查分类延保排出品牌实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckCategoryExtendWarrantyDisuseBrandInfo(CategoryExtendWarrantyDisuseBrand entity)
            {
                int result = _CategoryExtendWarrantyDA.CheckCategoryExtendWarrantyDisuseBrand(entity);

                if (result == -1)
                {
                    //该不参加延保品牌已存在！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyDisuseBrandIsExists"));
                }

                if (result == -4)
                {
                    //该品牌不存在！
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantyBrandNoExists"));
                }
            }

            /// <summary>
            /// 检查分类延保编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckCategoryExtendWarrantySysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantySysNOIsNull"));
                }
            }

            /// <summary>
            /// 检查分类延保编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckCategoryExtendWarrantyDisuseBrandSysNo(int? sysNo)
            {

                if (sysNo == null || sysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.CategoryExtendWarranty", "CategoryExtendWarrantySysNOIsNull"));
                }
            }

        }
        #endregion

    }
}
