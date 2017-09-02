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
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class RoleMgmtVM : ModelBase
    {

        private List<RoleMgmtSearchResultVM> result;
        public List<RoleMgmtSearchResultVM> Result
        {
            get { return result; }
            set {
                SetValue<List<RoleMgmtSearchResultVM>>("Result", ref result, value);
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set { SetValue<int>("TotalCount", ref totalCount, value); }
        }

    }

    public class RoleMgmtSearchResultVM : ModelBase
    {

        public int? SysNo { get; set; }

        public string RoleName { get; set; }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        public string EditUser { get; set; }

        public DateTime? EditDate { get; set; }

        public ValidStatus Status { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get{return isChecked;}
            set{
                SetValue<bool>("IsChecked", ref isChecked, value);
            }
        }
 
    }
}
