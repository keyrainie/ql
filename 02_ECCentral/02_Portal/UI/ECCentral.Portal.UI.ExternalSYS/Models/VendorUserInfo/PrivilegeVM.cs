﻿using System;
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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class PrivilegeVM : ModelBase
    {
        public int SysNo { get; set; }

        public int? ParentSysNo { get; set; }

        public string PrivilegeName { get; set; }

        public int? OrderNo { get; set; }

        public string Memo { get; set; }
    }
}
