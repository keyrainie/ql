using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductAccessoriesAppService))]
    public class ProductAccessoriesAppService
    {

        #region "配件查询操作"
        
   
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="info"></param>
        public void CreateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.CreateAccessoriesQueryMaster(info);
        }
         /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public void UpdateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.UpdateAccessoriesQueryMaster(info);
        }

        #endregion

        #region "查询条件操作"
        
       
        /// <summary>
        /// 创建查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.CreateAccessoriesQueryCondition(Info);
        }

        /// <summary>
        /// 修改查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.UpdateAccessoriesQueryCondition(Info);
        }
        /// <summary>
        /// 删除查询条件
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteAccessoriesQueryCondition(int SysNo)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.DeleteAccessoriesQueryCondition(SysNo);
        }
        #endregion

        #region "选项值操作"
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="Info"></param>
        public void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.CreateProductAccessoriesQueryConditionValue(Info);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.UpdateProductAccessoriesQueryConditionValue(Info);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductAccessoriesQueryConditionValue(int SysNo)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.DeleteProductAccessoriesQueryConditionValue(SysNo);
        }
        #endregion
        #region "查询效果操作"
        /// <summary>
        /// 删除bing
        /// </summary>
        /// <param name="info"></param>
        public void DeleteAccessoriesQueryConditionBind(List<ProductAccessoriesQueryConditionPreViewInfo> info)
        {
            ObjectFactory<ProductAccessoriesProcessor>.Instance.DeleteAccessoriesQueryConditionBind(info);
        }
        #endregion

    }
}
