using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(CountdownAppService))]
    public class CountdownAppService
    {
        public BizEntity.MKT.CountdownInfo Load(int id)
        {
            return ObjectFactory<CountdownProcessor>.Instance.Load(id);
        }

        public CountdownInfo CreateCountdown(BizEntity.MKT.CountdownInfo info)
        {
            return ObjectFactory<CountdownProcessor>.Instance.CreateCountdown(info);
        }

        public CountdownInfo UpdateCountdown(BizEntity.MKT.CountdownInfo info)
        {
            return ObjectFactory<CountdownProcessor>.Instance.UpdateCountdown(info);
        }

        public void VerifyCountdown(BizEntity.MKT.CountdownInfo info)
        {
            ObjectFactory<CountdownProcessor>.Instance.VerifyCountdown(info);
        }

        public CountdownInfo AbandonCountdown(CountdownInfo info)
        {
            return ObjectFactory<CountdownProcessor>.Instance.AbandonCountdown(info);
        }

        public CountdownInfo InterruptCountdown(CountdownInfo info)
        {
            return ObjectFactory<CountdownProcessor>.Instance.InterruptCountdown(info);
        }

        public BizEntity.IM.ProductInfo GetProductInfo(int id)
        {
            return ExternalDomainBroker.GetProductInfo(id);
        }

        public BizEntity.Inventory.ProductInventoryInfo GetProductTotalInventoryInfo(int id)
        {
            return ExternalDomainBroker.GetProductTotalInventoryInfo(id);
        }

        public DateTime? GetLastPoDate(int id)
        {
            return ExternalDomainBroker.GetLastPoDate(id);
        }

        public List<CodeNamePair> GetQuickTimes()
        {
            return ObjectFactory<CountdownProcessor>.Instance.GetQuickTimeList();
        }

        public void GetGrossMargin(CountdownInfo entity, out decimal GrossMargin, out decimal GrossMarginWithOutPointAndGift, out decimal GrossMarginRate, out decimal GrossMarginRateWithOutPointAndGift)
        {
            ObjectFactory<CountdownProcessor>.Instance.GetGrossMargin(entity, out GrossMargin, out GrossMarginWithOutPointAndGift, out GrossMarginRate, out GrossMarginRateWithOutPointAndGift);
        }

        public BizEntity.Inventory.ProductSalesTrendInfo GetProductTotalSalesTrendInfo(int id)
        {
            return ExternalDomainBroker.GetProductSalesTrendInfoTotal(id);
        }

        public decimal GetProductCurrentMarginRate(BizEntity.IM.ProductInfo product)
        {
            return ObjectFactory<GrossMarginProcessor>.Instance.GetSalesGrossMarginRate(product);
        }

        public string CheckOptionalAccessoriesInfoMsg(CountdownInfo info)
        {
            return ObjectFactory<CountdownProcessor>.Instance.CheckOptionalAccessoriesInfoMsg(info);
        }

        public List<BizEntity.Common.UserInfo> GetAllCountdownCreateUser(string channleID)
        {
            return ObjectFactory<CountdownProcessor>.Instance.GetAllCountdownCreateUser(channleID);
        }

        public List<ECCentral.BizEntity.PO.VendorInfo> GetCountdownVendorList()
        {
            return ObjectFactory<CountdownProcessor>.Instance.GetCountdownVendorList();
        }
        public void ImportCountInfo(string fileName, int pmRole, bool IsPromotionSchedule)
        {
            ObjectFactory<CountdownProcessor>.Instance.ImportCountInfo(fileName, pmRole, IsPromotionSchedule);
        }
    }
}
