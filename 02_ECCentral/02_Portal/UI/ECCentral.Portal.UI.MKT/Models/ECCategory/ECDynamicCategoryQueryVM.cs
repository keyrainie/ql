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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ECDynamicCategoryQueryVM : ModelBase
    {
        public ECDynamicCategoryQueryVM()
        {
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<DynamicCategoryType>(EnumConverter.EnumAppendItemType.None);
            this.IsOnlyShowActive = true;
            this.CategoryType = DynamicCategoryType.Standard;
        }

        public DynamicCategoryStatus? Status
        {
            get
            {
                if (IsOnlyShowActive)
                    return DynamicCategoryStatus.Active;
                return null;
            }
        }

        private DynamicCategoryType categoryType;
        public DynamicCategoryType CategoryType
        {
            get
            {
                return categoryType;
            }
            set
            {
                SetValue("CategoryType", ref categoryType, value);
            }
        }

        private bool isOnlyShowActive;
        public bool IsOnlyShowActive
        {
            get
            {
                return isOnlyShowActive;
            }
            set
            {
                SetValue("IsOnlyShowActive", ref isOnlyShowActive, value);
            }
        }

        public List<KeyValuePair<DynamicCategoryType?, string>> CategoryTypeList { get; set; }
    }
}
