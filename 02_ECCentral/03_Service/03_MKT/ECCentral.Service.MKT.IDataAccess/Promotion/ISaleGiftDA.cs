using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.Promotion
{
    public interface ISaleGiftDA
    {
        SaleGiftInfo Load(int? sysNo);

        List<SaleGiftInfo> LoadAllRunSaleGiftList();

        int? CreateMaster(SaleGiftInfo info);

        void UpdateMaster(SaleGiftInfo info);
                
        void UpdateStatus(int sysNo, SaleGiftStatus status, string userName);

        void UpdateGiftIsGlobal(int promotionSysNo, bool isGlobal, string userName);

        void DeleteSaleRules(int promotionSysNo);
        void CreateSaleRules(int promotionSysNo, SaleGift_RuleSetting setting);
        void CreateGloableSaleRules(int promotionSysNo, SaleGiftInfo info);

        void DeleteGiftItemRules(int promotionSysNo);
        void CreateGiftItemRules(int promotionSysNo, RelProductAndQty setting);

        void UpdateGiftItemCount(int promotionSysNo, SaleGiftGiftItemType giftComboType, int? itemGiftCount, string userName);

        bool CheckExistMultiSameGiftItem(int productSysNo);

        bool ProductIsGift(int productSysNo);

        bool CheckMarketIsActivity(int productSysNo);

        List<SaleGiftInfo> GetGiftInfoListByProductSysNo(int productSysNo, SaleGiftStatus status);

        List<SaleGiftInfo> GetGiftItemListByProductSysNo(int productSysNo, SaleGiftStatus status);

        List<ProductPromotionDiscountInfo> GetGiftAmount(int productSysNo);

        void SyncGiftStatus(int requestSysNo, SaleGiftStatus status);

        List<RelVendor> GetGiftVendorList();

        int GetVendorSysNoByProductSysNo(int productsysno);
    }
}
