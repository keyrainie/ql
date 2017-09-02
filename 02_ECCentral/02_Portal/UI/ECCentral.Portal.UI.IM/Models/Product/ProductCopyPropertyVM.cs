using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductCopyPropertyVM : ModelBase
    {
        public List<KeyValuePair<bool, string>> CanOverriteList { get; set; }

        public ProductCopyPropertyVM()
        {
            var statusList = new List<KeyValuePair<bool, string>>
                            {
                                new KeyValuePair<bool, string>(true, ResProductCopy.Yes),
                                new KeyValuePair<bool, string>(false, ResProductCopy.No)
                            };

            CanOverriteList = statusList;
        }

        private bool _canOverrite;

        public bool CanOverrite
        {
            get { return _canOverrite; }
            set { SetValue("CanOverrite", ref _canOverrite, value); }
        }

        private string _sourceProductID;
        public string SourceProductID
        {
            get { return _sourceProductID; }
            set { SetValue("SourceProductID", ref _sourceProductID, value); }
        }

        private string _targetProductID;
        public string TargetProductID
        {
            get { return _targetProductID; }
            set { SetValue("TargetProductID", ref _targetProductID, value); }
        }

        private int? _sourceProductSysNo;

        public int? SourceProductSysNo
        {
            get { return _sourceProductSysNo; }
            set { SetValue("SourceProductSysNo", ref _sourceProductSysNo, value); }
        }

        private int? _targetProductSysNo;
        public int? TargetProductSysNo
        {
            get { return _targetProductSysNo; }
            set { SetValue("TargetProductSysNo", ref _targetProductSysNo, value); }
        }
    }
}
