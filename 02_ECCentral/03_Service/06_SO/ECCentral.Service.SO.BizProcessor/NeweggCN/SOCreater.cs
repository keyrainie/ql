using System;
using System.Linq;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.BizEntity.Common;
using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity;

namespace ECCentral.Service.SO.BizProcessor
{
    #region 特殊订单处理

    #region SIM卡订单

    [VersionExport(typeof(SOAction), new string[] { "SIM", "Update" })]
    public class SIMCardSOUpdater : SOUpdater
    {
        /// <summary>
        /// 订单主信息持久化
        /// </summary>
        public override void SOPersistence()
        {
            // 持久化订单主体信息
            SODA.PersintenceMaster(this.CurrentSO, true);

            // 持久化订单商品信息
            SODA.PersintenceItem(this.CurrentSO, true);

            // 持久化订单促销信息
            SODA.PersintencePromotion(this.CurrentSO, true);

            // 持久化订单礼品卡信息
            SODA.PersintenceGiftCard(this.CurrentSO, true);

            //持久化订单其他相关信息
            SODA.PersintenceExtend(this.CurrentSO, true);

            //更新订单SIM卡信息
            SODA.UpdateSOSIMCardOrContractPhoneInfo(this.CurrentSO);
        }
    }
    #endregion

    #region 联通合约机订单

    [VersionExport(typeof(SOAction), new string[] { "ContractPhone", "Update" })]
    public class ContractPhoneSOUpdater : SOUpdater
    {
        /// <summary>
        /// 订单主信息持久化
        /// </summary>
        public override void SOPersistence()
        {
            // 持久化订单主体信息
            SODA.PersintenceMaster(this.CurrentSO, true);

            // 持久化订单商品信息
            SODA.PersintenceItem(this.CurrentSO, true);

            // 持久化订单促销信息
            SODA.PersintencePromotion(this.CurrentSO, true);

            // 持久化订单礼品卡信息
            SODA.PersintenceGiftCard(this.CurrentSO, true);

            //持久化订单其他相关信息
            SODA.PersintenceExtend(this.CurrentSO, true);

            //更新订单SIM卡信息
            SODA.UpdateSOSIMCardOrContractPhoneInfo(this.CurrentSO);
        }
    }

    #endregion

    #region 联通0元购机订单

    [VersionExport(typeof(SOAction), new string[] { "UnicomFreeBuy", "Update" })]
    public class UnicomFreeBuySOUpdater : SOUpdater
    {
        /// <summary>
        /// 订单主信息持久化
        /// </summary>
        public override void SOPersistence()
        {
            // 持久化订单主体信息
            SODA.PersintenceMaster(this.CurrentSO, true);

            // 持久化订单商品信息
            SODA.PersintenceItem(this.CurrentSO, true);

            // 持久化订单促销信息
            SODA.PersintencePromotion(this.CurrentSO, true);

            // 持久化订单礼品卡信息
            SODA.PersintenceGiftCard(this.CurrentSO, true);

            //持久化订单其他相关信息
            SODA.PersintenceExtend(this.CurrentSO, true);

            //更新订单SIM卡信息
            SODA.UpdateSOSIMCardOrContractPhoneInfo(this.CurrentSO);
        }
    }

    #endregion 
    [VersionExport(typeof(SOAction), new string[] { "BuyMobileSettlement", "Create" })]
    public class BuyMobileSettlementSOCreater : SOCreater
    {
        private int MasterSOSysNo 
        {
            get
            {
                return CurrentSO.BaseInfo.ReferenceSysNo.Value;
            }
        }
        private SOInfo MasterSOInfo
        {
            get
            {
                return SODA.GetSOBySOSysNo(MasterSOSysNo);
            }
        }
        protected override void LoadData()
        {
            SetSOInfo();
            base.LoadData();
        }

