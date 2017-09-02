using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface IConfiguration : IComponent
    {
        void LoadConfig(Action callback);

        string GetConfigValue(string domainName, string key);

        void GetECCentralServiceURL(Action callback);
    }

}
