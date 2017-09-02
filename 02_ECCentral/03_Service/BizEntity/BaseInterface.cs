using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity
{
    public interface IIdentity
    {
        int? SysNo { get; set; }
    }

    public interface ICompany
    {
        string CompanyCode { get; set; }
    }

    public interface ILanguage
    {
        string LanguageCode { get; set; }
    }

    public interface IWebChannel : ICompany
    {
        WebChannel WebChannel { get; set; }
    }

    public interface IMarketingPlace : ICompany
    {
        Merchant Merchant { get; set; }
    }
}
