using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Member;
using ECommerce.Entity;
using ECommerce.Entity.Member;

namespace ECommerce.Facade.Member
{
    public class ProductPriceNotifyFacade
    {
        public static QueryResult<ProductPriceNotifyInfo> QueryProductPriceNotify(ProducePriceNotifiyQueryFilter filter)
        {
            return ProductPriceNotifyDA.QueryProductPriceNotify(filter);
        }

        public static ProductPriceNotifyInfo GetProductPriceNotify(int customerSysno, int productSysNo)
        {
            return ProductPriceNotifyDA.GetProductPriceNotify(customerSysno, productSysNo);
        }

        public static int CreateProductPriceNotify(ProductPriceNotifyInfo entity)
        {
            return ProductPriceNotifyDA.CreateProductPriceNotify(entity);
        }

        public static void UpdateProductPriceNotify(ProductPriceNotifyInfo entity)
        {
            ProductPriceNotifyDA.UpdateProductPriceNotify(entity);
        }

        public static void CancelProductPriceNotify(int sysNo, int customerSysno)
        {
            ProductPriceNotifyDA.CancelProductPriceNotify(sysNo, customerSysno);
        }

        public static void DeleteProductPriceNotify(int sysNo, int customerSysno)
        {
            ProductPriceNotifyDA.DeleteProductPriceNotify(sysNo, customerSysno);
        }

        public static void ClearProductPriceNotify(int customerSysno)
        {
            ProductPriceNotifyDA.ClearProductPriceNotify(customerSysno);
        }
    }
}
