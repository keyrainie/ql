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
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.Models
{
    /// <summary>
    /// 炒货订单的VM
    /// </summary>
    public class CCSetVM : ModelBase
    {
        public int? SysNo { get; set; }
        public string Title { get; set; }

        public string Params
        {
            get {
                if (Param1.HasValue && Param2.HasValue)
                    return string.Format("{0}|{1}", Param1, Param2);
                else
                    return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Param1 = int.Parse(value.Split('|')[0]);
                    Param2 = int.Parse(value.Split('|')[1]);
                }
            }
        }
        private int? _Param1;

        public int? Param1
        {
            get { return _Param1; }
            set { base.SetValue("Param1", ref _Param1, value); }
        }
        private int? _Param2;

        public int? Param2
        {
            get { return _Param2; }
            set { base.SetValue("Param2", ref _Param2, value); }
        }
        private bool? _Status;

        public bool? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }
        public bool Enable { get; set; }

        public Visibility Parame1Visibility
        {
            get
            {
                if (Param1 != null)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility Parame2Visibility
        {
            get
            {
                if (Param2 != null)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public string Description { get; set; }

        public string LastEditUserName { get; set; }
        public DateTime LastEditDate { get; set; }
    }
}
