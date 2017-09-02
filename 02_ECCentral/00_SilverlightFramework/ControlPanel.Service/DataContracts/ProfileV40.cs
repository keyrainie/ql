using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    [DataContract]
    public class ProfileQueryV40 : DefaultDataContract
    {
        [DataMember]
        public ProfileQueryMsg Body { get; set; }
    }

    [DataContract]
    public class ProfileQueryMsg
    {
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string ProfileType { get; set; }

        [DataMember]
        public string InUser { get; set; }
    }

    [DataContract]
    public class UserProfile
    {
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public string Key { get; set; }
    }
    public static class ProfileTransformers
    {
        public static ProfileQueryEntity ToProfileDataQueryEntity(this ProfileQueryMsg contract)
        {
            ProfileQueryEntity entity = null;

            if (contract != null)
            {
                entity = new ProfileQueryEntity
                             {
                                 ApplicationId = contract.ApplicationId,
                                 ProfileType = contract.ProfileType, 
                                 InUser = contract.InUser,
                             };
            }

            return entity;
        }

        public static List<UserProfile> ToUserProfileEntities(this List<ProfileEntity> entities)
        {
            var result = new List<UserProfile>();

            foreach (var profileEntity in entities)
            {
                result.Add(new UserProfile
                               {
                                   ApplicationId = profileEntity.ApplicationId,
                                   Key = profileEntity.ProfileType,
                                   InUser = profileEntity.InUser,
                                   Data = profileEntity.ProfileValue
                               });
            }

            return result;
        }
    }
}
