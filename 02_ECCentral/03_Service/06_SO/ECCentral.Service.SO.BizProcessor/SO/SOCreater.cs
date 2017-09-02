using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.Text.RegularExpressions;
//using ECCentral.Service.ThirdPart.Interface;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.SO.BizProcessor
{


    #region 正常订单

    /// <summary>
    /// 创建订单
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Create" })]
    public class SOCreater : SOAction
    {
        public ISODA SODA = ObjectFactory<ISODA>.Instance;
        public ISOKFCDA SOKFCDA = ObjectFactory<ISOKFCDA>.Instance;
        public ISOItemDA SOItemDA = ObjectFactory<ISOItemDA>.Instance;
        public override void Do()
        {
            //加载数据
            LoadData();

            //规则检查
            RulesCheck();

            TransactionOptions options = new TransactionOptions();
            //options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //价格运算与应用
                Run();
                scope.Complete();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected virtual void LoadData()
        {
            var itemSysNos = this.CurrentSO.Items.Where(item => item.ProductType != SOProductType.Coupon
                                                        && item.ProductType != SOProductType.ExtendWarranty)
                                                 .Select(item => item.ProductSysNo.Value)
                                                 .ToList();
            //填充商品列表
            List<ProductInfo> itemList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(itemSysNos);
            //填充商品入境设置列表
            List<ProductEntryInfo> productEntryList = ExternalDomainBroker.GetProductEntryInfoListByProductSysNoList(itemSysNos);

#warning 延保信息填充需要IM提供延保接口

            foreach (SOItemInfo item in this.CurrentSO.Items)
            {
                //延保特殊处理
                if (item.ProductType == SOProductType.ExtendWarranty)
                {

                }
                else if (item.ProductType != SOProductType.Coupon && itemList != null && itemList.Count > 0)
                {
                    ProductInfo product = itemList.Find(p => p.SysNo == item.ProductSysNo);
                    ProductEntryInfo productEntryInfo = productEntryList.Find(p => p.ProductSysNo.Value == item.ProductSysNo.Value);
                    if (product != null)
                    {
                        item.ProductSysNo = product.SysNo;
                        item.ProductID = product.ProductID;
                        item.ProductName = product.ProductName;
#warning C3 进一步验证一下
                        item.C3SysNo = Convert.ToInt32(product.ProductBasicInfo.ProductCategoryInfo.CategoryID);
                        item.Weight = (int)product.ProductBasicInfo.ProductDimensionInfo.Weight;
#warning Warranty进一步验证一下
                        item.Warranty = product.ProductWarrantyInfo.Warranty.Content;
                        //商品库存模式
                        item.InventoryType = product.InventoryType;
                        if (productEntryInfo != null)
                        {
                            item.StoreType = productEntryInfo.StoreType;
                            item.EntryRecord = productEntryInfo.EntryCode;
                            item.Tariffcode = productEntryInfo.TariffCode;
                            item.TariffRate = productEntryInfo.TariffRate;
                        }
                        else
                        {
                            item.StoreType = StoreType.Narmal;
                        }
                    }
                    else
                    {
                        BizExceptionHelper.Throw("SO_Creater_SOItemInfoError");
                    }
                }
            }
        }

        /// <summary>
        /// 订单业务规则检查与校验
        /// </summary>
        public virtual void RulesCheck()
        {

            #region 实体完整性校验
            if (this.CurrentSO == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOInfoError");
            }

            //主体信息完整性检测
            if (this.CurrentSO.BaseInfo == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOBaseInfoError");
            }

            //商品信息完整性
            if (this.CurrentSO.Items == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOItemError");
            }

            //配送信息完整性
            if (this.CurrentSO.ShippingInfo == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOShippingInfoError");
            }

            //收货信息完整性
            if (this.CurrentSO.BaseInfo.SOType != SOType.ElectronicCard)
            {
                if (this.CurrentSO.ReceiverInfo == null)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOReceiveInfoError");
                }
            }

            //支付方式必须有效
            if (this.CurrentSO.BaseInfo.PayTypeSysNo == 0)
            {
                BizExceptionHelper.Throw("SO_Creater_SOPayTypeError");
            }

            if (this.CurrentSO.BaseInfo.CustomerSysNo == 0)
            {
                BizExceptionHelper.Throw("SO_Creater_SOCustomerInfoError");
            }

            //增值税发票信息被勾中,增票信息不能为空
            if (this.CurrentSO.InvoiceInfo.IsVAT.Value
                && this.CurrentSO.InvoiceInfo.VATInvoiceInfo.BankAccount == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOVATInfoError");
            }

            #endregion

            #region 优惠券逻辑校验收
            if (this.CurrentSO.SOPromotions != null
                && this.CurrentSO.SOPromotions.Count > 0
                && this.CurrentSO.SOPromotions.Exists(item => item.PromotionType == SOPromotionType.Coupon))
            {
                //团购订单不能使用优惠券
                if (this.CurrentSO.BaseInfo.SOType == SOType.GroupBuy)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOUSePromotionError");
                }

                //是否存在优惠券和积分同时使用的权限
                if (this.CurrentSO.BaseInfo.PointPay > 0)
                {
                    List<CustomerRight> rights = ExternalDomainBroker.GetCustomerRight(this.CurrentSO.BaseInfo.CustomerSysNo.Value);
                    if (rights == null
                        || rights.Count == 0
                        || !rights.Exists(item => item.Right == 5))
                    {
                        BizExceptionHelper.Throw("SO_Creater_SOUsePointAndPromotionError");
                    }
                }
            }
            #endregion

            //配送方式与支付方式匹配性校验
            if (!SOCommon.ValidatePayTypeAndShipType(this.CurrentSO))
            {
                BizExceptionHelper.Throw("SO_Creater_SOPayTypeAndShipTypeError");
            }

            //货到付款禁运规则检查
            if (this.CurrentSO.BaseInfo.PayWhenReceived.Value)
            {
                //获取货到付款禁运C3类别列表
                List<int> EmbargoC3s = SOCommon.GetEmbargoC3s(this.CurrentSO.BaseInfo.CompanyCode);

                //获取货到付款禁运商品编号列表
                List<int> EmbargoProducts = SOCommon.GetEmbargoProducts(this.CurrentSO.BaseInfo.CompanyCode);

                List<string> EmbargoPIDs = new List<string>();

                //检查订单中是否含有禁运的C3类别商品或禁运商品
                this.CurrentSO.Items.ForEach(item =>
                {
                    if (EmbargoC3s.Contains(item.C3SysNo > 0 ? item.C3SysNo.Value : 0)
                        || EmbargoProducts.Contains(item.ProductSysNo.Value))
                    {
                        EmbargoPIDs.Add(item.ProductID);
                    }

                });

                if (EmbargoPIDs.Count > 0)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOPayTypeSelectError", string.Join(",", EmbargoPIDs.ToArray()));
                }
            }

            //商品限定配送方式规则检查
            ValidateItemShipRule();

            //礼品卡规则检查
            ValidateGiftCard();

            //检查团购订单
            CheckGroupBuyProducts();

            #region 检查单个订单不拆单条件

            var productItems = this.CurrentSO.Items.Where(item => item.ProductType == SOProductType.Accessory
                                                 || item.ProductType == SOProductType.Award
                                                 || item.ProductType == SOProductType.Gift
                                                 || item.ProductType == SOProductType.Product
                                                 || item.ProductType == SOProductType.SelfGift);

            //检查是否是同一个商家的商品
            List<ProductInfo> productInfoList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productItems.Select(x => x.ProductSysNo.Value).ToList());
            var merchantGroupProduct = productInfoList.GroupBy(g => g.Merchant.SysNo.Value);
            if (merchantGroupProduct.Count() > 1)
            {
                BizExceptionHelper.Throw("SO_Create_MerchantSame_Error");
            }
            this.CurrentSO.Merchant = new Merchant()
            {
                MerchantID = merchantGroupProduct.First().Key,
                SysNo = merchantGroupProduct.First().Key
            };

            if (productItems.Any(x => x.StockSysNo.Value != this.CurrentSO.ShippingInfo.LocalWHSysNo.Value))
            {
                BizExceptionHelper.Throw("SO_Create_ShipTypeSame_Error");
            }

            var shippingType = ExternalDomainBroker.GetShippingTypeBySysNo(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value);
            if (productItems.Any(x => x.StoreType.Value != shippingType.StoreType.Value))
            {
                BizExceptionHelper.Throw("SO_Create_StoreTypeSame_Error");
            }

            if (productItems.Sum(item => item.ProductType==SOProductType.Product ? (item.Quantity.HasValue?item.Quantity.Value:1):0) > 1)
            {
                //检查存储运输方式
                var storeType = productItems.First().StoreType;
                if (productItems.Any(x => x.StoreType.Value != storeType.Value))
                {
                    BizExceptionHelper.Throw("SO_Create_StoreTypeSame_Error");
                }

                //检查金额限制
                var productItemsPriceSum = productItems.Sum(item => item.ProductType == SOProductType.Product ? (item.OriginalPrice.Value * item.Quantity.Value) : 0m);

                var warehouseInfo = ExternalDomainBroker.GetWarehouseInfo(this.CurrentSO.ShippingInfo.LocalWHSysNo.Value);

                if (warehouseInfo.CountryCode.ToUpper() == "HK")
                {
                    if (productItemsPriceSum > 800m)
                    {
                        BizExceptionHelper.Throw("SO_Create_ItemPrice800_Error");
                    }
                }
                else if (warehouseInfo.CountryCode.ToUpper() == "JP")
                {
                    if (productItemsPriceSum > 1000m)
                    {
                        BizExceptionHelper.Throw("SO_Create_ItemPrice1000_Error");
                    }
                }
            }

            #endregion

            //检查库存模式
            //CheckInventoryType();
        }
        //库存模式校验
        //protected virtual void CheckInventoryType()
        //{
        //    //库存模式兼容列表
        //    Dictionary<string,int> adapter = new Dictionary<string,int>();
        //    adapter.Add("0,1,2",1);
        //    adapter.Add("3",2);
        //    adapter.Add("4",3);
        //    adapter.Add("5",4);
        //    adapter.Add("6",5);           
        //    List<int> result = new List<int>();
        //    var invtenoryGp = this.CurrentSO.Items.GroupBy(p=>(int)p.InventoryType).TakeWhile(g=>g.Count()>0);         
        //    foreach (var g in invtenoryGp)
        //    {
        //        string find=g.Key.ToString();
        //        var temp=adapter.Where(p=>p.Key.IndexOf(find)!=-1);
        //        if (temp.Count() > 0)
        //        {
        //            if (!result.Contains(temp.FirstOrDefault().Value))
        //            {
        //                result.Add(temp.FirstOrDefault().Value);
        //            }                  
        //        }               
        //    }
        //    if (result.Count>1)
        //    {
        //        BizExceptionHelper.Throw("SO_Create_InventoryTypeError"); 
        //    }

        //    var erpPrds = this.CurrentSO.Items.Where(p => p.InventoryType != ProductInventoryType.Normal);
        //    ERPItemInventoryInfo info = new ERPItemInventoryInfo();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var item in erpPrds)
        //    {
        //        info=ObjectFactory<IAdjustERPInventory>.Instance.GetERPItemInventoryInfoByProductSysNo(item.ProductSysNo.Value);
        //        switch (item.InventoryType)
        //        {
        //            case ProductInventoryType.Normal:
        //                break;
        //            case ProductInventoryType.GetShopInventory:
        //                if (item.Quantity>info.DeptQuantity-info.B2CSalesQuantity)
        //                {
        //                    sb.AppendFormat("商品{0}的下单数量大于ERP门店可用库存:{1}>{2}", item.ProductID, item.Quantity, info.DeptQuantity - info.B2CSalesQuantity);
        //                }
        //                break;
        //            case ProductInventoryType.GetShopUnInventory:
        //                break;
        //            case ProductInventoryType.Company:
        //                if (item.Quantity > info.HQQuantity - info.B2CSalesQuantity)
        //                {
        //                    sb.AppendFormat("商品{0}的下单数量大于ERP总部可用库存:{1}>{2}", item.ProductID, item.Quantity, info.HQQuantity - info.B2CSalesQuantity);
        //                }
        //                break;
        //            case ProductInventoryType.Factory:
        //                break;
        //            case ProductInventoryType.TwoDoor:
        //                if (item.Quantity > info.HQQuantity - info.B2CSalesQuantity)
        //                {
        //                    sb.AppendFormat("商品{0}的下单数量大于ERP总部可用库存:{1}>{2}", item.ProductID, item.Quantity, info.HQQuantity - info.B2CSalesQuantity);
        //                }
        //                break;
        //            case ProductInventoryType.Merchent:
        //                break;
        //            default:
        //                break;
        //        }              
        //    }
        //    if (sb.Length > 0)
        //    {
        //        throw new BizException(sb.ToString());
        //    }

        //}       

        /// <summary>
        /// 检查团购订单
        /// </summary>
        protected virtual void CheckGroupBuyProducts()
        {
            //筛选团购商品
            var itemList = this.CurrentSO.Items.Where(x => x.ProductType == SOProductType.Product
                || x.ProductType == SOProductType.Gift
                || x.ProductType == SOProductType.Award
                || x.ProductType == SOProductType.Accessory
                || x.ProductType == SOProductType.SelfGift
                ).Select(x => x).ToList();

            if (itemList.Count > 0)
            {
                if (this.CurrentSO.BaseInfo.SOType == SOType.GroupBuy)
                {
                    //团购处理逻辑已经迁移至 GroupBuyNewSOUpdater 类中  见下方
                }
                else
                {
                    //非团购订单判断
                    List<int> productsOnGroupBuying = ExternalDomainBroker.GetProductsOnGroupBuying(itemList.Select(p => p.ProductSysNo.Value));

                    //没有就过
                    if (productsOnGroupBuying.Count == 0)
                        return;
                    ValidateGroupBuyProductsError(productsOnGroupBuying);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productsOnGroupBuying"></param>
        protected virtual void ValidateGroupBuyProductsError(List<int> productsOnGroupBuying)
        {
            string errorProductIDS = string.Join(",", productsOnGroupBuying);
            BizExceptionHelper.Throw("SO_Create_NotAllowCreateGroupBuySOInBackPart", errorProductIDS);
        }


        /// <summary>
        /// 商品限定配送方式检查
        /// </summary>
        protected virtual void ValidateItemShipRule()
        {
            //不符合规则的商品
            List<string> erroProductSysno = new List<string>();

            //配送区域信息
            AreaInfo area = ExternalDomainBroker.GetAreaInfoByDistrictSysNo(this.CurrentSO.ReceiverInfo.AreaSysNo.Value);

            //非配送地址
            var areaUnShipTypeList = ExternalDomainBroker.GetShipTypeAreaUnList(this.CurrentSO.CompanyCode);
            if (areaUnShipTypeList.Exists(p => (
                                                p.AreaSysNo == area.ProvinceSysNo
                                                || p.AreaSysNo == area.CitySysNo
                                                || p.AreaSysNo == area.DistrictSysNo
                                               )
                                               && p.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo
                                         ))
            {
                BizExceptionHelper.Throw("SO_Creater_SOShipTypeNotInAreaError");
            }

            //配送方式信息
            ShippingType shipTypeInfo = ExternalDomainBroker.GetShippingTypeBySysNo(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value);

            //筛选商品
            List<SOItemInfo> itemList = this.CurrentSO.Items.Where(x => x.ProductType == SOProductType.Product
                || x.ProductType == SOProductType.Gift
                || x.ProductType == SOProductType.SelfGift
                || x.ProductType == SOProductType.Accessory
                || x.ProductType == SOProductType.Award
                ).Select(x => x).ToList();

            //专用配送方式检查
            if (shipTypeInfo.IsSpecified.HasValue && shipTypeInfo.IsSpecified.Value == IsSpecial.Yes)
            {
                //专用方式列表
                List<ItemShipRuleInfo> specialRules = ExternalDomainBroker.GetSpecialItemShipRule(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value, this.CurrentSO.BaseInfo.CompanyCode);
                if (specialRules != null && specialRules.Count > 0)
                {
                    //对比每个Item
                    foreach (SOItemInfo item in itemList)
                    {
                        int warehouseNumber = item.StockSysNo == null ? 0 : item.StockSysNo.Value;
                        bool isPass = false;
                        if (specialRules.Exists(x => x.ItemRange == "P"
                            && x.ItemSysNo == item.ProductSysNo
                            && (x.StockSysNo == warehouseNumber || x.StockSysNo == null)
                            && (x.AreaSysNo == area.SysNo || x.AreaSysNo == area.CitySysNo || x.AreaSysNo == area.ProvinceSysNo || x.AreaSysNo == null)
                            ))
                        {
                            isPass = true;
                        }

                        if (specialRules.Exists(x => x.ItemRange == "C"
                            && x.ItemSysNo == item.C3SysNo
                            && (x.StockSysNo == warehouseNumber || x.StockSysNo == null)
                            && (x.AreaSysNo == area.SysNo || x.AreaSysNo == area.CitySysNo || x.AreaSysNo == area.ProvinceSysNo || x.AreaSysNo == null)
                            ))
                        { isPass = true; }

                        if (!isPass)
                        {
                            //不符合专用配送方式
                            erroProductSysno.Add(item.ProductID);
                        }
                    }
                }
                else
                {
                    //不符合专用配送方式
                    BizExceptionHelper.Throw("SO_Creater_SORuleError");
                }
            }
            List<ItemShipRuleInfo> rules = null;
            string productSysNoStr = string.Join(",", itemList.Select(x => x.ProductSysNo.ToString()).ToArray());
            string c3SysNoStr = string.Join(",", itemList.Select(x => x.C3SysNo == null ? "0" : x.C3SysNo.ToString()).ToArray());
            if (!string.IsNullOrEmpty(productSysNoStr) && !string.IsNullOrEmpty(c3SysNoStr))
            {
                rules = ExternalDomainBroker.GetItemShipRuleList(c3SysNoStr, productSysNoStr, area.ProvinceSysNo, area.CitySysNo, area.SysNo, this.CurrentSO.CompanyCode);
            }
            if (rules != null && rules.Count > 0)
            {
                //规则检查
                foreach (SOItemInfo item in itemList)
                {
                    int warehouseNumber = item.StockSysNo == null ? 0 : item.StockSysNo.Value;

                    List<ItemShipRuleInfo> productRules = rules.Where(x => x.ItemRange == "P"
                        && x.ItemSysNo == item.ProductSysNo
                        && (x.StockSysNo == warehouseNumber || x.StockSysNo == null))
                        .Select(x => x)
                        .ToList();

                    // 商品规则检查
                    if (productRules != null && productRules.Count > 0)
                    {
                        //检查禁运
                        if (productRules.Exists(x => x.Type == "D"
                            && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                        {
                            erroProductSysno.Add(item.ProductID);//符合禁运规则提示
                            continue;
                        }

                        //检查必选
                        if (productRules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.SysNo))
                        {
                            if (!productRules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.SysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (productRules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.CitySysNo))
                        {
                            if (!productRules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.CitySysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (productRules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.ProvinceSysNo))
                        {
                            if (!productRules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.ProvinceSysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (productRules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == null))
                        {

                            if (!productRules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == null
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }

                        if (productRules.Exists(x => x.Type == "E"))
                        {
                            if (!productRules.Exists(x => x.Type == "E"
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                    }

                    List<ItemShipRuleInfo> c3Rules = rules.Where(x => x.ItemRange == "C"
                        && x.ItemSysNo == item.C3SysNo
                        && (x.StockSysNo == warehouseNumber || x.StockSysNo == null))
                        .Select(x => x)
                        .ToList();

                    //商品类别规则检查
                    if (c3Rules != null && c3Rules.Count > 0)
                    {
                        //检查禁运
                        if (c3Rules.Exists(x => x.Type == "D"
                            && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                        {
                            erroProductSysno.Add(item.ProductID);//符合禁运规则提示
                            continue;
                        }

                        //检查必选
                        if (c3Rules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.SysNo))
                        {
                            if (!c3Rules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.SysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (c3Rules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.CitySysNo))
                        {
                            if (!c3Rules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.CitySysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (c3Rules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == area.ProvinceSysNo))
                        {
                            if (!c3Rules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == area.ProvinceSysNo
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                        else if (c3Rules.Exists(x => x.Type == "E"
                            && x.AreaSysNo == null))
                        {
                            if (!c3Rules.Exists(x => x.Type == "E"
                                && x.AreaSysNo == null
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }

                        if (c3Rules.Exists(x => x.Type == "E"))
                        {
                            if (!c3Rules.Exists(x => x.Type == "E"
                                && x.ShipTypeSysNo == this.CurrentSO.ShippingInfo.ShipTypeSysNo))
                            {
                                //不符合
                                erroProductSysno.Add(item.ProductID);
                                continue;
                            }
                        }
                    }
                }

                if (erroProductSysno.Count > 0)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOShipTypeNotMathRuleError"
                        , string.Join(",", erroProductSysno.ToArray()));
                }
            }
        }

        //礼品卡检查(暂时综合判断，是否需要提出，再议)
        /// <summary>
        /// 礼品卡检查
        /// </summary>
        private void ValidateGiftCard()
        {
            #region 礼品卡订单规则检测
            bool isECard = false;
            bool isPCard = false;
            bool isNormal = false;

            this.CurrentSO.Items.ForEach(item =>
            {
                if (item.ProductID.StartsWith("GC-001-"))
                    isECard = true;
                else
                    if (item.ProductID.StartsWith("GC-002-"))
                        isPCard = true;
                    else isNormal = true;
            });

            if (isECard || isPCard)
            {
                if ((this.CurrentSO.BaseInfo.SOType != SOType.ElectronicCard) && (this.CurrentSO.BaseInfo.SOType != SOType.PhysicalCard))
                {
                    BizExceptionHelper.Throw("SO_Creater_SONotExistsGiftCardType");
                }
            }

            if (isECard && isPCard)
            {
                BizExceptionHelper.Throw("SO_Creater_SOExistsECardAndPCard");
            }

            if ((isECard && isNormal) || (isPCard && isNormal))
            {
                BizExceptionHelper.Throw("SO_Creater_SOExistsCardAndNormal");
            }

            // 4 电子卡订单--5 实物卡订单
            if (this.CurrentSO.BaseInfo.SOType == SOType.ElectronicCard
                || this.CurrentSO.BaseInfo.SOType == SOType.PhysicalCard)
            {
                if (this.CurrentSO.InvoiceInfo.IsVAT.HasValue)
                    if (this.CurrentSO.InvoiceInfo.IsVAT.Value)
                    {
                        BizExceptionHelper.Throw("SO_Creater_SOCanNotUseVATCode");
                    }

                if ((this.CurrentSO.BaseInfo.PointPay.HasValue
                         && this.CurrentSO.BaseInfo.PointPay.Value > 0)
                        || (!string.IsNullOrEmpty(this.CurrentSO.CouponCode))
                        || (this.CurrentSO.BaseInfo.IsUseGiftCard.HasValue
                            && this.CurrentSO.BaseInfo.IsUseGiftCard.Value)
                    )
                {
                    BizExceptionHelper.Throw("SO_Creater_SOGiftOnlyUserCash");
                }

                //判断是否是款到发货
                var isPayWhenReceived = ObjectFactory<SOProcessor>.Instance.IsPayWhenReceived(this.CurrentSO.BaseInfo.PayTypeSysNo.Value);
                if (isPayWhenReceived)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOGiftNotPayWhenRecv");
                }
            }

            #endregion
        }

        /// <summary>
        /// 价格运算与应用
        /// </summary>
        protected virtual void Run()
        {
            // 预处理
            PreProcess();

            // 分仓
            AllocateStock();

            // 计算价格
            Calculate();

            // 价格业务检验
            CostCheck();

            //设置客户类型(可疑、欺诈)
            KFCLogic();

            // 调仓
            AdjustInventory();

            // 持久化订单信息
            SOPersistence();

            // 调整积分，余额，礼品卡,帐期
            SOCostAdjust();

            // 发送邮件
            SendMessage();

            // 记日志
            WriteLog();
        }

        /// <summary>
        /// 记订单日志
        /// </summary>
        public virtual void WriteLog()
        {
            new SOLogProcessor().WriteSOLog(BizLogType.Sale_SO_Create
                , this.CurrentSO.SysNo.Value
                , "创建订单");
        }

        /// <summary>
        /// 订单数据预处理
        /// </summary>
        protected virtual void PreProcess()
        {
            // 计算订单重量
            this.CurrentSO.ShippingInfo.Weight = SOCommon.GetSOWeight(this.CurrentSO);

            // 计算订单包裹费
            this.CurrentSO.ShippingInfo.PackageFee = SOCommon.GetSOPackageFee(this.CurrentSO);

            // 订单状态设为待审核
            this.CurrentSO.BaseInfo.Status = SOStatus.Origin;

            #region 预约配送优化

            var shipType = ExternalDomainBroker.GetShippingTypeBySysNo(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value);
            switch (shipType.DeliveryType)
            {
                case ShipDeliveryType.OneDayOnce:
                    this.CurrentSO.ShippingInfo.DeliveryTimeRange = 0;
                    this.CurrentSO.ShippingInfo.DeliverySection = "一日一送";
                    this.CurrentSO.ShippingInfo.RingOutShipType = null;
                    break;
                case ShipDeliveryType.OneDayTwice:
                    this.CurrentSO.ShippingInfo.RingOutShipType = null;
                    switch (this.CurrentSO.ShippingInfo.DeliveryTimeRange)
                    {
                        case 1:
                            this.CurrentSO.ShippingInfo.DeliverySection = "上午";
                            break;
                        case 2:
                            this.CurrentSO.ShippingInfo.DeliverySection = "下午";
                            break;
                        default:
                            break;
                    }
                    break;
                case ShipDeliveryType.EveryDay:
                    this.CurrentSO.ShippingInfo.DeliveryDate = null;
                    switch (this.CurrentSO.ShippingInfo.RingOutShipType)
                    {
                        case "A":
                            this.CurrentSO.ShippingInfo.DeliverySection = "工作日和双休日均可配送";
                            this.CurrentSO.ShippingInfo.DeliveryTimeRange = 0;
                            break;
                        case "N":
                            this.CurrentSO.ShippingInfo.DeliverySection = "工作日配送";
                            this.CurrentSO.ShippingInfo.DeliveryTimeRange = 1;
                            break;
                        case "W":
                            this.CurrentSO.ShippingInfo.DeliverySection = "双休日配送";
                            this.CurrentSO.ShippingInfo.DeliveryTimeRange = 2;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    this.CurrentSO.ShippingInfo.DeliveryTimeRange = 0;
                    this.CurrentSO.ShippingInfo.DeliverySection = "";
                    this.CurrentSO.ShippingInfo.DeliveryDate = null;
                    this.CurrentSO.ShippingInfo.RingOutShipType = null;
                    break;
            }

            this.CurrentSO.ShippingInfo.DeliveryType = shipType.DeliveryType;

            #endregion

#warning 赠品主商品关联性
        }

        /// <summary>
        /// 计算订单金额
        /// </summary>
        public virtual void Calculate()
        {
            SOCaculator cacler = new SOCaculator();
            cacler.IsUpdate = false;
            cacler.Calculate(this.CurrentSO);

            //毛利放这里是因为Calculate没有是否只是计算的属性
            CalculateItemGrossProfit(this.CurrentSO);
        }


        public virtual void CalculateItemGrossProfit(SOInfo soInfo)
        {
            //赠品毛利分摊
            ItemGrossProfitActionType actionType = GetActionType();

            //获取商品
            List<int> itemSysNos = soInfo.Items.Where(item => item.ProductType == SOProductType.Accessory || item.ProductType == SOProductType.Gift)
                                               .Select<SOItemInfo, int>(item => item.ProductSysNo.Value).ToList<int>();
            //检查附件
            if (soInfo.SOPromotions == null || itemSysNos == null || itemSysNos.Count == 0)
                return;
            //商品信息
            List<ProductInfo> productInfoList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(itemSysNos);

            List<ItemGrossProfitInfo> grossProfitList = new List<ItemGrossProfitInfo>();
            //查找厂商赠品和附件
            List<SOPromotionInfo> promotions = soInfo.SOPromotions.FindAll(x => x.PromotionType == SOPromotionType.Accessory || x.PromotionType == SOPromotionType.VendorGift);
            //遍历附件Promoitons
            foreach (SOPromotionInfo promotion in promotions)
            {
                string disCountType = SOItemDA.GetDisCountTypeByPromotionSysNo(promotion.PromotionSysNo.Value);

                if (string.IsNullOrEmpty(disCountType) || disCountType == "G")
                    continue;


                // 有赠品分摊到主商品
                if (disCountType == "M" && promotion.GiftList != null)
                {
                    foreach (SOPromotionInfo.GiftInfo gift in promotion.GiftList)
                    {
                        if (gift.Quantity > 0 && soInfo.Items.Exists(x => x.ProductSysNo == gift.ProductSysNo))
                        {
                            //赠品商品信息信息
                            ProductInfo productInfo = productInfoList.Find(x => (x.SysNo == gift.ProductSysNo));

                            //没有价格信息的就跳过
                            if (productInfo == null
                                || productInfo.ProductPriceInfo == null
                                || !productInfo.ProductPriceInfo.CurrentPrice.HasValue)
                                continue;

                            //主商品 没主商品就跳过
                            if (promotion.MasterList == null || !soInfo.Items.Exists(x => x.ProductSysNo == promotion.MasterList[0].ProductSysNo))
                                continue;

                            int masterProductSysNo = promotion.MasterList[0].ProductSysNo;

                            decimal currentPrice = productInfo.ProductPriceInfo.CurrentPrice.Value;
                            int qty = gift.Quantity;
                            decimal DisCount = qty * currentPrice;



                            AddItemGrossProfit(grossProfitList, new ItemGrossProfitInfo
                            {
                                ProductSysNo = gift.ProductSysNo
                                ,
                                ActionType = actionType
                                ,
                                DisCount = -DisCount
                                ,
                                GrossProfitType = GrossProfitType.Normal
                                ,
                                Quantity = qty
                                ,
                                SOSysNo = soInfo.BaseInfo.SysNo.Value
                                ,
                                Status = ECCentral.BizEntity.SO.ValidStatus.Active
                            });

                            AddItemGrossProfit(grossProfitList, new ItemGrossProfitInfo
                            {
                                ProductSysNo = masterProductSysNo
                                ,
                                ActionType = actionType
                                ,
                                DisCount = DisCount
                                ,
                                GrossProfitType = GrossProfitType.Normal
                                ,
                                Quantity = qty
                                ,
                                SOSysNo = soInfo.BaseInfo.SysNo.Value
                                ,
                                Status = ECCentral.BizEntity.SO.ValidStatus.Active
                            });
                        }
                    }
                }
            }


            soInfo.ItemGrossProfitList = grossProfitList;

        }
        //Hook
        protected virtual ItemGrossProfitActionType GetActionType()
        {
            return ItemGrossProfitActionType.CreateSO;
        }
        private void AddItemGrossProfit(List<ItemGrossProfitInfo> grossProfitList, ItemGrossProfitInfo gorssProfit)
        {

            ItemGrossProfitInfo repeatedProduct = grossProfitList.Find(x => x.ProductSysNo == gorssProfit.ProductSysNo);

            if (repeatedProduct == null)
            {
                grossProfitList.Add(gorssProfit);
            }
            else
            {
                repeatedProduct.DisCount += gorssProfit.DisCount;
                repeatedProduct.Quantity += gorssProfit.Quantity;
            }
        }

        /// <summary>
        /// 调整积分，余额，礼品卡,帐期
        /// </summary>
        public virtual void SOCostAdjust()
        {
            //调整积分
            if (this.CurrentSO.BaseInfo.PointPay > 0)
            {
                AdjustPointRequest pointAdjust = new AdjustPointRequest();
                pointAdjust.CustomerSysNo = this.CurrentSO.BaseInfo.CustomerSysNo.Value;
                pointAdjust.Memo = "创建订单扣减积分";
                pointAdjust.OperationType = AdjustPointOperationType.AddOrReduce;
                pointAdjust.Point = -this.CurrentSO.BaseInfo.PointPay;
                pointAdjust.PointType = (int)AdjustPointType.CreateOrder;
                pointAdjust.SOSysNo = this.CurrentSO.BaseInfo.SysNo;
                pointAdjust.Source = "OrderMgmt";

                ExternalDomainBroker.AdjustPoint(pointAdjust);
            }

            //调整余额
            if (this.CurrentSO.BaseInfo.IsUsePrePay.Value)
            {
                CustomerPrepayLog prePay = new CustomerPrepayLog();
                prePay.AdjustAmount = -this.CurrentSO.BaseInfo.PrepayAmount.Value;
                prePay.CustomerSysNo = this.CurrentSO.BaseInfo.CustomerSysNo.Value;
                prePay.Note = "创建订单扣减余额";
                prePay.PrepayType = PrepayType.SOPay;
                prePay.SOSysNo = this.CurrentSO.SysNo;

                ExternalDomainBroker.AdjustPrePay(prePay);
            }

            //送礼品
            foreach (SOItemInfo soItem in this.CurrentSO.Items)
            {
                if (soItem.ProductType == SOProductType.SelfGift)
                {
                    ExternalDomainBroker.GetGift(this.CurrentSO.BaseInfo.CustomerSysNo.Value
                                         , soItem.ProductSysNo.Value
                                         , this.CurrentSO.SysNo.Value);
                }
            }

            //使用礼品卡
            if (this.CurrentSO.BaseInfo.IsUseGiftCard == true)
            {
                if (this.CurrentSO.SOGiftCardList != null && this.CurrentSO.SOGiftCardList.Count > 0)
                {
                    foreach (var item in this.CurrentSO.SOGiftCardList)
                    {
                        item.ActionSysNo = this.CurrentSO.BaseInfo.SysNo.Value;
                        item.ActionType = ActionType.SO;
                    }
                    ExternalDomainBroker.GiftCardConsumeForSOCreate(this.CurrentSO.BaseInfo.GiftCardPay.Value, this.CurrentSO.SOGiftCardList, this.CurrentSO.BaseInfo.CompanyCode);
                }
            }
        }

        /// <summary>
        /// 金额相关校验
        /// </summary>
        public virtual void CostCheck()
        {
            // 订单金额不能大于配送方式指定的订单赔付金额上限
            ShippingType st = ExternalDomainBroker.GetShippingTypeBySysNo(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value);
            if (st.CompensationLimit.HasValue
                && st.CompensationLimit.Value != 0
                && this.CurrentSO.BaseInfo.SOAmount > st.CompensationLimit.Value)
            {
                BizExceptionHelper.Throw("SO_Creater_SOAmountNotMathShipTypeRuleError");
            }
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public virtual void KFCLogic()
        {

        }

        /// <summary>
        /// 分配仓库
        /// </summary>
        public virtual void AllocateStock()
        {
            if (this.CurrentSO.BaseInfo.SOType == SOType.ElectronicCard)
            {
                foreach (var item in this.CurrentSO.Items)
                {
                    item.StockSysNo = AppSettingHelper.ElectronicCardDefaultStockSysNo;
                }
                return;
            }

            //往ShoppingMaster．ShoppingTransaction里写信息
            this.CurrentSO.BaseInfo.ShoppingMasterSysNo = SODA.InsertShoppingMaster(this.CurrentSO);
            //往ShoppingTransaction里插入数据
            foreach (SOItemInfo itemInfo in this.CurrentSO.Items)
            {
                //非延保 商品 需要向 ShoppingTransaction 添加记录
                if (itemInfo.ProductType != SOProductType.ExtendWarranty)
                {
                    SODA.InsertShoppingTransaction(itemInfo, this.CurrentSO.BaseInfo.ShoppingMasterSysNo.Value, this.CurrentSO.BaseInfo.CompanyCode);
                }
            }
            AllocateInventoryStatus status;
            // 调用存储过程实现分仓
            List<SOItemInfo> allocatedItemList = SODA.AllocateStock(this.CurrentSO.BaseInfo.ShoppingMasterSysNo.Value
                                                                    , this.CurrentSO.ReceiverInfo.AreaSysNo.Value
                                                                    , this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value
                                                                    , this.CurrentSO.BaseInfo.CompanyCode
                                                                    , out status);
            //分仓结果解析
            switch (status)
            {
                case AllocateInventoryStatus.Failed:
                    BizExceptionHelper.Throw("SO_Creater_SOAllocateStockError");
                    break;
                case AllocateInventoryStatus.Success:
                case AllocateInventoryStatus.SuccessAndUpdated:
                    SetSOStock(this.CurrentSO, allocatedItemList);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置订单仓库
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="allocatedItemList"></param>
        public virtual void SetSOStock(SOInfo soInfo, List<SOItemInfo> allocatedItemList)
        {
            if (allocatedItemList == null || allocatedItemList.Count == 0)
            {
                return;
            }
            foreach (SOItemInfo allocatedItem in allocatedItemList)
            {
                for (int i = soInfo.Items.Count - 1; i >= 0; i--)
                {
                    if (soInfo.Items[i].ProductType == SOProductType.Coupon)
                    {
                        soInfo.Items[i].StockSysNo = 00;
                    }
                    if (allocatedItem.ProductSysNo == soInfo.Items[i].ProductSysNo)
                    {
                        if (allocatedItem.AvailableQty == 0)
                        {
                            soInfo.Items.RemoveAt(i);
                        }
                        else
                        {
                            soInfo.Items[i].Quantity = allocatedItem.AvailableQty;
                            soInfo.Items[i].StockSysNo = allocatedItem.StockSysNo;
                            soInfo.Items[i].StockName = allocatedItem.StockName;
                        }

                        if (allocatedItem.AvailableQty != allocatedItem.ShoppingQty)
                        {
                            string[] strArray = new string[3];
                            strArray[0] = allocatedItem.ProductID;
                            strArray[1] = allocatedItem.ShoppingQty.ToString();
                            strArray[2] = allocatedItem.AvailableQty.ToString();
                            BizExceptionHelper.Throw("SO_Create_QuantityHaveNotEnough", strArray);
                        }
                        break;
                    }
                }
            }

            if (!soInfo.Items.Exists(item => item.ProductType == 0))
            {
                BizExceptionHelper.Throw("SO_Create_SOHaveNotMainItem");
            }
        }

        /// <summary>
        /// 调整库存
        /// </summary>
        public virtual void AdjustInventory()
        {
            //延保与优惠券及泰隆优选模式非0不需要调整库存,
            List<SOItemInfo> soItemList = this.CurrentSO.Items.FindAll(
                item => item.ProductType.HasValue
                && item.ProductType != SOProductType.Coupon
                && item.ProductType != SOProductType.ExtendWarranty
                && (item.InventoryType == ProductInventoryType.Normal
                || item.InventoryType == ProductInventoryType.Merchent
                || item.InventoryType == ProductInventoryType.Factory
                || item.InventoryType == ProductInventoryType.GetShopUnInventory));
            if (this.CurrentSO.BaseInfo.SOType != SOType.ElectronicCard)
            {
                List<InventoryAdjustItemInfo> adjustItemList = new List<InventoryAdjustItemInfo>();
                soItemList.ForEach(x =>
                {
                    adjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.ProductSysNo.Value,
                        AdjustQuantity = x.Quantity.Value,
                        StockSysNo = x.StockSysNo.Value
                    });
                });
                InventoryAdjustContractInfo adjustInfo = new InventoryAdjustContractInfo();
                adjustInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                adjustInfo.AdjustItemList = adjustItemList;
                adjustInfo.ReferenceSysNo = this.CurrentSO.BaseInfo.SysNo.ToString();
                adjustInfo.SourceActionName = InventoryAdjustSourceAction.Create;
                adjustInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;

                //调整订单商品对应的库存
                ExternalDomainBroker.AdjustProductInventory(adjustInfo);
                List<Int32> productSysNoList = new List<int>();
                foreach (var item in adjustItemList)
                {
                    productSysNoList.Add(item.ProductSysNo);
                }
                List<ProductInventoryInfo> pInventoryInfoList = ExternalDomainBroker.GetProductTotalInventoryInfoByProductList(productSysNoList);
                if (pInventoryInfoList != null && pInventoryInfoList.Count > 0)
                {
                    foreach (var item in this.CurrentSO.Items)
                    {
                        foreach (var itemInventory in pInventoryInfoList)
                        {
                            if (item.ProductSysNo == itemInventory.ProductSysNo)
                            {
                                item.OnlineQty = (itemInventory.AvailableQty + itemInventory.ConsignQty + itemInventory.VirtualQty);
                                break;
                            }
                        }
                    }
                }
            }
            AdjustERPInventory();
        }

        public virtual void AdjustERPInventory()
        {
            //门店库存下单时需要扣ERP库存
            List<SOItemInfo> soItemList = this.CurrentSO.Items.FindAll(
                 item => item.InventoryType == ProductInventoryType.GetShopInventory
                );
            string adjustXml = "";//ERP参数序列化

            //门店库存的商品，调整ERP库存      
            ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo
            {
                OrderSysNo = this.CurrentSO.SysNo.Value,
                OrderType = "SO",
                AdjustItemList = new List<ERPItemInventoryInfo>(),
                Memo = "下单增加销售数量"
            };

            foreach (var item in soItemList)
            {
                ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                {
                    ProductSysNo = item.ProductSysNo,
                    B2CSalesQuantity = item.Quantity.Value
                };

                erpAdjustInfo.AdjustItemList.Add(adjustItem);
            }
            if (erpAdjustInfo.AdjustItemList.Count > 0)
            {
                adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("下单增加销售数量：{0}", adjustXml)
                  , BizLogType.Sale_SO_Create
                  , this.CurrentSO.SysNo.Value
                  , this.CurrentSO.CompanyCode);
                //ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);            

            }
        }
        /// <summary>
        /// 持久化订单信息
        /// </summary>
        public virtual void SOPersistence()
        {
            // 持久化订单主体信息
            SODA.PersintenceMaster(this.CurrentSO, false);

            // 持久化订单商品信息
            SODA.PersintenceItem(this.CurrentSO, false);

            //持久化配送方式
            GenerateSOSplitType();
            SODA.InsertSOCheckShippingInfo(this.CurrentSO);

#warning 原来逻辑在执行Item后有个关于前台页面加载的赋值逻辑（回填数据供前台显示）

            //应用优惠券
            ApplyCoupon(this.CurrentSO);

            // 持久化订单促销信息
            SODA.PersintencePromotion(this.CurrentSO, false);

            //持久化毛利分配信息
            SODA.PersintenceGrossProfit(this.CurrentSO, false);

            // 持久化订单礼品卡信息
            SODA.PersintenceGiftCard(this.CurrentSO, false);

            //持久化订单其他相关信息
            SODA.PersintenceExtend(this.CurrentSO, false);
        }

        /// <summary>
        /// 应用优惠券
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        private void ApplyCoupon(SOInfo soInfo)
        {
            if (soInfo.SOPromotions != null && soInfo.SOPromotions.Count > 0)
            {
                foreach (var item in soInfo.SOPromotions)
                {
                    if (item.PromotionType == SOPromotionType.Coupon)
                    {
                        ExternalDomainBroker.ApplyCoupon(item.PromotionSysNo.Value, soInfo.CouponCode, soInfo.BaseInfo.CustomerSysNo.Value, soInfo.BaseInfo.SysNo.Value, 0, item.SOPromotionDetails[0].DiscountAmount.Value);
                    }
                }
            }
        }

        /// <summary>
        ///  如果订单满足折单条件，给订单打折单标记
        /// </summary>
        public virtual void GenerateSOSplitType()
        {
            // 以旧换新订单，分期付款订单，赠品订单，特殊订单，帐期支付订单不支持拆分（和IPP先保持一致）
            if (this.CurrentSO.BaseInfo.SOType == SOType.Gift
                || this.CurrentSO.BaseInfo.SOType == SOType.ElectronicCard
                || this.CurrentSO.BaseInfo.SpecialSOType == SpecialSOType.TaoBao
                )
            {
                this.CurrentSO.BaseInfo.SplitType = SOSplitType.Normal;//正常单
            }

            //如果商品从多个仓库发货
            if (SOCommon.IsMultiStock(this.CurrentSO.Items))
            {
                //订单配送方式为指定仓发货或存在商品接受预定 折单类型为选择拆单
                if (SODA.IsOnlyForStockShipType(this.CurrentSO.ShippingInfo.ShipTypeSysNo.Value))
                {
                    if (SODA.IsSameCityStock(this.CurrentSO.SysNo.Value, this.CurrentSO.CompanyCode))
                    {
                        this.CurrentSO.BaseInfo.SplitType = SOSplitType.Force;
                    }
                    else
                    {
                        this.CurrentSO.BaseInfo.SplitType = SOSplitType.Customer;
                    }

                    var gifts = CurrentSO.Items
                        .Where(item => item.ProductType == SOProductType.Gift
                            || item.ProductType == SOProductType.Award
                            || item.ProductType == SOProductType.Accessory
                            || item.ProductType == SOProductType.SelfGift
                            )
                        .ToList();

                    var products = CurrentSO.Items.Where(item => item.ProductType == SOProductType.Product)
                        .ToList();

                    gifts.ForEach(gift =>
                    {
                        if (!string.IsNullOrEmpty(gift.MasterProductSysNo))
                        {
                            products.ForEach(product =>
                            {
                                if (gift.MasterProductSysNo.Split(',').Contains(product.ProductSysNo.ToString())
                                    && gift.StockSysNo != product.StockSysNo
                                    )
                                {
                                    this.CurrentSO.BaseInfo.SplitType = SOSplitType.Normal;
                                    return;
                                }
                            });
                        }
                        else
                        {
#warning 暂时不实现
                            //List<int?> masterProducts = CurrentSO.SOPromotions.FindAll(x => x.ProductSysNo == gift.ProductSysNo).Select(x => x.MasterProductSysNo).ToList();

                            //masterProducts.ForEach(master =>
                            //{
                            //    if (products.Exists(x => x.ProductSysNo == master.Value && x.StockSysNo != gift.StockSysNo))
                            //    {
                            //        this.CurrentSO.BaseInfo.SplitType = SOSplitType.Normal;
                            //        return;
                            //    }
                            //});

                        }
                    });
                }
                else if (SODA.ExistEngageItem(this.CurrentSO.SysNo.Value, this.CurrentSO.CompanyCode))
                {
                    this.CurrentSO.BaseInfo.SplitType = SOSplitType.Customer;
                }
                else
                {
                    //正常单
                    this.CurrentSO.BaseInfo.SplitType = SOSplitType.Normal;
                }
            }
            else
            {
                //正常单
                this.CurrentSO.BaseInfo.SplitType = SOSplitType.Normal;
            }
        }

        /// <summary>
        /// 发送邮件及短信
        /// </summary>
        public virtual void SendMessage()
        {
            SOSendMessageProcessor soMail = new SOSendMessageProcessor();
            soMail.SOCreatedSendEmailToCustomer(this.CurrentSO);
        }
    }

    /// <summary>
    /// 修改订单
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Update" })]
    public class SOUpdater : SOCreater
    {
        public IInvoiceBizInteract InvoiceBiz = ObjectFactory<IInvoiceBizInteract>.Instance;

        //更新订单需要比较新旧订单之前的差异
        private SOInfo m_OriginalInfo;
        protected SOInfo OriginalInfo
        {
            get
            {
                if (this.CurrentSO != null)
                {
                    if (m_OriginalInfo == null || m_OriginalInfo.SysNo != this.CurrentSO.SysNo)
                    {
                        m_OriginalInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(this.CurrentSO.SysNo.Value);
                    }
                }
                return m_OriginalInfo;
            }
            set
            {
                m_OriginalInfo = value;
            }
        }

        /// <summary>
        /// 计算订单金额
        /// </summary>
        public override void Calculate()
        {
            SOCaculator cacler = new SOCaculator();
            cacler.IsUpdate = true;
            cacler.OriginalSOInfo = this.OriginalInfo;
            cacler.Calculate(this.CurrentSO);

            //毛利放这里是因为Calculate没有是否只是计算的属性
            //作废原先分配日志
            AbandonGrossProfit(OriginalInfo);
            base.CalculateItemGrossProfit(this.CurrentSO);

        }

        protected virtual void AbandonGrossProfit(SOInfo soInfo)
        {
            SOItemDA.ChangedGossLogStatus(soInfo.SysNo.Value, "D");
        }



        /// <summary>
        /// 订单业务规则检查与校验
        /// </summary>
        public override void RulesCheck()
        {
            //检查订单是否被前台锁定
            SOHolder holder = new SOHolder();
            holder.CurrentSO = this.CurrentSO;
            holder.CheckSOIsWebHold();

            if (OriginalInfo.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF
                && OriginalInfo.InvoiceInfo.InvoiceType == InvoiceType.MET
                && OriginalInfo.ShippingInfo.StockType == ECCentral.BizEntity.Invoice.StockType.SELF)
            {
                BizExceptionHelper.Throw("SO_Update_Model4NotUpdate");
            }


            //订单不是待审核状态,不允许修改
            if (OriginalInfo.BaseInfo.Status != SOStatus.Origin)
            {
                BizExceptionHelper.Throw("SO_Creater_SOStatusError");
            }

            //增值税发票信息被勾中,增票信息不能为空
            if (this.CurrentSO.InvoiceInfo.IsVAT.Value
                && this.CurrentSO.InvoiceInfo.VATInvoiceInfo.BankAccount == null)
            {
                BizExceptionHelper.Throw("SO_Creater_SOVATInfoError");
            }

            if (OriginalInfo.SOPromotions != null
                && OriginalInfo.SOPromotions.Exists(x => x.PromotionType == SOPromotionType.Combo
                && x.ReferenceType == 3))
            {
                BizExceptionHelper.Throw("SO_Update_SuiXinSONotUpdate");
            }

            base.RulesCheck();
        }

        protected override void ValidateGroupBuyProductsError(List<int> productsOnGroupBuying)
        {
            this.OriginalInfo.Items.ForEach(o =>
            {
                productsOnGroupBuying.ForEach(p =>
                {
                    if (o.ProductSysNo == p)
                    {
                        productsOnGroupBuying.Remove(p);
                    }
                });
            });

            if (productsOnGroupBuying.Count > 0)
            {
                base.ValidateGroupBuyProductsError(productsOnGroupBuying);
            }
        }

        /// <summary>
        /// 客户类型更新
        /// </summary>
        public override void KFCLogic()
        {
            if (this.CurrentSO.FPInfo == null || !this.CurrentSO.FPInfo.FPSOType.HasValue)
            {
                return;
            }
            if (this.CurrentSO.FPInfo.FPSOType != OriginalInfo.FPInfo.FPSOType)
            {
                CustomerInfo customerInfo = ExternalDomainBroker.GetCustomerInfo(this.CurrentSO.BaseInfo.CustomerSysNo.Value);
                KnownFraudCustomer kfcEntity = new KnownFraudCustomer();
                kfcEntity.CustomerSysNo = this.CurrentSO.BaseInfo.CustomerSysNo;
                kfcEntity.KFCType = (KFCType)this.CurrentSO.BaseInfo.KFCStatus;
                if (SOKFCDA.GetKFCByCustomerSysNo(this.CurrentSO.BaseInfo.CustomerSysNo.Value) == null
                    && (kfcEntity.KFCType == KFCType.KeYi || kfcEntity.KFCType == KFCType.QiZha)
                    )
                {
                    kfcEntity.CreateDate = DateTime.Now;
                    kfcEntity.CreateUserName = ServiceContext.Current.UserSysNo.ToString();
                    kfcEntity.BrowseInfo = string.Empty;
                    kfcEntity.EmailAddress = customerInfo.BasicInfo.Email;
                    kfcEntity.MobilePhone = this.CurrentSO.ReceiverInfo.MobilePhone;
                    kfcEntity.ShippingAddress = this.CurrentSO.ReceiverInfo.Address;
                    kfcEntity.ShippingContact = this.CurrentSO.ReceiverInfo.Name;
                    kfcEntity.Status = 0;
                    kfcEntity.Telephone = customerInfo.BasicInfo.Phone;
                    ObjectFactory<SOKFCProcessor>.Instance.InsertKnownFraudCustomer(kfcEntity);
                }
                else
                {
                    ObjectFactory<SOKFCProcessor>.Instance.UpdateKnownFraudCustomerStatus(kfcEntity);
                }
            }
        }

        /// <summary>
        /// 调仓
        /// </summary>
        /// <param name="originalInfo"></param>
        public override void AdjustInventory()
        {
            #region 先归还原始单据的库存   在调整现在单据的库存

            //延保与优惠券不需要调整库存 
            List<SOItemInfo> originalSOItemList = OriginalInfo.Items.FindAll(
                item => item.ProductType.HasValue
                && item.ProductType != SOProductType.Coupon
                && item.ProductType != SOProductType.ExtendWarranty
                && (item.InventoryType == ProductInventoryType.Normal
                || item.InventoryType == ProductInventoryType.Merchent
                || item.InventoryType == ProductInventoryType.Factory
                || item.InventoryType == ProductInventoryType.GetShopUnInventory));

            if (OriginalInfo.BaseInfo.SOType != SOType.ElectronicCard)
            {
                List<InventoryAdjustItemInfo> adjustItemList = new List<InventoryAdjustItemInfo>();
                originalSOItemList.ForEach(x =>
                {
                    adjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.ProductSysNo.Value,
                        AdjustQuantity = x.Quantity.Value,
                        StockSysNo = x.StockSysNo.Value
                    });
                });
                InventoryAdjustContractInfo originalAdjustInfo = new InventoryAdjustContractInfo();
                originalAdjustInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                originalAdjustInfo.AdjustItemList = adjustItemList;
                originalAdjustInfo.ReferenceSysNo = OriginalInfo.BaseInfo.SysNo.ToString();
                originalAdjustInfo.SourceActionName = InventoryAdjustSourceAction.Abandon_RecoverStock;
                originalAdjustInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;
                //调整订单商品对应的库存 (归还库存)
                ExternalDomainBroker.AdjustProductInventory(originalAdjustInfo);
            }

            #endregion

            #region   调整现在单据的库存

            //延保与优惠券和泰隆优选非0的模式不需要调整库存
            List<SOItemInfo> soItemList = this.CurrentSO.Items.FindAll(
                item => item.ProductType.HasValue
                && item.ProductType != SOProductType.Coupon
                && item.ProductType != SOProductType.ExtendWarranty
                && (item.InventoryType == ProductInventoryType.Normal
                || item.InventoryType == ProductInventoryType.Merchent
                || item.InventoryType == ProductInventoryType.Factory
                || item.InventoryType == ProductInventoryType.GetShopUnInventory));

            if (this.CurrentSO.BaseInfo.SOType != SOType.ElectronicCard)
            {
                List<InventoryAdjustItemInfo> adjustItemList = new List<InventoryAdjustItemInfo>();
                soItemList.ForEach(x =>
                {
                    adjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.ProductSysNo.Value,
                        AdjustQuantity = x.Quantity.Value,
                        StockSysNo = x.StockSysNo.Value
                    });
                });
                InventoryAdjustContractInfo adjustInfo = new InventoryAdjustContractInfo();
                adjustInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                adjustInfo.AdjustItemList = adjustItemList;
                adjustInfo.ReferenceSysNo = this.CurrentSO.BaseInfo.SysNo.ToString();
                adjustInfo.SourceActionName = InventoryAdjustSourceAction.Create;
                adjustInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;
                //调整订单商品对应的库存

                ExternalDomainBroker.AdjustProductInventory(adjustInfo);
                List<Int32> productSysNoList = new List<int>();
                foreach (var item in adjustItemList)
                {
                    productSysNoList.Add(item.ProductSysNo);
                }
                List<ProductInventoryInfo> pInventoryInfoList = ExternalDomainBroker.GetProductTotalInventoryInfoByProductList(productSysNoList);
                if (pInventoryInfoList != null && pInventoryInfoList.Count > 0)
                {
                    foreach (var item in this.CurrentSO.Items)
                    {
                        foreach (var itemInventory in pInventoryInfoList)
                        {
                            if (item.ProductSysNo == itemInventory.ProductSysNo)
                            {
                                item.OnlineQty = (itemInventory.AvailableQty + itemInventory.ConsignQty + itemInventory.VirtualQty);
                                break;
                            }
                        }
                    }
                }
            }
            AdjustERPInventory();
            #endregion
        }


        public override void AdjustERPInventory()
        {
            List<SOItemInfo> originalSOItemList = OriginalInfo.Items.FindAll(item => item.InventoryType == ProductInventoryType.GetShopInventory);
            string adjustXml = "";//ERP参数序列化  

            ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo
            {
                OrderSysNo = this.CurrentSO.SysNo.Value,
                OrderType = "SO",
                AdjustItemList = new List<ERPItemInventoryInfo>(),
                Memo = "修改订单更改库存"
            };

            foreach (var item in originalSOItemList)
            {
                ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                {
                    ProductSysNo = item.ProductSysNo,
                    B2CSalesQuantity = -item.Quantity.Value
                };

                erpAdjustInfo.AdjustItemList.Add(adjustItem);
            }

            List<SOItemInfo> currentSOItemList = CurrentSO.Items.Where(p => p.InventoryType == ProductInventoryType.GetShopInventory).ToList();
            foreach (var item in currentSOItemList)
            {
                ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                {
                    ProductSysNo = item.ProductSysNo,
                    B2CSalesQuantity = item.Quantity.Value
                };

                erpAdjustInfo.AdjustItemList.Add(adjustItem);
            }

            if (erpAdjustInfo.AdjustItemList.Count > 0)
            {
                adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("修改订单调整ERP库存：{0}", adjustXml)
                , BizLogType.Sale_SO_Create
                , this.CurrentSO.SysNo.Value
                , this.CurrentSO.CompanyCode);
                //ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);
            }
        }

        /// <summary>
        /// 订单金额相关检查
        /// </summary>
        public override void CostCheck()
        {
            // 如果SO的修改涉及到金额变化，并且已生成销售应收款或者网上支付，那么不允许修改
            // 如果SO的修改涉及到余额支付的变化，并且已经生成销售收款单，那么不允许修改
            // 支付方式改变，且改变后与NetPay不一致，不允许修改
            if (OriginalInfo.BaseInfo.PayTypeSysNo != this.CurrentSO.BaseInfo.PayTypeSysNo
                || OriginalInfo.BaseInfo.ReceivableAmount != this.CurrentSO.BaseInfo.ReceivableAmount
                || OriginalInfo.BaseInfo.SOAmount != this.CurrentSO.BaseInfo.SOAmount
                || OriginalInfo.BaseInfo.SOTotalAmount != this.CurrentSO.BaseInfo.SOTotalAmount)
            {
                //存在收款单记录
                if (InvoiceBiz.GetValidSOIncome(this.CurrentSO.SysNo.Value, SOIncomeOrderType.SO) != null)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOIncomeError");
                }

                //存在有效的网上支付
                if (InvoiceBiz.GetSOValidNetPay(this.CurrentSO.SysNo.Value) != null)
                {
                    BizExceptionHelper.Throw("SO_Creater_SONetPayError");
                }

                if (InvoiceBiz.GetValidPostPayBySOSysNo(this.CurrentSO.SysNo.Value) != null)
                {
                    BizExceptionHelper.Throw("SO_Creater_SOPostPayError");
                }
            }

            base.CostCheck();
        }

        /// <summary>
        /// 订单主信息持久化
        /// </summary>
        public override void SOPersistence()
        {
            // 持久化订单主体信息
            SODA.PersintenceMaster(this.CurrentSO, true);

            // 持久化订单商品信息
            SODA.PersintenceItem(this.CurrentSO, true);

            //持久化配送方式
            GenerateSOSplitType();
            SODA.SetSoSplitType(this.CurrentSO);

            // 持久化订单促销信息（赠品、附件、）
            SODA.PersintencePromotion(this.CurrentSO, true);

            //持久化毛利分配信息
            SODA.PersintenceGrossProfit(this.CurrentSO, false);

            //应用优惠券
            ApplyCoupon(this.CurrentSO);

            // 持久化订单礼品卡信息
            SODA.PersintenceGiftCard(this.CurrentSO, true);

            //持久化订单其他相关信息
            SODA.PersintenceExtend(this.CurrentSO, true);
        }

        /// <summary>
        /// 应用优惠券
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        private void ApplyCoupon(SOInfo soInfo)
        {
            //更新时，如果没有修改优惠券，则不应用或取消应用规则
            bool tmpIsSameCouponCode = false;

            #region 取消原始订单使用的优惠券
            if (OriginalInfo.SOPromotions != null && OriginalInfo.SOPromotions.Count > 0)
            {
                List<SOItemInfo> CouponList = OriginalInfo.Items.FindAll(item =>
                {
                    return item.ProductType == SOProductType.Coupon;
                });

                int couponSysNo = CouponList != null && CouponList.Count > 0 ? CouponList[0].ProductSysNo.Value : 0;
                string couponCode = string.Empty;
                bool isThirdPartSO = OriginalInfo.BaseInfo.SpecialSOType != SOConst.SelfSO;

                //更新时，如果没有修改优惠券，则不应用或取消应用规则
                List<SOItemInfo> tmpCouponList = soInfo.Items.FindAll(item =>
                {
                    return item.ProductType == SOProductType.Coupon;
                });
                int tmpCouponSysNo = tmpCouponList != null && tmpCouponList.Count > 0 ? tmpCouponList[0].ProductSysNo.Value : 0;
                if (couponSysNo == tmpCouponSysNo) tmpIsSameCouponCode = true;

                //取消优惠券使用
                if (couponSysNo != 0 && !isThirdPartSO && !tmpIsSameCouponCode)
                {
                    OriginalInfo.BaseInfo.CouponAmount = 0;
                    foreach (SOItemInfo item in OriginalInfo.Items)
                    {
                        item.Price = item.OriginalPrice;
                        item.CouponAverageDiscount = 0;
                    }
                    OriginalInfo.SOPromotions.RemoveAll(p =>
                    {
                        return p.PromotionType == SOPromotionType.Coupon;
                    });
                    ExternalDomainBroker.CheckCouponIsValid(couponSysNo, out couponCode);
                    #region 取消优惠券的使用
                    if (CurrentSO.BaseInfo.SplitType != SOSplitType.SubSO)
                    {
                        int shoppingCartSysNo = SODA.GetShoppingCartSysNoBySOSysNo(OriginalInfo.BaseInfo.SysNo.Value);
                        ExternalDomainBroker.CancelCoupon(couponCode, OriginalInfo.BaseInfo.SysNo.Value, shoppingCartSysNo);
                    }
                    //更新订单商品信息
                    foreach (SOItemInfo item in OriginalInfo.Items)
                    {
                        if (item.ProductType != SOProductType.Coupon)
                        {
                            SODA.UpdateSOItemAmountInfo(item);
                        }
                    }
                    #endregion
                }
            }

            #endregion
            #region 重新使用优惠券

            if (soInfo.SOPromotions != null && soInfo.SOPromotions.Count > 0 && !tmpIsSameCouponCode)
            {
                foreach (var item in soInfo.SOPromotions)
                {
                    if (item.PromotionType == SOPromotionType.Coupon)
                    {
                        ExternalDomainBroker.ApplyCoupon(item.PromotionSysNo.Value, soInfo.CouponCode, soInfo.BaseInfo.CustomerSysNo.Value, soInfo.BaseInfo.SysNo.Value, 0, item.SOPromotionDetails[0].DiscountAmount.Value);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public override void SendMessage()
        {
            //修改订单不发送邮件
        }

        /// <summary>
        /// 调整积分，余额，礼品卡
        /// </summary>
        public override void SOCostAdjust()
        {
            //调整积分
            int point = OriginalInfo.BaseInfo.PointPay.Value - this.CurrentSO.BaseInfo.PointPay.Value;
            if (point != 0)
            {
                AdjustPointRequest pointAdjust = new AdjustPointRequest();
                pointAdjust.CustomerSysNo = this.CurrentSO.BaseInfo.CustomerSysNo.Value;
                pointAdjust.Memo = "更新订单调整积分";
                pointAdjust.OperationType = AdjustPointOperationType.AddOrReduce;
                pointAdjust.Point = point;
                pointAdjust.PointType = (int)AdjustPointType.UpdateSO;
                pointAdjust.SOSysNo = this.CurrentSO.BaseInfo.SysNo;
                pointAdjust.Source = "OrderMgmt";

                ExternalDomainBroker.AdjustPoint(pointAdjust);
            }

            //调整余额
            decimal prePay = OriginalInfo.BaseInfo.PrepayAmount.Value - this.CurrentSO.BaseInfo.PrepayAmount.Value;
            if (prePay != 0)
            {
                CustomerPrepayLog prePayInfo = new CustomerPrepayLog();
                prePayInfo.AdjustAmount = prePay;
                prePayInfo.CustomerSysNo = this.CurrentSO.BaseInfo.CustomerSysNo.Value;
                prePayInfo.Note = "创建订单扣减余额";
                prePayInfo.PrepayType = PrepayType.SOPay;
                prePayInfo.SOSysNo = this.CurrentSO.SysNo;

                ExternalDomainBroker.AdjustPrePay(prePayInfo);
            }

            //送礼品
            foreach (SOItemInfo soItem in this.CurrentSO.Items)
            {
                if (soItem.ProductType == SOProductType.SelfGift)
                {
                    //ExternalDomainBroker.GetGift(this.CurrentSO.BaseInfo.CustomerSysNo.Value
                    //    , soItem.ProductSysNo.Value
                    //    , this.CurrentSO.SysNo.Value);
                    //商家商品不允许修改
                    BizExceptionHelper.Throw("SO_Update_SelfActiveNotUpdate");
                }
            }

            //同步增值税信息
            if (this.CurrentSO.InvoiceInfo.IsVAT.Value)
            {
                if (OriginalInfo.InvoiceInfo != null && OriginalInfo.InvoiceInfo.VATInvoiceInfo != null && OriginalInfo.InvoiceInfo.VATInvoiceInfo.SysNo != 0)
                {
                    //调用Customer提供的同步增票信息功能 更新客户的增值税信息
                    ExternalDomainBroker.SetCustomerValueAddedTax(this.CurrentSO.InvoiceInfo.VATInvoiceInfo.SysNo, this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CustomerSysNo
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.BankAccount
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyName
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyAddress
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyPhone
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.TaxNumber
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.ExistTaxpayerCertificate
                        , true
                        );
                }
                else
                {
                    //调用Customer提供的同步增票信息功能 创建客户的增值税信息
                    ExternalDomainBroker.SetCustomerValueAddedTax(this.CurrentSO.InvoiceInfo.VATInvoiceInfo.SysNo, this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CustomerSysNo
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.BankAccount
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyName
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyAddress
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.CompanyPhone
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.TaxNumber
                        , this.CurrentSO.InvoiceInfo.VATInvoiceInfo.ExistTaxpayerCertificate
                        , false
                        );
                }
            }

            //调整礼品卡  （采用先做废 在新建的方式进行更新）
            if ((this.CurrentSO.BaseInfo.IsUseGiftCard == false && OriginalInfo.BaseInfo.GiftCardPay > 0)
                || this.CurrentSO.BaseInfo.IsUseGiftCard == true)
            {
                //作废原先订单中存在的礼品卡信息
                if (OriginalInfo.BaseInfo.GiftCardPay > 0)
                {
                    List<GiftCardRedeemLog> usedRedeemLog = ExternalDomainBroker.GetSOGiftCardBySOSysNo(OriginalInfo.BaseInfo.SysNo.Value);
                    ExternalDomainBroker.GiftCardVoidForSOUpdate(OriginalInfo.BaseInfo.GiftCardPay.Value, usedRedeemLog, OriginalInfo.BaseInfo.CompanyCode);
                    OriginalInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(this.CurrentSO.SysNo.Value);
                }

                //新建订单中使用礼品卡信息
                if (this.CurrentSO.SOGiftCardList != null && this.CurrentSO.SOGiftCardList.Count > 0)
                {
                    foreach (var item in this.CurrentSO.SOGiftCardList)
                    {
                        item.ActionSysNo = this.CurrentSO.BaseInfo.SysNo.Value;
                        item.ActionType = ActionType.SO;
                    }
                    ExternalDomainBroker.GiftCardConsumeForSOCreate(this.CurrentSO.BaseInfo.GiftCardPay.Value, this.CurrentSO.SOGiftCardList, this.CurrentSO.BaseInfo.CompanyCode);
                }
            }
        }

        /// <summary>
        /// 记订单日志
        /// </summary>
        public override void WriteLog()
        {
            new SOLogProcessor().WriteSOLog(BizLogType.Sale_SO_Create
                , this.CurrentSO.SysNo.Value
                , "更新订单");
        }
    }

    #endregion

    #region 特殊订单处理

    #region 实物卡 订单

    [VersionExport(typeof(SOAction), new string[] { "PhysicalCard", "Create" })]
    public class PhysicalCardSOCreater : SOCreater
    {

    }

    [VersionExport(typeof(SOAction), new string[] { "PhysicalCard", "Update" })]
    public class PhysicalCardSOUpdater : SOUpdater
    {

    }

    #endregion

    #region 电子卡 订单

    [VersionExport(typeof(SOAction), new string[] { "ElectronicCard", "Create" })]
    public class ElectronicCardSOCreater : SOCreater
    {
        public override void AllocateStock()
        {
            //默认为Item列表定义仓库信息
            foreach (var item in this.CurrentSO.Items)
            {
                item.StockSysNo = AppSettingHelper.ElectronicCardDefaultStockSysNo;
            }
            base.AllocateStock();
        }

        protected override void LoadData()
        {
            this.CurrentSO.Items[0].PriceType = SOProductPriceType.Normal;
            //其他不用再次读取
        }

        //电子卡无需检查配送
        protected override void ValidateItemShipRule()
        {

        }
    }

    [VersionExport(typeof(SOAction), new string[] { "ElectronicCard", "Update" })]
    public class ElectronicCardSOUpdater : SOUpdater
    {
        public override void AllocateStock()
        {
            //默认为Item列表定义仓库信息
            foreach (var item in this.CurrentSO.Items)
            {
                item.StockSysNo = AppSettingHelper.ElectronicCardDefaultStockSysNo;
            }
            base.AllocateStock();
        }

        protected override void LoadData()
        {
            this.CurrentSO.Items[0].PriceType = SOProductPriceType.Normal;
            //其他不用再次读取
        }

        //电子卡无需检查配送
        protected override void ValidateItemShipRule()
        {

        }
    }

    #endregion

    #region 赠品订单

    [VersionExport(typeof(SOAction), new string[] { "Gift", "Create" })]
    public class GiftSOCreater : SOCreater
    {
        #region Parameter参数
        /* Parameter说明:
            * Parameter[0] : int? , 主订单编号
            *
            *
            */
        /// <summary>
        /// 主订单编号,由 Parameter[0] 传入,null.
        /// </summary>
        protected int MasterSOSysNo
        {
            get
            {
                return GetParameterByIndex<int>(0, int.MinValue);
            }
        }
        #endregion
        private SOInfo _masterSOInfo;
        private SOInfo MasterSOInfo
        {
            get
            {
                if (MasterSOSysNo > int.MinValue)
                {
                    _masterSOInfo = _masterSOInfo ?? SODA.GetSOBySOSysNo(MasterSOSysNo);
                }
                return _masterSOInfo;
            }
        }

        protected override void LoadData()
        {
            CurrentSO.BaseInfo.PayTypeSysNo = AppSettingHelper.GiftSOPayTypeSysNo;
            CurrentSO.ShippingInfo.ShipTypeSysNo = AppSettingHelper.GiftSOShipTypeSysNo;
            if (MasterSOInfo != null)
            {
                CurrentSO.ReceiverInfo = MasterSOInfo.ReceiverInfo;
                CurrentSO.ReceiverInfo.SOSysNo = CurrentSO.SysNo;
                CurrentSO.InvoiceInfo.Header = MasterSOInfo.InvoiceInfo.Header;
                CurrentSO.InvoiceInfo.IsVAT = false;
                CurrentSO.BaseInfo.IsWholeSale = true;
                CurrentSO.BaseInfo.ManualShipPrice = 0;
                CurrentSO.CompanyCode = MasterSOInfo.CompanyCode;
                CurrentSO.SysNo = CurrentSO.SysNo;
            }
            base.LoadData();
        }
        public override void Calculate()
        {
            base.Calculate();
        }
        public override void WriteLog()
        {
            new SOLogProcessor().WriteSOLog(BizLogType.Sale_SO_Create
                , this.CurrentSO.SysNo.Value
                , "创建赠品订单");
        }

        protected override void Run()
        {
            base.Run();

            ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(new SOLogInfo
            {
                SOSysNo = MasterSOSysNo,
                OperationType = BizLogType.Sale_SO_CreateGiftSO,
                Note = CurrentSO.SysNo.ToString()
            });

            SOInternalMemoInfo memoInfo = new SOInternalMemoInfo
            {
                CallType = AppSettingHelper.InternalMemo_CallType_GiftSO,
                Content = string.Format("补发订单号{0}的赠品订单,生成赠品订单号码为{1}", MasterSOSysNo, CurrentSO.SysNo),
                Status = 0,
                SOSysNo = MasterSOSysNo,
                Note = CurrentSO.BaseInfo.Memo,
                CompanyCode = CurrentSO.CompanyCode
            };

            ObjectFactory<SOInternalMemoProcessor>.Instance.AddSOInternalMemoInfo(memoInfo, CurrentSO.CompanyCode);
        }
    }

    [VersionExport(typeof(SOAction), new string[] { "Gift", "Update" })]
    public class GiftSOUpdater : SOUpdater
    {
        public override void WriteLog()
        {
            new SOLogProcessor().WriteSOLog(BizLogType.Sale_SO_Create
                , this.CurrentSO.SysNo.Value
                , "创建赠品订单");
        }
    }

    #endregion

    #region 团购订单

    [VersionExport(typeof(SOAction), new string[] { "GroupBuy", "Update" })]
    public class GroupBuyNewSOUpdater : SOUpdater
    {
        protected override void PreProcess()
        {
            base.PreProcess();
            SetSOSettlementStatus(this.CurrentSO);
        }

        /// <summary>
        /// 重新计算订单团购状态
        /// </summary>
        private void SetSOSettlementStatus(SOInfo soInfo)
        {
            if (soInfo.BaseInfo.SOType == SOType.GroupBuy)
            {
                //已经去除了团购失败的商品，则将状态更改为S
                bool tmpExistGroupError = soInfo.Items.Exists(item =>
                {
                    return item.SettlementStatus == SettlementStatus.PlanFail
                        && item.SettlementStatus == SettlementStatus.Fail;
                });
                if (!tmpExistGroupError)
                {
                    soInfo.BaseInfo.SettlementStatus = SettlementStatus.Success;
                }
            }
        }

        protected override void CheckGroupBuyProducts()
        {
            //筛选团购商品
            var itemList = this.CurrentSO.Items.Where(x => x.ProductType == SOProductType.Product
                || x.ProductType == SOProductType.Gift
                || x.ProductType == SOProductType.Award
                || x.ProductType == SOProductType.Accessory
                || x.ProductType == SOProductType.SelfGift
                ).Select(x => x).ToList();

            if (itemList.Count > 0)
            {
                if (this.CurrentSO.BaseInfo.SOType == SOType.GroupBuy)
                {
                    #region 团购订单判断

                    List<int> groupProducts = new List<int>();

                    // 由于老团购订单已经处理完毕 所以可以注释掉老团购订单的代码了  2012-9-6 jack

                    #region 新团购逻辑

                    //判断商品列表中是否含有团购商品
                    bool tmpIsExistGroupBuyPro = itemList.Count(p => p.ReferenceSysNo.HasValue) > 0;
                    //是团购商品，但无团购商品
                    //出现的场景：新团购订单被拆单后的子単中可能全部被拆分成了非团购的商品
                    if (!tmpIsExistGroupBuyPro)
                    {
                        //如果该团购订单中没有团购商品，则直接验证非团购商品的合法性
                        CheckProductNotGroupBuying(itemList);
                        return;
                    }
                    //订单中不能包含团购未成功的商品
                    if (CurrentSO.Items.Exists(item =>
                    {
                        return item.ReferenceSysNo.HasValue && item.SettlementStatus != SettlementStatus.Success;
                    }))
                    {
                        BizExceptionHelper.Throw("SO_Create_UnScessfulProductInGroupBuySO");
                    }

                    //获取订单中全部的团购成功的团购组号
                    int[] referenceNoArr = CurrentSO.Items.Where(item => item.SettlementStatus == SettlementStatus.Success).Select(item => item.ReferenceSysNo.Value).Distinct().ToArray();

                    //获取全部团购成功的商品信息
                    List<ProductGroupBuyInfo> productGroupBuyingList = ObjectFactory<ISODA>.Instance.GetAllGoupMaxPreOrder(referenceNoArr);

                    //筛选出所有的团购商品
                    List<SOItemInfo> groupBuyingItemList = itemList.FindAll(item => item.ReferenceSysNo.HasValue && item.SettlementStatus == SettlementStatus.Success);

                    //检测如果团购商品集合数量不等于原集合数量，则表示原集合中存在非本团购组商品
                    if (groupBuyingItemList.Count != itemList.Count)
                    {
                        //进行非团购商品的验证
                        var notGroupItemList = itemList.FindAll(delegate(SOItemInfo item)
                        {
                            //不在订单中的团购商品集合中并且不在订单中的团购成功的团购组中
                            return !groupBuyingItemList.Select(p => p.ProductSysNo).Contains(item.ProductSysNo)
                                && !productGroupBuyingList.Select(p => p.ProductSysNo).Contains(item.ProductSysNo.Value);
                        });

                        CheckProductNotGroupBuying(notGroupItemList);
                    }

                    //check 团购商品的购买数量的限制
                    foreach (int groupSysNo in referenceNoArr)
                    {
                        var maxPerOrder = productGroupBuyingList.FirstOrDefault(p => groupSysNo == p.SysNo);
                        int soMaxPerOrder = groupBuyingItemList.Where(item =>
                                                                        productGroupBuyingList.Select(p => p.ProductSysNo)
                                                                        .Contains(item.ProductSysNo.Value))
                                                               .Sum(item => item.Quantity.Value);
                        if (maxPerOrder != null
                            && soMaxPerOrder > maxPerOrder.MaxPerOrder)
                        {
                            BizExceptionHelper.Throw("SO_Create_ProductOutMaxCountGroupBuy");
                        }
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    //非团购订单判断
                    List<int> productsOnGroupBuying = ExternalDomainBroker.GetProductsOnGroupBuying(itemList.Select(p => p.ProductSysNo.Value));

                    //没有就过
                    if (productsOnGroupBuying.Count == 0)
                        return;
                    ValidateGroupBuyProductsError(productsOnGroupBuying);
                }

            }
        }

        //检测新团购订单中的非团购的商品的合法性
        //检测逻辑：
        //         1、这些商品如果不在老订单中出现
        //         2、这些商品的购买数量和老订单不一致
        private void CheckProductNotGroupBuying(List<SOItemInfo> itemList)
        {
            //获取老订单的SOItem
            var orgItemList = OriginalInfo.Items;
            itemList.ForEach(delegate(SOItemInfo item)
            {
                SOItemInfo entity = orgItemList.Find(p => p.ProductSysNo == item.ProductSysNo);
                //如果新订单中的商品在老订单中不存在，则不允许添加
                if (entity == null)
                {
                    BizExceptionHelper.Throw("SO_Create_NotAllowCreateProductInGroupBuySO", item.ProductID);
                }
                //如果购买数量发生变化，则不允许修改
                if (entity.Quantity != item.Quantity)
                {
                    BizExceptionHelper.Throw("SO_Create_NotAllowUpdateGeneralProductQtyInGroupBuySO", item.ProductID);
                }
            });
        }
    }

    #endregion

    #endregion
}
