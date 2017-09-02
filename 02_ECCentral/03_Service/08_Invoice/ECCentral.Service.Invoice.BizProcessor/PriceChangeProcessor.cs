using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PriceChangeProcessor))]
    public class PriceChangeProcessor
    {
        public int SavePriceChange(PriceChangeMaster item)
        {
            PreCheckSaveOrUpdate(item);

            using (TransactionScope ts = new TransactionScope())
            {
                int masterSysNo = ObjectFactory<IPriceChangeDA>.Instance.SavePriceChangeMaster(item);

                if (item.ItemList != null && item.ItemList.Count > 0)
                {
                    item.ItemList.ForEach(p =>
                    {
                        p.MasterSysNo = masterSysNo;
                        SavePriceChangeItem(p);
                    }
                    );
                }

                #region write log

                item.SysNo = masterSysNo;
                CreateOperationLog(item, "创建",BizLogType.PriceChange_Add);

                #endregion

                ts.Complete();

                return masterSysNo;
            }
        }

        public void UpdatePriceChange(PriceChangeMaster item)
        {
            PreCheckSaveOrUpdate(item);

            using (TransactionScope ts = new TransactionScope())
            {
                ObjectFactory<IPriceChangeDA>.Instance.UpdatePriceChangeMaster(item);

                ObjectFactory<IPriceChangeDA>.Instance.DeletePriceChangeItemByMasterSysNo(item.SysNo);
                if (item.ItemList != null && item.ItemList.Count > 0)
                {
                    item.ItemList.ForEach(p =>
                    {
                        p.MasterSysNo = item.SysNo;
                        SavePriceChangeItem(p);
                    }
                    );
                }

                #region write log

                CreateOperationLog(item, "更新",BizLogType.PriceChange_Update);

                #endregion

                ts.Complete();
            }
        }

        public int ClonePriceChange(PriceChangeMaster item)
        {
            PreCheckSaveOrUpdate(item);

            using (TransactionScope ts = new TransactionScope())
            {
                int masterSysNo = ObjectFactory<IPriceChangeDA>.Instance.SavePriceChangeMaster(item);

                if (item.ItemList != null && item.ItemList.Count > 0)
                {
                    item.ItemList.ForEach(p =>
                    {
                        p.MasterSysNo = masterSysNo;
                        SavePriceChangeItem(p);
                    }
                    );
                }

                #region write log

                item.SysNo = masterSysNo;
                CreateOperationLog(item, "复制", BizLogType.PriceChange_Clone);

                #endregion

                ts.Complete();

                return masterSysNo;
            }
        }

        public void SavePriceChangeItem(PriceChangeItem item)
        {
            ObjectFactory<IPriceChangeDA>.Instance.SavePriceChangeItem(item);
        }


        public PriceChangeMaster GetPriceChangeBySysNo(int sysno)
        {
            return ObjectFactory<IPriceChangeDA>.Instance.GetPriceChangeBySysNo(sysno);
        }

        public List<PriceChangeMaster> GetPriceChangeByStatus(RequestPriceStatus status)
        {
            return ObjectFactory<IPriceChangeDA>.Instance.GetPriceChangeByStatus(status);
        }

        public void AuditPriceChange(PriceChangeMaster item)
        {
            PreCheckAudit(item);

            item.Status = RequestPriceStatus.Audited;

            UpdatePriceChangeStatus(item);

            #region write log

            CreateOperationLog(item, item.Status.ToDisplayText(), BizLogType.PriceChange_Audit);

            #endregion
        }

        public void VoidPriceChange(PriceChangeMaster item)
        {
            if (item.Status != RequestPriceStatus.Auditting)
            {
                //throw new BizException("只有待审核状态才能进行作废操作！");
                ThrowBizException("PriceChange_JustWaitAuditCanVoid");
            }

            item.Status = RequestPriceStatus.Void;

            UpdatePriceChangeStatus(item);

            #region write log

            CreateOperationLog(item, item.Status.ToDisplayText(),BizLogType.PriceChange_Void);

            #endregion
        }

        public void UpdatePriceChangeStatus(PriceChangeMaster item)
        {
            ObjectFactory<IPriceChangeDA>.Instance.UpdatePriceChangeStatus(item);
        }

        private void PreCheckSaveOrUpdate(PriceChangeMaster item)
        {
            item.ItemList.ForEach(p =>
            {
                if (item.PriceType == RequestPriceType.PurchasePrice && p.NewInstockPrice <= 0)
                {
                    //throw new BizException(string.Format("变价类型为采购价时，采购新价必填且不为0！"));
                    ThrowBizException("PriceChange_PurchasePriceNotNullWhenPurchase");
                }
                else if (item.PriceType == RequestPriceType.SalePrice && (p.NewShowPrice <= 0 && p.NewPrice <= 0))
                {
                    //throw new BizException("变价类型为销售价时，市场价或销售价其中之一必填且不为0!");
                    ThrowBizException("PriceChange_MarketPriceOrCurrentPriceNotNullWhenCurrent");
                }
            });

        }

        private bool IsExistsAuditedOrRuningProduct(PriceChangeItem item)
        {
            return ObjectFactory<IPriceChangeDA>.Instance.IsExistsAuditedOrRuningProduct(item);
        }

        private void PreCheckAudit(PriceChangeMaster item)
        {
            PriceChangeMaster info = GetPriceChangeBySysNo(item.SysNo);

            if (string.IsNullOrEmpty(item.AuditMemo))
            {
                //throw new BizException(string.Format("审核时需要输入审核备注"));
                ThrowBizException("PriceChange_AuditMemoNotNullWhenAudit");
            }

            if (info.Status != RequestPriceStatus.Auditting)
            {
                //throw new BizException("只有待审核状态才能进行审核操作！");
                ThrowBizException("PriceChange_JustWaitAuditCanAudit");
            }

            //审核的时候，去除校验新单中商品存在有效变价单据的逻辑，执行新单据的变价规则
            //string expMsg = string.Empty;

            //if (info.ItemList != null && info.ItemList.Count > 0)
            //{
            //    info.ItemList.ForEach(p =>
            //    {
            //        if (IsExistsAuditedOrRuningProduct(p))
            //        {
            //            expMsg += string.Format("状态为待启动或运行中的变价单存在商品【{0}】！\r\n", p.ProductID);
            //        }
            //    });
            //}

            //if (!string.IsNullOrEmpty(expMsg))
            //{
            //    throw new BizException(expMsg);
            //}
        }

        private void CreateOperationLog(PriceChangeMaster item, string action, BizLogType logType)
        {
            ExternalDomainBroker.CreateOperationLog(string.Format("用户：\"{0}\"{1}了编号为\"{2}\"类型为\"{3}\"的变价单", (string.IsNullOrEmpty(ServiceContext.Current.UserDisplayName) ? "Job" : ServiceContext.Current.UserDisplayName), action, item.SysNo, item.PriceType.ToDisplayText())
                , logType, item.SysNo, "8601");
        }

        public VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo)
        {
            return ExternalDomainBroker.GetVendorBasicInfoBySysNo(vendorSysNo);
        }

        #region JOB


        /// <summary>
        /// 启动变价单
        /// </summary>
        /// <param name="IsJobRun">true:自动中止；false:人工中止</param>
        public void RunPriceChange(PriceChangeMaster mst, bool IsJobRun)
        {
            if (mst != null)
            {
                PrecheckRun(mst);

                using (TransactionScope ts = new TransactionScope())
                {
                    if ((DateTime.Now > mst.BeginDate && DateTime.Now < mst.EndDate && IsJobRun)
                        || !IsJobRun)
                    {
                        if (mst.PriceType == RequestPriceType.PurchasePrice)
                        {
                            UpdatePriceWhenPurchaseType(mst, true);

                        }
                        else if (mst.PriceType == RequestPriceType.SalePrice)
                        {
                            UpdatePriceWhenSaleType(mst, true);
                        }


                        //新单压就单，使旧单的状态为不可用状态
                        foreach (var sub in mst.ItemList)
                        {
                            ObjectFactory<IPriceChangeDA>.Instance.DisableOldChangeItemStatusByNewItemSysNo(sub.SysNo);
                        }

                        // update realbegindate;
                        UpdateRealBeginDate(mst.SysNo);
                        CreateOperationLog(mst, "启动", BizLogType.PriceChange_Run);

                        ts.Complete();
                    }
                }
            }
        }

        /// <summary>
        /// 中止变价单
        /// </summary>
        /// <param name="IsJobRun">true:自动中止；false:人工中止</param>
        public void AbortedPriceChange(PriceChangeMaster mst, bool IsJobRun)
        {
            if (mst != null)
            {
                PreCheckAborted(mst);

                if ((DateTime.Now > mst.EndDate && IsJobRun) || !IsJobRun)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {

                        if (mst.PriceType == RequestPriceType.PurchasePrice)
                        {
                            UpdatePriceWhenPurchaseType(mst, false);
                        }
                        else if (mst.PriceType == RequestPriceType.SalePrice)
                        {
                            UpdatePriceWhenSaleType(mst, false);
                        }

                        // update status = aborted;
                        UpdatePriceChangeAborted(mst);

                        CreateOperationLog(mst, "终止", BizLogType.PriceChange_Run);

                        ts.Complete();

                    }
                }
            }
        }

        /// <summary>
        /// 销售变价时更新价格
        /// </summary>
        /// <param name="mst">变价单</param>
        /// <param name="IsRun">true:启动操作；false:中止操作</param>
        private void UpdatePriceWhenSaleType(PriceChangeMaster mst, bool IsRun)
        {
            if (mst.ItemList != null && mst.ItemList.Count > 0)
            {
                mst.ItemList.ForEach(p =>
                {
                    // 更新了市场价格
                    if (p.NewShowPrice != decimal.Zero)
                    {
                        if (IsRun)
                        {
                            // 启动操作更新市场价格
                            UpdateProductBasicPrice(p.ProductsysNo, p.NewShowPrice);
                        }
                        else
                        {
                            if (p.Status == PriceChangeItemStatus.Enable)
                            {
                                // 中止操作还原市场价格
                                UpdateProductBasicPrice(p.ProductsysNo, p.OldShowPrice);
                            }
                        }
                    }
                    // 更新了销售价格
                    if (p.NewPrice != decimal.Zero)
                    {
                        if (IsRun)
                        {
                            // 启动操作更新销售价格
                            UpdateProductCurrentPrice(p.ProductsysNo, p.NewPrice);
                        }
                        else
                        {
                            if (p.Status == PriceChangeItemStatus.Enable)
                            {
                                // 中止操作还原销售价格
                                UpdateProductCurrentPrice(p.ProductsysNo, p.OldPrice);
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 采购变价时更新价格
        /// </summary>
        /// <param name="mst">变价单</param>
        /// <param name="IsRun">true:启动操作；false:中止操作</param>
        private void UpdatePriceWhenPurchaseType(PriceChangeMaster mst, bool IsRun)
        {
            if (mst.ItemList != null && mst.ItemList.Count > 0)
            {
                mst.ItemList.ForEach(p =>
                {
                    if (IsRun)
                    {
                        // 启动操作更新采购价格
                        UpdateProductVirtualPrice(p.ProductsysNo, p.OldInstockPrice, p.NewInstockPrice);
                    }
                    else
                    {
                        if (p.Status == PriceChangeItemStatus.Enable)
                        {
                            // 中止操作还原采购价格
                            UpdateProductVirtualPrice(p.ProductsysNo, p.NewInstockPrice, p.OldInstockPrice);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 获取待启动的变价单
        /// </summary>
        /// <returns></returns>
        public List<PriceChangeMaster> GetAuditedPriceChangeInfos()
        {
            return GetPriceChangeByStatus(RequestPriceStatus.Audited);
        }

        /// <summary>
        /// 获取运行中的变价单
        /// </summary>
        /// <returns></returns>
        public List<PriceChangeMaster> GetRunningProceChangeInfos()
        {
            return GetPriceChangeByStatus(RequestPriceStatus.Running);
        }

        /// <summary>
        /// 更新商品正常采购价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="originalVirtualPrice"></param>
        /// <param name="newVirtualPrice"></param>
        public virtual void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice)
        {
            ExternalDomainBroker.UpdateProductVirtualPrice(productSysNo, originalVirtualPrice, newVirtualPrice);
        }

        /// <summary>
        /// 更新市场价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="newPrice"></param>
        public virtual void UpdateProductBasicPrice(int productSysNo, decimal newPrice)
        {
            ExternalDomainBroker.UpdateProductBasicPrice(productSysNo, newPrice);
        }

        /// <summary>
        /// 更新销售价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="newPrice"></param>
        public virtual void UpdateProductCurrentPrice(int productSysNo, decimal newPrice)
        {
            ExternalDomainBroker.UpdateProductCurrentPrice(productSysNo, newPrice);
        }

        /// <summary>
        /// 更新变价单实际开始时间
        /// </summary>
        /// <param name="sysNo"></param>
        private void UpdateRealBeginDate(int sysNo)
        {
            ObjectFactory<IPriceChangeDA>.Instance.UpdateRealBeginDate(sysNo);
        }

        /// <summary>
        /// 中止变价单
        /// </summary>
        /// <param name="master"></param>
        public virtual void UpdatePriceChangeAborted(PriceChangeMaster master)
        {
            master.Status = RequestPriceStatus.Aborted;

            UpdatePriceChangeStatus(master);
        }

        private void PrecheckRun(PriceChangeMaster mst)
        {
            if (mst.Status != RequestPriceStatus.Audited)
            {
                //throw new BizException(string.Format("单据编号：【{0}】的变价单为{1}状态，只有待启动状态才允许启动！", mst.SysNo, mst.Status.ToDisplayText()));
                ThrowBizException("PriceChange_JustWaitStartCanStart", mst.SysNo, mst.Status.ToDisplayText());
            }
        }

        private void PreCheckAborted(PriceChangeMaster mst)
        {
            if (mst.Status != RequestPriceStatus.Running)
            {
                //throw new BizException(string.Format("单据编号：【{0}】的变价单为{1}状态，只有运行中的状态才可以中止！", mst.SysNo, mst.Status.ToDisplayText()));
                ThrowBizException("PriceChange_JustWaitStartCanStop",mst.SysNo, mst.Status.ToDisplayText());
            }

        }


        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.SOIncome, msgKeyName), args);
        }
        #endregion
    }
}
