using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class KeystoneAttribute : ModelBase
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string ApplicationId { get; set; }
    }

    public class RoleAttribute : KeystoneAttribute
    {
        public string RoleName { get; set; }
    }

}
