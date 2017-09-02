using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Report;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using System.Collections;
using InvoiceInfoReport = ECCentral.BizEntity.Invoice.InvoiceReport.InvoiceInfo;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(InvoiceReportProcessor))]
    public class InvoiceReportProcessor
    {
        private IInvoiceReportDA DA = ObjectFactory<IInvoiceReportDA>.Instance;

        public virtual TrackingNumberInfo CreateTrackingNumber(TrackingNumberInfo entity)
        {
            return DA.CreateTrackingNumber(entity);
        }

        #region 发票打印相关

        static readonly string InvoicePrintStocks = AppSettingManager.GetSetting("Invoice", "InvoicePrintStocks-8601");//发票打印支持的分仓号
        private Hashtable htUnion3GCardItems = new Hashtable(); //联通3G卡的Items
        private SOInvoiceMaster soMaster = null; //订单发票Master信息
        private List<InvoiceItem> notSplitInvoiceItems = null; //未拆分的发票明细Items          


        /// <summary>
        /// 根据订单发票实体对象获取对应的要打印的发票List
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SOInvoiceInfo GetNew(SOInvoiceInfo entity)
        {
            if (entity.SOInfo.SOSysNo == 0)
            {
                //throw new BizException("SOInfo.SOSysNo不能为空");
                ThrowBizException("InvoiceReport_SOInfoSOSysNoNotNull");
            }
            if (entity.SOInfo.StockSysNo == 0)
            {
                //throw new BizException("SOInfo.StockSysNo不能为空");
                ThrowBizException("InvoiceReport_SOInfoStockSysNoNoNotNull");
            }

            //获取订单发票Master信息（其中包括业务模式、是否拆分发票、其他费用等字段）
            this.soMaster = DA.GetSOInvoiceMaster(entity.SOInfo.SOSysNo.Value
                , entity.SOInfo.StockSysNo.Value
                , "NEG"
                , "MET");
            //数据校验
            if (soMaster == null)
            {
                //throw new BizException(string.Format("此订单号({0})分仓号({1})在拆分单中没有记录",
                //    entity.SOInfo.SOSysNo.ToString(),
                //    entity.SOInfo.StockSysNo.ToString()));
                ThrowBizException("InvoiceReport_NotExistOrderIDStockIDInSplit", entity.SOInfo.SOSysNo.ToString(), entity.SOInfo.StockSysNo.ToString());
            }
            if (soMaster.InvoiceType == null || soMaster.ShippingType == null || soMaster.StockType == null)
            {
                //throw new BizException(string.Format("此订单号({0})分仓号({1})的业务模式相关字段为NULL值",
                //  this.soMaster.SOSysNo.ToString(),
                //  this.soMaster.StockSysNo.ToString()));
                ThrowBizException("InvoiceReport_NotExistFieldOrderIDStockIDInSplit", this.soMaster.SOSysNo.ToString(), this.soMaster.StockSysNo.ToString());
            }
            //判断MKT PlACE II需求的业务模式            
            if (!this.IsAllowGetPrintIncoiceinfo())
            {
                //throw new BizException(string.Format("此订单号({0})分仓号({1})的业务模式不支持发票打印",
                //    this.soMaster.SOSysNo.ToString(),
                //    this.soMaster.StockSysNo.ToString()));
                ThrowBizException("InvoiceReport_NotSurportInvoicePrintOfOrderIDStockID", this.soMaster.SOSysNo.ToString(), this.soMaster.StockSysNo.ToString());
            }
            else
            {
                //判断业务模式5的发票金额是否小于零，不打印
                if (this.soMaster.BussinessMode == 5 && this.soMaster.InvoiceAmt <= 0)
                {
                    //throw new BizException(string.Format("此订单号({0})分仓号({1})的业务模式为“商家仓储泰隆优选配送”+“商家开票”模式，但发票金额为零元，不支持发票打印",
                    // this.soMaster.SOSysNo.ToString(),
                    // this.soMaster.StockSysNo.ToString()));
                    ThrowBizException("InvoiceReport_NotSurportInvoicePrintOfOrderIDStockIDWhenAmountIsZero", this.soMaster.SOSysNo.ToString(), this.soMaster.StockSysNo.ToString());
                }
                //判断业务模式5的发票是否发生拆分，不支持
                if (this.soMaster.BussinessMode == 5 && this.soMaster.IsMultiInvoice)
                {
                    //throw new BizException(string.Format("此订单号({0})分仓号({1})的业务模式为“商家仓储泰隆优选配送”+“商家开票”模式，但存在拆分发票的异常数据，不支持发票打印",
                    //this.soMaster.SOSysNo.ToString(),
                    //this.soMaster.StockSysNo.ToString()));
                    ThrowBizException("InvoiceReport_NotSurportInvoicePrintOfOrderIDStockIDWhenDataError", this.soMaster.SOSysNo.ToString(), this.soMaster.StockSysNo.ToString());
                }
            }
            //获取联通3G卡产品Items
            if (htUnion3GCardItems.Count == 0)
            {
                htUnion3GCardItems = this.GetUnion3GSIMCardItems();
            }

            //获取发票实体列表
            entity.InvoiceInfoList = this.GetListInvoiceInfo();

            //未拆分的发票明细Items清空
            this.notSplitInvoiceItems = new List<InvoiceItem>();

            return entity;
        }

        #region 获取发票实体列表
        /// <summary>
        /// 获取发票实体列表
        /// </summary>
        /// <returns></returns>
        private List<InvoiceInfoReport> GetListInvoiceInfo()
        {
            List<InvoiceInfoReport> results = new List<InvoiceInfoReport>();

            //分页大小
            int nSplitPageSize = this.GetInvoiceInfoPageSizeByStockNo(soMaster.StockSysNo);

            //先构造未执行任何拆分的源发票实体
            InvoiceInfoReport sourceInvoiceEntity = this.CreateSourceInvoiceInfoEntity();

            //如果有发票拆分，执行发票拆分
            if (soMaster.IsMultiInvoice)
            {
                //第一次拆分：按照发票拆分规则
                List<InvoiceInfoReport> lstNewSub = this.SplitByInvoiceSub(sourceInvoiceEntity);

                //第二次拆分：按照分页大小
                lstNewSub.ForEach(x =>
                {
                    List<InvoiceInfoReport> lstNewPager = this.SplitByPageSize(x, nSplitPageSize);
                    results.AddRange(lstNewPager);
                });
            }
            else
            {
                //没有发票拆分，将延保产品的ItemNumber恢复为空值
                this.InvoiceItems_YBEmptyItemNumber();

                //执行分页拆分
                List<InvoiceInfoReport> lstNewPager = this.SplitByPageSize(sourceInvoiceEntity, nSplitPageSize);
                results.AddRange(lstNewPager);
            }
            return results;
        }

        /// <summary>
        /// 构造未拆分的源发票实体
        /// </summary>
        private InvoiceInfoReport CreateSourceInvoiceInfoEntity()
        {
            ECCentral.BizEntity.Invoice.InvoiceReport.InvoiceInfo invoiceInfo = DA.GetInvoicePrintHead(soMaster.SOSysNo, soMaster.StockSysNo);

            //发票总额处理（万里通/中智积分支付的特殊处理）            
            if (this.soMaster.PayTypeSysNo == 47 || this.soMaster.PayTypeSysNo == 48)
            {
                //修改总发票金额为1分钱
                invoiceInfo.InvoiceAmt = 0.01m;
            }
            invoiceInfo.RMBConvert = this.GetRMBConvert(invoiceInfo.InvoiceAmt); //人民币大写金额

            //Head默认值处理
            invoiceInfo.InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd");
            invoiceInfo.InvoiceSeq = 1; //拆分序号，先默认为1
            invoiceInfo.InvoiceSeqEx = "";
            invoiceInfo.InvoiceSumPageNum = 1; //总页码，先默认为1
            invoiceInfo.InvoiceCurPageNum = 1; //当前页码，先默认为1

            //Head特殊处理
            //是否阿斯利康客户
            if (this.soMaster.SpecialSOType == 2)
            {
                invoiceInfo.ServicePhone = "阿斯利康客服热线：400-600-5566";
                invoiceInfo.ReceiveContact = "阿斯利康(无锡)贸易有限公司";
            }
            //支付方式特殊处理         
            if (this.soMaster.IsUseChequesPay)
            {
                invoiceInfo.PayTypeName += "(支票支付)";
            }
            if (this.soMaster.IsRequireShipInvoice)
            {
                invoiceInfo.PayTypeName += "(客户需要圆通发票)";
            }
            //收货地址分解
            if (invoiceInfo.ReceiveAddress.Length > 35)
            {
                invoiceInfo.ReceiveAddress1 = invoiceInfo.ReceiveAddress.Substring(0, 35);
                invoiceInfo.ReceiveAddress2 = invoiceInfo.ReceiveAddress.Substring(35);
            }
            else
            {
                invoiceInfo.ReceiveAddress1 = invoiceInfo.ReceiveAddress;
            }

            //特别说明
            if (this.soMaster.IsPayWhenRecv && this.soMaster.PrepayAmt > 0m)
            {
                if (this.soMaster.IsCombine)
                {
                    invoiceInfo.Importance = "本订单已收到余额支付" + this.soMaster.PrepayAmt.ToString("#########0.00") + "元";
                }
                else
                {
                    decimal cashPay = this.GetFinallReceivableAmt(invoiceInfo.InvoiceAmt, this.soMaster.PrepayAmt, this.soMaster.IsPayWhenRecv);
                    invoiceInfo.Importance = "预支付" + this.soMaster.PrepayAmt.ToString("#########0.00") + "元，现支付" + cashPay.ToString("#########0.00") + "元";
                }
            }

            //获取发票明细
            invoiceInfo.Items = GetInvoiceItems();

            return invoiceInfo;
        }

        /// <summary>
        /// 获取发票明细
        /// </summary>
        /// <returns></returns>
        private List<InvoiceItem> GetInvoiceItems()
        {
            //MKT Place II需求,判断是否允许添加商品款项
            if (this.IsAllowAddBasicItems())
            {
                this.AddInvoiceItem_BasicItems();
            }
            //MKT Place II需求,判断是否允许添加其他费用款项
            if (this.IsAllowAddOtherFeesItems())
            {
                this.AddInvoiceItem_OtherFees();
            }
            return this.notSplitInvoiceItems;
        }
        #endregion

        #region 发票明细处理-基本项
        /// <summary>
        /// 添加商品款项
        /// </summary>
        private void AddInvoiceItem_BasicItems()
        {
            //基本产品Items
            InvoiceItems_Product();

            //如果有延保服务，处理延保服务Items
            if (soMaster.IsExtendWarrantyOrder)
            {
                InvoiceItems_ExtendWarrantyOrder();
            }

            //二手品Item保修信息处理
            InvoiceItems_SecondHand();

            //3G卡的Item处理            
            InvoiceItems_Union3GSimCardItems();
        }

        /// <summary>
        /// 发票明细处理-商品明细的处理
        /// </summary>
        /// <param name="soinfo"></param>
        /// <returns></returns>
        private void InvoiceItems_Product()
        {
            if (this.notSplitInvoiceItems == null)
            {
                this.notSplitInvoiceItems = new List<InvoiceItem>();
            }
            List<InvoiceItem> productItems = DA.GetSOInvoiceProductItem(soMaster.SOSysNo, soMaster.StockSysNo);
            this.notSplitInvoiceItems.AddRange(productItems);
        }

        /// <summary>
        /// 发票明细处理-延保服务处理
        /// </summary>
        private void InvoiceItems_ExtendWarrantyOrder()
        {
            if (this.notSplitInvoiceItems == null)
            {
                this.notSplitInvoiceItems = new List<InvoiceItem>();
            }
            List<InvoiceItem> extendItems = DA.GetSOExtendWarrantyItem(soMaster.SOSysNo, soMaster.StockSysNo);
            this.notSplitInvoiceItems.AddRange(extendItems);
        }

        /// <summary>
        /// 发票明细处理-二手品保修信息处理
        /// </summary>
        private void InvoiceItems_SecondHand()
        {
            if (this.notSplitInvoiceItems != null)
            {
                //如果产品类型为二手品，修改保修条例信息
                this.notSplitInvoiceItems.ForEach(x =>
                {
                    if (x.ProductType == 1)
                    {
                        x.RepairWarrantyDays = "请参照官网二手品保修条例";
                    }
                });
            }
        }

        /// <summary>
        /// 发票明细处理-对于延保产品，现在数据源中是“YB-”开头的，恢复成空值
        /// </summary>
        private void InvoiceItems_YBEmptyItemNumber()
        {
            if (this.notSplitInvoiceItems != null)
            {
                //如果是延保产品，恢复ItemNumber为空值
                this.notSplitInvoiceItems.ForEach(x =>
                {
                    if (x.ItemNumber.StartsWith("YB-"))
                    {
                        x.ItemNumber = "";
                    }
                });
            }
        }

        /// <summary>
        /// 联通3G卡的Items
        /// </summary>
        /// <returns></returns>
        private void InvoiceItems_Union3GSimCardItems()
        {
            if (this.notSplitInvoiceItems != null)
            {
                //如果是联通3G卡的Items，则将单价和金额修改为0元，只显示数量
                this.notSplitInvoiceItems.ForEach(x =>
                {
                    if (this.IsUnicom3GSIMCardSysNO(x.ProductSysNo))
                    {
                        x.UnitPrice = 0m;
                        x.SumExtendPrice = 0m;
                    }
                });
            }
        }


        #endregion

        #region 发票明细处理-非商品款项
        /// <summary>
        /// 添加其他费用款项
        /// </summary>
        private void AddInvoiceItem_OtherFees()
        {
            //如果发票未发生拆分，则直接增加其他费用Item；对发生拆分的在拆分逻辑中追加
            if (!soMaster.IsMultiInvoice)
            {
                if (this.notSplitInvoiceItems == null)
                {
                    this.notSplitInvoiceItems = new List<InvoiceItem>();
                }
                this.notSplitInvoiceItems.AddRange(this.GetOtherFreesItems());
            }
        }

        /// <summary>
        /// 增加发票非商品款项的明细
        /// </summary>        
        /// <param name="curInvoiceInfo">需要增加其他费用明细的拆分发票实体</param>
        /// <returns></returns>
        private void AddOtherFreesItems(ref InvoiceInfoReport curInvoiceInfo)
        {
            curInvoiceInfo.Items.AddRange(this.GetOtherFreesItems());
        }

        /// <summary>
        /// 获取其他费用款项明细列表
        /// </summary>
        /// <returns></returns>
        private List<InvoiceItem> GetOtherFreesItems()
        {
            List<InvoiceItem> otherFeesItems = new List<InvoiceItem>();

            //优惠券抵扣 (不累加项)
            if (this.soMaster.Promotion != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = this.soMaster.PromotionCodeSysNo.ToString(),
                    Description = string.IsNullOrEmpty(this.soMaster.PromotionCodeName) ? "优惠卷抵扣" : this.soMaster.PromotionCodeName,
                    ProductType = 3,
                    SumExtendPrice = this.soMaster.Promotion
                });
            }
            //折扣(-)
            if (this.soMaster.Discount != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "促销优惠",
                    SumExtendPrice = this.soMaster.Discount
                });
            }
            //本单积分抵扣 (不累加项)
            if (this.soMaster.PointPay != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "本单积分抵扣",
                    SumExtendPrice = this.soMaster.PointPay
                });
            }
            //本单可得积分 (不累加项)
            if (this.soMaster.PointAmt != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "本单可得积分",
                    ProductType = 4,
                    SumExtendPrice = this.soMaster.PointAmt
                });
            }
            //运费(+)
            if (this.soMaster.ShippingCharge != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "运费",
                    SumExtendPrice = this.soMaster.ShippingCharge
                });
            }
            //保价费(+)
            if (this.soMaster.PremiumAmt != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "保价费",
                    SumExtendPrice = this.soMaster.PremiumAmt
                });
            }
            //手续费(+)
            if (this.soMaster.PayPrice != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "手续费",
                    SumExtendPrice = this.soMaster.PayPrice
                });
            }
            //礼品卡支付(-)
            if (this.soMaster.GiftCardPay != 0)
            {
                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = "礼品卡支付",
                    SumExtendPrice = this.soMaster.GiftCardPay
                });
            }
            //万里通/中智积分支付 (-)
            if (this.soMaster.PayTypeSysNo == 47 || this.soMaster.PayTypeSysNo == 48)
            {
                //获取积分抵扣的金额                
                decimal pointPayDiscount = -1 * this.GetPointPayDiscountAmt();
                string payTypeName = (this.soMaster.PayTypeSysNo == 47) ? "中智积分汇" : "平安万里通";

                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = payTypeName + "抵扣",
                    SumExtendPrice = pointPayDiscount
                });

                otherFeesItems.Add(new InvoiceItem()
                {
                    ItemNumber = payTypeName + "手续费",
                    SumExtendPrice = 0.01m
                });
            }

            return otherFeesItems;
        }

        #endregion

        #region 第一次拆分：按发票拆分规则处理
        /// <summary>
        /// 按照发票拆分规则拆分发票实体
        /// 注意：拆分序号为1的拆分票需要增加其他费用Items，但最后输出
        /// </summary>
        /// <param name="unSplitInvoiceEntity">未拆分的发票实体</param>
        /// <returns>已拆分的发票实体List</returns>
        private List<InvoiceInfoReport> SplitByInvoiceSub(InvoiceInfoReport unSplitInvoiceEntity)
        {
            int maxInvoiceSeq = 1; //最大的拆分序号，默认为1            
            decimal sumInvoiceSum = unSplitInvoiceEntity.InvoiceAmt; // 源发票的总金额            
            decimal subInvoiceSum = 0; // 累计已拆分的单张发票的总金额

            List<InvoiceInfoReport> results = new List<InvoiceInfoReport>();

            #region 获取发票拆分规则
            //获取发票拆分规则
            List<InvoiceSub> invoiceSubs = DA.GetInvoiceSub(soMaster.SOSysNo, soMaster.StockSysNo);
            //按照InvoiceSeq字段Group集合
            var groups = from u in invoiceSubs
                         group u by new { u.InvoiceSeq } into g
                         select g;
            //计算最大的拆分序号
            Dictionary<int, List<InvoiceSub>> dicSeqSubs = new Dictionary<int, List<InvoiceSub>>();
            foreach (var data in groups)
            {
                dicSeqSubs.Add(data.Key.InvoiceSeq, data.ToList());
            }
            //最大拆分号 就是 dicSeqSubs.Count值
            maxInvoiceSeq = dicSeqSubs.Count;
            #endregion

            #region 按InvoiceSeq分组生成拆分发票
            //拆分后新的序号，默认从1开始
            int curNewSeqNo = 1;
            //按InvoiceSeq分组生成拆分发票，为保证InvoiceSub表中序号为1的追加费用明细，倒序拆分            
            for (int xhkey = dicSeqSubs.Count; xhkey > 0; xhkey--)
            {
                //当前拆分实体                
                List<InvoiceSub> curSeqs = dicSeqSubs[xhkey];

                //当前拆分序号
                int curInvoiceSeq = xhkey;

                #region Head处理
                //生成一个新的发票实体(只取Head部分);
                InvoiceInfoReport newInvoiceInfo = new InvoiceInfoReport();

                newInvoiceInfo.InvoiceSeq = curNewSeqNo; //修改：InvoiceSeq
                newInvoiceInfo.InvoiceSeqEx = curNewSeqNo.ToString() + "-" + maxInvoiceSeq.ToString(); //修改：拆分序号-总拆分张数

                newInvoiceInfo.InvoiceDate = unSplitInvoiceEntity.InvoiceDate;
                newInvoiceInfo.InvoiceSumPageNum = unSplitInvoiceEntity.InvoiceSumPageNum;
                newInvoiceInfo.InvoiceCurPageNum = unSplitInvoiceEntity.InvoiceCurPageNum;
                newInvoiceInfo.ServicePhone = unSplitInvoiceEntity.ServicePhone;
                newInvoiceInfo.SOSysNo = unSplitInvoiceEntity.SOSysNo;
                newInvoiceInfo.CustomerSysNo = unSplitInvoiceEntity.CustomerSysNo;
                newInvoiceInfo.InvoiceNote = unSplitInvoiceEntity.InvoiceNote;
                newInvoiceInfo.PayTypeName = unSplitInvoiceEntity.PayTypeName;
                newInvoiceInfo.PayTypeSysNo = unSplitInvoiceEntity.PayTypeSysNo;
                newInvoiceInfo.ReceiveAddress1 = unSplitInvoiceEntity.ReceiveAddress1;
                newInvoiceInfo.ReceiveAddress2 = unSplitInvoiceEntity.ReceiveAddress2;
                newInvoiceInfo.ReceiveContact = unSplitInvoiceEntity.ReceiveContact;
                newInvoiceInfo.ReceiveName = unSplitInvoiceEntity.ReceiveName;
                newInvoiceInfo.ReceivePhone = unSplitInvoiceEntity.ReceivePhone;
                newInvoiceInfo.ShipTypeName = unSplitInvoiceEntity.ShipTypeName;
                newInvoiceInfo.TotalWeight = unSplitInvoiceEntity.TotalWeight;
                newInvoiceInfo.Importance = unSplitInvoiceEntity.Importance;
                newInvoiceInfo.Items = new List<InvoiceItem>();
                #endregion

                #region Items处理
                //Items处理
                //对当前Seq的拆分规则排序，以保证正常商品Item在前面，延保商品Item在后面
                var orderQuery = from o in curSeqs
                                 orderby o.IsExtendWarrantyItem
                                 select o;

                List<InvoiceItem> splitNewItems = newInvoiceInfo.Items;
                foreach (InvoiceSub splits in orderQuery.ToList<InvoiceSub>())
                {
                    //查找源明细中对应拆分项的Item
                    InvoiceItem sourceItem = null;
                    var itemQuery = from invoicetems in unSplitInvoiceEntity.Items
                                    where invoicetems.ItemNumberEx == splits.ProductID
                                    select invoicetems;

                    List<InvoiceItem> sourceItems = itemQuery.ToList();
                    if (sourceItems.Count > 0)
                    {
                        sourceItem = sourceItems[0];

                        //将拆分Item添加到List
                        InvoiceItem splitItem = new InvoiceItem();
                        splitItem.ItemNumber = sourceItem.ItemNumber;
                        splitItem.Description = sourceItem.Description;
                        splitItem.UnitPrice = sourceItem.UnitPrice;
                        splitItem.Quantity = splits.SplitQty; //取拆分数量
                        splitItem.SumExtendPrice = sourceItem.UnitPrice * splits.SplitQty; //金额按拆分数量重新计算
                        splitItem.RepairWarrantyDays = sourceItem.RepairWarrantyDays;
                        splitItem.ProductType = sourceItem.ProductType;
                        splitItem.IsExtendWarrantyItem = splits.IsExtendWarrantyItem;
                        splitItem.ProductSysNo = splits.ProductSysNo;
                        splitItem.MasterProductSysNo = splits.MasterProductSysNo;

                        splitNewItems.Add(splitItem);
                    }
                }

                //拆分完毕后，将延保产品的ItemNumber恢复为空值
                this.InvoiceItems_YBEmptyItemNumber();
                #endregion

                #region 金额处理
                //当前拆分发票的总金额
                decimal curInvoiceSumAmt = 0;
                //如果是序号为1的拆分票，则增加运费等杂项
                if (curInvoiceSeq == 1)
                {
                    //增加费用明细项,MKT Place II需求要判断是否可添加其他费用明细项
                    if (this.IsAllowAddOtherFeesItems())
                    {
                        this.AddOtherFreesItems(ref newInvoiceInfo);
                    }

                    //当前票的金额 = 总发票金额 - 前面已经拆分的发票累计金额
                    curInvoiceSumAmt = sumInvoiceSum - subInvoiceSum;
                }
                else
                {
                    //按照当前发票实体明细计算总金额
                    curInvoiceSumAmt = this.CalculateInvoiceItemSumAmt(newInvoiceInfo);
                    //累计已拆分的总金额
                    subInvoiceSum += curInvoiceSumAmt;
                }

                //更新Head中金额相关字段
                newInvoiceInfo.InvoiceAmt = curInvoiceSumAmt;
                newInvoiceInfo.RMBConvert = this.GetRMBConvert(curInvoiceSumAmt);
                #endregion

                //ADD InvoiceInfo
                results.Add(newInvoiceInfo);

                curNewSeqNo++;
            }
            #endregion

            return results;
        }
        #endregion

        #region 第二次拆分：分页拆分处理
        /// <summary>
        /// 按分页大小拆分发票实体
        /// </summary>
        /// <param name="unSplitInvoiceEntity">未拆分的发票实体</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>已拆分的发票实体List</returns>
        private List<InvoiceInfoReport> SplitByPageSize(InvoiceInfoReport unSplitInvoiceEntity, int pageSize)
        {
            int sumSourceItemsNum = unSplitInvoiceEntity.Items.Count; //原始未拆分的Item总数
            int sumSplitPagerNum = 1; //拆分的页总数

            decimal sumInvoiceSum = unSplitInvoiceEntity.InvoiceAmt; // 源发票的总金额            
            decimal subInvoiceSum = 0; // 累计已拆分的单张发票的总金额

            List<InvoiceInfoReport> results = new List<InvoiceInfoReport>();
            if (sumSourceItemsNum > pageSize)
            {
                //计算要拆分的总页数
                sumSplitPagerNum = (sumSourceItemsNum % pageSize == 0 ? sumSourceItemsNum / pageSize : sumSourceItemsNum / pageSize + 1);

                for (int i = 1; i <= sumSplitPagerNum; i++)
                {
                    //当前页码
                    int curPagerNum = i;

                    #region Head处理
                    //生成一个新的发票实体(只取Head部分);
                    InvoiceInfoReport newPageInvoiceInfo = new InvoiceInfoReport();

                    newPageInvoiceInfo.InvoiceCurPageNum = curPagerNum; //修改当前页码
                    newPageInvoiceInfo.InvoiceSumPageNum = sumSplitPagerNum; //修改分页总数

                    newPageInvoiceInfo.InvoiceDate = unSplitInvoiceEntity.InvoiceDate;
                    newPageInvoiceInfo.ServicePhone = unSplitInvoiceEntity.ServicePhone;
                    newPageInvoiceInfo.ReceiveName = unSplitInvoiceEntity.ReceiveName;
                    newPageInvoiceInfo.ReceivePhone = unSplitInvoiceEntity.ReceivePhone;
                    newPageInvoiceInfo.ReceiveContact = unSplitInvoiceEntity.ReceiveContact;
                    newPageInvoiceInfo.ReceiveAddress1 = unSplitInvoiceEntity.ReceiveAddress1;
                    newPageInvoiceInfo.ReceiveAddress2 = unSplitInvoiceEntity.ReceiveAddress2;
                    newPageInvoiceInfo.SOSysNo = unSplitInvoiceEntity.SOSysNo;
                    newPageInvoiceInfo.CustomerSysNo = unSplitInvoiceEntity.CustomerSysNo;
                    newPageInvoiceInfo.InvoiceNote = unSplitInvoiceEntity.InvoiceNote;
                    newPageInvoiceInfo.PayTypeName = unSplitInvoiceEntity.PayTypeName;
                    newPageInvoiceInfo.PayTypeSysNo = unSplitInvoiceEntity.PayTypeSysNo;
                    newPageInvoiceInfo.InvoiceSeq = unSplitInvoiceEntity.InvoiceSeq;
                    newPageInvoiceInfo.InvoiceSeqEx = unSplitInvoiceEntity.InvoiceSeqEx;
                    newPageInvoiceInfo.ShipTypeName = unSplitInvoiceEntity.ShipTypeName;
                    newPageInvoiceInfo.TotalWeight = unSplitInvoiceEntity.TotalWeight;
                    newPageInvoiceInfo.Importance = unSplitInvoiceEntity.Importance;
                    newPageInvoiceInfo.Items = new List<InvoiceItem>();
                    #endregion

                    #region Item处理
                    //获取当前页面的Items
                    List<InvoiceItem> curPageItems = unSplitInvoiceEntity.Items.Skip((curPagerNum - 1) * pageSize).Take(pageSize).ToList();
                    //追加到Items
                    newPageInvoiceInfo.Items.AddRange(curPageItems);
                    #endregion

                    #region 金额处理
                    //当前拆分发票的总金额
                    decimal curInvoiceSumAmt = 0;
                    //如果是最后一张拆分票
                    if (curPagerNum == sumSplitPagerNum)
                    {
                        //当前票的金额 = 总发票金额 - 前面已经拆分的发票累计金额
                        curInvoiceSumAmt = sumInvoiceSum - subInvoiceSum;
                    }
                    else
                    {
                        //按照当前发票实体明细的计算总金额
                        curInvoiceSumAmt = this.CalculateInvoiceItemSumAmt(newPageInvoiceInfo);
                        //累计已拆分的总金额
                        subInvoiceSum += curInvoiceSumAmt;
                    }

                    //更新Head中金额相关字段
                    newPageInvoiceInfo.InvoiceAmt = curInvoiceSumAmt;
                    newPageInvoiceInfo.RMBConvert = this.GetRMBConvert(curInvoiceSumAmt);
                    #endregion

                    //ADD InvoiceInfo
                    results.Add(newPageInvoiceInfo);
                }
            }
            else
            {
                results.Add(unSplitInvoiceEntity);
            }
            return results;
        }

        #endregion

        #region 其他方法
        /// <summary>
        /// 联通3G卡需求,获取联通3G卡的产品Items
        /// </summary>
        /// <returns></returns>
        private Hashtable GetUnion3GSIMCardItems()
        {
            Hashtable htItems = new Hashtable();
            string key1 = "SIMCardDepositItem";
            if (!htItems.ContainsKey(key1))
            {
                htItems.Add(key1, GetValue(key1));
            }

            string key2 = "SIMCardItemList_Aft";
            if (!htItems.ContainsKey(key2))
            {
                htItems.Add(key2, GetValue(key2));
            }

            string key3 = "SIMCardItemList_Pre";
            if (!htItems.ContainsKey(key3))
            {
                htItems.Add(key3, GetValue(key3));
            }
            return htItems;
        }

        private string GetValue(string key)
        {
            string value = DA.GetSysConfiguration(key);
            return value;
        }

        /// <summary>
        /// MKT Place II需求,获取支持MKT Place II需求的分仓，目前只在51、59、90、99仓支持
        /// </summary>
        /// <returns></returns>
        private string GetMKTPlaceIIStockNOString()
        {
            if (InvoicePrintStocks == null)
            {
                return "51,59,90,99";
            }
            return InvoicePrintStocks;
        }

        /// <summary>
        /// 联通3G卡需求，是否是联通3G卡的产品Item
        /// </summary>
        /// <param name="proSysNO">产品SysNO</param>
        /// <returns></returns>
        private bool IsUnicom3GSIMCardSysNO(int proSysNO)
        {
            if (this.htUnion3GCardItems.ContainsValue(proSysNO.ToString()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// MKT Place II需求,是否是支持MKT Place II需求的分仓
        /// </summary>
        /// <param name="stockNo">分仓号</param>
        /// <returns>true:支持；false:不支持</returns>
        private bool IsSupportMKTPLaceIIStockNo(int stockNo)
        {
            string MKTPlaceIIStockSysNo = GetMKTPlaceIIStockNOString();

            if (MKTPlaceIIStockSysNo.IndexOf(stockNo.ToString()) != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// MKT Place II需求,是否允许发票打印
        /// </summary>
        /// <returns>true:允许；false:不允许</returns>
        private bool IsAllowGetPrintIncoiceinfo()
        {
            bool allow = false;
            //MKT Place II需求，需判断业务模式是否支持发票打印
            if (this.IsSupportMKTPLaceIIStockNo(this.soMaster.StockSysNo))  //
            {
                //判断业务模式
                switch (this.soMaster.BussinessMode)
                {
                    //业务模式1,2,3: 泰隆优选开票的，全部支持
                    case 1:
                        allow = true;
                        break;
                    case 2:
                        allow = true;
                        break;
                    case 3:
                        allow = true;
                        break;
                    //业务模式4：泰隆优选仓储泰隆优选配送 + 商家开票,不支持
                    case 4:
                        allow = false;
                        break;
                    //业务模式5：商家仓储泰隆优选配送 + 商家开票,支持
                    case 5:
                        allow = true;
                        break;
                    //业务模式6：商家仓储配送 + 商家开票,不支持 
                    case 6:
                        allow = false;
                        break;
                    //其他不存在的模式-1：不支持 
                    default:
                        allow = false;
                        break;
                }
            }
            else
            {
                allow = true;
            }
            return allow;
        }

        /// <summary>
        /// MKT Place II需求,是否允许添加商品款项
        /// </summary>
        /// <returns></returns>
        private bool IsAllowAddBasicItems()
        {
            bool allow = false;
            //MKT Place II需求，需判断业务模式。。。
            if (this.IsSupportMKTPLaceIIStockNo(this.soMaster.StockSysNo))  //上海仓
            {
                //业务模式1,2,3,即泰隆优选开票的：商品款项 + 其他费用项                
                if (this.soMaster.BussinessMode == 1
                    || this.soMaster.BussinessMode == 2
                    || this.soMaster.BussinessMode == 3)
                {
                    allow = true;
                }
            }
            else
            {
                allow = true;
            }
            return allow;
        }

        /// <summary>
        /// MKT Place II需求,是否允许添加其他费用款项
        /// </summary>
        /// <returns></returns>
        private bool IsAllowAddOtherFeesItems()
        {
            bool allow = false;
            //MKT Place II需求，需判断业务模式。。。
            if (this.IsSupportMKTPLaceIIStockNo(this.soMaster.StockSysNo))  //上海仓
            {
                //业务模式1，2，3，5的：可以添加其他费用项                
                if (this.soMaster.BussinessMode == 1
                    || this.soMaster.BussinessMode == 2
                    || this.soMaster.BussinessMode == 3
                    || this.soMaster.BussinessMode == 5)
                {
                    allow = true;
                }
            }
            else
            {
                allow = true;
            }
            return allow;
        }

        /// <summary>
        /// 根据分仓编号获取发票分页大小
        /// </summary>
        /// <param name="stockNo">分仓编号</param>
        /// <returns>发票分页大小</returns>
        private int GetInvoiceInfoPageSizeByStockNo(int stockNo)
        {
            //分页拆分的大小，目前先默认为上海仓的发票模板14，应根据各地分仓的模板大小设定，           
            int nSplitPageSize = 0;
            // 如果是成都仓，则12行商品编号就换页
            if (stockNo == 54)
            {
                nSplitPageSize = 12;
            }
            // 如果是北京仓，则10行商品编号就换页
            else if (stockNo == 52)
            {
                nSplitPageSize = 10;
            }
            // 其它仓，则14行商品编号就换页
            else
            {
                nSplitPageSize = 14;
            }
            return nSplitPageSize;
        }

        /// <summary>
        /// 计算发票明细的总金额
        /// </summary>
        /// <param name="invoiceInfo">发票</param>
        /// <returns></returns>
        private Decimal CalculateInvoiceItemSumAmt(InvoiceInfoReport invoiceInfo)
        {
            decimal resultSumAmt = 0;
            invoiceInfo.Items.ForEach(x =>
            {
                //判断不累加的Item(可得积分不累加)                
                if (x.ProductType != 4)
                {
                    resultSumAmt += x.SumExtendPrice;
                }
            });

            //累加小于0，则等于0
            if (resultSumAmt < 0)
            {
                resultSumAmt = 0m;
            }

            return resultSumAmt;
        }

        /// <summary>
        /// 获取万里通/中智积分抵扣的金额
        /// </summary>
        /// <returns></returns>
        private decimal GetPointPayDiscountAmt()
        {
            //直接获取发票总金额          
            decimal tempPrice = this.soMaster.InvoiceAmt;
            return tempPrice;
        }

        /// <summary>
        /// 获取人民币大写金额字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetRMBConvert(decimal value)
        {
            RMBCapitalization RMBConvert = new RMBCapitalization();
            string invoiceAmtConv = RMBConvert.RMBAmount(value);
            return invoiceAmtConv + string.Format("(￥{0})", value.ToString("#########0.00"));
        }

        /// <summary>
        ///  计算最终的应收款金额
        /// </summary>
        private decimal GetFinallReceivableAmt(decimal TotalAmount, decimal PrepayAmt, bool IsPayWhenRecv)
        {
            decimal amt = Math.Max(TotalAmount - (PrepayAmt != -999999 ? PrepayAmt : 0M), 0M);
            // 如果是货到付款，舍去分
            if (IsPayWhenRecv)
            {
                amt = RMBCapitalization.TruncMoney(amt);
            }
            return amt;
        }


        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.InvoiceReport, msgKeyName), args);
        }
        #endregion 

        #endregion
    }
}