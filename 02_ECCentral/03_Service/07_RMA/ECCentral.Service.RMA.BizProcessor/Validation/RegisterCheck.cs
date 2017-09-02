using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.BizProcessor
{
    internal static class RegisterCheck
    {
        #region verify creation

        public static bool VerifyCreate(this RMARegisterInfo register)
        {
            if (register.RequestType == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RequestType"));
                throw new BizException(msg);
            }
            if (register.BasicInfo.RMAReason == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RMAReason"));
                throw new BizException(msg);
            }

            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ProductSysNo"), register.BasicInfo.ProductSysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-CustomerDesc"), register.BasicInfo.CustomerDesc);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RequestType"), (int?)register.RequestType);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-Status"), (int?)register.BasicInfo.Status);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SOItemType"), (int?)register.BasicInfo.SOItemType);
            return register.VerifyFieldsLength();
        }

        #endregion

        #region verify update

        public static bool VerifyUpdate(this RMARegisterInfo register, RMARegisterInfo registerInDb)
        {
            if (registerInDb == null)
            {
                throw new ArgumentNullException("registerInDb");
            }
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }

            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SysNo"), register.BasicInfo.SysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ProductSysNo"), register.BasicInfo.ProductSysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-CustomerDesc"), register.BasicInfo.CustomerDesc);            
           
            register.VerifyFieldsLength();

            if (registerInDb.BasicInfo.Status.Value == RMARequestStatus.Complete
                || registerInDb.BasicInfo.Status.Value == RMARequestStatus.Abandon)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotEditWhenClosedOrAbandon");
                throw new BizException(msg);
            }
            if (register.BasicInfo.Status != registerInDb.BasicInfo.Status)
            {
                switch (register.BasicInfo.Status.Value)
                {
                    case RMARequestStatus.Abandon:
                        registerInDb.VerifyAbandon();
                        break;
                    case RMARequestStatus.Complete:
                        register.VerifyClose(registerInDb);
                        break;
                    case RMARequestStatus.Handling:
                        register.VerifyReceive();
                        break;
                    case RMARequestStatus.Origin:
                        break;
                }
            }
            return true;
        }

        public static bool VerifyWaitingRevert(this RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register is required!");
            }

            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SysNo"), register.BasicInfo.SysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-NewProductStatus"), (int?)register.RevertInfo.NewProductStatus);
            
            register.VerifyFieldsLength();

            if (!ObjectFactory<IRegisterDA>.Instance.CanWaitingRevert(register.BasicInfo.SysNo.Value))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotSetWaitingRevert");
                throw new BizException(msg);
            }

            switch (register.RevertInfo.NewProductStatus.Value)
            {
                // 非换货和调新品时，RevertProductSysNo 将被设定为原始的 ProductSysNo，因此 ProductSysNo 不能为空
                case RMANewProductStatus.Origin:
                case RMANewProductStatus.NewProduct:
                    CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ProductSysNo"), register.BasicInfo.ProductSysNo);
                    break;
                // 调二手和非当前 Case 时，RevertProductSysNo 由用户从 UI 设定，不能为空
                case RMANewProductStatus.SecondHand:
                case RMANewProductStatus.OtherProduct:
                    CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RevertProductSysNo"), register.RevertInfo.RevertProductSysNo);
                    if (register.RevertInfo.RevertProductSysNo.Value == register.BasicInfo.ProductSysNo.Value)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Request", "RevertProductSysnoCannotSameAsProductSysNo");
                        throw new BizException(msg);
                    }
                    break;
            }
            // 不为“非换货”时，RevertStockSysNo 不能为空
            if (register.RevertInfo.NewProductStatus != RMANewProductStatus.Origin)
            {
                CommonCheck.VerifyNotNull("发货仓库", register.RevertInfo.RevertStockSysNo);
            }
            return true;
        }

        public static bool VerifyRevertAudit(this RMARegisterInfo register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register is required!");
            }

            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SysNo"), register.SysNo);
            CommonCheck.VerifyNotNull( ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RevertProductSysNo"), register.RevertInfo.RevertProductSysNo);
            // verify fields' max length
            register.VerifyFieldsLength();

            if (register.RevertInfo.RevertAuditMemo == null) // only verify whether it's null, but empty string is valid
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RequestMsg"));
                throw new BizException(msg);
            }
            return true;
        }

        public static bool VerifyClose(this RMARegisterInfo register, RMARegisterInfo registerInDb)
        {
            if (registerInDb == null)
            {
                throw new ArgumentNullException("registerInDb");
            }
            if (register == null)
            {
                throw new ArgumentNullException("request is required!");
            }
            string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCloseRegister");
            msg = string.Format(msg, registerInDb.SysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SysNo"), register.BasicInfo.SysNo);           

            // Extend warranty register doesn't need OwnBy/Location validation
            if (registerInDb.BasicInfo.SOItemType != SOProductType.ExtendWarranty)
            {
               

                if ((!registerInDb.BasicInfo.Status.HasValue || registerInDb.BasicInfo.Status.Value != RMARequestStatus.Handling))
                {                  
                    throw new BizException(msg);                           
                }
                //ERP模式不用接收
                if (registerInDb.BasicInfo.InventoryType == ProductInventoryType.Company || registerInDb.BasicInfo.InventoryType == ProductInventoryType.TwoDoor)
                    return true;
                if ((registerInDb.BasicInfo.OwnBy.HasValue && registerInDb.BasicInfo.OwnBy.Value != RMAOwnBy.Origin) ||
                (registerInDb.BasicInfo.Location.HasValue && registerInDb.BasicInfo.Location.Value != RMALocation.Origin))
                {
                    
                    throw new BizException(msg);
                }           
            }
            return true;
        }

        public static bool VerifyCloseForVendorRefund(
            this RMARegisterInfo register, RMARegisterInfo registerInDb)
        {
            if (registerInDb == null)
            {
                throw new ArgumentNullException("registerInDb");
            }
            if (register == null)
            {
                throw new ArgumentNullException("request is required!");
            }

            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SysNo"), register.BasicInfo.SysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-CloseUserSysNo"), register.BasicInfo.CloseUserSysNo);
            CommonCheck.VerifyNotNull(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-CloseTime"), register.BasicInfo.CloseTime);
            if (!registerInDb.BasicInfo.Status.HasValue || registerInDb.BasicInfo.Status.Value != RMARequestStatus.Handling)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCloseRegisterFormat");
                msg = string.Format(msg, register.BasicInfo.SysNo);
                throw new BizException(msg);
            }
            return true;
        }

        #endregion
       
        private static bool VerifyReceive(this RMARegisterInfo register)
        {
            return true;
        }

        private static bool VerifyAbandon(this RMARegisterInfo registerInDb)
        {
            if (registerInDb == null)
            {
                throw new ArgumentNullException("registerInDb");
            }
            if (registerInDb.BasicInfo.RefundStatus.HasValue
                && registerInDb.BasicInfo.RefundStatus.Value == RMARefundStatus.Refunded)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonRefundedRegister");
                throw new BizException(msg);
            }
            if (registerInDb.BasicInfo.ReturnStatus.HasValue
                && registerInDb.BasicInfo.ReturnStatus.Value == RMAReturnStatus.Returned)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonReturnedRegister");
                throw new BizException(msg);
            }
            if (registerInDb.RevertInfo.RevertStatus.HasValue)
            {
                if (registerInDb.RevertInfo.RevertStatus.Value == RMARevertStatus.Reverted)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonRevertedRegister");
                    throw new BizException(msg);
                }
                else
                {
                    /*
                     * RevertStatus 是 WaitingRevert/WaitingAudit 状态的单件，
                     * 其 NewProductStatus 又不等于 Origin 的话，则不允许进行作废操作
                     */
                    if ((registerInDb.RevertInfo.RevertStatus.Value == RMARevertStatus.WaitingRevert
                            || registerInDb.RevertInfo.RevertStatus.Value == RMARevertStatus.WaitingAudit)
                        && registerInDb.RevertInfo.NewProductStatus.HasValue
                        && registerInDb.RevertInfo.NewProductStatus.Value != RMANewProductStatus.Origin)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonWithoutOriginNewProductStatus");
                        throw new BizException(msg);
                    }
                }
            }

            return true;
        }

        public static bool VerifyFieldsLength(this RMARegisterInfo register)
        {
            if (register != null)
            {
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-CustomerDesc"), register.BasicInfo.CustomerDesc, 500);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-Memo"), register.BasicInfo.Memo, 3000);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-SerialNumber"), register.BasicInfo.ProductNo, 50);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-LocationWarehouse"), register.BasicInfo.LocationWarehouse, 4);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-LocationWarehouse"), register.BasicInfo.OwnByWarehouse, 4);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ShippedWarehouse"), register.BasicInfo.ShippedWarehouse, 4);

                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ShippedWarehouse"), register.CheckInfo.CheckDesc, 1000);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-InspectionResultType"), register.CheckInfo.InspectionResultType, 100);

                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ResponseDesc"), register.ResponseInfo.ResponseDesc, 1000);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ResponseProductNo"), register.ResponseInfo.ResponseProductNo, 50);
                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-VendorRepairResultType"), register.ResponseInfo.VendorRepairResultType, 100);

                CommonCheck.VerifyLength(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-RequestMsg"), register.RevertInfo.RevertAuditMemo, 200);             
            }
            return true;
        }      
    }
}
