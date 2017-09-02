using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Xml;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.EventConsumer;
using ECCentral.Service.EventMessage.WMS;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.EventMessage;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.EventConsumer.WMS;
using ECCentral.Service.EventMessage.PO;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 采购单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(PurchaseOrderProcessor))]
    public class PurchaseOrderProcessor
    {

        #region [Fields]
        private IPurchaseOrderDA m_PurchaseOrderDA;
        private ICommonBizInteract m_CommmonBizInteract;
        private IPurchaseOrderBasketDA m_BasketDA;
        private IVendorDA m_VendorDA;
        private IConsignSettlementDA m_ConsignSettlementDA;
        private IPurchaseOrderQueryDA m_PurchaseOrderQueryDA;
        private VendorProcessor m_VendorProcessor;
        private PurchaseOrderProcessor m_PurchaseOrderProcessor;
        private IInventoryBizInteract m_InventoryBizInteract;
        private IIMBizInteract m_IMBizInteract;

        public IIMBizInteract IMBizInteract
        {
            get
            {
                if (null == m_IMBizInteract)
                {
                    m_IMBizInteract = ObjectFactory<IIMBizInteract>.Instance;
                }
                return m_IMBizInteract;
            }
        }



        #endregion

        #region [Properties]

        public bool IsChangeTPStaus = false;

        public IPurchaseOrderDA PurchaseOrderDA
        {
            get
            {
                if (null == m_PurchaseOrderDA)
                {
                    m_PurchaseOrderDA = ObjectFactory<IPurchaseOrderDA>.Instance;
                }
                return m_PurchaseOrderDA;
            }
        }

        public ICommonBizInteract CommmonBizInteract
        {
            get
            {
                if (null == m_CommmonBizInteract)
                {
                    m_CommmonBizInteract = ObjectFactory<ICommonBizInteract>.Instance;
                }
                return m_CommmonBizInteract;
            }
        }

        public IPurchaseOrderBasketDA BasketDA
        {
            get
            {
                if (null == m_BasketDA)
                {
                    m_BasketDA = ObjectFactory<IPurchaseOrderBasketDA>.Instance;
                }
                return m_BasketDA;
            }
        }

        public IVendorDA VendorDA
        {
            get
            {
                if (null == m_VendorDA)
                {
                    m_VendorDA = ObjectFactory<IVendorDA>.Instance;
                }
                return m_VendorDA;
            }
        }

        public IConsignSettlementDA ConsignSettlementDA
        {
            get
            {
                if (null == m_ConsignSettlementDA)
                {
                    m_ConsignSettlementDA = ObjectFactory<IConsignSettlementDA>.Instance;
                }
                return m_ConsignSettlementDA;
            }
        }

        public IPurchaseOrderQueryDA PurchaseOrderQueryDA
        {
            get
            {
                if (null == m_PurchaseOrderQueryDA)
                {
                    m_PurchaseOrderQueryDA = ObjectFactory<IPurchaseOrderQueryDA>.Instance;
                }
                return m_PurchaseOrderQueryDA;
            }
        }

        public VendorProcessor VendorProcessor
        {
            get
            {
                if (null == m_VendorProcessor)
                {
                    m_VendorProcessor = ObjectFactory<VendorProcessor>.Instance;
                }
                return m_VendorProcessor;
            }

        }

        public PurchaseOrderProcessor PurchaseOrderProcess
        {
            get
            {
                if (null == m_PurchaseOrderProcessor)
                {
                    m_PurchaseOrderProcessor = ObjectFactory<PurchaseOrderProcessor>.Instance;
                }
                return m_PurchaseOrderProcessor;
            }
        }

        public IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                if (null == m_InventoryBizInteract)
                {
                    m_InventoryBizInteract = ObjectFactory<IInventoryBizInteract>.Instance;
                }
                return m_InventoryBizInteract;
            }
        }

        #endregion

        #region 产品线相关验证,如果验证通过，则返回PO单商品的产品线  CRL21776  by Jack.W.Wang 2012-11-9

        private string CheckPoPrductLine(PurchaseOrderInfo entity)
        {
            return CheckPoPrductLine(entity, null);
        }
        private string CheckPoPrductLine(PurchaseOrderInfo entity, List<int> addProductSysNoList)
        {
            int tNum = 0;
            return CheckPoPrductLine(entity, addProductSysNoList, out tNum);
        }
        private string CheckPoPrductLine(PurchaseOrderInfo entity, out int ProductLineSysNo)
        {
            return CheckPoPrductLine(entity, null, out ProductLineSysNo);
        }
        /// <summary>
        /// 产品线相关验证,如果验证通过，则返回PO单商品的产品线
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPoPrductLine(PurchaseOrderInfo entity, List<int> addProductSysNoList, out int ProductLineSysNo)
        {
            ProductLineSysNo = 0;
            /*CRL21776  2012-11-6  by Jack
             * 验证业务说明:
                *1.判断商品是否都有产品线
                *2.判断登陆PM是否拥有所有商品的产品线权限
                *3.判断一张订单上是否只有一条产品线
                *4.判断单据的产品线的OwnerPM是否为单据上的所属PM
             */

            //验证当前登陆PM是否有对item的操作权限
            //List<ProductPMLine> tPMLineList = ExternalDomainBroker.GetProductLineInfoByPM(ServiceContext.Current.UserSysNo);
            //bool tIsManager = entity.PurchaseOrderBasicInfo.IsManagerPM ?? false;
            //if (tPMLineList.Count > 0 || tIsManager)
            //{
            //    List<int> tProList = entity.POItems.Select(x => x.ProductSysNo.Value).ToList<int>();
            //    if (addProductSysNoList != null && addProductSysNoList.Count != 0)
            //        addProductSysNoList.ForEach(x => tProList.Add(x));
            //    List<ProductPMLine> tList = ExternalDomainBroker.GetProductLineSysNoByProductList(tProList.ToArray());
            //    string tErrorMsg = string.Empty;
            //    //检测没有产品线的商品
            //    tList.ForEach(x =>
            //    {
            //        if (x.ProductLineSysNo == null)
            //            tErrorMsg += x.ProductID + Environment.NewLine;
            //    });
            //    if (!tErrorMsg.Equals(string.Empty))
            //    {
            //        return GetMessageString("PO_CheckMsg_NoProductLine") + Environment.NewLine + tErrorMsg;
            //    }
            //    //检测当前登陆PM对ItemList中商品是否有权限
            //    if (!tIsManager)
            //        tList.ForEach(x =>
            //        {
            //            if (tPMLineList.SingleOrDefault(item => item.ProductLineSysNo == x.ProductLineSysNo) == null)
            //                tErrorMsg += x.ProductID + Environment.NewLine;
            //        });
            //    if (!tErrorMsg.Equals(string.Empty))
            //    {
            //        return GetMessageString("PO_CheckMsg_NoProductAccess") + Environment.NewLine + tErrorMsg;
            //    }
            //    //验证ItemList中产品线是否唯一
            //    if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count != 1)
            //    {
            //        return GetMessageString("PO_CheckMsg_NotSamePL");
            //    }
            //    if ((entity.PurchaseOrderBasicInfo.ProductManager.SysNo == null) || (entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value != tList[0].PMSysNo.Value))
            //    {
            //        //注释原因：因为PO单 可能 存在返点PM验证，所以不能自动更新PM
            //        //需要根据商品的产品线加载PO单的所属PM   
            //        //此处IsAutoFillPM标志，用于后台创建PO单 使用，暂时应用于补充创建po单
            //        //entity.PurchaseOrderBasicInfo.ProductManager.SysNo = tList[0].PMSysNo;  
            //        if (entity.PurchaseOrderBasicInfo.IsAutoFillPM.HasValue && entity.PurchaseOrderBasicInfo.IsAutoFillPM.Value)
            //            entity.PurchaseOrderBasicInfo.ProductManager.SysNo = tList[0].PMSysNo;
            //        else
            //            return GetMessageString("PO_CheckMsg_NotOwnerPM");
            //    }
            //    //回写返回值
            //    ProductLineSysNo = tList[0].ProductLineSysNo.Value;
            //}
            //else
            //{
            //    return GetMessageString("PO_CheckMsg_NotOperationPL");
            //}

            return string.Empty;
        }

        #endregion

        /// <summary>
        /// 创建PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo CreatePO(PurchaseOrderInfo poInfo)
        {
            //获取ExchangeRate:
            poInfo.PurchaseOrderBasicInfo.ExchangeRate = ExternalDomainBroker.GetExchangeRateBySysNo(poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value, poInfo.CompanyCode);
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Origin;
            List<int> privilege = new List<int>();
            string exceptionInfo;
            #region [创建时,检查PO单实体]

            //产品线相关验证,如果验证通过，则回写PO单产品线SysNo
            // edit by jason [OYSD]bug1037 
            //int tPLineSysNo = 0;
            //exceptionInfo = CheckPoPrductLine(poInfo, out tPLineSysNo);
            //if (!string.IsNullOrEmpty(exceptionInfo))
            //    throw new BizException(exceptionInfo);
            //else if (tPLineSysNo != 0)
            //    poInfo.PurchaseOrderBasicInfo.ProductLineSysNo = tPLineSysNo;

            exceptionInfo = PreCheckCreatePO(poInfo);
            if (exceptionInfo.Length > 4)
            {
                throw new BizException(exceptionInfo);
            }
            #endregion [创建时,检查PO单实体]

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //设置初始化值:
                poInfo.SysNo = PurchaseOrderDA.CreatePOSequenceSysNo();
                poInfo.PurchaseOrderBasicInfo.TotalAmt = CaclTotalAmt(poInfo.POItems);
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderID = poInfo.SysNo.Value.ToString();
                poInfo.PurchaseOrderBasicInfo.CreateDate = System.DateTime.Now;
                poInfo.PurchaseOrderBasicInfo.IsApportion = 0;
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderLeaseFlag = poInfo.PurchaseOrderBasicInfo.PurchaseOrderLeaseFlag == null ? PurchaseOrderLeaseFlag.unLease : poInfo.PurchaseOrderBasicInfo.PurchaseOrderLeaseFlag;
                //转租赁的单据创建时为已入库
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderLeaseFlag != PurchaseOrderLeaseFlag.Lease)
                {
                    poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
                }
                else
                {
                    poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.InStocked;
                }

                //创建操作:
                PurchaseOrderDA.CreatePO(poInfo);
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    if (item.Quantity != 0)
                    {
                        item.Quantity = 0;
                    }
                    item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + poInfo.PurchaseOrderBasicInfo.TaxRate.Value), 2);

                    //针对采购篮的操作
                    if (item.ItemSysNo.HasValue)
                    {
                        if (item.ItemSysNo.Value < 0)
                        {
                            BasketDA.DeleteBasket(item.ItemSysNo.Value * -1);
                        }
                        else
                        {
                            BasketDA.DeleteBasket(item.ItemSysNo.Value);
                        }
                    }
                    item.POSysNo = poInfo.SysNo;
                    //创建PO Item:
                    PurchaseOrderDA.CreatePOItem(item);
                }

                //创建多返点信息
                if (null != poInfo.EIMSInfo && null != poInfo.EIMSInfo.EIMSInfoList)
                {
                    foreach (EIMSInfo item in poInfo.EIMSInfo.EIMSInfoList)
                    {
                        item.PurchaseOrderSysNo = poInfo.SysNo.Value;
                        PurchaseOrderDA.CreatePOEIMSInfo(item);
                    }
                }

                //写LOG 日志记录
                //CommonService.WriteLog<POEntity>(entity, " Created PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Create);
                ExternalDomainBroker.CreateLog(" Created PO "
               , BizEntity.Common.BizLogType.Purchase_Create
               , poInfo.SysNo.Value
               , poInfo.CompanyCode);

                scope.Complete();
            }

            return poInfo;
        }

        /// <summary>
        /// 补充创建PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo RenewCreatePO(PurchaseOrderInfo poInfo)
        {
            #region [Check操作]

            //1.检查当前PO单是否已经全部入库，不能补充创建PO单
            List<PurchaseOrderItemInfo> getItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            List<PurchaseOrderItemInfo> checkItems = new List<PurchaseOrderItemInfo>();
            if (null != getItems)
            {
                foreach (PurchaseOrderItemInfo item in getItems)
                {
                    item.PurchaseQty = item.PurchaseQty - item.Quantity;
                    item.Quantity = 0;
                    if (item.PurchaseQty != 0)
                    {
                        checkItems.Add(item);
                    }
                }

                if (checkItems.Count == 0)
                {
                    throw new BizException(GetMessageString("PO_Create_RecreateInvalid"));
                }
            }
            //2.检查EIMS信息和金额:
            List<EIMSInfo> getPOEIMSInfo = PurchaseOrderDA.GetEIMSInvoiceInfoByPOSysNo(poInfo.SysNo.Value);
            if (null != getPOEIMSInfo)
            {
                foreach (EIMSInfo eimsInfo in getPOEIMSInfo)
                {
                    EIMSInfo getAvailableEIMS = PurchaseOrderDA.GetAvilableEimsAmtInfo(eimsInfo.EIMSNo.Value);
                    if (null == getAvailableEIMS)
                    {
                        throw new BizException(string.Format(GetMessageString("PO_EIMSNotFound"), eimsInfo.EIMSSysNo.Value));
                    }
                    if (eimsInfo.LeftAmt > getAvailableEIMS.EIMSLeftAmt)
                    {
                        throw new BizException(GetMessageString("PO_Create_EIMSAmt_Failed"));
                    }
                }
            }

            #endregion [Check操作]

            #region [Create操作]

            List<PurchaseOrderItemInfo> items = new List<PurchaseOrderItemInfo>();

            PurchaseOrderInfo newPOInfo = new PurchaseOrderInfo();

            newPOInfo = LoadPO(poInfo.SysNo.Value);
            if (newPOInfo != null)
            {
                //crl21776
                newPOInfo.PurchaseOrderBasicInfo.IsManagerPM = poInfo.PurchaseOrderBasicInfo.IsManagerPM;
                newPOInfo.PurchaseOrderBasicInfo.IsAutoFillPM = poInfo.PurchaseOrderBasicInfo.IsAutoFillPM;

                newPOInfo = GetReturnPoint(newPOInfo);
                newPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
                newPOInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime = null;
                newPOInfo.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay = null;
                newPOInfo.PurchaseOrderBasicInfo.PM_ReturnPointSysNo = null;
                newPOInfo.PurchaseOrderBasicInfo.UsingReturnPoint = null;
                newPOInfo.PurchaseOrderBasicInfo.ReturnPointC3SysNo = null;
                newPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress = null;
                newPOInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo;
                //if (!string.IsNullOrEmpty(newPOInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo) || newPOInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo.Length > 0)
                //{
                //    newPOInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo += string.Format("{1}:[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CommmonBizInteract.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true));
                //}


                items = PurchaseOrderDA.LoadPOItems(newPOInfo.SysNo.Value);

                List<PurchaseOrderItemInfo> itemsList = new List<PurchaseOrderItemInfo>();
                foreach (PurchaseOrderItemInfo pi in items)
                {
                    pi.PurchaseQty = pi.PurchaseQty - pi.Quantity;
                    pi.Quantity = 0;
                    if (pi.PurchaseQty != 0)
                    {
                        itemsList.Add(pi);
                    }
                }
                items = itemsList;

                decimal ActualPriceAmt = 0m;
                newPOInfo.PurchaseOrderBasicInfo.TotalActualPrice = ActualPriceAmt;

                #region 多返点部分

                List<EIMSInfo> poEimsEntitys = PurchaseOrderDA.GetEIMSInvoiceInfoByPOSysNo(poInfo.SysNo.Value).Where(ite => ite.LeftAmt > 0).ToList();
                foreach (EIMSInfo eimsEntity in poEimsEntitys)
                {
                    //////////////////////////////设置EIMS信息///////////
                    EIMSInfo avilableEims = PurchaseOrderDA.GetAvilableEimsAmtInfo(eimsEntity.EIMSNo.Value);
                    if (eimsEntity.LeftAmt > avilableEims.EIMSLeftAmt)
                    {
                        throw new BizException(GetMessageString("PO_Create_EIMSAmt_Failed"));
                    }
                }

                #endregion 多返点部分

                newPOInfo.EIMSInfo = new PurchaseOrderEIMSInfo()
                {
                    EIMSInfoList = new List<EIMSInfo>()
                };
                newPOInfo.EIMSInfo.EIMSInfoList = poEimsEntitys;
                return CreatePO(newPOInfo);
            }
            else
            {
                return null;
            }

            #endregion [Create操作]
        }

        /// <summary>
        /// 更新PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo UpdatePO(PurchaseOrderInfo poInfo)
        {
            #region [Check实体]

            //2012-9-13  Jack
            CheckReturnPointAndPrice(poInfo);

            string exceptionInfo = PreCheckCreatePO(poInfo);
            if (!string.IsNullOrEmpty(exceptionInfo))
            {
                throw new BizException(exceptionInfo);
            }

            //产品线相关验证,如果验证通过，则回写PO单产品线SysNo
            int tPLineSysNo = 0;
            exceptionInfo = CheckPoPrductLine(poInfo, out tPLineSysNo);
            if (!string.IsNullOrEmpty(exceptionInfo))
                throw new BizException(exceptionInfo);
            else if (tPLineSysNo != 0)
                poInfo.PurchaseOrderBasicInfo.ProductLineSysNo = tPLineSysNo;

            #endregion [Check实体]

            if (!string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo))
            {
                string[] strs = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo.Split(new char[] { '[' });
                if (strs.Length > 0)
                {
                    if (strs[strs.Length - 1].LastIndexOf(']') == strs[strs.Length - 1].Length - 1)
                    {
                        poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo += "|";
                        poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo.Replace("[" + strs[strs.Length - 1] + "|", "");
                    }
                }
                else
                {
                    poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo += "[" + ExternalDomainBroker.GetUserNameByUserSysNo(poInfo.PurchaseOrderBasicInfo.CreateUserSysNo.Value) + ":" + DateTime.Now.ToString() + "]";
                }
            }
            //调用Common接口获取 ExchangeRate:
            poInfo.PurchaseOrderBasicInfo.ExchangeRate = ExternalDomainBroker.GetExchangeRateBySysNo(poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value, poInfo.CompanyCode);

            SetDataEntyType(poInfo);
            List<PurchaseOrderItemInfo> itemEntity = poInfo.POItems;
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            PurchaseOrderInfo localEntity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
            if (localEntity == null)
            {
                throw new BizException(GetMessageString("PO_PONotFound"));
            }

            //CRL17821 对于已经发送过创建消息的采购单(检查POSSB_Log)，在更新时不允许更新入库仓
            if (!CheckStockSysNo(poInfo, localEntity))
            {
                throw new BizException(GetMessageString("PO_WMS_UpdateFailed"));
            }

            //必须是初始状态
            if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Created && localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Returned)
            {
                throw new BizException(GetMessageString("PO_Operate_Failed"));
            }

            // 系统已经关闭调价单功能,请使用'采购单扣减'来处理此EIMS单据!
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Adjust)
            {
                throw new BizException(GetMessageString("PO_Update_TypeError1"));
            }

            //// 系统已经关闭历史采购单功能!
            //if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.HistoryNegative)
            //{
            //    throw new BizException(GetMessageString("PO_Update_TypeError2"));
            //}

            //// 系统已经关闭临时代销功能!
            //if (poInfo.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.TempConsign)
            //{
            //    throw new BizException(GetMessageString("PO_Update_TypeError3"));
            //}

            #region PO自动Email 检查

            //邮件总共超过500字符 不保存 改为记日志  否则更新 POMaster AutoSendMail字段（追加）
            localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress = localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress == null ? "" : localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress.Replace(Environment.NewLine, "");
            //清空 AutoSenMail:
            if (poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress != "-999" && poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress != "")
            {
                string[] autoSendEnailArray = localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress.Split(';');
                for (int i = 0; i < autoSendEnailArray.Length; i++)
                {
                    if (localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress != "" && localEntity.PurchaseOrderBasicInfo.AutoSendMailAddress.Contains(autoSendEnailArray[i].Replace(" ", "")))//判断重复
                    {
                        autoSendEnailArray[i] = autoSendEnailArray[i].Replace(autoSendEnailArray[i], "");//剔除重复
                    }
                }
                string INSendMail = "";
                for (int i = 0; i < autoSendEnailArray.Length; i++)
                {
                    if (autoSendEnailArray[i] != "")
                    {
                        INSendMail += autoSendEnailArray[i].Replace(" ", "") + ";";
                    }
                }
                INSendMail = INSendMail.Substring(0, INSendMail.Length - 1 < 0 ? 0 : INSendMail.Length - 1);//生成非重复邮件
                //大于500个收件人地址，则提示自动收件人过多.
                if (INSendMail.Length > 500)
                {
                    throw new BizException(GetMessageString("PO_AutoMailAddess_MaxCount"));
                }
            }

            #endregion PO自动Email 检查

            //负采购时Item数量不能大于分仓对应的可用库存加代销库存数量！
            CheckNegativePoItemNumber(poInfo);

            //update by kathy 20100618 解决死锁问题
            foreach (PurchaseOrderItemInfo item in poInfo.POItems)
            {
                //如果是负采购 批量更新负采购成本
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                {
                    //调用IM接口，获取商品的UnitCost:
                    item.ReturnCost = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductPriceInfo.UnitCost;
                }
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                #region PO自动Email

                if (poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress == "-999")//清空 AutoSenMail
                {
                    poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress = "";
                    UpdatePOAutoAutoSendMail(poInfo, false);
                }
                else
                {
                    if (poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress.Length > 500)
                    {
                        //记日志保存
                        throw new BizException(GetMessageString("PO_AutoMailAddess_MaxCount"));
                    }
                    else
                    {
                        string InDBEmail = string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress) ? null : poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress;
                        if (InDBEmail != "")
                        {
                            UpdatePOAutoAutoSendMail(poInfo, false);
                        }
                    }
                }

                #endregion PO自动Email

                poInfo.PurchaseOrderBasicInfo.TotalAmt = null;
                PurchaseOrderDA.UpdatePOMaster(poInfo);

                //清空返点信息
                PurchaseOrderDA.DeletePOEIMSInfo(poInfo.SysNo.Value);
                //创建多返点信息
                foreach (EIMSInfo item in poInfo.EIMSInfo.EIMSInfoList)
                {
                    item.PurchaseOrderSysNo = poInfo.SysNo.Value;
                    PurchaseOrderDA.CreatePOEIMSInfo(item);
                }

                //写LOG:
                //CommonService.WriteLog<POEntity>(entity, " updated PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Master_Update);

                ExternalDomainBroker.CreateLog(" updated PO "
               , BizEntity.Common.BizLogType.Purchase_Master_Update
               , poInfo.SysNo.Value
               , poInfo.CompanyCode);

                scope.Complete();
            }

            return poInfo;
        }

        /// <summary>
        /// 检查PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo CheckPO(PurchaseOrderInfo entity)
        {
            PurchaseOrderInfo localpo = entity;
            entity = PurchaseOrderDA.LoadPOMaster(entity.SysNo.Value);
            entity.VendorInfo = VendorDA.LoadVendorInfo(entity.VendorInfo.SysNo.Value);
            entity.PurchaseOrderBasicInfo.Privilege = localpo.PurchaseOrderBasicInfo.Privilege;
            entity.PurchaseOrderBasicInfo.AuditUserSysNo = localpo.PurchaseOrderBasicInfo.AuditUserSysNo;
            entity.POItems = PurchaseOrderDA.LoadPOItems(entity.SysNo.Value);
            entity.EIMSInfo = new PurchaseOrderEIMSInfo()
            {
                EIMSInfoList = new List<EIMSInfo>()
            };
            entity.EIMSInfo.EIMSInfoList = PurchaseOrderDA.LoadPOEIMSInfo(entity.SysNo.Value);
            if (!(entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed))
            {
                //检查PO：
                string checkResult = PreCheckPoCheck(entity);
                entity.PurchaseOrderBasicInfo.CheckResult = checkResult;
                PurchaseOrderDA.UpdateCheckResult(entity);
            }

            return entity;
        }

        /// <summary>
        /// 确认PO单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo WaitingInStockPO(PurchaseOrderInfo poInfo, bool needCheckStatus)
        {

            PurchaseOrderInfo localEntity = poInfo;
            //获取PO单主信息:
            poInfo = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
            //获取PO单商品信息:
            poInfo.POItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            //获取PO单EIMS信息:
            if (null == poInfo.EIMSInfo)
            {
                poInfo.EIMSInfo = new PurchaseOrderEIMSInfo();
            }
            poInfo.PurchaseOrderBasicInfo.AuditUserSysNo = ServiceContext.Current.UserSysNo;
            poInfo.EIMSInfo.EIMSInfoList = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);
            poInfo.PurchaseOrderBasicInfo.Privilege = localEntity.PurchaseOrderBasicInfo.Privilege;

            #region Check逻辑
            if (needCheckStatus)
            {
                PreCheckPOStatusWhenAudit(poInfo);
            }
            PreCheckPreviewAndPM(poInfo);
            string exceptionInfo = PreCheckWaitingInStock(poInfo);

            #endregion Check逻辑

            #region 获得用户所有权限
            //获得用户所有权限
            if (!string.IsNullOrEmpty(exceptionInfo))
            {
                if (IsChangeTPStaus == true)
                {
                    if (CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditGeneric, poInfo) && poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus == 1)
                    {
                        poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = 2;
                        poInfo.PurchaseOrderBasicInfo.ApportionTime = DateTime.Now;
                        poInfo.PurchaseOrderBasicInfo.ApportionUserSysNo = poInfo.PurchaseOrderBasicInfo.AuditUserSysNo;
                        //写LOG：CommonService.WriteLog<POEntity>(entity, "权限不足由待TL审核调整为待PMD审核 ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);
                        ExternalDomainBroker.CreateLog(GetMessageString("PO_UpdateMemo_Audit1")
                          , BizEntity.Common.BizLogType.Purchase_Verify_InStock
                          , poInfo.SysNo.Value
                          , poInfo.CompanyCode);
                    }
                    else if (CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditLow, poInfo) && (poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus == null || !poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus.HasValue))
                    {
                        poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = 1;
                        //写LOG：CommonService.WriteLog<POEntity>(entity, "权限不足由待审核调整为待TL审核 ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

                        ExternalDomainBroker.CreateLog(GetMessageString("PO_UpdateMemo_Audit2")
                   , BizEntity.Common.BizLogType.Purchase_Verify_InStock
                   , poInfo.SysNo.Value
                   , poInfo.CompanyCode);
                    }

                    //更新TP Status:
                    PurchaseOrderDA.UpdatePOTPStatus(poInfo);

                    //发送ESB消息
                    EventPublisher.Publish<PurchaseOrderConfirmMessage>(new PurchaseOrderConfirmMessage()
                    {
                        ConfirmUserSysNo = ServiceContext.Current.UserSysNo,
                        SysNo = poInfo.SysNo.Value
                    });
                }
                throw new BizException(exceptionInfo);
            }
            #endregion

            poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime;
            poInfo.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay;
            bool notIsPMD = false;
            if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditAll, poInfo))
            {
                notIsPMD = true;
            }

            #region 审核通过对数据进行操作

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus;
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.WaitingInStock;

                if (notIsPMD)
                {
                    poInfo.PurchaseOrderBasicInfo.ApportionTime = DateTime.Now;
                    poInfo.PurchaseOrderBasicInfo.ApportionUserSysNo = poInfo.PurchaseOrderBasicInfo.AuditUserSysNo;
                }
                ////创建人、审核人、终审人不能相同
                //if (poInfo.PurchaseOrderBasicInfo.CreateUserSysNo == poInfo.PurchaseOrderBasicInfo.AuditUserSysNo || poInfo.PurchaseOrderBasicInfo.CreateUserSysNo == poInfo.PurchaseOrderBasicInfo.ApportionUserSysNo
                // || (!notIsPMD && poInfo.PurchaseOrderBasicInfo.ApportionUserSysNo == poInfo.PurchaseOrderBasicInfo.AuditUserSysNo))
                //{
                //    throw new BizException(GetMessageString("PO_CreateAndAuditUserNotTheSame"));
                //}
                poInfo.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo = null;
                poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = localEntity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo;

                List<PurchaseOrderItemInfo> items = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);

                //抵扣返点金额更新成本
                if (poInfo.PurchaseOrderBasicInfo.UsingReturnPoint.HasValue)
                {
                    decimal totalAmt = CaclTotalAmt(items);
                    if (totalAmt <= poInfo.PurchaseOrderBasicInfo.UsingReturnPoint)
                    {
                        poInfo.PurchaseOrderBasicInfo.UsingReturnPoint = totalAmt;
                    }
                }

                int row = PurchaseOrderDA.WaitingInStockPO(poInfo);
                if (row != 1)
                {
                    //修改失败
                    throw new BizException(GetMessageString("PO_Update_Failed"));
                }

                List<KeyValuePair<int, int>> kv = new List<KeyValuePair<int, int>>();

                #region

                foreach (PurchaseOrderItemInfo item in items)
                {
                    kv.Add(new KeyValuePair<int, int>(item.ProductSysNo.Value, item.PurchaseQty.Value));

                    //当时京东价格 总仓有效库存 上月销售总量
                    PurchaseOrderItemInfo tempPoItem = PurchaseOrderDA.LoadExtendPOItem(item.ProductSysNo.Value);
                    item.JingDongPrice = tempPoItem.JingDongPrice;
                    item.M1 = tempPoItem.M1;
                    item.AvailableQty = tempPoItem.AvailableQty;
                    item.UnitCostWithoutTax = item.UnitCostWithoutTax ?? 0;
                    item.CurrentUnitCost = tempPoItem.CurrentUnitCost;
                    item.CurrentPrice = tempPoItem.CurrentPrice;
                    item.LastInTime = tempPoItem.LastInTime;
                    item.LastAdjustPriceDate = tempPoItem.LastAdjustPriceDate;
                    item.LastOrderPrice = tempPoItem.LastOrderPrice;
                    PurchaseOrderDA.UpdatePOItem(item);
                }

                #endregion 审核通过对数据进行操作

                //如果有返点，进行返点的扣除和计算
                //List<POAttachInfo> poAttachEntitys = new List<POAttachInfo>();
                List<EIMSInfo> poEimsEntitys = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);
                if (poEimsEntitys != null && poEimsEntitys.Count > 0)
                {
                    foreach (EIMSInfo poEims in poEimsEntitys)
                    {
                        //POAttachInfo poAttachInfo = new POAttachInfo()
                        //{
                        //    PONumber = entity.SysNo.Value,
                        //    InvoiceNumber = poEims.EIMSNo,
                        //    UseInvoiceAmount = poEims.EIMSAmt,
                        //    UseTime = System.DateTime.Now,
                        //    PostTime = System.DateTime.Now,
                        //    POStatus = (int)localEntity.Status.Value,
                        //    PM = entity.PMSysNo.Value.ToString(),
                        //    C3SysNo = -1
                        //};
                        //poAttachEntitys.Add(poAttachInfo);
                    }

                    //TODO:调用EIMS接口，进行返点的扣除和计算:
                    //EIMSMgmtService.SetPMRemnantReturnPoint(poAttachEntitys, entity.AuditUserSysNo.Value.ToString());
                }

                //新进商品初始化
                //设置采购在途数量,（代销PO该业务逻辑不变）
                //调用Inventory接口:
                InventoryAdjustContractInfo contractInfo = new InventoryAdjustContractInfo()
                {
                    ReferenceSysNo = poInfo.SysNo.Value.ToString(),
                    SourceActionName = InventoryAdjustSourceAction.Audit,
                    SourceBizFunctionName = (InventoryAdjustSourceBizFunction.PO_Order)
                };

                contractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                kv.ForEach(x =>
                {
                    contractInfo.AdjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.Key,
                        StockSysNo = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value,
                        AdjustQuantity = x.Value
                    });
                });

                ExternalDomainBroker.AdjustProductInventory(contractInfo);

                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    //****TODO:调用IM接口，更新价格:ContentMgmtService.UpdataVailaPrice(item, header);
                    //ObjectFactory<IIMBizInteract>.Instance.UpdateProductPriceForOther(new ProductPriceInfo()
                    //{
                    //    SysNo = item.ProductSysNo,
                    //    VirtualPrice = item.VirtualPrice.HasValue ? item.VirtualPrice.Value : 0m
                    //});
                }
                //如果是负采购单:
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                {
                    poInfo.POItems = items;
                    SetInvotryInfo(poInfo, "");
                }

                //写LOG：CommonService.WriteLog<POEntity>(entity, " Verify PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

                ExternalDomainBroker.CreateLog(" Verify PO "
               , BizEntity.Common.BizLogType.Commission_CloseCommission
               , poInfo.SysNo.Value
               , poInfo.CompanyCode);

                scope.Complete();
            }

            if (IsSSBEnabled())
            {
                #region 发送SSB消息

                string messageType;
                PurchaseOrderSSBMsgType actionType;

                if (PurchaseOrderDA.LoadPOSSBLog(poInfo.SysNo.Value, PurchaseOrderSSBMsgType.I).Count == 0)
                {
                    messageType = "POCreate";
                    actionType = PurchaseOrderSSBMsgType.I;
                }
                else
                {
                    messageType = "POUpdate";
                    actionType = PurchaseOrderSSBMsgType.U;
                }
                //加载PO单Items:
                var poInfoList = PurchaseOrderDA.LoadPODetialInfo(poInfo.SysNo.Value);

                if (poInfoList.Count <= 0)
                {
                    throw new Exception(string.Format(GetMessageString("PO_WaitingInStock_SeachPOFailed"), poInfo.SysNo.Value));
                }

                var first = poInfoList.First();
                //获取商品附件格式字符串:
                var accessoriesInfo = PurchaseOrderDA.GetItemAccessoriesStringByPurchaseOrder(poInfoList.Select(item => item.ProductSysNo).ToList(), poInfo.CompanyCode);

                PurchaseOrderInfo poMaster = PurchaseOrderDA.LoadPOMaster(first.PONumber.Value);

                string memo = string.Empty;
                //如果是经中转仓的,则将中转仓信息拼到Memo中
                if (first.ITStockSysNo.HasValue)
                {
                    var stockName = ExternalDomainBroker.GetWarehouseInfo(first.ITStockSysNo.Value).WarehouseName;

                    memo = string.Format("{0}(移入仓库:{1})", first.Memo, stockName);
                }
                else
                {
                    memo = first.Memo;
                }

                PurchaseOrderWaitingInStockMessage msg = new PurchaseOrderWaitingInStockMessage()
                {
                    SendType = messageType,
                    PONumber = first.PONumber.Value.ToString(),
                    CompanyCode = poInfo.CompanyCode,
                    VendorSysNo = first.VendorNumber.ToString(),
                    WarehouseNumber = first.WarehouseNumber.Value.ToString(),
                    AccessoriesInfo = accessoriesInfo,
                    POType = first.POType,
                    PMContact = first.PONumber.HasValue ? PurchaseOrderDA.GetPhoneNumberByPOSysNo(first.PONumber.Value) : "",
                    ETATime = poMaster.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? poMaster.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString() : string.Empty,
                    ETAHalfDay = poMaster.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.ToString(),
                    Memo = memo
                };
                msg.POItems = new List<POItemInfoMessage>();

                foreach (var item in poInfoList)
                {
                    POItemInfoMessage itemMsg = new POItemInfoMessage()
                    {
                        ProductSysNo = item.ProductSysNo.Value.ToString(),
                        ItemNumber = item.ItemNumber.ToString(),
                        ProductName = item.ProductName.ToString(),
                        PurchaseQty = item.PurchaseQty.Value.ToString(),
                        Price = item.Price.ToString(),
                        Weight = item.Weight.Value.ToString(),
                        BatchInfo = string.IsNullOrEmpty(item.BatchInfo) ? "" : item.BatchInfo.ToString(),

                    };
                    msg.POItems.Add(itemMsg);
                }
                EventPublisher.Publish<PurchaseOrderWaitingInStockMessage>(msg);
                #endregion 发送SSB消息

                //创建SSB LOG 日志:
                PurchaseOrderDA.CreatePOSSBLog(new PurchaseOrderSSBLogInfo
                {
                    POSysNo = poInfo.SysNo.Value,
                    Content = msg.ToXmlString(),
                    ActionType = actionType.ToString(),
                    InUser = 1,
                    SendErrMail = "N"
                }, poInfo.CompanyCode);
            }
            #endregion

            #region PO审核读取自动发送邮件通过发送邮件
            if (!string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress))
            {
                var variables = new KeyValueVariables();
                var tableVariables = new KeyTableVariables();
                var CommonBizInteract = ObjectFactory<ICommonBizInteract>.Instance;

                #region [构建供应商和采购单的基本信息]
                string getPrintTitle = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.Consign)
                {
                    getPrintTitle = string.Format("采购单({0})", EnumHelper.GetDisplayText(poInfo.PurchaseOrderBasicInfo.ConsignFlag));
                }
                else
                {
                    if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType.HasValue && poInfo.PurchaseOrderBasicInfo.PurchaseOrderType != PurchaseOrderType.Normal)
                    {
                        getPrintTitle = string.Format("采购单({0})", EnumHelper.GetDisplayText(poInfo.PurchaseOrderBasicInfo.PurchaseOrderType));
                    }
                    else
                    {
                        getPrintTitle = "采购单";
                    }
                }
                variables.Add("PrintTitle", getPrintTitle);

                WarehouseInfo getStockInfo = InventoryBizInteract.GetWarehouseInfoBySysNo(poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value);
                string getStockName = getStockInfo.WarehouseName;
                string CompanyName = "网信（香港）有限公司";
                string CompanyAddress = "香港九龙湾启祥道20号大昌行集团大厦8楼";
                string ComapnyTel = "（852）27683388";
                string CompanyWebsite = "";
                string StockAddress = getStockInfo.ReceiveAddress;
                string StockContact = getStockInfo.ReceiveContact;
                string StockTel = getStockInfo.ReceiveContactPhoneNumber;
                string ETATime = poInfo.PurchaseOrderBasicInfo.ETATimeInfo == null || !poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? string.Empty : poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                switch (poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo)
                {
                    case 51:
                        //上海
                        break;
                    case 52:
                        //香港:
                        CompanyAddress = "香港九龙湾启祥道20号大昌行集团大厦8楼";
                        ComapnyTel = "（852）27683388";
                        break;
                    case 53:
                        //日本:
                        break;
                    default:
                        break;
                }
                variables.Add("StockName", getStockName);
                variables.Add("ETATime", ETATime);
                variables.Add("CompanyName", CompanyName);
                variables.Add("CompanyAddress", CompanyAddress);
                variables.Add("CompanyTel", ComapnyTel);
                variables.Add("CompanyWebSite", CompanyWebsite);

                string getSerialNumber = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    getSerialNumber = CommonBizInteract.GetSystemConfigurationValue("PONumber", poInfo.CompanyCode);
                    if (!string.IsNullOrEmpty(getSerialNumber))
                    {
                        //TODO:此方法尚未实现:
                        CommonBizInteract.UpdateSystemConfigurationValue("PONumber", (Convert.ToInt32(getSerialNumber) + 1).ToString(), poInfo.CompanyCode);
                    }
                    string getOldSerialNumber = GetWareHouseReceiptSerialNumber(poInfo.SysNo.Value);
                    //更新PO单当前的流水号:
                    UpdateWareHouseReceiptSerialNumber(poInfo.SysNo.Value, getSerialNumber);
                    if (!string.IsNullOrEmpty(getOldSerialNumber))
                    {
                        getSerialNumber = string.Format("{0}({1})", getSerialNumber, getOldSerialNumber);
                    }
                }
                else
                {
                    getSerialNumber = "无";
                }

                variables.Add("PrintSerialNumber", getSerialNumber);
                variables.Add("PurchaseOrderID", poInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
                variables.Add("PrintTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                variables.Add("VendorNameAndSysNo", string.Format("{0}({1})", poInfo.VendorInfo.VendorBasicInfo.VendorNameLocal, poInfo.VendorInfo.SysNo.Value));
                variables.Add("VendorAddress", poInfo.VendorInfo.VendorBasicInfo.Address);
                variables.Add("StockAddress", StockAddress);
                variables.Add("StockContact", StockContact);
                variables.Add("StockTel", StockTel);

                string getInStockDate = string.Empty;
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    getInStockDate = poInfo.PurchaseOrderBasicInfo.InTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                variables.Add("InStockDate", getInStockDate);
                variables.Add("CreateUserName", CommonBizInteract.GetUserFullName(poInfo.PurchaseOrderBasicInfo.CreateUserSysNo.Value.ToString(), true));
                variables.Add("PayTypeName", poInfo.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName);
                variables.Add("VendorContact", poInfo.VendorInfo.VendorBasicInfo.Contact);
                variables.Add("VendorPhoneAndFax", poInfo.VendorInfo.VendorBasicInfo.Phone + " FAX : " + poInfo.VendorInfo.VendorBasicInfo.Fax);
                variables.Add("ShipTypeName", poInfo.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName);
                variables.Add("CurrencyName", poInfo.PurchaseOrderBasicInfo.CurrencyName);
                variables.Add("Memo", poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
                variables.Add("InStockMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);

                string getTotalAmt = string.Empty;
                string getTotalReturnPoint = string.Empty;
                decimal? eimsAmt = 0.00m;
                decimal? totalInPage = 0;
                poInfo.POItems.ForEach(x =>
                {
                    totalInPage += x.PurchaseQty * x.OrderPrice;

                });
                poInfo.EIMSInfo.EIMSInfoList.ForEach(x =>
                {
                    eimsAmt += x.EIMSAmt.HasValue ? x.EIMSAmt.Value : 0m;
                });
                if (eimsAmt > 0.00m)
                {
                    getTotalAmt = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(totalInPage - eimsAmt).ToString("f2");
                    getTotalReturnPoint = "产品总价：" + poInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPage.Value.ToString("f2") + " &nbsp;&nbsp; " + "使用返点：" + eimsAmt.Value.ToString("f2");
                }
                else
                {
                    getTotalAmt = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPage.Value.ToString("f2");
                    getTotalReturnPoint = "";
                }

                variables.Add("TotalReturnPoint", getTotalReturnPoint);
                variables.Add("TotalAmt", getTotalAmt);
                #endregion

                #region [构建商品列表信息]

                bool needShowInStockQty = ((poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed) == true ? true : false);
                variables.Add("NeedShowInStockQty", needShowInStockQty);

                int purchaseQtyTotal = 0;
                int quantityTotal = 0;
                decimal amountTotal = 0.00m;

                DataTable tblProductInfo = new DataTable();
                tblProductInfo.Columns.Add("ProductID");
                tblProductInfo.Columns.Add("ProductMode");
                tblProductInfo.Columns.Add("BMCode");
                tblProductInfo.Columns.Add("BriefName");
                tblProductInfo.Columns.Add("OrderPrice");
                tblProductInfo.Columns.Add("PurchaseQty");
                tblProductInfo.Columns.Add("InStockQty");
                tblProductInfo.Columns.Add("ProductPriceSummary");
                tblProductInfo.Columns.Add("NeedShowInStockQtyHtml");

                tblProductInfo.Columns.Add("TaxRateType");

                poInfo.POItems.ForEach(x =>
                {
                    DataRow dr = tblProductInfo.NewRow();
                    dr["ProductID"] = x.ProductID + (x.IsVirtualStockProduct == true ? "[虚库商品]" : string.Empty);
                    dr["ProductMode"] = x.ProductMode;
                    dr["BMCode"] = x.BMCode;
                    dr["NeedShowInStockQtyHtml"] = needShowInStockQty;
                    dr["BriefName"] = x.BriefName;
                    dr["OrderPrice"] = poInfo.PurchaseOrderBasicInfo.CurrencySymbol + x.OrderPrice.Value.ToString("f2");
                    dr["PurchaseQty"] = x.PurchaseQty;
                    dr["InStockQty"] = x.Quantity;
                    dr["ProductPriceSummary"] = (needShowInStockQty ? (x.Quantity.Value * x.OrderPrice.Value).ToString("f2") : (x.PurchaseQty.Value * x.OrderPrice.Value).ToString("f2"));
                    dr["TaxRateType"] = ((int)poInfo.PurchaseOrderBasicInfo.TaxRateType.Value).ToString();//税率
                    tblProductInfo.Rows.Add(dr);

                    purchaseQtyTotal += x.PurchaseQty.Value;
                    quantityTotal += x.Quantity.Value;
                    amountTotal += (needShowInStockQty ? x.Quantity.Value * x.OrderPrice.Value : x.PurchaseQty.Value * x.OrderPrice.Value);
                });

                tableVariables.Add("tblProductList", tblProductInfo);
                //总和:
                variables.Add("PurchaseQtyTotal", purchaseQtyTotal);
                variables.Add("NeedShowInStockQtySummary", needShowInStockQty);
                variables.Add("QuantityTotal", needShowInStockQty ? quantityTotal.ToString() : string.Empty);
                variables.Add("AmountTotal", amountTotal.ToString("f2"));

                #endregion

                #region 构建合计信息

                Func<PurchaseOrderTaxRate, decimal, decimal> Shuijin = (a, b) =>
                {
                    return ((decimal)(((int)a) / 100.00)) * b / ((decimal)(((int)a) / 100.00) + 1);
                };

                Func<PurchaseOrderTaxRate, decimal, decimal> Jiakuan = (a, b) =>
                {
                    return b / ((decimal)(((int)a) / 100.00) + 1);
                };

                Func<decimal?, decimal> IsNullDecimal = (a) =>
                {
                    if (a != null && a.HasValue) { return a.Value; } else { return 0m; }
                };

                var taxAndCost = IsNullDecimal(poInfo.PurchaseOrderBasicInfo.TotalActualPrice);
                var taxAndCostStr = taxAndCost.ToString("C");
                var allCost = Jiakuan(poInfo.PurchaseOrderBasicInfo.TaxRateType.Value, taxAndCost);
                var allCostStr = allCost.ToString("C");
                var allTax = Shuijin(poInfo.PurchaseOrderBasicInfo.TaxRateType.Value, taxAndCost);
                var allTaxStr = allTax.ToString("C");

                variables.Add("AllCost", allCostStr);
                variables.Add("AllTax", allTaxStr);
                variables.Add("TaxAndCost", taxAndCostStr);
                #endregion

                #region [构建配件信息]
                DataTable dtPOAccessories = GetPurchaseOrderAccessories(poInfo.SysNo.Value);
                DataTable tblAccessoriesList = new DataTable();
                tblAccessoriesList.Columns.Add("ProductID");
                tblAccessoriesList.Columns.Add("AccessoryIDAndName");
                tblAccessoriesList.Columns.Add("Qty");

                bool needShowAccessoriesList = dtPOAccessories == null || dtPOAccessories.Rows.Count <= 0 ? false : true;
                variables.Add("NeedShowAccessoriesList", needShowAccessoriesList);
                if (needShowAccessoriesList)
                {
                    foreach (DataRow dr in dtPOAccessories.Rows)
                    {
                        DataRow drRow = tblAccessoriesList.NewRow();
                        drRow["ProductID"] = dr["ProductID"].ToString();
                        drRow["AccessoryIDAndName"] = dr["AccessoriesID"].ToString() + "]" + dr["AccessoriesIDAndName"].ToString();
                        drRow["Qty"] = dr["Qty"].ToString();
                        tblAccessoriesList.Rows.Add(drRow);
                    }
                }
                tableVariables.Add("tblAccessoriesList", tblAccessoriesList);

                #endregion

                #region [构建返点信息]

                decimal eimsAmtTotal = 0.00m;
                decimal usedThisPlaceAmountTotal = 0.00m;

                bool needShowEIMSList = poInfo.EIMSInfo == null || poInfo.EIMSInfo.EIMSInfoList.Count <= 0 ? false : true;
                variables.Add("NeedShowEIMSList", needShowEIMSList);
                DataTable tblEIMSList = new DataTable();
                tblEIMSList.Columns.Add("EIMSNo");
                tblEIMSList.Columns.Add("EIMSName");
                tblEIMSList.Columns.Add("EIMSTotalAmt");
                tblEIMSList.Columns.Add("RelateNotReceivedAmount");
                tblEIMSList.Columns.Add("ReceivedAmount");
                tblEIMSList.Columns.Add("EIMSAmt");
                tblEIMSList.Columns.Add("UseThisPlaceAmount");
                tblEIMSList.Columns.Add("LeaveUseThisPlaceAmount");

                if (needShowEIMSList)
                {
                    foreach (var item in poInfo.EIMSInfo.EIMSInfoList)
                    {
                        DataRow dr = tblEIMSList.NewRow();
                        dr["EIMSNo"] = item.EIMSSysNo;
                        dr["EIMSName"] = item.EIMSName;
                        dr["EIMSTotalAmt"] = item.EIMSTotalAmt.HasValue ? item.EIMSTotalAmt.Value.ToString("f2") : "0.00";
                        dr["RelateNotReceivedAmount"] = (item.RelateAmount - item.ReceivedAmount).HasValue ? (item.RelateAmount - item.ReceivedAmount).Value.ToString("f2") : "0.00";
                        dr["ReceivedAmount"] = item.ReceivedAmount.HasValue ? item.ReceivedAmount.Value.ToString("f2") : "0.00";
                        dr["EIMSAmt"] = item.EIMSAmt.HasValue ? item.EIMSAmt.Value.ToString("f2") : "0.00";
                        dr["UseThisPlaceAmount"] = (item.EIMSAmt - item.LeftAmt).HasValue ? (item.EIMSAmt - item.LeftAmt).Value.ToString("f2") : "0.00";
                        dr["LeaveUseThisPlaceAmount"] = (item.EIMSTotalAmt - item.RelateAmount).HasValue ? (item.EIMSTotalAmt - item.RelateAmount).Value.ToString("f2") : "0.00";

                        tblEIMSList.Rows.Add(dr);

                        eimsAmtTotal += item.EIMSAmt.HasValue ? item.EIMSAmt.Value : 0;
                        usedThisPlaceAmountTotal += Convert.ToDecimal(dr["UseThisPlaceAmount"].ToString());
                    }
                }
                tableVariables.Add("tblEIMSList", tblEIMSList);
                //总和:
                variables.Add("EimsAmtTotal", eimsAmtTotal.ToString("f2"));
                variables.Add("UsedThisPlaceAmountTotal", usedThisPlaceAmountTotal.ToString("f2"));

                #endregion

                //发送异步，内部邮件
                EmailHelper.SendEmailByTemplate(poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress, "PO_AutoCreateMail", variables, tableVariables, true);
            }
            #endregion

            return poInfo;
        }

        /// <summary>
        /// PO单 - 终止入库操作(PO单部分入库状态下，中止入库（即手动关闭）)
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo StopInStockPO(PurchaseOrderInfo poInfo)
        {
            PurchaseOrderInfo poentity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
            poInfo.PurchaseOrderBasicInfo.TotalAmt = poentity.PurchaseOrderBasicInfo.TotalAmt;
            poInfo.POItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            poInfo.EIMSInfo.EIMSInfoList = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = poentity.PurchaseOrderBasicInfo.PurchaseOrderStatus;
            poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo = poentity.PurchaseOrderBasicInfo.StockInfo.SysNo;

            //计算剩余返点
            decimal noUseEims = 0.00m;
            poInfo.EIMSInfo.EIMSInfoList.ForEach(x =>
            {
                noUseEims += (x.LeftAmt.HasValue ? x.LeftAmt.Value : 0);
            });
            if (PurchaseOrderStatus.PartlyInStocked != poentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.Value)
            {
                throw new BizException(GetMessageString("PO_Close_Failed"));
            }
            //获取PO LOG信息:
            PurchaseOrderLogInfo poLogInfo = PurchaseOrderDA.LoadPOLogInfo(poInfo.SysNo.Value);
            //调用Invoice接口:CreateNewpay

            decimal? totalAmt = poInfo.POItems.Sum(x => x.PurchaseQty * x.OrderPrice);
            decimal? trueAmt = poInfo.POItems.Sum(p => p.Quantity * p.OrderPrice);
            //状态直接写int
            ExternalDomainBroker.CreatePayByVendor(poInfo.SysNo.Value, 1, 6, PayableOrderType.POAdjust, (totalAmt - trueAmt - noUseEims), poInfo.CompanyCode);

            using (TransactionScope scope = new TransactionScope())
            {
                // 扣减采购在途库存
                List<KeyValuePair<int, int>> kv = new List<KeyValuePair<int, int>>();
                foreach (PurchaseOrderItemInfo pi in poInfo.POItems)
                {
                    if (poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
                    {
                        kv.Add(new KeyValuePair<int, int>(pi.ProductSysNo.Value, pi.Quantity.Value - pi.PurchaseQty.Value));
                    }
                }

                //调用Inventory接口,扣减采购在途库存
                //InventoryMgmtService.SetPurchaseQty(poentity.StockSysNo.Value, kv, poentity.SysNo.Value, 2);

                InventoryAdjustContractInfo contractInfo = new InventoryAdjustContractInfo()
                {
                    ReferenceSysNo = poInfo.SysNo.Value.ToString(),
                    SourceActionName = InventoryAdjustSourceAction.StopInStock,
                    SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order
                };

                contractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                kv.ForEach(x =>
                {
                    contractInfo.AdjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.Key,
                        StockSysNo = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value,
                        AdjustQuantity = x.Value
                    });
                });

                ExternalDomainBroker.AdjustProductInventory(contractInfo);

                if (poLogInfo != null)
                {
                    List<EIMSInfo> poEimsEntitys = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);

                    ////TODO:调用EIMS接口,返还多余返点
                    //List<POAttachInfo> poAttachEntitys = new List<POAttachInfo>();

                    //if (poEimsEntitys != null && poEimsEntitys.Count > 0)
                    //{
                    //    foreach (EIMSInfo poEims in poEimsEntitys)
                    //    {
                    //        if (poEims.LeftAmt > 0.00m)
                    //        {
                    //            POAttachInfo poAttachInfo = new POAttachInfo()
                    //            {
                    //                PONumber = poInfo.SysNo.Value,
                    //                InvoiceNumber = poEims.EIMSSysNo,
                    //                PostTime = System.DateTime.Now,
                    //                UseTime = System.DateTime.Now,
                    //                POStatus = (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus.Value,
                    //                C3SysNo = -1
                    //            };
                    //            poAttachEntitys.Add(poAttachInfo);
                    //        }
                    //    }
                    //    //TODO:调用EIMS接口,返还多余返点:EIMSMgmtService.ReturnPartPMPoint(poAttachEntitys, entity.OperationUserSysNumber.Value.ToString());
                    //}

                    //将PO总价格TotalAmt更新为各批次入库总金额
                    poInfo.PurchaseOrderBasicInfo.UsingReturnPoint = poLogInfo.SumEIMSAmt;
                    poInfo.PurchaseOrderBasicInfo.TotalAmt = poLogInfo.SumTotalAmt;
                }

                //将PO的状态修改为“手动关闭”:
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.ManualClosed;
                PurchaseOrderDA.UpdatePOMasterStock(poInfo);
                scope.Complete();
            }
            //CRL17821 手动关闭发送SSB消息
            SendCloseMessage(poInfo.SysNo.Value, 1, poInfo.CompanyCode);


            return poInfo;
        }

        /// <summary>
        /// PO单 - 取消作废
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo CancelAbandonPO(PurchaseOrderInfo poInfo)
        {
            //不能为空
            if (!poInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_SysNoEmpty"));
            }
            bool notAbandon = (PurchaseOrderDA.LoadPOSSBLog(poInfo.SysNo.Value, PurchaseOrderSSBMsgType.C).Count > 0);
            if (notAbandon)
            {
                throw new BizException(string.Format(GetMessageString("PO_Abandon_Failed"), poInfo.SysNo.Value));
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            PurchaseOrderInfo localEntity;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                localEntity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
                if (localEntity == null)
                {
                    throw new BizException(GetMessageString("PO_PONotFound"));
                }

                if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Abandoned &&
                    ((int)localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus.Value) > 0)
                {
                    throw new BizException(GetMessageString("PO_CancelAbandon_Failed"));
                }

                //设置 单号、状态和审核人
                localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;

                PurchaseOrderDA.CanecelAbandonPO(localEntity);

                //写LOG：CommonService.WriteLog<POEntity>(entity, " Cancel abandon PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_CancelAbandon);

                ExternalDomainBroker.CreateLog(" Cancel abandon PO "
               , BizEntity.Common.BizLogType.Purchase_CancelAbandon
               , poInfo.SysNo.Value
               , poInfo.CompanyCode);
                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// PO单 - 作废
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo AbandonPO(PurchaseOrderInfo poInfo)
        {
            if (!poInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_SysNoEmpty"));
            }

            var options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            PurchaseOrderInfo localEntity;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                localEntity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
                if (localEntity == null)
                {
                    throw new BizException(GetMessageString("PO_PONotFound"));
                }

                if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Created && localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Returned)
                {
                    throw new BizException(GetMessageString("PO_Operate_Failed"));
                }

                //设置 单号、状态和审核人, 作废操作 ：
                localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Abandoned;
                PurchaseOrderDA.AbandonPO(localEntity);

                PurchaseOrderETATimeInfo poetaEntity = new PurchaseOrderETATimeInfo();
                poetaEntity.Status = -1;
                poetaEntity.POSysNo = poInfo.SysNo;
                PurchaseOrderDA.UpdatePOETAInfo(poetaEntity);

                //写LOG：CommonService.WriteLog<POEntity>(entity, " Abandon PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Abandon);
                ExternalDomainBroker.CreateLog(" Abandon PO "
      , BizEntity.Common.BizLogType.Purchase_Abandon
      , poInfo.SysNo.Value
      , poInfo.CompanyCode);

                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// PO单 - 提交审核
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo SubmitPO(PurchaseOrderInfo poInfo)
        {
            #region [Check操作]

            if (poInfo == null)
            {
                throw new BizException(GetMessageString("PO_Submit_Failed"));
            }
            if (!string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo))
            {
                string[] strs = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo.Split(new char[] { '[' });
                if (strs.Length > 0)
                {
                    if (strs[strs.Length - 1].LastIndexOf(']') == strs[strs.Length - 1].Length - 1)
                    {
                        poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo += "|";
                        poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo.Replace("[" + strs[strs.Length - 1] + "|", "");
                    }
                }
            }
            //泰隆优选:采购单编辑页面，提交审核采购单，请去除验证商品是否属于此商家的代理品牌的逻辑
            ////代销类型的数据
            //if (poInfo.PurchaseOrderBasicInfo.ConsignFlag.Value == PurchaseOrderConsignFlag.Consign)
            //{
            //    string message = string.Empty;
            //    //获取代理商品:
            //    List<ProductInfo> products = PurchaseOrderDA.QueryVendorProductByVendorSysNo(poInfo.VendorInfo.SysNo.Value);
            //    //获取比对 PO Item 是否在代理商品中:
            //    poInfo.POItems.ForEach(item =>
            //    {
            //        bool exists = products.Exists(x => (x.SysNo == item.ProductSysNo.Value));
            //        if (!exists)
            //        {
            //            message += string.Format(GetMessageString("PO_Product_NotMatchVendor") + Environment.NewLine, item.ProductID, poInfo.VendorInfo.VendorBasicInfo.VendorNameLocal);
            //        }
            //    });

            //    if (message != string.Empty)
            //    {
            //        throw new BizException(message);
            //    }
            //}
            if (poInfo.SysNo.HasValue)
            {
                PurchaseOrderInfo result = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
                if (!(result.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || result.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
                {
                    throw new BizException(GetMessageString("PO_ResubmitError"));
                }
            }
            else
            {
                throw new BizException(GetMessageString("PO_POSysNoNotFound"));
            }

            //2012-9-13 Jack
            CheckReturnPointAndPrice(poInfo);

            StringBuilder exceptionInfo = new StringBuilder();
            exceptionInfo.Append(CheckETAIsNull(poInfo));
            exceptionInfo.Append(CheckETA(poInfo, PurchaseOrderActionType.Check));
            exceptionInfo.Append(CheckResultIsNull(poInfo));
            exceptionInfo.Append(CheckPMRequestMemoIsNull(poInfo));
            exceptionInfo.Append(CheckStateCanSubmit(poInfo));
            var entityold = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
            var poitems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            entityold.POItems = poitems;
            exceptionInfo.Append(CheckPOItemNumber(entityold));
            exceptionInfo.Append(CheckPOItemHasBatchInfo(entityold));
            if (exceptionInfo.ToString().Length > 4)
            {
                throw new BizException(exceptionInfo.ToString());
            }

            #endregion

            poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo + "[" + CommmonBizInteract.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true) + ":" + DateTime.Now.ToString() + "]";
            poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = string.Empty;
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.WaitingAudit;
            PurchaseOrderInfo localEntity = poInfo;
            poInfo.PurchaseOrderBasicInfo.Privilege = localEntity.PurchaseOrderBasicInfo.Privilege;
            poInfo.POItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            poInfo.PurchaseOrderBasicInfo.AuditUserSysNo = localEntity.PurchaseOrderBasicInfo.AuditUserSysNo;
            if (null == poInfo.EIMSInfo)
            {
                poInfo.EIMSInfo = new PurchaseOrderEIMSInfo();
            }
            if (null == poInfo.EIMSInfo.EIMSInfoList)
            {
                poInfo.EIMSInfo.EIMSInfoList = new List<EIMSInfo>();
            }
            poInfo.EIMSInfo.EIMSInfoList = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);

            localEntity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus;

            if (0 < PurchaseOrderDA.UpdatePOStatus(localEntity))
            {
                PurchaseOrderETATimeInfo etaEntity = new PurchaseOrderETATimeInfo()
                {
                    ETATime = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value,
                    HalfDay = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay,
                    POSysNo = localEntity.SysNo
                };

                PurchaseOrderDA.UpdatePOMasterETAInfo(etaEntity);
                //提交检查时进行审核操作
                Dictionary<string, string> dictonary = CreateCheckDictionary(poInfo);
                int countInt = 0;
                foreach (var item in dictonary)
                {
                    if (item.Value.IndexOf(GetMessageString("PO_AuditText")) > 1)
                    {
                        countInt++;
                    }
                }
                if (countInt == 0)
                {
                    PurchaseOrderInfo returnEntity = WaitingInStockPO(localEntity, false);
                    poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = returnEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus;
                }
                else
                {
                    poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = 1;
                    //写LOG:CommonService.WriteLog<POEntity>(entity, "权限不足由待审核调整为待TL审核 ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

                    ExternalDomainBroker.CreateLog(GetMessageString("PO_UpdateMemo_Audit2")
           , BizEntity.Common.BizLogType.Purchase_Verify_InStock
           , poInfo.SysNo.Value
           , poInfo.CompanyCode);
                    PurchaseOrderDA.UpdatePOTPStatus(poInfo);
                }
                return poInfo;
            }

            //发送ESB消息
            EventPublisher.Publish<PurchaseOrderSubmitAuditMessage>(new PurchaseOrderSubmitAuditMessage()
            {
                SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = poInfo.SysNo.Value
            });

            return null;
        }

        /// <summary>
        /// PO单 - 取消确认
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo CancelVerifyPO(PurchaseOrderInfo poInfo)
        {
            if (!poInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_SysNoEmpty"));
            }
            //调用Invoice接口，获取付款的预付款记录 ,PurchaseOrderDAL.GetPOPrepay(entity.SysNo.Value);
            PayItemStatus? prepayStatus = ExternalDomainBroker.GetPOPrePayItemStatus(poInfo.SysNo.Value);

            if (prepayStatus.HasValue && prepayStatus == PayItemStatus.Paid)
            {
                throw new BizException(GetMessageString("PO_CancelVerify_Error"));
            }

            if (prepayStatus.HasValue && prepayStatus == PayItemStatus.Origin)
            {
                throw new BizException(GetMessageString("PO_CancelVerify_AbandonPay"));
            }

            PurchaseOrderInfo localEntity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
            if (localEntity == null)
            {
                throw new BizException(GetMessageString("PO_PONotFound"));
            }
            //采购单的状态不是等待入库状态，操作失败
            if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingInStock)
            {
                throw new BizException(GetMessageString("PO_NotWaitingInStock_OprFailed"));
            }
            List<PurchaseOrderItemInfo> listItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                //判断是否属于批次商品:
                var emptyBatchItems = from item in listItems
                                      where PurchaseOrderDA.IsBatchProduct(item) && string.IsNullOrEmpty(item.BatchInfo)
                                      select item.ProductID;

                if (emptyBatchItems.Count() > 0)
                {
                    throw new BizException(string.Format(GetMessageString("PO_IsNotBatchProductInfo"), string.Join(",", emptyBatchItems.ToArray())));
                }
            }
            int operationStatus = 0;
            try
            {
                //取消审核时，需要优先调用WMS提供的hold接口，返回成功后，才能进行取消逻辑
                //TLYH 移除与仓库的交互
                //CallWMSHoldMethod(poInfo, ref operationStatus);
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = TransactionManager.DefaultTimeout;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    //设置 单号、状态和审核人
                    localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
                    localEntity.PurchaseOrderBasicInfo.AuditDate = DateTime.Now;
                    localEntity.PurchaseOrderBasicInfo.AuditUserSysNo = 1;
                    localEntity.PurchaseOrderBasicInfo.IsApportion = 0;
                    localEntity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo;

                    //采购单取消确认操作:
                    if (PurchaseOrderDA.UpdatePOItemsStatus(localEntity) == null)
                    {
                        throw new BizException(GetMessageString("PO_Confirm_Failed"));
                    }

                    List<PurchaseOrderItemInfo> items = PurchaseOrderDA.LoadPOItems(localEntity.SysNo.Value);
                    List<KeyValuePair<int, int>> kv = new List<KeyValuePair<int, int>>();
                    foreach (PurchaseOrderItemInfo item in items)
                    {
                        kv.Add(new KeyValuePair<int, int>((int)item.ProductSysNo, -1 * (int)item.PurchaseQty));
                    }

                    //如果有返点金额被扣除，需要恢复

                    //List<POAttachInfo> poAttachEntitys = new List<POAttachInfo>();
                    List<EIMSInfo> poEimsEntitys = PurchaseOrderDA.LoadPOEIMSInfo(localEntity.SysNo.Value);
                    if (poEimsEntitys != null && poEimsEntitys.Count > 0)
                    {
                        //foreach (PurchaseOrderEIMSInfo poEims in poEimsEntitys)
                        //{
                        //    POAttachInfo poAttachInfo = new POAttachInfo()
                        //    {
                        //        PONumber = localEntity.SysNo.Value,
                        //        InvoiceNumber = poEims.EIMSNo,
                        //        UseInvoiceAmount = poEims.EIMSAmt * -1,
                        //        PostTime = System.DateTime.Now,
                        //        UseTime = System.DateTime.Now,
                        //        C3SysNo = -1
                        //    };
                        //    poAttachEntitys.Add(poAttachInfo);
                        //}
                        //TODO:调用EIMS接口，恢复返点金额:
                        //EIMSMgmtService.ResumePMRemnantReturnPoint(poAttachEntitys, entity.AuditUserSysNo.Value.ToString());
                    }

                    //调用Inventory接口 , 设置采购在途数量,（代销PO该业务逻辑不变）
                    //InventoryMgmtService.SetPurchaseQty(localEntity.StockSysNo.Value, kv, localEntity.SysNo.Value, 1);
                    InventoryAdjustContractInfo contractInfo = new InventoryAdjustContractInfo()
                    {
                        ReferenceSysNo = poInfo.SysNo.Value.ToString(),
                        SourceActionName = InventoryAdjustSourceAction.CancelAudit,
                        SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order
                    };

                    contractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();
                    kv.ForEach(x =>
                    {
                        contractInfo.AdjustItemList.Add(new InventoryAdjustItemInfo()
                        {
                            ProductSysNo = x.Key,
                            StockSysNo = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value,
                            AdjustQuantity = x.Value
                        });
                    });

                    ExternalDomainBroker.AdjustProductInventory(contractInfo);

                    PurchaseOrderETATimeInfo poetaEntity = new PurchaseOrderETATimeInfo();
                    poetaEntity.EditUser = "SysAdmin";
                    poetaEntity.Status = -1;
                    poetaEntity.POSysNo = poInfo.SysNo;
                    //更新ETA:
                    PurchaseOrderDA.UpdatePOETAInfo(poetaEntity);
                    if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                    {
                        poInfo.POItems = items;
                        SetInvotryInfo(poInfo, "-");
                    }

                    scope.Complete();
                }
            }
            finally
            {
                //记录ＷＭＳ的Ｈold结果
                WriteWMSLog(operationStatus, poInfo);
            }

            return localEntity;
        }
        private string WriteWMSLog(int operationStatus, PurchaseOrderInfo poInfo)
        {
            string logTitle;

            if (operationStatus == 2)
            {
                logTitle = " Cancel Verify PO Succeeded";
            }
            else if (operationStatus == 1)
            {
                logTitle = " Cancel Verify PO -- hold succeeded,but other operations failed";
            }
            else
            {
                logTitle = " Cancel Verify PO Failed";
            }
            ExternalDomainBroker.CreateLog(logTitle
              , BizEntity.Common.BizLogType.Purchase_CancelVerify
              , poInfo.SysNo.Value
              , poInfo.CompanyCode);
            return logTitle;
        }
        /// <summary>
        /// PO单 PM与供应商确认
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo PMConfirmWithVendor(PurchaseOrderInfo poInfo)
        {
            if (!poInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_SysNoEmpty"));
            }
            //与供应商确认操作:
            PurchaseOrderDA.ConfirmWithVendor(poInfo);
            return poInfo;
        }

        /// <summary>
        /// 加载PO单:
        /// </summary>
        /// <param name="poBasicInfo"></param>
        public virtual PurchaseOrderInfo LoadPO(int poSysNo)
        {
            PurchaseOrderInfo poInfo = new PurchaseOrderInfo()
            {
                SysNo = poSysNo
            };
            //1.加载采购单基本信息：
            poInfo.PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo();
            poInfo = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);

            PayType getPayTypeInfo = ExternalDomainBroker.GetPayTypeBySysNo(poInfo.PurchaseOrderBasicInfo.PayType.SysNo.Value);
            if (null != getPayTypeInfo)
            {
                poInfo.PurchaseOrderBasicInfo.PayType.PayTypeName = getPayTypeInfo.PayTypeName;
            }
            poInfo.PurchaseOrderBasicInfo.ETATimeInfo.POSysNo = poInfo.SysNo;
            PurchaseOrderETATimeInfo getCheckETMTimeInfo = PurchaseOrderDA.LoadPOETATimeInfo(poInfo.SysNo.Value);
            if (null != getCheckETMTimeInfo)
            {
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo = getCheckETMTimeInfo;
            }
            //2.加载采购单供应商信息：
            int? getVendorSysNo = poInfo.VendorInfo.SysNo;
            poInfo.VendorInfo = new VendorInfo()
            {
                SysNo = getVendorSysNo,
                VendorAgentInfoList = new List<VendorAgentInfo>()
            };
            poInfo.VendorInfo = VendorProcessor.LoadVendorInfo(poInfo.VendorInfo.SysNo.Value);
            //3.加载采购单返点信息：
            poInfo.EIMSInfo = new PurchaseOrderEIMSInfo();
            poInfo.EIMSInfo.EIMSInfoList = PurchaseOrderDA.LoadPOEIMSInfo(poInfo.SysNo.Value);
            //4.加载采购单商品列表：
            poInfo.POItems = new List<PurchaseOrderItemInfo>();
            poInfo.POItems = PurchaseOrderDA.LoadPOItems(poInfo.SysNo.Value);
            foreach (var item in poInfo.POItems)
            {

                ////获取本地货币:
                if (poInfo.PurchaseOrderBasicInfo.CurrencyCode.HasValue)
                {
                    item.CurrencyCode = poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value;
                    CurrencyInfo localCurrency = CommmonBizInteract.GetCurrencyInfoBySysNo(item.CurrencyCode.Value);
                    item.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;
                }
                item.Tax = CalculateProductRate(item);
                item.JingDongTax = CalculateJDRate(item);
                //***********获取销售数量，价格等信息 ：

                #region

                InitializePOItemStockInfo(item);

                #endregion
            }

            //获取polog的入库总金额和总返点:
            PurchaseOrderLogInfo poLogInfo = PurchaseOrderDA.LoadPOLogInfo(poInfo.SysNo.Value);
            if (null != poLogInfo)
            {
                poInfo.PurchaseOrderBasicInfo.TotalActualPrice = poLogInfo.SumTotalAmt;
            }
            if (poInfo.PurchaseOrderBasicInfo.TotalActualPrice == 0)
            {
                foreach (PurchaseOrderItemInfo pitem in poInfo.POItems)
                {
                    poInfo.PurchaseOrderBasicInfo.TotalActualPrice += pitem.OrderPrice.Value * pitem.Quantity;
                }
            }

            //5.加载采购单收货信息:
            poInfo.ReceivedInfoList = new List<PurchaseOrderReceivedInfo>();
            poInfo.ReceivedInfoList = PurchaseOrderDA.LoadPurchaseOrderReceivedInfo(poInfo);
            foreach (PurchaseOrderReceivedInfo revInfo in poInfo.ReceivedInfoList)
            {
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    if (revInfo.ProductSysNo == item.ProductSysNo)
                    {
                        revInfo.PurchaseQty = (item.PurchaseQty.HasValue ? item.PurchaseQty.Value : 0);
                        revInfo.WaitInQty = revInfo.PurchaseQty - revInfo.ReceivedQuantity;
                    }
                }
            }
            //返回PO实体:
            return poInfo;
        }

        public virtual PurchaseOrderItemInfo LoadPOItemInfo(int itemSysNo)
        {
            PurchaseOrderItemInfo itemInfo = PurchaseOrderDA.LoadPOItem(itemSysNo);
            if (itemInfo.CurrencyCode.HasValue)
            {
                CurrencyInfo localCurrency = CommmonBizInteract.GetCurrencyInfoBySysNo(itemInfo.CurrencyCode.Value);
                itemInfo.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;
            }
            return itemInfo;
        }

        /// <summary>
        /// 加载PO单返点信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public virtual List<EIMSInfo> LoadPOEIMSInfo(int poSysNo)
        {
            return PurchaseOrderDA.LoadPOEIMSInfo(poSysNo);
        }

        /// <summary>
        /// 更新采购单入库金额
        /// </summary>
        /// <param name="poSysNo"></param>
        public virtual void UpdatePurchaseOrderInstockAmt(int poSysNo)
        {
            PurchaseOrderDA.UpdatePurchaseOrderInstockAmt(poSysNo);
        }

        /// <summary>
        /// PO单 - 更新入库备注
        /// </summary>
        /// <param name="poInfo"></param>
        public virtual PurchaseOrderInfo UpdatePOInStockMemo(PurchaseOrderInfo poInfo)
        {
            #region [Check 为空]

            if (!poInfo.PurchaseOrderBasicInfo.CarriageCost.HasValue)
            {
                //"到付运费金额不能为空！"
                throw new BizException(GetMessageString("PO_CarriageCost_Null"));
            }

            if (poInfo.PurchaseOrderBasicInfo.CarriageCost.Value < 0)
            {
                //到付运费金额必须大于0！
                throw new BizException(GetMessageString("PO_CarriageCost_LargerThanZero"));
            }

            if (!poInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_SysNoEmpty"));
            }

            #endregion

            PurchaseOrderInfo localEntity = new PurchaseOrderInfo();
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                localEntity = PurchaseOrderDA.LoadPOMaster(poInfo.SysNo.Value);
                if (localEntity == null)
                {
                    throw new BizException(GetMessageString("PO_PONotFound"));
                }

                localEntity.PurchaseOrderBasicInfo.CarriageCost = poInfo.PurchaseOrderBasicInfo.CarriageCost;
                localEntity.PurchaseOrderBasicInfo.MemoInfo.InStockMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo;
                localEntity = PurchaseOrderDA.UpdateInStockMemo(localEntity);

                //日志记录：
                //写LOG：CommonService.WriteLog<POEntity>(entity, " Update InStock Memo PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_InStockMemo);

                ExternalDomainBroker.CreateLog(" Update InStock Memo PO "
                , BizEntity.Common.BizLogType.Purchase_InStockMemo
                , poInfo.SysNo.Value
                , poInfo.CompanyCode);

                scope.Complete();
            }

            return poInfo;
        }

        /// <summary>
        /// PO单 - 拒绝并退回
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo RefusePO(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null)
            {
                throw new BizException(GetMessageString("PO_EntityNull"));
            }
            if (string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo))
            {
                throw new BizException(GetMessageString("PO_RefuseMemoNull"));
            }
            //审核人不能与创建人相同:
            //if (poInfo.PurchaseOrderBasicInfo.CreateUserSysNo == ServiceContext.Current.UserSysNo || poInfo.PurchaseOrderBasicInfo.AuditUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    throw new BizException(GetMessageString("PO_CreateAndAuditUserNotTheSame"));
            //}
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingAudit
                && poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus != 1
                && poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus != 2)
            {
                throw new BizException(GetMessageString("PO_NotTLOrPMDAudit"));
            }
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = null;
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Returned;

            ;
            if (PurchaseOrderDA.UpdatePOStatus(poInfo) > 0)
            {
                return poInfo;
            }

            //发送ESB消息
            EventPublisher.Publish<PurchaseOrderRejectMessage>(new PurchaseOrderRejectMessage()
            {
                RejectUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = poInfo.SysNo.Value
            });

            return null;
        }

        /// <summary>
        /// PO单 - 提交审核ETA信息
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderETATimeInfo SubmitETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            if (etaInfo == null)
            {
                throw new BizException(GetMessageString("PO_EntityNull"));
            }
            else
            {
                if (string.IsNullOrEmpty(etaInfo.Memo))
                {
                    throw new BizException(GetMessageString("PO_ReasonNull"));
                }
                else if (etaInfo.Status != 1)
                {
                    throw new BizException(GetMessageString("PO_ETAStatusInvalid"));
                }
                etaInfo = PurchaseOrderDA.CreatePOETAInfo(etaInfo);
                //发送ESB消息
                EventPublisher.Publish<PurchaseOrderETATimeInfoSubmitMessage>(new PurchaseOrderETATimeInfoSubmitMessage()
                {
                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                    ETATimeSysNo = etaInfo.ETATimeSysNo.Value
                });
            }
            return null;
        }

        /// <summary>
        /// PO单 - 取消审核ETA信息
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderETATimeInfo CancelETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            if (etaInfo == null)
            {
                throw new BizException(GetMessageString("PO_EntityNull"));
            }
            else
            {
                if (etaInfo.Status != 0)
                {
                    throw new BizException(GetMessageString("PO_ETAStatusInvalid"));
                }
                if (null != PurchaseOrderDA.UpdatePOETAInfo(etaInfo))
                {
                    return etaInfo;
                }

                //发送ESB消息
                EventPublisher.Publish<PurchaseOrderETATimeInfoCancelMessage>(new PurchaseOrderETATimeInfoCancelMessage()
                {
                    CancelUserSysNo = ServiceContext.Current.UserSysNo,
                    ETATimeSysNo = etaInfo.ETATimeSysNo.Value
                });
            }
            return null;
        }

        /// <summary>
        /// PO单 - ETA信息申请通过
        /// </summary>
        /// <param name="etaInfo"></param>
        /// <returns></returns>
        public virtual PurchaseOrderETATimeInfo PassETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            if (etaInfo == null)
            {
                throw new BizException(GetMessageString("PO_EntityNull"));
            }
            else
            {
                if (etaInfo.Status != 2)
                {
                    throw new BizException(GetMessageString("PO_ETAStatusInvalid"));
                }
                else if (!etaInfo.HalfDay.HasValue)
                {
                    throw new BizException(GetMessageString("PO_AMPMNull"));
                }
                else if (etaInfo.ETATime == DateTime.MinValue || etaInfo.ETATime == null)
                {
                    throw new BizException(GetMessageString("PO_ArriveTimeInvalid"));
                }
                using (TransactionScope ts = new TransactionScope())
                {
                    //更新ETA信息
                    PurchaseOrderDA.UpdatePOETAInfo(etaInfo);
                    //更新ETA信息到PO信息
                    PurchaseOrderDA.UpdatePOMasterETAInfo(etaInfo);

                    //发送ESB消息
                    EventPublisher.Publish<PurchaseOrderETATimeInfoAuditMessage>(new PurchaseOrderETATimeInfoAuditMessage()
                    {
                        AuditUserSysNo = ServiceContext.Current.UserSysNo,
                        ETATimeSysNo = etaInfo.ETATimeSysNo.Value
                    });

                    ts.Complete();
                }
            }
            return etaInfo;
        }

        /// <summary>
        /// 更新 PO单的 AutoSendEmail地址
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="NeedWriteLog"></param>
        public virtual void UpdatePOAutoAutoSendMail(PurchaseOrderInfo entity, bool NeedWriteLog)
        {
            if (NeedWriteLog)
            {
                //写LOG: CommonService.WriteLog<POEntity>(entity, "Update AutoSendEmail info : " + entity.AutoSendMail, entity.SysNo.Value.ToString(), (int)LogType.Purchase_Master_Update);
                ExternalDomainBroker.CreateLog("Update AutoSendEmail info : "
               , BizEntity.Common.BizLogType.Purchase_Master_Update
               , entity.SysNo.Value
               , entity.CompanyCode);
            }
            else
            {
                PurchaseOrderDA.UpdatePOAutoSendMailAddress(entity.SysNo.Value, entity.PurchaseOrderBasicInfo.AutoSendMailAddress);
            }
        }

        /// <summary>
        /// 审核通过后发邮件
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="toAddress"></param>
        /// <param name="emailTemplateID"></param>
        public virtual void SendEmailForCustomer(int poSysNo, string toAddress, string emailTemplateID)
        {
            PurchaseOrderInfo getPOInfo = LoadPO(poSysNo);

            if (null != getPOInfo)
            {
                WarehouseInfo getStockInfo = InventoryBizInteract.GetWarehouseInfoBySysNo(getPOInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value);
                string getStockName = getStockInfo.WarehouseName;
                string getCompanyName = "";
                string getCompanyAddress = "";
                string getComapnyTel = "";
                string getCompanyWebsite = "";
                string getStockAddress = getStockInfo.ReceiveAddress;
                string getStockContact = getStockInfo.ReceiveContact;
                string getStockTel = getStockInfo.ReceiveContactPhoneNumber;
                switch (getPOInfo.PurchaseOrderBasicInfo.StockInfo.SysNo)
                {
                    //case 50:
                    //    if (poInfo.PurchaseOrderBasicInfo.ITStockInfo != null && poInfo.PurchaseOrderBasicInfo.ITStockInfo.SysNo.HasValue)
                    //    {
                    //        string getITStockInfoName = InventoryBizInteract.GetWarehouseInfoBySysNo(poInfo.PurchaseOrderBasicInfo.ITStockInfo.SysNo.Value).WarehouseName;
                    //        getStockName = string.Format("经中转到{0}", getITStockInfoName);
                    //    }

                    //    break;
                    case 51:
                        //上海
                        break;
                    case 52:
                        //香港:
                        getCompanyAddress = "";
                        getComapnyTel = "";
                        break;
                    case 53:
                        //日本:
                        break;
                    //case 54:
                    //    //成都:
                    //    CompanyAddress = "成都双流县大件路西南航空港新地物流园区（西南民大新校区对面）";
                    //    ComapnyTel = "15982082844";
                    //    break;
                    //case 55:
                    //    //武汉:
                    //    CompanyAddress = "武汉市东西湖区革新大道（四明路与五环路之间）长江物流园C库10号门";
                    //    ComapnyTel = "13339983123";
                    //    break;
                    //case 59:
                    //    //上海市闵行:
                    //    CompanyAddress = "上海市闵行区虹梅南路3988号2号库";
                    //    ComapnyTel = "13122693665";
                    //    break;
                    default:
                        break;
                }

                KeyValueVariables keyValues = new KeyValueVariables();
                keyValues.Add("StockName", getPOInfo.PurchaseOrderBasicInfo.StockInfo.StockName);
                keyValues.Add("ETATime", getPOInfo.PurchaseOrderBasicInfo.ETATimeInfo == null || !getPOInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? string.Empty : getPOInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                keyValues.Add("CompanyName", getCompanyName);
                keyValues.Add("CompanyAddress", getCompanyAddress);
                keyValues.Add("CompanyTel", getComapnyTel);
                keyValues.Add("CompanyWebSite", getCompanyWebsite);
                keyValues.Add("StockAddress", getStockAddress);
                keyValues.Add("StockContact", getStockContact);
                keyValues.Add("StockTel", getStockTel);
                keyValues.Add("displayNo", "");


                string getOldSNNumber = GetWareHouseReceiptSerialNumber(poSysNo);
                string numberString = getOldSNNumber.Trim();
                if (getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked)
                {
                    string number = "0";

                    if (getOldSNNumber.Trim().Equals("") == false)
                    {
                        number = getOldSNNumber.Trim();
                    }
                    numberString = number;
                }
                else
                {
                    numberString = "无";
                }
                keyValues.Add("numberString", numberString);
                keyValues.Add("shipTypeString", getPOInfo.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName);

                decimal totalInPagedec = 0;
                string totalInPage = string.Empty;
                foreach (PurchaseOrderItemInfo item in getPOInfo.POItems)
                {
                    totalInPagedec += (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.OrderPrice.HasValue ? item.OrderPrice.Value : 0);
                }
                totalInPage = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + totalInPagedec.ToString("#########0.00");
                keyValues.Add("totalInPage", totalInPage);

                string totalAmt = string.Empty;
                string totalReturnPoint = string.Empty;
                if (getPOInfo.PurchaseOrderBasicInfo.PM_ReturnPointSysNo > 0)
                {
                    totalAmt = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()) - Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.UsingReturnPoint)).ToString("#########0.00");
                    totalReturnPoint = "产品总价：" + getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()).ToString("#########0.00") + " &nbsp;&nbsp; " + "使用返点：" + (Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.UsingReturnPoint)).ToString("#########0.00");
                }
                else
                {
                    totalAmt = getPOInfo.PurchaseOrderBasicInfo.CurrencySymbol + Convert.ToDecimal(getPOInfo.PurchaseOrderBasicInfo.TotalAmt.ToString()).ToString("#########0.00");
                    totalReturnPoint = "";
                }
                keyValues.Add("totalAmt", totalAmt);
                keyValues.Add("totalReturnPoint", totalReturnPoint);
                keyValues.Add("PMName", getPOInfo.PurchaseOrderBasicInfo.ProductManager.UserInfo.UserName);
                keyValues.Add("CurrencyName", ObjectFactory<ICommonBizInteract>.Instance.GetCurrencyInfoBySysNo(getPOInfo.PurchaseOrderBasicInfo.CurrencyCode.Value).CurrencyName);

                keyValues.Add("entity.POID", getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
                keyValues.Add("DateTime.Now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                keyValues.Add("vendor.VendorName", getPOInfo.VendorInfo.VendorBasicInfo.VendorNameLocal);
                keyValues.Add("vendor.SysNo", getPOInfo.VendorInfo.SysNo.Value.ToString());
                keyValues.Add("vendor.Address", getPOInfo.VendorInfo.VendorBasicInfo.Address);
                keyValues.Add("vendor.Contact", getPOInfo.VendorInfo.VendorBasicInfo.Contact);
                keyValues.Add("vendor.Fax", getPOInfo.VendorInfo.VendorBasicInfo.Fax);
                keyValues.Add("entity.InTime", getPOInfo.PurchaseOrderBasicInfo.InTime.HasValue ? getPOInfo.PurchaseOrderBasicInfo.InTime.Value.ToString() : "");
                keyValues.Add("entity.PayTypeName", getPOInfo.PurchaseOrderBasicInfo.PayType.PayTypeName);
                keyValues.Add("entity.Memo", getPOInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
                keyValues.Add("entity.InStockMemo", getPOInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);
                keyValues.Add("SendTimeString", DateTime.Now.ToString());

                //构建商品列表信息
                bool needShowInStockQty = ((getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked || getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed || getPOInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed) == true ? true : false);

                KeyTableVariables keyTables = new KeyTableVariables();
                DataTable productAccessoryList = new DataTable();

                DataTable productItemList = new DataTable();
                productItemList.Columns.Add("item.ProductID");
                productItemList.Columns.Add("item.IsVirtualStockProduct");
                productItemList.Columns.Add("item.ProductMode");
                productItemList.Columns.Add("item.BriefName");
                productItemList.Columns.Add("item.CurrencySymbol");
                productItemList.Columns.Add("item.OrderPrice");
                productItemList.Columns.Add("item.PurchaseQty");
                productItemList.Columns.Add("item.InStockQty");
                productItemList.Columns.Add("item.TaxRateType");
                productItemList.Columns.Add("item.PurchaseQtyOrderPrice");
                productItemList.Columns.Add("item.ProductPriceSummary");

                getPOInfo.POItems.ForEach(x =>
                {
                    DataRow dr = productItemList.NewRow();
                    dr["item.ProductID"] = x.ProductID;
                    dr["item.IsVirtualStockProduct"] = x.IsVirtualStockProduct;
                    dr["item.ProductMode"] = x.ProductMode;
                    dr["item.BriefName"] = x.BriefName;
                    dr["item.CurrencySymbol"] = x.CurrencySymbol;
                    dr["item.OrderPrice"] = x.OrderPrice.Value.ToString("#########0.00");
                    dr["item.PurchaseQty"] = x.PurchaseQty.Value.ToString();
                    dr["item.PurchaseQty"] = x.PurchaseQty;
                    dr["item.InStockQty"] = x.Quantity;
                    dr["item.TaxRateType"] = ((int)getPOInfo.PurchaseOrderBasicInfo.TaxRateType.Value).ToString();//税率
                    dr["item.ProductPriceSummary"] = (needShowInStockQty ? (x.Quantity.Value * x.OrderPrice.Value).ToString("f2") : (x.PurchaseQty.Value * x.OrderPrice.Value).ToString("f2"));
                    productItemList.Rows.Add(dr);
                });

                DataTable dt = GetPurchaseOrderAccessories(poSysNo);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow sr in dt.Rows)
                    {
                        DataRow dr = productAccessoryList.NewRow();
                        dr["ProductID"] = dr["ProductID"].ToString();
                        dr["AccessoriesID"] = dr["AccessoriesID"].ToString();
                        dr["AccessoriesIDAndName"] = dr["AccessoriesIDAndName"].ToString();
                        dr["Qty"] = dr["Qty"].ToString();
                        productAccessoryList.Rows.Add(dr);
                    }
                }
                keyTables.Add("tblProductItemsList", productItemList);
                keyTables.Add("tblProductAccessoryList", productItemList);

                ExternalDomainBroker.SendExternalEmail(toAddress, emailTemplateID, keyValues, keyTables, Thread.CurrentThread.CurrentCulture.Name);
            }
            else
            {
                throw new BizException("未找到相关PO单数据!");
            }

        }

        /// <summary>
        /// 审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="mailAddress"></param>
        public virtual void UpdateMailAddressAndHasSentMail(int poSysNo, string mailAddress, string companyCode)
        {
            string result = string.Empty;
            //获取数据库中实际 已发送邮件列表
            string DBMailAddress = PurchaseOrderDA.GetMailAddressByPOSysNo(poSysNo);

            if (string.IsNullOrEmpty(mailAddress))
                return;

            if (!string.IsNullOrEmpty(DBMailAddress))
            {
                string[] inMailAddressList = mailAddress.Split(';');//获取输入的邮件列表项

                for (int i = 0; i < inMailAddressList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(inMailAddressList[i]) && DBMailAddress.Contains(inMailAddressList[i].Replace(" ", "")))//判断重复
                    {
                        inMailAddressList[i] = inMailAddressList[i].Replace(inMailAddressList[i], ""); //剔除重复
                    }
                }

                mailAddress = "";

                for (int i = 0; i < inMailAddressList.Length; i++)
                {
                    if (inMailAddressList[i] != "")
                    {
                        mailAddress += inMailAddressList[i].Replace(" ", "") + ";";
                    }
                }
                mailAddress = mailAddress.Substring(0, mailAddress.Length - 1 < 0 ? 0 : mailAddress.Length - 1);//生成非重复 的 输入邮件
                if (mailAddress.Length + DBMailAddress.Length + 1 > 500)
                {
                    //记日志保存
                    //CommonService.WriteLog<POEntity>(poEntity, "Update MailAddress Info : " + MailAddress, poEntity.SysNo.Value.ToString(), (int)LogType.Purchase_Master_Update);

                    ExternalDomainBroker.CreateLog("Update MailAddress Info : "
                    , BizEntity.Common.BizLogType.Purchase_Master_Update
                    , poSysNo
                    , companyCode);
                    result = GetMessageString("PO_AddressMailMaxCount");
                }
                else
                {
                    if (!string.IsNullOrEmpty(mailAddress))
                    {
                        result = DBMailAddress + ";" + result;
                        int flag = PurchaseOrderDA.UpdateMailAddressByPOSysNo(poSysNo, mailAddress);
                        if (flag <= 0)
                        {
                            result = GetMessageString("PO_SendMailFailed");
                            throw new BizException(result);
                        }
                        else
                        {
                            //发送邮件
                            this.SendEmailForCustomer(poSysNo, mailAddress, "PO_SendCustomerMail");
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mailAddress))
                {
                    if (mailAddress.Length > 500)
                    {
                        //记日志保存
                        //CommonService.WriteLog<POEntity>(poEntity, "Update MailAddress Info : " + MailAddress, poEntity.SysNo.Value.ToString(), (int)LogType.Purchase_Master_Update);

                        ExternalDomainBroker.CreateLog("Update MailAddress Info : "
                     , BizEntity.Common.BizLogType.Purchase_Master_Update
                     , poSysNo
                     , companyCode);
                        result = GetMessageString("PO_AddressMailMaxCount");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mailAddress))
                        {
                            result = DBMailAddress + ";" + result;
                            int flag = PurchaseOrderDA.UpdateMailAddressByPOSysNo(poSysNo, mailAddress);
                            if (flag <= 0)
                            {
                                result = GetMessageString("PO_SendMailFailed");
                                throw new BizException(result);
                            }
                            else
                            {

                                //发送邮件
                                this.SendEmailForCustomer(poSysNo, mailAddress, "PO_SendCustomerMail");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  确认 VendorPortal 提交的PO单据信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo ConfirmVendorPortalPurchaseOrder(PurchaseOrderInfo entity)
        {
            //记录VP端的PO ID，用于发送SSB消息。
            string vpPOID = entity.PurchaseOrderBasicInfo.PurchaseOrderID;

            string exceptionInfo = PreCheckCreatePO(entity);    //校验PO单信息
            if (!string.IsNullOrEmpty(exceptionInfo))
            {
                throw new BizException(exceptionInfo);
            }

            //确认 PO 单信息
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //设置初始化值
                entity.SysNo = entity.SysNo.Value;
                decimal totalAmt = 0;
                if (null != entity.POItems)
                {
                    foreach (PurchaseOrderItemInfo item in entity.POItems)
                    {
                        totalAmt += item.OrderPrice.Value * item.PurchaseQty.Value;  //计算采购总金额
                    }
                }
                entity.PurchaseOrderBasicInfo.TotalAmt = totalAmt;
                entity.PurchaseOrderBasicInfo.PurchaseOrderID = entity.SysNo.Value.ToString();
                entity.PurchaseOrderBasicInfo.CreateDate = System.DateTime.Now;
                entity.PurchaseOrderBasicInfo.IsApportion = 0;
                entity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
                entity.PurchaseOrderBasicInfo.AuditDate = null;
                entity.PurchaseOrderBasicInfo.AuditUserSysNo = null;
                entity.PurchaseOrderBasicInfo.InUserSysNo = null;

                string CreateUserSysNo = AppSettingManager.GetSetting("PO", "VendorCreateUserSysNo");
                if (entity.PurchaseOrderBasicInfo.Source.ToUpper() == "VP" && !string.IsNullOrEmpty(CreateUserSysNo))
                {
                    entity.PurchaseOrderBasicInfo.CreateUserSysNo = int.Parse(CreateUserSysNo);
                }
                PurchaseOrderDA.ConfirmVendorPortalPurchaseOrder(entity); //修改采购单信息
                //<--设置PO BatchInfo 信息
                List<PurchaseOrderItemInfo> oldPoItems = PurchaseOrderDA.LoadPOItems(entity.SysNo.Value);
                List<PurchaseOrderItemInfo> currentPoItems = entity.POItems;
                //-->
                entity.POItems.ForEach(x =>
                {
                    PurchaseOrderItemInfo item = oldPoItems.Find(old => x.ProductSysNo == old.ProductSysNo);
                    if (item != null)
                    {
                        x.BatchInfo = item.BatchInfo;
                    }
                });

                PurchaseOrderDA.DeletePOItems(entity.SysNo.Value); //删除原PO item
                foreach (PurchaseOrderItemInfo item in entity.POItems)
                {
                    if (item.Quantity != 0)
                    {
                        item.Quantity = 0;
                    }
                    item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + entity.PurchaseOrderBasicInfo.TaxRate.Value), 2);

                    //针对采购篮的操作
                    if (item.ItemSysNo.HasValue)
                    {
                        if (item.ItemSysNo.Value < 0)
                        {
                            BasketDA.DeleteBasket(item.ItemSysNo.Value * -1);
                        }
                        else
                        {
                            BasketDA.DeleteBasket(item.ItemSysNo.Value);
                        }
                    }
                    item.POSysNo = entity.SysNo;
                    PurchaseOrderDA.CreatePOItem(item);
                }
                scope.Complete();
            }

            if (entity.PurchaseOrderBasicInfo.Source == "VP")
            {   //是Vendo Portal创建的PO单，修改临时表信息
                //发送SSB消息
                VendorPortalPOConfirmMessage msg = new VendorPortalPOConfirmMessage()
                {
                    Status = "2",
                    VPOID = vpPOID,
                    IPPSysNo = entity.SysNo.Value,
                    PMComfirmTime = DateTime.Now
                };
                EventPublisher.Publish<VendorPortalPOConfirmMessage>(msg);
            }
            //日志记录
            //CommonService.WriteLog<POEntity>(entity, " Created Vendor Portal PO ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Create);

            ExternalDomainBroker.CreateLog(" Created Vendor Portal PO "
            , BizEntity.Common.BizLogType.Purchase_Create
            , entity.SysNo.Value
            , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 审核 VendorPortal 高级用户创建的PO单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PurchaseOrderInfo AuditVendorPortalPurchaseOrder(PurchaseOrderInfo entity)
        {
            #region Check

            #endregion
            int auditUserSysNo = entity.PurchaseOrderBasicInfo.AuditUserSysNo.Value;
            string vpPOID = entity.PurchaseOrderBasicInfo.PurchaseOrderID;    //记录VP端的PO ID，用于发送SSB消息。

            entity = CheckPO(entity);               //检查数据信息
            entity.PurchaseOrderBasicInfo.AuditUserSysNo = auditUserSysNo;
            entity = ConfirmVendorPortalPurchaseOrder(entity); //提交数据信息（确认）
            entity.PurchaseOrderBasicInfo.AuditUserSysNo = auditUserSysNo;
            entity = WaitingInStockPO(entity, false);        //审核数据信息

            if (entity.PurchaseOrderBasicInfo.Source == "VP")
            {   //是Vendo Portal创建的PO单，修改临时表信息
                //发送SSB消息
                VendorPortalPOAuditMessage msg = new VendorPortalPOAuditMessage()
                {
                    Status = "4",
                    VPOID = vpPOID,
                    IPPSysNo = entity.SysNo.Value,
                    PMComfirmTime = DateTime.Now
                };
                EventPublisher.Publish<VendorPortalPOAuditMessage>(msg);
            }
            //日志记录
            string logMessage = GetMessageString("PO_Log_UpdateVPPO");
            //CommonService.WriteLog<POEntity>(entity, logMessage, entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(logMessage
           , BizEntity.Common.BizLogType.Purchase_Verify_InStock
           , entity.SysNo.Value
           , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 退回 VendorPortal 创建的PO单
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="retreatType"></param>
        public virtual void RetreatVendorPortalPurchaseOrder(int poSysNo, string retreatType)
        {
            PurchaseOrderInfo poInfo = PurchaseOrderDA.LoadPOMaster(poSysNo);
            //撤回 PO 单（修改状态）
            PurchaseOrderDA.RetreatVendorPortalPurchaseOrder(poSysNo);
            //发送SSB消息:

            VendorPortalPORetreatMessage msg = new VendorPortalPORetreatMessage()
            {
                VPOID = poInfo.PurchaseOrderBasicInfo.PurchaseOrderID,
                IPPSysNo = null,
                PMComfirmTime = null
            };

            if (retreatType == "ComfirmReturn")
            {
                msg.Status = "8";                               //确认被退回
            }
            else if (retreatType == "AuditReturn")
            {
                msg.Status = "9";                               //审核被退回
            }
            EventPublisher.Publish<VendorPortalPORetreatMessage>(msg);
        }

        /// <summary>
        /// 根据商品信息，创建PO Item:
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public virtual PurchaseOrderItemInfo AddPurchaseOrderItemFromProductInfo(PurchaseOrderItemProductInfo productInfo)
        {
            PurchaseOrderItemInfo item = new PurchaseOrderItemInfo();
            List<PurchaseOrderItemInfo> items = new List<PurchaseOrderItemInfo>();
            item = PurchaseOrderDA.AddPurchaseOrderItemByProduct(productInfo);

            if (null != productInfo)
            {
                //2。进行数据的统计:
                decimal getExchangeRate = ExternalDomainBroker.GetExchangeRateBySysNo(productInfo.CurrencySysNo.Value, "8601");
                PurchaseOrderItemInfo getExendPOitem = PurchaseOrderDA.GetExtendPurchaseOrderItemInfo(productInfo.SysNo);
                item.ProductSysNo = productInfo.SysNo;
                item.OrderPrice = Decimal.Round(productInfo.OrderPrice, 2);
                item.PurchaseQty = productInfo.PurchaseQty;
                item.CheckStatus = PurchaseOrdeItemCheckStatus.UnCheck;
                item.UnitCostWithoutTax = Decimal.Round((item.UnitCost.HasValue ? item.UnitCost.Value : 0.0m) / (1 + productInfo.TaxRate), 2);
                //当前成本:
                item.UnitCost = Decimal.Round(item.OrderPrice.Value * getExchangeRate, 2);
                item.CurrentUnitCost = productInfo.UnitCost;
                item.LineReturnedPointCost = item.UnitCost * productInfo.PurchaseQty;
                item.Quantity = 0;
                item.Tax = CalculateProductRate(item);
                //调用IM接口,获取Item价格信息:
                item.JingDongPrice = PurchaseOrderDA.JDPriceByProductSysNo(item.ProductSysNo.Value);
                item.JingDongTax = CalculateJDRate(item);
                item.LastOrderPrice = PurchaseOrderDA.GetLastPriceBySysNo(item.ProductSysNo.Value);
                item.AvailableQty = BasketDA.AvailableQtyByProductSysNO(item.ProductSysNo.Value);
                item.UnActivatyCount = PurchaseOrderDA.GetUnActivatyCount(item.ProductSysNo.Value);
                item.ApportionAddOn = 0;
                item.M1 = BasketDA.M1ByProductSysNO(item.ProductSysNo.Value);
                item.ItemSysNo = productInfo.POItemSysNo;
                item.ReadyQuantity = productInfo.ReadyQuantity;
                ////获取本地货币:
                if (productInfo.CurrencySysNo.HasValue)
                {
                    CurrencyInfo localCurrency = CommmonBizInteract.GetCurrencyInfoBySysNo(productInfo.CurrencySysNo.Value);
                    item.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;
                }
                item.LastAdjustPriceDate = getExendPOitem.LastAdjustPriceDate;
                item.LastInTime = getExendPOitem.LastInTime;
                item.UnActivatyCount = getExendPOitem.UnActivatyCount;
                //////////////////////////////////////////////////////////
                item.AcquireReturnPointType = productInfo.AcquireReturnPointType;
                item.AcquireReturnPoint = productInfo.AcquireReturnPoint;
                item.ReturnCost = 0m;
                item.CompanyCode = productInfo.CompanyCode;

                if (PurchaseOrderDA.IsVirtualStockPurchaseOrderProduct(item.ProductSysNo.Value))
                {
                    item.IsVirtualStockProduct = true;
                }
                //获取Item 的Except Status:
                PurchaseOrderDA.GetPurchaseOrderItemExceptStatus(item);
                if (item != null && item.ProductSysNo != 0)
                {
                    //获取商品的库存信息 ：
                    InitializePOItemStockInfo(item);
                }

            }
            return item;
        }

        /// <summary>
        /// 获取采购单赠品信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public virtual List<PurchaseOrderItemProductInfo> GetPurchaseOrderGiftInfo(List<int> productSysNoList)
        {
            return PurchaseOrderDA.QueryPurchaseOrderGiftList(productSysNoList);
        }

        /// <summary>
        /// 获取采购单附件信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public virtual List<PurchaseOrderItemProductInfo> GetPurchaseOrderAccessoriesInfo(List<int> productSysNoList)
        {
            return PurchaseOrderDA.QueryPurchaseOrderAccessoriesList(productSysNoList);
        }

        /// <summary>
        /// 更新PO单ITEM的批次信息
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdatePurchaeOrderBatchInfo(PurchaseOrderItemInfo info)
        {
            PurchaseOrderDA.UpdatePurchaseOrderBatchInfo(info);
        }

        public PurchaseOrderInfo EditDeletePOItem(PurchaseOrderInfo info)
        {
            CheckDeleteItem(info);
            PurchaseOrderInfo oldentity = PurchaseOrderDA.LoadPOMaster(info.SysNo.Value);
            if (!(oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BizException(string.Format(GetMessageString("PO_UpdateBatchInfo_Invalid"), oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.ToString()));
            }
            foreach (PurchaseOrderItemInfo item in info.POItems)
            {
                PurchaseOrderDA.DeletePOItem(item.ItemSysNo.Value);
            }
            //log;
            PurchaseOrderDA.UpdatePOMasterTotalAmt(info.SysNo.Value);
            return info;
        }

        public PurchaseOrderInfo EditAddOnePOItem(PurchaseOrderInfo info)
        {
            CheckItem(info, 1);
            PurchaseOrderInfo oldentity = PurchaseOrderDA.LoadPOMaster(info.SysNo.Value);
            if (!(oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BizException(string.Format(GetMessageString("PO_UpdateBatchInfo_Invalid"), oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.ToString()));
            }

            //产品线相关检测
            string tExceptionInfo = CheckPoPrductLine(info, PurchaseOrderDA.GetProductSysNoByPOSysNo(info.SysNo.Value));
            if (!string.IsNullOrEmpty(tExceptionInfo))
                throw new BizException(tExceptionInfo);

            foreach (PurchaseOrderItemInfo item in info.POItems)
            {
                //if (info.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.HistoryNegative)
                //{
                //    //调用IM接口，获取Item信息:
                //    item.ReturnCost = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductPriceInfo.UnitCost;
                //}
                item.POSysNo = info.SysNo.Value;
                item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + info.PurchaseOrderBasicInfo.TaxRate.Value), 2);
                ;
                item.CheckStatus = PurchaseOrdeItemCheckStatus.UnCheck;
                PurchaseOrderDA.CreatePOItem(item);
            }
            PurchaseOrderDA.UpdatePOMasterTotalAmt(info.SysNo.Value);
            return info;
        }

        public PurchaseOrderInfo EditUpdateOnePOItem(PurchaseOrderInfo info)
        {
            CheckItem(info, 0);
            PurchaseOrderInfo oldentity = PurchaseOrderDA.LoadPOMaster(info.SysNo.Value);
            if (!(oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BizException(string.Format(GetMessageString("PO_UpdateBatchInfo_Invalid"), oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.ToString()));
            }

            //产品线相关检测
            string tExceptionInfo = CheckPoPrductLine(info, PurchaseOrderDA.GetProductSysNoByPOSysNo(info.SysNo.Value));
            if (!string.IsNullOrEmpty(tExceptionInfo))
                throw new BizException(tExceptionInfo);

            foreach (PurchaseOrderItemInfo item in info.POItems)
            {
                //if (info.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.HistoryNegative)
                //{
                //    //调用IM接口，获取Item信息:
                //    item.ReturnCost = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductPriceInfo.UnitCost;
                //}
                item.POSysNo = info.SysNo;
                item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + info.PurchaseOrderBasicInfo.TaxRate.Value), 2);
                PurchaseOrderDA.UpdatePOItem(item);
            }
            PurchaseOrderDA.UpdatePOMasterTotalAmt(info.SysNo.Value);
            return info;
        }

        public PurchaseOrderInfo EditAllEditOnePOItem(PurchaseOrderInfo info)
        {
            CheckItem(info, 0);
            PurchaseOrderInfo oldentity = PurchaseOrderDA.LoadPOMaster(info.SysNo.Value);
            if (!(oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BizException(string.Format(GetMessageString("PO_UpdateBatchInfo_Invalid"), oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.ToString()));
            }

            //产品线相关检测
            string tExceptionInfo = CheckPoPrductLine(info, PurchaseOrderDA.GetProductSysNoByPOSysNo(info.SysNo.Value));
            if (!string.IsNullOrEmpty(tExceptionInfo))
                throw new BizException(tExceptionInfo);

            int? AcquireReturnPointType = null;
            decimal? AcquireReturnPoint = null;
            foreach (PurchaseOrderItemInfo item in info.POItems)
            {
                if (info.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                {
                    //调用IM接口，获取Item信息:
                    item.ReturnCost = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductPriceInfo.UnitCost;
                }
                item.POSysNo = info.SysNo;
                item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + info.PurchaseOrderBasicInfo.TaxRate.Value), 2);
                AcquireReturnPointType = item.AcquireReturnPointType;
                AcquireReturnPoint = item.AcquireReturnPoint;
                PurchaseOrderDA.UpdatePOItem(item);
            }

            PurchaseOrderDA.UpdateAllAcquireReturnInfo(info.SysNo.Value, AcquireReturnPointType, AcquireReturnPoint);
            PurchaseOrderDA.UpdatePOMasterTotalAmt(info.SysNo.Value);
            return info;
        }

        public PurchaseOrderInfo EditAllAddOnePOItem(PurchaseOrderInfo info)
        {
            CheckItem(info, 1);
            PurchaseOrderInfo oldentity = PurchaseOrderDA.LoadPOMaster(info.SysNo.Value);
            if (!(oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created || oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BizException(string.Format(GetMessageString("PO_UpdateBatchInfo_Invalid"), oldentity.PurchaseOrderBasicInfo.PurchaseOrderStatus.ToString()));
            }

            //产品线相关检测
            string tExceptionInfo = CheckPoPrductLine(info, PurchaseOrderDA.GetProductSysNoByPOSysNo(info.SysNo.Value));
            if (!string.IsNullOrEmpty(tExceptionInfo))
                throw new BizException(tExceptionInfo);

            int? AcquireReturnPointType = null;
            decimal? AcquireReturnPoint = null;
            foreach (PurchaseOrderItemInfo item in info.POItems)
            {
                if (info.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                {
                    //调用IM接口，获取Item信息:
                    item.ReturnCost = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductPriceInfo.UnitCost;
                }
                item.POSysNo = info.SysNo;
                item.UnitCostWithoutTax = Decimal.Round(item.UnitCost.Value / (1 + info.PurchaseOrderBasicInfo.TaxRate.Value), 2);
                item.CheckStatus = PurchaseOrdeItemCheckStatus.UnCheck;
                AcquireReturnPointType = item.AcquireReturnPointType;
                AcquireReturnPoint = item.AcquireReturnPoint;
                PurchaseOrderDA.CreatePOItem(item);
            }
            PurchaseOrderDA.UpdateAllAcquireReturnInfo(info.SysNo.Value, AcquireReturnPointType, AcquireReturnPoint);
            PurchaseOrderDA.UpdatePOMasterTotalAmt(info.SysNo.Value);
            return info;
        }

        private void CheckDeleteItem(PurchaseOrderInfo entity)
        {
            List<PurchaseOrderItemInfo> poitemList = PurchaseOrderDA.LoadPOItems(entity.SysNo.Value);
            if (entity.POItems.Count >= poitemList.Count)
            {
                throw new BizException(GetMessageString("PO_DeleteProduct_Invalid"));
            }
        }

        private void CheckItem(PurchaseOrderInfo entity, int Add)
        {
            string excestr = string.Empty;
            StringBuilder exceptinIno = new StringBuilder();
            Dictionary<string, string> preCheckCreatPo = new Dictionary<string, string>();
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_ProductPrice"), CheckProductInitPrice(entity));
            if (entity.PurchaseOrderBasicInfo.MemoInfo.Note == "EditAddOnePOItem" || entity.PurchaseOrderBasicInfo.MemoInfo.Note == "EditAllAddOnePOItem")
            {
                preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_IsProductAdded"), CheckRepeatProduct(entity));
            }
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_POTypeMatchItemType"), CheckItemIsConsin(entity));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_ItemQty"), CheckNormalPO(entity));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_Product"), CheckItemProduct(entity, Add));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NegativeQty"), CheckNegativePOItemNumber(entity));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NormalProductPrice"), CheckItemPrice(entity.POItems));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_Qty"), CheckItemNumber(entity));
            StringBuilder exceptionTable = new StringBuilder();
            int i = 0;
            foreach (var item in preCheckCreatPo)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    i++;
                    exceptionTable.Append(i.ToString() + ". " + item.Key + " : " + GetCheckResult(item.Value) + Environment.NewLine);
                }
            }
            if (i == 0)
            {
                excestr = string.Empty;
                return;
            }
            throw new BizException("提示信息:" + Environment.NewLine + exceptionTable.ToString());
        }

        /// <summary>
        /// 调整批次库存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sysNo"></param>
        private void SetInvotryInfo(PurchaseOrderInfo entity, string sysNo)
        {
            string xml = @"<Message>
                              <Header>
                                <NameSpace>http://soa.newegg.com/CustomerProfile</NameSpace>
                                <Action>#Audit#</Action>
                                <Version>V10</Version>
                                <Type>NPO</Type>
                                <CompanyCode>#CompanyCode#</CompanyCode>      
                                <Tag>POInstock</Tag>
                                <Language>zh-CN</Language>
                                <From>IPP</From>
                                <GlobalBusinessType>Listing</GlobalBusinessType>
                                <StoreCompanyCode>#StoreCompanyCode#</StoreCompanyCode>   
                                <TransactionCode>05-001-0-001</TransactionCode>
                              </Header>
                                <Body>
                                   <Number>#Number#</Number> 
                                   <InUser>#InUser#</InUser>  
                                   ######
                                </Body>
                            </Message>";
            string batch = @"<ItemBatchInfo>
                                       <BatchNumber>#BatchNumber#</BatchNumber>  
                                       <Status></Status>
                                       <ProductNumber>#ProductNumber#</ProductNumber> 
                                       <ExpDate></ExpDate>
                                       <MfgDate></MfgDate>
                                       <LotNo></LotNo>
                                       <Stocks>
                                          <Stock>
                                          <Quantity>#Quantity#</Quantity>            
                                          <AllocatedQty>#Quantity#</AllocatedQty>  
                                          <WarehouseNumber>#WarehouseNumber#</WarehouseNumber> 
                                          </Stock>
                                       </Stocks>
                                   </ItemBatchInfo>";
            string newxml = xml.Replace("#InUser#", entity.PurchaseOrderBasicInfo.AuditUserSysNo.ToString())
                             .Replace("#Number#", entity.SysNo.ToString())
                             .Replace("#StoreCompanyCode#", entity.CompanyCode)
                             .Replace("#CompanyCode#", entity.CompanyCode);
            if (sysNo == "")
            {
                newxml = newxml.Replace("#Audit#", "Audit");
            }
            else
            {
                newxml = newxml.Replace("#Audit#", "CancelAudit");
            }
            StringBuilder strb = new StringBuilder();

            foreach (var item in entity.POItems)
            {

                if (!PurchaseOrderDA.IsBatchProduct(item))
                {
                    continue;
                }

                string[] strs = item.BatchInfo.Split(new char[] { ';' });
                foreach (string str in strs)
                {
                    string[] strChild = str.Split(new char[] { ':' });
                    if (strChild.Length == 3)
                    {
                        strb.Append(batch.Replace("#WarehouseNumber#", strChild[1])
                             .Replace("#Quantity#", sysNo + strChild[2])
                             .Replace("#ProductNumber#", item.ProductSysNo.ToString())
                             .Replace("#BatchNumber#", strChild[0])
                             );
                    }
                }
            }

            ExternalDomainBroker.AdjustBatchNumberInventory(newxml.Replace("######", strb.ToString()));
        }

        /// <summary>
        /// 设置PO库存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="symber"></param>
        //private void SetInvotryInfo(PurchaseOrderInfo poInfo, string sysNo)
        //{
        //    try
        //    {
        //        //调用Inventory接口，设置负库存,m_PODAL.SetNavigateInventory(entity, sysNo);
        //        //调用Inventory接口，调整库存（负采购单）: m_PODAL.SetNavigateInventory(entity, "");
        //        InventoryAdjustContractInfo negativeContractInfo = new InventoryAdjustContractInfo()
        //        {
        //            SourceActionName = sysNo == "" ? InventoryAdjustSourceAction.Audit : InventoryAdjustSourceAction.CancelAudit,
        //            ReferenceSysNo = poInfo.SysNo.Value.ToString(),
        //            SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order
        //        };
        //        negativeContractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();

        //        foreach (var item in poInfo.POItems)
        //        {
        //            if (!PurchaseOrderDA.IsBatchProduct(item))
        //            {
        //                continue;
        //            }

        //            InventoryAdjustItemInfo itemInfo = new InventoryAdjustItemInfo();

        //            string[] strs = item.BatchInfo.Split(new char[] { ';' });
        //            foreach (string str in strs)
        //            {
        //                string[] strChild = str.Split(new char[] { ':' });
        //                if (strChild.Length == 3)
        //                {
        //                    itemInfo.ProductSysNo = item.ProductSysNo.Value;
        //                    itemInfo.AdjustQuantity = !string.IsNullOrEmpty(strChild[2]) ? Convert.ToInt32(strChild[2]) : 0;
        //                    itemInfo.StockSysNo = !string.IsNullOrEmpty(strChild[1]) ? Convert.ToInt32(strChild[1]) : 0;                          
        //                    //strb.Append(batch.Replace("#WarehouseNumber#", strChild[1])
        //                    //     .Replace("#Quantity#", symber + strChild[2])
        //                    //     .Replace("#ProductNumber#", item.ProductSysNo.ToString())
        //                    //     .Replace("#BatchNumber#", strChild[0])
        //                    //     );
        //                }
        //            }
        //        }
        //        //此处逻辑有问题
        //        ExternalDomainBroker.AdjustProductInventory(negativeContractInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BizException(ex.InnerException.Message);
        //    }
        //}

        private string PreCheckPoCheck(PurchaseOrderInfo entity)
        {
            Dictionary<string, string> dictonary = CreateCheckDictionary(entity);

            return BuildCheckMsg(dictonary, false);
        }

        /// <summary>
        /// 根据消息字典创建消息字符串
        /// </summary>
        /// <param name="dictonary"></param>
        /// <param name="IsValidateItemValue">是否跳过值为空的Item</param>
        /// <returns></returns>
        private string BuildCheckMsg(Dictionary<string, string> dictonary, bool IsValidateItemValue)
        {
            StringBuilder messageInfo = new StringBuilder();
            messageInfo.Append("<table>");
            //messageInfo.Append(GetMessageString("PO_CheckMsg_ResultTitle") + Environment.NewLine);
            int i = 0;
            foreach (var item in dictonary)
            {
                if (IsValidateItemValue && string.IsNullOrEmpty(item.Value))
                    continue;
                i++;
                //messageInfo.Append(i.ToString() + ".  " + item.Key + ":  " + item.Value);
                //messageInfo.Append("<tr><td style='padding-right: 12px;'>" + i.ToString() + ".&nbsp;</td><td style='padding-right: 12px;'>" + item.Key + ":&nbsp;</td><td>" + (string.IsNullOrEmpty(item.Value) ? GetMessageString("PO_CheckMsg_Success") + Environment.NewLine : item.Value + "</td></tr>"));
                string content = string.IsNullOrEmpty(item.Value) ? GetMessageString("PO_CheckMsg_Success") : item.Value;
                messageInfo.AppendFormat("<tr><td>[{0}]->[{1}]->{2}</td></tr>", i.ToString("00"), item.Key, content);
            }
            if (IsValidateItemValue && i == 0)
                return string.Empty;
            messageInfo.Append("</table>");
            return messageInfo.ToString();
        }

        private string BuildCheckMsg4EC(Dictionary<string, string> dictonary, bool IsValidateItemValue)
        {
            StringBuilder messageInfo = new StringBuilder();

            messageInfo.Append(GetMessageString("PO_CheckMsg_ResultTitle") + Environment.NewLine);
            int i = 0;
            foreach (var item in dictonary)
            {
                if (IsValidateItemValue && string.IsNullOrEmpty(item.Value))
                    continue;
                i++;

                //messageInfo.Append("<tr><td style='padding-right: 12px;'>" + i.ToString() + ".&nbsp;</td><td style='padding-right: 12px;'>" + item.Key + ":&nbsp;</td><td>" + (string.IsNullOrEmpty(item.Value) ? GetMessageString("PO_CheckMsg_Success") + Environment.NewLine : item.Value + "</td></tr>"));

                messageInfo.AppendLine(string.Format("{0} {1} {2}", i, item.Key, string.IsNullOrEmpty(item.Value) ? GetMessageString("PO_CheckMsg_Success") + Environment.NewLine : item.Value));

            }
            if (IsValidateItemValue && i == 0)
                return string.Empty;

            return messageInfo.ToString();

        }

        private PurchaseOrderInfo GetReturnPoint(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo.HasValue)
            {
                //TODO：调用EIMS接口，查询返点信息:
                //if (Convert.ToInt32(entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo.Value) > 0)
                //{
                //    ReturnPointList returnPoint = POService.GetReturnPointNameBySysNo(new ReturnPointCondition()
                //    {
                //        SysNo = Convert.ToInt32(entity.PM_ReturnPointSysNo),
                //    });
                //    if (returnPoint != null)
                //    {
                //        entity.ReturnPointName = returnPoint.ReturnPointName;
                //        entity.RemnantReturnPoint = returnPoint.RemnantReturnPoint.ToString(AppConst.DECIMAL_FORMAT);
                //    }
                //}
                //else
                //{
                //    entity.PM_ReturnPointSysNo = "";
                //}

                #region 获取 Category信息

                //if (entity.ReturnPointC3SysNo != null && Convert.ToInt32(entity.ReturnPointC3SysNo) > 0)
                //{
                //    var category = QueryProviderFactory.GetQueryProvider<IQueryCommon>().QueryCategory(Convert.ToInt32(entity.ReturnPointC3SysNo));
                //    if (category != null)
                //    {
                //        entity.Category = new
                //        {
                //            c3SysNo = Convert.ToInt32(entity.ReturnPointC3SysNo),
                //            c2SysNo = category.Category2Sysno,
                //            c1SysNo = category.Category1Sysno,

                //            c2List = QueryProviderFactory.GetQueryProvider<IQueryCommon>().QueryCategory2(category.Category1Sysno),
                //            c3List = QueryProviderFactory.GetQueryProvider<IQueryCommon>().QueryCategory3(category.Category2Sysno),
                //        };
                //    }
                //}

                #endregion
            }

            return entity;
        }

        #region [创建PO单，Check操作]

        private string PreCheckCreatePO(PurchaseOrderInfo poInfo)
        {
            StringBuilder exceptinIno = new StringBuilder();

            Dictionary<string, string> preCheckCreatPo = new Dictionary<string, string>();

            if (poInfo == null)
            {
                throw new BizException(GetMessageString("PO_CheckMsg_POEntityNull"));
            }
            if (poInfo.VendorInfo == null || !poInfo.VendorInfo.SysNo.HasValue || !poInfo.VendorInfo.SysNo.HasValue)
            {
                throw new BizException(GetMessageString("PO_CheckMsg_VendorNull"));
            }
            if (poInfo.POItems == null || poInfo.POItems.Count <= 0)
            {
                throw new BizException(GetMessageString("PO_CheckMsg_ProductNull"));
            }
            if (poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                //验证是否是无效的采购仓库
                string invalidStocks = AppSettingManager.GetSetting("PO", "Po_CreatePO_InvaildStocks");
                if (!string.IsNullOrEmpty(invalidStocks))
                {
                    int stockSysNo = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value;
                    string stockStr = stockSysNo.ToString();
                    //有中转仓
                    if (stockStr.Length > 2)
                    {
                        stockSysNo = Int32.Parse(stockStr.Substring(2));
                    }
                    if (invalidStocks.Split(',').Contains(stockSysNo.ToString()))
                    {
                        throw new BizException(GetMessageString("PO_CheckMsg_InValidPurchaseOrderStock"));
                    }
                }

            }
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_ProductPrice"), CheckProductInitPrice(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_POConsignMatchVendorConsign"), CheckVendorIsConsol(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_ETA"), CheckETA(poInfo, PurchaseOrderActionType.Check));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_AddProduct"), CheckRepeatProduct(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_EIMSAmt"), CheckReturnPoint(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_SettleCompanyNull"), CheckPayCompany(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_InStock"), CheckPoInStock(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_PMNull"), CheckPoBelongToPM(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_POTypeNull"), CheckPOType(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NegativeInStock"), CheckReturnPOValid(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_MemoInputCount"), CheckMemo(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_POTypeMatchItemType"), CheckItemIsConsin(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_Item"), CheckNormalPO(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_Product"), CheckItemProduct(poInfo, 0));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_VendorInfo"), CheckVendorInfo(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NegativePOSysNo"), CheckNegativePO(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_ClosedPO"), CheckPOCloseStatus(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NegativeQty"), CheckNegativePOItemNumber(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_Qty"), CheckItemNumber(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_NormalProductPrice"), CheckItemPrice(poInfo.POItems));
            preCheckCreatPo.Add(GetMessageString("PO_CheckItem_CostInventory"), CheckCostInventory(poInfo));
            preCheckCreatPo.Add(GetMessageString("PO_CheckMsg_AudoMailLength"), CheckAutoSendMailLength(poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress));

            //return BuildCheckMsg(preCheckCreatPo, true);
            return BuildCheckMsg4EC(preCheckCreatPo, true);
        }
        /// <summary>
        /// 负采购成本库存检查
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string CheckCostInventory(PurchaseOrderInfo info)
        {
            if (info.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                List<string> costErr = new List<string>();
                foreach (var item in info.POItems)
                {
                    if (PurchaseOrderDA.GetCostQuantity(item.ProductSysNo.Value, item.OrderPrice.Value,info.PurchaseOrderBasicInfo.StockInfo.SysNo.Value) < Math.Abs(item.PurchaseQty.Value))
                    {
                        costErr.Add(item.ProductID);
                    }
                }
                if (costErr.Count > 0)
                {
                    return string.Format("商品编号{0}的成本库存不足，无法扣减", costErr.ToListString());
                }
            }
            return string.Empty; ;
        }

        /// <summary>
        /// 验证商品 Item是否属于PO单归属PM, 商品数量不能大于80个
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        private string CheckItemProduct(PurchaseOrderInfo poInfo, int addCount)
        {
            StringBuilder exceptions = new StringBuilder();

            #region CRL21776 Jack.W.Wang 本代码已停用，更换为产品线验证
            ////Item不属于PO单归属PM，无法创建PO单

            ////调用IM接口， 获取Product PM List:
            //List<ProductManagerInfo> produPms = new List<ProductManagerInfo>();
            //List<ProductInfo> productInfos = ExternalDomainBroker.GetProductList(poInfo.POItems.Select(x => x.ProductSysNo.Value).ToList<int>());
            //productInfos.ForEach(x =>
            //{
            //    produPms.Add(x.ProductBasicInfo.ProductManager);
            //});

            //var result = from p in productInfos
            //             where p.ProductBasicInfo.ProductManager.UserInfo.SysNo != poInfo.PurchaseOrderBasicInfo.ProductManager.SysNo && !ConsignSettlementDA.GetBackUpPMList(p.ProductBasicInfo.ProductManager.UserInfo.SysNo.Value, poInfo.CompanyCode).Contains(poInfo.PurchaseOrderBasicInfo.ProductManager.SysNo.Value)
            //             select p.ProductID;
            //if (result.Count() > 0)
            //{
            //    exceptions.Append(string.Format(GetMessageString("PO_CheckMsg_PMInvalid"), result.ToList().ToListString(), "") + Environment.NewLine);
            //}
            #endregion

            //商品数量不能大于80个
            int poItemCountNumber = 80;
            if (!string.IsNullOrEmpty(AppSettingManager.GetSetting("PO", "PoItemCountNumber")))
            {
                poItemCountNumber = int.Parse(AppSettingManager.GetSetting("PO", "PoItemCountNumber"));
            }
            if (poInfo.POItems.Count + addCount > poItemCountNumber)
            {
                exceptions.Append(string.Format(GetMessageString("PO_CheckMsg_MaxProductCount") + Environment.NewLine, poItemCountNumber.ToString()));
            }
            return exceptions.ToString();
        }

        /// <summary>
        /// 商品价格=999999（初始值），则无法添加进入采购单
        /// </summary>
        /// <param name="poInfo"></param>
        /// <returns></returns>
        private string CheckProductInitPrice(PurchaseOrderInfo poInfo)
        {
            string message = string.Empty;
            //调用IM接口，获取产品的CurrentPrice:
            List<ProductPriceInfo> productprice = new List<ProductPriceInfo>();

            List<ProductInfo> productInfo = ExternalDomainBroker.GetProductList(poInfo.POItems.Select(x => x.ProductSysNo.Value).ToList<int>());
            productInfo.ForEach(x =>
            {
                productprice.Add(x.ProductPriceInfo);
            });

            var checkresult = from i in productprice
                              where i.CurrentPrice == 999999
                              select i.BasicPrice;
            if (checkresult.Count() > 0)
            {
                message = string.Format(string.Format(GetMessageString("PO_CheckMsg_Price999999") + Environment.NewLine, checkresult.ToList().ToListString()));
            }
            return message;
        }

        /// <summary>
        /// PO单帐期属性与供应商帐期属性一致验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckVendorIsConsol(PurchaseOrderInfo poInfo)
        {
            //PO单帐期属性与供应商帐期属性类型不同
            VendorInfo vendorEntity = VendorProcessor.LoadVendorInfo(poInfo.VendorInfo.SysNo.Value);
            if (vendorEntity == null)
            {
                throw new BizException(GetMessageString("PO_CheckMsg_VendorInfoInvalid"));
            }
            if ((int)vendorEntity.VendorBasicInfo.ConsignFlag != (int)poInfo.PurchaseOrderBasicInfo.ConsignFlag)
            {
                return GetMessageString("PO_CheckMsg_PayTypeNotSame") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查预计到货时间ETA
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        public string CheckETA(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            if (actionType == PurchaseOrderActionType.Check)
            {
                if (entity.PurchaseOrderBasicInfo.ETATimeInfo != null)
                {
                    if (entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue && entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.CompareTo(DateTime.Today) < 0)
                    {
                        return GetMessageString("PO_CheckMsg_ETATimeLessThanCurrentTime") + Environment.NewLine;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查预计到货时间ETA是否为空
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string CheckETAIsNull(PurchaseOrderInfo entity)
        {
            if (!entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue)
            {
                return GetMessageString("PO_CheckMsg_ETATimeNull");
            }
            return string.Empty;
        }

        /// <summary>
        /// 提交审核时CheckResult不能为空
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckResultIsNull(PurchaseOrderInfo entity)
        {
            PurchaseOrderInfo dbentity = PurchaseOrderDA.LoadPOMaster(entity.SysNo.Value);
            if (string.IsNullOrEmpty(dbentity.PurchaseOrderBasicInfo.CheckResult))
            {
                return GetMessageString("PO_CheckMsg_SubmitResultNull") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查申请理由不能为空
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPMRequestMemoIsNull(PurchaseOrderInfo entity)
        {
            if (entity != null && string.IsNullOrEmpty(entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo))
            {
                return GetMessageString("PO_CheckMsg_ReasonNull") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查只有待审核、已创建、已退回、已作废、系统作废的PO单可以提交审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckStateCanSubmit(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Returned
              && entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Created
              && entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Abandoned
              && entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.AutoAbandoned
              && entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingAudit)
            {
                return GetMessageString("PO_CheckMsg_POSubmit") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 验证负Po商品XXX批次退货信息数量A与采购数量B不符
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPOItemNumber(PurchaseOrderInfo entity)
        {
            string msg = string.Empty;
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                foreach (PurchaseOrderItemInfo poItem in entity.POItems)
                {
                    //调用IM接口，判断ITEM是否是Batch item:

                    if (!PurchaseOrderDA.IsBatchProduct(poItem))
                    {
                        continue;
                    }
                    int? batchNumber = GetCountNumber(poItem.BatchInfo);

                    if (Math.Abs(poItem.PurchaseQty.Value) != Math.Abs(batchNumber.Value))
                    {
                        msg += string.Format(GetMessageString("PO_CheckMsg_BatchInfoNotMatchQty") + Environment.NewLine, poItem.ProductID, batchNumber, poItem.PurchaseQty);
                    }
                }
            }
            return msg;
        }

        private int? GetCountNumber(string p)
        {
            if (string.IsNullOrEmpty(p))
            {
                return 0;
            }
            char[] charl = new char[] { ';' };
            string[] listString = p.Split(charl);
            int countNumber = 0;
            foreach (string str in listString)
            {
                try
                {
                    string[] strNumber = str.Split(new char[] { ':' });
                    if (strNumber.Length == 3)
                    {
                        countNumber += int.Parse(strNumber[2]);
                    }
                }
                catch
                {
                    throw new BizException(GetMessageString("PO_CheckMsg_BatchInfoError"));
                }
            }
            return countNumber;
        }

        /// <summary>
        /// 如果是负Po需要验证该是否存在批次信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        private string CheckPOItemHasBatchInfo(PurchaseOrderInfo entity)
        {
            string msg = string.Empty;
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                //TODO:调用IM接口，判断ITEM是否是Batch item:
                //foreach (PurchaseOrderItemInfo poItem in entity.POItems)
                //{
                //    if (!m_PODAL.IsBatchProduct(poItem))
                //    {
                //        continue;
                //    }
                //    if (string.IsNullOrEmpty(poItem.BatchInfo))
                //    {
                //        msg += poItem.ProductID + ",";
                //    }
                //}
            }
            if (msg.Length > 2)
            {
                return string.Format(GetMessageString("PO_CheckMsg_BatchInfoMissing"), msg.ToString().Remove(msg.ToString().Length - 1));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 采购的商品是否存在重复item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckRepeatProduct(PurchaseOrderInfo entity)
        {
            var repeatItem = from i in entity.POItems
                             group i by new
                             {
                                 i.ProductSysNo,
                                 i.ProductID
                             } into g
                             where g.Count() > 1
                             select g;
            if (repeatItem.Count() > 0)
            {
                string result = string.Empty;
                foreach (var item in repeatItem)
                {
                    result += item.Key.ProductID + ",";
                }
                result = result.TrimEnd(',');
                return string.Format(GetMessageString("PO_CheckMsg_SameProduct") + Environment.NewLine, result);
            }
            return string.Empty;
        }

        /// <summary>
        ///  校验返点金额
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string CheckReturnPoint(PurchaseOrderInfo entity)
        {
            // 12. 如果选择了返点公司，就必须做下面的检查
            if (entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo.HasValue && entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo.Value > 0)
            {
                //必须返点金额
                if (!entity.PurchaseOrderBasicInfo.UsingReturnPoint.HasValue || entity.PurchaseOrderBasicInfo.UsingReturnPoint.Value <= 0)
                {
                    return GetMessageString("PO_CheckMsg_EIMSAmtRequired" + Environment.NewLine);
                }

                //使用返点金额不能超出该返点提供的金额
                string department = "";
                int pmSysNo = 0;
                decimal remnantReturnPoint = 0;
                bool returnPontEntity = true;//TODO:调用EIMS接口:
                //EIMSMgmtService.GetReturnPoint(entity.PM_ReturnPointSysNo.Value, out department, out pmSysNo, out remnantReturnPoint);

                if (returnPontEntity == false)
                {
                    // "使用了无效的返点,操作失败！"
                    return GetMessageString("PO_CheckMsg_EIMSAmtInvalid") + Environment.NewLine;
                }

                if (entity.PurchaseOrderBasicInfo.UsingReturnPoint.Value > remnantReturnPoint) //需要调用服务检查
                {
                    return GetMessageString("PO_CheckMsg_EIMSAmtOverflow") + Environment.NewLine;
                }

                //返点与对应的PM不符,不能通过
                if (!(((department == null || department == "" || department == "PM") && (entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value == pmSysNo)) || (department == "MKT")))
                {
                    return GetMessageString("PO_CheckMsg_EIMSAmtPMInvalid") + Environment.NewLine;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 付款结算公司不能为空
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPayCompany(PurchaseOrderInfo entity)
        {
            //if (!entity.VendorInfo.VendorBasicInfo.PaySettleCompany.HasValue)
            //{
            //    return GetMessageString("PO_CheckMsg_PaySettleCompany") + Environment.NewLine;
            //}
            return string.Empty;
        }

        /// <summary>
        /// 入库仓验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPoInStock(PurchaseOrderInfo entity)
        {
            //  入库仓库验证！
            if (!entity.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                //入库仓库不能为空
                return GetMessageString("PO_CheckMsg_InStockNull") + Environment.NewLine;
            }
            //采购入库仓库目前只能选择上海、北京、广州,成都仓库其中的一个，或中转仓

            //TODO:调用Inventory接口，获得北京，上海，广州,成都仓库的SysNo
            //if (!InventoryMgmtService.GetStockSysNo().Contains((int)entity.StockSysNo))
            //{
            //   return GetMessageString("PO_CheckMsg_InStockLimit")+ Environment.NewLine;
            //}
            return string.Empty;
        }

        /// <summary>
        /// 采购单归属PM不能为空
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPoBelongToPM(PurchaseOrderInfo entity)
        {
            if (!entity.PurchaseOrderBasicInfo.ProductManager.SysNo.HasValue)
            {
                return GetMessageString("PO_CheckMsg_PMRequired") + Environment.NewLine;
            }

            return string.Empty;
        }

        /// <summary>
        /// 采购单的类型必须选择
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPOType(PurchaseOrderInfo entity)
        {
            if (!entity.PurchaseOrderBasicInfo.PurchaseOrderType.HasValue)
            {
                return GetMessageString("PO_CheckMsg_POTypeRequired") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 负采购，入库仓不可选择经中转到**仓
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckReturnPOValid(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                //负采购不支持中转:
                if (entity.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue && entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value > 5000)
                {
                    return GetMessageString("PO_CheckMsg_ITStockInvalid") + Environment.NewLine;
                }
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 备忘录的输入数据不能超过1000个字符
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckMemo(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.MemoInfo != null)
            {
                if (string.IsNullOrEmpty(entity.PurchaseOrderBasicInfo.MemoInfo.Memo))
                {
                    return string.Empty;
                }
                if (entity.PurchaseOrderBasicInfo.MemoInfo.Memo.Length > 1000)
                {
                    return GetMessageString("PO_CheckMsg_MemoLengthOverflow") + Environment.NewLine;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 判断PO类型与Item类型是否一致[同为代销或非代销]
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckItemIsConsin(PurchaseOrderInfo entity)
        {
            string msg = string.Empty;
            List<ProductInfo> productConsigns = new List<ProductInfo>();
            //调用IM接口，获得商品的代销类型：
            productConsigns = ExternalDomainBroker.GetProductList(entity.POItems.Select(x => x.ProductSysNo.Value).ToList());
            var result = from p in productConsigns
                         where (int)p.ProductConsignFlag != (int)entity.PurchaseOrderBasicInfo.ConsignFlag
                         select p.ProductID;
            if (result.Count() > 0)
            {
                msg = string.Format(GetMessageString("PO_CheckMsg_TypeAndProductTypeMismatch") + Environment.NewLine, result.ToList().ToListString());
            }
            return msg;
        }

        /// <summary>
        /// 正常采购单的采购数据要大于或等于0，负采购单的采购数据要小于或等于0
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckNormalPO(PurchaseOrderInfo entity)
        {
            bool isCheckPassed = true;
            entity.POItems.ForEach(x =>
            {
                if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal && x.PurchaseQty < 0)
                {
                    isCheckPassed = false;
                }
                if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative && x.PurchaseQty >= 0)
                {
                    isCheckPassed = false;
                }

            });
            return isCheckPassed ? string.Empty : GetMessageString("PO_CheckMsg_QtyCheck") + Environment.NewLine;
        }

        /// <summary>
        /// 供应商信息验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckVendorInfo(PurchaseOrderInfo entity)
        {
            // 如果供应商是Invalid状态，不允许新建PO单
            VendorInfo getVendorInfo = VendorProcessor.LoadVendorInfo(entity.VendorInfo.SysNo.Value);
            if (null != getVendorInfo)
            {
                if (getVendorInfo.VendorBasicInfo.HoldMark.HasValue && getVendorInfo.VendorBasicInfo.HoldMark.Value)
                {
                    return GetMessageString("PO_CheckMsg_VendorHolded") + Environment.NewLine;
                }

                // 如果供应商是Invalid状态，不允许新建PO单
                if (getVendorInfo.VendorBasicInfo.VendorStatus != VendorStatus.Available)
                {
                    return GetMessageString("PO_CheckMsg_VendorUnavailable") + Environment.NewLine;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// PO单为负采购单，应包含对应的正采购编号
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckNegativePO(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType.Value == PurchaseOrderType.Negative && string.IsNullOrEmpty(entity.PurchaseOrderBasicInfo.MemoInfo.Memo))
            {
                return GetMessageString("PO_CheckMsg_NegativePOCheckInput") + Environment.NewLine;
            }
            return string.Empty;
        }

        /// <summary>
        /// 负PO审核
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        public string CheckNegativePO(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                IsChangeTPStaus = true;
                return GetMessageString("PO_CheckMsg_NegativePOAudit") + Environment.NewLine;
            }

            return string.Empty;
        }

        /// <summary>
        /// 负PO供应商应付款校验
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckNegativePOVenderAmt(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                decimal amtPo = entity.POItems.Sum(item => item.PurchaseQty.Value * item.OrderPrice.Value);
                //调用Invoice接口，获取该负PO的供应商财务应付款，NOTE:查询Sql
                decimal vendorPayBalance = ExternalDomainBroker.GetVendorPayBalanceByVendorSysNo(entity.VendorInfo.SysNo.Value);
                if (vendorPayBalance == 0 || (vendorPayBalance > 0 && vendorPayBalance < Math.Abs(amtPo)))
                {
                    return GetMessageString("PO_CheckMsg_PayAmtLessThanZero") + Environment.NewLine;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 系统已关闭的单据类型检查:调价单检查,历史负采购单检查,临时代销单据检查
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckPOCloseStatus(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Adjust)
            {
                return GetMessageString("PO_Update_TypeError1") + Environment.NewLine;
            }

            //// 9. 系统已经关闭历史采购单功能!
            //if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.HistoryNegative)
            //{
            //    return GetMessageString("PO_Update_TypeError2") + Environment.NewLine;
            //}

            //// 系统已经关闭临时代销功能!
            //if (entity.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.TempConsign)
            //{
            //    return GetMessageString("PO_Update_TypeError3") + Environment.NewLine;
            //}
            return string.Empty;
        }

        /// <summary>
        /// 负采购时Item数量不能大于分仓对应的可用库存加代销库存数量！
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckNegativePOItemNumber(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                foreach (PurchaseOrderItemInfo item in entity.POItems)
                {
                    //TODO:调用Inventory接口,获取库存，判断PO负采购时分仓库存是否大于采购数量
                    //if (!m_PODAL.CheckReturnPOValid(entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value, item.ProductSysNo.Value, item.PurchaseQty.Value))
                    //{
                    //return GetMessageString("PO_CheckMsg_NegativeProductCount");
                    //}
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查采购商品数量
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckItemNumber(PurchaseOrderInfo entity)
        {
            StringBuilder str = new StringBuilder();
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                foreach (PurchaseOrderItemInfo item in entity.POItems)
                {
                    if (item.PurchaseQty >= 0)
                    {
                        str.Append(string.Format(GetMessageString("PO_CheckMsg_ProductCountLargerThanZero") + Environment.NewLine, item.ProductID));
                    }
                }
            }
            else
            {
                foreach (PurchaseOrderItemInfo item in entity.POItems)
                {
                    if (item.PurchaseQty < 0)
                    {
                        str.Append(string.Format(GetMessageString("PO_CheckMsg_ProductCountLessThanZero") + Environment.NewLine, item.ProductID));
                    }
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// 验证正常采购价格
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string CheckItemPrice(List<PurchaseOrderItemInfo> list)
        {
            string msg = string.Empty;
            List<int> productSysNoList = list.Select(x => x.ProductSysNo.Value).ToList();
            List<ProductInfo> getProductList = ExternalDomainBroker.GetProductList(productSysNoList);
            //List<ProductPriceInfo> productInfo = getProductList.Select(p => p.ProductPriceInfo).ToList();
            //var result = from p in productInfo
            //             where p.VirtualPrice <= 0
            //             select p.BasicPrice;

            List<string> productIds = new List<string>();
            foreach (var item in getProductList)
            {
                if (item.ProductPriceInfo == null || item.ProductPriceInfo.VirtualPrice < 0)
                {
                    productIds.Add(item.ProductID);
                }
            }
            if (productIds.Count() > 0)
            {
                msg = string.Format(GetMessageString("PO_CheckMsg_OrderPriceRequired") + Environment.NewLine, productIds.ToListString());
            }
            return msg;
        }

        /// <summary>
        /// 验证自动邮件收件人列表长度
        /// </summary>
        /// <param name="autoSendMail"></param>
        /// <returns></returns>
        private string CheckAutoSendMailLength(string autoSendMail)
        {
            StringBuilder execStr = new StringBuilder();
            if (autoSendMail != null && autoSendMail.Length > 500)
            {
                execStr.Append(GetMessageString("PO_CheckMsg_AutoMailMaxCount") + Environment.NewLine);
            }
            return execStr.ToString();
        }

        /// <summary>
        /// 是否启动SSB
        /// </summary>
        /// <returns></returns>
        private bool IsSSBEnabled()
        {
            //获取系统配置：POOffLine
            var status = ExternalDomainBroker.GetSystemConfiguration("POOffLine", "8601");
            var configValue = AppSettingManager.GetSetting("PO", "EnablePOOfflineDebugging");
            var isDebuggingEnabled = Convert.ToBoolean(configValue);
            return status == "Y" || isDebuggingEnabled;
        }

        /// <summary>
        /// CheckStockSysNo
        /// </summary>
        /// <returns>通过返回true,否则返回false</returns>
        private bool CheckStockSysNo(PurchaseOrderInfo entity, PurchaseOrderInfo localEntity)
        {
            if (!IsSSBEnabled())
            {
                return true;
            }
            //CRL17821 对于已经发送过创建消息的采购单(检查POSSB_Log)，在更新时不允许更新入库仓
            if (PurchaseOrderDA.LoadPOSSBLog(entity.SysNo.Value, PurchaseOrderSSBMsgType.I).Count > 0)
            {
                var originalStockSysNo = localEntity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value.ToString() +
                    (localEntity.PurchaseOrderBasicInfo.ITStockInfo.SysNo.HasValue ? localEntity.PurchaseOrderBasicInfo.ITStockInfo.SysNo.Value.ToString() : string.Empty);

                var newStockSysNo = entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value.ToString();

                if (originalStockSysNo != newStockSysNo)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check负采购时Item数量不能大于分仓对应的可用库存加代销库存数量
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckNegativePoItemNumber(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                foreach (PurchaseOrderItemInfo item in entity.POItems)
                {
                    if (!PurchaseOrderDA.CheckReturnPurchaseOrderValid(entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value, item.ProductSysNo.Value, item.PurchaseQty.Value, entity.CompanyCode))
                    {
                        return GetMessageString("PO_CheckMsg_NegativeProductCount") + Environment.NewLine;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 构建检查结果信息
        /// </summary>
        /// <param name="exceptionInfo"></param>
        /// <returns></returns>
        private string GetCheckResult(string exceptionInfo)
        {
            if (string.IsNullOrEmpty(exceptionInfo))
            {
                return GetMessageString("PO_CheckMsg_Success") + Environment.NewLine;
            }
            return exceptionInfo;
        }

        /// <summary>
        /// 检查PO单是否超过合作金额及期限等信息
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckOverMoney(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            if (actionType == PurchaseOrderActionType.Check)
            {
                string execStr = string.Empty;
                bool isOK = true;
                decimal contractAmt = 0;
                decimal totalPOMoney = 0;
                decimal totalAmt = 0;
                DateTime validDate = System.DateTime.MinValue;
                DateTime expiredDate = System.DateTime.MinValue;
                DataTable dt = PurchaseOrderDA.GetPOTotalAmt(entity.SysNo.Value);

                if (dt.Rows[0]["ValidDate"] != null && dt.Rows[0]["ValidDate"].ToString() != "")
                {
                    validDate = Convert.ToDateTime(dt.Rows[0]["ValidDate"]);
                }

                if (dt.Rows[0]["expiredDate"] != null && dt.Rows[0]["expiredDate"].ToString() != "")
                {
                    expiredDate = Convert.ToDateTime(dt.Rows[0]["ExpiredDate"]);
                }

                if (dt.Rows[0]["ContractAmt"] != null && dt.Rows[0]["ContractAmt"].ToString() != "")
                {
                    contractAmt = Convert.ToDecimal(dt.Rows[0]["ContractAmt"]);
                }
                else
                {
                    contractAmt = -1m;
                }

                if (dt.Rows[0]["TotalPOMoney"] != null && dt.Rows[0]["TotalPOMoney"].ToString() != "")
                {
                    totalPOMoney = Convert.ToDecimal(dt.Rows[0]["TotalPOMoney"]);
                }
                else
                {
                    totalPOMoney = -1m;
                }

                if (dt.Rows[0]["TotalAmt"] != null && dt.Rows[0]["TotalAmt"].ToString() != "")
                {
                    totalAmt = Convert.ToDecimal(dt.Rows[0]["TotalAmt"]);
                }
                else
                {
                    totalAmt = -1m;
                }

                DateTime curday = DateTime.Now;

                if (validDate != System.DateTime.MinValue && expiredDate != System.DateTime.MinValue)
                {
                    validDate = DateTime.Parse(validDate.ToShortDateString() + " 00:00:00");
                    expiredDate = DateTime.Parse(expiredDate.ToShortDateString() + " 23:59:59");
                    if (validDate > curday || expiredDate < curday)
                    {
                        isOK = false;
                        execStr += string.Format(GetMessageString("PO_CheckMsg_ContractDateExpaired") + Environment.NewLine, validDate.ToString(), expiredDate.ToString());
                    }
                }

                if (contractAmt != -1m && totalAmt != -1m)
                {
                    if (totalPOMoney == -1m)
                    {
                        totalPOMoney = 0;
                    }

                    if (totalAmt + totalPOMoney > contractAmt)
                    {
                        isOK = false;
                        execStr += string.Format(GetMessageString("PO_CheckMsg_ContractAmtMax") + Environment.NewLine, contractAmt.ToString("#0.00"));
                    }
                }

                if (isOK == false)
                {
                    return execStr;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 检查库存金额基准值
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        public string CheckInventoryAmount(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            if (actionType == PurchaseOrderActionType.Check)
            {
                string strMsg = string.Empty;

                decimal PMSaleRatePerMonth = 0, PMSaleTargetPerMonth = 0, PMMaxAmtPerDay, PMTLSaleRatePerMonth, PMMaxAmtPerOrder, PMDMaxAmtPerDay, PMDMaxAmtPerOrder;
                //获取PM相关信息
                PurchaseOrderDA.LoadPMSaleInfo(
                                        entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value
                                        , out PMSaleRatePerMonth
                                        , out  PMSaleTargetPerMonth
                                        , out PMMaxAmtPerOrder
                                        , out  PMMaxAmtPerDay
                                        , out  PMTLSaleRatePerMonth
                                        , out PMDMaxAmtPerOrder
                                        , out PMDMaxAmtPerDay
                                        , entity.CompanyCode
                                       );

                strMsg = GetMessageString("PO_CheckMsg_InventoryAmt") + Environment.NewLine;
                decimal totalAmt = PMSaleRatePerMonth * PMSaleTargetPerMonth;
                //调用Inventory接口获取库存信息,
                decimal amtInventory = PurchaseOrderDA.GetPMInventoryAmt(entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value, entity.CompanyCode);

                decimal poAuditAmt = PurchaseOrderDA.GetAuditPOTotalAmt(entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value);

                if (entity.PurchaseOrderBasicInfo.ConsignFlag.Value == PurchaseOrderConsignFlag.Consign)
                {
                    amtInventory += (poAuditAmt) / 1.17m;
                }
                else
                {
                    amtInventory += (poAuditAmt + entity.PurchaseOrderBasicInfo.TotalAmt.Value) / 1.17m;
                }

                if (amtInventory > totalAmt)
                {
                    return string.Format(strMsg, amtInventory.ToString("#,###,###,##0.00"), totalAmt.ToString("#,###,###,##0.00"));
                }
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 采购限额
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private string CheckAmtJustCheck(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            decimal todayPOAmt = PurchaseOrderDA.GetPOTotalAmtToday(entity.SysNo.Value, entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value);
            decimal POAmt = entity.PurchaseOrderBasicInfo.TotalAmt.Value;
            ProductManagerInfo pmEntity = new ProductManagerInfo();

            #region 权限
            //调用IM接口获取PM信息：
            decimal getSaleRatePerMonth = 0m;
            decimal getSaleTargetPerMonth = 0m;
            decimal getMaxAmtPerDay = 0m;
            decimal getMaxAmtPerOrder = 0m;
            decimal getPMDMaxAmtPerOrder = 0m;
            decimal getPMDMaxAmtPerDay = 0m;
            decimal getTLSaleRatePerMonth = 0m;
            PurchaseOrderDA.LoadPMSaleInfo(entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value, out getSaleRatePerMonth, out getSaleTargetPerMonth, out getMaxAmtPerOrder, out getMaxAmtPerDay, out getTLSaleRatePerMonth, out getPMDMaxAmtPerOrder, out getPMDMaxAmtPerDay, entity.CompanyCode);

            //如果用户是TL权限
            if (POAmt > getPMDMaxAmtPerOrder)
            {
                return string.Format(GetMessageString("PO_CheckMsg_InventoryAmt1"), POAmt.ToString("#,###,###,##0.00"), getPMDMaxAmtPerOrder.ToString("#,###,###,##0.00")) + Environment.NewLine;
            }
            if (todayPOAmt > getPMDMaxAmtPerDay)
            {
                return string.Format(GetMessageString("PO_CheckMsg_InventoryAmt2"), getPMDMaxAmtPerDay.ToString("#,###,###,##0.00"), todayPOAmt.ToString("#,###,###,##0.00")) + Environment.NewLine;
            }
            //如果用户是PM权限
            if (POAmt > getMaxAmtPerOrder)
            {
                return string.Format(GetMessageString("PO_CheckMsg_InventoryAmt3"), POAmt.ToString("#,###,###,##0.00"), getMaxAmtPerOrder.ToString("#,###,###,##0.00")) + Environment.NewLine;
            }

            if (todayPOAmt > getMaxAmtPerDay)
            {
                return string.Format(GetMessageString("PO_CheckMsg_InventoryAmt4"), getMaxAmtPerDay.ToString("#,###,###,##0.00"), todayPOAmt.ToString("#,###,###,##0.00")) + Environment.NewLine;
            }

            #endregion

            return string.Empty;
        }

        private string CheckUnActivityInvetory(List<PurchaseOrderItemInfo> list)
        {
            StringBuilder strbui = new StringBuilder();
            foreach (PurchaseOrderItemInfo item in list)
            {
                if (item.UnActivatyCount.HasValue && item.UnActivatyCount.Value > 0)
                {
                    strbui.Append(item.ProductID + ";");
                }
            }
            if (strbui.ToString().Length > 2)
            {
                return string.Format(GetMessageString("PO_CheckMsg_InventoryInvalid"), strbui.ToString());
            }
            else
            {
                return GetMessageString("PO_CheckMsg_InventoryNotActive");
            }
        }

        private Dictionary<string, string> CreateCheckDictionary(PurchaseOrderInfo entity)
        {
            Dictionary<string, string> dictonary = new Dictionary<string, string>();
            dictonary.Add(GetMessageString("PO_CheckItem_PM"), GetCheckResult(CheckPM(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_Contract"), GetCheckResult(CheckOverMoney(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_ETA"), GetCheckResult(CheckETA(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_VendorRank"), GetCheckResult(CheckRank(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_Price"), GetCheckResult(CheckPrice(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_Margin3"), GetCheckResult(CheckRateOfMargin(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_InventoryAmtBase"), GetCheckResult(CheckInventoryAmount(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_InventoryAmt2"), GetCheckResult(InventoryAmtCheck(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_DayBaseLine"), GetCheckResult(CheckPoorSaleBaseline(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_POLimit"), GetCheckResult(CheckAmtJustCheck(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_NegativePOAudit"), GetCheckResult(CheckNegativePO(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_NegativePOPay"), GetCheckResult(CheckNegativePOVenderAmt(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_DelayInvoice"), GetCheckResult(CheckDelayedInvoice(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_DelayGoods"), GetCheckResult(CheckDelayedGoods(entity, PurchaseOrderActionType.Check)));
            dictonary.Add(GetMessageString("PO_CheckItem_PurchasePrice"), GetCheckResult(CheckVirtualPrice(entity)));
            dictonary.Add(GetMessageString("PO_CheckItem_EIMS"), GetCheckResult(CheckEIMSInvoiceUseInfo(entity)));
            dictonary.Add(GetMessageString("PO_CheckItem_BatchInfo"), CheckUnActivityInvetory(entity.POItems));
            dictonary.Add(GetMessageString("PO_CheckItem_CostInventory"), CheckCostInventory(entity));
            return dictonary;
        }

        private void SetDataEntyType(PurchaseOrderInfo entity)
        {
            for (int i = 0; i < entity.POItems.Count; i++)
            {
                if (entity.POItems[i].AcquireReturnPointType.HasValue)
                {
                    if (entity.POItems[i].AcquireReturnPointType.Value == 0 || entity.POItems[i].AcquireReturnPointType.Value == 1)
                    {
                    }
                    else
                    {
                        entity.POItems[i].AcquireReturnPointType = null;
                    }
                }
            }
        }

        #endregion

        #region [计算采购总金额 方法]

        private int ConvertInteger(int? obj)
        {
            return !obj.HasValue ? 0 : obj.Value;
        }

        /// <summary>
        /// //计算采购总金额
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public decimal CaclTotalAmt(List<PurchaseOrderItemInfo> items)
        {
            decimal totalAmt = 0;
            foreach (PurchaseOrderItemInfo item in items)
            {
                totalAmt += item.OrderPrice.Value * item.PurchaseQty.Value;
            }
            return totalAmt;
        }

        #endregion

        /// <summary>
        /// 计算商品的毛利率
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal? CalculateProductRate(PurchaseOrderItemInfo item)
        {
            decimal rate = 0m;
            //调用IM接口， ProductPriceInfo price = new ProductProvider().QueryProductPriceInfo(item.ProductSysNo);
            ProductInfo product = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value);
            if (null != product)
            {

                ProductPriceInfo price = product.ProductPriceInfo;
                if (price != null)
                {
                    if (!price.CurrentPrice.HasValue || !price.Point.HasValue || !item.OrderPrice.HasValue)
                    {
                        return rate;
                    }
                }
                if ((price != null) && (price.CurrentPrice - price.Point * 0.10m) != 0)
                {
                    rate = (price.CurrentPrice.Value - price.Point.Value * 0.10m - item.OrderPrice.Value) / (price.CurrentPrice.Value - price.Point.Value * 0.10m);
                }
            }
            return rate;
        }

        /// <summary>
        /// 计算京东价毛利率
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal? CalculateJDRate(PurchaseOrderItemInfo item)
        {
            decimal rate = 0m;
            if (item.JingDongPrice.HasValue && Convert.ToDecimal(item.JingDongPrice) > 0)
            {
                rate = (Convert.ToDecimal(item.JingDongPrice.Value) - item.OrderPrice.Value) / Convert.ToDecimal(item.JingDongPrice.Value);
                return rate;
            }
            return null;
        }

        /// <summary>
        /// 调用IM,Inventory相关接口,获取库存相关信息
        /// </summary>
        /// <param name="result"></param>
        protected virtual void InitializePOItemStockInfo(PurchaseOrderItemInfo item)
        {
            List<PurchaseOrderItemInfo> getPOItemsInfo = PurchaseOrderDA.LoadPOItemAddInfo(new List<int>() { item.ProductSysNo.Value });
            List<ProductInventoryInfo> getInventoryList = ExternalDomainBroker.GetInventoryInfo((int)item.ProductSysNo);
            List<ProductSalesTrendInfo> saleInfoList = ExternalDomainBroker.GetProductSalesTrendInfo(item.ProductSysNo.Value);

            PurchaseOrderItemInfo getSaleInfoItem = PurchaseOrderDA.GetPurchaseOrderItemSalesTrend(item.ProductSysNo.Value);

            if (null != getInventoryList && 0 < getInventoryList.Count)
            {
                List<ProductInventoryInfo> getSHInventoryInfo = getInventoryList.Where(x => x.StockSysNo == 51 || x.StockSysNo == 59).ToList();
                //上海仓库存:
                item.SHInventoryStock = getSHInventoryInfo.Sum(q => q.ConsignQty + q.AvailableQty);
                //上海仓移仓在途数量:
                item.SHSheftOnRoadNumber = getSHInventoryInfo.Sum(y => y.ShiftQty);
                //上海仓已采购数量:
                item.SHHaveStockNumber = ConvertInteger(getPOItemsInfo[0].SHHaveStockNumber);
                //上海仓在途数量:
                item.SHOnRoadNumber = ConvertInteger(item.SHSheftOnRoadNumber) + ConvertInteger(item.SHHaveStockNumber);
                //上海仓待入库数量:
                item.SHWaitInStockNumber = ConvertInteger(getPOItemsInfo[0].SHWaitInStockNumber);
                //上海仓待审核数量 ：
                item.SHWaitCheckNumber = ConvertInteger(getPOItemsInfo[0].SHWaitCheckNumber);
                //上海仓W1:
                item.SHW1 = getSaleInfoItem == null ? 0 : getSaleInfoItem.SHW1;

                List<ProductInventoryInfo> getBJInventoryInfo = getInventoryList.Where(x => x.StockSysNo == 52).ToList();
                //北京仓库存:
                item.BJInventoryStock = getBJInventoryInfo.Sum(q => q.ConsignQty + q.AvailableQty);

                //北京仓移仓在途数量:
                item.BJSheftOnRoadNumber = getBJInventoryInfo.Sum(y => y.ShiftQty);
                //北京仓已采购数量:
                item.BJHaveStockNumber = ConvertInteger(getPOItemsInfo[0].BJHaveStockNumber);
                //北京仓在途数量:
                item.BJOnRoadNumber = ConvertInteger(item.BJSheftOnRoadNumber) + ConvertInteger(item.BJHaveStockNumber);
                //北京仓待入库数量:
                item.BJWaitInStockNumber = ConvertInteger(getPOItemsInfo[0].BJWaitInStockNumber);
                //北京仓待审核数量 ：
                item.BJWaitCheckNumber = ConvertInteger(getPOItemsInfo[0].BJWaitCheckNumber);
                //北京仓W1:
                item.BJW1 = getSaleInfoItem == null ? 0 : getSaleInfoItem.BJW1;

                List<ProductInventoryInfo> getGZInventoryInfo = getInventoryList.Where(x => x.StockSysNo == 53).ToList();
                //广州仓库存:
                item.GZInventoryStock = getGZInventoryInfo.Sum(q => q.ConsignQty + q.AvailableQty);

                //广州仓移仓在途数量:
                item.GZSheftOnRoadNumber = getGZInventoryInfo.Sum(y => y.ShiftQty);
                //广州仓已采购数量:
                item.GZHaveStockNumber = ConvertInteger(getPOItemsInfo[0].GZHaveStockNumber);
                //广州仓在途数量:
                item.GZOnRoadNumber = ConvertInteger(item.GZSheftOnRoadNumber) + ConvertInteger(item.GZHaveStockNumber);
                //广州仓待入库数量:
                item.GZWaitInStockNumber = ConvertInteger(getPOItemsInfo[0].GZWaitInStockNumber);
                //广州仓待审核数量 ：
                item.GZWaitCheckNumber = ConvertInteger(getPOItemsInfo[0].GZWaitCheckNumber);
                //广州仓W1:
                item.GZW1 = getSaleInfoItem == null ? 0 : getSaleInfoItem.GZW1;

                List<ProductInventoryInfo> getWHInventoryInfo = getInventoryList.Where(x => x.StockSysNo == 54).ToList();
                //武汉仓库存:
                item.WHInventoryStock = getWHInventoryInfo.Sum(q => q.ConsignQty + q.AvailableQty);

                //武汉仓移仓在途数量:
                item.WHSheftOnRoadNumber = getWHInventoryInfo.Sum(y => y.ShiftQty);
                //武汉仓已采购数量:
                item.WHHaveStockNumber = ConvertInteger(getPOItemsInfo[0].WHHaveStockNumber);
                //武汉仓在途数量:
                item.WHOnRoadNumber = ConvertInteger(item.WHSheftOnRoadNumber) + ConvertInteger(item.WHHaveStockNumber);
                //武汉仓待入库数量:
                item.WHWaitInStockNumber = ConvertInteger(getPOItemsInfo[0].WHWaitInStockNumber);
                //武汉仓待审核数量 ：
                item.WHWaitCheckNumber = ConvertInteger(getPOItemsInfo[0].WHWaitCheckNumber);
                //武汉仓W1:
                item.WHW1 = getSaleInfoItem == null ? 0 : getSaleInfoItem.WHW1;

                List<ProductInventoryInfo> getCDInventoryInfo = getInventoryList.Where(x => x.StockSysNo == 55).ToList();
                //成都仓库存:
                item.CDInventoryStock = getCDInventoryInfo.Sum(q => q.ConsignQty + q.AvailableQty);

                //成都仓移仓在途数量:
                item.CDSheftOnRoadNumber = getCDInventoryInfo.Sum(y => y.ShiftQty);
                //成都仓已采购数量:
                item.CDHaveStockNumber = ConvertInteger(getPOItemsInfo[0].CDHaveStockNumber);
                //成都仓在途数量:
                item.CDOnRoadNumber = ConvertInteger(item.CDSheftOnRoadNumber) + ConvertInteger(item.CDHaveStockNumber);
                //成都仓待入库数量:
                item.CDWaitInStockNumber = ConvertInteger(getPOItemsInfo[0].CDWaitInStockNumber);
                //成都仓待审核数量 ：
                item.CDWaitCheckNumber = ConvertInteger(getPOItemsInfo[0].CDWaitCheckNumber);
                //成都仓W1:
                item.CDW1 = getSaleInfoItem == null ? 0 : getSaleInfoItem.CDW1;
            }
        }

        #region  WaitingInStockPO Check 审核PO单

        /// <summary>
        /// 检查POEntity中是否有指定的权限
        /// </summary>
        /// <param name="privliege">需要判断的权限</param>
        /// <param name="entity">PO实体</param>
        /// <returns>有权限返回true,否则返回false</returns>
        private bool CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege privliege, PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.Privilege != null && entity.PurchaseOrderBasicInfo.Privilege.Count != 0)
            {
                var privilegeValue = (int)privliege;

                if (privilegeValue >= 0 && privilegeValue <= 2)
                {
                    var maxprivliege = entity.PurchaseOrderBasicInfo.Privilege.OrderBy(i => ((int)i)).ToList()[0];

                    return (int)maxprivliege <= (int)privilegeValue;
                }

                return entity.PurchaseOrderBasicInfo.Privilege.Contains((PurchaseOrderPrivilege)Enum.Parse(typeof(PurchaseOrderPrivilege), privilegeValue.ToString()));
            }
            else
            {
                throw new BizException(GetMessageString("PO_CheckMsg_AccessDenied"));
            }
        }

        private string CheckPreviewWaitInStock(PurchaseOrderInfo entity)
        {
            bool result = true;
            PurchaseOrderInfo localentity = PurchaseOrderDA.LoadPOMaster(entity.SysNo.Value);
            if (localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus != null && localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus.HasValue)
            {
                if (localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus != null && localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus.Value.ToString() == "1" && !CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditGeneric, entity))
                {
                    result = false;
                }
                else if (localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus == 2 && !CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditAll, entity))
                {
                    result = false;
                }
            }
            else
            {
                result = true;
            }
            if (result == false)
            {
                switch (localentity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus)
                {
                    case 1:
                        return GetMessageString("PO_CheckMsg_AccessDeniedForTLOrPMD");
                    case 2:
                        return GetMessageString("PO_CheckMsg_AccessDeniedForPMD");
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查是否是合理的PM
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckPM(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            var userSysNo = entity.PurchaseOrderBasicInfo.AuditUserSysNo;

            bool isTl = entity.PurchaseOrderBasicInfo.Privilege.Exists(item =>
            {
                return item == PurchaseOrderPrivilege.CanAuditGeneric;
            });
            if (isTl)
            {
                return string.Empty;
            }
            //PMD验证
            bool relult = entity.PurchaseOrderBasicInfo.Privilege.Exists(item =>
            {
                return item == PurchaseOrderPrivilege.CanAuditAll;
            });
            if (relult)
            {
                return string.Empty;
            }

            //调用IM接口，根据userSysNo 获取PM组信息:
            List<ProductManagerInfo> pmList = ExternalDomainBroker.GetPMList(entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value).ProductManagerInfoList;
            relult = false;
            if (pmList.Count > 0)
            {
                string usersyStr = userSysNo.Value.ToString();
                foreach (ProductManagerInfo dr in pmList)
                {
                    if (dr.UserInfo.SysNo != null)
                    {
                        var user = dr.UserInfo.SysNo.Value.ToString();
                        if (user == usersyStr)
                        {
                            relult = true;
                            break;
                        }
                    }
                }
            }
            if (entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value != userSysNo && !isTl && !relult)
            {
                //你不是该PO单归属PM，也不是该PM的组长，所以无法审核该PO单！
                return GetMessageString("PO_CheckMsg_AuditDenied");
            }

            return string.Empty;
        }

        private void PreCheckPreviewAndPM(PurchaseOrderInfo entity)
        {
            string exceptinInfo = CheckPM(entity, PurchaseOrderActionType.Check);
            if (!string.IsNullOrEmpty(exceptinInfo))
            {
                throw new BizException(exceptinInfo);
            }
            exceptinInfo = CheckPreviewWaitInStock(entity);
            if (!string.IsNullOrEmpty(exceptinInfo))
            {
                throw new BizException(exceptinInfo);
            }
        }

        private void PreCheckPOStatusWhenAudit(PurchaseOrderInfo entity)
        {
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingAudit)
            {
                throw new BizException(GetMessageString("PO_Update_AlreadyConfirmed"));
            }
        }

        /// <summary>
        /// PO单等待入库操作 - Check逻辑.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string PreCheckWaitingInStock(PurchaseOrderInfo entity)
        {
            Dictionary<string, string> dictonary = new Dictionary<string, string>();

            dictonary.Add(GetMessageString("PO_CheckItem_PM"), CheckPM(entity, PurchaseOrderActionType.Check));
            if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditAll, entity))
            {
                dictonary.Add(GetMessageString("PO_CheckItem_VendorRank"), CheckRank(entity, PurchaseOrderActionType.Audit));
                dictonary.Add(GetMessageString("PO_CheckItem_Price"), CheckPrice(entity, PurchaseOrderActionType.Check));
                dictonary.Add(GetMessageString("PO_CheckItem_Margin3"), CheckRateOfMargin(entity, PurchaseOrderActionType.Audit));
                dictonary.Add(GetMessageString("PO_CheckItem_InventoryAmt2"), InventoryAmtCheck(entity, PurchaseOrderActionType.Check));
                dictonary.Add(GetMessageString("PO_CheckItem_DayBaseLine"), CheckPoorSaleBaseline(entity, PurchaseOrderActionType.Check));
                dictonary.Add(GetMessageString("PO_CheckItem_POLimit"), CheckAmt(entity, PurchaseOrderActionType.Audit));
                dictonary.Add(GetMessageString("PO_CheckItem_PurchasePrice"), CheckVirtualPrice(entity));
                dictonary.Add(GetMessageString("PO_CheckItem_EIMS"), CheckEIMSInvoiceUseInfo(entity));
                dictonary.Add(GetMessageString("PO_CheckItem_EIMS"), CheckEIMSInvoiceUseInfo(entity));
                dictonary.Add(GetMessageString("PO_CheckItem_CostInventory"), CheckCostInventory(entity));
            }

            if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditNegativeStock, entity))
            {
                dictonary.Add(GetMessageString("PO_CheckItem_NegativePOAudit"), CheckNegativePO(entity, PurchaseOrderActionType.Check));
            }
            if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditLagInvoice, entity))
            {
                dictonary.Add(GetMessageString("PO_CheckItem_DelayInvoice"), CheckDelayedInvoice(entity, PurchaseOrderActionType.Check));
            }
            if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditLagGoods, entity))
            {
                dictonary.Add(GetMessageString("PO_CheckItem_DelayGoods"), CheckDelayedGoods(entity, PurchaseOrderActionType.Audit));
            }
            StringBuilder messageInfo = new StringBuilder();
            messageInfo.Append(GetMessageString("PO_CheckMsg_Result"));
            int i = 0;
            foreach (var item in dictonary)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    i++;
                    messageInfo.Append(i.ToString() + "." + item.Key + GetCheckResult(item.Value) + Environment.NewLine);
                }
            }
            if (i == 0)
            {
                return string.Empty;
            }
            return messageInfo.ToString();
        }

        /// <summary>
        /// 根据传入的操作类型进行PO检查或者PO审核
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">操作类型</param>
        /// <returns>消息字符串</returns>
        private Dictionary<string, string> CheckOrAudit(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            entity.POItems = PurchaseOrderDA.LoadPOItems(entity.SysNo.Value);
            var messageDic = new Dictionary<string, string>();

            messageDic.Add(GetMessageString("PO_CheckItem_PM"), CheckPM(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_VendorRank"), CheckRank(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_Price"), CheckPrice(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_Margin3"), CheckRateOfMargin(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_InventoryAmt2"), InventoryAmtCheck(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_DayBaseLine"), CheckPoorSaleBaseline(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_POLimit"), CheckAmt(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckItem_NegativePOAudit"), CheckNegativePO(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckMsg_DelayInvoicePO"), CheckDelayedInvoice(entity, actionType));
            messageDic.Add(GetMessageString("PO_CheckMsg_DelayGoodsPO"), CheckDelayedGoods(entity, actionType));

            //如果是检查,则需要多做如下检验
            if (actionType == PurchaseOrderActionType.Check)
            {
                messageDic.Add(GetMessageString("PO_CheckItem_Contract"), CheckOverMoney(entity, actionType));
                messageDic.Add(GetMessageString("PO_CheckItem_ETA"), CheckETA(entity, actionType));
                messageDic.Add(GetMessageString("PO_CheckItem_InventoryAmtBase"), CheckInventoryAmount(entity, actionType));
                messageDic.Add(GetMessageString("PO_CheckItem_NegativePOPay"), CheckNegativePOVenderAmt(entity, actionType));
            }

            return messageDic;
        }

        #endregion

        #region 检查供应商等级逻辑

        /// <summary>
        /// 检查供应商等级逻辑
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckRank(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //如果不是PMD则需要进行验证
            switch (entity.VendorInfo.VendorBasicInfo.VendorRank)
            {
                case VendorRank.A://不限品牌和类别，PM自己审
                    break;
                case VendorRank.B:
                    if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditGeneric, entity))
                    {
                        IsChangeTPStaus = true;
                        return GetMessageString("PO_CheckMsg_RankBSubmitToTL") + Environment.NewLine;
                    }
                    break;
                case VendorRank.C:
                    {
                        if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditAll, entity))
                        {
                            IsChangeTPStaus = true;
                            return GetMessageString("PO_CheckMsg_RankCSubmitToPMD") + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// 价格检查
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckPrice(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //如果用户不是PMD
            var sbLastprice = new StringBuilder();
            var sbCurrentPrice = new StringBuilder();
            var sbJDPrice = new StringBuilder();
            string prdSysNos = entity.POItems.ToListString("ProductSysNo");
            List<int> getSysNoList = entity.POItems.Select(x => x.ProductSysNo.Value).ToList();

            List<ProductInfo> getProductInfoList = ExternalDomainBroker.GetProductList(getSysNoList);

            ProductPriceInfo info = null;

            foreach (var item in entity.POItems)
            {
                decimal? getLastPrice = PurchaseOrderDA.GetPurchaseOrderItemLastPrice(item.ProductSysNo.Value);

                ///商品{0}的采购价格{1}高于上次采购价格{2}，需要提交PMD审核:
                if (getLastPrice.HasValue && item.OrderPrice > getLastPrice.Value)
                {
                    sbLastprice.AppendFormat(string.Format(GetMessageString("PO_CheckMsg_POPriceLargerThanLastPurchasePrice"), item.ProductID, item.OrderPrice.Value.ToString("#,###,###,##0.00"), getLastPrice.Value.ToString("#,###,###,##0.00")));
                    IsChangeTPStaus = true;
                }
                info = getProductInfoList.Where(p => p.SysNo == item.ProductSysNo.Value && p.ProductBasicInfo != null).Select(p => p.ProductPriceInfo).FirstOrDefault();
                //未找到商品{0}的前台售价信息:
                if (info == null)
                {
                    sbLastprice.AppendFormat(string.Format(GetMessageString("PO_CheckMsg_BasicPriceNotFound"), item.ProductID));
                }
                //商品{0}的采购价格{1}高于前台售价{2}，需要提交PMD审核
                else if (item.OrderPrice > info.CurrentPrice)
                {
                    sbCurrentPrice.AppendFormat(GetMessageString("PO_CheckMsg_POPriceLargerThanBasicPrice"), item.ProductID, item.OrderPrice.Value.ToString("#,###,###,##0.00"), info.CurrentPrice.Value.ToString("#,###,###,##0.00"));
                    IsChangeTPStaus = true;
                }
                // 京东价Check：未找到商品{0}的京东价格信息
                decimal? getJingDongPrice = PurchaseOrderDA.GetPurchaseOrderItemJingDongPrice(item.ProductSysNo.Value);
                if (getJingDongPrice.HasValue && item.OrderPrice > getJingDongPrice.Value)
                {
                    sbJDPrice.AppendFormat(GetMessageString("PO_CheckMsg_PriceLargerThanJDPrice") + Environment.NewLine, item.ProductID, item.OrderPrice.Value.ToString("#,###,###,##0.00"), getJingDongPrice.Value.ToString("#,###,###,##0.00"));
                    IsChangeTPStaus = true;
                }
            }

            return (sbLastprice.ToString() + sbCurrentPrice.ToString() + sbJDPrice.ToString()).Length <= 0 ? string.Empty : (sbLastprice.ToString() + sbCurrentPrice.ToString() + sbJDPrice.ToString() + Environment.NewLine);
        }

        /// <summary>
        /// TODO:调用IM接口,检查三级类别基准毛利率
        ///
        ///
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckRateOfMargin(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            string showInfor = string.Empty;
            StringBuilder strBuidExce = new StringBuilder();
            DataTable minInterstList = PurchaseOrderDA.GetPurchaseOrderItemMinInterestRate(entity.POItems.ToListString("ProductSysNo"));


            foreach (var item in entity.POItems)
            {
                DataRow rateinfo = null;

                foreach (DataRow dr in minInterstList.Rows)
                {
                    if (dr["SysNo"].ToString().Trim() == item.ProductSysNo.Value.ToString())
                    {
                        rateinfo = dr;
                        break;
                    }
                }

                if (rateinfo != null)
                {
                    if ((rateinfo["CurrentPrice"].ToDecimal() - rateinfo["Point"].ToInteger() / 10) == 0)
                    {
                        strBuidExce.AppendFormat(GetMessageString("PO_CheckMsg_SubPointError") + Environment.NewLine, item.ProductID);
                        continue;
                    }

                    rateinfo["Tax"] = ((rateinfo["CurrentPrice"].ToDecimal() - rateinfo["Point"].ToInteger() * 0.10m - item.OrderPrice.Value) / (rateinfo["CurrentPrice"].ToDecimal() - rateinfo["Point"].ToInteger() * 0.10m)).ToString();

                    if (rateinfo["Tax"].ToDecimal() < rateinfo["MinMarginPMD"].ToDecimal())
                    {
                        IsChangeTPStaus = true;
                        strBuidExce.AppendFormat(
                            GetMessageString("PO_CheckMsg_CurrentMarginLessThanBaseMargin") + Environment.NewLine,
                            item.ProductID,
                            (rateinfo["Tax"].ToDecimal() * 100).ToString("#0.00"),
                            (rateinfo["MinMarginPMD"].ToDecimal() * 100).ToString("#0.00"));
                    }

                    if (rateinfo["Tax"].ToDecimal() < rateinfo["MinMargin"].ToDecimal() && rateinfo["Tax"].ToDecimal() >= rateinfo["MinMarginPMD"].ToDecimal())
                    {
                        if (actionType == PurchaseOrderActionType.Check)
                        {
                            CheckTLRate(strBuidExce, rateinfo["ProductID"].ToString(), rateinfo["Tax"].ToDecimal(), rateinfo["MinMargin"].ToDecimal());
                        }
                        else if (!CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditGeneric, entity))
                        {
                            CheckTLRate(strBuidExce, rateinfo["ProductID"].ToString(), rateinfo["Tax"].ToDecimal(), rateinfo["MinMargin"].ToDecimal());
                        }

                    }
                }
            }

            return strBuidExce.ToString();
        }

        /// <summary>
        /// 检查商品的前毛利 ,是否低于TL基准毛利率
        /// </summary>
        /// <param name="strBuidExce"></param>
        /// <param name="rateinfo"></param>
        private void CheckTLRate(StringBuilder strBuidExce, string productID, decimal tax, decimal minMargin)
        {
            IsChangeTPStaus = true;
            strBuidExce.AppendFormat(
         GetMessageString("PO_CheckMsg_CurrentMarginLessThanTLMargin") + Environment.NewLine,
                 productID,
                 (tax * 100).ToString("#0.00"),
                 (minMargin * 100).ToString("#0.00"));
        }

        /// <summary>
        /// 二级类别的库存金额
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string InventoryAmtCheck(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //如果用户不是PMD
            StringBuilder msgBuilder = new StringBuilder();
            //TODO:调用Inventory和IM接口，获取2级类别，和库存

            List<string> c2SysNoList = new List<string>();
            entity.POItems.ForEach(x =>
            {
                CategoryInfo getProductCategoryInfo = ExternalDomainBroker.GetProductInfoBySysNo(x.ProductSysNo.Value).ProductBasicInfo.ProductCategoryInfo;
                int getC2SysNo = ExternalDomainBroker.GetCategory3Info(getProductCategoryInfo.SysNo.Value).ParentSysNumber.Value;
                c2SysNoList.Add(getC2SysNo.ToString());

            });

            DataTable c2InventoryAmts = PurchaseOrderDA.GetC2TotalAmt(string.Join(",", c2SysNoList));
            if (c2InventoryAmts != null && c2InventoryAmts.Rows.Count > 0)
            {
                entity.POItems.ForEach((item) =>
                {
                    int count = 0;
                    string quotaString = string.Empty;
                    string C2InventoryAmtString = string.Empty;
                    for (int i = 0; i < c2InventoryAmts.Rows.Count; i++)
                    {
                        if (c2InventoryAmts.Rows[i]["C2SysNo"] != null)
                        {
                            CategoryInfo getProductCategoryInfo = ExternalDomainBroker.GetProductInfoBySysNo(item.ProductSysNo.Value).ProductBasicInfo.ProductCategoryInfo;
                            int itemC2SysNo = ExternalDomainBroker.GetCategory3Info(getProductCategoryInfo.SysNo.Value).ParentSysNumber.Value;
                            if (Convert.ToInt32(c2InventoryAmts.Rows[i]["C2SysNo"].ToString()) == itemC2SysNo)
                            {
                                count++;
                                quotaString = Convert.ToDecimal(c2InventoryAmts.Rows[i]["Quota"].ToString()).ToString("f2");
                                C2InventoryAmtString = Convert.ToDecimal(c2InventoryAmts.Rows[i]["C2InventoryAmt"].ToString()).ToString("f2");
                                break;
                            }
                        }
                    }
                    if (count > 0)
                    {
                        IsChangeTPStaus = true;
                        msgBuilder.AppendFormat(string.Format(GetMessageString("PO_CheckMsg_InventoryAmtLevel2LargerThanAmt") + Environment.NewLine, item.ProductID, C2InventoryAmtString, quotaString));
                    }
                });
            }

            return msgBuilder.ToString();
        }

        /// <summary>
        /// 检查滞销天数基准
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckPoorSaleBaseline(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //TODO:调用IM接口，获取产品最大代销天数:List<ProductMaxAgeInfo> list = m_PODAL.GetMaxProuductAge(entity);
            StringBuilder exceStr = new StringBuilder();
            //foreach (ProductMaxAgeInfo item in list)
            //{
            //    if (!item.InStockDays.HasValue || !item.ProductMaxAge.HasValue)
            //    {
            //        continue;
            //    }
            //    if (item.ProductMaxAge > item.InStockDays)
            //    {
            //        IsChangeTPStaus = true;
            //exceStr.Append(string.Format(GetMessageString("PO_CheckMsg_DelayDaysLargerThanSetting") + Environment.NewLine, item.ProductID, item.ProductMaxAge.ToString(), item.InStockDays.ToString()));
            //    }
            //}
            return exceStr.ToString();
        }

        /// <summary>
        /// 检查采购限额
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        private string CheckAmt(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            decimal todayPOAmt = PurchaseOrderDA.GetPOTotalAmtToday(entity.SysNo.Value, entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value);
            decimal POAmt = entity.PurchaseOrderBasicInfo.TotalAmt.Value;
            //获取PM信息, m_PMDAL.GetPMSaleInfor(entity.PMSysNo.Value);
            decimal PMSaleRatePerMonth = 0, PMSaleTargetPerMonth = 0, PMMaxAmtPerDay, PMTLSaleRatePerMonth, PMMaxAmtPerOrder, PMDMaxAmtPerDay, PMDMaxAmtPerOrder;
            //获取PM相关信息
            PurchaseOrderDA.LoadPMSaleInfo(
                                    entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value
                                    , out PMSaleRatePerMonth
                                    , out  PMSaleTargetPerMonth
                                    , out PMMaxAmtPerOrder
                                    , out  PMMaxAmtPerDay
                                    , out  PMTLSaleRatePerMonth
                                    , out PMDMaxAmtPerOrder
                                    , out PMDMaxAmtPerDay
                                    , entity.CompanyCode
                                   );

            string result = string.Empty;
            //如果用户是PM权限
            if (actionType == PurchaseOrderActionType.Audit)
            {
                //如果用户是TL权限
                if (CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditGeneric, entity))
                {
                    result = result + PMDAmtCheck(POAmt, todayPOAmt, PMDMaxAmtPerOrder, PMDMaxAmtPerDay);
                }
                else if (CheckPrivilegeNewWithEquqls(PurchaseOrderPrivilege.CanAuditLow, entity))
                {
                    result = result + TLAmtCheck(POAmt, todayPOAmt, PMMaxAmtPerOrder, PMMaxAmtPerDay);
                }
            }
            else
            {
                result = result + TLAmtCheck(POAmt, todayPOAmt, PMMaxAmtPerOrder, PMMaxAmtPerDay);
                result = result + PMDAmtCheck(POAmt, todayPOAmt, PMDMaxAmtPerOrder, PMDMaxAmtPerDay);
            }
            return result;
        }

        private string TLAmtCheck(decimal POAmt, decimal todayPOAmt, decimal MaxAmtPerOrder, decimal MaxAmtPerDay)
        {
            string resutl = "";
            if (POAmt > MaxAmtPerOrder)
            {
                IsChangeTPStaus = true;
                //数字格式是否需要控制?
                resutl = resutl + string.Format(GetMessageString("PO_CheckMsg_SingleAmtLargerThanTLAmt") + Environment.NewLine, POAmt.ToString("f2"), MaxAmtPerOrder.ToString("f2"));
            }

            if (todayPOAmt > MaxAmtPerDay)
            {
                IsChangeTPStaus = true;
                //数字格式是否需要控制?   已用去的咋算?
                resutl = resutl + string.Format(GetMessageString("PO_CheckMsg_TLAmtUse") + Environment.NewLine, MaxAmtPerDay.ToString("f2"), todayPOAmt.ToString("f2"), POAmt.ToString("f2"));
            }
            return resutl;
        }

        private string PMDAmtCheck(decimal POAmt, decimal todayPOAmt, decimal PMDMaxAmtPerOrder, decimal PMDMaxAmtPerDay)
        {
            string resutl = "";

            if (POAmt > PMDMaxAmtPerOrder)
            {
                IsChangeTPStaus = true;
                //数字格式是否需要控制?
                resutl = resutl + string.Format(GetMessageString("PO_CheckMsg_SingleAmtLargerThanPMDAmt") + Environment.NewLine, POAmt.ToString("f2"), PMDMaxAmtPerOrder.ToString("f2"));
            }
            if (todayPOAmt > PMDMaxAmtPerDay)
            {
                IsChangeTPStaus = true;
                //数字格式是否需要控制?   已用去的咋算?
                resutl = resutl + string.Format(GetMessageString("PO_CheckMsg_PMDAmtUse") + Environment.NewLine, PMDMaxAmtPerDay.ToString("f2"), todayPOAmt.ToString("f2"), POAmt.ToString("f2"));
            }
            return resutl;
        }

        /// <summary>
        /// 检查单据采购价格高于商品正常采购价，则必须要走PMD级的审批流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckVirtualPrice(PurchaseOrderInfo entity)
        {
            StringBuilder execStr = new StringBuilder();
            foreach (PurchaseOrderItemInfo item in entity.POItems)
            {
                if (!item.VirtualPrice.HasValue)
                {
                    IsChangeTPStaus = true;
                    execStr.Append(string.Format(GetMessageString("PO_CheckMsg_BasicPriceToPMD") + Environment.NewLine, item.ProductID));
                }
                if (item.OrderPrice.Value > item.VirtualPrice.Value)
                {
                    IsChangeTPStaus = true;
                    execStr.Append(string.Format(GetMessageString("PO_CheckMsg_POPriceLargerToPMD") + Environment.NewLine, item.ProductID, item.OrderPrice.Value.ToString("#0.00"), item.VirtualPrice.Value.ToString("#0.00")));
                }
            }
            return execStr.ToString();
        }

        /// <summary>
        /// 检查如果没有使用返点，提交PMD审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string CheckEIMSInvoiceUseInfo(PurchaseOrderInfo entity)
        {
            StringBuilder execStr = new StringBuilder();
            //获取PO 的返点使用情况单据信息
            //int poEimsCount = PurchaseOrderDA.QueryEIMSInvoiceInfoByPMAndVendor(entity.VendorInfo.SysNo.Value, entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value.ToString());
            int poEimsCount = PurchaseOrderDA.QueryEIMSInvoiceInfoByVendor(entity.VendorInfo.SysNo.Value);

            //供应商和PM有剩余返点，但采购单未使用返点，需要PMD审核
            if (entity.EIMSInfo != null && entity.EIMSInfo.EIMSInfoList.Count == 0 && poEimsCount > 0)
            {
                //有可用返点，但是没有使用
                IsChangeTPStaus = true;
                execStr.Append(GetMessageString("PO_CheckMsg_VendorPMEIMSLeft") + Environment.NewLine);
            }
            return execStr.ToString();
        }

        /// <summary>
        /// 检查滞收发票PO
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        public string CheckDelayedInvoice(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //该已付款给该供应商，但缺少发票，超过30天
            //如果有采购单审核-发票超期权限，则不用检查。
            string error = string.Empty;
            //检查该供应商已付款30天但缺少发票，或者已付款10但还没收到货的情况:
            DataSet dsPoHistory = PurchaseOrderDA.GetPOHistoryAbsentInvoiceOrWaitingInStock(entity.VendorInfo.SysNo.Value, (int)PurchaseOrderStatus.WaitingInStock, entity.CompanyCode);
            if (dsPoHistory != null && (dsPoHistory.Tables[0].Rows.Count > 0 || dsPoHistory.Tables[1].Rows.Count > 0))
            {
                if (dsPoHistory.Tables[0].Rows.Count > 0)
                {
                    //"已付款，但缺少发票，超过30天";
                    error += GetMessageString("PO_CheckMsg_InvoiceMissing30Days");
                    IsChangeTPStaus = true;
                }

                return error;
            }

            return string.Empty;
        }

        /// <summary>
        /// 检查滞收货物PO
        /// </summary>
        /// <param name="entity">PO实体</param>
        /// <param name="actionType">检查类型</param>
        /// <returns>消息字符串</returns>
        public string CheckDelayedGoods(PurchaseOrderInfo entity, PurchaseOrderActionType actionType)
        {
            //付款给供应商，但7天仍未入库，则不能审核。
            //如果有采购单审核-发票超期权限，则不用检查。
            string error = string.Empty;
            //
            DataSet dsPoHistory = PurchaseOrderDA.GetPOHistoryAbsentInvoiceOrWaitingInStock(entity.VendorInfo.SysNo.Value, (int)PurchaseOrderStatus.WaitingInStock, entity.CompanyCode);
            if (dsPoHistory != null && dsPoHistory.Tables[1].Rows.Count > 0)
            {
                if (dsPoHistory.Tables[1].Rows.Count > 0)
                {
                    //"已付款，但7天仍未入库 ";
                    error += GetMessageString("PO_CheckMsg_InStockOver7Days");
                    IsChangeTPStaus = true;
                }
                return error;
            }
            return error;
        }

        #endregion

        #region 调用WMS提供的hold接口

        /// <summary>
        /// 调用WMS提供的hold接口
        /// </summary>
        /// <param name="entity"></param>
        public void CallWMSHoldMethod(PurchaseOrderInfo entity, ref int orderStatus)
        {
            PurchaseOrderCancelVerifyMessage msg = new PurchaseOrderCancelVerifyMessage()
            {
                PONumber = entity.SysNo.Value.ToString(),
                CompanyCode = entity.CompanyCode
            };
            //原来走的SSB处理机制，返回值无法取到，改为直接调用服务 by freegod 20130530 
            msg.CompanyCode = msg.CompanyCode.Trim();
            IPOService service = WCFAdapter<IPOService>.GetProxy();
            int result = service.MerchantHoldPORequest(msg.PONumber, msg.CompanyCode.Trim());
            orderStatus = result;
            if (result != 1)
            {
                var message = result == -1 ?
                    "Hold PO失败，请两分钟后重试，如多次重试后仍有问题，请与管理员联系。" :
                    "PO存在，但是已经开始处理，不能Hold";
                throw new BizException(message);
            }
        }

        public virtual void SendCloseMessage(int poSysNo, int userSysNo, string companyCode)
        {
            if (!IsSSBEnabled())
            {
                return;
            }
            PurchaseOrderInfo po = PurchaseOrderDA.LoadPOMaster(poSysNo);

            PurchaseOrderCloseMessage msg = new PurchaseOrderCloseMessage()
            {
                PONumber = poSysNo.ToString(),
                Memo = po.PurchaseOrderBasicInfo.MemoInfo != null ? po.PurchaseOrderBasicInfo.MemoInfo.Memo : string.Empty,
                CompanyCode = companyCode
            };
            EventPublisher.Publish<PurchaseOrderCloseMessage>(msg);

            PurchaseOrderDA.CreatePOSSBLog(new PurchaseOrderSSBLogInfo
            {
                POSysNo = poSysNo,
                Content = msg.ToXmlString(),
                ActionType = PurchaseOrderSSBMsgType.C.ToString(),
                InUser = userSysNo,
                SendErrMail = "N"
            }, companyCode);

        }

        #endregion

        #region [EIMS相关]

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="ruleNumber"></param>
        /// <returns></returns>
        public virtual PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoBySysNo(int ruleNumber)
        {
            return PurchaseOrderDA.GetEIMSRuleInfoBySysNo(ruleNumber);
        }

        /// <summary>
        /// 查询EIMS合同信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoByAssignedCode(string id)
        {
            return PurchaseOrderDA.GetEIMSRuteInfoByAssignedCode(id);
        }

        #endregion

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.PurchaseOrder", key);
        }

        public virtual DateTime? GetLastPoDate(int productSysNo)
        {
            return PurchaseOrderDA.GetLastPoDate(productSysNo);
        }

        public virtual int? GetPurchaseOrderReturnPointSysNo(int poSysNo)
        {
            return PurchaseOrderDA.GetPurchaseOrderReturnPointSysNo(poSysNo);
        }

        public virtual string GetWareHouseReceiptSerialNumber(int poSysNo)
        {
            return PurchaseOrderDA.GetWareHouseReceiptSerialNumber(poSysNo);
        }

        public virtual int UpdateWareHouseReceiptSerialNumber(int poSysNo, string receiptSerialNumber)
        {
            return PurchaseOrderDA.UpdateGetWareHouseReceiptSerialNumber(poSysNo, receiptSerialNumber);
        }

        public virtual List<int> GetPurchaseOrderSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return PurchaseOrderDA.GetPurchaseOrderSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        /// <summary>
        /// 获取采购单配件信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public virtual DataTable GetPurchaseOrderAccessories(int poSysNo)
        {
            return PurchaseOrderQueryDA.GetPurchaseOrderAccessories(poSysNo);
        }

        public virtual List<EIMSInfo> LoadPOEIMSInfoForPrint(int poSysNo)
        {
            return PurchaseOrderDA.LoadPOEIMSInfoForPrint(poSysNo);
        }


        public virtual List<WarehouseInfo> GetPurchaseOrderWarehouseList(string companyCode)
        {
            var list = InventoryBizInteract.GetWarehouseList(companyCode);
            return list.FindAll(p => p.WarehouseStatus == BizEntity.Inventory.ValidStatus.Valid);
        }

        /// <summary>
        /// JOB设置PO单系统关闭
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="closeUser"></param>
        /// <returns></returns>
        public virtual int SetPurchaseOrderClose(int poSysNo, string closeUser)
        {
            return PurchaseOrderDA.SetPurchaseOrderStatusClose(poSysNo, closeUser);
        }

        public virtual List<PurchaseOrderInfo> GetNeedToClosePurchaseOrderList()
        {
            return PurchaseOrderDA.GetNeedToClosePurchaseOrderList();
        }

        public virtual List<PurchaseOrderInfo> GetPurchaseOrderListForETA(string companyCode)
        {
            return PurchaseOrderDA.GetPurchaseOrderForJobETA(companyCode);
        }

        public int AbandonPOForJob(int poSysNo)
        {
            return PurchaseOrderDA.AbandonPOForJOB(poSysNo);
        }
        public int AbandonETAForJob(int poSysNo)
        {
            return PurchaseOrderDA.AbandonETAForJOB(poSysNo);
        }

        public int UpdateExtendPOInfoForJob(int poSysNo, int productSysNo)
        {
            return PurchaseOrderDA.UpdateExtendPOInfoForJob(poSysNo, productSysNo);
        }

        public int SendMailWhenAuditPurchaseOrder(string content, int poSysNo, string mailAddress)
        {
            string emailAddress = mailAddress.Trim();
            string posysno = poSysNo.ToString();
            if (emailAddress == "9999")
            {
                PurchaseOrderInfo poInfo = LoadPO(poSysNo);
                emailAddress = poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress;
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    int syso = 0;
                    int flagA = 0;
                    int flagB = 0;
                    if (emailAddress != null && emailAddress.Length > 200)
                    {
                        string part1EmailAddress = string.Empty;
                        string part2EmailAddress = string.Empty;
                        string part3EmailAddress = string.Empty;
                        string[] emailArray = emailAddress.Split(';');
                        for (int i = 0; i < emailArray.Length; i++)
                        {
                            if (part1EmailAddress.Length + emailArray[i].Length > 200)
                            {
                                flagA = i;
                                break;
                            }
                            else
                            {
                                part1EmailAddress += emailArray[i] + ";";
                            }
                        }

                        if (flagA != 0)
                        {
                            for (int k = flagA; k < emailArray.Length; k++)
                            {
                                if (part2EmailAddress.Length + emailArray[k].Length > 200)
                                {
                                    flagB = k;
                                    break;
                                }
                                else
                                {
                                    part2EmailAddress += emailArray[k] + ";";
                                }
                            }
                        }
                        if (flagB != 0)
                        {
                            for (int t = flagB; t < emailArray.Length; t++)
                            {
                                if (part3EmailAddress.Length + emailArray[t].Length > 200)
                                {
                                    break;
                                }
                                else
                                {
                                    part3EmailAddress += emailArray[t] + ";";
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(part1EmailAddress))
                        {
                            part1EmailAddress = part1EmailAddress.Substring(0, part1EmailAddress.Length - 1);
                            //   syso = provider.WriteMailAddressDB(part1EmailAddress, content, posysno);

                        }

                        if (!string.IsNullOrEmpty(part2EmailAddress))
                        {
                            part2EmailAddress = part2EmailAddress.Substring(0, part2EmailAddress.Length - 1);
                            //   syso = provider.WriteMailAddressDB(part2EmailAddress, content, posysno);

                        }

                        if (!string.IsNullOrEmpty(part3EmailAddress))
                        {
                            part3EmailAddress = part3EmailAddress.Substring(0, part3EmailAddress.Length - 1);
                            //  syso = provider.WriteMailAddressDB(part3EmailAddress, content, posysno);

                        }
                    }
                    else
                    {
                        //   syso = provider.WriteMailAddressDB(emailAddress, content, posysno);
                    }
                }
                else
                {
                    //ViewData["EmailError"] = Resources.GlobalResources.Waring_VSPO_EmailErrorMessage;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    int syso = 0;
                    int flagA = 0;
                    int flagB = 0;
                    if (emailAddress != null && emailAddress.Length > 200)
                    {
                        string part1EmailAddress = string.Empty;
                        string part2EmailAddress = string.Empty;
                        string part3EmailAddress = string.Empty;
                        string[] emailArray = emailAddress.Split(';');
                        for (int i = 0; i < emailArray.Length; i++)
                        {
                            if (part1EmailAddress.Length + emailArray[i].Length > 200)
                            {
                                flagA = i;
                                break;
                            }
                            else
                            {
                                part1EmailAddress += emailArray[i] + ";";
                            }
                        }

                        if (flagA != 0)
                        {
                            for (int k = flagA; k < emailArray.Length; k++)
                            {
                                if (part2EmailAddress.Length + emailArray[k].Length > 200)
                                {
                                    flagB = k;
                                    break;
                                }
                                else
                                {
                                    part2EmailAddress += emailArray[k] + ";";
                                }
                            }
                        }


                        if (flagB != 0)
                        {
                            for (int t = flagB; t < emailArray.Length; t++)
                            {
                                if (part3EmailAddress.Length + emailArray[t].Length > 200)
                                {
                                    break;
                                }
                                else
                                {
                                    part3EmailAddress += emailArray[t] + ";";
                                }
                            }

                        }

                        if (!string.IsNullOrEmpty(part1EmailAddress))
                        {
                            part1EmailAddress = part1EmailAddress.Substring(0, part1EmailAddress.Length - 1);
                            //  syso = provider.WriteMailAddressDB(part1EmailAddress, content, posysno);
                        }

                        if (!string.IsNullOrEmpty(part2EmailAddress))
                        {
                            part2EmailAddress = part2EmailAddress.Substring(0, part2EmailAddress.Length - 1);
                            //   syso = provider.WriteMailAddressDB(part2EmailAddress, content, posysno);
                        }

                        if (!string.IsNullOrEmpty(part3EmailAddress))
                        {
                            part3EmailAddress = part3EmailAddress.Substring(0, part3EmailAddress.Length - 1);
                            //   syso = provider.WriteMailAddressDB(part3EmailAddress, content, posysno);
                        }
                    }
                    else
                    {
                        // syso = provider.WriteMailAddressDB(emailAddress, content, posysno);

                    }


                }
                else
                {
                    //  ViewData["EmailError"] = Resources.GlobalResources.Waring_VSPO_EmailErrorMessage;
                }
            }
            return 1;
        }

        /// <summary>
        /// 查找所有的Item项再和返点进行比较
        /// </summary>
        /// <param name="entity"></param>
        private void CheckReturnPointAndPrice(PurchaseOrderInfo poInfo)
        {
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal)
            {
                PurchaseOrderInfo entity = ObjectFactory<IPurchaseOrderDA>.Instance.LoadPOMaster(poInfo.SysNo.Value);
                List<PurchaseOrderItemInfo> result = ObjectFactory<IPurchaseOrderDA>.Instance.SearchPOItemsList(poInfo.SysNo.Value);
                foreach (PurchaseOrderItemInfo item in result)
                {
                    item.LineReturnedPointCost = item.UnitCost * item.Quantity;//折扣后总价
                    //item.LineCost = item.OrderPrice.Value * item.PurchaseQty;//总价

                    if (!item.CheckStatus.HasValue)
                    {
                        item.CheckStatus = PurchaseOrdeItemCheckStatus.UnCheck;
                    }
                    if (entity.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal)
                    {
                        if (item.CheckStatus.HasValue)
                        {
                            if (!item.CheckStatus.HasValue)
                            {
                                item.CheckStatus = PurchaseOrdeItemCheckStatus.UnCheck;
                            }
                        }
                    }
                    item.CurrencySymbol = entity.PurchaseOrderBasicInfo.CurrencySymbol;
                    item.CurrentPrice = item.OrderPrice.Value * item.Quantity.Value;  //实际总价
                    //在“待入库”、“已入库”、“已作废”、“系统作废”状态下保持原数据(V_CM_Product_LastPOInfo & Product_Price)。
                    //其他状态实时取当前po_item数值；

                    if (entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock
                        || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Origin
                        || entity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned
                       )
                    {
                        PurchaseOrderItemInfo temp = ObjectFactory<IPurchaseOrderDA>.Instance.LoadExtendPOItem(item.ProductSysNo.Value);
                        item.LastOrderPrice = temp.LastOrderPrice;
                        item.AvailableQty = temp.AvailableQty;
                        item.M1 = temp.M1;
                        item.JingDongPrice = temp.JingDongPrice;

                        item.CurrentUnitCost = temp.CurrentUnitCost;
                        item.CurrentPrice = temp.CurrentPrice;
                        item.LastInTime = temp.LastInTime;
                        item.LastAdjustPriceDate = temp.LastAdjustPriceDate;
                        item.UnActivatyCount = temp.UnActivatyCount;
                    }

                    item.Tax = CalculateProductRate(item);
                    item.JingDongTax = CalculateJDRate(item);

                    if (ObjectFactory<IPurchaseOrderDA>.Instance.IsVirtualStockPurchaseOrderProduct(item.ProductSysNo.Value)) ;
                    {
                        item.IsVirtualStockProduct = true;
                    }
                }

                List<PurchaseOrderItemInfo> items = poInfo.POItems;
                List<string> productIDList = poInfo.POItems.Select(item => item.ProductID).ToList();
                decimal total = poInfo.POItems.Sum(item => (item.PurchaseQty ?? 0M) * (item.OrderPrice ?? 0M));
                foreach (var item in result)
                {
                    if (!productIDList.Contains(item.ProductID))
                    {
                        total += (item.PurchaseQty ?? 0M) * (item.OrderPrice ?? 0M);
                    }
                }
                decimal eimsTotal = poInfo.EIMSInfo.EIMSInfoList.Sum(item => item.EIMSAmt.Value);
                eimsTotal *= 1;
                if (eimsTotal > total)
                {
                    //采购单返点金额({0})不能大于采购总价({1}) !
                    throw new BizException(string.Format(GetMessageString("PO_CheckMsg_ReturnPointValidate"), eimsTotal.TruncateDecimal(2), total.TruncateDecimal(2)));
                }
            }
        }
    }
}
