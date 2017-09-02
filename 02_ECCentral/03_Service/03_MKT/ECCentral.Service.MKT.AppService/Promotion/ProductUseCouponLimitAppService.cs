using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using System.Transactions;

namespace ECCentral.Service.MKT.AppService
{
     [VersionExport(typeof(ProductUseCouponLimitAppService))]
   public class ProductUseCouponLimitAppService
    {
         private ProductUseCouponLimitProcessor _Processor = 
             ObjectFactory<ProductUseCouponLimitProcessor>.Instance;

         /// <summary>
         /// 创建
         /// </summary>
         /// <param name="list"></param>
        public void CreateProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            _Processor.CreateProductUseCouponLimit(list);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="list"></param>
        public void ModifyProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            _Processor.ModifyProductUseCouponLimit(list);
        }

        #region Job行为
        public virtual List<ProductJobLimitProductInfo> GetLimitProductList(string datacommandname)
        {
            return _Processor.GetLimitProductList(datacommandname);
        }

        public virtual String DeleteProductUseCouponLimit(List<ProductJobLimitProductInfo> Products)
        {
            StringBuilder result = new StringBuilder();
            foreach (ProductJobLimitProductInfo item in Products)
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        _Processor.DeleteProductUseCouponLimit(item.ProductSysNo);
                        result.AppendFormat(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_DelLimitProduct"), item.ProductSysNo);
                        ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result.AppendFormat(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_DelProductFaile")
                        , DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), item.ProductSysNo);
                    result.AppendFormat(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_DelProductFaileAndDetaile")
                        , DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
                }
            }
            return result.ToString();
        }

        public ProductJobLimitProductInfo GetLimitProductByProductSysNo(int productSysNo)
        {
            return _Processor.GetLimitProductByProductSysNo(productSysNo);
        }

        public virtual void CreateLimitProduct(ProductJobLimitProductInfo entity)
        {
            _Processor.CreateLimitProduct(entity);
        }
        #endregion
    }
}
