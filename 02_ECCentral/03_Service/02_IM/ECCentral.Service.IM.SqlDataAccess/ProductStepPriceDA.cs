using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.IM.SqlDataAccess
{
    public class ProductStepPriceDA
    {
        public static DataTable GetProductStepPrice(int? vendorSysNo, int? productSysno, int? pageIndex, int? pageSize, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductStepPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysno);
            cmd.SetParameterValue("@MerchantSysNo", vendorSysNo);
            cmd.SetParameterValue("@EndNumber", (pageIndex + 1) * pageSize);
            cmd.SetParameterValue("@StartNumber", pageIndex * pageSize);
            //cmd.SetParameterValue("@TotalCount", 0);
            DataTable dt = cmd.ExecuteDataTable();
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        public static List<ProductStepPriceInfo> GetProductStepPricebyProductSysNo(int productSysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductStepPricebyProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysno);
            return  cmd.ExecuteEntityList<ProductStepPriceInfo>();

        }

        public static int DeleteProductStepPrice(List<int> sysNos)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductStepPrice");
            cmd.SetParameterValue("@TempStr", string.Join(",", sysNos));
            return cmd.ExecuteNonQuery();
        }

        public static int CreateProductStepPrice(ProductStepPriceInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductStepPrice");
            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@BaseCount", entity.BaseCount);
            cmd.SetParameterValue("@TopCount", entity.TopCount);
            cmd.SetParameterValue("@StepPrice", entity.StepPrice);
            cmd.SetParameterValue("@InUser", entity.InUser);
            return cmd.ExecuteNonQuery();
        }

        public static int UpdateProductStepPrice(ProductStepPriceInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductStepPrice");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@BaseCount", entity.BaseCount);
            cmd.SetParameterValue("@TopCount", entity.TopCount);
            cmd.SetParameterValue("@StepPrice", entity.StepPrice);
            cmd.SetParameterValue("@EditUser", entity.InUser);
            return cmd.ExecuteNonQuery();
        }
    }
}

