using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISODA))]
    public partial class SODA : ISODA
    {
        #region 特殊的Mapping方法

        /// <summary>
        /// 现在的订单状态Mapping到IPP的订单状态
        /// </summary>
        /// <param name="status">现在的订单状态</param>
        /// <param name="operatorType">操作者类型</param>
        /// <param name="isBackOrder">是否是BackOrder:由SOBaseInfo.IsBackOrder标识</param>
        /// <returns></returns>
        internal static int Mapping_SOStatus_ThisToIPP(SOStatus status, SOOperatorType operatorType, bool isBackOrder)
        {
            int ippSOStatus = 0;
            switch (status)
            {
                case SOStatus.Abandon:
                    {
                        ippSOStatus = -1;
                        switch (operatorType)
                        {
                            case SOOperatorType.Customer:
                                ippSOStatus = -2;
                                break;
                            case SOOperatorType.System:
                                ippSOStatus = -4;
                                break;
                        }
                        break;
                    }
                case SOStatus.Origin:
                    ippSOStatus = isBackOrder ? -5 : 0;
                    break;
                case SOStatus.WaitingOutStock:
                    ippSOStatus = 1;
                    break;
                case SOStatus.WaitingManagerAudit:
                    ippSOStatus = 3;
                    break;
                case SOStatus.OutStock:
                    ippSOStatus = 4;
                    break;
                //case SOStatus.Complete:
                //    ippSOStatus = 4;
                //    break;
                case SOStatus.Shipping:
                    ippSOStatus = 4;
                    break;
                case SOStatus.Split:
                    ippSOStatus = -6;
                    break;
                case SOStatus.Reject:
                    ippSOStatus = 6;
                    break;
                case SOStatus.Reported:
                    ippSOStatus = 41;
                    break;
                case SOStatus.CustomsPass:
                    ippSOStatus = 45;
                    break;
                case SOStatus.CustomsReject:
                    ippSOStatus = 65;
                    break;
                case SOStatus.Complete:
                    ippSOStatus = 5;
                    break;
                case SOStatus.ShippingReject:
                    ippSOStatus = 7;
                    break;
                default:
                    {
                        ippSOStatus = (int)status;
                        break;
                    }
            }
            return ippSOStatus;
        }

        /// <summary>
        ///  IPP的订单状态Mapping到现在的订单状态
        /// </summary>
        /// <param name="ippStatus"></param>
        /// <returns></returns>
        internal static SOStatus Mapping_SOStatus_IPPToThis(int ippStatus, bool isAutoRMA, bool isCombine, bool isMergeComplete)
        {
            SOStatus status;
            switch (ippStatus)
            {
                case -6://已拆分
                    status = SOStatus.Split;
                    break;
                case -5://BackOrder
                    status = SOStatus.Origin;
                    break;
                case -4://
                    status = SOStatus.SystemCancel;
                    break;
                case -1://
                    status = SOStatus.Abandon;
                    break;
                case 0://待审核
                    status = SOStatus.Origin;
                    break;
                case 1://待出库
                    status = SOStatus.WaitingOutStock;
                    break;
                case 2://待支付
                    status = SOStatus.Origin;
                    break;
                case 3://待主管审核
                    status = SOStatus.WaitingManagerAudit;
                    break;
                case 4://已出库
                    //if (isAutoRMA)
                    //{
                    //    status = SOStatus.Reject;
                    //}
                    //else if (isCombine && !isMergeComplete)
                    //{
                    //    status = SOStatus.Shipping;
                    //}
                    //else
                    //{
                        status = SOStatus.OutStock;
                    //}
                    break;
                case 5://已完成
                    status = SOStatus.Complete;
                    break;
                case 41://已报关
                    status = SOStatus.Reported;
                    break;
                case 45://已通关发往顾客
                    status = SOStatus.CustomsPass;
                    break;
                case 6://申报失败订单作废
                    status = SOStatus.Reject;
                    break;
                case 65://通关失败订单作废
                    status = SOStatus.CustomsReject;
                    break;
                case 7://物流作废
                    status = SOStatus.ShippingReject;
                    break;
                default://已作废
                    status = SOStatus.Abandon;
                    break;
            }
            return status;

        }


        /// <summary>
        /// 订单商品的DiscountType(SO_Item.DiscountType) Mapping到现在的促销活动类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ECCentral.BizEntity.MKT.PromotionType Mapping_IPPDiscountTypeToPromotionType(int discountType)
        {
            return (ECCentral.BizEntity.MKT.PromotionType)discountType;
        }

        internal static int Mapping_SOHoldStatus_ThisToIPP(SOHoldStatus status)
        {
            return (int)status;
        }
        internal static SOHoldStatus Mapping_SOHoldStatus_IPPToThis(int ippHoldStatus, bool ippHoldMark)
        {
            SOHoldStatus status = (SOHoldStatus)ippHoldStatus;
            if (ippHoldStatus == -1)
            {
                status = SOHoldStatus.Unhold;
            }
            if (ippHoldMark)
            {
                status = SOHoldStatus.BackHold;
            }
            return status;
        }

        #endregion

        #region 订单查询 相关方法

        /// <summary>
        /// 整合订单编号列表
        /// </summary>
        /// <param name="soSysNos"></param>
        /// <returns></returns>
        private string SOSysNoListToString(List<int> soSysNos)
        {
            string soSysNos_Str = null;
            if (soSysNos != null && soSysNos.Count > 0)
            {
                int i = 0;
                soSysNos_Str = String.Empty;
                foreach (int no in soSysNos)
                {
                    if (no > 0)
                    {
                        soSysNos_Str += String.Format(i == 0 ? "{0}" : ",{0}", no);
                        i++;
                    }
                }
            }
            return soSysNos_Str;
        }

        /// <summary>
        /// Mapping订单基础信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isAutoRMA"></param>
        /// <param name="ippSOStatus"></param>
        /// <param name="ippCashPay"></param>
        /// <param name="ippHoldStatus"></param>
        /// <param name="ippHoldMark"></param>
        private void SOBaseInfoSpecificMapping(SOBaseInfo info, bool isAutoRMA, int ippSOStatus, decimal ippCashPay, int ippHoldStatus, bool ippHoldMark, bool isCombine, bool isMergeComplete)
        {
            info.Status = Mapping_SOStatus_IPPToThis(ippSOStatus, isAutoRMA, isCombine, isMergeComplete);

            info.PointPay = info.PointPay == null || !info.PointPay.HasValue ? 0 : info.PointPay;
            if (info.PointPay > 0)
            {
                decimal pointToMoneyRatio = decimal.TryParse(AppSettingManager.GetSetting(ECCentral.BizEntity.Customer.CustomerConst.DomainName, ECCentral.BizEntity.Customer.CustomerConst.Key_PointToMonetyRatio), out pointToMoneyRatio) ? pointToMoneyRatio : 0M;
                info.PointPayAmount = info.PointPay / pointToMoneyRatio;
            }
            else
            {
                info.PointPayAmount = 0M;
            }
            info.CouponAmount = ippCashPay - info.SOAmount + info.PointPayAmount;

            info.HoldStatus = Mapping_SOHoldStatus_IPPToThis(ippHoldStatus, ippHoldMark);
        }

        /// <summary>
        /// 填充订单基础信息
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private SOBaseInfo DataRowToSOBaseInfo(DataRow dr)
        {
            SOBaseInfo info = DataMapper.GetEntity<SOBaseInfo>(dr);
            int ippSOStatus = dr["Status"] == null || dr["Status"] == DBNull.Value ? -1 : Convert.ToInt32(dr["Status"]);
            bool isAutoRMA = dr["HaveAutoRMA"] == null || dr["HaveAutoRMA"] == DBNull.Value ? false : Convert.ToBoolean(dr["HaveAutoRMA"]);
            decimal ippCashPay = dr["CashPay"] == null || dr["CashPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CashPay"]);
            int ippHoldStatus = dr["HoldStatus"] == null || dr["HoldStatus"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HoldStatus"]);
            bool ippHoldMark = dr["HoldMark"] == null || dr["HoldMark"] == DBNull.Value ? false : Convert.ToBoolean(dr["HoldMark"]);
            bool isCombine = dr["IsCombine"] == null || dr["IsCombine"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCombine"]);
            bool isMergeComplete = dr["IsMergeComplete"] == null || dr["IsMergeComplete"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsMergeComplete"]);
            SOBaseInfoSpecificMapping(info, isAutoRMA, ippSOStatus, ippCashPay, ippHoldStatus, ippHoldMark, isCombine, isMergeComplete);
            return info;
        }

        #region 订单促销活动Mapping
        /// <summary>
        /// 订单优惠券使用Mapping
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="couponSysNo">订单使用的优惠券系统编号</param>
        private void MappingCouponToSOInfo(SOInfo soInfo)
        {
            SOItemInfo couponItem = soInfo.Items.Find(i => { return i.ProductType == SOProductType.Coupon; });
            if (couponItem != null)
            {
                SOPromotionInfo promotionInfo = new SOPromotionInfo
                {
                    PromotionType = SOPromotionType.Coupon,
                    PromotionSysNo = couponItem.ProductSysNo,
                    DiscountAmount = soInfo.BaseInfo.CouponAmount,
                    Time = 1,
                    SOSysNo = soInfo.SysNo

                };
                soInfo.Items.ForEach(item =>
                {
                    if (item.CouponAmount != 0)
                    {
                        promotionInfo.SOPromotionDetails.Add(new SOPromotionDetailInfo
                        {
                            MasterProductQuantity = item.Quantity,
                            DiscountAmount = item.CouponAmount,
                            MasterProductSysNo = item.ProductSysNo,
                            // MasterProductType = item.ProductType
                        });
                    }
                });
                soInfo.SOPromotions.Add(promotionInfo);
            }
        }

        /// <summary>
        /// itemExMapping
        /// </summary>
        /// <param name="soInfo"></param>
        private void MappingExtensionInfo(SOInfo soInfo)
        {
            if (soInfo != null && soInfo.Items != null && soInfo.Items.Count > 0)
            {
                var extensions = GetSOItemExtensionBySOSysNo(soInfo.SysNo.Value);

                if (extensions != null && extensions.Count > 0)
                {
                    foreach (var item in soInfo.Items)
                    {
                        item.ItemExtList = extensions.FindAll(x => x.ProductSysNo == item.ProductSysNo && x.SOSysNo == item.SOSysNo);
                        if (item.ItemExtList != null && item.ItemExtList.Count > 0)
                        {
                            item.SettlementStatus = item.ItemExtList[0].SettlementStatus;
                            item.ActivityType = item.ItemExtList[0].Type;
                            item.ReferenceSysNo = item.ItemExtList[0].ReferenceSysNo;
                        }
                    }
                }
            }
        }

        public List<ItemExtension> GetSOItemExtensionBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOItemExtensionBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<ItemExtension>();
        }

        public int InsertSOItemExtension(ItemExtension entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSOItemExtension");
            command.SetParameterValue("@CreateDate", DateTime.Now);
            command.SetParameterValue("@CreateUserName", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@LastEditDate", DateTime.Now);
            command.SetParameterValue("@LastEditUserName", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@ReferenceSysNo", entity.ReferenceSysNo);
            command.SetParameterValue("@SettlementStatus", LegacyEnumMapper.ConvertSettlementStatus(entity.SettlementStatus));
            command.SetParameterValue("@SOSysNo", entity.SOSysNo);
            command.SetParameterValue("@Type", LegacyEnumMapper.ConvertSOProductActivityType(entity.Type));
            command.SetParameterValue("@OriginalCurrentPrice", entity.OriginalCurrentPrice);
            return command.ExecuteNonQuery();
        }
        /// <summary>
        /// 订单使用的组合销售(销售规则)Mapping
        /// </summary>
        /// <param name="soInfo"></param>
        private void MappingComboToSOInfo(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOComboBySOSysNo");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);
            using (DataSet ds = command.ExecuteDataSet())
            {
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dtDetail = ds.Tables[1];
                    List<SOPromotionInfo> promotionList = DataMapper.GetEntityList<SOPromotionInfo, List<SOPromotionInfo>>(ds.Tables[0].Rows,
                    (dr, p) =>
                    {
                        p.PromotionType = SOPromotionType.Combo;
                        dtDetail.DefaultView.RowFilter = String.Format("PromotionSysNo={0}", p.PromotionSysNo);
                        p.SOPromotionDetails = DataMapper.GetEntityList<SOPromotionDetailInfo, List<SOPromotionDetailInfo>>(dtDetail.DefaultView.ToTable().Rows);
                    });
                    if (promotionList.Count > 0)
                    {
                        soInfo.SOPromotions.AddRange(promotionList);
                    }
                }
            }
        }

        /// <summary>
        /// 订单参与的赠品规则的Mapping
        /// </summary>
        /// <param name="soInfo"></param>
        private void MappingSelfGiftToSOInfo(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOSelfGiftBySOSysNo");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);
            using (DataSet ds = command.ExecuteDataSet())
            {
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dtDetail = ds.Tables[1];
                    List<SOPromotionInfo> promotionList = DataMapper.GetEntityList<SOPromotionInfo, List<SOPromotionInfo>>(ds.Tables[0].Rows,
                        (dr, p) =>
                        {
                            p.PromotionType = SOPromotionType.SelfGift;
                            dtDetail.DefaultView.RowFilter = String.Format("PromotionSysNo={0}", p.PromotionSysNo);
                            p.GiftList = DataMapper.GetEntityList<SOPromotionInfo.GiftInfo, List<SOPromotionInfo.GiftInfo>>(dtDetail.DefaultView.ToTable().Rows);

                            p.InnerType = dr["InnerType"].ToString();
                            p.SOSysNo = soInfo.SysNo;
                        });

                    if (promotionList.Count > 0)
                    {
                        soInfo.SOPromotions.AddRange(promotionList);
                    }
                }
            }
        }

        private void MappingAccessoryAndVendorGiftToSOInfoV2(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOAccessoryAndVendorGiftBySOSysNoV2");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);

            List<SOItemAccessory> accessorList = command.ExecuteEntityList<SOItemAccessory>();

            List<SOPromotionInfo> promotionList = new List<SOPromotionInfo>();

            //转换成赠品Promotion
            if (accessorList != null)
            {
                List<int> promotionSysNos = accessorList.Select(x => x.PromotionSysNo.Value).Distinct().ToList();



                foreach (var promotionSysNo in promotionSysNos)
                {
                    //每个活动的所有赠品信息
                    List<SOItemAccessory> accessorP = accessorList.FindAll(x => x.PromotionSysNo == promotionSysNo);

                    SOPromotionInfo promotion = new SOPromotionInfo();

                    switch (accessorP[0].Type)
                    {
                        case "V":
                            promotion.PromotionType = SOPromotionType.VendorGift;
                            break;
                        case "A":
                            promotion.PromotionType = SOPromotionType.Accessory;
                            break;
                    }

                    promotion.PromotionSysNo = promotionSysNo;

                    //主商品
                    promotion.MasterList.Add(new SOPromotionInfo.MasterInfo { ProductSysNo = accessorP[0].MasterProductSysNo.Value, QtyPreTime = 1 });

                    foreach (var accessor in accessorP)
                    {
                        promotion.GiftList.Add(new SOPromotionInfo.GiftInfo { ProductSysNo = accessor.ProductSysNo.Value, Quantity = accessor.Quantity.Value });

                    }
                    promotionList.Add(promotion);
                }
            }

            soInfo.SOPromotions.AddRange(promotionList);

        }

        /// <summary>
        /// 订单商品的附件和厂商赠品的Mapping
        /// </summary>
        /// <param name="soInfo"></param>
        private void MappingAccessoryAndVendorGiftToSOInfo(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOAccessoryAndVendorGiftBySOSysNo");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);


            using (DataTable dt = command.ExecuteDataTable())
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<SOPromotionInfo> promotionList = new List<SOPromotionInfo>();
                    List<SOPromotionDetailInfo> promotions = DataMapper.GetEntityList<SOPromotionDetailInfo, List<SOPromotionDetailInfo>>(dt.Rows,
                        (dr, d) =>
                        {
                            SOPromotionInfo newPromotionInfo = new SOPromotionInfo();
                            newPromotionInfo.PromotionSysNo = dr["PromotionSysNo"] == null || dr["PromotionSysNo"] == DBNull.Value ? 0 : (int)dr["PromotionSysNo"];
                            if (dr["PromotionType"] != null && dr["PromotionType"] != DBNull.Value)
                            {
                                newPromotionInfo.PromotionType = (SOPromotionType)Enum.Parse(typeof(SOPromotionType), dr["PromotionType"].ToString());
                            }

                            SOPromotionInfo.GiftInfo newGift = DataMapper.GetEntity<SOPromotionInfo.GiftInfo>(dr);
                            //d.GiftList.Add(newGift);
                            SOPromotionInfo promotionInfo = promotionList.Find(p =>
                             {
                                 return p.PromotionType == newPromotionInfo.PromotionType && p.PromotionSysNo == newPromotionInfo.PromotionSysNo;
                             });
                            if (promotionInfo == null)
                            {
                                promotionInfo = newPromotionInfo;
                                promotionInfo.SOPromotionDetails.Add(d);
                                promotionInfo.GiftList.Add(newGift);
                                promotionList.Add(promotionInfo);
                            }
                            else
                            {
                                SOPromotionInfo.GiftInfo gift = promotionInfo.GiftList.Find(g => { return g.ProductSysNo == newGift.ProductSysNo; });
                                if (gift == null)
                                {
                                    promotionInfo.GiftList.Add(newGift);
                                }
                                else
                                {
                                    gift.Quantity = gift.Quantity + newGift.Quantity;
                                }
                                SOPromotionDetailInfo detail = promotionInfo.SOPromotionDetails.Find(td =>
                                  {
                                      return td.MasterProductSysNo == d.MasterProductSysNo;//&& td.MasterProductType == d.MasterProductType;
                                  });
                                if (detail == null)
                                {
                                    promotionInfo.SOPromotionDetails.Add(d);
                                }
                                //else
                                //{
                                //    gift = detail.GiftList.Find(g => { return g.ProductSysNo == newGift.ProductSysNo; });
                                //    if (gift == null)
                                //    {
                                //        detail.GiftList.Add(newGift);
                                //    }
                                //    else
                                //    {
                                //        gift.Quantity = gift.Quantity + newGift.Quantity;
                                //    }
                                //}
                            }
                        });
                    if (promotionList.Count > 0)
                    {
                        soInfo.SOPromotions.AddRange(promotionList);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SOItemInfo> DataTableToSOItems(DataTable dtItems)
        {
            if (dtItems == null)
            {
                return new List<SOItemInfo>();
            }
            List<SOItemInfo> itemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(dtItems.Rows);

            itemList.ForEach(item =>
            {
                if (item.ProductType == SOProductType.ExtendWarranty)
                {
                    int mSysNo = 0;
                    if (int.TryParse(item.MasterProductSysNo, out mSysNo))
                    {
                        SOItemInfo masterProduct = itemList.Find(mp => mSysNo == mp.ProductSysNo);
                        item.StockSysNo = masterProduct == null ? null : masterProduct.StockSysNo;
                    }
                }
                item.Price_End = item.Price.Value;
            });

            return itemList;
        }

        /// <summary>
        /// 填充订单实体
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private SOInfo DataSetToSOInfo(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            SOInfo soInfo = DateSetToSOMasterInfo(ds);

            //  2.  取得订单的所有商品信息
            if (ds.Tables.Count > 1)
            {
                soInfo.Items = DataTableToSOItems(ds.Tables[1]);
                //填充ItemsEx信息
                MappingExtensionInfo(soInfo);
                MappingCouponToSOInfo(soInfo);
            }
            //  3.  Mapping订单参与的促销活动
            MappingComboToSOInfo(soInfo);
            //MappingAccessoryAndVendorGiftToSOInfo(soInfo);
            MappingAccessoryAndVendorGiftToSOInfoV2(soInfo);
            MappingSelfGiftToSOInfo(soInfo);
            SOVATInvoiceInfo vatInfo = GetSOVATInvoiceInfoBySoSysNo(soInfo.SysNo.Value);
            if (vatInfo != null)
            {
                soInfo.InvoiceInfo.VATInvoiceInfo = vatInfo;
            }

            List<SOInterceptInfo> soInterceptInfoList = ObjectFactory<ISOInterceptDA>.Instance.GetSOInterceptInfoListBySOSysNo(soInfo.BaseInfo.SysNo.Value);
            if (soInterceptInfoList != null && soInterceptInfoList.Count > 0)
            {
                soInfo.SOInterceptInfoList = soInterceptInfoList;
            }



            soInfo.ItemGrossProfitList = GetSOItemGrossProfit(soInfo.SysNo.Value);
            soInfo.SysNo = soInfo.SysNo;
            soInfo.CompanyCode = soInfo.CompanyCode;
            soInfo.Merchant = soInfo.Merchant;
            soInfo.WebChannel = soInfo.WebChannel;
            soInfo.CustomerAuthentication = GetSOCustomerAuthentication(soInfo.SysNo.Value);
            return soInfo;
        }

        public List<SOItemAccessory> GetSOItemAccessoryList(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOItemAccessories");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<SOItemAccessory>();
        }

        private SOInfo DateSetToSOMasterInfo(DataSet ds)
        {
            //  1.  取得订单的主信息
            DataRow dr = ds.Tables[0].Rows[0];
            SOInfo soInfo = DataMapper.GetEntity<SOInfo>(dr);
            int ippStatus = dr["Status"] == null || dr["Status"] == DBNull.Value ? -1 : Convert.ToInt32(dr["Status"]);
            bool autoRMA = dr["HaveAutoRMA"] == null || dr["HaveAutoRMA"] == DBNull.Value ? false : Convert.ToBoolean(dr["HaveAutoRMA"]);
            decimal cashPay = dr["CashPay"] == null || dr["CashPay"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CashPay"]);
            int ippHoldStatus = dr["HoldStatus"] == null || dr["HoldStatus"] == DBNull.Value ? 0 : Convert.ToInt32(dr["HoldStatus"]);
            bool ippHoldMark = dr["HoldMark"] == null || dr["HoldMark"] == DBNull.Value ? false : Convert.ToBoolean(dr["HoldMark"]);
            SOBaseInfoSpecificMapping(soInfo.BaseInfo, autoRMA, ippStatus, cashPay, ippHoldStatus, ippHoldMark, soInfo.ShippingInfo.IsCombine.Value, soInfo.ShippingInfo.IsMergeComplete.Value);

            soInfo.StatusChangeInfoList = soInfo.StatusChangeInfoList ?? new List<SOStatusChangeInfo>();
            if (dr["AuditUserSysNo"] != null && dr["AuditUserSysNo"] != DBNull.Value
                && dr["AuditTime"] != null && dr["AuditTime"] != DBNull.Value
                && soInfo.BaseInfo.Status.Value != SOStatus.Origin)
            {
                SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
                {
                    OldStatus = SOStatus.Origin,
                    ChangeTime = Convert.ToDateTime(dr["AuditTime"]),
                    SOSysNo = soInfo.SysNo,
                    OperatorSysNo = Convert.ToInt32(dr["AuditUserSysNo"]),
                };
                if (soInfo.BaseInfo.Status.Value == SOStatus.WaitingManagerAudit) statusChangeInfo.Status = SOStatus.WaitingManagerAudit;
                statusChangeInfo.OperatorType = dr["AuditType"] != null && dr["AuditType"].ToString() == "0" ? SOOperatorType.System : SOOperatorType.User;
                soInfo.StatusChangeInfoList.Add(statusChangeInfo);
            }

            if (dr["ManagerAuditUserSysNo"] != null && dr["ManagerAuditUserSysNo"] != DBNull.Value
               && dr["ManagerAuditTime"] != null && dr["ManagerAuditTime"] != DBNull.Value
               && soInfo.BaseInfo.Status.Value != SOStatus.Origin && soInfo.BaseInfo.Status.Value != SOStatus.WaitingManagerAudit)
            {
                SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
                {
                    OldStatus = SOStatus.WaitingManagerAudit,
                    Status = SOStatus.WaitingOutStock,
                    ChangeTime = Convert.ToDateTime(dr["AuditTime"]),
                    SOSysNo = soInfo.SysNo,
                    OperatorSysNo = Convert.ToInt32(dr["AuditUserSysNo"]),
                    OperatorType = SOOperatorType.User
                };
                soInfo.StatusChangeInfoList.Add(statusChangeInfo);
            }
            return soInfo;
        }

        /// <summary>
        /// 将DataSet转换成订单列表
        /// </summary>
        /// <param name="ds">初始数据</param>
        /// <returns>映射好的订单列表</returns>
        private List<SOInfo> DataSetToSOList(DataSet ds)
        {
            return DataSetToSOList(ds, false);
        }

        /// <summary>
        /// 将DataSet转换成订单列表
        /// </summary>
        /// <param name="ds">初始数据</param>
        /// <param name="isOnlyReadMaster">是否只读取主表数据</param>
        /// <returns>映射好的订单列表</returns>
        private List<SOInfo> DataSetToSOList(DataSet ds, bool isOnlyReadMaster)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return new List<SOInfo>();
            }
            List<SOInfo> soList = new List<SOInfo>();
            DataTable dtSOMasterInfo = ds.Tables[0];
            foreach (DataRow dr in dtSOMasterInfo.Rows)
            {
                DataSet subDS = new DataSet();
                //把一个子订单的主要信息转换成一个新的DataTable,添加到子DataSet 中
                dtSOMasterInfo.DefaultView.RowFilter = String.Format("SysNo={0}", dr["SysNo"]);
                subDS.Tables.Add(dtSOMasterInfo.DefaultView.ToTable());

                if (!isOnlyReadMaster)
                {
                    if (ds.Tables.Count > 1)
                    {
                        //把一个子订单的商品信息转换成一个新的DataTable,添加到子DataSet 中
                        ds.Tables[1].DefaultView.RowFilter = String.Format("SOSysNo={0}", dr["SysNo"]);
                        subDS.Tables.Add(ds.Tables[1].DefaultView.ToTable());
                    }
                    soList.Add(DataSetToSOInfo(subDS));
                }
                else
                {
                    soList.Add(DateSetToSOMasterInfo(subDS));
                }
            }
            return soList;
        }

        /// <summary>
        /// 根据订单系统编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public SOInfo GetSOBySOSysNo(int soSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOInfo");
            string so_ConditionString = string.Format(" WHERE m.sysno = {0} ", soSysNo);
            string item_ConditionString = string.Format(" WHERE i.SOSysNo ={0} ", soSysNo);
            command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString)
                                                        .Replace("#SOItem_ConditionString#", item_ConditionString)
                                                        .Replace("#Top#", "");
            using (DataSet ds = command.ExecuteDataSet())
            {
                if (ds != null)
                {
                    return DataSetToSOInfo(ds);
                }
            }
            return null;
        }

        public List<string> TrackingNumberBySoSysno(int sosysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetTrackingNumberStrBySoSysno");
            command.SetParameterValue("@SOSysNo", sosysno);
            DataTable dt = command.ExecuteDataTable();
            List<string> numberList = new List<string>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    numberList.Add(row[0].ToString());
                }
            }

            return numberList;
        }

        //public string GetItemsSHDSysNo(int productSysno, int soSysno)
        //{
        //    DataCommand command = DataCommandManager.GetDataCommand("GetItemsSHDSysNo");
        //    command.SetParameterValue("@SOSysNo", soSysno);
        //    command.SetParameterValue("@ProductSysNo", productSysno);
        //    return Convert.ToString(command.ExecuteScalar());
        //}

        /// <summary>
        /// 根据子订单系统编号取得主订单信息
        /// </summary>
        /// <param name="subSOSysNo">子订单系统编号</param>
        /// <returns></returns>
        public SOInfo GetMasterSOBySubSOSysNo(int subSOSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOInfo");
            string masterSysNo = string.Format("SELECT TOP 1 SoSplitMaster FROM IPP3.dbo.SO_CheckShipping WHERE SOSysNo ={0}", subSOSysNo);
            string so_ConditionString = string.Format(" WHERE m.sysno = ({0}) ", masterSysNo);
            string item_ConditionString = string.Format(" WHERE i.SOSysNo =({0}) ", masterSysNo);
            command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString)
                                                    .Replace("#SOItem_ConditionString#", item_ConditionString)
                                                    .Replace("#Top#", "");

            using (DataSet ds = command.ExecuteDataSet())
            {
                if (ds != null)
                {
                    return DataSetToSOInfo(ds);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据主订单编号取得其所有子订单
        /// </summary>
        /// <param name="masterSOSysNo"></param>
        /// <returns></returns>
        public List<SOInfo> GetSubSOByMasterSOSysNo(int masterSOSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOInfo");
            string so_ConditionString = string.Format(@"WHERE c.SoSplitMaster = {0} AND m.[Status]>=0", masterSOSysNo);
            string item_ConditionString = so_ConditionString;
            command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString)
                                                    .Replace("#SOItem_ConditionString#", item_ConditionString)
                                                    .Replace("#Top#", "");

            //取出所有子订单的信息
            DataSet ds = command.ExecuteDataSet();
            return DataSetToSOList(ds);
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public List<SOInfo> GetSOBySOSysNoList(List<int> soSysNos)
        {
            List<SOInfo> soList = new List<SOInfo>();

            string soSysNos_Str = SOSysNoListToString(soSysNos);

            if (!String.IsNullOrEmpty(soSysNos_Str))
            {
                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOInfo");
                string so_ConditionString = String.Format(" WHERE m.SysNo IN ({0})", soSysNos_Str);
                string item_ConditionString = String.Format(" WHERE i.SOSysNo IN ({0}) ", soSysNos_Str);
                command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString)
                                                        .Replace("#SOItem_ConditionString#", item_ConditionString)
                                                        .Replace("#Top#", "");
                using (DataSet ds = command.ExecuteDataSet())
                {
                    soList = DataSetToSOList(ds);
                }
            }

            return soList;
        }

        /// <summary>
        /// 根据订单编号 获取订单基础信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public SOBaseInfo GetSOBaseInfoBySOSysNo(int soSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOBaseInfo");
            string so_ConditionString = string.Format(" WHERE m.sysno ={0} ", soSysNo);
            command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString).Replace("#Top#", "");

            DataRow dr = command.ExecuteDataRow();
            if (dr != null)
            {
                return DataRowToSOBaseInfo(dr);
            }
            return null;
        }

        /// <summary>
        /// 根据订单编号列表 获取订单基础信息 列表
        /// </summary>
        /// <param name="soSysNos"></param>
        /// <returns></returns>
        public List<SOBaseInfo> GetSOBaseInfoBySOSysNoList(List<int> soSysNos)
        {
            List<SOBaseInfo> soList = new List<SOBaseInfo>();

            string soSysNos_Str = SOSysNoListToString(soSysNos);

            if (!String.IsNullOrEmpty(soSysNos_Str))
            {
                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SOBaseInfo");
                string so_ConditionString = string.Format(" WHERE m.sysno IN ({0}) ", soSysNos_Str);
                command.CommandText = command.CommandText.Replace("#SO_ConditionString#", so_ConditionString).Replace("#Top#", "");


                using (DataTable dt = command.ExecuteDataTable())
                {
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            SOBaseInfo info = DataRowToSOBaseInfo(dr);
                            if (info != null)
                            {
                                soList.Add(info);
                            }
                        }
                    }
                }
            }
            return soList;
        }

        /// <summary>
        /// 根据订单编号或去订单商品信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public List<SOItemInfo> GetSOItemsBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOItemsBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            List<SOItemInfo> itemList = new List<SOItemInfo>();
            using (DataSet ds = command.ExecuteDataSet())
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    //请在SQL中添加取得订单商品促销折扣的语句，由新的设计，所有需要加表，在加好表后就添加上。
                    itemList = DataTableToSOItems(ds.Tables[0]);
                }
            }
            return itemList;
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单，但取出的不是订单的所有信息
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public List<SOInfo> GetSimpleSOInfoList(List<int> soSysNos)
        {
            List<SOInfo> soList = new List<SOInfo>();

            string soSysNos_Str = SOSysNoListToString(soSysNos);

            if (!String.IsNullOrEmpty(soSysNos_Str))
            {
                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Get_SimpleSOListBySOSysNos");
                command.CommandText = command.CommandText.Replace("#SOSysNo#", soSysNos_Str);
                using (DataSet ds = command.ExecuteDataSet())
                {
                    soList = DataSetToSOList(ds);
                }
            }

            return soList;
        }

        /// <summary>
        /// 根据客户编号获取订单对应的增值税发票
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public List<SOVATInvoiceInfo> GetSOVATInvoiceInfoByCustomerSysNo(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetDistinctSOVATInvoiceInfo");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            return command.ExecuteEntityList<SOVATInvoiceInfo>();
        }

        /// <summary>
        /// 根据订单编号 获取订单对应的增值税发票信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public SOVATInvoiceInfo GetSOVATInvoiceInfoBySoSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOVATInvoiceInfoBySoSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<SOVATInvoiceInfo>();
        }

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public List<GiftCardInfo> QueryGiftCardListInfoByCustomerSysNo(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGiftCardsInfoByCustomerSysNo");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            return command.ExecuteEntityList<GiftCardInfo>();
        }

        /// <summary>
        /// 获取商品毛利额
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public List<ItemGrossProfitInfo> GetSOItemGrossProfit(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_ItemGrossProfitsBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<ItemGrossProfitInfo>();
        }

        /// <summary>
        /// 获取配置文件数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "key", "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public string GetSysConfigurationValue(string key, string companyCode)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryConfigurationByGetConfigKey");
            dataCommand.ReplaceParameterValue("@Key", key);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToString(dataCommand.ExecuteScalar());
        }

        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info"></param>
        public bool WHUpdateStock(SOWHUpdateInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SO_Update_ItemWareHouseNumber");
            dc.SetParameterValue("@SOSysNo", info.SOSysNo);
            dc.SetParameterValue("@TargetStockSysNo", info.StockSysNo);
            dc.SetParameterValue("@ProductSysNo", info.ProductSysNo);
            dc.SetParameterValue("@CompanyCode", info.CompanyCode);
            return dc.ExecuteNonQuery() > 0 ? true : false;
        }

        public string GetCertificaterNameBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_CertificaterNameBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<string>();
        }

        #endregion

        #region 修改订单状态
        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateSOStatus(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatus");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateSOStatusForAudit(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusForAudit");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateSOStatusForManagerAudit(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusForManagerAudit");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateSOStatusToSplit(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusToSplit");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改订单状态为物流拒收
        /// </summary>
        /// <param name="info"></param>
        public bool UpdateSOStatusToReject(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusToReject");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 修改订单状态为待审核，取消审核时调用 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToOrigin(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_CancelAudit");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 订单出库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToOutStock(SOStatusChangeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_StockOut");
            command.SetParameterValue<SOStatusChangeInfo>(info);
            return command.ExecuteNonQuery() > 0;
        }
        #endregion

        #region 创建订单相关方法

        /// <summary>
        /// 获取新订单号
        /// </summary>
        /// <returns></returns>
        public int NewSOSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_NewSOSysNo");
            int result = Convert.ToInt32(command.ExecuteScalar());
            return result;
        }

        /// <summary>
        /// 添加订单信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <returns></returns>
        public SOInfo InsertSOMainInfo(SOInfo soInfo)
        {
            InsertSOMasterInfo(soInfo);
            return soInfo;
        }

        /// <summary>
        /// 添加订单主信息
        /// </summary>
        /// <param name="soInfo"></param>
        public void InsertSOMasterInfo(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOMaster");
            if (soInfo!=null && soInfo.BaseInfo != null && soInfo.BaseInfo.TariffAmount.HasValue)
            {
                soInfo.BaseInfo.TariffAmount = Math.Round(soInfo.BaseInfo.TariffAmount.Value, 2, MidpointRounding.AwayFromZero);
            }
            command.SetParameterValue<SOInfo>(soInfo, true, false);
            command.SetParameterValue("@BaseInfo_CreateTime", DateTime.Now);
            command.SetParameterValue("@BaseInfo_SalesManSysNo", ServiceContext.Current.UserSysNo);
            if (soInfo.BaseInfo.HoldStatus.HasValue)
            {
                command.SetParameterValue("@HoldMark", soInfo.BaseInfo.HoldStatus.Value == SOHoldStatus.BackHold);
            }
            else
            {
                command.SetParameterValue("@HoldMark", DBNull.Value);
            }
            command.SetParameterValue("@StockSysNo", soInfo.ShippingInfo.MergeToStockSysNo);
            if (soInfo.StatusChangeInfoList != null && soInfo.StatusChangeInfoList.Count > 0)
            {
                soInfo.StatusChangeInfoList = soInfo.StatusChangeInfoList.OrderByDescending(info => info.ChangeTime).ToList();
                SOStatusChangeInfo statusChangeInfo = soInfo.StatusChangeInfoList.Find(info => { return info.OldStatus == SOStatus.Origin; });
                if (statusChangeInfo != null)
                {
                    command.SetParameterValue("@AuditUserSysNo", statusChangeInfo.OperatorSysNo);
                    command.SetParameterValue("@AuditTime", statusChangeInfo.ChangeTime); ;
                }
                statusChangeInfo = soInfo.StatusChangeInfoList.Find(info => { return info.OldStatus == SOStatus.WaitingManagerAudit; });
                if (statusChangeInfo != null)
                {
                    command.SetParameterValue("@ManagerAuditUserSysNo", statusChangeInfo.OperatorSysNo);
                    command.SetParameterValue("@ManagerAuditTime", statusChangeInfo.ChangeTime);
                }
                else
                {
                    command.SetParameterValue("@ManagerAuditUserSysNo", 0);
                }
            }
            else
            {
                command.SetParameterValue("@ManagerAuditUserSysNo", 0);
            }
            command.SetParameterValue("@IsPrintPackageCover", soInfo.ShippingInfo.IsPrintPackageCover);
            command.SetParameterValue("@PositiveSOSysNo", DBNull.Value);
            SOItemInfo soItem = soInfo.Items.Find(item => { return item.ProductType == SOProductType.Coupon; });
            if (soItem != null)
            {
                command.SetParameterValue("@PromotionCodeSysNo", soItem.ProductSysNo);
                command.SetParameterValue("@PromotionCustomerSysNo", soInfo.BaseInfo.CustomerSysNo);
            }
            command.SetParameterValue("@HaveAutoRMA", soInfo.BaseInfo.Status == SOStatus.Reject);
            command.SetParameterValue("@StoreCompanyCode", soInfo.BaseInfo.CompanyCode);
            command.SetParameterValue("@ReceivingStatus", DBNull.Value);
            command.SetParameterValue("@ShippingInfo_AllocatedManSysNo", soInfo.ShippingInfo.AllocatedManSysNo ?? 0);
            command.SetParameterValue("@BaseInfo_Memo", string.IsNullOrEmpty(soInfo.BaseInfo.Memo) ? "" : soInfo.BaseInfo.Memo);
            command.SetParameterValue("@InvoiceInfo_FinanceNote", string.IsNullOrEmpty(soInfo.InvoiceInfo.FinanceNote) ? "" : soInfo.InvoiceInfo.FinanceNote);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加订单CheckShipping信息
        /// </summary>
        /// <param name="soInfo"></param>
        public void InsertSOCheckShippingInfo(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOCheckShipping");
            command.SetParameterValue<SOInfo>(soInfo, true, false);
            command.SetParameterValue("@BaseInfo_CreateTime", DateTime.Now);
            if (soInfo.StatusChangeInfoList != null && soInfo.StatusChangeInfoList.Count > 0)
            {
                soInfo.StatusChangeInfoList = soInfo.StatusChangeInfoList.OrderByDescending(info => info.ChangeTime).ToList();
                SOStatusChangeInfo statusChangeInfo = soInfo.StatusChangeInfoList.Find(info => { return info.OldStatus == SOStatus.Origin; });
                if (statusChangeInfo != null)
                {
                    command.SetParameterValue("@AuditType", statusChangeInfo.OperatorType == SOOperatorType.System);
                    command.SetParameterValue("@AuditSOSendMailFlag", statusChangeInfo.IsSendMailToCustomer);
                    command.SetParameterValue("@AutoAuditMemo", statusChangeInfo.Note);
                }
            }
            command.SetParameterValue("@CurrencySysNo", 1);
            command.SetParameterValue("@StoreCompanyCode", soInfo.BaseInfo.CompanyCode);
            command.SetParameterValue("@IsDuplicateOrder", DBNull.Value);
            command.SetParameterValue("@IsDirectAlipay", DBNull.Value);
            command.SetParameterValue("@TenpayCoupon", DBNull.Value);
            command.SetParameterValue("@MKTActivityType", DBNull.Value);
            //command.SetParameterValue("@LocalWHSysNo", soInfo.ShippingInfo.MergeToStockSysNo);
            command.SetParameterValue("@LocalWHSysNo", soInfo.ShippingInfo.LocalWHSysNo);
            command.SetParameterValue("@BaseInfo_DCStatus", soInfo.BaseInfo.DCStatus ?? 0);
            command.SetParameterValue("@ShippingInfo_StockStatus", soInfo.ShippingInfo.StockStatus ?? 0);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加订单主商品
        /// </summary>
        /// <param name="?"></param>
        public void InsertSOItemInfo(SOInfo soInfo)
        {
            int ippItemDiscountType;
            string stockSysNo;

            foreach (SOItemInfo soItemInfo in soInfo.Items)
            {
                ippItemDiscountType = soInfo.BaseInfo.SOType == SOType.ElectronicCard ? 1 : (soInfo.BaseInfo.SOType == SOType.PhysicalCard ? 2 : 0);

                //对于正常商品直接取仓库号
                if (soItemInfo.StockSysNo.HasValue && soItemInfo.StockSysNo.Value!=0)
                {
                    stockSysNo = soItemInfo.StockSysNo.Value.ToString();
                }
                else if (soItemInfo.InventoryType == ProductInventoryType.GetShopInventory || soItemInfo.InventoryType == ProductInventoryType.Company
                    || soItemInfo.InventoryType == ProductInventoryType.TwoDoor) //商品库存模式是1，3，5的如果没有取到仓库编号，则赋默认值51
                {
                    stockSysNo = "51";
                }
                else //无库存的优惠券订单则要求仓库编号为00
                {
                    stockSysNo = "00";
                }

                DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOItem");
                command.SetParameterValue<SOItemInfo>(soItemInfo, true, false);

                command.SetParameterValue("@StockSysNo", stockSysNo);                
                command.SetParameterValue("@CompanyCode", soInfo.CompanyCode);
                command.SetParameterValue("@DiscountType", ippItemDiscountType);
                command.SetParameterValue("@CreateDate", DateTime.Now);
                command.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
                soItemInfo.SysNo = command.ExecuteScalar<int>();

                //Extension插入
                if (soItemInfo.ItemExtList != null && soItemInfo.ItemExtList.Count > 0)
                {
                    foreach (var ext in soItemInfo.ItemExtList)
                    {
                        InsertSOItemExtension(ext);
                    }
                }
            }
        }

        /// <summary>
        /// 添加订单参与的促销规则（赠品、附件、）
        /// </summary>
        /// <param name="promotionInfo"></param>
        public void InsertSOPromotionInfo(SOPromotionInfo promotionInfo, string companyCode)
        {
            if (promotionInfo != null)
            {
                switch (promotionInfo.PromotionType.Value)
                {
                    case SOPromotionType.SelfGift:
                        InsertSOGift(promotionInfo);
                        break;
                    case SOPromotionType.Accessory:
                    case SOPromotionType.VendorGift:
                        InsertSOAccessory(promotionInfo);
                        break;
                    case SOPromotionType.Combo:
                        InsertSOCombo(promotionInfo, companyCode);
                        break;
                    case SOPromotionType.SaleDiscountRule:
                        InsertNewPromoLog(promotionInfo);
                        break;
                }
            }
        }

        /// <summary>
        /// 删除新促销应用日志
        /// </summary>
        /// <param name="soSysNo"></param>
        private void DeleteNewPromoLog(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_NewPromo_DeleteMasterAndDetail");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 写入新促销应用日志
        /// </summary>
        /// <param name="promotionInfo"></param>
        private void InsertNewPromoLog(SOPromotionInfo promotionInfo)
        {
            DataCommand masterCommand = DataCommandManager.GetDataCommand("SO_NewPromo_InsertLogMaster");
            masterCommand.SetParameterValue("@SOSysNo", promotionInfo.SOSysNo);
            masterCommand.SetParameterValue("@PromoSysNo", promotionInfo.PromotionSysNo);
            masterCommand.SetParameterValue("@PromoType", (int)promotionInfo.PromotionType);
            masterCommand.SetParameterValue("@VendorSysNo", promotionInfo.VendorSysNo);
            masterCommand.SetParameterValue("@PromoRuleData", promotionInfo.PromoRuleData);
            masterCommand.SetParameterValue("@InUser", ServiceContext.Current.UserDisplayName);
            masterCommand.ExecuteNonQuery();
            int masterSysNo = int.Parse(masterCommand.GetParameterValue("@SysNo").ToString());
            if (promotionInfo.SOPromotionDetails != null)
            {
                foreach (var detail in promotionInfo.SOPromotionDetails)
                {
                    DataCommand giftitemCommand = DataCommandManager.GetDataCommand("SO_NewPromo_InsertLogDetail");
                    giftitemCommand.SetParameterValue("@SOMarketingPromoSysNo", masterSysNo);
                    giftitemCommand.SetParameterValue("@ProductSysNo", detail.MasterProductSysNo);
                    giftitemCommand.SetParameterValue("@ProductQuantity", detail.MasterProductQuantity);
                    giftitemCommand.SetParameterValue("@DiscountAmount", detail.DiscountAmount);
                    giftitemCommand.ExecuteNonQuery();
                }
            }
        }


        private void InsertSOCombo(SOPromotionInfo promotionInfo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOSaleRule");
            command.SetParameterValue("@SOSysNo", promotionInfo.SOSysNo);
            command.SetParameterValue("@SaleRuleSysNo", promotionInfo.PromotionSysNo);
            command.SetParameterValue("@SaleRuleName", promotionInfo.PromotionName);
            command.SetParameterValue("@Discount", promotionInfo.Discount);
            command.SetParameterValue("@Times", promotionInfo.Time);
            command.SetParameterValue("@Note", promotionInfo.Note);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除的订单参与的销售规则
        /// </summary>
        /// <param name="SOSysNo">订单编号</param>
        /// <param name="companyCode">公司编号</param> 
        public void DeleteSOPromotionInfo(int SOSysNo)
        {
            DeleteSOGiftMaster(SOSysNo);
            DeleteSOGiftItem(SOSysNo);
            DeleteSOAccessory(SOSysNo);
            DeleteSOCombo(SOSysNo);
            DeleteNewPromoLog(SOSysNo);
        }

        /// <summary>
        /// 添加订单参与的促销规则
        /// </summary>
        /// <param name="promotionInfo"></param>
        private void InsertSOGift(SOPromotionInfo promotionInfo)
        {
            DataCommand giftCommand = DataCommandManager.GetDataCommand("SO_Insert_GiftMaster");
            giftCommand.SetParameterValue<SOPromotionInfo>(promotionInfo, true, false);
            giftCommand.ExecuteNonQuery();

            if (promotionInfo.GiftList != null)
            {
                foreach (SOPromotionInfo.GiftInfo gift in promotionInfo.GiftList)
                {
                    DataCommand giftitemCommand = DataCommandManager.GetDataCommand("SO_Insert_SOGiftItem");
                    giftitemCommand.SetParameterValue(gift, true, false);
                    giftitemCommand.SetParameterValue("@SOSysNo", promotionInfo.SOSysNo);
                    giftitemCommand.SetParameterValue("@PromotionSysNo", promotionInfo.PromotionSysNo);
                    giftitemCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 添加订单参与的促销规则
        /// </summary>
        /// <param name="promotionInfo"></param>
        private void InsertSOAccessory(SOPromotionInfo promotionInfo)
        {
            if (promotionInfo.GiftList != null)
            {
                foreach (var gift in promotionInfo.GiftList)
                {
                    DataCommand giftitemCommand = DataCommandManager.GetDataCommand("SO_Insert_SOItemAccessory");
                    giftitemCommand.SetParameterValue("@SOSysNo", promotionInfo.SOSysNo);
                    giftitemCommand.SetParameterValue("@PromotionSysNo", promotionInfo.PromotionSysNo);
                    giftitemCommand.SetParameterValue("@Type", promotionInfo.PromotionType == SOPromotionType.Accessory ? "A" : "V");
                    giftitemCommand.SetParameterValue("@MasterProductSysNo", promotionInfo.MasterList[0].ProductSysNo);
                    giftitemCommand.SetParameterValue("@ProductSysNo", gift.ProductSysNo);
                    giftitemCommand.SetParameterValue("@Quantity", gift.Quantity);
                    giftitemCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 更新订单增值税发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        public void UpdateSOVATInvoice(SOVATInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOVATInvoice");
            command.SetParameterValue<SOVATInvoiceInfo>(entity);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加订单商品扩展信息
        /// </summary>
        /// <param name="soItemInfo"></param>
        /// <param name="companyCode"></param>
        /// <param name="InUser"></param>
        public void InsertItemExtend(SOItemInfo soItemInfo, string companyCode, string InUser)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSOItemExtend");
            command.SetParameterValue("@SOSysNo", soItemInfo.SOSysNo);
            command.SetParameterValue("@ProductSysNo", soItemInfo.ProductSysNo);
            command.SetParameterValue("@Price", soItemInfo.Price);
            command.SetParameterValue("@Discount", soItemInfo.AdjustPrice);
            command.SetParameterValue("@Reason", soItemInfo.AdjustPriceReason);
            command.SetParameterValue("@EditUser", InUser);
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@InUser", InUser);
            command.SetParameterValue("@InDate", DateTime.Now);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 分仓逻辑中： 往 Shopping.dbo.ShoppingMaster 里添加数据
        /// </summary>
        /// <param name="info"></param>
        public int InsertShoppingMaster(SOInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_InsertSOShoppingMaster");
            command.SetParameterValue("@SOSysNo", info.BaseInfo.SysNo);
            command.SetParameterValue("@CustomerSysNo", info.BaseInfo.CustomerSysNo);
            command.SetParameterValue("@ShippingAreaSysNo", info.ReceiverInfo.AreaSysNo);
            command.SetParameterValue("@ShipTypeSysNo", info.ShippingInfo.ShipTypeSysNo);
            command.SetParameterValue("@ActionType", (info.SysNo.HasValue && info.SysNo.Value != 0) ? "IPPUpdate" : "IPPCreate");
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            command.ExecuteNonQuery();
            return (int)command.GetParameterValue("@SysNo");
        }

        /// <summary>
        /// 分仓逻辑中： 往ShoppingTransaction里插入数据
        /// </summary>
        /// <param name="info"></param>
        public void InsertShoppingTransaction(SOItemInfo itemInfo, int shoppingMasterSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_InsertSOShoppingTransaction");
            command.SetParameterValue("@ShoppingMasterSysNo", shoppingMasterSysNo);
            command.SetParameterValue("@ProductType", itemInfo.ProductType.HasValue ? itemInfo.ProductType.Value : SOProductType.Product);
            command.SetParameterValue("@ProductSysNo", itemInfo.ProductSysNo);
            command.SetParameterValue("@ItemNumber", itemInfo.ProductID);
            command.SetParameterValue("@ShoppingQty", itemInfo.Quantity);
            command.SetParameterValue("@AvailableQty", itemInfo.AvailableQty);
            command.SetParameterValue("@Price", itemInfo.Price);
            command.SetParameterValue("@CostPrice", itemInfo.CostPrice);
            command.SetParameterValue("@MasterProductSysNo", itemInfo.MasterProductSysNo);
            command.SetParameterValue("@StockSysNo", itemInfo.StockSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Allocate Inventory
        /// </summary>
        /// <param name="shoppingMasterSysNo">ShoppingMaster SysNo</param>
        /// <param name="areaSysNo">Recevice Area Sysno</param>
        /// <param name="shipTypeSysNo">ShipType SysNo</param>
        /// <param name="status">success or failed</param>
        /// <returns>Updated SO ShoppingTransaction List</returns>
        public List<SOItemInfo> AllocateStock(int shoppingMasterSysNo, int areaSysNo, int shipTypeSysNo, string companyCode, out AllocateInventoryStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllocateInventoryForSaleOrder");
            command.SetParameterValue("@ShoppingMasterSysNo", shoppingMasterSysNo);
            command.SetParameterValue("@ShippingAreaSysNo", areaSysNo);
            command.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = command.ExecuteDataTable();
            List<SOItemInfo> result = new List<SOItemInfo>();
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SOItemInfo soItemInfo = new SOItemInfo();
                    soItemInfo.ProductSysNo = Convert.ToInt32(dt.Rows[i]["ProductSysNo"]);
                    soItemInfo.ProductID = dt.Rows[i]["ItemNumber"].ToString();
                    soItemInfo.ProductType = (SOProductType)Convert.ToInt32(dt.Rows[i]["ProductType"]);
                    soItemInfo.StockSysNo = Convert.ToInt32(dt.Rows[i]["WarehouseNumber"]);
                    soItemInfo.ShoppingQty = Convert.ToInt32(dt.Rows[i]["ShoppingQty"]);
                    soItemInfo.AvailableQty = Convert.ToInt32(dt.Rows[i]["AvailableQty"]);
                    soItemInfo.StockName = dt.Rows[i]["StockName"]+"";
                    result.Add(soItemInfo);
                }
            }
            status = (AllocateInventoryStatus)command.GetParameterValue("@returnValue");
            return result;
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="soTotalWeight">订单重量</param>
        /// <param name="SOAmount">订单商品总金额</param>
        /// <param name="ShipTypeSysNo">配送方式编号</param>
        /// <param name="AreaSysNo">收货区域编号</param>
        /// <param name="CustomerSysNo">客户编号</param>
        /// <param name="soSingelMaxWeight">订单中单品最大重量</param>
        /// <param name="CompanyCode">公司编号</param>
        /// <returns></returns>
        public decimal? CaclShipPrice(int soTotalWeight, decimal? soAmount, int? shipTypeSysNo, int? areaSysNo, int? customerSysNo, int soSingelMaxWeight, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetShipPrice_EC");

            command.SetParameterValue("@SOTotalWeight", soTotalWeight);
            command.SetParameterValue("@SOAmount", soAmount);
            command.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            command.SetParameterValue("@AreaSysNo", areaSysNo);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@IsUseDiscount", false);
            command.SetParameterValue("@SOSingleMaxWeight", soSingelMaxWeight);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@CompanyCode", "8601");
            command.SetParameterValue("@StoreCompanyCode", "8601");

            DataSet ds = command.ExecuteDataSet();
            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                decimal result = Convert.ToDecimal(ds.Tables[0].Rows[0]["ShippingPrice"]);
                return result < 0 ? 0 : result;
            }
            return 0;
        }

        /// <summary>
        /// 从数据库删除订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="companyCode">公司编号</param>
        public void DeleteSOItem(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOItem");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 从数据库删除订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="companyCode">公司编号</param>
        public void DeleteSOItemExtend(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOItemExtend");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除主单中对应的Item
        /// </summary>
        /// <param name="SOSysNo"> 主单编号</param>
        /// <param name="ProductSysNo">商品编号</param>
        /// <param name="companyCode">公司编号</param>
        public void DeleteSOItemByProductSysNo(int soSysNo, int productSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOItemByProductSysNo");
            command.SetParameterValue("@companyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除订单赠品主信息
        /// </summary>
        /// <param name="SOSysNo">订单编号</param>
        public void DeleteSOGiftMaster(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOGiftMaster");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除订单赠品Item信息
        /// </summary>
        /// <param name="SOSysNo">订单编号</param>
        public void DeleteSOGiftItem(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOGiftItem");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除订单附件
        /// </summary>
        /// <param name="SOSysNo">订单编号</param>
        public void DeleteSOAccessory(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSOAccessory");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除绑定销售
        /// </summary>
        /// <param name="SOSysNo"></param>
        private void DeleteSOCombo(int SOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Delete_SOSaleRuleBySOSysNo");
            command.SetParameterValue("@SOSysNo", SOSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 设置订单拆分类型
        /// </summary>
        /// <param name="?"></param>
        public void SetSoSplitType(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetSoSplitType");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);
            command.SetParameterValue("@SoSplitType", soInfo.BaseInfo.SplitType);
            command.SetParameterValue("@CompanyCode", soInfo.BaseInfo.CompanyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 订单中的商品是否是同城分仓
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public bool IsSameCityStock(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsSameCityStock");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 存在商品接受预定
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public bool ExistEngageItem(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ExistEngageItem");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SOSysNo", soSysNo);
            int count = command.ExecuteScalar<int>();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据配送方式编号 判断是否为 指定仓发货
        /// </summary>
        /// <param name="ShipTypeSysNo"></param>
        /// <returns></returns>
        public bool IsOnlyForStockShipType(int ShipTypeSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetOnlyForStockSysNoByShipTypSysNo");
            command.SetParameterValue("@SysNo", ShipTypeSysNo);
            object obj = command.ExecuteScalar();
            int ResultVale = (obj == null || Convert.IsDBNull(obj)) ? 0 : Convert.ToInt32(obj);
            if (ResultVale != 0)
            {
                return true;
            }
            return false;
        }

        #region 订单创建 更新 持久化

        /// <summary>
        /// 持久化订单主信息
        /// </summary>
        /// <param name="sOInfo"></param>
        public void PersintenceMaster(SOInfo soInfo, bool isUpdate)
        {
            if (isUpdate)
            {
                this.UpdateSOMaster(soInfo);
                this.UpdateSOCheckShipping(soInfo);
            }
            else
            {
                InsertSOMainInfo(soInfo);
            }
        }

        /// <summary>
        /// 持久化订单商品信息
        /// </summary>
        /// <param name="soItemInfo"></param>
        public void PersintenceItem(SOInfo soInfo, bool isUpdate)
        {
            if (isUpdate)
            {
                this.DeleteSOItem(soInfo.SysNo.Value, soInfo.BaseInfo.CompanyCode);
            }

            this.InsertSOItemInfo(soInfo);
        }

        /// <summary>
        /// 持久化订单促销信息（赠品、附件、）
        /// </summary>
        /// <param name="promotionInfo"></param>
        public void PersintencePromotion(SOInfo soInfo, bool isUpdate)
        {
            if (isUpdate)
            {
                this.DeleteSOPromotionInfo(soInfo.SysNo.Value);
            }
            foreach (SOPromotionInfo promotion in soInfo.SOPromotions)
            {
                this.InsertSOPromotionInfo(promotion, soInfo.CompanyCode);
            }
        }

        /// <summary>
        /// 持久化订单礼品卡信息
        /// </summary>
        /// <param name="giftCardInfo"></param>
        public void PersintenceGiftCard(SOInfo soInfo, bool isUpdate)
        {

        }

        /// <summary>
        /// 持久化订单其他相关信息
        /// </summary>
        /// <param name="promotionInfo"></param>
        public void PersintenceExtend(SOInfo soInfo, bool isUpdate)
        {
            if (soInfo.InvoiceInfo != null
                && soInfo.InvoiceInfo.IsVAT.HasValue
                && soInfo.InvoiceInfo.IsVAT == true
                && soInfo.InvoiceInfo.VATInvoiceInfo != null)
            {
                //更新增值税发票信息
                UpdateSOVATInvoice(soInfo.InvoiceInfo.VATInvoiceInfo);
                ECCentral.BizEntity.SO.SOInvoiceChangeLogInfo invoiceChanageLog = new SOInvoiceChangeLogInfo
                {
                    SOSysNo = soInfo.BaseInfo.SysNo.Value,
                    StockSysNo = soInfo.Items[0].StockSysNo > 0 ? soInfo.Items[0].StockSysNo.Value : 0,
                    ChangeTime = DateTime.Now,
                    ChangeType = InvoiceChangeType.VATChange,
                    CompanyCode = soInfo.BaseInfo.CompanyCode,
                    UserSysNo = ServiceContext.Current.UserSysNo,
                    Note = string.Format(ResouceManager.GetMessageString("SO.SOInfo", "Res_SO_VATInvoice_ChangeInfo")
                    , soInfo.InvoiceInfo.VATInvoiceInfo.SysNo
                    , soInfo.InvoiceInfo.VATInvoiceInfo.CompanyName
                    , soInfo.InvoiceInfo.VATInvoiceInfo.TaxNumber
                    , soInfo.InvoiceInfo.VATInvoiceInfo.BankAccount)
                };
                ObjectFactory<ISOLogDA>.Instance.InsertSOInvoiceChangeLogInfo(invoiceChanageLog);
            }

            if (isUpdate)
            {
                DeleteSOItemExtend(soInfo.SysNo.Value);
            }

            bool isManualPrice = soInfo.Items.Exists(p => p.AdjustPrice != 0 && !string.IsNullOrEmpty(p.AdjustPriceReason));
            if (isManualPrice)
            {
                //价格补偿
                foreach (var item in soInfo.Items)
                {
                    if (item.AdjustPrice != 0 && !string.IsNullOrEmpty(item.AdjustPriceReason))
                    {
                        InsertItemExtend(item, soInfo.BaseInfo.CompanyCode, ServiceContext.Current.UserSysNo.ToString());
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 添加订单邮政自提信息
        /// </summary>
        /// <param name="postInfo"></param>
        public void InsertChinaPost(SOChinaPostInfo postInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_ChinaPost");
            command.SetParameterValue(postInfo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///  验证支付方式与配送方式是否匹配
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public bool CheckPayTypeByShipType(int payTypeSysNo, int shipTypeSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetShipPayList");
            command.SetParameterValue("@payTypeSysNo", payTypeSysNo);
            command.SetParameterValue("@shipTypeSysNo", shipTypeSysNo);
            command.SetParameterValue("@companyCode", companyCode);
            return command.ExecuteScalar() == null ? true : false;
        }

        /// <summary>
        /// 添加毛利分配信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertSOItemGossProfit(ItemGrossProfitInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_ItemGossProfit");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }

        public void PersintenceGrossProfit(SOInfo soInfo, bool isUpdate)
        {
            if (soInfo.ItemGrossProfitList != null && soInfo.ItemGrossProfitList.Count > 0)
            {

                //作废原先数据
                this.ChangedGossLogStatus(soInfo.BaseInfo.SysNo.Value, "D", ServiceContext.Current.UserSysNo.ToString());


                //插入数据
                foreach (ItemGrossProfitInfo gross in soInfo.ItemGrossProfitList)
                {
                    this.InsertSOItemGossProfit(gross);
                }
            }


        }

        public int ChangedGossLogStatus(int soSysNo, string status, string editUser)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangedGossLogStatus");

            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@EditUser", editUser);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 判断是否可用帐期支付
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="payDays">账期天数</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public bool IsAcctConfirmedSO(int customerSysNo, int payDays, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_HasAcctConfirmed");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            command.SetParameterValue("@PayDays", payDays);

            int count = command.ExecuteScalar<int>();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 修改订单相关方法

        public void UpdateSOMaster(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOMaster");
            
            if (soInfo != null && soInfo.BaseInfo != null && soInfo.BaseInfo.TariffAmount.HasValue)
            {
                soInfo.BaseInfo.TariffAmount = Math.Round(soInfo.BaseInfo.TariffAmount.Value, 2, MidpointRounding.AwayFromZero);
            }
            command.SetParameterValue(soInfo, true, false);
            command.SetParameterValue("@HoldMark", soInfo.BaseInfo.HoldStatus == SOHoldStatus.BackHold ? 1 : 0);
            SOItemInfo itemInfo = soInfo.Items.Find(info =>
            {
                return info.ProductType.Value == SOProductType.Coupon;
            });
            if (itemInfo != null)
            {
                command.SetParameterValue("@PromotionCodeSysNo", itemInfo.ProductSysNo);
            }
            SOStatusChangeInfo statusChangeInfo = soInfo.StatusChangeInfoList.Find(info => { return info.OldStatus == SOStatus.Origin && info.Status != info.OldStatus; });
            if (statusChangeInfo != null)
            {
                command.SetParameterValue("@AuditUserSysNo", statusChangeInfo.OperatorSysNo);
                command.SetParameterValue("@AuditTime", statusChangeInfo.ChangeTime);
            }
            statusChangeInfo = soInfo.StatusChangeInfoList.Find(info => { return info.OldStatus == SOStatus.WaitingManagerAudit && info.Status != info.OldStatus; });
            if (statusChangeInfo != null)
            {
                command.SetParameterValue("@ManagerAuditUserSysNo", statusChangeInfo.OperatorSysNo);
                command.SetParameterValue("@ManagerAuditTime", statusChangeInfo.ChangeTime);
            }
            command.ExecuteNonQuery();
        }

        public void UpdateSOCheckShipping(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOCheckShipping");
            command.SetParameterValue("@SOSysNo", soInfo.SysNo);
            command.SetParameterValue("@CompanyCode", soInfo.BaseInfo.CompanyCode);
            command.SetParameterValue("@WeightSO", soInfo.ShippingInfo.Weight);
            command.SetParameterValue("@ShippingFee", soInfo.ShippingInfo.ShippingFee);
            command.SetParameterValue("@PackageFee", soInfo.ShippingInfo.PackageFee);
            command.SetParameterValue("@RegisteredFee", soInfo.ShippingInfo.RegisteredFee);
            command.SetParameterValue("@UpdateTime", DateTime.Now);
            command.SetParameterValue("@Weight3PL", soInfo.ShippingInfo.Weight3PL);
            command.SetParameterValue("@ShipCost", soInfo.ShippingInfo.ShipCost);
            command.SetParameterValue("@IsFPSO", soInfo.FPInfo.FPSOType);
            command.SetParameterValue("@FPReason", soInfo.FPInfo.FPReason);
            command.SetParameterValue("@CustomerIPAddress", soInfo.ClientInfo.CustomerIPAddress);
            command.SetParameterValue("@IsFPCheck", soInfo.FPInfo.IsFPCheck);
            command.SetParameterValue("@FPCheckTime", soInfo.FPInfo.FPCheckTime);
            command.SetParameterValue("@SpecialSOType", soInfo.BaseInfo.SpecialSOType);
            command.SetParameterValue("@ShipCost3PL", soInfo.ShippingInfo.ShipCost3PL);
            command.SetParameterValue("@MemoForCustomer", soInfo.BaseInfo.MemoForCustomer);
            command.SetParameterValue("@VPOStatus", soInfo.ShippingInfo.VPOStatus);
            command.SetParameterValue("@IsPhoneOrder", soInfo.BaseInfo.IsPhoneOrder);
            command.SetParameterValue("@NeedInvoice", soInfo.BaseInfo.NeedInvoice);
            command.SetParameterValue("@OriginShipPrice", soInfo.ShippingInfo.OriginShipPrice);
            command.SetParameterValue("@IsMultiInvoice", soInfo.InvoiceInfo.IsMultiInvoice);
            command.SetParameterValue("@SplitUserSysNo", soInfo.InvoiceInfo.SplitUserSysNo);
            command.SetParameterValue("@SplitDateTime", soInfo.InvoiceInfo.SplitDateTime);
            command.SetParameterValue("@IsRequireShipInvoice", soInfo.InvoiceInfo.IsRequireShipInvoice);
            command.SetParameterValue("@IsVATPrinted", soInfo.InvoiceInfo.IsVATPrinted);
            command.SetParameterValue("@IsCombine", soInfo.ShippingInfo.IsCombine);
            command.SetParameterValue("@IsExtendWarrantyOrder", soInfo.BaseInfo.IsExtendWarrantyOrder);
            command.SetParameterValue("@IsExpiateOrder", soInfo.BaseInfo.IsExpiateOrder);
            command.SetParameterValue("@IsExperienceOrder", soInfo.BaseInfo.IsExperienceOrder);
            command.SetParameterValue("@RingOutShipType", soInfo.ShippingInfo.RingOutShipType);
            command.SetParameterValue("@SOType", soInfo.BaseInfo.SOType);
            command.SetParameterValue("@DeliveryPromise", soInfo.ShippingInfo.DeliveryPromise);
            command.SetParameterValue("@HoldDate", soInfo.BaseInfo.HoldTime);
            command.SetParameterValue("@HoldStatus", soInfo.BaseInfo.HoldStatus);
            command.SetParameterValue("@SoSplitMaster", soInfo.BaseInfo.SOSplitMaster);
            command.SetParameterValue("@DeliverySection", soInfo.ShippingInfo.DeliverySection);
            command.SetParameterValue("@DeliveryType", soInfo.ShippingInfo.DeliveryType);
            command.SetParameterValue("@VIPUserType", soInfo.BaseInfo.VIPUserType);
            command.SetParameterValue("@VIPSOType", soInfo.BaseInfo.VIPSOType);
            command.SetParameterValue("@SettlementStatus", soInfo.BaseInfo.SettlementStatus);
            command.SetParameterValue("@LocalWHSysNo", soInfo.ShippingInfo.LocalWHSysNo);
            command.ExecuteNonQuery();
        }

        public void UpdateSOCombineInfo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_UpdateCombineSOInfo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public void UpdateSOOutStockTime(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_UpdateOutStockTime");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public void UpdateSOVATPrinted(List<int> soSysNos)
        {
            string soSysNos_Str = SOSysNoListToString(soSysNos);

            if (!String.IsNullOrEmpty(soSysNos_Str))
            {
                CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Update_SOVATPrinted");
                command.CommandText = command.CommandText.Replace("#SOSysNo#", soSysNos_Str);
                command.ExecuteNonQuery();
            }
        }

        public bool UpdateSOHoldInfo(SOBaseInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOHoldInfo");
            command.SetParameterValue<SOBaseInfo>(info, true, false);
            command.SetParameterValue("@HoldMark", info.HoldStatus == SOHoldStatus.BackHold);
            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateSOForAbandon(SOInfo soInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_AbandonSO");
            command.SetParameterValue("@SysNo", soInfo.SysNo);
            SOOperatorType operatorType = soInfo.StatusChangeInfoList != null && soInfo.StatusChangeInfoList.Count > 0 ? soInfo.StatusChangeInfoList[0].OperatorType : (ServiceContext.Current.UserSysNo > 0 ? SOOperatorType.User : SOOperatorType.System);
            command.SetParameterValue("@Status", Mapping_SOStatus_ThisToIPP(soInfo.BaseInfo.Status.Value, operatorType, soInfo.BaseInfo.IsBackOrder.Value));
            command.SetParameterValue("@CashPay", soInfo.BaseInfo.CashPay);
            command.SetParameterValue("@Note", soInfo.SysNo);
            return command.ExecuteNonQuery() > 0;
        }

        public void UpdateSOItemAmountInfo(SOItemInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOItemAmountInfoBySysNo");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@CouponAverageDiscount", info.CouponAverageDiscount);
            command.SetParameterValue("@Price", info.Price);
            command.ExecuteNonQuery();
        }

        public void UpdateSOAmountInfo(SOBaseInfo soBaseInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOAmountInfo");
            InputMap<SOBaseInfo> maps = Map<SOBaseInfo>.Add("@IsUsePrepay", info => info.IsUsePrePay);
            command.SetParameterValue<SOBaseInfo>(maps, soBaseInfo, true, false);
            command.ExecuteNonQuery();
        }

        public void UpdateSOForSplitInvoice(int soSysNo, bool isMultiInvoice)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_ForSplitInvoice");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@IsMultiInvoice", isMultiInvoice);
            command.ExecuteNonQuery();
        }

        public bool WMSIsDownloadSO(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOIsDownLoad");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteScalar<bool>();
        }

        public int GetShoppingCartSysNoBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_ShoppingCartSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            object o = command.ExecuteScalar();
            return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        }

        public void DeleteSOItemBySOSysNoAndProductSysNo(int soSysNo, int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Delete_SOItemBySOSysNoAndProductSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.ExecuteNonQuery();
        }

        public int GetShiftSysNoBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_ShiftSysNoBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@Status", 1);
            object o = command.ExecuteScalar();
            return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        }

        public void DeleteSOItemBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Delete_SOItemBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            command.ExecuteNonQuery();
        }

        public void CancelSOExtendWarranty(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_CancelSOExtendWarranty");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public int? GetSOSysNoByCouponSysNo(int couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOSysNoByCouponSysNo");
            command.SetParameterValue("@CouponSysNo", couponSysNo);
            return command.ExecuteScalar<int?>();
        }

        public bool IsFirstSO(int soSysNo, int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsFristSO");
            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);
            return command.ExecuteScalar<int>() == 0;
        }

        public int UpdateSONote(int soSysNo, string note)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSONote");

            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@Note", note);

            int result = command.ExecuteNonQuery();
            return result;
        }
        #endregion

        #region 订单虚库相关

        /// <summary>
        /// 检查订单是否存在虚库申请
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public bool IsExistVirtualItemRequest(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsExistVirtualItemRequest");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 如果有没有作废的虚库申请则作废订单虚库申请
        /// </summary>
        /// <param name="soSysNo"></param>
        public void AbandonSOVirtualItemRequest(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_VirtualItemRequestStatusBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更改订单虚库采购单状态 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void UpdateSOCheckShippingVPOStatus(int soSysNo, BizEntity.PO.VirtualPurchaseOrderStatus vpoStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOCheckShippingVPOStatus");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@VPOStatus", vpoStatus);
            command.ExecuteNonQuery();
        }

        #endregion

        #region Newegg数据库 相关的方法
        /// <summary>
        /// 订单是否出库 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public bool IsNeweggOutStock(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_IsNeweggOutStock");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteNonQuery() > 0;
        }
        #endregion

        #region 虚库采购单相关

        public int GetGeneratedSOVirtualCount(int soItemSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("IsGeneratedSOVirtual");
            dataCommand.SetParameterValue("@Status", (int)VirtualPurchaseOrderStatus.Abandon);
            dataCommand.SetParameterValue("@SysNo", soItemSysNo);
            object o = dataCommand.ExecuteScalar();
            if (o == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(o);
            }
        }

        #endregion

        #region 获取推荐商品信息
        /// <summary>
        /// 获取某个订单的推荐商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public List<CommendatoryProductsInfo> GetCommendatoryProducts(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_CommendatoryProducts");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<CommendatoryProductsInfo>();
        }
        #endregion

        #region 团购相关

        public List<ProductGroupBuyInfo> GetAllGoupMaxPreOrder(int[] groupBuyingNoArr)
        {
            if (groupBuyingNoArr == null || groupBuyingNoArr.Length == 0)
            {
                return new List<ProductGroupBuyInfo>();
            }
            List<string> param = new List<string>();
            groupBuyingNoArr.ToList().ForEach(delegate(int tmp)
            {
                param.Add(tmp.ToString());
            });
            string result = string.Join(",", param.ToArray());
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllGoupMaxPreOrder");
            string sql = command.CommandText;
            sql = sql.Replace("#SysNo#", result);
            command.CommandText = sql;
            return command.ExecuteEntityList<ProductGroupBuyInfo>();
        }

        #endregion


        public StockInfo GetProductStockByProductSysNoAndQty(int productSysNo, int qty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductStockByProductSysNoAndQty");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@ProductQty", qty);

            return command.ExecuteEntity<StockInfo>();
        }

        /// <summary>
        /// 修改订单状态为报关失败作废
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToReportedFailure(int sosysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusToReportedFailure");
            command.SetParameterValue("@SOSysNo", sosysno);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改订单状态为 已申报待通关
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToReported(int sosysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusToReported");
            command.SetParameterValue("@SOSysNo", sosysno);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改订单SO_CheckShipping的LastChangeStatusDate
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOCheckShippingLastChangeStatusDate(int sosysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOCheckShippingLastChangeStatusDate");
            command.SetParameterValue("@SOSysNo", sosysno);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingList(ExpressType expressType)
        {
            List<string> result = null;
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_WaitingFinishShippingList");
            command.SetParameterValue("@ShipTypeSysNo", expressType);
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                result = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(dr[0].ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 获取快递100查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingListForKD100(ExpressType expressType)
        {
            List<string> result = null;
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_WaitingFinishShippingListForKD100");
            //command.SetParameterValue("@ShipTypeSysNo", expressType);
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                result = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(dr[0].ToString() + "," + dr[1].ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 根据运单号查询订单号
        /// </summary>
        /// <param name="trackingNumber">运单号</param>
        /// <returns></returns>
        public int GetSOSysNoByTrackingNumber(string trackingNumber)
        {
            int soSysNo = 0;
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOSysNoByTrackingNumber");
            command.SetParameterValue("@TrackingNumber", trackingNumber);
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                soSysNo = int.Parse(dt.Rows[0][0].ToString());
            }
            return soSysNo;
        }

        /// <summary>
        /// 修改订单状态为 申报失败订单作废
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToCustomsPass(int sosysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOStatusToCustomsPass");
            command.SetParameterValue("@SOSysNo", sosysno);
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 根据订单编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo)
        {
            return this.LoadVendorCustomsInfo(soSysNo, null);
        }
        /// <summary>
        /// 根据商家编号获取关务对接相关信息
        /// </summary>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public VendorCustomsInfo LoadVendorCustomsInfoByMerchant(int merchantSysNo)
        {
            return this.LoadVendorCustomsInfo(null, merchantSysNo);
        }
        /// <summary>
        /// 根据订单编号或者商家编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        private VendorCustomsInfo LoadVendorCustomsInfo(int? soSysNo, int? merchantSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_LoadVendorCustomsInfo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command, "ORDER BY som.SysNo DESC"))
            {
                if (soSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "som.SOID", DbType.String, "@SOID", QueryConditionOperatorType.Equal, soSysNo);
                }
                if (merchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vci.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, merchantSysNo);
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
            }
            return command.ExecuteEntity<VendorCustomsInfo>();
        }

        public SOCustomerAuthentication GetSOCustomerAuthentication(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOCustomer_Authentication");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<SOCustomerAuthentication>();
        }

        public void SOMaintainUpdateNote(SOInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOMaintain_UpdateNote");

            command.SetParameterValue("@SOSysNo", info.SysNo);
            command.SetParameterValue("@MemoForCustomer", info.BaseInfo.MemoForCustomer);
            command.SetParameterValue("@Note", info.BaseInfo.Note);
            command.SetParameterValue("@InvoiceNote", info.InvoiceInfo.InvoiceNote);
            command.SetParameterValue("@FinanceNote", info.InvoiceInfo.FinanceNote);
            //在保存备注时尝试保存区域变化 by John.E.Kang 2015.10.20
            command.SetParameterValue("@ReceiveAreaSysNo",info.ReceiverInfo.AreaSysNo);
            command.SetParameterValue("@ReceiveAddress",info.ReceiverInfo.Address);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 是否归还库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="soCreatDate">订单创建时间</param>
        /// <returns>true:归还</returns>
        public bool CheckReturnInventory(int productSysNo, DateTime soCreatDate)
        {
            bool result = false;
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckReturnInventory");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SOCreatDate", soCreatDate);
            int i = cmd.ExecuteScalar<Int32>();
            if (i > 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 根据订单号查询Netpay
        /// </summary>
        /// <param name="soSysNo">订单号</param>
        /// <returns></returns>
        public ECCentral.BizEntity.Invoice.NetPayInfo GetCenterDBNetpayBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetCenterDBNetpayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<ECCentral.BizEntity.Invoice.NetPayInfo>();
        }
            
    }
}

