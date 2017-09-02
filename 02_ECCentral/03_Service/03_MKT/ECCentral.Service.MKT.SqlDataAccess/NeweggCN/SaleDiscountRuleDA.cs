using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ISaleDiscountRuleDA))]
    public class SaleDiscountRuleDA : ISaleDiscountRuleDA
    {
        #region ISaleDiscountRuleDA Members

        public SaleDiscountRule Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_Load");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<SaleDiscountRule>();
        }

        public void Insert(SaleDiscountRule data)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_Insert");

            cmd.SetParameterValue(data);

            cmd.ExecuteNonQuery();
            data.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        public void Update(SaleDiscountRule data)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_Update");

            cmd.SetParameterValue(data);

            cmd.ExecuteNonQuery();
        }

        //限定分类
        public bool CheckExistsProductScope_Category(int excludeSysNo, int c3SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_CheckExistsProductScope_Category");

            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            return cmd.ExecuteScalar<bool>();
        }

        //限定品牌
        public bool CheckExistsProductScope_Brand(int excludeSysNo, int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_CheckExistsProductScope_Brand");

            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            return cmd.ExecuteScalar<bool>();
        }

        //限定分类+品牌
        public bool CheckExistsProductScope_CategoryBrand(int excludeSysNo, int c3SysNo, int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_CheckExistsProductScope_CategoryBrand");

            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@C3SysNo", c3SysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            return cmd.ExecuteScalar<bool>();
        }

        //限定商品(包含商品组的概念)
        public bool CheckExistsProductScope_Product(int excludeSysNo, params int[] productSysNos)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("NewPromotion_SaleDiscountRule_CheckExistsProductScope_Product");

            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.ReplaceParameterValue("#ProductSysNos#", string.Join(",", productSysNos));
            return cmd.ExecuteScalar<bool>();
        }

        //获取所有有效的销售立减规则
        public List<SaleDiscountRule> GetAllValid()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Promotion_SaleDiscountRule_GetAllValid");

            return cmd.ExecuteEntityList<SaleDiscountRule>();
        }

        #endregion
    }
}
