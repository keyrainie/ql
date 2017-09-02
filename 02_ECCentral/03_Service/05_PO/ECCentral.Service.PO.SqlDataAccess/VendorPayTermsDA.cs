using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IVendorPayTermsDA))]
    public class VendorPayTermsDA : IVendorPayTermsDA
    {
        #region IVendorPayTermsDA Members

        public List<VendorPayTermsItemInfo> GetAllVendorPayTerms(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllVendorPayTermsAndNoUse");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<VendorPayTermsItemInfo>();
        }

        public VendorPayTermsItemInfo GetVendorPayTermsInfoBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorPayTermDescBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<VendorPayTermsItemInfo>();
        }
        #endregion
    }
}
