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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface ICompanyManager : IComponent
    {
        void GetAllCompanies(int userSysNo ,Action callback);
    }
}
