using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    /// <summary>
    /// 业务操作用户查询条件
    /// </summary>
    [Serializable]
    [DataContract]
    public class BizOperationUserQueryFilter
    {
        [DataMember]
        public string BizTableName
        {
            get;
            set;
        }

        [DataMember]
        public BizOperationUserType? UserType
        {
            get;
            set;
        }

        [DataMember]
        public BizOperationUserStatus? UserStatus
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
    }
}