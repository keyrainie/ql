using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductResourcesQueryVM : ModelBase
    {
        public IList<string> CommonSKUNumberList { get; set; }
    }


}
