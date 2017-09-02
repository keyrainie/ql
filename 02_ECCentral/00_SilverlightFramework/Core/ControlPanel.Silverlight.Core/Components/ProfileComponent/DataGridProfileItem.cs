using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class DataGridProfileItem
    {
        public AuthUser Owner { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Owner.DisplayName))
                {
                    return this.Owner.LoginName;
                }
                return this.Owner.DisplayName;
            }
        }

        public string DataGridProfileXml { get; set; }
    }
}
