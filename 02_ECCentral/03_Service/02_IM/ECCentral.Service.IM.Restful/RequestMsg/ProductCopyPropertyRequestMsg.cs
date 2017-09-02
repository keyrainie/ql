using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.Restful.RequestMsg
{
    public class ProductCopyPropertyRequestMsg : ICompany, ILanguage
    {
        public bool CanOverrite { get; set; }

        public int? SourceProductSysNo { get; set; }

        public int? TargetProductSysNo { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }

        public UserInfo OperationUser { get; set; }
    }
}
