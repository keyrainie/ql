using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models
{
    [EntityValidation]
    public class MenuItemModel : ModelBase
    {
        private Guid? m_menuId;
        private string m_displayName;
        private string m_description;
        private string m_iconStyle;
        private string m_linkPath;
        private string m_type;
        private string m_authKey;
        private bool? m_isDisplay;
        private string m_status;
        private Guid? m_parentMenuId;
        private int? m_sortIndex;
        private string m_applicationId;
        private string m_applicationName;
        private string m_languageCode;
        private DateTime? m_inDate;
        private string m_inUser;
        private DateTime? m_editDate;
        private string m_editUser;

        private List<LocalizedRes> m_localizedResCollection = new List<LocalizedRes>();

        public MenuItemModel()
        { }

        public MenuItemModel(MenuItemModel model)
        {
            this.SetFields(model);
        }

        public Guid? MenuId
        {
            get { return this.m_menuId; }
            set { base.SetValue("MenuId", ref this.m_menuId, value); }
        }

        public string DisplayName
        {
            get { return this.m_displayName; }
            set { base.SetValue("DisplayName", ref this.m_displayName, value); }
        }

        public string LocalizedDisplayName
        {
            get
            {
                var localizedRes = m_localizedResCollection.FirstOrDefault(localized => string.Equals(localized.LanguageCode, CPApplication.Current.LanguageCode, StringComparison.OrdinalIgnoreCase));
                if (localizedRes != null && !localizedRes.Name.IsNullOrEmpty())
                {
                    return localizedRes.Name;
                }
                return DisplayName;
            }
        }

        public string Description
        {
            get { return this.m_description; }
            set { base.SetValue("Description", ref this.m_description, value); }
        }

        public string IconStyle
        {
            get { return this.m_iconStyle; }
            set { base.SetValue("IconStyle", ref this.m_iconStyle, value); }
        }

        public string LinkPath
        {
            get { return this.m_linkPath; }
            set { base.SetValue("LinkPath", ref this.m_linkPath, value); }
        }

        public string Type
        {
            get { return this.m_type; }
            set { base.SetValue("Type", ref this.m_type, value); }
        }

        public string AuthKey
        {
            get { return this.m_authKey; }
            set { base.SetValue("AuthKey", ref this.m_authKey, value); }
        }

        public bool? IsDisplay
        {
            get { return this.m_isDisplay; }
            set { base.SetValue("IsDisplay", ref this.m_isDisplay, value); }
        }

        public string Status
        {
            get { return this.m_status; }
            set { base.SetValue("Status", ref this.m_status, value); }
        }


        public Guid? ParentMenuId
        {
            get { return this.m_parentMenuId; }
            set { base.SetValue("ParentMenuId", ref this.m_parentMenuId, value); }
        }

        public int? SortIndex
        {
            get { return this.m_sortIndex; }
            set { base.SetValue("SortIndex", ref this.m_sortIndex, value); }
        }

        public string ApplicationId
        {
            get { return this.m_applicationId; }
            set { base.SetValue("ApplicationId", ref this.m_applicationId, value); }
        }

        public string ApplicationName
        {
            get { return this.m_applicationName; }
            set { base.SetValue("ApplicationName", ref this.m_applicationName, value); }
        }

        public string LanguageCode
        {
            get { return this.m_languageCode; }
            set { base.SetValue("LanguageCode", ref this.m_languageCode, value); }
        }

        public DateTime? InDate
        {
            get { return this.m_inDate; }
            set { base.SetValue("InDate", ref this.m_inDate, value); }
        }

        public string InUser
        {
            get { return this.m_inUser; }
            set { base.SetValue("InUser", ref this.m_inUser, value); }
        }

        public DateTime? EditDate
        {
            get { return this.m_editDate; }
            set { base.SetValue("EditDate", ref this.m_editDate, value); }
        }

        public string EditUser
        {
            get { return this.m_editUser; }
            set { base.SetValue("EditUser", ref this.m_editUser, value); }
        }



        [IgnoreDataMember]
        public MenuItemModel Parent { get; set; }

        [IgnoreDataMember]
        public ObservableCollection<MenuItemModel> Children { get; set; }

        public List<LocalizedRes> LocalizedResCollection
        {
            get
            {
                var zhCN = new LocalizedRes { LanguageCode = "zh-CN", Description = string.Empty, Name = string.Empty };
                var zhTW = new LocalizedRes { LanguageCode = "zh-TW", Description = string.Empty, Name = string.Empty };
                var zhJP = new LocalizedRes { LanguageCode = "ja-JP", Description = string.Empty, Name = string.Empty };
                if (!m_localizedResCollection.Contains(zhCN))
                {
                    m_localizedResCollection.Add(zhCN);
                }
                if (!m_localizedResCollection.Contains(zhTW))
                {
                    m_localizedResCollection.Add(zhTW);
                }
                if (!m_localizedResCollection.Contains(zhJP))
                {
                    m_localizedResCollection.Add(zhJP);
                }
                return m_localizedResCollection.TrimValue();
            }
            set
            {
                m_localizedResCollection = value;
            }
        }


        [IgnoreDataMember]
        public string IconPath
        {
            get
            {
                return String.Format("/Images/menuMaintain/tree_icon{0}.png", this.Type);
            }
        }
        
        [IgnoreDataMember]
        public string MenuPath{get;set;}


        public object Clone()
        {
            MenuItemModel model = new MenuItemModel()
            {
                MenuId = this.MenuId,
                DisplayName = this.DisplayName,
                Description = this.Description,
                IconStyle = this.IconStyle,
                LinkPath = this.LinkPath,
                AuthKey = this.AuthKey,
                Type = this.Type,
                SortIndex = this.SortIndex,
                ParentMenuId = this.ParentMenuId,
                Status = this.Status,
                IsDisplay = this.IsDisplay,
                LanguageCode = this.LanguageCode,
                ApplicationId = this.ApplicationId,
                m_applicationName = this.ApplicationName,
                InDate = this.InDate,
                InUser = this.InUser,
                EditDate = this.EditDate,
                EditUser = this.EditUser,
                Parent = this.Parent,
                Children = this.Children,
                LocalizedResCollection = this.LocalizedResCollection
            };

            return model;
        }

        public void SetFields(MenuItemModel model)
        {
            if (model != null)
            {
                this.MenuId = model.MenuId;
                this.DisplayName = model.DisplayName;
                this.Description = model.Description;
                this.IconStyle = model.IconStyle;
                this.LinkPath = model.LinkPath;
                this.AuthKey = model.AuthKey;
                this.Type = model.Type;
                this.SortIndex = model.SortIndex;
                this.ParentMenuId = model.ParentMenuId;
                this.Status = model.Status;
                this.IsDisplay = model.IsDisplay;
                this.LanguageCode = model.LanguageCode;
                this.ApplicationId = model.ApplicationId;
                this.ApplicationName = model.ApplicationName;
                this.InDate = model.InDate;
                this.InUser = model.InUser;
                this.EditDate = model.EditDate;
                this.EditUser = model.EditUser;
                this.LocalizedResCollection = model.LocalizedResCollection;
            }
        }

        public MenuItemModel NewCategory(MenuItemModel parentItem)
        {
            this.m_menuId = null;
            this.m_type = "C";
            this.m_isDisplay = true;
            this.m_status = "A";
            this.m_sortIndex = 0;
            this.m_languageCode = "en-US";

            this.m_parentMenuId = null;

            if (parentItem != null)
            {
                this.m_parentMenuId = parentItem.MenuId;
            }
            this.m_applicationId = (parentItem == null) ? null : parentItem.ApplicationId;
            this.m_applicationName = (parentItem == null) ? null : parentItem.ApplicationName;

            this.Parent = parentItem;

            return this;
        }

        public MenuItemModel NewPage(MenuItemModel parentItem)
        {
            this.m_menuId = null;
            this.m_type = "P";
            this.m_isDisplay = true;
            this.m_status = "A";
            this.m_sortIndex = 0;
            this.m_languageCode = "en-US";

            this.m_parentMenuId = null;

            if (parentItem != null)
            {
                this.m_parentMenuId = parentItem.MenuId;
            }

            this.m_applicationId = (parentItem == null) ? null : parentItem.ApplicationId;
            this.m_applicationName = (parentItem == null) ? null : parentItem.ApplicationName;

            this.Parent = parentItem;

            return this;
        }
    }

    public class LocalizedRes
    {
        [System.Xml.Serialization.XmlAttribute()]
        public string LanguageCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is LocalizedRes))
            {
                return base.Equals(obj);
            }
            return string.Compare(LanguageCode, ((LocalizedRes)obj).LanguageCode, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class MenuItemModelExtenstion
    {
        public static ObservableCollection<MenuItemModel> Clone(this ObservableCollection<MenuItemModel> list)
        {
            ObservableCollection<MenuItemModel> result = Clone(list, null);

            return result;
        }

        private static ObservableCollection<MenuItemModel> Clone(ObservableCollection<MenuItemModel> list, MenuItemModel parentItem)
        {
            ObservableCollection<MenuItemModel> result = null;

            if (list != null)
            {
                result = new ObservableCollection<MenuItemModel>();

                foreach (MenuItemModel item in list)
                {
                    MenuItemModel model = item.Clone() as MenuItemModel;

                    model.Parent = parentItem;

                    if (item.Children != null)
                    {
                        model.Children = Clone(item.Children, item);
                    }

                    result.Add(model);
                }
            }

            return result;
        }

        public static ObservableCollection<MenuItemModel> GenerateMenuTree(this ObservableCollection<MenuItemModel> list)
        {
            return GenerateMenuTree(list, null);
        }

        private static ObservableCollection<MenuItemModel> GenerateMenuTree(ObservableCollection<MenuItemModel> dataItems, MenuItemModel parentItem)
        {
            ObservableCollection<MenuItemModel> matchedItems = new ObservableCollection<MenuItemModel>();

            if (dataItems != null)
            {
                // 1、Find matched items
                foreach (MenuItemModel item in dataItems)
                {
                    if ((parentItem == null && item.ParentMenuId == null) || (parentItem != null && item.ParentMenuId == parentItem.MenuId))
                    {
                        matchedItems.Add(item);
                    }
                }
                // 2、Remove matched items from original collection
                foreach (MenuItemModel item in matchedItems)
                {
                    dataItems.Remove(item);
                }
                // 3、Find children for matched items
                foreach (MenuItemModel item in matchedItems)
                {
                    item.Parent = parentItem;
                    item.Children = GenerateMenuTree(dataItems, item);
                }
            }

            return matchedItems;
        }

        public static ObservableCollection<MenuItemData> ToXmlEntity(this ObservableCollection<MenuItemModel> models)
        {
            var result = new ObservableCollection<MenuItemData>();

            models.ToList().ForEach(item =>
            {
                result.Add(item.ToXmlEntity());
            });

            return result;
        }

        public static MenuItemData ToXmlEntity(this MenuItemModel model)
        {
            if (model == null)
                return null;

            string parentMenuId = null;
            if (model.ParentMenuId.HasValue)
            {
                parentMenuId = model.ParentMenuId.Value.ToString();
            }

            return new MenuItemData
            {
                MenuId = model.MenuId.Value.ToString(),
                Name = model.DisplayName,
                Description = model.Description,
                LinkPath = model.LinkPath,
                MenuType = model.Type,
                AuthKey = model.AuthKey,
                ParentMenuId = parentMenuId,
                ApplicationName = model.ApplicationName,
                IsDisplay = model.IsDisplay,
                Status = model.Status,

                LocalizedResList = model.LocalizedResCollection
            };
        }

        public static ObservableCollection<MenuItemModel> ToEntity(this ObservableCollection<MenuItemData> data)
        {
            var result = new ObservableCollection<MenuItemModel>();

            data.ToList().ForEach(item =>
            {
                result.Add(item.ToEntity());
            });

            return result;
        }

        public static MenuItemModel ToEntity(this MenuItemData data)
        {
            Guid? parentMenuId = null;

            if (!data.ParentMenuId.IsNullOrEmpty())
            {
                parentMenuId = new Guid(data.ParentMenuId);
            }

            return new MenuItemModel
            {
                MenuId = new Guid(data.MenuId),
                DisplayName = data.Name,
                Description = data.Description,
                LinkPath = data.LinkPath,
                Type = data.MenuType,
                AuthKey = data.AuthKey,
                ApplicationName = data.ApplicationName,
                ParentMenuId = parentMenuId,
                InUser = CPApplication.Current.LoginUser.ID,
                IsDisplay = data.IsDisplay,
                Status = data.Status,
                LanguageCode = "en-US",
                SortIndex = 0,
                LocalizedResCollection = data.LocalizedResList
            };
        }

        public static List<LocalizedRes> TrimValue(this List<LocalizedRes> value)
        {
            value.ForEach(res => 
            {
                res.Name = res.Name.Trim();
                res.Description = res.Description.IsNullOrEmpty() ? "" : res.Description.Trim();
            });

            return value;
        }
    }

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (bool)value;
            var param = parameter.ToString();

            if (val && param == "A")
            {
                return "A";
            }

            if (val && param == "I")
            {
                return "I";
            }

            return null;
        }
    }


    public class EntityValidation : BaseValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = value as MenuItemModel;

            var members = new List<string>();

            if (obj.DisplayName.IsNullOrEmpty())
            {
                members.Add("DisplayName");
            }
            if (obj.ApplicationId.IsNullOrEmpty())
            {
                members.Add("ApplicationId");
            }
            if (obj.Type == "P" && obj.LinkPath.IsNullOrEmpty())
            {
                members.Add("LinkPath");
            }

            if (members.Count > 0)
            {
                return new ValidationResult(MenuMaintainResource.Validation_RequiredField, members);
            }

            return ValidationResult.Success;
        }
    }
}
