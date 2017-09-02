using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductStepPriceQueryFilterVM : ModelBase
    {
        public ProductStepPriceQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
        }
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? _vendorSysNo;
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set { SetValue("VendorSysNo", ref _vendorSysNo, value); }
        }



        /// <summary>
        /// 商品编号
        /// </summary>
        public int? _productSysNo;
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }



    }
}
