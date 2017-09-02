using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ContentMgmt.SellerPortalProductDescAndImage.Biz;
using System.Threading;

namespace SellerPortalProductDescAndImage
{
    public class SellerPortalProductDescAndImageProgram
    {
        static void Main(string[] args)
        {
            SellerPortalProductDescAndImageBP.BizLogFile = "Log\\biz.log";
            SellerPortalProductDescAndImageBP.SellerPortalProductRequestForNewProduct();
        }
    }
}
