
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PMQueryVM : ModelBase
    {

        public string PMID { get; set; }

        public string PMName { get; set; }

        public string Status { get; set; }

        public string PMGroupName { get; set; }

        public List<KeyValuePair<int, string>> PMStatusList { get; set; }

        public PMQueryVM()
        {
            List<KeyValuePair<int, string>> statusList = new List<KeyValuePair<int, string>>();

            statusList.Add(new KeyValuePair<int, string>(-999, ResCategoryKPIMaintain.SelectTextAll));
            statusList.Add(new KeyValuePair<int, string>(0, ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<int, string>(-1, ResCategoryKPIMaintain.SelectTextInvalid));

            this.PMStatusList = statusList;
        }
    }

    public class PMManufacturerQueryVM : ModelBase
    {
        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string VendorName { get; set; }
    }
}
