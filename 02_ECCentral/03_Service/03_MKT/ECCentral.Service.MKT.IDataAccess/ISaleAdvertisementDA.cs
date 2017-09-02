using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ISaleAdvertisementDA
    {
        SaleAdvertisement CreateSaleAdv(SaleAdvertisement entity);

        void UpdateSaleAdv(SaleAdvertisement entity);

        void UpdateIsHoldSaleAdvBySysNo(SaleAdvertisement entity);

        SaleAdvertisement LoadBySysNo(int sysNo);

        List<SaleAdvertisementItem> GetSaleAdvItems(int saleAdvSysNo);

        SaleAdvertisementItem LoadSaleAdvItemBySysNo(int sysNo);

        bool CheckSaleAdvItemDuplicate(SaleAdvertisementItem entity);       

        SaleAdvertisementItem CreateItem(SaleAdvertisementItem entity);

        void UpdateItem(SaleAdvertisementItem entity);

        void UpdateItemStatus(SaleAdvertisementItem entity);

        void DeleteItem(int sysNo);

        void CreateSaleAdvItemLog(SaleAdvertisementItem entity, string action);

        SaleAdvertisementGroup LoadSaleAdvGroupBySysNo(int sysNo);

        SaleAdvertisementGroup CreateSaleAdvGroup(SaleAdvertisementGroup entity);

        void UpdateSaleAdvGroup(SaleAdvertisementGroup entity);

        void DeleteSaleAdvGroup(int sysNo);

        List<SaleAdvertisementGroup> LoadSaleAdvGroupsBySaleAdvSysNo(int saleAdvSysNo);
    }
}
