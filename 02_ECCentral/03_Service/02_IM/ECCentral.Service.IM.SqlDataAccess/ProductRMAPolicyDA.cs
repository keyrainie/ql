using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
     [VersionExport(typeof(IProductRMAPolicyDA))]
   public class ProductRMAPolicyDA:IProductRMAPolicyDA
    {
        #region IProductRMAPolicyDA Members

        public List<ProductRMAPolicyInfo> GetProductRMAPolicyList(string productSysNos)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRMAPolicyList");
            cmd.SetParameterValue("@productSysNos", productSysNos);
            List<ProductRMAPolicyInfo> data = cmd.ExecuteEntityList<ProductRMAPolicyInfo>();
            return data;
        }

        #endregion

        #region IProductRMAPolicyDA Members


        public ProductRMAPolicyInfo GetProductRMAPolicyByProductSysNo(int? productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductRMAPolicyByProductSysNo");
            cmd.SetParameterValue("@productSysNo", productSysNo);
            ProductRMAPolicyInfo info = cmd.ExecuteEntity<ProductRMAPolicyInfo>();
            return info;
        }

    

        #endregion

        #region IProductRMAPolicyDA Members


        public void CreateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductRMAPolicy");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            cmd.SetParameterValue("@RMAPolicySysNo", info.RMAPolicyMasterSysNo);
            cmd.SetParameterValue("@IsBrandWarranty", info.IsBrandWarranty);
            cmd.SetParameterValue("@WarrantyDay", info.WarrantyDay);
            cmd.SetParameterValue("@WarrantyDesc", info.WarrantyDesc);
            cmd.SetParameterValue("@InUser", info.User.UserDisplayName);
            cmd.SetParameterValue("@EditUser", info.User.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region IProductRMAPolicyDA Members


        public void UpdateProductRMAPolicy(ProductRMAPolicyInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductRMAPolicy");
            cmd.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            cmd.SetParameterValue("@RMAPolicySysNo", info.RMAPolicyMasterSysNo);
            cmd.SetParameterValue("@IsBrandWarranty", info.IsBrandWarranty);
            cmd.SetParameterValue("@WarrantyDay", info.WarrantyDay);
            cmd.SetParameterValue("@WarrantyDesc", info.WarrantyDesc);
            cmd.SetParameterValue("@InUser", info.User.UserDisplayName);
            cmd.SetParameterValue("@EditUser", info.User.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
