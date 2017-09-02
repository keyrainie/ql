using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;
using System.Data;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductAccessoriesProcessor))]
    public class ProductAccessoriesProcessor
    {
        private readonly IProductAccessoriesDA productAccessoriesDA = ObjectFactory<IProductAccessoriesDA>.Instance;

        #region "配件查询操作" 

      
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        public void CreateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            //检测查询功能是否已存在
            if (!productAccessoriesDA.IsExistsAccessoriesQuery(info))
            {
                productAccessoriesDA.CreateAccessoriesQueryMaster(info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateAccessoriesQueryMasterResult"));
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public void UpdateAccessoriesQueryMaster(ProductAccessoriesInfo info)
        {
            //检测查询功能是否已存在
            if (!productAccessoriesDA.IsExistsAccessoriesQuery(info))
            {
                productAccessoriesDA.UpdateAccessoriesQueryMaster(info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateAccessoriesQueryMasterResult"));
            }
        }

        #endregion
        #region "查询条件操作"
          /// <summary>
        /// 创建查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void CreateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            if (!productAccessoriesDA.IsExistsAccessoriesQueryCondition(Info))
            {
                productAccessoriesDA.CreateAccessoriesQueryCondition(Info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateAccessoriesQueryConditionResult"));
            }

        }

        /// <summary>
        /// 修改查询条件
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateAccessoriesQueryCondition(ProductAccessoriesQueryConditionInfo Info)
        {
            if (!productAccessoriesDA.IsExistsAccessoriesQueryCondition(Info))
            {
                productAccessoriesDA.UpdateAccessoriesQueryCondition(Info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateAccessoriesQueryConditionResult"));
            }
        }


        /// <summary>
        /// 删除查询条件
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteAccessoriesQueryCondition(int SysNo)
        {
            productAccessoriesDA.DeleteAccessoriesQueryCondition(SysNo);
        }

        #endregion


        #region "选项值操作"
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="Info"></param>
        public void CreateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            if (!productAccessoriesDA.IsExistsAccessoriesQueryConditionValue(Info))
            {
                productAccessoriesDA.CreateProductAccessoriesQueryConditionValue(Info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateProductAccessoriesQueryConditionValueResult"));
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateProductAccessoriesQueryConditionValue(ProductAccessoriesQueryConditionValueInfo Info)
        {
            if (!productAccessoriesDA.IsExistsAccessoriesQueryConditionValue(Info))
            {
                productAccessoriesDA.UpdateProductAccessoriesQueryConditionValue(Info);
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductAccessoriesQueryConditionValueResult"));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductAccessoriesQueryConditionValue(int SysNo)
        {
            productAccessoriesDA.DeleteProductAccessoriesQueryConditionValue(SysNo);
        }
        #endregion

        #region "查询效果操作"
         /// <summary>
        /// 删除bing
        /// </summary>
        /// <param name="info"></param>
        public void DeleteAccessoriesQueryConditionBind(List<ProductAccessoriesQueryConditionPreViewInfo> info)
        {
            foreach (var item in info)
            {
                productAccessoriesDA.DeleteAccessoriesQueryConditionBind(item);
            }
           
        }
        #endregion

    }
}
