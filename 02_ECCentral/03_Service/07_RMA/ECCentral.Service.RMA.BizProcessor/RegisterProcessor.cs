using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
//using ECCentral.Service.ThirdPart.Interface;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(RegisterProcessor))]
    public class RegisterProcessor
    {
        private const string IDREGEX = @"S?A?\w{2}-\w{3}-\w{3}(-\w{2})?(_\w{3})?";

        private IRegisterDA registerDA = ObjectFactory<IRegisterDA>.Instance;

        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo UpdateBasicInfo(RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }
            if (!register.BasicInfo.SysNo.HasValue)
            {
                throw new ArgumentNullException("register.BasicInfo.SysNo");
            }
            RMARegisterInfo originRegister = LoadBySysNo(register.BasicInfo.SysNo.Value);

            register.VerifyUpdate(originRegister);

            if (originRegister.BasicInfo.NextHandler != register.BasicInfo.NextHandler)
            {
                BizLogType? logType = null;
                logType = GetNextHandlerLogType(register.BasicInfo.NextHandler);

                if (logType.HasValue)
                {
                    ExternalDomainBroker.CreateOperationLog(logType.ToString(), logType.Value, register.SysNo.Value, originRegister.CompanyCode);
                }
            }
            registerDA.UpdateBasicInfo(register);

            return LoadBySysNo(register.BasicInfo.SysNo.Value);
        }

        /// <summary>
        /// 更新全部信息
        /// </summary>
        /// <param name="register"></param>
        public virtual void Update(RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }
            if (!register.BasicInfo.SysNo.HasValue)
            {
                throw new ArgumentNullException("register.SysNo");
            }
            RMARegisterInfo originRegister = LoadBySysNo(register.BasicInfo.SysNo.Value);

            register.VerifyUpdate(originRegister);

            if (originRegister.BasicInfo.NextHandler != register.BasicInfo.NextHandler)
            {
                BizLogType? logType = GetNextHandlerLogType(register.BasicInfo.NextHandler);

                if (logType.HasValue)
                {
                    ExternalDomainBroker.CreateOperationLog(logType.ToString(), logType.Value, register.SysNo.Value, originRegister.CompanyCode);
                }
            }
            registerDA.PurelyUpdate(register);
        }

        /// <summary>
        /// 更新检测信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo UpdateCheckInfo(RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }

            register.CheckInfo.CheckUserSysNo = ServiceContext.Current.UserSysNo;
            register.CheckInfo.CheckTime = DateTime.Now;

            CommonCheck.VerifyNotNull("SysNo", register.CheckInfo.SysNo);

            RMARegisterInfo origin = LoadBySysNo(register.CheckInfo.SysNo.Value);
            if (origin.BasicInfo.Status == RMARequestStatus.Complete || origin.BasicInfo.Status == RMARequestStatus.Abandon)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotEditWhenClosedOrAbandon");
                throw new BizException(msg);
            }
            CommonCheck.VerifyNotNull("CheckUserSysNo", register.CheckInfo.CheckUserSysNo);

            string userName = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo).UserDisplayName;
            register.CheckInfo.CheckDesc = string.Format(
                "{0}\r\n{1} {2}",
                register.CheckInfo.CheckDesc,
                userName,
                DateTime.Now.ToString("yyyy-MM-dd")
            );

            register.VerifyFieldsLength();

            registerDA.UpdateCheckInfo(register);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Check", BizLogType.RMA_Register_Check, register.SysNo.Value, origin.CompanyCode);

            return LoadForEditBySysNo(register.CheckInfo.SysNo.Value);
        }

        /// <summary>
        /// 更新返还信息
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo UpdateResponseInfo(RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }
            CommonCheck.VerifyNotNull("SysNo", register.ResponseInfo.SysNo);

            RMARegisterInfo origin = LoadBySysNo(register.ResponseInfo.SysNo.Value);
            if (origin.BasicInfo.Status == RMARequestStatus.Complete || origin.BasicInfo.Status == RMARequestStatus.Abandon)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotEditWhenClosedOrAbandon");
                throw new BizException(msg);
            }

            CommonCheck.VerifyNotNull("ResponseProductNo", register.ResponseInfo.ResponseProductNo);

            string userName = ExternalDomainBroker.GetUserInfo(ServiceContext.Current.UserSysNo).UserDisplayName;

            register.ResponseInfo.ResponseDesc = string.Format(
                "{0}\r\n{1} {2}",
                register.ResponseInfo.ResponseDesc, userName, DateTime.Now.ToString("yyyy-MM-dd")
            );
            register.VerifyFieldsLength();

            registerDA.UpdateResponseInfo(register);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_ResponseInfo", BizLogType.RMA_Register_ResponseInfo, register.SysNo.Value, origin.CompanyCode);

            return LoadForEditBySysNo(register.ResponseInfo.SysNo.Value);
        }

        /// <summary>
        /// 待退款
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo SetWaitingRefund(int sysNo)
        {
            if (!registerDA.CanWaitingRefund(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotSetWaitingRefund");
                throw new BizException(msg);
            }

            RMARegisterInfo register = LoadBySysNo(sysNo);
            //同步到ERP
            SyncERPAction(register);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                registerDA.UpdateRefundStatus(sysNo, RMARefundStatus.WaitingRefund);

                ExternalDomainBroker.CreateOperationLog("RMA_Register_Refund", BizLogType.RMA_Register_Refund, sysNo, register.CompanyCode);

                ts.Complete();
            }

            return LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 取消待退款
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual bool CancelWaitingRefund(int sysNo)
        {
            if (!registerDA.CanCancelWaitingRefund(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelWaitingRefund");
                throw new BizException(msg);
            }

            bool result = registerDA.UpdateRefundStatus(sysNo, null);

            RMARegisterInfo register = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Refund", BizLogType.RMA_Register_Refund, sysNo, register.CompanyCode);

            return result;
        }

        /// <summary>
        /// 待退货
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo SetWaitingReturn(int sysNo)
        {
            if (!registerDA.CanWaitingReturn(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotSetWaitingReturn");
                throw new BizException(msg);
            }

            registerDA.UpdateReturnStatus(sysNo, RMAReturnStatus.WaitingReturn);

            RMARegisterInfo register = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Return", BizLogType.RMA_Register_Return, sysNo, register.CompanyCode);

            return LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 取消待退货
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual bool CancelWaitingReturn(int sysNo)
        {
            if (!registerDA.CanCancelWaitingReturn(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelWaitingReturn");
                throw new BizException(msg);
            }

            bool result = registerDA.UpdateReturnStatus(sysNo, null);

            RMARegisterInfo register = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Return", BizLogType.RMA_Register_Return, sysNo, register.CompanyCode);

            return result;
        }

        /// <summary>
        /// 待发货
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo SetWaitingRevert(RMARegisterInfo register)
        {
            register.VerifyWaitingRevert();

            RMARegisterInfo original = LoadBySysNo(register.RevertInfo.SysNo.Value);

            original.RevertInfo.NewProductStatus = register.RevertInfo.NewProductStatus;
            original.RevertInfo.SetWaitingRevertTime = DateTime.Now;
            // 非换货时 RevertStockSysNo 留空
            original.RevertInfo.RevertStockSysNo = register.RevertInfo.NewProductStatus == RMANewProductStatus.Origin
                ? null
                : register.RevertInfo.RevertStockSysNo;
            // 非换货或者调新品时，直接将原始 ProductSysNo 设置到 RevertProductSysNo
            original.RevertInfo.RevertProductSysNo = (register.RevertInfo.NewProductStatus == RMANewProductStatus.Origin || register.RevertInfo.NewProductStatus == RMANewProductStatus.NewProduct)
                ? register.BasicInfo.ProductSysNo
                : register.RevertInfo.RevertProductSysNo;
            // 非当前 Case 产品需要进行审批
            original.RevertInfo.RevertStatus = register.RevertInfo.NewProductStatus == RMANewProductStatus.OtherProduct
                ? RMARevertStatus.WaitingAudit
                : RMARevertStatus.WaitingRevert;
            //同步到ERP
            SyncERPAction(original);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                registerDA.UpdateRevertStatus(original);

                ExternalDomainBroker.CreateOperationLog("RMA_Register_Revert", BizLogType.RMA_Register_Revert, register.SysNo.Value, original.CompanyCode);

                ts.Complete();
            }

            return LoadBySysNo(register.SysNo.Value);
        }

        /// <summary>
        /// 取消待发货
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="revertAudit"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual bool CancelWaitingRevert(int sysNo, bool revertAudit, params RMARegisterInfo[] register)
        {
            if (!registerDA.CanCancelWaitingRevert(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelWaitingRevert");
                throw new BizException(msg);
            }
            RMARegisterInfo original = LoadBySysNo(sysNo);
            RegisterRevertInfo revertInfo = original.RevertInfo;
            revertInfo.RevertStatus = null;
            revertInfo.NewProductStatus = RMANewProductStatus.Origin;
            revertInfo.RevertProductSysNo = null;
            revertInfo.RevertStockSysNo = null;
            revertInfo.SetWaitingRevertTime = null;
            if (revertAudit)
            {
                if (register != null && register.Length > 0)
                {
                    revertInfo.RevertAuditUserSysNo = register[0].RevertInfo.RevertAuditUserSysNo;
                    revertInfo.RevertAuditMemo = register[0].RevertInfo.RevertAuditMemo;
                    revertInfo.RevertAuditTime = register[0].RevertInfo.RevertAuditTime;
                }
            }
            else
            {
                revertInfo.RevertAuditUserSysNo = null;
                revertInfo.RevertAuditMemo = null;
                revertInfo.RevertAuditTime = null;
            }

            bool result = registerDA.UpdateRevertStatus(original);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Revert", BizLogType.RMA_Register_Revert, sysNo, original.CompanyCode);

            return result;
        }

        /// <summary>
        /// 发还审核
        /// </summary>
        /// <param name="register"></param>
        /// <param name="approved">是否通过</param>
        /// <returns></returns>
        public virtual RMARegisterInfo RevertAudit(RMARegisterInfo register, bool approved)
        {
            register.VerifyRevertAudit();

            bool result;
            if (approved)
            {
                register.RevertInfo.RevertStatus = RMARevertStatus.WaitingRevert;
                register.RevertInfo.RevertAuditUserSysNo = ServiceContext.Current.UserSysNo;
                register.RevertInfo.RevertAuditTime = DateTime.Now;

                result = registerDA.UpdateRevertStatus(register);
            }
            else
            {
                result = CancelWaitingRevert(register.RevertInfo.SysNo.Value, true, register);
            }


            ExternalDomainBroker.CreateOperationLog("RMA_Register_RevertAudit", BizLogType.RMA_Register_RevertAudit, register.SysNo.Value, register.CompanyCode);

            return LoadForEditBySysNo(register.SysNo.Value);
        }

        /// <summary>
        /// 待送修
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo SetWaitingOutbound(int sysNo)
        {
            if (!registerDA.CanWaitingOutbound(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotSetWaitingOutbound");
                throw new BizException(msg);
            }
            registerDA.UpdateOutboundStatus(sysNo, RMAOutBoundStatus.Origin);

            RMARegisterInfo register = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Outbound", BizLogType.RMA_Register_Outbound, sysNo, register.CompanyCode);

            return register;
        }

        /// <summary>
        /// 取消待送修
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual bool CancelWaitingOutbound(int sysNo)
        {
            if (!registerDA.CanCancelWaitingOutbound(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelWaitingOutbound");
                throw new BizException(msg);
            }

            bool result = registerDA.UpdateOutboundStatus(sysNo, null);

            RMARegisterInfo register = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Outbound", BizLogType.RMA_Register_Outbound, sysNo, register.CompanyCode);

            return result;
        }

        /// <summary>
        /// 关闭单件
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo Close(int sysNo)
        {
            RMARegisterInfo original = LoadBySysNo(sysNo);

            original.VerifyClose(original);

            original.BasicInfo.Status = RMARequestStatus.Complete;

            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //先关闭单件
                registerDA.Close(original);

                RMARequestInfo request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(sysNo);
                request.Registers = LoadByRequestSysNo(request.SysNo.Value);
                //判断所有单件是否都已关闭
                bool allClosed = true;
                request.Registers.ForEach(reg =>
                {
                    if (reg.BasicInfo.Status == RMARequestStatus.Handling)
                    {
                        allClosed = false;
                    }
                });
                //关闭申请单
                if (allClosed && request.Status != RMARequestStatus.Complete)
                {
                    request.Status = RMARequestStatus.Complete;

                    ObjectFactory<RequestProcessor>.Instance.UpdateWithoutRegisters(request);
                }

                //scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Close", BizLogType.RMA_Register_Close, sysNo, original.CompanyCode);

            return LoadForEditBySysNo(sysNo);
        }

        public virtual RMARegisterInfo CloseForVendorRefund(RMARegisterInfo register)
        {
            RMARegisterInfo original = LoadBySysNo(register.SysNo.Value);

            register.BasicInfo.CloseTime = DateTime.Now;
            register.BasicInfo.CloseUserSysNo = ServiceContext.Current.UserSysNo;
            register.VerifyCloseForVendorRefund(original);

            register.BasicInfo.Status = RMARequestStatus.Complete;
            register.BasicInfo.OwnBy = RMAOwnBy.Origin;
            register.BasicInfo.Location = RMALocation.Origin;
            register.BasicInfo.OutBoundStatus = RMAOutBoundStatus.VendorRefund;

            using (TransactionScope tran = new TransactionScope())
            {
                registerDA.CloseForVendorRefund(register);

                RMARequestInfo request = ObjectFactory<RequestProcessor>.Instance.LoadByRegisterSysNo(register.SysNo.Value);
                request.Registers = LoadByRequestSysNo(request.SysNo.Value);

                bool allClosed = true;
                request.Registers.ForEach(
                    reg =>
                    {
                        if (reg.BasicInfo.Status == RMARequestStatus.Handling)
                        {
                            allClosed = false;
                        }
                    }
                );
                if (allClosed && request.Status != RMARequestStatus.Complete)
                {
                    request.Status = RMARequestStatus.Complete;
                    ObjectFactory<RequestProcessor>.Instance.UpdateWithoutRegisters(request);
                }

                tran.Complete();
            }

            return register;
        }

        /// <summary>
        /// 重新打开单件
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo ReOpen(int sysNo)
        {
            if (!registerDA.CanReOpen(sysNo))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotReopen");
                throw new BizException(msg);
            }

            RMARegisterInfo register = new RMARegisterInfo();
            register.BasicInfo.SysNo = sysNo;
            register.BasicInfo.CloseUserSysNo = null;
            register.BasicInfo.CloseTime = null;
            register.BasicInfo.Status = RMARequestStatus.Handling;

            bool result = registerDA.ReOpen(register);

            RMARegisterInfo re = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Register_Close", BizLogType.RMA_Register_Close, sysNo, re.CompanyCode);

            return re;
        }

        /// <summary>
        /// 作废单件
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo SetAbandon(int sysNo)
        {
            RMARegisterInfo registerEntity = LoadBySysNo(sysNo);
            if (registerEntity.BasicInfo.Status != RMARequestStatus.Origin)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonRegister");
                throw new BizException(msg);
            }

            registerDA.SetAbandon(sysNo);

            return LoadBySysNo(sysNo);
        }

        #region Load

        /// <summary>
        /// 根据SysNo加载单件信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo LoadBySysNo(int sysNo)
        {
            var result = registerDA.LoadBySysNo(sysNo);
            if (result == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RegisterNotExists");
                msg = string.Format(msg, sysNo);
                throw new BizException(msg);
            }
            return result;
        }

        public virtual List<RMARegisterInfo> LoadBySysNoList(List<int> sysNoList)
        {
            return registerDA.GetRegistersBySysNoList(sysNoList);
        }

        /// <summary>
        /// UI专用：根据sysNo加载单件信息，包含了单件的一些关联表的相关信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RMARegisterInfo LoadForEditBySysNo(int sysNo)
        {
            var result = registerDA.LoadForEditBySysNo(sysNo);
            if (result == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RegisterNotExists");
                msg = string.Format(msg, sysNo);
                throw new BizException(msg);
            }
            return result;
        }

        /// <summary>
        /// 根据RequestSysNo加载单件信息，单表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<RMARegisterInfo> LoadByRequestSysNo(int requestSysNo)
        {
            return registerDA.QueryByRequestSysNo(requestSysNo);
        }

        public virtual void GetSecondHandProducts(string productID)
        {
            string realProductID = string.Empty;
            if (productID.StartsWith("A") || productID.StartsWith("S"))
            {
                Regex regEx = new Regex(IDREGEX);
                realProductID = regEx.Match(productID).Value;

            }
            else
            {
                realProductID = productID.Substring(0, 10);
            }
            //secondHandProducts = QueryModelProxy.InventoryQueryProvider.LoadSecondHandProducts(realProductID);           
        }

        public virtual List<ProductInventoryInfo> GetWarehouseProducts(ProcessType processType, int productSysNo, string shippedWarehouse)
        {
            //(1)业务模式2,3,5,6 直接取 reg.ShipedWarehouse (2)业务模式1 取 配置]
            int realStockSysNo = Convert.ToInt32(AppSettingManager.GetSetting("RMA", "StockSysNo"));

            if (processType == ProcessType.MET)
            {
                realStockSysNo = Convert.ToInt32(shippedWarehouse);
            }
            List<ProductInventoryInfo> result = new List<ProductInventoryInfo>();
            ProductInventoryInfo item = ExternalDomainBroker.GetProductInventoryInfo(productSysNo, realStockSysNo);
            if (item != null)
            {
                result.Add(item);
            }
            return result;
        }

        #endregion

        private BizLogType? GetNextHandlerLogType(RMANextHandler? nextHandler)
        {
            if (nextHandler.HasValue)
            {
                switch ((int)nextHandler.Value)
                {
                    case 0:
                        return BizLogType.RMA_Register_ToCC;
                    case 1:
                        return BizLogType.RMA_Register_ToRMA;
                    case 2:
                        return BizLogType.RMA_Register_ToRegister;
                    case 3:
                        return BizLogType.RMA_Register_ToCheck;
                    case 4:
                        return BizLogType.RMA_Register_ToASK;
                    case 5:
                        return BizLogType.RMA_Register_ToRevert;
                    case 6:
                        return BizLogType.RMA_Register_ToReturn;
                    case 7:
                        return BizLogType.RMA_Register_ToECC;
                    default:
                        break;
                }
            }

            return null;
        }

        #region For PO Domain

        /// <summary>
        /// 根据单件号获取接收仓库:
        /// </summary>
        /// <param name="registerSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual string[] GetReceiveWarehouseByRegisterSysNo(int registerSysNo)
        {
            return ObjectFactory<IRegisterDA>.Instance.GetReceiveWarehouseByRegisterSysNo(registerSysNo);
        }

        #endregion


        public virtual void LoadRegisterMemo(int registerSysNo, ref string memo, ref string productID, ref string productName, ref string vendorName)
        {
            DataRow dr = registerDA.LoadRegisterMemoBySysNo(registerSysNo);
            if (dr != null)
            {
                memo = dr["Memo"].ToString();
                vendorName = dr["VendorName"].ToString();
                int productSysNo = Convert.ToInt32(dr["ProductSysNo"]);

                ProductInfo productInfo = ExternalDomainBroker.GetProductInfo(productSysNo);
                if (productInfo != null)
                {
                    productID = productInfo.ProductID;
                    productName = productInfo.ProductBasicInfo.ProductTitle.Content
;
                }
            }
            else
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RegisterNotExists");
                msg = string.Format(msg, registerSysNo);
                throw new BizException(msg);
            }

        }

        public virtual RMARegisterInfo SyncERP(int sysNo)
        {
            RMARegisterInfo registerEntity = LoadBySysNo(sysNo);
            if (registerEntity.BasicInfo.InventoryType != ProductInventoryType.Company && registerEntity.BasicInfo.InventoryType != ProductInventoryType.TwoDoor)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RegisterCheck-OnlyTwoDoor"));
            }
            if (registerEntity.BasicInfo.ERPStatus != ERPReturnType.UnReturn)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RegisterCheck-UnReturn"));
            }

            if (registerEntity.RequestType != RMARequestType.Return && registerEntity.RequestType != RMARequestType.Exchange)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RegisterCheck-Exchange"));
            }

            SyncERPAction(registerEntity);

            return LoadBySysNo(sysNo);
        }

        private void SyncERPAction(RMARegisterInfo registerEntity)
        {
            bool isInventoryTypeInvalid = registerEntity.BasicInfo.InventoryType != ProductInventoryType.Company && registerEntity.BasicInfo.InventoryType != ProductInventoryType.TwoDoor;
            bool isERPStatusInvalid = registerEntity.BasicInfo.ERPStatus != ERPReturnType.UnReturn;
            bool isRequestTypeInvalid = registerEntity.RequestType != RMARequestType.Return && registerEntity.RequestType != RMARequestType.Exchange;

            if (isInventoryTypeInvalid
                || isERPStatusInvalid
                || isRequestTypeInvalid)
            {
                return;
            }
            ERPSHDInfo erpinfo = new ERPSHDInfo();
            erpinfo.RefOrderNo = registerEntity.SysNo.Value.ToString();
            erpinfo.RefOrderType = "退款单件";
            erpinfo.SysMemo = erpinfo.RefOrderNo + "/" + erpinfo.RefOrderType;
            erpinfo.ZDR = ServiceContext.Current.UserSysNo;
            erpinfo.ZDSJ = DateTime.Now;
            erpinfo.ZXR = ServiceContext.Current.UserSysNo;
            erpinfo.ZXSJ = DateTime.Now;
            erpinfo.SHDItemList = new List<ERPSHDItem>();
            ERPSHDItem erpitem = new ERPSHDItem();
            erpitem.ProductSysNo = registerEntity.BasicInfo.ProductSysNo;
            erpitem.SL = 1;
            erpinfo.SHDItemList.Add(erpitem);

            //if (registerEntity.RequestType == RMARequestType.Exchange)
            //{
            //    ObjectFactory<ISyncERPBizRecord>.Instance.CreateSHD_TCWF(erpinfo);
            //}
            //else if (registerEntity.RequestType == RMARequestType.Return)
            //{
            //    ObjectFactory<ISyncERPBizRecord>.Instance.CreateSHD_FC(erpinfo);
            //}
            //更新ERPStatus
            //registerDA.SyncERP(registerEntity.SysNo.Value);
        }
    }
}
