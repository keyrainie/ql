using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductRMAPolicyProcessor))]
   public class ProductRMAPolicyProcessor
    {
        /// <summary>
        /// 根据商品编号得到商品退换货信息
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <returns></returns>
       public List<ProductRMAPolicyInfo> GetProductRMAPolicyList(List<int> productSysNos)
       {
           if (productSysNos == null || productSysNos.Count == 0)
           {
               throw new BizException(ResouceManager.GetMessageString("IM.Product", "GetProductRMAPolicyListResult"));
           }
           string SysNos = string.Empty;
           foreach (var item in productSysNos)
           {
               SysNos = SysNos + "," + item;
           }
           SysNos = SysNos.Substring(1, SysNos.Length - 1);
           return ObjectFactory<IProductRMAPolicyDA>.Instance.GetProductRMAPolicyList(SysNos);
        }

       public ProductRMAPolicyInfo GetProductRMAPolicyByProductSysNo(int? productSysNo)
       {
           return ObjectFactory<IProductRMAPolicyDA>.Instance.GetProductRMAPolicyByProductSysNo(productSysNo);
       }

        
        public void CreateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            ObjectFactory<IProductRMAPolicyDA>.Instance.CreateProductRMAPolicy(info);
        }

      
         public void UpdateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            if (info.WarrantyDay == 0)
            {
                info.WarrantyDay = null;
            }
            ProductInfo product = ObjectFactory<ProductProcessor>.Instance.GetProductInfo((int)info.ProductSysNo);
            ProductBrandWarrantyInfo warrantyentity = ObjectFactory<IProductBrandWarrantyDA>.Instance.GetBrandWarranty((int)product.ProductBasicInfo.ProductCategoryInfo.SysNo,(int)product.ProductBasicInfo.ProductBrandInfo.SysNo);
            if (info.IsBrandWarranty.ToUpper()=="Y"&&warrantyentity == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "UpdateProductRMAPolicyResult"));
            }
           ObjectFactory<IProductRMAPolicyDA>.Instance.UpdateProductRMAPolicy(info);
        }

    }
}
