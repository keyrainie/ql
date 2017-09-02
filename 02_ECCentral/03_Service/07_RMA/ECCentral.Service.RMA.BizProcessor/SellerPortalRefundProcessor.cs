using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(SellerPortalRefundProcessor))]
    public class SellerPortalRefundProcessor
    {
        private readonly decimal pointExchangeRate = ExternalDomainBroker.GetPointToMoneyRatio();
        private IRefundDA refundDA = ObjectFactory<IRefundDA>.Instance;
        private IRequestDA requestDA = ObjectFactory<IRequestDA>.Instance;
        private IRegisterDA registerDA = ObjectFactory<IRegisterDA>.Instance;
        private IGiftCardRMARedeemLogDA giftCardRMARedeemLogDA = ObjectFactory<IGiftCardRMARedeemLogDA>.Instance;

        private SOInfo soInfo = null;
        private List<RMARegisterInfo> registers = null;
        private SOIncomeInfo incomeForSO = null;
        private decimal cashPointRate = 0;
        private bool soPaid = false;
        private List<GiftCardInfo> giftCards = null;
        private int customerValidScore = 0;


        #region private method
        private SOInfo GetSOInfo(int soNumber)
        {
            SOInfo result = ExternalDomainBroker.GetSOInfo(soNumber);
            if (result == null || result.Items == null || result.Items.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_SOInfoException"));
            }

            if (result.BaseInfo.SOType.HasValue)
            {
                if (!result.BaseInfo.Status.HasValue || result.BaseInfo.Status.Value != SOStatus.OutStock)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_SOInfoSatatusNoOutStock"));
                }

                if (result.BaseInfo.SOType == SOType.ElectronicCard)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_ElectronicCardNoLogistics"));
                }
                else if (result.BaseInfo.SOType == SOType.PhysicalCard)
                {
                    List<GiftCardInfo> cards = ExternalDomainBroker.GetGiftCardInfoBySOSysNo(result.SysNo.Value, GiftCardType.Standard);
                    if (cards == null || cards.Count == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_CardsException"));
                    }

                    giftCards = cards;
                }
            }

            foreach (var item in result.Items)
            {
                if (item.ProductType.Value == SOProductType.ExtendWarranty)
                {
                    foreach (var item1 in result.Items)
                    {
                        if (item1.ProductType.Value == SOProductType.Product && item1.ProductSysNo.ToString() == item.MasterProductSysNo)
                        {
                            item.StockSysNo = item1.StockSysNo;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private SOIncomeInfo GetIncomeInfo(int soNumber)
        {
            return ExternalDomainBroker.GetValidSOIncomeInfo(soNumber, SOIncomeOrderType.SO);
        }

        /// <summary>
        /// 获取支付详细信息
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        private PayType GetPayType(int payTypeSysNo)
        {
            return ExternalDomainBroker.GetPayType(payTypeSysNo);
        }

        /// <summary>
        /// 根据订单编号获取单件列表
        /// </summary>
        /// <param name="soNumber"></param>
        /// <returns></returns>
        private List<RMARegisterInfo> GetRegisters(int soNumber)
        {
            List<RMARegisterInfo> list = registerDA.LoadBySOSysNo(soNumber);
            if (list != null && list.Count > 0)
            {
                List<RMARegisterInfo> result = new List<RMARegisterInfo>();
                foreach (var register in list)
                {
                    if (register.BasicInfo.Status == RMARequestStatus.Handling
                        && register.BasicInfo.SOItemType != SOProductType.Coupon)
                    {
                        result.Add(register);
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 创建退款单Master信息
        /// </summary>
        /// <param name="warehouseRefund"></param>
        /// <param name="refundItems"></param>
        /// <param name="shipFee"></param>
        /// <returns></returns>
        private RefundInfo CreateRefundMaster(WarehouseRefund warehouseRefund, List<RefundItemInfo> refundItems, ShipFeeEntity shipFee)
        {
            RefundInfo refundMaster = new RefundInfo();
            refundMaster.CompanyCode = this.soInfo.CompanyCode;
            refundMaster.RefundItems = refundItems;
            refundMaster.CheckIncomeStatus = null;
            refundMaster.FinanceNote = string.Empty;
            refundMaster.IncomeBankInfo = null;
            refundMaster.RefundID = string.Empty;
            refundMaster.AuditTime = DateTime.Now;
            refundMaster.AuditUserSysNo = ServiceContext.Current.UserSysNo;
            if (soPaid)
            {
                refundMaster.CashFlag = CashFlagStatus.Yes;
            }
            else
            {
                refundMaster.CashFlag = CashFlagStatus.No;
            }
            refundMaster.CompensateShipPrice = CalculateCompensateShipPrice(shipFee);

            refundMaster.CreateTime = DateTime.Now;
            refundMaster.CreateUserSysNo = ServiceContext.Current.UserSysNo;
            refundMaster.CustomerSysNo = soInfo.BaseInfo.CustomerSysNo;
            refundMaster.InvoiceLocation = warehouseRefund.WarehouseNumber;
            refundMaster.Note = "物流拒收之退换货自动处理";
            refundMaster.RefundReason = 2;//物流拒收
            refundMaster.RefundTime = DateTime.Now;
            refundMaster.RefundUserSysNo = ServiceContext.Current.UserSysNo;
            refundMaster.SOCashPointRate = this.cashPointRate;
            refundMaster.SOInvoiceNo = shipFee.InvoiceNo;
            refundMaster.Status = RMARefundStatus.Refunded;
            refundMaster.SOSysNo = this.soInfo.SysNo;
            refundMaster.CashAmt = shipFee.SumExtendPrice;


            refundMaster.RefundPayType = RefundPayType.NetWorkRefund;


            refundMaster.OrgGiftCardAmt = 0;
            refundMaster.OrgPointAmt = (from r in refundItems select r.RefundPoint).Sum();

            #region PriceprotectPoint 减去价保积分补偿逻辑
            List<int> registerSysNoList = new List<int>();
            refundItems.ForEach(item =>
            {
                if (item.RegisterSysNo != null)
                {
                    registerSysNoList.Add(item.RegisterSysNo.Value);
                }
            });
            // 3、检查是否有效的 Register
            List<RMARegisterInfo> registers = refundDA.GetRegistersForCreate(registerSysNoList);
            if (registers == null || registers.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Register_ItemsInvalid"));
            }
            if (registers != null && registers.Count > 0)
            {
                refundMaster.PriceprotectPoint = ExternalDomainBroker.GetPriceprotectPoint(refundMaster.SOSysNo.Value,
                registers.Select(item => item.BasicInfo.ProductSysNo.Value).ToList());

                refundMaster.HasPriceprotectPoint = true;
            }

            if (refundMaster.PriceprotectPoint.HasValue && refundMaster.PriceprotectPoint > 0)
            {
                refundMaster.OrgPointAmt -= (refundMaster.PriceprotectPoint ?? 0);
            }
            #endregion

            refundMaster.OrgCashAmt = (from r in refundItems select r.RefundCash).Sum();
            refundMaster.DeductPointFromCurrentCash = 0;

            if (refundMaster.OrgPointAmt < 0)
            {
                if (refundMaster.OrgPointAmt * -1 < customerValidScore)
                {
                    refundMaster.DeductPointFromAccount = refundMaster.OrgPointAmt * -1;
                    customerValidScore -= refundMaster.OrgPointAmt.Value * -1;
                }
                else
                {
                    refundMaster.DeductPointFromAccount = customerValidScore;
                    refundMaster.DeductPointFromCurrentCash =
                         Decimal.Round(
                         (
                         (refundMaster.OrgPointAmt ?? 0) * -1
                        - customerValidScore
                        ) / pointExchangeRate
                        , 2);
                    customerValidScore = 0;
                }
            }
            else
            {
                refundMaster.DeductPointFromAccount = 0;
            }
            refundMaster.PointAmt = refundMaster.OrgPointAmt;
            if (refundMaster.PointAmt < 0)
            {
                refundMaster.PointAmt = 0;
            }

            refundMaster.GiftCardAmt = shipFee.GiftCardPay;
            refundMaster.OrgGiftCardAmt = refundMaster.GiftCardAmt;

            if (refundMaster.OrgGiftCardAmt >= refundMaster.OrgCashAmt)
            {
                refundMaster.OrgGiftCardAmt = refundMaster.OrgCashAmt;
                refundMaster.OrgCashAmt = 0;
            }
            else
            {
                refundMaster.OrgCashAmt -= refundMaster.OrgGiftCardAmt;
            }
            AllocateGiftCardAmount(refundMaster);

            if (refundMaster.DeductPointFromCurrentCash > 0)
            {
                if (refundMaster.CashAmt == 0)
                {
                    refundMaster.GiftCardAmt -= refundMaster.DeductPointFromCurrentCash;
                }
                else
                {
                    refundMaster.CashAmt -= refundMaster.DeductPointFromCurrentCash;
                }
            }
            return refundMaster;
        }

        /// <summary>
        /// 查收退款单ID
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        protected virtual string GenerateRefundID(int sysno)
        {
            return String.Format("R3{0:00000000}", sysno);  //return "R3" + sysno.ToString().PadLeft(8, '0');
        }

        /// <summary>
        /// 创建退款单Item信息
        /// </summary>
        /// <param name="warehouseRefund"></param>
        /// <returns></returns>
        protected virtual List<RefundItemInfo> CreateRefundItems(WarehouseRefund warehouseRefund)
        {
            List<RefundItemInfo> refundItems = new List<RefundItemInfo>();
            decimal? unitDiscount = 0;
            foreach (RefundRegister refundRegister in warehouseRefund.RefundRegisters)
            {
                unitDiscount = refundRegister.DiscountAmount / refundRegister.Quantity;
                RefundItemInfo refundItem = new RefundItemInfo();
                refundItem.CompanyCode = this.soInfo.CompanyCode;
                refundItem.RegisterSysNo = refundRegister.Register.SysNo;
                refundItem.OrgGiftCardAmt = 0;
                refundItem.OrgPoint = refundRegister.Point;
                refundItem.OrgPrice = refundRegister.Price;
                refundItem.RefundPriceType = ReturnPriceType.OriginPrice;
                refundItem.ProductSysNo = refundRegister.Register.BasicInfo.ProductSysNo;
                refundItem.ProductValue = refundItem.OrgPrice + unitDiscount;
                int refundPoint = CalculateRefundPoint(refundItem.ProductValue.Value, refundRegister.PointType.Value);
                refundItem.RefundPoint = refundPoint - refundItem.OrgPoint;
                refundItem.RefundCash = CalculateRefundCash(refundItem.ProductValue.Value, refundPoint);
                refundItem.RefundCost = refundRegister.Cost;
                refundItem.RefundCostPoint = (int)(refundRegister.Point / pointExchangeRate);
                refundItem.RefundCostWithoutTax = refundRegister.UnitCostWithoutTax;
                refundItem.RefundPrice = refundItem.ProductValue;
                refundItem.UnitDiscount = unitDiscount;
                refundItem.PointType = refundRegister.PointType;
                refundItems.Add(refundItem);
            }
            return refundItems;
        }

        /// <summary>
        /// 计算现金退款
        /// </summary>
        /// <param name="productValue"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        protected virtual decimal CalculateRefundCash(decimal productValue, int point)
        {
            decimal soCashPointRate = GetSOCashPointRate(productValue);

            #region pointRedeem
            decimal pointRedeem = -Decimal.Round(
                (Decimal.Round(
                productValue * (1 - soCashPointRate) * pointExchangeRate, 0)
                -
                (productValue * (1 - soCashPointRate) * pointExchangeRate)) * 0.1M
                , 2);

            #endregion
            decimal returnValue = 0;
            returnValue = productValue - (
                Decimal.Round(
                productValue
                * (1 - soCashPointRate)
                , 2)
                );
            returnValue += pointRedeem;
            return returnValue;
        }

        /// <summary>
        /// 计算积分退款
        /// </summary>
        /// <param name="productValue"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        protected virtual int CalculateRefundPoint(decimal productValue, ProductPayType payType)
        {
            if (payType == ProductPayType.MoneyOnly)
            {
                return 0;
            }
            else if (payType == ProductPayType.PointOnly)
            {
                return (int)Decimal.Round(productValue * pointExchangeRate, 0);
            }
            else if (payType == ProductPayType.All)
            {
                decimal soCashPointRate = GetSOCashPointRate(productValue);

                return (int)Decimal.Round(productValue * (1 - soCashPointRate) * pointExchangeRate, 0);
            }
            return 0;
        }

        /// <summary>
        /// 获取商品金额中现金支付比例
        /// </summary>
        /// <param name="productValue"></param>
        /// <returns></returns>
        protected virtual decimal GetSOCashPointRate(decimal productValue)
        {
            //根据单件退款额度，调整比率精度
            int decimals = Decimal.Round(productValue, 0).ToString().Length + 2;
            //实际计算精度
            decimal soCashPointRate = Decimal.Round(cashPointRate, decimals);
            return soCashPointRate;
        }

        /// <summary>
        /// 计算补偿运费
        /// </summary>
        /// <param name="shipFee"></param>
        /// <returns></returns>
        private decimal CalculateCompensateShipPrice(ShipFeeEntity shipFee)
        {
            decimal totalAmount = 0;
            if (shipFee != null)
            {
                totalAmount = shipFee.PremiumAmount + shipFee.ShippingCharge + shipFee.PayPrice;
            }

            return totalAmount;
        }

        /// <summary>
        /// 计算现金积分支付比例
        /// </summary>
        /// <returns></returns>
        protected virtual decimal CalculateCashPointRate()
        {
            decimal? cashOnly = 0;//仅支持现金支付的金额
            decimal? pointOnly = 0;//仅支持积分支付的金额
            decimal? couponDiscount = 0;//优惠卷优惠金额
            bool existedBothSupported = false;
            foreach (SOItemInfo item in this.soInfo.Items)
            {
                decimal? discountAmount = item.PromotionAmount;
                if (item.PayType == ProductPayType.MoneyOnly)
                {
                    cashOnly += item.Price * item.Quantity + discountAmount;
                }
                else if (item.PayType == ProductPayType.PointOnly)
                {
                    pointOnly += item.Price * item.Quantity + discountAmount;
                }
                else if (item.PayType == ProductPayType.All)
                {
                    existedBothSupported = true;
                }
                couponDiscount += item.CouponAmount;
            }

            if (existedBothSupported && this.soInfo.BaseInfo.PointPay > 0)
            {
                decimal? cashPointRate = 0;

                if ((this.soInfo.BaseInfo.SOAmount + this.soInfo.BaseInfo.PromotionAmount) - (pointOnly + cashOnly) == 0)
                {
                    cashPointRate = 0;
                }
                else if ((this.soInfo.BaseInfo.SOAmount + this.soInfo.BaseInfo.PromotionAmount) - (
                    couponDiscount + pointOnly + cashOnly) == 0M)
                {
                    cashPointRate = 0;
                }
                else if (this.soInfo.BaseInfo.CashPay - cashOnly == 0)
                {
                    cashPointRate = 0;
                }
                else
                {
                    cashPointRate = (this.soInfo.BaseInfo.CashPay - cashOnly + this.soInfo.BaseInfo.PromotionAmount) /
                    (this.soInfo.BaseInfo.SOAmount - cashOnly - pointOnly + this.soInfo.BaseInfo.PromotionAmount - couponDiscount);
                }
                return cashPointRate.Value;
            }
            else
            {
                return 1;
            }
        }

        private List<ShipFeeEntity> GetInvoiceLog(int soNumber)
        {
            List<ShipFeeEntity> result = new List<ShipFeeEntity>();
            List<InvoiceMasterInfo> invoiceList = ExternalDomainBroker.GetSOInvoiceMaster(soNumber);
            if (invoiceList != null)
            {
                invoiceList.ForEach(p =>
                {
                    ShipFeeEntity entity = new ShipFeeEntity
                    {
                        DiscountAmt = p.DiscountAmt ?? 0,
                        GiftCardPay = p.GiftCardPayAmt ?? 0,
                        InvoiceNo = p.InvoiceNo,
                        PayPrice = p.ExtraAmt ?? 0,
                        PointAmt = p.GainPoint ?? 0,
                        PointPay = p.PointPaid ?? 0,
                        PremiumAmount = p.PremiumAmt ?? 0,
                        ShippingCharge = p.ShippingCharge ?? 0,
                        SumExtendPrice = p.InvoiceAmt ?? 0,
                        WarehouseNumber = p.StockSysNo.ToString()
                    };
                    result.Add(entity);
                });
            }

            foreach (var shipFee in result)
            {
                shipFee.GiftCardPay = Math.Abs(shipFee.GiftCardPay);
            }
            return result;
        }

        /// <summary>
        /// 更新对应单件的退款状态
        /// </summary>
        /// <param name="warehouseRefund"></param>
        protected virtual void UpdateRegisterStatus(WarehouseRefund warehouseRefund)
        {
            foreach (RefundRegister refundRegister in warehouseRefund.RefundRegisters)
            {
                if (refundRegister.ProductType == SOProductType.ExtendWarranty)
                {
                    refundRegister.Register.BasicInfo.Status = RMARequestStatus.Complete;
                }
                else
                {
                    refundRegister.Register.BasicInfo.OwnBy = RMAOwnBy.Self;
                }
                refundRegister.Register.BasicInfo.OwnByWarehouse = warehouseRefund.WarehouseNumber;
                refundRegister.Register.BasicInfo.RefundStatus = RMARefundStatus.Refunded;
                refundRegister.Register.BasicInfo.Status = RMARequestStatus.Complete;

                this.registerDA.UpdateForRMAAuto(refundRegister.Register);
            }
        }

        /// <summary>
        /// 更新申请单状态
        /// </summary>
        /// <param name="warehouseRefund"></param>
        private void UpdateRequestStatus(int soSysNo, RMARequestStatus status)
        {
            this.requestDA.UpdateStatus(soSysNo, status);
        }

        private void SendMessageToSSB(RefundInfo refundMaster, int warehouseNumber)
        {
#warning 需要修改为统一的SSB发送方式

            #region  [20110426 发送赠票修改]
            //refundMaster.InvoiceLocation = refundMaster.InvoiceLocation.Trim().ToLower();
            //if (refundMaster.InvoiceLocation == CommonBiz.WarehouseNo_GZ
            //    || refundMaster.InvoiceLocation == CommonBiz.WarehouseNo_XA
            //    || this.orderInfo.IsVAT == EBooleanStatus.Yes)
            //{
            //CommonService.SendInvoiceMessage(refundMaster.SysNo.Value.ToString(), refundMaster.InvoiceLocation, "RO");
            //}
            #endregion

            string ssbXml = BulidSSB(refundMaster, warehouseNumber);

            ssbXml = ssbXml.Replace(" xmlns=\"\"", "");

            ObjectFactory<IRMAForSellerPortalDA>.Instance.SendVatSSB(ssbXml);
        }

        /// <summary>
        /// 构造SSB消息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <returns></returns>
        private string BulidSSB(RefundInfo refundMaster, int warehouseNumber)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode root = doc.CreateNode(XmlNodeType.Element, "Publish", "http://soa.newegg.com/SOA/CN/InfrastructureService/V10/NeweggCNPubSubService");
            doc.AppendChild(root);

            //common
            XmlElement subject = doc.CreateElement("Subject");
            subject.InnerText = string.Format("DC01WMSShippingProcessDownLoad_{0}", 51);
            root.AppendChild(subject);

            XmlElement fromService = doc.CreateElement("FromService");
            fromService.InnerText = "http://soa.newegg.com/SOA/CN/InventoryManagement/V10/Warehouse/WMSShippingProcessDownLoad";
            root.AppendChild(fromService);

            XmlElement toService = doc.CreateElement("ToService");
            toService.InnerText = "http://soa.newegg.com/SOA/CN/InfrastructureService/V10/NeweggCN/PubSubService";
            root.AppendChild(toService);

            //node
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Node", "");
            root.AppendChild(node);

            //MessageHead
            XmlNode messageHead = doc.CreateNode(XmlNodeType.Element, "MessageHead", "");
            node.AppendChild(messageHead);

            XmlNode type = doc.CreateNode(XmlNodeType.Element, "Type", "");
            type.InnerText = "ImportInvoice";
            messageHead.AppendChild(type);

            XmlNode comment = doc.CreateNode(XmlNodeType.Element, "Comment", "");
            comment.InnerText = "Send so message to local warehouse.";
            messageHead.AppendChild(comment);

            //body
            XmlNode body = CreateBody(refundMaster, doc, warehouseNumber);

            node.AppendChild(body);

            return doc.InnerXml;
        }

        /// <summary>
        /// 创建赠票消息主体
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private XmlNode CreateBody(RefundInfo refundMaster, XmlDocument doc, int warehouseNumber)
        {

            XmlNode body = doc.CreateNode(XmlNodeType.Element, "body", "");

            XmlNode invoiceNodes = doc.CreateNode(XmlNodeType.Element, "InvoiceNodes", "");
            body.AppendChild(invoiceNodes);

            XmlNode invoiceNode = doc.CreateNode(XmlNodeType.Element, "InvoiceNode", "");
            invoiceNodes.AppendChild(invoiceNode);

            //OrderSysNo
            XmlNode orderSysNo = doc.CreateNode(XmlNodeType.Element, "OrderSysNo", "");
            //orderSysNo.InnerText = soInfo.SOMaster.SystemNumber.ToString();
            orderSysNo.InnerText = refundMaster.SysNo.ToString();
            invoiceNode.AppendChild(orderSysNo);

            //OrderType
            XmlNode orderType = doc.CreateNode(XmlNodeType.Element, "OrderType", "");
            orderType.InnerText = "RO";
            invoiceNode.AppendChild(orderType);

            //WarehouseNumber
            XmlNode warehouseNumberNode = doc.CreateNode(XmlNodeType.Element, "WarehouseNumber", "");
            warehouseNumberNode.InnerText = warehouseNumber.ToString();
            invoiceNode.AppendChild(warehouseNumberNode);

            //ComputerNo
            XmlNode computerNo = doc.CreateNode(XmlNodeType.Element, "ComputerNo", "");
            computerNo.InnerText = "0";
            invoiceNode.AppendChild(computerNo);

            return body;
        }

        protected virtual void CreateSOIncomeRefundInfo(RefundInfo refundMaster)
        {
            //47-关爱积分支付,48-平安万里通积分支付
            //if (this.soInfo.BaseInfo.PayTypeSysNo == 47 || this.soInfo.BaseInfo.PayTypeSysNo == 48 || this.soPaid)
            if (this.soPaid)
            {
                SOIncomeRefundInfo entity = new SOIncomeRefundInfo();
                entity.CompanyCode = refundMaster.CompanyCode;
                entity.OrderType = RefundOrderType.RO;
                entity.OrderSysNo = refundMaster.SysNo.Value;
                entity.SOSysNo = refundMaster.SOSysNo.Value;
                entity.Status = RefundStatus.WaitingRefund;
                entity.HaveAutoRMA = true;
                entity.RefundReason = 2;//物流拒收
                entity.RefundCashAmt = refundMaster.CashAmt;
                entity.RefundPoint = refundMaster.PointAmt - refundMaster.DeductPointFromAccount;
                entity.RefundGiftCard = refundMaster.GiftCardAmt;

                entity.Note = ResouceManager.GetMessageString("RMA.Refund", "Refund_SOIncomeInfoSoPaid");
                entity.RefundPayType = RefundPayType.NetWorkRefund;

                ExternalDomainBroker.CreateSOIncomeRefundInfo(entity);
            }
        }

        private void CreateSOIncomeForRO(RefundInfo refundMaster)
        {
            SOIncomeInfo entity = new SOIncomeInfo();
            entity.CompanyCode = refundMaster.CompanyCode;
            entity.OrderType = SOIncomeOrderType.RO;
            entity.OrderSysNo = refundMaster.SysNo.Value;

            if (refundMaster.RefundPayType.Value == RefundPayType.TransferPointRefund)
            {
                entity.OrderAmt = 0;
            }
            else
            {
                entity.OrderAmt = -1 * refundMaster.CashAmt.Value;
            }

            if (!soPaid && this.incomeForSO.PrepayAmt > 0)
            {
                entity.PrepayAmt = -1 * this.incomeForSO.PrepayAmt;
                entity.IncomeAmt = entity.OrderAmt - entity.PrepayAmt;
            }
            else
            {
                entity.IncomeAmt = entity.OrderAmt;
            }
            entity.IncomeStyle = SOIncomeOrderStyle.RO;
            entity.Note = ResouceManager.GetMessageString("RMA.Refund", "Refund_SOIncomeInfoAutoRO");

            if (this.soInfo.BaseInfo.SpecialSOType == ECCentral.BizEntity.SO.SpecialSOType.TaoBao)
            {
                entity.Note = ResouceManager.GetMessageString("RMA.Refund", "Refund_SOIncomeInfoTaoBao");
            }
            entity.GiftCardPayAmt = (refundMaster.GiftCardAmt ?? 0) * -1;
            entity.Status = SOIncomeStatus.Origin;

            SOIncomeInfo incomeInfo = ExternalDomainBroker.CreateSOIncome(entity);
            if (!soPaid)
            {
                ExternalDomainBroker.AutoConfirmIncomeInfo(incomeInfo.SysNo.Value, refundMaster.SysNo ?? 0, GetAutoRMAUserSysNo());
            }
        }

        private void SetSOIncomeForSO()
        {
            if (!this.soPaid)
            {
                ExternalDomainBroker.AutoConfirmIncomeInfo(this.incomeForSO.SysNo.Value, soInfo.SysNo.Value, GetAutoRMAUserSysNo());
            }
        }

        /// <summary>
        /// 获取RMAAuto用户的系统编号
        /// </summary>
        /// <returns></returns>
        private int GetAutoRMAUserSysNo()
        {
            return ExternalDomainBroker.GetUserSysNo(AppSettingManager.GetSetting("RMA", RMAConst.AutoRMAPhysicalUserName),
                            AppSettingManager.GetSetting("RMA", RMAConst.AutoRMALoginUserName), AppSettingManager.GetSetting("RMA", RMAConst.AutoRMASourceDirectoryKey));
        }

        /// <summary>
        /// 返余额
        /// </summary>
        /// <param name="refundMasters"></param>       
        private void RefundPrepay(List<RefundInfo> refundMasters)
        {
            if (!soPaid && this.incomeForSO.PrepayAmt > 0)
            {
                var result = from item in refundMasters
                             select item.SysNo.ToString();
                string sysNoList = String.Join(",", result.ToArray());

                CustomerPrepayLog entity = new CustomerPrepayLog();
                entity.CustomerSysNo = this.soInfo.BaseInfo.CustomerSysNo;
                entity.SOSysNo = refundMasters[0].SOSysNo;
                entity.AdjustAmount = this.incomeForSO.PrepayAmt.Value;
                entity.PrepayType = PrepayType.ROReturn;
                entity.Note = ResouceManager.GetMessageString("RMA.Refund", "Refund_DeclinedROSysNo") + sysNoList;

                ExternalDomainBroker.AdjustPrePay(entity);
            }
        }

        /// <summary>
        /// 代销转财务接口
        /// </summary>
        /// <param name="refundMasters"></param>        
        private void BatchCreateConsignToAcc(RefundInfo refundMaster, int warehouseNumber, int vendorSysNo)
        {
            //SOInfo so = ExternalDomainBroker.GetSOInfo(refundMaster.SOSysNo.Value);
            //代销转财务(业务模式2，3 才可以调用)
            if (
                (this.soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.MET
                && this.soInfo.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF
                && this.soInfo.InvoiceInfo.InvoiceType == InvoiceType.SELF)
                ||
                   (this.soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.MET
                   && this.soInfo.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.MET
                   && this.soInfo.InvoiceInfo.InvoiceType == InvoiceType.SELF)
                )
            {
                List<ConsignToAcctLogInfo> list = new List<ConsignToAcctLogInfo>();

                foreach (RefundItemInfo item in refundMaster.RefundItems)
                {
                    //只允许 0,1,2 模式代销转财务 。ForSale = 0, Gift = 1, CustomerGift = 2,
                    if (item.ProductType == SOProductType.Product || item.ProductType == SOProductType.Gift || item.ProductType == SOProductType.Award)
                    {
                        //代销转财务
                        ConsignToAcctLogInfo msg = new ConsignToAcctLogInfo();
                        msg.ProductSysNo = item.ProductSysNo;
                        msg.VendorSysNo = vendorSysNo;
                        msg.StockSysNo = warehouseNumber;
                        msg.ProductQuantity = -1;
                        msg.CreateCost = item.RefundCost;
                        msg.SalePrice = item.RefundPrice;
                        msg.Point = item.RefundPoint;
                        msg.OrderSysNo = refundMaster.SysNo;
                        msg.ConsignToAccType = ConsignToAccountType.Manual;
                        list.Add(msg);
                    }
                }

                ExternalDomainBroker.BatchCreateConsignToAcctLogs(list);
            }
        }

        /// <summary>
        /// 返积分
        /// </summary>
        /// <param name="refundMasters"></param>        
        private void RefundPoint(List<RefundInfo> refundMasters)
        {
            int? refundPoint = 0;
            int deductPointFromAccount =
                refundMasters.Sum(item => item.DeductPointFromAccount.Value);
            decimal deductPointFromCurrentCash =
                refundMasters.Sum(item => item.DeductPointFromCurrentCash.Value);

            if (deductPointFromAccount == 0 && deductPointFromCurrentCash == 0)
            {
                refundPoint = this.soInfo.BaseInfo.PointPay - this.soInfo.BaseInfo.GainPoint;
            }
            else
            {
                refundPoint = deductPointFromAccount * -1;
            }

            if (refundPoint != 0)
            {
                AdjustPointRequest entity = new AdjustPointRequest();
                entity.CustomerSysNo = this.soInfo.BaseInfo.CustomerSysNo;
                entity.Point = refundPoint;
                entity.PointType = (int)AdjustPointType.ReturnProductPoint;
                entity.SOSysNo = this.soInfo.SysNo;
                entity.Memo = string.Format(ResouceManager.GetMessageString("RMA.Refund", "Refund_AdjustPointRequestMemo"), this.soInfo.BaseInfo.PointPay, this.soInfo.BaseInfo.GainPoint);
                entity.OperationType = AdjustPointOperationType.Abandon;
                entity.Source = "SellerPortal";
                ExternalDomainBroker.AdjustPoint(entity);
            }
        }

        private List<WarehouseRefund> GetWarehouseRefunds()
        {
            this.soInfo.Items.RemoveAll(p => { return (p.ProductType.Value == SOProductType.Coupon); });
            List<WarehouseRefund> result = new List<WarehouseRefund>();
            //2011.12.27 修复物流拒收服务(多仓含有SoItemType==6)
            //this.orderInfo.Items.Sort(SOItemComparison);
            //this.registers.Sort(RegisterComparison);
            WarehouseRefund warehouseRefund = null;
            RefundRegister refundRegister = null;
            int j = 0;
            int masterProductSysNo;
            string oldWarehouse = string.Empty;
            for (int i = 0; i < this.soInfo.Items.Count; i++)
            {
                if (string.Compare(oldWarehouse, this.soInfo.Items[i].StockSysNo.ToString()) != 0)
                {
                    if (warehouseRefund == null || result.Count(p => p.WarehouseNumber == this.soInfo.Items[i].StockSysNo.ToString()) == 0)
                    {
                        warehouseRefund = new WarehouseRefund();
                        result.Add(warehouseRefund);
                        warehouseRefund.WarehouseNumber = this.soInfo.Items[i].StockSysNo.ToString();
                        warehouseRefund.RefundRegisters = new List<RefundRegister>();
                        oldWarehouse = warehouseRefund.WarehouseNumber;
                    }
                }

                for (j = 0; j < this.registers.Count; j++)
                {
                    if (this.soInfo.Items[i].StockSysNo.ToString() == this.registers[j].BasicInfo.ShippedWarehouse.Trim()
                        && (
                        (this.soInfo.Items[i].ProductSysNo == this.registers[j].BasicInfo.ProductSysNo
                        && (this.registers[j].BasicInfo.SOItemType.Value != SOProductType.ExtendWarranty)
                        && this.registers[j].BasicInfo.SOItemType.Value != SOProductType.Coupon
                        )

                        || (this.soInfo.Items[i].MasterProductSysNo == this.registers[j].BasicInfo.ProductSysNo.ToString()
                        && this.registers[j].BasicInfo.SOItemType.Value == SOProductType.ExtendWarranty)
                        )
                        && this.soInfo.Items[i].ProductType == this.registers[j].BasicInfo.SOItemType)
                    {
                        refundRegister = new RefundRegister();
                        WarehouseRefund current = result.Single(p => p.WarehouseNumber.Trim() == this.registers[j].BasicInfo.ShippedWarehouse.Trim());
                        current.RefundRegisters.Add(refundRegister);
                        refundRegister.DiscountAmount = this.soInfo.Items[i].PromotionAmount;
                        refundRegister.Point = this.soInfo.Items[i].GainAveragePoint;
                        refundRegister.Price = this.soInfo.Items[i].Price;
                        refundRegister.PromotionDiscount = this.soInfo.Items[i].CouponAmount;
                        refundRegister.Quantity = this.soInfo.Items[i].Quantity;
                        refundRegister.PointType = this.soInfo.Items[i].PayType;
                        refundRegister.OriginalPrice = this.soInfo.Items[i].OriginalPrice;
                        refundRegister.ProductSysNo = this.soInfo.Items[i].ProductSysNo;
                        refundRegister.ProductType = this.soInfo.Items[i].ProductType;
                        refundRegister.Cost = this.soInfo.Items[i].CostPrice;
                        refundRegister.UnitCostWithoutTax = this.soInfo.Items[i].NoTaxCostPrice;
                        refundRegister.PointPay = soInfo.BaseInfo.PointPay;
                        if (int.TryParse(this.soInfo.Items[i].MasterProductSysNo, out masterProductSysNo))
                        {
                            refundRegister.MasterProductSysNo = masterProductSysNo;
                        }
                        refundRegister.Register = this.registers[j];
                        continue;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 持久化退款单
        /// </summary>
        /// <param name="refundMaster"></param>        
        private void PersistRefund(RefundInfo refundMaster)
        {
            //#if DEBUG
            //            LogHelper.WriteLog<RefundInfo>("RMA4SellerPortalRefundBiz.PersistRefund", refundMaster);
            //#endif
            //Bob.H.Li 添加事务
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //以前退款单未添加事务
                refundMaster.SysNo = refundDA.CreateSysNo();
                refundMaster.RefundID = GenerateRefundID(refundMaster.SysNo.Value);

                refundDA.InsertMaster(refundMaster);

                refundMaster.RefundItems.ForEach(item =>
                {
                    item.RefundSysNo = refundMaster.SysNo;
                    refundDA.InsertItem(item);
                });

                scope.Complete();
            }
        }

        private List<GiftCardRedeemLog> GetGiftCardRedeemLog(int soNumber)
        {
            return ExternalDomainBroker.GetGiftCardRedeemLog(soNumber, ActionType.SO);
        }

        private void AllocateGiftCardAmount(RefundInfo refundMaster)
        {
            if (refundMaster.GiftCardAmt.HasValue && refundMaster.GiftCardAmt > 0)
            {
                decimal remainingAmount = refundMaster.GiftCardAmt.Value;
                foreach (RefundItemInfo entity in refundMaster.RefundItems)
                {
                    if (remainingAmount == 0)
                    {
                        break;
                    }
                    else if (entity.RefundCash > 0)
                    {
                        if (remainingAmount >= entity.RefundCash)
                        {
                            entity.OrgGiftCardAmt = entity.RefundCash;
                        }
                        else
                        {
                            entity.OrgGiftCardAmt = remainingAmount;
                        }
                        entity.RefundCash -= entity.OrgGiftCardAmt;
                        remainingAmount -= entity.OrgGiftCardAmt.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 返礼品卡
        /// </summary>
        /// <param name="refundMasters"></param>
        private void RefundGiftCard(List<RefundInfo> refundMasters)
        {
            if (this.soInfo.BaseInfo.GiftCardPay.HasValue && this.soInfo.BaseInfo.GiftCardPay.Value > 0)
            {
                List<GiftCardRedeemLog> cardConsumeLogs = GetGiftCardRedeemLog(this.soInfo.SysNo.Value);
                if (cardConsumeLogs == null || cardConsumeLogs.Count == 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRedeemLogNotExists"));
                }

                decimal remainingAmount = 0;
                GiftCardRMARedeemLog log = null;
                List<GiftCard> toRefundCards = new List<GiftCard>();
                List<GiftCardRMARedeemLog> logs = new List<GiftCardRMARedeemLog>();
                GiftCard toRefundCard = null;
                foreach (RefundInfo refundEntity in refundMasters)
                {
                    if (refundEntity.GiftCardAmt > 0)
                    {
                        remainingAmount = refundEntity.GiftCardAmt.Value;

                        foreach (var card in cardConsumeLogs)
                        {
                            if (remainingAmount == 0)
                            {
                                break;
                            }
                            else if (card.Amount > 0)
                            {
                                log = new GiftCardRMARedeemLog();
                                toRefundCard = new GiftCard();

                                if (remainingAmount >= card.Amount)
                                {
                                    log.Amount = -1 * card.Amount;
                                }
                                else
                                {
                                    log.Amount = -1 * remainingAmount;
                                }

                                log.Code = card.Code;
                                toRefundCard.Code = log.Code;
                                log.CustomerSysNo = this.soInfo.BaseInfo.CustomerSysNo;
                                toRefundCard.CustomerSysNo = log.CustomerSysNo.Value;
                                log.RefundSysNo = refundEntity.SysNo.Value;
                                toRefundCard.ReferenceSysNo = log.RefundSysNo.Value;
                                log.SOSysNo = this.soInfo.SysNo;
                                log.Status = "A";
                                log.Memo = "RMARefund";
                                log.CurrencySysNo = 1;
                                log.CompanyCode = this.soInfo.CompanyCode;
                                card.Amount += log.Amount;
                                toRefundCard.ConsumeAmount = log.Amount.Value;
                                remainingAmount += log.Amount.Value;
                                logs.Add(log);
                                toRefundCards.Add(toRefundCard);
                            }
                        }
                    }
                }

                foreach (var item in toRefundCards)
                {
                    string statusCode = ExternalDomainBroker.GiftCardRMARefund(new List<GiftCard> { item }, this.soInfo.CompanyCode);
                    string msg = ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_GiftCardRefundFailed");
                    throw new BizException(msg);
                }

                foreach (var item in logs)
                {
                    giftCardRMARedeemLogDA.Create(item);
                }
            }
        }

        private void WriteCreateRefundLog(RefundInfo refundInfo)
        {
            ExternalDomainBroker.CreateOperationLog("创建退款单", BizLogType.RMA_Refund_Create, refundInfo.SysNo.Value, refundInfo.CompanyCode);
        }

        private List<ShipFeeEntity> GetSubSOMaster(int soSysNo)
        {
            List<ShipFeeEntity> list = new List<ShipFeeEntity>();

            List<InvoiceMasterInfo> invoiceList = ExternalDomainBroker.GetSOInvoiceMaster(soSysNo);
            List<SOPriceMasterInfo> subso = ExternalDomainBroker.GetSOPriceBySOSysNo(soSysNo);
            if (subso != null)
            {
                subso.ForEach(p =>
                {
                    var invoice = invoiceList.FirstOrDefault(i => i.StockSysNo == p.StockSysNo);
                    ShipFeeEntity entity = new ShipFeeEntity
                    {
                        WarehouseNumber = p.StockSysNo.ToString(),
                        PremiumAmount = p.PremiumAmount.Value,
                        GiftCardPay = p.GiftCardPay.Value,
                        PayPrice = p.PayPrice.Value,
                        PointAmt = p.GainPoint.Value,
                        PointPay = p.PointPayAmount.Value,
                        ShippingCharge = p.ShipPrice.Value,
                        DiscountAmt = p.PromotionAmount.Value
                    };
                    if (invoice != null)
                    {
                        entity.InvoiceNo = invoice.InvoiceNo;
                    }
                    entity.SumExtendPrice = (p.CashPay + p.ShipPrice + p.PayPrice + p.PremiumAmount + p.PromotionAmount + p.GiftCardPay).Value;
                    entity.GiftCardPay = Math.Abs(entity.GiftCardPay);
                    list.Add(entity);
                });
            }
            return list;
        }

        /// <summary>
        ///  如果顾客已经付款则退款逻辑是不一样的。
        ///    获取此订单的有效的财务收款记录
        ///    循环针对不同的仓库，生成不同的RMA退款记录
        ///     1、生成退款单号
        ///      2、生成退款单ID
        ///       3、计算补偿运费
        ///       补偿运费 = 运费 +　保价费 +　手续费
        ///       运费，保价费，手续费都直接从Shipping_InvoiceLog表里获取
        ///       运费 = ShippingCharge，保价费= PremiumAmt，手续费=PayPrice
        ///      4、如果是 中智积分支付,平安万里通积分支付则退款方式与其对应，否则默认为银行转账
        ///       5、Note备注信息为物流拒收之RMA自动处理,状态为已退款
        ///       6、CashFlag是否涉及现金为-1，标示为物流拒收
        ///       7、SOInvoiceNo发票号从发票日志记录里取
        ///       8、发票所在地即为产品当时所在的仓库
        ///       9、退款原因为    RMAReject = 2 [Description("物流拒收")]
        ///       10、退款金额为 直接从Shipping_InvoiceLog取SumExtendPrice
        ///      11、获取这个订单对应的所有单件记录
        ///       12、计算总的折扣金额，遍历SO_Item表里的每一条数据做累加
        ///      13、计算积分支付比例
        ///      就是混合支付的Product中用现金支付的总额/混合支付的总金额，这个支付比例只计算一次即可,注意折扣扣减只有一次
        ///       14、遍历对仓库的单件列表，生成Refund Item记录
        ///      15、SO_Item表里的Price是 OriginalPrice  - PromotionDiscount，但是没有减DiscountAmt
        ///      16、产品价值为产品原价 - PromotionDiscount - DiscountAmt/Quantity
        ///      17、OrgPoint = 赠送的积分= SOItem.Point
        ///       18、RefundPrice 退款价值 = OrgPrice - unitDiscount
        ///      19、RefundPriceType退款类型
        ///      20、退款的成本RefundCost ，退款时赋值，数据来源于V_SO_Item 视图对应记录的 Cost
        ///     21、无税的成本 RefundCostWithoutTax，退款时赋值，数据来源于V_SO_Item 视图对应记录的 UnitCostWithoutTax
        ///     22、RefundCostPoint退款的积分价值 SO_item.Point/10
        ///     23、根据积分支付比例，计算每一个Item的RefundPoint和RefundCash
        ///    24、对RefundCashAmt和RefundPoint做累加
        ///    25、如果是延保产品则直接 关闭此单件，并且更新单件的OwnBy，OwnByWarehouse
        ///     26、发送SSB消息给税控中心
        ///    但是中智和平安万里不用发的，北京，广州，西安，还有是增票的要发ＳＳＢ的
        ///    27、计算应退积分PointAmt和应扣积分DeductPointFromAccount
        ///    28、生成SOIncomeBankInfo记录
        ///    中智和平安万里是要插入记录的
        ///  但是其他方式的如果发生了资金的流转则也要插入记录的
        ///   同时写日志
        ///   29、如果没有有效的SoIncome的RO记录，则生成相应的记录
        ///   设置每个SOIncome的PrepayAmout，基本上是按照记录顺序，逐个赋值
        ///   30、如果已经有实际的资金流，设置IncomeAmt = 退款金额 - 预付款金额，否则IncomeAmt = 退款金额 
        ///    如果已经发生实际的资金流，则状态为原始，否则为已确认，记录日志
        ///   自动confirm收款单
        ///   如果顾客已经支付过帐户余额的话就给顾客返还余额
        ///   如果此订单有赠送给顾客的积分但是还没有给顾客赠送积分则给顾客加积分，
        ///  最后根据顾客在此订单消费的积分 - 赠送的积分 调整顾客的积分      /// 
        /// </summary>
        /// <param name="refund"></param>        
        private List<RefundInfo> CreateRefund(AutoRefundInfo refund, int warehouseNumber, int vendorSysNo)
        {
            List<ShipFeeEntity> invoiceLogs = null;
            List<RefundInfo> entitys = refundDA.GetMasterByOrderSysNo(refund.SOSysNo.Value);
            if (entitys != null && entitys.Count > 0
            && entitys.Exists(p => { return p.Status.Value != RMARefundStatus.Abandon; }))
            {

                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_ExistsRMARefund"));
            }

            this.soInfo = GetSOInfo(refund.SOSysNo.Value);
            if (soInfo.BaseInfo.GainPoint > soInfo.BaseInfo.PointPay)
            {
                CustomerInfo customer = ExternalDomainBroker.GetCustomerInfo(soInfo.BaseInfo.CustomerSysNo.Value);
                if (customer == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_CustomerNotExists"));
                }
                customerValidScore = (customer.ValidScore > 0) ? customer.ValidScore.Value : 0;
            }
            //5,6 商家开票需要在订单捞数据；2，3泰隆优选开票需要在发票中捞数据 2011.1021
            if (((soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.MET)
                && (soInfo.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF)
                 && (soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET))
                ||
                (
                ((soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.MET)
               && (soInfo.ShippingInfo.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.MET)
                 && (soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET))
                )
                )
                invoiceLogs = GetSubSOMaster(refund.SOSysNo.Value);
            else
                invoiceLogs = GetInvoiceLog(refund.SOSysNo.Value);

            this.registers = GetRegisters(refund.SOSysNo.Value);
            this.incomeForSO = GetIncomeInfo(refund.SOSysNo.Value);

            if (incomeForSO == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_IncomeForSOException"));
            }

            if (registers == null || registers.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_NoExistsRegisters"));
            }

            PayType payType = GetPayType(soInfo.BaseInfo.PayTypeSysNo.Value);

            if (payType == null)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_InvalidPayType"));
            }

            if (payType.IsPayWhenRecv.Value && incomeForSO.Status == SOIncomeStatus.Origin)
            {
                soPaid = false;
            }
            else
            {
                soPaid = true;
            }

            cashPointRate = CalculateCashPointRate();

            List<WarehouseRefund> warehouseRefunds = GetWarehouseRefunds();
            List<RefundInfo> refundMasters = new List<RefundInfo>();

            //#if DEBUG
            //            try
            //            {

            //                LogHelper.WriteLog<bool>("soPaid:", this.soPaid);
            //                LogHelper.WriteLog<PayType>("payType", payType);
            //                LogHelper.WriteLog<List<ShipFeeEntity>>("this.invoiceLogs:", this.invoiceLogs);
            //                LogHelper.WriteLog<SOIncomeInfo>("this.incomeForSO:", this.incomeForSO);
            //                LogHelper.WriteLog<SOInfo>("this.orderInfo:", this.soInfo);
            //                LogHelper.WriteLog<List<WarehouseRefund>>("warehouseRefunds", warehouseRefunds);
            //            }
            //            catch
            //            {
            //                ;
            //            }
            //#endif
            //是否已确认财务收款记录
            bool hasConsureSO = false;
            foreach (WarehouseRefund warehouseRefund in warehouseRefunds)
            {
                ShipFeeEntity shipFee = invoiceLogs.Find(p => p.WarehouseNumber == warehouseRefund.WarehouseNumber);
                if (shipFee == null)
                {
                    if (warehouseRefund.WarehouseNumber != "00")
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_NoExistsShipInvoiceLog"));
                    }
                    else
                    {
                        //HACK:Bob.H.Li 直接返回null ,如何知道没有创建成功的原因。原先的代码有问题??
                        //#if DEBUG
                        //                        LogHelper.WriteLog("sosysNO:{0}ShipFeeEntity= null", refund.SOSysNo);
                        //#endif
                        return null;

                    }
                }

                //2011.0427  解决RMA物流拒收以后，还是给客户加了积分（因为财务收款纪录是后期确认的，
                //确认后客户积分才会有值）
                if (!hasConsureSO)
                {
                    SetSOIncomeForSO();
                    hasConsureSO = true;
                }

                #region [设置  SetSOIncomeForSO 需要重新计算客户积分(2011.04.28)]
                if (soInfo.BaseInfo.GainPoint > soInfo.BaseInfo.PointPay)
                {
                    this.customerValidScore = ExternalDomainBroker.GetCustomerInfo(soInfo.BaseInfo.CustomerSysNo.Value).ValidScore ?? 0;
                    if (this.customerValidScore <= 0)
                    {
                        this.customerValidScore = 0;
                    }
                }
                #endregion
                List<RefundItemInfo> refundItems = CreateRefundItems(warehouseRefund);
                RefundInfo refundMaster = CreateRefundMaster(warehouseRefund, refundItems, shipFee);


                PersistRefund(refundMaster);

                //更新SO_Master BOb.H.Li
                UpdateSOMasterState(refund.SOSysNo.Value);

                //扣减用户经验值 Bob.H.Li
                ReduceCustomerExperience(refund.SOSysNo.Value, this.soInfo);

                refundMasters.Add(refundMaster);

                //更新单件状态
                UpdateRegisterStatus(warehouseRefund);

                //更新申请单状态
                UpdateRequestStatus(refund.SOSysNo.Value, RMARequestStatus.Complete);

                CreateSOIncomeRefundInfo(refundMaster);

                CreateSOIncomeForRO(refundMaster);

                BatchCreateConsignToAcc(refundMaster, warehouseNumber, vendorSysNo);

                SendMessageToSSB(refundMaster, warehouseNumber);

                WriteCreateRefundLog(refundMaster);
            }

            //返余额
            RefundPrepay(refundMasters);
            //返礼品卡
            RefundGiftCard(refundMasters);

            this.soPaid = true;

            //返积分
            RefundPoint(refundMasters);

            return refundMasters;
        }

        /// <summary>
        /// TODO:扣减用户的经验值
        /// </summary>
        /// <returns></returns>        
        private void ReduceCustomerExperience(int soSysNo, SOInfo orderInfo)
        {
            decimal? experience = orderInfo.BaseInfo.CashPay + orderInfo.BaseInfo.PayPrice + orderInfo.BaseInfo.ShipPrice + orderInfo.BaseInfo.PremiumAmount + orderInfo.BaseInfo.PromotionAmount;
            experience = -experience;

            ExternalDomainBroker.AdjustCustomerExperience(orderInfo.BaseInfo.CustomerSysNo.Value, experience.Value, ExperienceLogType.LogisticsRejected, "物流拒收扣减用户经验值");
        }

        private void UpdateSOMasterState(int soSysNo)
        {
            ExternalDomainBroker.UpdateSOStatusToReject(soSysNo);
        }

        #endregion

        public bool CreateRMAAutoRefund(int soNumber, string memo, int warehouseNumber, int userSysNo, int vendorSysNo)
        {
            bool result = false;
            AutoRefundInfo refundForRMAAuto = new AutoRefundInfo();
            refundForRMAAuto.IsCombinedSO = 0;
            refundForRMAAuto.Memo = memo;
            refundForRMAAuto.SOSysNo = soNumber;
            refundForRMAAuto.WarehouseNumber = warehouseNumber;
            List<RefundInfo> list = null;

            list = CreateRefund(refundForRMAAuto, warehouseNumber, vendorSysNo);

            if (list != null)
            {
                result = true;
            }

            return result;
        }
    }
}
