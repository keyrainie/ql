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

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAccessoriesQueryConditionPreViewQueryVM : ModelBase
    {

        public int ConditionValueSysNo1 { get; set; }
        public int ConditionValueSysNo2 { get; set; }
        public int ConditionValueSysNo3 { get; set; }
        public int ConditionValueSysNo4 { get; set; }

        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        public string ProductID { get; set; }

        public int? ProductSysNo { get; set; }
        
        public int MasterSysNo { get; set; }
    }
}
