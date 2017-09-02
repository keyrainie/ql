using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.Service.PO.IDataAccess;
using System.Transactions;
using System.Data;
using System.Text.RegularExpressions;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 虚库采购单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(VSPOProcessor))]
    public class VSPOProcessor
    {
        #region [Fields]
        private IVirtualPurchaseOrderDA m_VSPurchaseOrderDA;
        private ISOBizInteract m_SOBizInteract;
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

        public ISOBizInteract SOBizInteract
        {
            get
            {
                if (null == m_SOBizInteract)
                {
                    m_SOBizInteract = ObjectFactory<ISOBizInteract>.Instance;
                }
                return m_SOBizInteract;
            }
        }

        public IVirtualPurchaseOrderDA VSPurchaseOrderDA
        {
            get
            {
                if (null == m_VSPurchaseOrderDA)
                {
                    m_VSPurchaseOrderDA = ObjectFactory<IVirtualPurchaseOrderDA>.Instance;
                }
                return m_VSPurchaseOrderDA;
            }
        }
        #endregion


        /// <summary>
        /// 虚库采购单 - 创建
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        public virtual VirtualStockPurchaseOrderInfo CreateVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            //验证
            vspoInfo.PurchaseQty = 1;
            vspoInfo.Status = VirtualPurchaseOrderStatus.Normal; //初始状态
            vspoInfo.CreateTime = DateTime.Now;
            vspoInfo.InStockOrderType = VirtualPurchaseInStockOrderType.PO;

            if (vspoInfo.SOItemSysNo == null || !vspoInfo.SOItemSysNo.HasValue)
            {
                //销售单Item编号不能为空！
                throw new BizException(GetMessageString("VSPO_ItemSysNoEmpty"));
            }


            //不能再生成虚库商品采购单
            int i = VSPurchaseOrderDA.CalcVSPOQuantity(vspoInfo.SOItemSysNo.Value);
            if (i <= 0)
            {
                //不能再生成虚库商品采购单
                throw new BizException(GetMessageString("VSPO_CannotCreateVSPO"));
            }

            for (int j = 1; j <= i; j++)
            {
                //生成虚库采购单
                vspoInfo.SysNo = VSPurchaseOrderDA.CreateVSPO(vspoInfo).SysNo;

                //写LOG： CommonService.WriteLog<VSPOEntity>(entity, " created VSPO ", entity.SysNo.Value.ToString(), (int)LogType.St_SOVirtualItemRequest_Add);

                ExternalDomainBroker.CreateLog(" created VSPO "
           , BizEntity.Common.BizLogType.St_SOVirtualItemRequest_Add
           , vspoInfo.SysNo.Value
           , vspoInfo.CompanyCode);
            }

            //邮件发送
            SendEmailWhenCreate(vspoInfo);

            //调用Order接口,生成虚库采购单后将对应的订单标识为备货状态
            ExternalDomainBroker.UpdateSOCheckShipping(vspoInfo.SOSysNo.Value, VirtualPurchaseOrderStatus.Close);

            return vspoInfo;
        }

        /// <summary>
        /// 虚库采购单 - 更新
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        public virtual VirtualStockPurchaseOrderInfo UpdateVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            #region [Check实体逻辑]
            //单据号不能为空
            if (!vspoInfo.InStockOrderSysNo.HasValue)
            {
                //单据号不能为空
                throw new BizException(GetMessageString("VSPO_OrderSysNoEmpty"));
            }

            //请输入估计到达时间
            if (!vspoInfo.EstimateArriveTime.HasValue)
            {
                //估计到达时间不能为空
                throw new BizException(GetMessageString("VSPO_EATimeEmpty"));
            }

            //预计到货时间 -小时分 不能为空
            if (vspoInfo.EstimateArriveTime.Value.ToShortTimeString() == "00:00" || vspoInfo.EstimateArriveTime.Value < Convert.ToDateTime("1900-01-01"))
            {
                throw new BizException(GetMessageString("VSPO_EATLongTimeEmpty"));
            }

            //请输入正确的10位移仓单号
            if (vspoInfo.InStockOrderType.Value == VirtualPurchaseInStockOrderType.Shift && vspoInfo.InStockOrderSysNo.ToString().Length != 10)
            {
                //移仓单号格式不正确
                throw new BizException(GetMessageString("VSPO_ShiftSysNoFormatWrong"));
            }

            //请输入正确的10位转换单号
            if (vspoInfo.InStockOrderType.Value == VirtualPurchaseInStockOrderType.Convert && vspoInfo.InStockOrderSysNo.ToString().Length != 10)
            {
                //转换单号格式不正确
                throw new BizException(GetMessageString("VSPO_ConvertSysNoFormatWrong"));
            }

            //请输入正确的小于8位采购单号
            if (vspoInfo.InStockOrderType.Value == VirtualPurchaseInStockOrderType.PO && vspoInfo.InStockOrderSysNo.ToString().Length > 8)
            {
                //采购单号格式不正确
                throw new BizException(GetMessageString("VSPO_POSysNoFormatWrong"));
            }
            #endregion

            //虚库采购单为作废状态，不能更新:
            VirtualStockPurchaseOrderInfo localEntiy = VSPurchaseOrderDA.LoadVSPO(vspoInfo.SysNo.Value);
            if (localEntiy.Status.HasValue && localEntiy.Status.Value == VirtualPurchaseOrderStatus.Abandon)
            {
                //虚库采购单为作废状态，不能更新!
                throw new BizException(GetMessageString("VSPO_AbandonVSPO_Invalid_Update"));
            }
            #region [根据入库单类型，检查单据状态 以及单据对应商品是否存在:]
            if (localEntiy.InStockOrderType.Value != VirtualPurchaseInStockOrderType.PO)
            {
                localEntiy.InStockOrderSysNo = Convert.ToInt32(localEntiy.InStockOrderSysNo.Value.ToString().Substring(2));
            }

            //验证对应的PO中是否有对应的商品:
            if (localEntiy.InStockOrderType.Value == VirtualPurchaseInStockOrderType.PO)
            {
                if (!VSPurchaseOrderDA.ValidateFromPO(vspoInfo.SysNo.Value, Convert.ToInt32(vspoInfo.InStockOrderSysNo)))
                {
                    //在您输入的采购单中找不到对应的商品或该采购单已经作废！
                    throw new BizException(GetMessageString("VSPO_PONotFound"));
                }
            }

            if (localEntiy.InStockOrderType.Value == VirtualPurchaseInStockOrderType.Shift)
            {
                if (!VSPurchaseOrderDA.ValidateFromShift(vspoInfo.SysNo.Value, Convert.ToInt32(vspoInfo.InStockOrderSysNo)))
                {
                    //在您输入的移仓单中找不到对应的商品或该移仓单已经作废！
                    throw new BizException(GetMessageString("VSPO_ShiftNotFound"));
                }
            }


            if (localEntiy.InStockOrderType.Value == VirtualPurchaseInStockOrderType.Convert)
            {
                if (!VSPurchaseOrderDA.ValidateFromTransfer(vspoInfo.SysNo.Value, Convert.ToInt32(vspoInfo.InStockOrderSysNo)))
                {
                    //在您输入的转换单中找不到对应的商品或该转换单已经作废！
                    throw new BizException(GetMessageString("VSPO_ConvertNotFound"));
                }
            }
            #endregion

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {

                localEntiy.Status = VirtualPurchaseOrderStatus.Close;

                if (vspoInfo.PMMemo.Trim().Length > 0)
                {
                    localEntiy.PMMemo = vspoInfo.PMMemo.Trim();
                }
                localEntiy.EstimateArriveTime = vspoInfo.EstimateArriveTime;
                localEntiy.InStockOrderSysNo = vspoInfo.InStockOrderSysNo;
                localEntiy.InStockOrderType = vspoInfo.InStockOrderType;

                VSPurchaseOrderDA.UpdateVSPO(localEntiy);
                //在保存，作废，CSMemo的时候 邮件发送
                SendEmailWhenUpdate(PurchaseOrderOperationType.Update, localEntiy);

                //写LOG: CommonService.WriteLog<VSPOEntity>(entity, " Updated VSPO ", entity.SysNo.Value.ToString(), (int)LogType.St_SOVirtualItemRequest_Save);
                ExternalDomainBroker.CreateLog(" Updated VSPO "
     , BizEntity.Common.BizLogType.St_SOVirtualItemRequest_Save
     , vspoInfo.SysNo.Value
     , vspoInfo.CompanyCode);
                scope.Complete();
            }
            return vspoInfo;
        }

        /// <summary>
        /// 虚库采购单 - 作废
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        public virtual VirtualStockPurchaseOrderInfo AdandonVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            #region [Check实体逻辑]
            //虚库采购单的编号不能为空:
            if (!vspoInfo.SysNo.HasValue)
            {
                //虚库采购单的编号不能为空
                throw new BizException(GetMessageString("VSPO_VSPOSysNoEmpty"));
            }
            #endregion

            //虚库采购单为作废状态，不能再作废
            VirtualStockPurchaseOrderInfo localEntiy = VSPurchaseOrderDA.LoadVSPO(vspoInfo.SysNo.Value);

            if (localEntiy == null)
            {
                ///该虚库采购单在数据中不存在,操作失败!
                throw new BizException(GetMessageString("VSPO_VSPONotFound"));
            }

            if (localEntiy.Status.HasValue && localEntiy.Status.Value == VirtualPurchaseOrderStatus.Abandon)
            {
                //虚库采购单为作废状态，不能再作废!
                throw new BizException(GetMessageString("VSPO_Abandon_Invalid_Abandon"));
            }

            if (vspoInfo.InStockOrderType.Value != VirtualPurchaseInStockOrderType.PO)
            {
                vspoInfo.InStockOrderSysNo = Convert.ToInt32(vspoInfo.InStockOrderSysNo.Value.ToString().Substring(2));
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                string strInfo = string.Empty;
                if (vspoInfo.InStockOrderType == VirtualPurchaseInStockOrderType.Shift)
                {
                    int isCanAbadon = 0;
                    int shiftsysno = 0;

                    //调用Inventory接口,判断对应的自动移仓单是否已出库
                    bool isShift = ExternalDomainBroker.IsAutoShift(vspoInfo.InStockOrderSysNo.Value, out isCanAbadon, out shiftsysno);
                    if (isShift)
                    {
                        if (isCanAbadon == 1)
                        {
                            //调用Invoice接口,设置 St_Shift 作废
                            ExternalDomainBroker.AbandonShift(shiftsysno);
                        }
                        if (isCanAbadon == 0)
                        {
                            //请注意：对应的自动移仓单已出库!
                            strInfo = GetMessageString("VSPO_OutStock_Note");
                        }
                    }
                }

                if (vspoInfo.InStockOrderType.Value != VirtualPurchaseInStockOrderType.PO)
                {
                    vspoInfo.InStockOrderSysNo = Convert.ToInt32(vspoInfo.InStockOrderSysNo.Value.ToString().Substring(2));
                }

                //更新状态操作:
                vspoInfo.Status = VirtualPurchaseOrderStatus.Abandon;
                VSPurchaseOrderDA.AbandonVSPO(vspoInfo);

                //邮件收发
                ////成功作废虚库商品采购单,并发邮件通知PM负责人员
                //vspoInfo.Messges = strInfo + MessageResource.VSPO_Check_ConfirmPMRequiredValue;


                //如果销售单没有生成虚库采购单：
                int getExistSOCount = VSPurchaseOrderDA.CheckExistsSOVirtualItemRequest(vspoInfo.SOSysNo.Value);
                if (getExistSOCount == 0)
                {
                    //调用Order接口,作废虚库采购单后将对应的订单标识为未备货状态:
                    ExternalDomainBroker.UpdateSOCheckShipping(vspoInfo.SOSysNo.Value, VirtualPurchaseOrderStatus.Abandon);
                }

                //发送邮件:
                SendEmailWhenUpdate(PurchaseOrderOperationType.Abandon, vspoInfo);

                //写LOG：CommonService.WriteLog<VSPOEntity>(entity, " Abandon VSPO ", entity.SysNo.Value.ToString(), (int)LogType.St_SOVirtualItemRequest_Abandon);
                ExternalDomainBroker.CreateLog(" Abandon VSPO "
, BizEntity.Common.BizLogType.St_SOVirtualItemRequest_Abandon
, vspoInfo.SysNo.Value
, vspoInfo.CompanyCode);
                scope.Complete();
            }
            return vspoInfo;
        }

        /// <summary>
        /// 加载虚库采购单信息
        /// </summary>
        /// <param name="vspoSysNo"></param>
        /// <returns></returns>
        public virtual VirtualStockPurchaseOrderInfo LoadVSPOInfo(int vspoSysNo)
        {
            return VSPurchaseOrderDA.LoadVSPO(vspoSysNo);
        }

        /// <summary>
        /// 虚库采购单 - 更新CS备注
        /// </summary>
        /// <param name="vspoInfo"></param>
        /// <returns></returns>
        public virtual VirtualStockPurchaseOrderInfo UpdateCSMemoForVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            #region [Check实体逻辑]

            if (!vspoInfo.SysNo.HasValue)
            {
                //虚库采购单的编号不能为空
                throw new BizException(GetMessageString("VSPO_VSPOSysNoEmpty"));
            }

            //CS备注不能为空
            if (vspoInfo.CSMemo == null || vspoInfo.CSMemo.Trim() == string.Empty)
            {
                //CS备注不能为空
                throw new BizException(GetMessageString("VSPO_CSMemoEmpty"));
            }

            VirtualStockPurchaseOrderInfo localEntity = VSPurchaseOrderDA.LoadVSPO(vspoInfo.SysNo.Value);

            if (localEntity == null)
            {
                //该虚库采购单在数据中不存在,操作失败
                throw new BizException(GetMessageString("VSPO_VSPONotFound"));
            }
            #endregion

            //更新操作:
            localEntity.CSMemo = vspoInfo.CSMemo;
            VSPurchaseOrderDA.UpdateVSPO(localEntity);

            //邮件发送:
            SendEmailWhenUpdate(PurchaseOrderOperationType.UpDateCSMemo, vspoInfo);

            //写LOG：CommonService.WriteLog<VSPOEntity>(entity, " updated CSMemo for VSPO ", entity.SysNo.Value.ToString(), (int)LogType.St_SOVirtualItemRequest_UpdateCSMemo);

            ExternalDomainBroker.CreateLog(" Updated CSMemo for VSPO "
, BizEntity.Common.BizLogType.St_SOVirtualItemRequest_UpdateCSMemo
, vspoInfo.SysNo.Value
, vspoInfo.CompanyCode);
            return vspoInfo;
        }


        public virtual VirtualStockPurchaseOrderInfo LoadVirtualPurchaseInfoBySOItemSysNo(int soSysNo, int productSysNo)
        {
            int soItemSysNo = 0;
            string productID = string.Empty;
            string productName = string.Empty;
            int pmUserSysNo = 0;
            SOInfo getCurrentSOInfo = SOBizInteract.GetSOInfo(soSysNo);
            if (null != getCurrentSOInfo)
            {
                SOItemInfo item = getCurrentSOInfo.Items.SingleOrDefault(x => x.ProductSysNo == productSysNo);
                if (null != item)
                {
                    ProductInfo getProductInfo = IMBizInteract.GetProductInfo(item.ProductSysNo.Value);
                    soItemSysNo = item.SysNo.Value;
                    productID = item.ProductID;
                    productName = item.ProductName;
                    pmUserSysNo = getProductInfo.ProductBasicInfo.ProductManager.SysNo.Value;
                }
                else
                {
                    throw new BizException("订单明细号错误!");
                }
            }
            else
            {
                throw new BizException("订单明细号错误!");
            }

            VirtualStockPurchaseOrderInfo memoInfo = null;
            int isExist = SOBizInteract.GetGeneratedSOVirtualCount(soItemSysNo);
            if (isExist > 0)
            {
                memoInfo = VSPurchaseOrderDA.GetMemoInfoFromVirtualRequest(soItemSysNo);
            }
            int needPurchaseQty = VSPurchaseOrderDA.CalcVSPOQuantity(soItemSysNo);
            VirtualStockPurchaseOrderInfo returnEntity = new VirtualStockPurchaseOrderInfo()
            {
                PMMemo = (memoInfo == null ? string.Empty : memoInfo.PMMemo),
                CSMemo = (memoInfo == null ? string.Empty : memoInfo.CSMemo),
                SOSysNo = getCurrentSOInfo.SysNo.Value,
                ProductID = productID,
                ProductName = productName,
                ProductSysNo = productSysNo,
                PMUserSysNo = pmUserSysNo,
                PurchaseQty = isExist > 0 ? isExist : needPurchaseQty,
                SOItemSysNo = soItemSysNo,
                SOVirtualCount = isExist
            };
            return returnEntity;
        }

        public virtual bool IsVSPOItemPriceLimited(int soSysNo, int productSysNo, int purchaseQty)
        {
            decimal price = 0m;
            SOInfo getCurrentSOInfo = SOBizInteract.GetSOInfo(soSysNo);
            if (null != getCurrentSOInfo)
            {
                SOItemInfo item = getCurrentSOInfo.Items.SingleOrDefault(x => x.ProductSysNo == productSysNo);
                if (null != item)
                {
                    if (item.Price.HasValue)
                    {
                        price = item.Price.Value;
                    }
                    if (price >= 2000 && purchaseQty >= 3)
                    {
                        return true;
                    }
                    if (purchaseQty * price >= 15000)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 在保存，作废，CSMemo的时候 邮件发送
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="entity"></param>
        private void SendEmailWhenUpdate(PurchaseOrderOperationType operationType, VirtualStockPurchaseOrderInfo entity)
        {
            //调用EmailService发送邮件:
            string getMessageFromAddress = AppSettingManager.GetSetting("PO", "VSPO_Message_Email_From");
            string getMessageToAddress = AppSettingManager.GetSetting("PO", "VSPO_Message_Email_To");


            DataTable dt = VSPurchaseOrderDA.GetEmailContentForUpdateVSPO(entity.SysNo.Value);

            if (dt.Rows.Count > 0)
            {
                string toMailAddress = string.Empty;
                string mailBody;
                //显示产品仓库名称
                string WarehouseName = null;
                if (dt.Rows[0]["WarehouseNumber"] != null)
                {
                    WarehouseName = dt.Rows[0]["WarehouseNumber"].ToString();
                }
                else
                {
                    WarehouseName = string.Empty;
                }
                #region     根据不同的OperationType来定义不同的标题
                string mailSubject = string.Empty;
                switch (operationType)
                {
                    case PurchaseOrderOperationType.Update:
                        if (dt.Rows[0]["InStockOrderSysNo"] == null || dt.Rows[0]["InStockOrderSysNo"].ToString().Trim() == string.Empty || dt.Rows[0]["InStockOrderSysNo"].ToString() == "0")
                        {
                            mailSubject = string.Format(GetMessageString("VSPO_UpdateVSPO_POMemo"), dt.Rows[0]["SysNo"].ToString());
                        }
                        else
                        {
                            mailSubject = string.Format(GetMessageString("VSPO_UpdateVSPO_ETA"), dt.Rows[0]["SysNo"].ToString());
                        }

                        mailSubject = string.Empty + DateTime.Now + string.Empty + mailSubject + string.Empty;
                        break;
                    case PurchaseOrderOperationType.UpDateCSMemo:
                        mailSubject = GetMessageString("VSPO_UpdateVSPO_CSMemo");//虚库采购单CS备注更新
                        break;
                    case PurchaseOrderOperationType.Abandon:
                        mailSubject = GetMessageString("VSPO_UpdateVSPO_Abandon") + dt.Rows[0]["SysNo"].ToString() + string.Empty;
                        mailSubject = string.Empty + DateTime.Now + string.Empty + mailSubject + string.Empty;
                        break;
                }
                #endregion

                KeyValueVariables keyValuesList = new KeyValueVariables();
                //**邮件标题:
                keyValuesList.Add("MailSubject", mailSubject);
                keyValuesList.Add("SysNo", dt.Rows[0]["SysNo"]);
                keyValuesList.Add("SOSysNo", dt.Rows[0]["SOSysNo"]);
                keyValuesList.Add("ProductID", dt.Rows[0]["ProductID"]);
                keyValuesList.Add("BriefName", dt.Rows[0]["BriefName"]);
                keyValuesList.Add("PurchaseQty", dt.Rows[0]["PurchaseQty"]);
                keyValuesList.Add("InStockOrderSysNo", dt.Rows[0]["InStockOrderSysNo"]);
                keyValuesList.Add("EstimateArriveTime", dt.Rows[0]["EstimateArriveTime"]);
                keyValuesList.Add("PMHandlerUserName", dt.Rows[0]["PMHandlerUserName"]);
                keyValuesList.Add("CreateTime", dt.Rows[0]["CreateTime"]);
                keyValuesList.Add("PMUserName", dt.Rows[0]["PMUserName"]);
                keyValuesList.Add("CSUserName", dt.Rows[0]["CSUserName"]);
                keyValuesList.Add("CSMemo", dt.Rows[0]["CSMemo"]);
                keyValuesList.Add("PMMemo", dt.Rows[0]["PMMemo"]);
                keyValuesList.Add("WarehouseName", WarehouseName);

                if (dt.Rows[0]["EmailAddress"] != null && dt.Rows[0]["EmailAddress"].ToString() != string.Empty)
                {
                    toMailAddress = dt.Rows[0]["EmailAddress"].ToString();
                }

                //发送异步，内部邮件:
                EmailHelper.SendEmailByTemplate(toMailAddress, "VSPO_UpdateActionMail", keyValuesList, true);
            }
        }

        /// <summary>
        ///  提供给创建时候 邮件发送
        /// </summary>
        /// <param name="entity"></param>
        private void SendEmailWhenCreate(VirtualStockPurchaseOrderInfo entity)
        {
            //调用EmailService发送邮件:
            DataTable dt = new DataTable();
            dt = VSPurchaseOrderDA.GetEmailContentForCreateVSPO(entity.SOSysNo.Value, entity.ProductSysNo.Value);
            string toEmailAddress = dt.Rows[0]["EmailAddress"].ToString();//PM负责人Email
            string backUPMailAddress = string.Empty;
            if (dt.Rows[0]["BackupUserList"] != null)
            {
                string[] arrayUserSysNo = dt.Rows[0]["BackupUserList"].ToString().Split(';');

                for (int n = 0; n < arrayUserSysNo.Length; n++)
                {
                    if (Regex.IsMatch(arrayUserSysNo[n], "^[0-9]+$"))
                    {
                        string email = VSPurchaseOrderDA.GetBackUpPMEmailAddress(Convert.ToInt32(arrayUserSysNo[n]));
                        if (email != null && Regex.IsMatch(email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                        {
                            backUPMailAddress += email + ";";
                        }

                    }
                }
            }
            if (backUPMailAddress.Length > 0)
            {
                toEmailAddress = toEmailAddress + ";" + backUPMailAddress;//PM负责人Email Address(加上BackUp)
            }


            KeyValueVariables keyValuesList = new KeyValueVariables();
            //**邮件标题:
            keyValuesList.Add("DateTime", DateTime.Now.ToString());
            keyValuesList.Add("SysNo", dt.Rows[0]["SysNo"]);
            keyValuesList.Add("SOSysNo", dt.Rows[0]["SOSysNo"]);
            keyValuesList.Add("ProductID", dt.Rows[0]["ProductID"]);
            keyValuesList.Add("BriefName", dt.Rows[0]["BriefName"]);
            keyValuesList.Add("PurchaseQty", dt.Rows[0]["PurchaseQty"]);
            keyValuesList.Add("CreateTime", dt.Rows[0]["CreateTime"]);
            keyValuesList.Add("PMUserName", dt.Rows[0]["PMUserName"]);
            keyValuesList.Add("CSUserName", dt.Rows[0]["CSUserName"]);
            keyValuesList.Add("CSMemo", dt.Rows[0]["CSMemo"]);
            keyValuesList.Add("WarehouseName", "");
            //发送异步，内部邮件:
            EmailHelper.SendEmailByTemplate(toEmailAddress, "VSPO_CreateActionMail", keyValuesList, true);

        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.VirtualPurchaseOrder", key);
        }
    }
}
