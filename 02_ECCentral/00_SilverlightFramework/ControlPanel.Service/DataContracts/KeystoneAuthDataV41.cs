using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BitKoo.Keystone.AuthService;

using Newegg.Oversea.Framework.Contract;

using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class KeystoneAuthDataV41 : DefaultDataContract
    {
        [DataMember]
        public KeystoneAuthDataMsg Body { get; set; }
    }

    [DataContract]
    public class KeystoneAuthDataMsg
    {
        [DataMember]
        public AuthApplicationMsg Application { get; set; }

        [DataMember]
        public List<KS_ApplicationMsg> KS_Applications { get; set; }

        [DataMember]
        public KeystoneAuthUserMsg AuthUser { get; set; }

        [DataMember]
        public List<AuthFunctionMsg> Functions { get; set; }

        [DataMember]
        public List<AuthMenuItemMsg> MenuData { get; set; }

        [DataMember]
        public List<RoleAttribute> RoleAttributes { get; set; }

        [DataMember]
        public List<Role> Roles { get; set; }
    }

    [DataContract]
    public class AuthApplicationMsg
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DefaultLanguage { get; set; }

        [DataMember]
        public string Version { get; set; }
    }

    [DataContract]
    public class KS_ApplicationMsg
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class AuthFunctionMsg
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ApplicationId { get; set; }
    }

    [DataContract]
    public class AuthMenuItemMsg
    {
        [DataMember]
        public string MenuId { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string IconStyle { get; set; }

        [DataMember]
        public bool IsDisplay { get; set; }

        [DataMember]
        public string LinkPath { get; set; }

        [DataMember]
        public MenuTypeEnum Type { get; set; }

        [DataMember]
        public string ParentMenuId { get; set; }

        [DataMember]
        public string AuthKey { get; set; }
    }

    [DataContract]
    public class RoleAttribute : KeystoneAttribute
    {
        [DataMember]
        public string RoleName { get; set; }
    }

    [DataContract]
    public class KeystoneAttribute
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string ApplicationId {get;set;}
    }

    [DataContract]
    public class Role
    {
        [DataMember]
        public string RoleID { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public string ApplicationID { get; set; }
    }

    [DataContract]
    public enum MenuTypeEnum
    {
        [EnumMember]
        Category,
        [EnumMember]
        Page,
        [EnumMember]
        Link
    }


    public static class KeystoneBizTransformers
    {
        public static AuthMenuItemMsg ToMessage(this ControlPanelLocalizedMenuEntity entity)
        {
            AuthMenuItemMsg msg = null;

            if (entity != null)
            {
                msg = new AuthMenuItemMsg()
                {
                    MenuId = entity.MenuId.ToString(),
                    DisplayName = entity.DisplayName,
                    Description = entity.Description,
                    IconStyle = entity.IconStyle,
                    LinkPath = entity.LinkPath,
                    IsDisplay = entity.IsDisplay,
                    Type = entity.Type.ToMessage(),
                    ParentMenuId = entity.ParentMenuId.HasValue ? entity.ParentMenuId.Value.ToString() : null,
                    AuthKey = entity.AuthKey
                };
            }

            return msg;
        }

        public static List<AuthMenuItemMsg> ToMessage(this List<ControlPanelLocalizedMenuEntity> entity)
        {
            List<AuthMenuItemMsg> msg = null;

            if (entity != null)
            {
                msg = new List<AuthMenuItemMsg>();

                entity.ForEach(item =>
                {
                    msg.Add(item.ToMessage());
                });
            }

            return msg;
        }

        public static AuthFunctionMsg ToMessage(this FunctionalAbility func, string appId)
        {
            AuthFunctionMsg msg = null;

            if (func != null)
            {
                msg = new AuthFunctionMsg()
                {
                    Name = func.Name,

                    ApplicationId = appId
                };
            }

            return msg;
        }

        public static List<AuthFunctionMsg> GetFunctions(this AuthorizationAssertion assert, string appId)
        {
            List<AuthFunctionMsg> functions = null;

            if (assert != null && assert.Roles != null)
            {
                functions = new List<AuthFunctionMsg>();

                foreach (BitKoo.Keystone.AuthService.Role role in assert.Roles)
                {
                    if (role != null && role.FunctionalAbilities != null)
                    {
                        foreach (FunctionalAbility func in role.FunctionalAbilities)
                        {
                            AuthFunctionMsg msg = func.ToMessage(appId);

                            functions.Add(msg);
                        }
                    }
                }
            }

            return functions;
        }

        public static List<RoleAttribute> GetRoleAttributes(this AuthorizationAssertion assert, string appId)
        {
            List<RoleAttribute> attributes = null;
            if (assert != null && assert.Roles != null)
            {
                attributes = new List<RoleAttribute>();

                foreach (BitKoo.Keystone.AuthService.Role role in assert.Roles)
                {
                    if (role != null && role.Attributes != null)
                    {
                        foreach (var attr in role.Attributes)
                        {
                            attributes.Add(new RoleAttribute 
                            {
                                RoleName = role.Name,
                                ApplicationId = appId,
                                Name = attr.Name,
                                Type = attr.AttributeType,
                                Value = attr.Value
                            });
                        }
                    }
                }
            }
            return attributes;
        }

        public static MenuTypeEnum ToMessage(this MenuType value)
        {
            switch (value)
            {
                case MenuType.Category: return MenuTypeEnum.Category;
                case MenuType.Page: return MenuTypeEnum.Page;
                case MenuType.Link: return MenuTypeEnum.Link;
                default: throw new NotSupportedException(String.Format("MenuTypeEnum.{0}", value));
            }
        }
    }

}
