using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.Product_Status.Biz;
using IPP.ContentMgmt.Product_Status.Providers;

namespace IPP.ContentMgmt.Product_Status.Providers
{
    public class ServiceJobProviderProduct_Status: ServiceJobProvider
    {
        public override void PostData()
        {
            Product_StatusBP.CheckProduct_StatusItem();
        }
    }
}
