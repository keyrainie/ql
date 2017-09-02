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
using System.Collections.Generic;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public interface IListControl<T>
    {
        void BindData(object filter);

        List<T> GetSelectedSysNoList();
    }
}
