using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using ProductSaleInfoInterface;


namespace ProductSaleInfoMain
{
    class CustomeReportFactory
    {
        public static List<IProductSaleInfo> GetHandlers()
        {
            List<IProductSaleInfo> handlers = new List<IProductSaleInfo>();
            Assembly ab = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductSaleInfoBiz.dll"));
            object obj = ab.CreateInstance("ProductSaleInfoBiz.Biz.ProductSaleInfoBiz");
            IProductSaleInfo handler = obj as IProductSaleInfo;
            handlers.Add(handler);
            return handlers;
        }
    }
}
