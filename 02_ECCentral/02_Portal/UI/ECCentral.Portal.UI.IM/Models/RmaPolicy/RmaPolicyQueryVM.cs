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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models
{
    public class RmaPolicyQueryVM : ModelBase
    {

        public RmaPolicyQueryVM()
        {
            RmaPolicyTypeList = EnumConverter.GetKeyValuePairs<RmaPolicyType>(EnumConverter.EnumAppendItemType.All);
            RmaPolicyStatusList = EnumConverter.GetKeyValuePairs<RmaPolicyStatus>(EnumConverter.EnumAppendItemType.All);
        }
        private string sysNO;
           [Validate(ValidateType.Interger)]
        public string SysNo {
            get { return sysNO; }
            set { SetValue("SysNo", ref sysNO, value); }
        }
        private string createUserName;
        public string CreateUserName
        {
            get { return createUserName; }
            set { SetValue("CreateUserName", ref createUserName, value); }
        }
       private RmaPolicyType? rmaType;
    public RmaPolicyType? RmaType 
        {
            get { return rmaType; }
            set { SetValue("RmaType", ref rmaType, value); }
        }
        private RmaPolicyStatus? status;
        public RmaPolicyStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }

        public List<KeyValuePair<RmaPolicyType?, string>> RmaPolicyTypeList { get; set; }

        public List<KeyValuePair<RmaPolicyStatus?, string>> RmaPolicyStatusList { get; set; }
    }
}
