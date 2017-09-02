using System;
using System.Collections.Generic;
using System.Linq;

using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Resources;
using System.Diagnostics;


namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess
{
    public class ControlPanelMenuBiz
    {
        private CPDataContext m_dataContext = new CPDataContext(OperationType.Action);

        #region Query

        public List<ControlPanelLocalizedMenuEntity> GetLocalizedMenuItems(ControlPanelLocalizedMenuQueryEntity queryEntity)
        {
            CPDataContext ctx = new CPDataContext(OperationType.Query);

            var query = from menu in ctx.FN_ControlPanelMenuWithLocalizedRes(queryEntity.LanguageCode ?? String.Empty)
                        where (queryEntity.ApplicationIds.Length > 0 ? queryEntity.ApplicationIds.Contains(menu.ApplicationId2) : true)
                            && (queryEntity.StatusCode != null ? (menu.StatusCode == queryEntity.StatusCode) : true)
                        orderby menu.SortIndex descending, menu.MenuId ascending
                        select menu;

            List<ControlPanelLocalizedMenuEntity> result = query.ToList();

            return result;
        }

        public List<ControlPanelMenuEntity> GetMenuItems(ControlPanelMenuQueryEntity queryEntity)
        {
            var query = this.GetMenuQueryable(queryEntity);

            List<ControlPanelMenuEntity> result = query.ToList();

            var localizedRes = this.GetLocalizedRes(null);

            foreach (var item in result)
            {
                item.LocalizedMenuEntities = localizedRes.FindAll(res => res.ReferenceMenuId == item.MenuId);
            }

            return result;
        }

        public ControlPanelMenuEntity GetMenuItem(ControlPanelMenuQueryEntity queryEntity)
        {
            var query = this.GetMenuQueryable(queryEntity);

            ControlPanelMenuEntity result = query.FirstOrDefault();

            return result;
        }

        public List<ControlPanelMenuLocalizedRes> GetLocalizedRes(Guid? referenceMenuId)
        {
            var queryResult = from res in this.m_dataContext.ControlPanelMenuLocalizedRes
                              where (referenceMenuId != null ? res.ReferenceMenuId == referenceMenuId : true)
                              orderby res.LanguageCode ascending
                              select res;

            return queryResult.ToList();
        }

        #endregion

        #region Action


        public ControlPanelMenuEntity Create(ControlPanelMenuEntity entity)
        {
            if (entity.Type == MenuType.Page)
            {
                var d = this.GetMenuItem(new ControlPanelMenuQueryEntity
                {
                    ApplicationIds = CPConfig.Keystone.ApplicationIds,
                    LinkPath = entity.LinkPath
                });

                if (d != null)
                {
                    throw new BusinessException(InfoMessage.Error_MenuMaintain_DuplicateMenu);
                }
            }

            this.CreateItem(entity);
            m_dataContext.ControlPanelMenuLocalizedRes.InsertAllOnSubmit(entity.LocalizedMenuEntities);
            this.m_dataContext.SubmitChanges();
            return entity;
        }

        public ControlPanelMenuEntity Update(ControlPanelMenuEntity entity)
        {
            if (entity.Type == MenuType.Page)
            {
                var dx = this.GetMenuQueryable(new ControlPanelMenuQueryEntity
                {
                    ApplicationIds = CPConfig.Keystone.ApplicationIds,
                    LinkPath = entity.LinkPath,

                }).ToList();

                if (dx.Count > 0 && dx.All(d => d.MenuId != entity.MenuId))
                {
                    throw new BusinessException(InfoMessage.Error_MenuMaintain_DuplicateMenu);
                }
            }

            entity = this.UpdateItem(entity);
            DeleteLocalizedRes(new List<Guid> { entity.MenuId });

            foreach (var localized in entity.LocalizedMenuEntities)
            {
                localized.EditDate = DateTime.Now;
            }

            m_dataContext.ControlPanelMenuLocalizedRes.InsertAllOnSubmit(entity.LocalizedMenuEntities);
            this.m_dataContext.SubmitChanges();

            return entity;
        }

        public List<ControlPanelMenuEntity> BatchUpdateSortIndex(List<ControlPanelMenuEntity> list)
        {
            List<ControlPanelMenuEntity> resultList = null;

            if (list != null)
            {
                resultList = new List<ControlPanelMenuEntity>();
                var changeList = BatchChangeSortIndex(list);
                resultList.AddRange(changeList);
                this.m_dataContext.SubmitChanges();
            }

            return resultList;
        }

        public void Delete(ControlPanelMenuEntity entity)
        {
            this.DeleteItem(entity);
            DeleteLocalizedRes(new List<Guid> { entity.MenuId });
            this.m_dataContext.SubmitChanges();
        }

        public List<ControlPanelMenuEntity> ImportMenu(List<ControlPanelMenuEntity> list)
        {

#if TRACE
            var stopwatch = new System.Diagnostics.Stopwatch();
#endif
            var addList = new List<ControlPanelMenuEntity>();
            var localizedMenuIds = new List<Guid>();
            var localizedList = new List<ControlPanelMenuLocalizedRes>();
            var collection = GetMenuItems(new ControlPanelMenuQueryEntity { ApplicationIds = CPConfig.Keystone.ApplicationIds });

#if TRACE
            stopwatch.Restart();
#endif
            #region 导入所有的页面菜单

            var pages = list.FindAll(item => item.Type == MenuType.Page);

            foreach (var item in pages)
            {
                var entity = collection.FirstOrDefault(p => p.LinkPath == item.LinkPath || p.MenuId == item.MenuId);

                if (entity != null)
                {
                    entity.DisplayName = item.DisplayName;
                    entity.Description = item.Description;
                    entity.LinkPath = item.LinkPath;
                    entity.AuthKey = item.AuthKey;
                    entity.IsDisplay = item.IsDisplay;
                    entity.Status = item.Status;

                    entity.EditDate = DateTime.Now;
                    entity.EditUser = item.InUser;
                }
                else
                {
                    addList.Add(item);
                }

                if (!localizedMenuIds.Contains(item.MenuId))
                {
                    localizedMenuIds.Add(item.MenuId);
                    localizedList.AddRange(item.LocalizedMenuEntities);
                }
            }

            #endregion

            #region 导入所有的分类菜单

            var _categories = new List<ControlPanelMenuEntity>();

            foreach (var item in addList)
            {
                GetCategories(item, list, ref _categories);
            }

            foreach (var c in _categories)
            {
                var entity = collection.FirstOrDefault(ca => c.MenuId == ca.MenuId);
                if (entity == null)
                {
                    addList.Add(c);

                    if (!localizedMenuIds.Contains(c.MenuId))
                    {
                        localizedMenuIds.Add(c.MenuId);
                        localizedList.AddRange(c.LocalizedMenuEntities);
                    }
                }
            }


            var cs = list.FindAll(c => c.Type == MenuType.Category);
            foreach (var c in cs)
            {
                var entity = collection.FirstOrDefault(ca => c.MenuId == ca.MenuId);
                if (entity != null)
                {
                    entity.DisplayName = c.DisplayName;
                    entity.Description = c.Description;
                    entity.IsDisplay = c.IsDisplay;
                    entity.Status = c.Status;


                    entity.EditDate = DateTime.Now;
                    entity.EditUser = c.InUser;

                    if (!localizedMenuIds.Contains(c.MenuId))
                    {
                        localizedMenuIds.Add(c.MenuId);
                        localizedList.AddRange(c.LocalizedMenuEntities);
                    }
                }
            }


            #endregion

            #region 导入所有的链接菜单

            var links = list.FindAll(item => item.Type == MenuType.Link);

            foreach (var link in links)
            {
                var linkPaths = link.LinkPath.Split('@');
                if (linkPaths.Length == 2)
                {
                    var p = collection.FirstOrDefault(m => m.Type == MenuType.Page && (m.MenuId.ToString() == linkPaths[0] || m.LinkPath == linkPaths[1]));
                    if (p == null)
                    {
                        p = addList.FirstOrDefault(m => m.Type == MenuType.Page && (m.MenuId.ToString() == linkPaths[0] || m.LinkPath == linkPaths[1]));
                    }

                    if (p != null)
                    {
                        var entity = collection.FirstOrDefault(l => l.MenuId == link.MenuId);

                        if (entity != null)
                        {
                            entity.LinkPath = p.MenuId.ToString();

                            entity.EditUser = link.InUser;
                            entity.EditDate = DateTime.Now;
                        }
                        else
                        {
                            link.LinkPath = p.MenuId.ToString();

                            addList.Add(link);
                        }
                    }
                }
            }

            #endregion

#if TRACE
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("ControlPanel.Service.ImportMenu.ProcessImportList => Elapsed time:{0} ms", stopwatch.ElapsedMilliseconds));
            stopwatch.Restart();
#endif

            var unMatchedMenuIds = GetUnMatchedAddMenuIds(addList, collection);

            addList.RemoveAll(item => unMatchedMenuIds.Contains(item.MenuId));
            localizedMenuIds.RemoveAll(item => unMatchedMenuIds.Contains(item));
            localizedList.RemoveAll(item => unMatchedMenuIds.Contains(item.ReferenceMenuId));

            this.DeleteLocalizedRes(localizedMenuIds);
            this.m_dataContext.ControlPanelMenuLocalizedRes.InsertAllOnSubmit(localizedList);
            this.m_dataContext.ControlPanelMenuEntities.InsertAllOnSubmit(addList);
            this.m_dataContext.SubmitChanges();

#if TRACE
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("ControlPanel.Service.ImportMenu.PersistToDB => Elapsed time:{0} ms", stopwatch.ElapsedMilliseconds));
#endif

            return GetMenuItems(new ControlPanelMenuQueryEntity { ApplicationIds = CPConfig.Keystone.ApplicationIds });
        }

        private List<Guid> GetUnMatchedAddMenuIds(List<ControlPanelMenuEntity> _addList, List<ControlPanelMenuEntity> list)
        {
            //var list = (from m in m_dataContext.ControlPanelMenuEntities select m).ToList();

            var unMatchedMenuIds = new List<Guid>();

            foreach (var item in _addList)
            {
                if (list.Any(m => m.MenuId == item.MenuId))
                {
                    unMatchedMenuIds.Add(item.MenuId);
                }
            }

            return unMatchedMenuIds;
        }

        #endregion

        #region Private Methods

        private void GetCategories(ControlPanelMenuEntity item, List<ControlPanelMenuEntity> list, ref List<ControlPanelMenuEntity> _categoryList)
        {
            var category = list.FirstOrDefault(c => c.MenuId == item.ParentMenuId);
            if (category != null)
            {
                if (!_categoryList.Contains(category))
                {
                    _categoryList.Add(category);
                }
                GetCategories(category, list, ref _categoryList);
            }


            //var uniques = new List<ControlPanelMenuEntity>();
            //foreach (var c in categories)
            //{
            //    if (!_categoryList.Contains(c))
            //    {
                   
            //    }
            //}
            //_categoryList.AddRange(uniques);

            //foreach (var c in categories)
            //{
                
            //}
        }

        //private IQueryable<ControlPanelLocalizedMenuEntity> GetMenuQueryable(ControlPanelLocalizedMenuQueryEntity queryEntity)
        //{
        //    if (queryEntity == null)
        //    {
        //        throw new ArgumentNullException("queryEntity");
        //    }

        //    if (queryEntity.ApplicationIds == null)
        //    {
        //        queryEntity.ApplicationIds = new string[] { };
        //    }

        //    var query = from menu in this.m_dataContext.FN_ControlPanelMenuWithLocalizedRes(queryEntity.LanguageCode ?? String.Empty)
        //                where (queryEntity.ApplicationIds.Length > 0 ? queryEntity.ApplicationIds.Contains(menu.ApplicationId2) : true)
        //                    && (queryEntity.StatusCode != null ? (menu.StatusCode == queryEntity.StatusCode) : true)
        //                orderby menu.SortIndex descending, menu.MenuId ascending
        //                select menu;

        //    return query;
        //}

        private IQueryable<ControlPanelMenuEntity> GetMenuQueryable(ControlPanelMenuQueryEntity queryEntity)
        {
            if (queryEntity == null)
            {
                throw new ArgumentNullException("queryEntity");
            }

            if (queryEntity.ApplicationIds == null)
            {
                queryEntity.ApplicationIds = new string[] { };
            }

            var query = from menu in this.m_dataContext.ControlPanelMenuEntities
                        where (queryEntity.MenuId != null ? menu.MenuId == queryEntity.MenuId : true)
                            && (queryEntity.ApplicationIds.Length > 0 ? queryEntity.ApplicationIds.Contains(menu.ApplicationId2) : true)
                            && (queryEntity.LinkPath != null ? menu.LinkPath == queryEntity.LinkPath : true)
                        orderby menu.SortIndex descending, menu.MenuId ascending
                        select menu;

            return query;
        }

        private ControlPanelMenuEntity CreateItem(ControlPanelMenuEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.InDate = DateTime.Now;

            this.m_dataContext.ControlPanelMenuEntities.InsertOnSubmit(entity);

            return entity;
        }

        private List<ControlPanelMenuEntity> BatchChangeSortIndex(List<ControlPanelMenuEntity> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                throw new ArgumentNullException("entity");
            }

            List<ControlPanelMenuEntity> menuEntities = new List<ControlPanelMenuEntity>();
                     
            var originalList = (from menu in this.m_dataContext.ControlPanelMenuEntities
                                      select menu).ToList();
            
            foreach (var entity in entities)
            {
                foreach (var originalEntity in originalList)
                {
                    if (entity.MenuId == originalEntity.MenuId)
                    {
                        if (entity.SortIndex != null && entity.SortIndex != originalEntity.SortIndex)
                        {
                            originalEntity.SortIndex = entity.SortIndex;
                            originalEntity.EditUser = entity.EditUser;
                            originalEntity.EditDate = DateTime.Now;
                            menuEntities.Add(originalEntity);
                        }
                    }
                }
            }
            return menuEntities;
        }

        private ControlPanelMenuEntity UpdateItem(ControlPanelMenuEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.MenuId == null)
            {
                throw new ArgumentNullException("entity.MenuId");
            }

            var original = from menu in this.m_dataContext.ControlPanelMenuEntities
                           where menu.MenuId == entity.MenuId
                           select menu;

            var item = original.FirstOrDefault();
            if (item != null)
            {
                if (entity.DisplayName != null)
                {
                    item.DisplayName = entity.DisplayName;
                }

                if (entity.Description != null)
                {
                    item.Description = entity.Description;
                }

                if (entity.IconStyle != null)
                {
                    item.IconStyle = entity.IconStyle;
                }

                if (entity.AuthKey != null)
                {
                    item.AuthKey = entity.AuthKey;
                }

                if (entity.LinkPath != null)
                {
                    item.LinkPath = entity.LinkPath;
                }

                if (entity.IsDisplay != null)
                {
                    item.IsDisplay = entity.IsDisplay;
                }

                if (entity.SortIndex != null)
                {
                    item.SortIndex = entity.SortIndex;
                }

                if (entity.ParentMenuId != null)
                {
                    item.ParentMenuId = entity.ParentMenuId;
                }

                if (entity.Status != null)
                {
                    item.Status = entity.Status;
                }

                if (entity.ApplicationId != null)
                {
                    //如果ApplicationId改变，并且此Menu为Domain Categoory
                    //则需要对其下面的所有的Category和Page的ApplicationId进行批量更新
                    if (item.TypeCode == "C" && item.ParentMenuId == null && item.ApplicationId != entity.ApplicationId)
                    {
                        var menuIds = new List<Guid>();
                        var collection = GetMenuItems(new ControlPanelMenuQueryEntity { ApplicationIds = new string[] { item.ApplicationId } });

                        GetAllSubMenuItems(ref menuIds, collection, item);

                        var list = from p in m_dataContext.ControlPanelMenuEntities
                                   where menuIds.Contains(p.MenuId)
                                   select p;

                        foreach (var i in list)
                        {
                            i.ApplicationId = entity.ApplicationId;
                            i.EditUser = entity.EditUser;
                            i.EditDate = DateTime.Now;
                        }
                    }

                    item.ApplicationId = entity.ApplicationId;
                }

                if (entity.EditUser != null)
                {
                    item.EditUser = entity.EditUser;
                }

                if (entity.LocalizedMenuEntities != null)
                {
                    item.LocalizedMenuEntities = entity.LocalizedMenuEntities;
                }

                item.EditDate = DateTime.Now;
            };

            return item;
        }

        private void DeleteItem(ControlPanelMenuEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.MenuId == null)
            {
                throw new ArgumentNullException("entity.MenuId");
            }

            var original = from menu in this.m_dataContext.ControlPanelMenuEntities
                           where menu.MenuId == entity.MenuId
                           select menu;
            this.m_dataContext.ControlPanelMenuEntities.DeleteAllOnSubmit(original);
        }

        private void GetAllSubMenuItems(ref List<Guid> menuIds, List<ControlPanelMenuEntity> collection, ControlPanelMenuEntity item)
        {
            var subCollection = collection.FindAll(p =>
            {
                if (item.MenuId == p.ParentMenuId)
                {
                    return true;
                }
                return false;
            });

            foreach (var sub in subCollection)
            {
                menuIds.Add(sub.MenuId);
                GetAllSubMenuItems(ref menuIds, collection, sub);
            }
        }

        private void DeleteLocalizedRes(List<Guid> referenceMenuIds)
        {
            var original = from res in this.m_dataContext.ControlPanelMenuLocalizedRes
                           where referenceMenuIds.Contains(res.ReferenceMenuId)
                           select res;
            this.m_dataContext.ControlPanelMenuLocalizedRes.DeleteAllOnSubmit(original);
        }

        #endregion
    }
}
