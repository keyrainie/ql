using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductPriceCompareDA))]
    public class ProductPriceCompareDA : IProductPriceCompareDA
    {
        public ProductPriceCompareEntity Load(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ProductPriceCompare_Load");

            dc.SetParameterValue("@SysNo", sysNo);

            return dc.ExecuteEntity<ProductPriceCompareEntity>();
        }


        //价格举报有效
        public void UpdateProductPriceCompareValid(ProductPriceCompareEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductPriceCompareValid");

            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@DisplayLinkStatus", entity.DisplayLinkStatus);
            dc.SetParameterValueAsCurrentUserSysNo("@AuditorSysNo");
            dc.ExecuteNonQuery();
        }

        //价格举报无效
        public void UpdateProductPriceCompareInvalid(ProductPriceCompareEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductPriceCompareInvalid");

            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@InvalidReason", entity.InvalidReason);
            dc.SetParameterValueAsCurrentUserSysNo("@AuditorSysNo");
            dc.ExecuteNonQuery();
        }

        //价格举报恢复
        public void UpdateProductPriceCompareResetLinkShow(ProductPriceCompareEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateProductPriceCompareResetlinkshow");

            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@DisplayLinkStatus", entity.DisplayLinkStatus);
            dc.SetParameterValueAsCurrentUserSysNo("@SetLinkShowPMSysNo");
            dc.ExecuteNonQuery();
        }
    }
}
