using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility; 
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess 
{
    [VersionExport(typeof(IGrossMarginProcessorDA))]
    public class GrossMarginProcessorDA : IGrossMarginProcessorDA
    {
        public DataTable GetSaleGiftCurrentGiftProductsForVendor(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSaleGiftCurrentGiftProductsForVendor");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            return dt;
        }

        public DataTable GetSaleGiftGiftProductsExcludeFull(int productSysNo, int saleGiftSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSaleGiftGiftProductsExcludeFull");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SaleGiftSysNo", saleGiftSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            return dt;
        }

        public List<int> GetCurrentCouponsForPM(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCurrentCouponsForPM");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            List<int> list = new List<int>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(int.Parse(dt.Rows[i][0].ToString()));
                }
            }
            return list;
        }

        public decimal GetCouponAmountForPM(int productSysNo, int couponSysNo)
        {
            throw new NotImplementedException();
        }
    }
}
