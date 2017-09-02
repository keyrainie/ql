using ECCentral.BizEntity.IM.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

using ECCentral.Service.IM.SqlDataAccess;namespace ECCentral.Service.IM.BizProcessor
{
    public class ProductStepPriceProcessor
    {
        public static int CreateProductStepPrice(ProductStepPriceInfo entity)
        {
            if (entity.SysNo != null)
            {
                ProductStepPriceDA.UpdateProductStepPrice(entity);
            }
            else
            {
                ProductStepPriceDA.CreateProductStepPrice(entity);
            }
            
            return 1;
        }

        public static DataTable GetProductStepPrice(int? vendorSysNo, int? productSysno, int? pageIndex, int? pageSize, out int totalCount)
        {
            DataTable dt = ProductStepPriceDA.GetProductStepPrice(vendorSysNo, productSysno, pageIndex, pageSize, out totalCount);
            return dt;
        }

        public static int DeleteProductStepPrice(List<int> sysNos)
        {
            ProductStepPriceDA.DeleteProductStepPrice(sysNos);
            return 1;
        }

        public static List<ProductStepPriceInfo> GetProductStepPricebyProductSysNo(int productSysNo)
        {
            return ProductStepPriceDA.GetProductStepPricebyProductSysNo(productSysNo);

        }
    }
}
