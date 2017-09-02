using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity;
using System.Data;
using System.Transactions;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.PO;
using System.Data.SqlClient;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(GiftCardProcessor))]
    public class GiftCardProcessor
    {
        private readonly IGiftCardDA giftCardDA = ObjectFactory<IGiftCardDA>.Instance;

        public virtual string GiftCardRMARefund(List<GiftCard> cardList, string companyCode)
        {
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "RMARefund",
                    Version = "V1",
                    From = "IPP.Service",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    Memo = string.Empty,
                    GiftCard = cardList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual string GiftCardSplitSO(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode)
        {
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "SplitSO",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    CustomerSysNo = customerSysNo,
                    ReferenceSOSysNo = mainSOSysNo,
                    GiftCard = cardList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual string GiftCardSplitSORollback(int mainSOSysNo, int customerSysNo, List<GiftCard> cardList, string companyCode)
        {
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "SplitSORollback",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    CustomerSysNo = customerSysNo,
                    ReferenceSOSysNo = mainSOSysNo,
                    GiftCard = cardList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual string CancelUsedGiftCard(int soSysNo, string companyCode)
        {
            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>(){
                new GiftCard()
                {                    
                    ReferenceSOSysNo= soSysNo,                    
                }
            };
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "VoidSO",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    GiftCard = GiftCardsElement
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual string MandatoryVoidGiftCard(List<string> cardList, string companyCode)
        {
            if (cardList == null || cardList.Count == 0)
            {
                return string.Empty;
            }
            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>();
            foreach (var item in cardList)
            {
                GiftCardsElement.Add(
                    new GiftCard()
                    {
                        Code = item
                    }
                    );
            }
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "MandatoryVoid",
                    Version = "V1",
                    From = "IPP.Service",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    Memo = ResouceManager.GetMessageString("IM.Product", "Note"),
                    GiftCard = GiftCardsElement
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);
            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual string CreateElectronicGiftCard(int soSysNo, int customerSysNo, int quantity, decimal cashAmt, GiftCardType internalType, string source, string memo, string companyCode)
        {
            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>(){
                new GiftCard()
                {                    
                    ReferenceSOSysNo= soSysNo,
                    CustomerSysNo = customerSysNo,
                    InternalType=(int)internalType,
                    ItemInfo = new ItemInfo()
                    {
                        Item = new List<Item>() { 
                            new Item(){
                                TotalAmount=cashAmt,
                                Quantity=quantity
                            }
                        }
                    }
                }
            };
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "ActiveElectronicCard",
                    Version = "V1",
                    From = source == null ? "IPP.Service" : source,
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = companyCode,
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    Memo = memo,
                    GiftCard = GiftCardsElement
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        public virtual GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type)
        {
            return giftCardDA.GetGiftCardInfoByReferenceSOSysNo(soSysNo, customerSysNo, internalType, type);
        }

        public virtual List<GiftCardInfo> GetGiftCardsByCodeList(List<string> codeList)
        {
            return giftCardDA.GetGiftCardsByCodeList(codeList);
        }

        public virtual List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType)
        {
            return giftCardDA.GetGiftCardInfoBySOSysNo(soSysNo, internalType);
        }

        public virtual List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType)
        {
            return giftCardDA.GetGiftCardRedeemLog(actionSysNo, actionType);
        }

        /// <summary>
        /// 礼品卡扣减接口(改单时使用）
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public virtual string GiftCardDeduction(List<GiftCard> usedGiftCardList, string companyCode)
        {
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "ChangeSO",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    GiftCard = usedGiftCardList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        /// <summary>
        /// 订单创建使用礼品卡使用接口
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public virtual string GiftCardConsume(List<GiftCard> usedGiftCardList, string companyCode)
        {
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "SOConsume",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    GiftCard = usedGiftCardList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        /// <summary>
        /// 礼品卡使用接口(创建订单使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        public virtual string GiftCardConsumeForSOCreate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            #region 计算单张礼品卡消费金额

            decimal needPayAmt = giftCardPay;
            for (int i = 0; i < usedGiftCardList.Count; i++)
            {
                if (needPayAmt <= 0)
                {
                    usedGiftCardList.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!usedGiftCardList[i].Amount.HasValue || usedGiftCardList[i].Amount == 0)
                {
                    //当前卡足够支付，直接在当前卡中减掉使用额度
                    if (usedGiftCardList[i].AvailAmount >= needPayAmt)
                    {
                        usedGiftCardList[i].Amount = needPayAmt;
                        usedGiftCardList[i].AvailAmount -= needPayAmt;
                        needPayAmt = 0;
                    }
                    else if (usedGiftCardList[i].AvailAmount != 0)
                    {
                        usedGiftCardList[i].Amount = usedGiftCardList[i].AvailAmount;
                        usedGiftCardList[i].AvailAmount = 0;
                        needPayAmt -= usedGiftCardList[i].Amount.Value;
                    }
                }
            }

            List<GiftCard> reqList = new List<GiftCard>();
            foreach (var item in usedGiftCardList)
            {
                reqList.Add(new GiftCard
                {
                    Code = item.Code,
                    ReferenceSOSysNo = item.ActionSysNo.Value,
                    CustomerSysNo = item.CustomerSysNo.Value,
                    ConsumeAmount = item.Amount.Value
                });
            }
            #endregion

            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "SOConsume",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    GiftCard = reqList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        /// <summary>
        /// 礼品卡使用接口(更新订单使用 更新更新礼品卡使用方式为 先作废礼品卡使用  在创建礼品卡使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        public virtual string GiftCardVoidForSOUpdate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            List<GiftCard> reqList = new List<GiftCard>();
            foreach (var item in usedGiftCardList)
            {
                reqList.Add(new GiftCard
                {
                    Code = item.Code,
                    ReferenceSOSysNo = item.ActionSysNo.Value,
                    CustomerSysNo = item.CustomerSysNo.Value,
                    ConsumeAmount = item.Amount.Value
                });
            }
            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "VoidSO",
                    Version = "V1",
                    From = "IPP.Order",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    CompanyCode = "8601",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    GiftCard = reqList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);

            return giftCardDA.OperateGiftCard(paramXml);
        }

        #region bober add

        /// <summary>
        /// 批量强制失效
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetGiftCardInvalid(List<GiftCardInfo> items)
        {
            int successCount = 0;

            foreach (GiftCardInfo obj in items)
            {
                if (!obj.SysNo.HasValue) continue;

                GiftCardInfo item = giftCardDA.LoadGiftCardInfo(obj.SysNo.Value);

                if (item.Status != GiftCardStatus.Valid || item.AvailAmount <= 0) continue;

                if (!CheckGiftCardData(item).Equals(string.Empty)) continue;

                if (string.IsNullOrEmpty(giftCardDA.OperateGiftCardStatus("MandatoryVoid", item).Trim())) successCount++;

            }

            throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardExpireReult"), successCount, items.Count - successCount));
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="items"></param>
        public virtual void SetGiftCardInvalid(GiftCardInfo item)
        {
            if (item == null || !item.SysNo.HasValue) throw new BizException(ResouceManager.GetMessageString("IM.Product", "TheRecordNotExists"));
            item = giftCardDA.LoadGiftCardInfo(item.SysNo.Value);
            if (item == null) throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "CardNumberTheRecordNotExists"), item.CardCode));
            if (item.Status != GiftCardStatus.Valid) throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "CardNumberStatusChanged"), item.CardCode));
            if (item.AvailAmount <= 0) throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "CardNumberAmountLessZero"), item.CardCode));
            string message = CheckGiftCardData(item);
            if (!string.IsNullOrEmpty(message)) throw new BizException(message);
            message = giftCardDA.OperateGiftCardStatus("MandatoryVoid", item);
            if (!string.IsNullOrEmpty(message.Trim())) throw new BizException(message);
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchLockGiftCard(List<int> items)
        {
            int successCount = 0;

            foreach (int sysNo in items)
            {
                GiftCardInfo item = giftCardDA.LoadGiftCardInfo(sysNo);
                if (item.IsHold == ECCentral.BizEntity.MKT.YNStatus.Yes)
                {

                    continue;
                }

                if (!CheckGiftCardData(item).Equals(string.Empty))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(giftCardDA.OperateGiftCardStatus("Hold", item)))
                {
                    successCount++;
                }

            }

            throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardLockedResult"), successCount, items.Count - successCount));
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateGiftCardInfo(ECCentral.BizEntity.IM.GiftCardInfo item)
        {
            string result = string.Empty;
            result = CheckGiftCardData(item);
            if (!result.Equals(string.Empty))
            {
                throw new BizException(result);
            }

            result = giftCardDA.OperateGiftCardStatus("AdjustExpireDate", item);//

            if (string.IsNullOrEmpty(result))
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CardChangeExpireFail"));
        }

        /// <summary>
        /// 批量解锁
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchUnLockGiftCard(List<int> items)
        {
            int successCount = 0;

            foreach (int sysNo in items)
            {
                GiftCardInfo item = giftCardDA.LoadGiftCardInfo(sysNo);
                if (item.IsHold == ECCentral.BizEntity.MKT.YNStatus.No)
                {
                    continue;
                }

                if (!CheckGiftCardData(item).Equals(string.Empty))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(giftCardDA.OperateGiftCardStatus("UnHold", item)))
                {
                    successCount++;
                }

            }

            throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardUnLockedResult"), successCount, items.Count - successCount));
        }



        public virtual void ActiveGiftCard(int sysNo)
        {
            GiftCardInfo item = giftCardDA.LoadGiftCardInfo(sysNo);
            // OY 没有锁定功能
            //if (item.IsHold == ECCentral.BizEntity.MKT.YNStatus.No)
            //{
            //    return;
            //}

            if (item.Status == GiftCardStatus.Valid)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardActiveResult1"), item.CardCode));
            }

            if (item.Status == GiftCardStatus.ManualActive)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardActiveResult2"), item.CardCode));
            }

            if (item.Status == GiftCardStatus.Used)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardActiveResult3"), item.CardCode));
            }

            if (item.Status == GiftCardStatus.Void)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardActiveResult4"), item.CardCode));
            }


            //xmlMessage
            List<GiftCard> GiftCardsElement = new List<GiftCard>();
            GiftCardsElement.Add(new GiftCard { Code = item.CardCode });

            Message message = new Message()
            {
                Header = new Header()
                {
                    Action = "ActiveGiftVoucher",
                    Version = "V1",
                    From = "IPP.Service",
                    CurrencySysNo = "1",
                    Language = "zh-CN",
                    StoreCompanyCode = "8601"
                },
                Body = new Body()
                {
                    EditUser = ServiceContext.Current.UserSysNo.ToString(),
                    Memo = ResouceManager.GetMessageString("IM.Product", "Note"),
                    GiftCard = GiftCardsElement
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(message);
            string temp = giftCardDA.OperateGiftCard(paramXml);
        }

        private string CheckGiftCardData(GiftCardInfo item)
        {
            StringBuilder strMessage = new StringBuilder();
            if (!item.PreEndDate.HasValue)
            {
                strMessage.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardCheckDataResult1"), item.CardCode));
            }

            if (!item.BeginDate.HasValue)
            {
                strMessage.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardCheckDataResult2"), item.CardCode));
            }

            if (!item.EndDate.HasValue)
            {
                strMessage.AppendLine(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardCheckDataResult3"), item.CardCode));
            }

            return strMessage.ToString();
        }


        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual List<GiftCardOperateLog> GetGiftCardOperateLogByCode(string code)
        {
            return giftCardDA.GetGiftCardOperateLogByCode(code);
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual List<GiftCardRedeemLog> GetGiftCardRedeemLogJoinSOMaster(string code)
        {
            return giftCardDA.GetGiftCardRedeemLogJoinSOMaster(code);
        }

        public virtual List<GiftCardFabrication> GetGiftCardFabricationItem(int sysNo)
        {
            int exchangeRate = string.IsNullOrEmpty(AppSettingManager.GetSetting("MKT", "CurrencySysNoForGiftCardFabrication")) ? 1 : int.Parse(AppSettingManager.GetSetting("MKT", "CurrencySysNoForGiftCardFabrication"));
            int payTypeSysNo = string.IsNullOrEmpty(AppSettingManager.GetSetting("MKT", "VendorSysNoForGiftCardFabrication")) ? 1605 : int.Parse(AppSettingManager.GetSetting("MKT", "VendorSysNoForGiftCardFabrication"));

            decimal d = giftCardDA.GetItemExchangeRate(exchangeRate);
            int i = giftCardDA.GetItemPayTypeSysNo(payTypeSysNo);
            List<GiftCardFabrication> list = giftCardDA.GetGiftCardFabricationItem(sysNo, d, i);
            return list;
        }

        public virtual DataTable GetGiftCardFabricationItemSum(int sysNo)
        {
            return giftCardDA.GetGiftCardFabricationItemSum(sysNo);
        }

        /// <summary>
        /// 更新礼品卡主体信息及其子项信息
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateGiftCardFabrications(GiftCardFabricationMaster item)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                int sysNo = 0;
                if (!item.SysNo.HasValue || item.SysNo.Value == 0)
                    sysNo = giftCardDA.InsertGiftCardFabricationMaster(item);
                else
                    giftCardDA.UpdateGiftCardFabricationMaster(item);

                //if (item.SysNo.Value == 0)
                //    gift.MasterSysNo = sysNo;

                foreach (GiftCardFabrication gift in item.GiftCardFabricationList)
                {
                    if (!item.SysNo.HasValue || item.SysNo.Value == 0)//!gift.MasterSysNo.HasValue || gift.MasterSysNo.Value == 0)
                    {
                        gift.MasterSysNo = sysNo;
                        giftCardDA.InsertGiftCardFabricationItem(gift);
                    }
                    else
                        giftCardDA.UpdateGiftCardFabricationItem(gift);
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteGiftCardFabrication(int sysNo)
        {
            giftCardDA.DeleteGiftCardFabrication(sysNo);
        }

        public virtual int CreatePOGiftCardFabrication(GiftCardFabricationMaster item)
        {

            BizEntity.PO.PurchaseOrderInfo newPOInfo = new BizEntity.PO.PurchaseOrderInfo();
            newPOInfo.CompanyCode = item.CompanyCode;
            newPOInfo.PurchaseOrderBasicInfo.IsManagerPM = item.IsManagerPM;
            List<PurchaseOrderItemInfo> poItems = new List<PurchaseOrderItemInfo>();
            foreach (GiftCardFabrication gift in item.GiftCardFabricationList)
            {
                PurchaseOrderItemInfo poItem = new PurchaseOrderItemInfo();
                poItem.OrderPrice = 0;
                poItem.ProductSysNo = gift.Product.SysNo;
                poItem.PurchaseQty = gift.Quantity.Value;
                poItem.ProductID = gift.Product.ProductID;
                poItem.ApportionAddOn = 0;
                poItem.ReturnCost = 0;
                poItem.Quantity = 0;
                poItem.Weight = 0;
                poItem.BriefName = ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(gift.Product.SysNo).ProductBasicInfo.ProductBriefName;//gift.Product.ProductBasicInfo.ProductBriefName; 
                poItem.UnitCost = 0;
                poItem.CompanyCode = item.CompanyCode;
                poItems.Add(poItem);
            }
            newPOInfo.POItems = poItems;
            #region VendorInfo

            VendorInfo vendor = new VendorInfo();
            vendor.CompanyCode = item.CompanyCode;
            vendor.SysNo = int.Parse(AppSettingManager.GetSetting("MKT", "VendorSysNoForGiftCardCreatePO"));

            VendorPayTermsItemInfo payItemInfo = new VendorPayTermsItemInfo();
            payItemInfo.PayTermsNo = 0;
            VendorFinanceInfo fin = new VendorFinanceInfo();
            fin.PayPeriodType = payItemInfo;
            vendor.VendorFinanceInfo = fin;

            vendor.VendorBasicInfo.PaySettleCompany = PaySettleCompany.SH;

            newPOInfo.VendorInfo = vendor;
            #endregion

            newPOInfo.PurchaseOrderBasicInfo.CreateUserSysNo = ServiceContext.Current.UserSysNo;

            #region PurchaseOrderBasicInfo

            PurchaseOrderBasicInfo poBasicInfo = new PurchaseOrderBasicInfo();
            poBasicInfo.PurchaseOrderLeaseFlag = PurchaseOrderLeaseFlag.unLease;
            poBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.UnConsign;//不确定--来自portal
            poBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Origin;
            poBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
            poBasicInfo.TaxRate = 0.17M;
            poBasicInfo.PurchaseStockSysNo = int.Parse(AppSettingManager.GetSetting("MKT", "StockSysNoForGiftCardFabrication"));
            poBasicInfo.StockInfo = new BizEntity.Inventory.StockInfo() { SysNo = int.Parse(AppSettingManager.GetSetting("MKT", "StockSysNoForGiftCardFabrication")) };//51
            poBasicInfo.CurrencyCode = 1;
            poBasicInfo.SettleCompanySysNo = int.Parse(AppSettingManager.GetSetting("MKT", "SettlementCompanyForGiftCardFabrication"));//3201

            int exchangeRate = string.IsNullOrEmpty(AppSettingManager.GetSetting("MKT", "CurrencySysNoForGiftCardFabrication")) ? 1 : int.Parse(AppSettingManager.GetSetting("MKT", "CurrencySysNoForGiftCardFabrication"));
            poBasicInfo.ExchangeRate = giftCardDA.GetItemExchangeRate(exchangeRate);// 0;//来自portal    ExchangeRate

            //int payTypeSysNo = string.IsNullOrEmpty(AppSettingManager.GetSetting("MKT", "VendorSysNoForGiftCardCreatePO")) ? 12 : int.Parse(AppSettingManager.GetSetting("MKT", "VendorSysNoForGiftCardCreatePO"));
            ECCentral.BizEntity.Common.PayType payType = new BizEntity.Common.PayType();
            payType.SysNo = 12;// giftCardDA.GetItemPayTypeSysNo(payTypeSysNo);// 12;//来自portal          PayTypeSysNo
            poBasicInfo.PayType = payType;


            ProductManagerInfo pmInfo = new ProductManagerInfo();
            pmInfo.SysNo = item.GiftCardFabricationList.FirstOrDefault().PMUserSysNo;//来自portal    PMUserSysNo
            poBasicInfo.ProductManager = pmInfo;

            ECCentral.BizEntity.Common.ShippingType shippingType = new BizEntity.Common.ShippingType();
            shippingType.SysNo = 12;
            poBasicInfo.ShippingType = shippingType;

            newPOInfo.PurchaseOrderBasicInfo = poBasicInfo;
            PurchaseOrderETATimeInfo eta = new PurchaseOrderETATimeInfo();
            eta.InDate = DateTime.Now;
            newPOInfo.PurchaseOrderBasicInfo.ETATimeInfo = eta;
            PurchaseOrderMemoInfo memo = new PurchaseOrderMemoInfo();
            newPOInfo.PurchaseOrderBasicInfo.MemoInfo = memo;
            newPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress = "-999";
            #endregion
            PurchaseOrderEIMSInfo eim = new PurchaseOrderEIMSInfo();
            newPOInfo.EIMSInfo = eim;

            string newPOSysNo = ExternalDomainBroker.CreatePurchaseOrder(newPOInfo);

            if (!string.IsNullOrEmpty(newPOSysNo))
            {
                item.POSysNo = int.Parse(newPOSysNo);
                giftCardDA.CreatePOGiftCardFabrication(item);
                return item.POSysNo.Value;
            }
            else
                return 0;
        }

        /// <summary>
        /// 导出制卡操作
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual bool GetAddGiftCardInfoList(int sysNo)
        {
            DataTable dt = giftCardDA.GetGiftCardFabricationItemSum(sysNo);
            if (dt != null && dt.Rows.Count > 0)
            {
                decimal decTotalCount = decimal.Parse(dt.Rows[0][0].ToString());
                decimal totalPass = giftCardDA.GetAddGiftCardInfoList(sysNo);
                if (decTotalCount == totalPass)
                {
                    giftCardDA.UpdateGiftCardInfoStatus(sysNo);//制卡成功后改变其状态
                    ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("IM.Product", "GiftCardExport"), BizLogType.Basic_GiftCard_Info_CreateOK, sysNo, "");//CompanyCode
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 获取当前生成的需要导出的礼品卡信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual DataTable GetGiftCardInfoByGiftCardFabricationSysNo(int sysNo)
        {
            DataTable dt = giftCardDA.GetGiftCardInfoByGiftCardFabricationSysNo(sysNo);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["Password"] = ECCentral.Service.Utility.CryptoManager.Decrypt(dr["Password"].ToString());
                }
            }
            return dt;
        }

        #region GiftVoucher

        /// <summary>
        /// 新增礼品券商品
        /// </summary>
        /// <param name="entity"></param>
        public virtual int AddGiftVoucher(GiftVoucherProduct entity)
        {
            PreCheckSave(entity);

            using (TransactionScope ts = new TransactionScope())
            {
                int voucherSysNo = SaveGiftVoucherProduct(entity);

                if (entity.RelationProducts != null && entity.RelationProducts.Count > 0)
                {
                    entity.RelationProducts.ForEach(p =>
                    {
                        p.GiftVoucherSysNo = voucherSysNo;
                        p.AuditStatus = GVRReqAuditStatus.AuditWaitting;
                        AddGiftVoucherProductRelation(p);
                    });
                }

                ts.Complete();

                return voucherSysNo;
            }
        }



        /// <summary>
        /// 保存礼品券商品
        /// </summary>
        /// <param name="item"></param>
        public virtual int SaveGiftVoucherProduct(GiftVoucherProduct item)
        {
            item.Status = GiftVoucherProductStatus.Audit;
            int sysNo = giftCardDA.SaveGiftVoucherProduct(item);

            if (sysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CreateGiftCardFail"));
            }

            return sysNo;
        }
        /// <summary>
        /// 更新礼品券状态
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateVoucherProductStatus(GiftVoucherProduct item)
        {
            giftCardDA.UpdateVoucherProduct(item);
        }

        /// <summary>
        /// 获取礼品券商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual GiftVoucherProduct GetVoucherProductBySysNo(int sysNo)
        {
            GiftVoucherProduct vProduct = giftCardDA.GetVoucherProductBySysNo(sysNo);
            if (vProduct != null)
                vProduct.RelationProducts = giftCardDA.GetVoucherProductRelationByVoucher(vProduct.SysNo);

            if (vProduct.RelationProducts != null && vProduct.RelationProducts.Count > 0)
            {
                foreach (var relation in vProduct.RelationProducts)
                {
                    // 分别调用考虑分页
                    //relation.RequestLogs = new List<GiftVoucherProductRelationRequest>();

                    //relation.RequestLogs = GetGiftVoucherProductRelationRequest(relation.SysNo);
                }
            }
            return vProduct;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="item"></param>
        public virtual GiftVoucherProduct UpdateVoucherProductInfo(GiftVoucherProduct item)
        {
            PreCheckUpdate(item);

            GiftVoucherProduct vOldProduct = GetVoucherProductBySysNo(item.SysNo);

            List<GiftVoucherProductRelation> deleteList = new List<GiftVoucherProductRelation>();
            List<GiftVoucherProductRelation> addList = new List<GiftVoucherProductRelation>();

            // Type Add
            foreach (var newRelation in item.RelationProducts)
            {
                int iCount = vOldProduct.RelationProducts.Where(old => old.ProductSysNo == newRelation.ProductSysNo).Count();
                GiftVoucherProductRelation oldRelation = vOldProduct.RelationProducts.Where(old => old.ProductSysNo == newRelation.ProductSysNo).FirstOrDefault();
                int iReq = 0;
                if (null != oldRelation)
                {
                    iReq = GetGiftVoucherProductRelationRequest(oldRelation.SysNo).Where(old => old.AuditStatus == GVRReqAuditStatus.AuditWaitting).Count();
                }
                //if (iReq > 0)
                //{
                //    throw new BizException(string.Format("编号【{0}】的商品还处于等待审核状态，不能进行下一步操作！", oldRelation.ProductSysNo));
                //}

                if (iCount > 0 && newRelation.Type == GVRReqType.Delete && iReq <= 0)
                {
                    // old exist new
                    newRelation.GiftVoucherSysNo = item.SysNo;
                    newRelation.Status = GiftVoucherRelateProductStatus.Deactive;



                    deleteList.Add(newRelation);
                }
                else if (iCount <= 0)
                {
                    // old not exist in new list
                    newRelation.Type = GVRReqType.Add;
                    newRelation.GiftVoucherSysNo = item.SysNo;
                    addList.Add(newRelation);
                }
            }

            // Check 




            using (TransactionScope ts = new TransactionScope())
            {
                // update voucher product
                giftCardDA.UpdateVoucherProduct(item);

                // update relation product giftvouchertype
                item.RelationProducts.ForEach(p => 
                {
                    ObjectFactory<IGiftCardDA>.Instance.UpdateProductGiftVoucherType(p.ProductSysNo, p.GiftVoucherType);
                });

                if (addList.Count > 0)
                {
                    addList.ForEach(p =>
                    {
                        //// 不同礼品券不能包含相同的兑换商品（保存或更新操作会更新Product的GiftVoucherType字段）
                        if (giftCardDA.IsExistsSameRelationProduct(p.ProductSysNo))
                        {
                            throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "ExchangeProductIsExchangeProduct") + "\r\n", p.ProductSysNo));
                        }

                        p.Type = GVRReqType.Add;
                        p.AuditStatus = GVRReqAuditStatus.AuditWaitting;
                        AddGiftVoucherProductRelation(p);
                    });
                }

                // if exist delete product send delete request
                // write relation request
                if (deleteList != null && deleteList.Count > 0)
                {
                    // add a delete request log
                    // write relation request

                    deleteList.ForEach(p =>
                    {
                        p.Type = GVRReqType.Delete;
                        p.AuditStatus = GVRReqAuditStatus.AuditWaitting;
                        giftCardDA.SaveGiftVoucherProductRelationRequest(new GiftVoucherProductRelationRequest()
                        {
                            RelationSysNo = p.SysNo,
                            Type = p.Type,
                            AuditStatus = p.AuditStatus
                        });
                    });
                }
                ts.Complete();
            }

            return GetVoucherProductBySysNo(item.SysNo);
        }

        /// <summary>
        /// 审核礼品券商品
        /// </summary>
        /// <param name="item"></param>
        public virtual void AuditVoucherProduct(GiftVoucherProduct item)
        {
            if (item != null && item.SysNo > 0)
            {
                GiftVoucherProduct info = GetVoucherProductBySysNo(item.SysNo);

                if (info.Status == GiftVoucherProductStatus.Audit)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "AuditVoucherProductResult1"));
                }
                if (info.Status == GiftVoucherProductStatus.Void)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "AuditVoucherProductResult2"));
                }

                item.Status = GiftVoucherProductStatus.Audit;

                UpdateVoucherProductStatus(item);
            }
        }

        public virtual void CancelAuditVoucherProduct(GiftVoucherProduct item)
        {
            if (item != null && item.SysNo > 0)
            {
                GiftVoucherProduct info = GetVoucherProductBySysNo(item.SysNo);

                if (info.Status == GiftVoucherProductStatus.Audit)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "CancelAuditVoucherProductResult1"));
                }
                if (info.Status == GiftVoucherProductStatus.Void)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Product", "CancelAuditVoucherProductResult2"));
                }

                item.Status = GiftVoucherProductStatus.AuditFail;

                UpdateVoucherProductStatus(item);
            }
        }

        public virtual void VoidVoucherProduct(GiftVoucherProduct item)
        {
            if (item != null && item.SysNo > 0)
            {
                GiftVoucherProduct info = GetVoucherProductBySysNo(item.SysNo);

                //if (info.Status == GiftVoucherProductStatus.Audit)
                //{
                //    throw new BizException(ResouceManager.GetMessageString("IM.Product", "VoidVoucherProductResult"));
                //}

                item.Status = GiftVoucherProductStatus.Void;

                UpdateVoucherProductStatus(item);
            }
        }

        /// <summary>
        /// 增加礼品券商品关联
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddGiftVoucherProductRelation(GiftVoucherProductRelation item)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                // write relation
                item.Status = GiftVoucherRelateProductStatus.Deactive;
                int sysNo = giftCardDA.SaveGiftVoucherProductRelation(item);

                // write relation request
                giftCardDA.SaveGiftVoucherProductRelationRequest(new GiftVoucherProductRelationRequest()
                {
                    RelationSysNo = sysNo,
                    Type = item.Type,
                    AuditStatus = item.AuditStatus
                });
                ts.Complete();
            }
        }

        /// <summary>
        /// 删除礼品券商品关联
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteGiftVoucherProductRelation(GiftVoucherProductRelation item)
        {
            if (item != null && item.SysNo > 0)
            {
                giftCardDA.DeleteGiftVoucherProductRelationByRelation(item);
            }
        }


        public virtual void UpdateGiftVoucherProductRelationRequestStatus(GiftVoucherProductRelationRequest request)
        {
            using (TransactionScope ts = new TransactionScope())
            {


                giftCardDA.UpdateGiftVoucherProductRelationRequestStatus(request);

                GiftVoucherProductRelation oldRelation = giftCardDA.GetVoucherProductRelationByRequest(request.SysNo);
                // update request status
                GiftVoucherRelateProductStatus status = oldRelation.Status;
                if (request.AuditStatus == GVRReqAuditStatus.AuditSuccess && request.Type == GVRReqType.Add)
                {
                    status = GiftVoucherRelateProductStatus.Active;
                }
                else if (request.AuditStatus == GVRReqAuditStatus.AuditSuccess && request.Type == GVRReqType.Delete)
                {
                    status = GiftVoucherRelateProductStatus.Deactive;
                }


                // update relation status
                if (request.Type == GVRReqType.Add)
                {
                    giftCardDA.UpdateGiftVoucherProductRelationStatus(new GiftVoucherProductRelation()
                    {
                        Status = status,
                        SysNo = request.RelationSysNo
                    });
                }
                else if (request.Type == GVRReqType.Delete && request.AuditStatus == GVRReqAuditStatus.AuditSuccess)
                {
                    // delete relation
                    giftCardDA.DeleteGiftVoucherProductReleationBySysNo(new GiftVoucherProductRelation()
                    {
                        SysNo = request.RelationSysNo
                    });
                    // update status
                    //giftCardDA.UpdateGiftVoucherProductRelationStatus(new GiftVoucherProductRelation() 
                    //{
                    //    Status = GiftVoucherRelateProductStatus.Deactive,
                    //    SysNo = request.RelationSysNo
                    //});

                }
                else if (request.Type == GVRReqType.Delete && request.AuditStatus == GVRReqAuditStatus.AuditFailed)
                {
                    // update status
                    giftCardDA.UpdateGiftVoucherProductRelationStatus(new GiftVoucherProductRelation()
                    {
                        Status = GiftVoucherRelateProductStatus.Active,
                        SysNo = request.RelationSysNo
                    });
                }

                ts.Complete();
            }
        }

        /// <summary>
        /// 审核通过礼品券商品关联
        /// </summary>
        /// <param name="request"></param>
        public virtual void AuditVoucherRelationRequest(GiftVoucherProductRelationRequest reqt)
        {
            GiftVoucherProductRelationRequest request = giftCardDA.GetGiftVoucherProductRelationRequestBySysNo(reqt.SysNo);

            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "AuditVoucherRelationRequestResult1"));
            }

            if (request.AuditStatus == GVRReqAuditStatus.AuditSuccess)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "AuditVoucherRelationRequestResult2"));
            }

            request.AuditStatus = GVRReqAuditStatus.AuditSuccess;
            UpdateGiftVoucherProductRelationRequestStatus(request);
        }

        /// <summary>
        /// 审核不通过礼品券商品关联
        /// </summary>
        /// <param name="request"></param>
        public virtual void CancelAuditVoucherRelationRequest(GiftVoucherProductRelationRequest reqt)
        {
            GiftVoucherProductRelationRequest request = giftCardDA.GetGiftVoucherProductRelationRequestBySysNo(reqt.SysNo);

            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "CancelAuditVoucherRelationRequestResult"));
            }

            request.AuditStatus = GVRReqAuditStatus.AuditFailed;
            UpdateGiftVoucherProductRelationRequestStatus(request);
        }

        /// <summary>
        /// 根据关联关系获取关联请求
        /// </summary>
        /// <param name="relationSysNo"></param>
        /// <returns></returns>
        public virtual List<GiftVoucherProductRelationRequest> GetGiftVoucherProductRelationRequest(int relationSysNo)
        {
            return giftCardDA.GetGiftVoucherProductRelationRequestByRelation(relationSysNo);
        }

        private void PreCheckCommon(GiftVoucherProduct item)
        {
            if (item == null || item.ProductSysNo <= 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckCommon1"));
            }

            if (item.RelationProducts != null && item.RelationProducts.Count > 0)
            {
                foreach (var tem in item.RelationProducts)
                {
                    if (item.ProductSysNo == tem.ProductSysNo)
                    {
                        throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckCommon2"), item.ProductID));
                    }
                }
            }

            if (giftCardDA.IsExistSameGiftVoucherPrice(item))
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckCommon3"), item.Price));
            }
            

        }

        private void PreCheckSave(GiftVoucherProduct item)
        {
            PreCheckCommon(item);

            bool isExists = giftCardDA.IsExistsSameProduct(item);
            if (isExists)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckSave1"), item.ProductID));
            }

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckSave2"));
            int len = strBuilder.Length;

            if (item.RelationProducts != null && item.RelationProducts.Count > 0)
            {
                foreach (var tem in item.RelationProducts)
                {
                    // 不同礼品券不能包含相同的兑换商品（保存或更新操作会更新Product的GiftVoucherType字段）
                    if (giftCardDA.IsExistsSameRelationProduct(tem.ProductSysNo))
                    {
                        strBuilder.AppendFormat(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckSave3") + "\r\n", tem.ProductSysNo);
                    }
                }

                if (strBuilder.Length > len)
                {
                    throw new BizException(strBuilder.ToString());
                }
            }
        }

        private void PreCheckUpdate(GiftVoucherProduct item)
        {
            PreCheckCommon(item);
            GiftVoucherProduct old = GetVoucherProductBySysNo(item.SysNo);

            if (old.ProductSysNo != item.ProductSysNo)
            {
                bool isExists = giftCardDA.IsExistsSameProduct(item);
                if (isExists)
                {
                    throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "GiftCardPreCheckUpdate"), item.ProductID));
                }
            }


        }
        #endregion
    }
}
