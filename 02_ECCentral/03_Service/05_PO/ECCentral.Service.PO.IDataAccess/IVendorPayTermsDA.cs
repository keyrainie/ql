using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IVendorPayTermsDA
    {
        /// <summary>
        /// 获取商家所有账期类型(Invoice):
        /// </summary>
        /// <returns></returns>
        List<VendorPayTermsItemInfo> GetAllVendorPayTerms(string companyCode);

        /// <summary>
        /// 根据系统编号获取账期
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        VendorPayTermsItemInfo GetVendorPayTermsInfoBySysNo(int sysNo);
    }
}
