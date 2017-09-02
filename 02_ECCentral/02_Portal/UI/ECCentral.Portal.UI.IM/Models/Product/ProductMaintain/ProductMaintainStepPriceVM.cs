using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainStepPriceVM : ModelBase
    {
        public ProductMaintainStepPriceVM()
        {
            AddEntity = new ProductStepPriceInfoVM();
            QueryResultList = new List<ProductStepPriceInfoVM>();
    }
        public int ProductSysNo;
        public ProductStepPriceInfoVM AddEntity { get; set; }
        public List<ProductStepPriceInfoVM> QueryResultList { get; set; }
    }
}
