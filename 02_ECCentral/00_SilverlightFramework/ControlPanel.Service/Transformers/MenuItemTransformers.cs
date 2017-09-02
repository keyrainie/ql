using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Framework.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public static class MenuItemTransformers
    {
        public static MenuItem ToItem(this ControlPanelMenuEntity entity)
        {
            MenuItem msg = null;

            if (entity != null)
            {
                msg = new MenuItem()
                {
                    MenuId = entity.MenuId,
                    DisplayName = entity.DisplayName,
                    Description = entity.Description,
                    IconStyle = entity.IconStyle,
                    LinkPath = entity.LinkPath,
                    AuthKey = entity.AuthKey,
                    Type = entity.TypeCode,
                    IsDisplay = entity.IsDisplay,
                    Status = entity.StatusCode,
                    SortIndex = entity.SortIndex,
                    ParentMenuId = entity.ParentMenuId,
                    ApplicationId = entity.ApplicationId == null ? null : entity.ApplicationId.ToUpper(),
                    LanguageCode = entity.LanguageCode,
                    InDate = entity.InDate,
                    InUser = entity.InUser,
                    EditDate = entity.EditDate,
                    EditUser = entity.EditUser,
                    LocalizedResCollection = entity.LocalizedMenuEntities.ToMsg()
                };

                if (msg.ApplicationId != null)
                {
                    foreach (var app in CPConfig.Keystone.KeystoneApplications)
                    {
                        if (app.Value.Equals(msg.ApplicationId, StringComparison.OrdinalIgnoreCase))
                        {
                            msg.ApplicationName = app.Key;
                            break;
                        }
                    }
                }
            }

            return msg;
        }

        public static List<MenuItem> ToItem(this List<ControlPanelMenuEntity> list)
        {
            List<MenuItem> msg = null;

            if (list != null)
            {
                msg = new List<MenuItem>();

                list.ForEach(item =>
                {
                    msg.Add(item.ToItem());
                });
            }

            return msg;
        }

        public static ControlPanelMenuEntity ToEntity(this MenuItem msg)
        {
            ControlPanelMenuEntity entity = null;

            if (msg != null)
            {
                entity = new ControlPanelMenuEntity()
                {
                    MenuId = msg.MenuId.HasValue ? msg.MenuId.Value : Guid.NewGuid(),
                    DisplayName = msg.DisplayName,
                    Description = msg.Description,
                    IconStyle = msg.IconStyle,
                    LinkPath = msg.LinkPath,
                    AuthKey = msg.AuthKey,
                    Type = msg.Type.ToMenuType(),
                    IsDisplay = msg.IsDisplay,
                    Status = msg.Status.ToMenuStatus(),
                    SortIndex = msg.SortIndex,
                    ParentMenuId = msg.ParentMenuId,
                    //ApplicationId = msg.ApplicationId == null ? null : msg.ApplicationId.ToUpper(),
                    LanguageCode = msg.LanguageCode,
                    InDate = msg.InDate.HasValue ? msg.InDate.Value : DateTime.Now,
                    InUser = msg.InUser,
                    EditDate = msg.EditDate,
                    EditUser = msg.EditUser
                };

                if (msg.ApplicationId != null)
                {
                    entity.ApplicationId = msg.ApplicationId.ToUpper();
                }
                else if (msg.ApplicationName != null)
                {
                    foreach (var app in CPConfig.Keystone.KeystoneApplications)
                    {
                        if (app.Key.Equals(msg.ApplicationName, StringComparison.OrdinalIgnoreCase))
                        {
                            entity.ApplicationId = app.Value.ToUpper();
                            break;
                        }
                    }
                    if (entity.ApplicationId == null)
                    {
                        throw new IndexOutOfRangeException("ApplicationId is not found via application name.");
                    }
                }

                entity.LocalizedMenuEntities = msg.LocalizedResCollection.ToEntity(entity);
            }

            if (entity.LocalizedMenuEntities != null)
            {
                for (int i = entity.LocalizedMenuEntities.Count - 1; i > -1; i--)
                {
                    if (StringUtility.IsNullOrEmpty(entity.LocalizedMenuEntities[i].DisplayName))
                    {
                        entity.LocalizedMenuEntities.RemoveAt(i);
                    }
                }
            }

            return entity;
        }

        public static List<ControlPanelMenuEntity> ToEntity(this List<MenuItem> msg)
        {
            List<ControlPanelMenuEntity> entity = null;

            if (msg != null)
            {
                entity = new List<ControlPanelMenuEntity>();

                msg.ForEach(item =>
                {
                    entity.Add(item.ToEntity());
                });
            }

            return entity;
        }

        public static ControlPanelMenuLocalizedRes ToEntity(this LocalizedRes msg, ControlPanelMenuEntity menuItemEntity)
        {
            ControlPanelMenuLocalizedRes entity = null;
            if (msg != null)
            {
                entity = new ControlPanelMenuLocalizedRes
                {
                    MenuDescription = msg.Description,
                    EditDate = menuItemEntity.InDate,
                    EditUser = menuItemEntity.InUser,
                    IconStyle = menuItemEntity.IconStyle,
                    InDate = menuItemEntity.InDate,
                    InUser = menuItemEntity.InUser,
                    LanguageCode = msg.LanguageCode,
                    LinkPath = menuItemEntity.LinkPath,
                    DisplayName = msg.Name,
                    ReferenceMenuId = menuItemEntity.MenuId
                };
            }
            return entity;
        }

        public static List<ControlPanelMenuLocalizedRes> ToEntity(this List<LocalizedRes> msg, ControlPanelMenuEntity menuItemEntity)
        {
            List<ControlPanelMenuLocalizedRes> entity = null;

            if (msg != null)
            {
                entity = new List<ControlPanelMenuLocalizedRes>();

                msg.ForEach(item =>
                {
                    entity.Add(item.ToEntity(menuItemEntity));
                });
            }

            return entity;
        }

        public static List<AuthFunctionMsg> ToMsg(this List<FunctionalAbilityEntity> entities)
        {
            List<AuthFunctionMsg> list = new List<AuthFunctionMsg>();
            if (entities == null || entities.Count == 0)
            {
                return list;
            }
            foreach (var item in entities)
            {
                list.Add(new AuthFunctionMsg
                {
                    ApplicationId = item.ApplicationId,
                    Name = item.Name
                });
            }
            return list;
        }

        public static List<RoleAttribute> ToMsg(this List<RoleAttributeEntity> entities)
        {
            List<RoleAttribute> list = new List<RoleAttribute>();
            if (entities == null || entities.Count == 0)
            {
                return list;
            }
            foreach (var item in entities)
            {
                list.Add(new RoleAttribute
                {
                    ApplicationId = item.ApplicationId,
                    Name = item.Name,
                    RoleName = item.RoleName,
                    Type = item.Type,
                    Value = item.Value
                });
            }
            return list;
        }

        public static List<Role> ToMsg(this List<RoleEntity> entities)
        {
            List<Role> list = new List<Role>();
            if (entities == null || entities.Count == 0)
            {
                return list;
            }
            foreach (var item in entities)
            {
                list.Add(new Role
                {
                    ApplicationID = item.ApplicationID,
                    RoleID = item.RoleID,
                    RoleName = item.RoleName
                });
            }
            return list;
        }

        public static LocalizedRes ToMsg(this ControlPanelMenuLocalizedRes entity)
        {
            LocalizedRes msg = null;
            if (entity != null)
            {
                msg = new LocalizedRes
                {
                    Description = entity.MenuDescription,
                    LanguageCode = entity.LanguageCode,
                    Name = entity.DisplayName
                };
            }

            return msg;
        }

        public static List<LocalizedRes> ToMsg(this List<ControlPanelMenuLocalizedRes> entities)
        {
            List<LocalizedRes> msg = new List<LocalizedRes>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    msg.Add(entity.ToMsg());
                }

            }
            return msg;
        }

    }

}
