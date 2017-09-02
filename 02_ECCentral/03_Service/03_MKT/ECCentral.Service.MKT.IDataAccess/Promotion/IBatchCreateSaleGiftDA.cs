using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.IDataAccess.Promotion
{
    public interface IBatchCreateSaleGiftDA
    {

        int CheckGiftRulesForVendor(int? productSysNo,bool isVendor,string CompanyCode);

        void CreateGiftRules(ProductItemInfo productItem, BatchCreateGiftRuleInfo ruleInfo);

        void UpdateItemGiftCouontGiftRules(int promotionSysNo, int? count, SaleGiftGiftItemType isGiftPool, string companyCode, string username, int special);

        

        void DeleteSaleRules(string promotionSysNo);

        int CheckIsVendorGift(int? productSysNo, string companyCode);


        void CreateSaleRules(BatchCreateSaleGiftSaleRuleInfo info, ProductItemInfo entity);

        void UpdateGiftIsGlobal(int promotionSysNo, string isGlobal, string companyCode, string user);

    }
}
