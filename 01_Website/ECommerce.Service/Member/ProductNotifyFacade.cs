using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Member;
using ECommerce.Entity;
using ECommerce.Entity.Member;

namespace ECommerce.Facade.Member
{
    public class ProductNotifyFacade
    {
        public static QueryResult<ProductNotifyInfo> QueryProductNotify(ProduceNotifiyQueryFilter filter)
        {
            return ProductNotifyDA.QueryProductNotify(filter);
        }

        public static ProductNotifyInfo GetProductNotify(string email, int productSysNo)
        {
            return ProductNotifyDA.GetProductNotify(email, productSysNo);
        }

        public static int CreateProductNotify(ProductNotifyInfo info)
        {
            return ProductNotifyDA.CreateProductNotify(info);
        }

        public static void UpdateProductNotify(int sysNo, int customerSysNo)
        {
            ProductNotifyDA.UpdateProductNotify(sysNo, customerSysNo);
        }

        public static void DeleteProductNotify(int sysNo, int customerSysNo)
        {
            ProductNotifyDA.DeleteProductNotify(sysNo, customerSysNo);
        }

        public static void ClearProductNotify(int customerSysNo)
        {
            ProductNotifyDA.ClearProductNotify(customerSysNo);
        }
    }
}
