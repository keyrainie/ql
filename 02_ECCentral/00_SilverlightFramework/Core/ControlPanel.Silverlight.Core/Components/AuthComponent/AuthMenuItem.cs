using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class AuthMenuItem : ModelBase
    {
        public AuthMenuItem Parent { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public AuthMenuItemType Type { get; set; }
        public string Description { get; set; }
        public string IconStyle { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsLongTimeLoadingLink { get; set; }
        public ObservableCollection<AuthMenuItem> Items { get; set; }
        public string AuthKey { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                AuthMenuItem other = (AuthMenuItem)obj;
                return this.Id == other.Id;
            }
        }

        public string IconPath
        {
            get
            {
                return String.Format("/Images/menuMaintain/tree_icon{0}.png", this.Type == AuthMenuItemType.Category ? "C" : "P");
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum AuthMenuItemType
    {
        Category = 0,
        Page = 1,
        Link=2
    }
}
