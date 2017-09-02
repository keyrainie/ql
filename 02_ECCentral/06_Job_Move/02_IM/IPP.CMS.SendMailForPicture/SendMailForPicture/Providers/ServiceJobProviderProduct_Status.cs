using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.SendMailForPicture.Biz;
using IPP.ContentMgmt.SendMailForPicture.Providers;

namespace IPP.ContentMgmt.SendMailForPicture.Providers
{
    public class ServiceJobProviderProduct_Status: ServiceJobProvider
    {
        public override void PostData()
        {
            Product_StatusBP.CheckProduct_StatusItem();
        }
    }
}
