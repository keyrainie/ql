using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.SOPipeline.Impl
{
    public class NeweggSpecialPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            //更新当前收货地址为用户的默认收货地址
            PipelineDA.SetCustomerShippingAddressAsDefault(order.Contact.ID, order.Customer.SysNo);

            foreach (var subOrder in order.SubOrderList.Values)
            {
                //更新用户积分信息
                if (subOrder.PointPayAmount > 0m)
                {
                    subOrder["Memo"] = "创建订单扣减积分";
                    var msgResult = PipelineDA.UpdatePointForCustomer(subOrder);
                    if (String.IsNullOrWhiteSpace(msgResult) || !msgResult.Trim().Equals("1000099"))
                    {
                        ECommerce.Utility.Logger.WriteLog("Update point of customer failed,ErrorCode:" + msgResult, "SOPipeline.NeweggSpecialPersister");
                        throw new BusinessException("用户积分不足");
                    }
                }

                //更新用户余额
                if (subOrder.BalancePayAmount > 0m)
                {
                    PipelineDA.UpdateCustomerPrepayBasic(subOrder);
                }

                //更新用户扩展信息
                PipelineDA.UpdateCustomerExtend(subOrder);

                //订单发货仓库
                //subOrder["LocalWHSysNo"] = subOrder.OrderItemGroupList.FirstOrDefault().ProductItemList.FirstOrDefault().WarehouseNumber;
                subOrder["LocalWHSysNo"] = subOrder["WarehouseNumber"];

                //订单运费检查
                PipelineDA.UpdateSOCheckShipping(subOrder);

                //创建订单扩展信息
                PipelineDA.CreateSOMasterExtension(subOrder);

                //创建订单商品扩展信息
                if (subOrder.OrderItemGroupList != null)
                {
                    //只记录团购商品信息，用于job后台结算
                    subOrder.OrderItemGroupList.ForEach(g =>
                    {
                        if (g.ProductItemList != null)
                        {
                            g.ProductItemList.Where(x => x.SpecialActivityType == 1)
                                             .ToList()
                                             .ForEach(item =>
                                                            {
                                                                item["SOSysNo"] = subOrder.ID;
                                                                item["ItemExtensionType"] = "G";
                                                                PipelineDA.CreateSOItemExtension(item);
                                                            });
                        }
                    });
                }

                //新赠品规则 
                subOrder.AttachmentItemList.ForEach(item =>
                {
                    item["SOSysNo"] = subOrder.ID;
                    PipelineDA.CreateSOItemAttachmentAccessory(item); //创建订单所有附件
                });

                List<DTOInfo> orderGiftMasterList = new List<DTOInfo>();
                subOrder.GiftItemList.ForEach(item =>
                {
                    DTOInfo dtoInfo = orderGiftMasterList.Find(f => (int)f["ActivityNo"] == item.ActivityNo);
                    if (dtoInfo == null)
                    {
                        dtoInfo = new DTOInfo();
                        dtoInfo["SOSysNo"] = subOrder.ID;
                        dtoInfo["ActivityNo"] = item.ActivityNo;
                        dtoInfo["SaleGiftType"] = item.SaleGiftType;
                        dtoInfo["Count"] = item.UnitQuantity;
                        dtoInfo["Order"] = 0;
                        orderGiftMasterList.Add(dtoInfo);
                    }
                    else
                    {
                        dtoInfo["Count"] = (int)dtoInfo["Count"] + item.UnitQuantity;
                        dtoInfo["Order"] = (int)dtoInfo["Order"] + 1;
                    }

                    PipelineDA.CreateSOItemGiftAccessory(item);//创建订单所有赠品
                    PipelineDA.CreateSOGiftItem(item);//促销活动订单赠品列表
                });

                foreach (DTOInfo dtoInfo in orderGiftMasterList)
                {
                    PipelineDA.CreateSOGiftMaster(dtoInfo);//促销活动订单赠送
                }

            }
        }
    }
}
