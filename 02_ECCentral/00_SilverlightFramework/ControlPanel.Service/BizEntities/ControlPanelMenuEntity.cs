using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public partial class ControlPanelLocalizedMenuEntity
    {
        public string ApplicationId
        {
            get { return this.ApplicationId2.GetValueByTrim(); }
        }

        public MenuType Type
        {
            get { return this.TypeCode.ToMenuType().Value; }
        }

        public bool IsDisplay
        {
            get { return this.IsDisplayCode.ToBoolean().Value; }
        }
    }

    public partial class ControlPanelMenuEntity
    {
        public string ApplicationId
        {
            get { return this.ApplicationId2.GetValueByTrim(); }
            set { this.ApplicationId2 = value.GetValueByTrim(); }
        }

        public MenuType? Type
        {
            get { return this.TypeCode.ToMenuType(); }
            set { this.TypeCode = value.ToCharValue(); }
        }

        public bool? IsDisplay
        {
            get { return this.IsDisplayCode.ToBoolean(); }
            set { this.IsDisplayCode = value.ToCharValue(); }
        }

        public MenuStatus? Status
        {
            get { return this.StatusCode.ToMenuStatus(); }
            set { this.StatusCode = value.ToCharValue(); }
        }

        public string LanguageCode
        {
            get { return this.LanguageCode2.GetValueByTrim(); }
            set { this.LanguageCode2 = value.GetValueByTrim(); }
        }

        public List<ControlPanelMenuLocalizedRes> LocalizedMenuEntities { get; set; }

    }

    public class ControlPanelLocalizedMenuQueryEntity : QueryEntityBase
    {
        public string LanguageCode { get; set; }

        public string[] ApplicationIds { get; set; }

        public string StatusCode { get; set; }
    }

    public class ControlPanelMenuQueryEntity : QueryEntityBase
    {
        public Guid? MenuId { get; set; }

        public string LinkPath { get; set; }

        public string[] ApplicationIds { get; set; }
    }


    public enum MenuType
    {
        Category,
        Page,
        Link,
    }

    public enum MenuStatus
    {
        Active,
        Inactive
    }

    public static class ControlPanelMenuEntityExtension
    {
        public static string GetValueByTrim(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Trim();
        }

        public static MenuType? ToMenuType(this string value)
        {
            if (value == null)
            {
                return null;
            };

            string code = value.Trim().ToUpper();

            switch (code)
            {
                case "C": return MenuType.Category;
                case "P": return MenuType.Page;
                case "L": return MenuType.Link;
                default: throw new NotSupportedException(String.Format("MenuType.{0}", code));
            }
        }

        public static string ToCharValue(this MenuType? value)
        {
            if (value == null)
            {
                return null;
            }

            switch (value.Value)
            {
                case MenuType.Category: return "C";
                case MenuType.Page: return "P";
                case MenuType.Link: return "L";
                default: throw new NotSupportedException(String.Format("MenuType.{0}", value));
            }
        }

        public static bool? ToBoolean(this string value)
        {
            if (value == null)
            {
                return null;
            };

            string code = value.Trim().ToUpper();

            switch (code)
            {
                case "Y": return true;
                case "N": return false;
                default: throw new NotSupportedException(String.Format("Boolean For {0}", code));
            }
        }

        public static string ToCharValue(this bool? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value ? "Y" : "N";
        }

        public static MenuStatus? ToMenuStatus(this string value)
        {
            if (value == null)
            {
                return null;
            };

            string code = value.Trim().ToUpper();

            switch (code)
            {
                case "A": return MenuStatus.Active;
                case "I": return MenuStatus.Inactive;
                default: throw new NotSupportedException(String.Format("MenuStatus.{0}", code));
            }
        }

        public static string ToCharValue(this MenuStatus? value)
        {
            if (value == null)
            {
                return null;
            }

            switch (value.Value)
            {
                case MenuStatus.Active: return "A";
                case MenuStatus.Inactive: return "I";
                default: throw new NotSupportedException(String.Format("MenuStatus.{0}", value));
            }
        }
    }

}
