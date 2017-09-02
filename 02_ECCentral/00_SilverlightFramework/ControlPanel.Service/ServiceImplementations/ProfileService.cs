using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BusinessProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [ServiceErrorHandling]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class ProfileService : IProfileV40
    {

        #region IProfileV40 Members

        [ErrorHandling]
        public SimpleTypeDataContract<List<UserProfile>> Query(ProfileQueryV40 contract)
        {
            var queryEntity = contract.Body.ToProfileDataQueryEntity();
            var result = new ProfileBiz().GetProfiles(queryEntity);

            return new SimpleTypeDataContract<List<UserProfile>>
                             {
                                 Header = contract.Header,
                                 Value = result.ToUserProfileEntities()
                             };
        }

        [ErrorHandling]
        public DefaultDataContract Save(SimpleTypeDataContract<List<UserProfile>> contract)
        {
            var biz = new ProfileBiz();

            foreach (var userProfile in contract.Value)
            {
                var entity = biz.GetProfile(new ProfileQueryEntity
                                                {
                                                    ApplicationId = userProfile.ApplicationId,
                                                    InUser = userProfile.InUser,
                                                    ProfileType = userProfile.Key
                                                });

                if (entity == null)
                {
                    entity = new ProfileEntity
                                 {
                                     ApplicationId = userProfile.ApplicationId,
                                     InUser = userProfile.InUser,
                                     ProfileType = userProfile.Key,
                                     ProfileValue = userProfile.Data
                                 };

                    biz.Create(entity);
                }
                else
                {
                    entity.ProfileValue = userProfile.Data;
                    entity.InDate = DateTime.Now;

                    biz.Update(entity);
                }
            }

            return new DefaultDataContract {Header = contract.Header};
        }

        [ErrorHandling]
        public DataGridProfileDataV40 GetDataGridProfileItems(SimpleTypeDataContract<string> guid) 
        {
            DataGridProfileDataV40 result = new DataGridProfileDataV40() 
            {
                Header = guid.Header,
                Body = new List<DataGridProfileItemMsg>()
            };
            List<ProfileItem> profileItems = new ProfileBiz().GetDataGridProfileItems(guid.Value);
            if (profileItems != null && profileItems.Count > 0)
            {
                List<string> userIds = new List<string>();
                foreach (var item in  profileItems)
                {
                    userIds.Add(item.InUser);
                }

                var userList = new KeystoneAuthService().BatchGetUserInfo(userIds);
                int i = 0;
                foreach (var item in profileItems)
                {
                    result.Body.Add(new DataGridProfileItemMsg 
                    {
                        Owner = userList.Body[i],
                        DataGridProfileXml = item.ProfileValue
                    });
                    i++;
                }
            }
            return result;
        }

        #endregion


        
    }

}
