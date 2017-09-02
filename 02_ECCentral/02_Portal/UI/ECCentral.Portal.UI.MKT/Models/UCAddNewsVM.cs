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
using ECCentral.Portal.Basic.Utilities;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    /// <summary>
    /// 无用
    /// </summary>
    public class UCAddNewsVM
    {
        public UCAddNewsVM() {
            this.UserGradeList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Customer.CustomerRank>(EnumConverter.EnumAppendItemType.All);
            this.YNStatusList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public List<KeyValuePair<ECCentral.BizEntity.Customer.CustomerRank?, string>> UserGradeList { get; set; }
        public List<KeyValuePair<YNStatus?, string>> YNStatusList { get; set; }
    }
}
