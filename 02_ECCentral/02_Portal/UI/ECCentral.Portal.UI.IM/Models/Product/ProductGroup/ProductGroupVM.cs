using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupVM : ModelBase
    {
        public String ProductGroupName { get; set; }

        /// <summary>
        /// 分组属性设置
        /// </summary>
        public IList<ProductGroupSettingVM> ProductGroupSettings { get; set; }

        public List<ProductVM> ProductList { get; set; }

        public int? SysNo { get; set; }

        public string ProductModel { get; set; }

        public string BrandName { get; set; }

        public string C3Name { get; set; }

        public string InUser { get; set; }

        public DateTime InDate { get; set; }

        public string EditUser { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
