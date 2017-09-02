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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupQueryVM : ModelBase
    {
        public ProductGroupQueryVM()
        {
            //ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            //ValidStatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
        }

        //public List<KeyValuePair<ValidStatus?, string>> ValidStatusList { get; set; }        

        //public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }

        public int? C1SysNo
        {
            get;
            set;
        }

        public int? C2SysNo
        {
            get;
            set;
        }

        public int? C3SysNo
        {
            get;
            set;
        }        

        public string BrandName
        {
            get;
            set;
        }

        public string ProductGroupName
        {
            get;
            set;
        }

        public int BrandSysNo { get; set; }

        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }

    }
}
