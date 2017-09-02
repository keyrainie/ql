using ECCentral.BizEntity.Inventory;
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

namespace ECCentral.Portal.UI.Inventory.Models.Inventory
{
    public class ExperienceHallAllocateOrderQueryVM : ModelBase
    {

        public string _SysNoStr;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.MaxLength,9)]
        public string SysNoStr
        {
            get { return _SysNoStr; }
            set
            {
                this.SetValue("SysNoStr", ref _SysNoStr, value);
            }
        }

         private int? _SysNo;
         public int? SysNo
         {
             get
             {
                 int tnp = 0;
                 if (int.TryParse(SysNoStr, out tnp))
                 {
                     return tnp;
                 }
                 return null;
             }
             set{
                 _SysNo = value;
                 this.SetValue("SysNo", ref _SysNo, value);
             }
         }

         private int? _ProductSysNo;
         public int? ProductSysNo
         {
             get
             { return _ProductSysNo; }
             set
             {
                 this.SetValue("ProductSysNo", ref _ProductSysNo, value);
             }
         }

         private string productID;
         public string ProductID
         {
             get
             {
                 return productID;
             }
             set
             {
                 SetValue("ProductID", ref productID, value);
             }
         }

        public int? ProductName{get;set;}

        public DateTime? InDate { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        
        //调拨性质AllocateType
        public AllocateType? AllocateType { get; set; }

        private List<KeyValuePair<AllocateType?, string>> _AllocateTypeList;
        public List<KeyValuePair<AllocateType?, string>> AllocateTypeList
        {
            get
            {
                _AllocateTypeList = _AllocateTypeList ?? EnumConverter.GetKeyValuePairs<AllocateType>(EnumConverter.EnumAppendItemType.All);
                return _AllocateTypeList;
            }
        }

       // 状态 ExperienceHallStatus
        public ExperienceHallStatus? ExperienceHallStatus { get; set; }

        private List<KeyValuePair<ExperienceHallStatus?, string>> _ExperienceHallStatusList;
        public List<KeyValuePair<ExperienceHallStatus?, string>> ExperienceHallStatusList
        {
            get
            {
                _ExperienceHallStatusList = _ExperienceHallStatusList ?? EnumConverter.GetKeyValuePairs<ExperienceHallStatus>(EnumConverter.EnumAppendItemType.All);
                return _ExperienceHallStatusList;
            }
        }
    }
}
