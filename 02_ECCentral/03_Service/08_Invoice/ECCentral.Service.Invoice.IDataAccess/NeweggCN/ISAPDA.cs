using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.SAP;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface ISAPDA
    {
        void UpdateIPPUser(SAPIPPUserInfo entity);

        int CreateSAPVendor(SAPVendorInfo entity);

        int CheckSAPStock(int stockID, string companyCode);

        int CheckSAPCoCode(int stockID, string sapCoCode, string companyCode);

        SAPCompanyInfo GetSAPCompanyInfoByStockID(int stockID, string companyCode);

        void CreateSAPCompany(SAPCompanyInfo entity);

        void UpdateSAPCompanyWorkStatus(int stockID, string companyCode);

        int CheckPayTypeForSAPIPPUser(int payTypeSysNo, string companyCode);

        void CreateSAPIPPUser(SAPIPPUserInfo entity);
    }
}