        private void SetSOInfo()
        {
            CurrentSO.BaseInfo.CustomerSysNo = AppSettingHelper.NEG_BuyMobileSettlementSO_CustomerSysNo; //14610683;
            CurrentSO.BaseInfo.PayTypeSysNo = AppSettingHelper.NEG_BuyMobileSettlementSO_PayTypeSysNo;
            CurrentSO.BaseInfo.ManualShipPrice = 0;
            CurrentSO.BaseInfo.IsLarge = false;
            CurrentSO.BaseInfo.SOType = SOType.BuyMobileSettlement;
            CurrentSO.BaseInfo.SpecialSOType = SpecialSOType.Normal;
            CurrentSO.BaseInfo.ReferenceSysNo = MasterSOSysNo;//-----------------------------------
            //配送方式
            CurrentSO.ShippingInfo.ShipTypeSysNo = 21;
            CurrentSO.ShippingInfo.DeliveryDate = DateTime.Now.Date;
            CurrentSO.ShippingInfo.DeliveryTimeRange = 2;
            CurrentSO.InvoiceInfo.IsVAT = true;
            //获取地区编号
            CurrentSO.ReceiverInfo.Address = "上海市浦东新区浦东大道900号";
            CurrentSO.ReceiverInfo.AreaSysNo = 3355;
            CurrentSO.ReceiverInfo.MobilePhone = "61201818";
            CurrentSO.ReceiverInfo.Name = "中国联合网络通信有限公司上海市分公司";
            CurrentSO.ReceiverInfo.Phone = "61201818";
            CurrentSO.ReceiverInfo.Zip = "200135";
            CurrentSO.InvoiceInfo.Header = "中国联合网络通信有限公司上海市分公司";
            SetVATInfo();
            SetItems();
            CurrentSO.CompanyCode = MasterSOInfo.CompanyCode;
            CurrentSO.WebChannel = MasterSOInfo.WebChannel;
        }

        private List<SOItemInfo> SetItems()
        {
            //           SELECT TOP 1 
            //pp.AcountPrice AS Price
            //,si.ProductSysNo
            //,si.Quantity
            //,si.BriefName
            //,si.WarehouseNumber
            //,W.StockName as WarehouseName
            //,P.Category3SysNo AS C3SysNo
            //,CASE WHEN si.ProductType<>4 THEN P.ProductID ELSE '' END AS ProductID
            //FROM ipp3.dbo.SO_Master sm WITH(nolock)
            //INNER JOIN ipp3.dbo.SO_Item si WITH(nolock)
            //ON sm.SysNo=si.SOSysNo
            //INNER JOIN OverseaContentManagement.dbo.V_CM_ItemBasicInfo P WITH(nolock) 
            //on P.SysNo=si.ProductSysNo 
            //INNER JOIN OverseaInventoryManagement.dbo.V_INM_Stock W WITH(nolock) 
            //on W.SysNo = si.WarehouseNumber
            //INNER JOIN OverseaContentManagement.dbo.Unicom_ContractPhone_ContractB_Price pp WITH(nolock)
            //ON si.ProductSysNo=pp.PhoneProductSysno
            //WHERE 
            //sm.SysNo=@SOSysNO
            //AND sm.OrderDate>=pp.StartDate
            //AND sm.OrderDate<pp.EndDate 

            //上面是取得订单商品的SQL
#warning 创建联通合约机补偿单，取得订单商品未实现
            SOItemInfo phoneItem = null;

            if (phoneItem == null)
                return null;

            List<SOItemInfo> itemList = new List<SOItemInfo>();

            SOItemInfo item = new SOItemInfo();
            item.ProductSysNo = phoneItem.ProductSysNo;
            item.StockSysNo = phoneItem.StockSysNo;
            //item.StockName = phoneItem.StockName;
            //item.ProductID =
            //item.C1SysNo =
            //item.C2SysNo =
            item.C3SysNo = phoneItem.C3SysNo;
            item.ProductID = phoneItem.ProductID;
            //item.Cost =
            //item.Weight =0;
            item.PromotionAmount = 0;
            //item.OnlineQty =
            item.OriginalPrice = phoneItem.Price;
            item.PriceType = SOProductPriceType.Normal;
            item.Price = phoneItem.Price;
            item.ProductName = phoneItem.ProductName;
            item.Quantity = phoneItem.Quantity;
            itemList.Add(item);
            return itemList;

        }

        private void SetVATInfo()
        {
            SOVATInvoiceInfo vat = new SOVATInvoiceInfo();
            vat.BankAccount = "中国银行上海市卢湾支行442959219683";
            vat.CompanyAddress = "上海市浦东新区浦东大道900号";
            vat.CompanyName = "中国联合网络通信有限公司上海市分公司";
            vat.CompanyPhone = "61201818";
            vat.CustomerSysNo = AppSettingHelper.NEG_BuyMobileSettlementSO_CustomerSysNo;
            vat.ExistTaxpayerCertificate = false;
            vat.Memo = "";
            vat.SOSysNo = CurrentSO.SysNo.Value;
            vat.TaxNumber = "310105X07320269 ";

            CurrentSO.InvoiceInfo.VATInvoiceInfo = vat;
        }
    }

    [VersionExport(typeof(SOAction), new string[] { "BuyMobileSettlement", "Update" })]
    public class BuyMobileSettlementSOUpdater:SOAction
    {
        public override void Do()
        {
            throw new BizException("合约机结算订单无法更新");
        }
    }
    #endregion
}
