using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Invoice;
using ECommerce.Entity.Invoice;

namespace ECommerce.Service.Invoice
{
    public class NetPayService
    {
        public static NetPayInfo GetValidNetPayBySOSysNo(int soSysNo)
        {
            return NetPayDA.GetValidBySOSysNo(soSysNo);
        }
    }
}
