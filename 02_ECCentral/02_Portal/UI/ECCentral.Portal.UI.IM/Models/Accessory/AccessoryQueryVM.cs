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
    public class AccessoryQueryVM : ModelBase
    {

        //public List<KeyValuePair<ValidStatus?, String>> AccessoryStatusList { get; set; }

        public AccessoryQueryVM()
        {
            //AccessoryStatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public string AccessoryID { get; set; }

        public string AccessoryName { get; set; }


    }
}
