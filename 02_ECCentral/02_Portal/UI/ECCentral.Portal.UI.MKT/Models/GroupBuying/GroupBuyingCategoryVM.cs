using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingCategoryVM:ModelBase
    {
        public GroupBuyingCategoryVM()
        {
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<GroupBuyingCategoryType>(EnumConverter.EnumAppendItemType.None);
            this.CategoryType = ECCentral.BizEntity.MKT.GroupBuyingCategoryType.Physical;

            this.StatusList = EnumConverter.GetKeyValuePairs<GroupBuyingCategoryStatus>(EnumConverter.EnumAppendItemType.None);
            this.Status = GroupBuyingCategoryStatus.Valid;
        }

        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private string name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetValue("Name", ref name, value);
            }
        }

        private bool isHotKey;        
        public bool IsHotKey
        {
            get
            {
                return isHotKey;
            }
            set
            {
                SetValue("IsHotKey", ref isHotKey, value);
            }
        }

        private GroupBuyingCategoryStatus? status;
        public GroupBuyingCategoryStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status",ref status,value);
            }
        }

        private GroupBuyingCategoryType? categoryType;
        public GroupBuyingCategoryType? CategoryType
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

        public List<KeyValuePair<GroupBuyingCategoryType?, string>> CategoryTypeList { get; set; }
        public List<KeyValuePair<GroupBuyingCategoryStatus?, string>> StatusList { get; set; }
    }
}
