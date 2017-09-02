using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Transactions;

using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [RestService]
    public class MenuRestService
    {
        [WebGet(UriTemplate = "")]
        public List<MenuItem> GetCollection()
        {
            var queryEntity = new ControlPanelMenuQueryEntity()
            {
                ApplicationIds = CPConfig.Keystone.ApplicationIds
            };

            var list = new ControlPanelMenuBiz().GetMenuItems(queryEntity);

            var result = list.ToItem();

            return result;
        }
        
        [WebGet(UriTemplate = "/{id}")]
        public MenuItem Get(string id)
        {
            ControlPanelMenuQueryEntity queryEntity = new ControlPanelMenuQueryEntity()
            {
                MenuId = Guid.Parse(id)
            };

            var biz = new ControlPanelMenuBiz();
            ControlPanelMenuEntity item = biz.GetMenuItem(queryEntity);
           
            MenuItem result = item.ToItem();
             if (result != null)
            {
                result.LocalizedResCollection = biz.GetLocalizedRes(result.MenuId).ToMsg();
            }
            return result;
        }

        [WebInvoke(UriTemplate="",Method = "POST")]
        public MenuItem Create(MenuItem item)
        {
            ControlPanelMenuEntity entity = item.ToEntity();

            entity = new ControlPanelMenuBiz().Create(entity);

            MenuItem result = entity.ToItem();

            return result;
        }

        [WebInvoke(UriTemplate = "/{id}", Method = "PUT")]
        public MenuItem Update(string id, MenuItem item)
        {
            ControlPanelMenuEntity entity = item.ToEntity();
            entity.MenuId = Guid.Parse(id);

            entity = new ControlPanelMenuBiz().Update(entity);

            MenuItem result = entity.ToItem();

            return result;
        }

        [WebInvoke(UriTemplate="",Method = "PUT")]
        public void BatchUpdate(List<MenuItem> items)
        {
            var list = items.ToEntity();

            new ControlPanelMenuBiz().BatchUpdateSortIndex(list);
        }

        [WebInvoke(UriTemplate = "Delete", Method = "DELETE")]
        public void Delete(string id)
        {
            var biz = new ControlPanelMenuBiz();
            var linkQuery = new ControlPanelMenuQueryEntity
            {
                LinkPath = id
            };

            var entity = new ControlPanelMenuEntity
            {
                MenuId = Guid.Parse(id)
            };

            var links = biz.GetMenuItems(linkQuery);

            using (var scope = new TransactionScope())
            {
                biz.Delete(entity);

                foreach (var link in links)
                {
                    biz.Delete(new ControlPanelMenuEntity {MenuId = link.MenuId});
                }

                scope.Complete();
            }
        }

        [WebInvoke(UriTemplate = "ImportMenu", Method = "POST")]
        public List<MenuItem> ImportMenu(List<MenuItem> items)
        {
            if (items == null)
            {
                return null;
            }

            var entity = items.ToEntity();

            var result = new ControlPanelMenuBiz().ImportMenu(entity).ToItem();

            return result;
        }
    }

}
