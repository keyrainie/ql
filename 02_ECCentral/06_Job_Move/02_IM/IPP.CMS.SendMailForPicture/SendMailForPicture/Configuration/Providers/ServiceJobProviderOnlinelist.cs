using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.ContentMgmt.Product_Status;
using IPP.ContentMgmt.Product_Status.Biz;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.Providers
{
    public class ServiceJobProviderOnlinelist : ServiceJobProvider
    {
        public override void PostData()
        {
            Product_StatusBP.CheckProduct_StatusItem();
        }
    }
}
