using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(UnicomContractPhoneProcessor))]
    public class UnicomContractPhoneProcessor
    {
        private readonly IUnicomContractPhoneDA _categoryDA = ObjectFactory<IUnicomContractPhoneDA>.Instance;


        /// <summary>
        /// 创建新的相关类别
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateUnicomContractPhoneNumberStatus(string phone, UnicomContractPhoneNumberStatus status, UserInfo operationUser)
        {
            if (string.IsNullOrEmpty(phone))
                throw new BizException("phone is null Or empty");
            if (operationUser == null)
                throw new BizException("operationUser is null");

            _categoryDA.UpdateUnicomContractPhoneNumberStatus(phone.Trim(), status, operationUser);
        }
    }
}
