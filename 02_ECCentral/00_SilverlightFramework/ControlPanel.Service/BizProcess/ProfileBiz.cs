using System;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Framework.DataConfiguration;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BusinessProcess
{
    public class ProfileBiz
    {
        private ControlPanelDataContext m_dataContext = new CPDataContext(OperationType.Action);

        public List<ProfileEntity> GetProfiles(ProfileQueryEntity queryEntity)
        {
            CPDataContext ctx = new CPDataContext(OperationType.Query);

            var query = from profile in ctx.ProfileEntities
                        where profile.ApplicationId2 == queryEntity.ApplicationId 
                            &&  profile.InUser == queryEntity.InUser
                            &&  profile.ProfileType == queryEntity.ProfileType
                        select profile;

            List<ProfileEntity> result = query.ToList();

            return result;
        }

        public ProfileEntity GetProfile(ProfileQueryEntity queryEntity)
        {
            CPDataContext ctx = new CPDataContext(OperationType.Action);

            var query = from profile in ctx.ProfileEntities
                        where profile.ApplicationId2 == queryEntity.ApplicationId
                            && profile.InUser == queryEntity.InUser
                            && profile.ProfileType == queryEntity.ProfileType
                        select profile;

            ProfileEntity result = query.FirstOrDefault();

            return result;
        }

        public ProfileEntity Create(ProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.ApplicationId == null)
            {
                throw new ArgumentNullException("entity.ApplicationId");
            }
            if (entity.InUser == null)
            {
                throw new ArgumentNullException("entity.InUser");
            }
            if (entity.ProfileValue == null)
            {
                throw new ArgumentNullException("entity.ProfileValue");
            }

            entity.InDate = DateTime.Now;

            this.m_dataContext.ProfileEntities.InsertOnSubmit(entity);
            this.m_dataContext.SubmitChanges();

            return entity;
        }

        public ProfileEntity Update(ProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.ProfileValue == null)
            {
                throw new ArgumentNullException("entity.PropertyValue");
            }
            if (entity.ProfileId == 0)
            {
                throw new ArgumentNullException("entity.ProfileId");
            }

            var original = from profile in this.m_dataContext.ProfileEntities
                           where profile.ProfileId == entity.ProfileId
                           select profile;

            original.ToList().ForEach(item =>
            {
                item.ProfileValue = entity.ProfileValue;
            });

            this.m_dataContext.SubmitChanges();

            return entity;
        }


        public List<ProfileItem> GetDataGridProfileItems(string guid)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryDataGridProfileByGuid");
            cmd.ReplaceParameterValue("#GridGuid#", guid);
            return cmd.ExecuteEntityList<ProfileItem>();
        }
    }

}
