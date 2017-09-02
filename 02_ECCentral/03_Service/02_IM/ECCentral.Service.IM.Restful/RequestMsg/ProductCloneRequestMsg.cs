using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class ProductCloneRequestMsg : ICompany,ILanguage
    {
        public List<String> ProductIDList { get; set; }

        public ProductCloneType CloneType { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }

        public UserInfo OperateUser { get; set; }
    }
}
