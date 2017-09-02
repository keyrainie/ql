using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IPurchaseOrderDA))]
    public class PurchaseOrderDA : IPurchaseOrderDA
    {
        #region IPurchaseOrderDA Members

        public int CreatePOSequenceSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOSequence");
            object result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public BizEntity.PO.PurchaseOrderInfo CreatePO(BizEntity.PO.PurchaseOrderInfo poInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOMaster");
            command.SetParameterValue("@ContractNumber", poInfo.EIMSInfo.ContractNumber);
            command.SetParameterValue("@SysNo", poInfo.SysNo.Value);
            command.SetParameterValue("@POID", poInfo.PurchaseOrderBasicInfo.PurchaseOrderID);
            command.SetParameterValue("@VendorSysNo", poInfo.VendorInfo.SysNo.Value);

            if (poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                string stockStr = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.ToString();
                //有中转仓
                if (stockStr.Length > 2)
                {
                    command.SetParameterValue("@ITStockSysNo", Int32.Parse(stockStr.Substring(2)));
                    command.SetParameterValue("@StockSysNo", 50);
                }
                else
                {
                    command.SetParameterValue("@ITStockSysNo", DBNull.Value);
                    command.SetParameterValue("@StockSysNo", poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo);
                }
            }
            else
            {
                command.SetParameterValue("@StockSysNo", DBNull.Value);
            }
            command.SetParameterValue("@ShipTypeSysNo", poInfo.PurchaseOrderBasicInfo.ShippingType.SysNo.Value);
            command.SetParameterValue("@PayTypeSysNo", poInfo.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
            command.SetParameterValue("@CurrencySysNo", poInfo.PurchaseOrderBasicInfo.CurrencyCode);
            command.SetParameterValue("@ExchangeRate", poInfo.PurchaseOrderBasicInfo.ExchangeRate);
            command.SetParameterValue("@TotalAmt", poInfo.PurchaseOrderBasicInfo.TotalAmt);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@AuditTime", DBNull.Value);
            command.SetParameterValue("@AuditUserSysNo", DBNull.Value);
            command.SetParameterValue("@InTime", DBNull.Value);
            command.SetParameterValue("@InUserSysNo", DBNull.Value);
            command.SetParameterValue("@IsApportion", poInfo.PurchaseOrderBasicInfo.IsApportion);
            command.SetParameterValue("@ApportionTime", DBNull.Value);
            command.SetParameterValue("@ApportionUserSysNo", DBNull.Value);
            command.SetParameterValue("@Memo", poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo);
            command.SetParameterValue("@Note", poInfo.PurchaseOrderBasicInfo.MemoInfo.Note);
            command.SetParameterValue("@Status", (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@ETP", poInfo.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@IsConsign", (int)poInfo.PurchaseOrderBasicInfo.ConsignFlag);
            command.SetParameterValue("@POType", (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderType);
            command.SetParameterValue("@PM_ReturnPointSysNo", poInfo.PurchaseOrderBasicInfo.PM_ReturnPointSysNo);
            command.SetParameterValue("@UsingReturnPoint", poInfo.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@ReturnPointC3SysNo", poInfo.PurchaseOrderBasicInfo.ReturnPointC3SysNo);
            command.SetParameterValue("@TaxRate", poInfo.PurchaseOrderBasicInfo.TaxRate);
            command.SetParameterValue("@PurchaseStockSysno", DBNull.Value);
            command.SetParameterValue("@PMSysNo", poInfo.PurchaseOrderBasicInfo.ProductManager.SysNo);
            command.SetParameterValue("@SettlementCompany", poInfo.PurchaseOrderBasicInfo.SettleCompanySysNo);
            command.SetParameterValue("@ETATime", poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", poInfo.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@PMRequestMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@TLRequestMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo == null ? "" : poInfo.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@AutoSendMail", poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress == "-999" ? "" : poInfo.PurchaseOrderBasicInfo.AutoSendMailAddress);
            command.SetParameterValue("@CompanyCode", poInfo.CompanyCode);
            command.SetParameterValue("@ProductLineSysNo", poInfo.PurchaseOrderBasicInfo.ProductLineSysNo);
            command.SetParameterValue("@PaySettleCompany", poInfo.VendorInfo.VendorBasicInfo.PaySettleCompany == null ? 0 : (int)poInfo.VendorInfo.VendorBasicInfo.PaySettleCompany);
            command.SetParameterValue("@LeaseFlag", (int)poInfo.PurchaseOrderBasicInfo.PurchaseOrderLeaseFlag);
            command.SetParameterValue("@LogisticsNumber", poInfo.PurchaseOrderBasicInfo.LogisticsNumber);
            command.SetParameterValue("@ExpressName", poInfo.PurchaseOrderBasicInfo.ExpressName);

            if (command.ExecuteNonQuery() > 0)
            {
                return poInfo;
            }
            else
            {
                return null;
            }
        }

        public BizEntity.PO.PurchaseOrderItemInfo CreatePOItem(BizEntity.PO.PurchaseOrderItemInfo itemInfo)
        {
            DataCommand command;
            if (itemInfo.LastAdjustPriceDate.HasValue || itemInfo.LastInTime.HasValue)
            {
                //补充创建PO:
                command = DataCommandManager.GetDataCommand("CreatePOItems4PartPO");
                command.SetParameterValue("@AcquireReturnPoint", itemInfo.AcquireReturnPoint);
                command.SetParameterValue("@AcquireReturnPointType", itemInfo.AcquireReturnPointType);
                command.SetParameterValue("@BatchInfo", itemInfo.BatchInfo);
                command.SetParameterValue("@POSysNo", itemInfo.POSysNo);
                command.SetParameterValue("@SysNo", itemInfo.ItemSysNo);
                command.SetParameterValue("@ProductSysNo", itemInfo.ProductSysNo);
                command.SetParameterValue("@BriefName", itemInfo.BriefName);
                command.SetParameterValue("@Quantity", itemInfo.Quantity);
                command.SetParameterValue("@Weight", itemInfo.Weight);
                command.SetParameterValue("@OrderPrice", itemInfo.OrderPrice);
                command.SetParameterValue("@ApportionAddOn", itemInfo.ApportionAddOn);
                command.SetParameterValue("@UnitCost", itemInfo.UnitCost);
                command.SetParameterValue("@ReturnCost", itemInfo.ReturnCost);
                command.SetParameterValue("@lastOrderPrice", itemInfo.LastOrderPrice);
                command.SetParameterValue("@ExecptStatus", itemInfo.ExecptStatus);
                command.SetParameterValue("@ProductID", itemInfo.ProductID);
                command.SetParameterValue("@UnitCostWithoutTax", itemInfo.UnitCostWithoutTax);
                command.SetParameterValue("@JDPrice", itemInfo.JingDongPrice);
                command.SetParameterValue("@AvailableQty", itemInfo.AvailableQty);
                command.SetParameterValue("@m1", itemInfo.M1);
                command.SetParameterValue("@PurchaseQty", itemInfo.PurchaseQty);
                command.SetParameterValue("@CurrencySysNo", itemInfo.CurrencyCode);
                command.SetParameterValue("@LastAdjustPriceDate", itemInfo.LastAdjustPriceDate);
                command.SetParameterValue("@LastInTime", itemInfo.LastInTime);
                command.SetParameterValue("@ReadyQuantity", itemInfo.ReadyQuantity);
            }
            else
            {
                //创建新PO:
                command = DataCommandManager.GetDataCommand("CreatePOItems");
                command.SetParameterValue("@AcquireReturnPoint", itemInfo.AcquireReturnPoint);
                command.SetParameterValue("@AcquireReturnPointType", itemInfo.AcquireReturnPointType);

                command.SetParameterValue("@POSysNo", itemInfo.POSysNo);
                command.SetParameterValue("@SysNo", itemInfo.ItemSysNo);
                command.SetParameterValue("@ProductSysNo", itemInfo.ProductSysNo);
                command.SetParameterValue("@BriefName", itemInfo.BriefName);
                command.SetParameterValue("@Quantity", itemInfo.Quantity);
                command.SetParameterValue("@Weight", itemInfo.Weight);
                command.SetParameterValue("@OrderPrice", itemInfo.OrderPrice);
                command.SetParameterValue("@ApportionAddOn", itemInfo.ApportionAddOn);
                command.SetParameterValue("@UnitCost", itemInfo.UnitCost);
                command.SetParameterValue("@ReturnCost", itemInfo.ReturnCost);
                command.SetParameterValue("@lastOrderPrice", itemInfo.LastOrderPrice);
                command.SetParameterValue("@ExecptStatus", itemInfo.ExecptStatus);
                command.SetParameterValue("@ProductID", itemInfo.ProductID);
                command.SetParameterValue("@UnitCostWithoutTax", itemInfo.UnitCostWithoutTax);
                command.SetParameterValue("@JDPrice", itemInfo.JingDongPrice);
                command.SetParameterValue("@AvailableQty", itemInfo.AvailableQty);
                command.SetParameterValue("@m1", itemInfo.M1);
                command.SetParameterValue("@PurchaseQty", itemInfo.PurchaseQty);
                command.SetParameterValue("@CurrencySysNo", itemInfo.CurrencyCode);
                command.SetParameterValue("@ReadyQuantity", itemInfo.ReadyQuantity);
                command.SetParameterValue("@BatchInfo", itemInfo.BatchInfo);
            }
            command.SetParameterValue("@CompanyCode", itemInfo.CompanyCode);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                itemInfo.ItemSysNo = Convert.ToInt32(o);
                return itemInfo;
            }
            else
            {
                return null;
            }
        }

        public BizEntity.PO.EIMSInfo CreatePOEIMSInfo(BizEntity.PO.EIMSInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPOEimsInfo");
            command.SetParameterValue("@POSysNo", entity.PurchaseOrderSysNo);
            command.SetParameterValue("@EIMSNo", entity.EIMSSysNo);
            command.SetParameterValue("@EIMSAmt", entity.EIMSAmt);
            command.SetParameterValue("@AlreadyUseAmt", entity.AlreadyUseAmt);
            command.SetParameterValue("@LeftAmt", entity.LeftAmt);
            command.SetParameterValue("@EIMSLeftAmt", entity.EIMSLeftAmt);
            command.ExecuteNonQuery();
            return entity;
        }

        public BizEntity.PO.PurchaseOrderInfo LoadPOMaster(int poSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOMaster");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, null, "Basket.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "po.SysNo",
                    DbType.Int32,
                    "@SysNo",
                    QueryConditionOperatorType.Equal,
                    poSysNo);

                command.CommandText = builder.BuildQuerySql();
            }
            return command.ExecuteEntity<PurchaseOrderInfo>();
        }

        public System.Collections.Generic.List<PurchaseOrderSSBLogInfo> LoadPOSSBLog(int poSysNo, PurchaseOrderSSBMsgType msgType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOSSBLog");

            command.SetParameterValue("@POSysNo", poSysNo);
            command.SetParameterValue("@ActionType", msgType.ToString());

            return command.ExecuteEntityList<PurchaseOrderSSBLogInfo>();
        }

        public int DeletePOEIMSInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeletePOEimsInfoByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);
            return command.ExecuteNonQuery();
        }

        public PurchaseOrderInfo UpdatePOMaster(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOMaster");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@VendorSysNo", entity.VendorInfo.SysNo.Value);
            command.SetParameterValue("@ContractNumber", entity.EIMSInfo.ContractNumber);

            if (entity.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                string stockStr = entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value.ToString();
                //有中转仓
                if (stockStr.Length > 2)
                {
                    command.SetParameterValue("@ITStockSysNo", Int32.Parse(stockStr.Substring(2)));
                    command.SetParameterValue("@StockSysNo", 50);
                }
                else
                {
                    command.SetParameterValue("@ITStockSysNo", DBNull.Value);
                    command.SetParameterValue("@StockSysNo", entity.PurchaseOrderBasicInfo.StockInfo.SysNo);
                }
            }
            else
            {
                command.SetParameterValue("@StockSysNo", entity.PurchaseOrderBasicInfo.StockInfo.SysNo);
            }
            command.SetParameterValue("@ShipTypeSysNo", entity.PurchaseOrderBasicInfo.ShippingType.SysNo);
            command.SetParameterValue("@PayTypeSysNo", entity.PurchaseOrderBasicInfo.PayType.SysNo);
            command.SetParameterValue("@CurrencySysNo", entity.PurchaseOrderBasicInfo.CurrencyCode);
            command.SetParameterValue("@ExchangeRate", entity.PurchaseOrderBasicInfo.ExchangeRate);
            //command.SetParameterValue("@TotalAmt", entity.TotalAmt);
            command.SetParameterValue("@ETP", entity.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@IsConsign", (int)entity.PurchaseOrderBasicInfo.ConsignFlag);
            command.SetParameterValue("@POType", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderType);
            command.SetParameterValue("@PM_ReturnPointSysNo", entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo);
            command.SetParameterValue("@UsingReturnPoint", entity.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@ReturnPointC3SysNo", entity.PurchaseOrderBasicInfo.ReturnPointC3SysNo);
            command.SetParameterValue("@TaxRate", entity.PurchaseOrderBasicInfo.TaxRate);
            command.SetParameterValue("@PMSysNo", entity.PurchaseOrderBasicInfo.ProductManager.SysNo.Value);
            command.SetParameterValue("@SettlementCompany", entity.PurchaseOrderBasicInfo.SettleCompanySysNo);
            command.SetParameterValue("@ExecptStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderExceptStatus);
            command.SetParameterValue("@Memo", entity.PurchaseOrderBasicInfo.MemoInfo.Memo);
            command.SetParameterValue("@Note", entity.PurchaseOrderBasicInfo.MemoInfo.Note);
            command.SetParameterValue("@PMRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@TLRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@InTime", entity.PurchaseOrderBasicInfo.InTime);
            command.SetParameterValue("@ETATime", entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", entity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@ProductLineSysNo", entity.PurchaseOrderBasicInfo.ProductLineSysNo);
            command.SetParameterValue("@LogisticsNumber", entity.PurchaseOrderBasicInfo.LogisticsNumber);
            command.SetParameterValue("@ExpressName", entity.PurchaseOrderBasicInfo.ExpressName);

            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return entity;
        }

        public int UpdatePOAutoSendMailAddress(int poSysNo, string autoSendMailAddress)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAutoAutoSendMailByPOSysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            command.SetParameterValue("@AutoSendMail", autoSendMailAddress);
            return command.ExecuteNonQuery();
        }

        public System.Collections.Generic.List<PurchaseOrderItemInfo> LoadPOItems(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOItemsByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);
            List<PurchaseOrderItemInfo> returnList = command.ExecuteEntityList<PurchaseOrderItemInfo>();
            returnList.ForEach(x =>
            {
                if (IsVirtualStockPurchaseOrderProduct(x.ProductSysNo.Value))
                {
                    x.IsVirtualStockProduct = true;
                }
            });
            return returnList;
        }

        public System.Collections.Generic.List<EIMSInfo> LoadPOEIMSInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOEimsRelevanceInfo");
            command.SetParameterValue("@POSysNo", poSysNo);
            return command.ExecuteEntityList<EIMSInfo>();
        }

        public System.Collections.Generic.List<EIMSInfo> LoadPOEIMSInfoForPrint(int poSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSInvoiceInfoForPrint");
            dataCommand.SetParameterValue("@POSysNo", poSysNo);
            return dataCommand.ExecuteEntityList<EIMSInfo>();
        }

        public int UpdatePOTPStatus(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePoMasterTpStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@TpStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);

            DateTime? datetime = null;
            if (entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus == 2)
            {
                command.SetParameterValueAsCurrentUserSysNo("@ApportionUserSysNo");
                datetime = DateTime.Now;
            }
            else
            {
                command.SetParameterValue("@ApportionUserSysNo", null);
            }
            command.SetParameterValue("@ApportionTime", datetime);
            return command.ExecuteNonQuery();
        }

        public int WaitingInStockPO(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItemStatusVerifyInStock");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@AuditTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@UsingReturnPoint", entity.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@PMRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@TLRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@ETP", entity.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@ETATime", entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", entity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@TPStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);
            command.SetParameterValue("@RefuseMemo", entity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo);

            command.SetParameterValue("@ApportionUserSysNo", entity.PurchaseOrderBasicInfo.ApportionUserSysNo);
            command.SetParameterValue("@ApportionTime", entity.PurchaseOrderBasicInfo.ApportionTime);
            return command.ExecuteNonQuery();
        }

        public PurchaseOrderItemInfo LoadExtendPOItem(int itemSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetJDPriceAndM1AndAndAvailableQty");
            command.SetParameterValue("@ProductSysNo", itemSysNo);
            PurchaseOrderItemInfo item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            if (!item.M1.HasValue)
            {
                item.M1 = 0;
            }
            return item;
        }

        public PurchaseOrderItemInfo UpdatePOItem(PurchaseOrderItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOItem");
            command.SetParameterValue("@AcquireReturnPoint", entity.AcquireReturnPoint);
            command.SetParameterValue("@AcquireReturnPointType", entity.AcquireReturnPointType);

            command.SetParameterValue("@SysNo", entity.ItemSysNo.Value);
            command.SetParameterValue("@POSysNo", entity.POSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@BriefName", entity.BriefName);

            command.SetParameterValue("@Quantity", entity.Quantity);
            command.SetParameterValue("@Weight", entity.Weight);
            command.SetParameterValue("@OrderPrice", entity.OrderPrice);

            command.SetParameterValue("@ApportionAddOn", entity.ApportionAddOn);
            command.SetParameterValue("@UnitCost", entity.UnitCost);
            command.SetParameterValue("@ReturnCost", entity.ReturnCost);

            command.SetParameterValue("@PurchaseQty", entity.PurchaseQty.Value);
            command.SetParameterValue("@CurrencySysNo", entity.CurrencyCode);
            command.SetParameterValue("@ExecptStatus", entity.ExecptStatus);
            command.SetParameterValue("@UnitCostWithoutTax", entity.UnitCostWithoutTax);

            command.SetParameterValue("@JDPrice", entity.JingDongPrice);
            command.SetParameterValue("@AvailableQty", entity.AvailableQty);
            command.SetParameterValue("@m1", entity.M1);

            command.SetParameterValue("@CurrentUnitCost", entity.UnitCost);
            command.SetParameterValue("@CurrentPrice", entity.OrderPrice);
            command.SetParameterValue("@LastAdjustPriceDate", entity.LastAdjustPriceDate);

            command.SetParameterValue("@LastInTime", entity.LastInTime);
            command.SetParameterValue("@lastOrderPrice", entity.LastOrderPrice);

            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return entity;
        }

        public System.Collections.Generic.List<PurchaseOrderDetailInfo> LoadPODetialInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOWithItems");

            command.SetParameterValue("@SysNo", poSysNo);

            List<PurchaseOrderDetailInfo> returnList = command.ExecuteEntityList<PurchaseOrderDetailInfo>();
            returnList.ForEach(x =>
            {
                x.PONumber = poSysNo;
            });
            return returnList;
        }

        public string GetPhoneNumberByPOSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPMNameAndPhoneNumberByPOSysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            return command.ExecuteScalar<string>();
        }

        public PurchaseOrderSSBLogInfo CreatePOSSBLog(PurchaseOrderSSBLogInfo logInfo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePOSSBLog");

            command.SetParameterValue("@POSysNo", logInfo.POSysNo);
            command.SetParameterValue("@Content", logInfo.Content);
            command.SetParameterValue("@ActionType", logInfo.ActionType);
            command.SetParameterValue("@InUser", logInfo.InUser);
            command.SetParameterValue("@ErrMSg", logInfo.ErrMSg);
            command.SetParameterValue("@ErrMSgTime", logInfo.ErrMSgTime);
            command.SetParameterValue("@SendErrMail", logInfo.SendErrMail);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@StoreCompanyCode", companyCode);

            command.ExecuteNonQuery();
            return logInfo;
        }

        public int CallSSBMessageSP(string message)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendSSBMessage");

            command.SetParameterValue("@RequestMSG", message);

            return command.ExecuteNonQuery();
        }

        public decimal GetPOTotalAmtToday(int POSysNo, int PmSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetTodayTotalAmt");
            command.SetParameterValue("@POSysNo", POSysNo);
            command.SetParameterValue("@PMSysNo", PmSysNo);
            return Convert.ToDecimal(command.ExecuteScalar());
        }

        public int QueryEIMSInvoiceInfoByPMAndVendor(int vendorSysNo, string pmSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("IsExistEIMSInvoiceInfoByPMAndVendor");
            dataCommand.SetParameterValue("@VendorNumber", vendorSysNo);
            dataCommand.SetParameterValue("@PM", pmSysNo);

            int count = dataCommand.ExecuteScalar<int>();
            return count;
        }

        public int QueryEIMSInvoiceInfoByVendor(int vendorSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("IsExistEIMSInvoiceInfoByVendor");
            dataCommand.SetParameterValue("@VendorNumber", vendorSysNo);

            int count = dataCommand.ExecuteScalar<int>();
            return count;
        }

        public PurchaseOrderLogInfo LoadPOLogInfo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOLog");
            command.SetParameterValue("@POSysNo", poSysNo);
            return command.ExecuteEntity<PurchaseOrderLogInfo>();
        }

        public PurchaseOrderInfo UpdatePOMasterStock(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOBySysNo");
            command.SetParameterValue("@POSysNo", entity.SysNo);
            command.SetParameterValue("@UsingReturnPoint", entity.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@TotalAmt", entity.PurchaseOrderBasicInfo.TotalAmt);
            command.SetParameterValueAsCurrentUserAcct("@CloseUser");
            command.SetParameterValue("@CloseTime", DateTime.Now);
            command.ExecuteNonQuery();
            return entity;
        }

        public PurchaseOrderInfo CanecelAbandonPO(PurchaseOrderInfo poInfo)
        {
            return UpdatePOItemsStatus(poInfo);
        }

        public PurchaseOrderInfo UpdatePOItemsStatus(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("VerifyInStock");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@AuditTime", entity.PurchaseOrderBasicInfo.AuditDate);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@UsingReturnPoint", entity.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@TLRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@ETP", entity.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@ETATime", entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", entity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay);
            command.SetParameterValue("@TPStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);
            command.SetParameterValue("@RefuseMemo", entity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo);
            command.ExecuteNonQuery();
            return entity;
        }

        public PurchaseOrderInfo AbandonPO(PurchaseOrderInfo poInfo)
        {
            return UpdatePOItemsStatus(poInfo);
        }

        public PurchaseOrderETATimeInfo UpdatePOETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("MaintainPO_ETACancle");
            command.SetParameterValue("@POSysNo", etaInfo.POSysNo);
            command.SetParameterValue("@Status", etaInfo.Status);
            command.SetParameterValue("@EditUser", etaInfo.EditUser);
            command.ExecuteNonQuery();
            return etaInfo;
        }

        public int UpdatePOStatus(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@RefuseMemo", entity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo);
            command.SetParameterValue("@PMRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@TLRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@AuditUserSysNo", DBNull.Value);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@TPStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus);
            return command.ExecuteNonQuery();
        }

        public int UpdatePOMasterETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("PO_ETAToPO");
            command.SetParameterValue("@POSysNo", etaInfo.POSysNo);
            command.SetParameterValue("@ETATime", etaInfo.ETATime);
            command.SetParameterValue("@HalfDay", etaInfo.HalfDay);
            return command.ExecuteNonQuery();
        }

        public DataTable GetPOTotalAmt(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOTotalAmt");
            command.SetParameterValue("@SysNo", poSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            return dt;
        }

        public decimal GetAuditPOTotalAmt(int pmSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAuditPOAmt");
            command.SetParameterValue("@PMSysNo", pmSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null)
            {
                return Convert.ToDecimal(dt.Rows[0][0]);
            }
            return 0m;
        }

        public int ConfirmWithVendor(PurchaseOrderInfo poInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ConFirmVindor");
            command.SetParameterValue("@SysNo", poInfo.SysNo);
            command.SetParameterValueAsCurrentUserSysNo("@ComfirmUserSysNo");
            command.SetParameterValue("@ComfirmTime", DateTime.Now);
            return command.ExecuteNonQuery();
        }

        public PurchaseOrderInfo UpdateInStockMemo(PurchaseOrderInfo poInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInStockMemo");
            command.SetParameterValue("@SysNo", poInfo.SysNo);
            command.SetParameterValue("@InStockMemo", poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo);
            command.SetParameterValue("@CarriageCost", poInfo.PurchaseOrderBasicInfo.CarriageCost);

            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return poInfo;
        }

        public PurchaseOrderETATimeInfo CreatePOETAInfo(PurchaseOrderETATimeInfo etaInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreatePO_ETA");
            command.SetParameterValue("@POSysNo", etaInfo.POSysNo);
            command.SetParameterValue("@ETATime", etaInfo.ETATime);
            command.SetParameterValue("@HalfDay", etaInfo.HalfDay);
            command.SetParameterValue("@Memo", etaInfo.Memo);
            command.SetParameterValue("@InUser", etaInfo.EditUser);
            etaInfo.ETATimeSysNo = command.ExecuteScalar<int>();
            return etaInfo;
        }

        public int UpdateCheckResult(PurchaseOrderInfo poInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePoMasterCheckResult");
            command.SetParameterValue("@SysNo", poInfo.SysNo);
            command.SetParameterValue("@CheckResult", poInfo.PurchaseOrderBasicInfo.CheckResult);
            return command.ExecuteNonQuery();
        }

        public System.Collections.Generic.List<BizEntity.IM.ProductInfo> QueryVendorProductByVendorSysNo(int vendorSysNo)
        {
            List<ProductInfo> resultList = new List<ProductInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("GetProductByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);

            using (IDataReader dr = command.ExecuteDataReader())
            {
                while (dr.Read())
                {
                    ProductInfo result = new ProductInfo();
                    result.SysNo = (int)dr["SysNo"];
                    resultList.Add(result);
                }
            }
            return resultList;
        }

        public List<PurchaseOrderReceivedInfo> LoadPurchaseOrderReceivedInfo(PurchaseOrderInfo poInfo)
        {
            List<PurchaseOrderReceivedInfo> result = new List<PurchaseOrderReceivedInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("GetPOReceivedInfoByPOSysNo");
            command.SetParameterValue("@POSysNo", poInfo.SysNo.Value);
            result = command.ExecuteEntityList<PurchaseOrderReceivedInfo>();
            return result;
        }

        public PurchaseOrderEIMSRuleInfo GetEIMSRuleInfoBySysNo(int ruleNumber)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSRuteInfoByNumber");
            dataCommand.AddInputParameter("@RuleNumber", DbType.Int32, ruleNumber);
            PurchaseOrderEIMSRuleInfo ruleInfo = dataCommand.ExecuteEntity<PurchaseOrderEIMSRuleInfo>();
            if (ruleInfo != null)
            {   //获取合同返利信息
                ruleInfo.RebateScheme = GetEIMSRuteRebateSchemeInfoByNumber(ruleNumber);
                ruleInfo.RebateSchemeTransactions = GetEIMSRuleRebateSchemeTransactionInfosByNumber(ruleInfo.RebateScheme.TransactionNumber.Value);
            }
            return ruleInfo;
        }

        public PurchaseOrderEIMSRuleInfo GetEIMSRuteInfoByAssignedCode(string ruleNumber)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSRuteInfoByAssignedCode");
            dataCommand.AddInputParameter("@AssignedCode", DbType.String, ruleNumber);
            PurchaseOrderEIMSRuleInfo rule = dataCommand.ExecuteEntity<PurchaseOrderEIMSRuleInfo>();

            if (rule != null)
            {   //获取合同返利信息
                rule.RebateScheme = GetEIMSRuteRebateSchemeInfoByNumber(rule.RuleNumber.Value);
                rule.RebateSchemeTransactions = GetEIMSRuleRebateSchemeTransactionInfosByNumber(rule.RebateScheme.TransactionNumber.Value);
            }
            return rule;
        }

        /// <summary>
        /// 获取合同返利信息
        /// </summary>
        /// <param name="ruleNumber">合同编号</param>
        /// <returns></returns>
        private EIMSRuleRebateScheme GetEIMSRuteRebateSchemeInfoByNumber(int ruleNumber)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSRuteRebateSchemeInfoByNumber");
            dataCommand.AddInputParameter("@RuleNumber", DbType.Int32, ruleNumber);
            EIMSRuleRebateScheme rebateScheme = dataCommand.ExecuteEntity<EIMSRuleRebateScheme>();
            return rebateScheme;
        }

        /// <summary>
        /// 获取合同返利信息（明细）
        /// </summary>
        /// <param name="ruleNumber"></param>
        /// <returns></returns>
        private List<EIMSRuleRebateSchemeTransaction> GetEIMSRuleRebateSchemeTransactionInfosByNumber(int rebateSchemeNumber)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSRuleRebateSchemeTransactionInfoByNumber");
            dataCommand.AddInputParameter("@RebateSchemeNumber", DbType.Int32, rebateSchemeNumber);
            List<EIMSRuleRebateSchemeTransaction> rebateSchemes = dataCommand.ExecuteEntityList<EIMSRuleRebateSchemeTransaction>();
            return rebateSchemes;
        }

        /// <summary>
        ///  加载PO单预计到货时间 - 审核信息
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public PurchaseOrderETATimeInfo LoadPOETATimeInfo(int poSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPOETA");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "Sysno desc"))
            {
                PurchaseOrderETATimeInfo poETA;
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "POSysNo",
                DbType.Int32, "@POSysNo", QueryConditionOperatorType.Equal, poSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "STATUS",
                DbType.Int32, "@STATUS", QueryConditionOperatorType.Equal, 1);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                poETA = dataCommand.ExecuteEntity<PurchaseOrderETATimeInfo>();
                return poETA;
            }
        }

        public List<EIMSInfo> GetEIMSInvoiceInfoByPOSysNo(int poSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetEIMSInvoiceInfoByPOSysNo");
            dataCommand.SetParameterValue("@POSysNo", poSysNo);
            List<EIMSInfo> poEimsEntitys = dataCommand.ExecuteEntityList<EIMSInfo>();
            return poEimsEntitys;
        }

        public EIMSInfo GetAvilableEimsAmtInfo(int sysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("GetAvilableEimsAmtInfo");
            cmd.SetParameterValue("@InvoiceNumber", sysNo);
            return cmd.ExecuteEntity<EIMSInfo>();
        }

        public int DeletePOItems(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeletePOItemsByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);

            return command.ExecuteNonQuery();
        }

        public PurchaseOrderInfo ConfirmVendorPortalPurchaseOrder(PurchaseOrderInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ConfirmVendorPortalPO");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@VendorSysNo", entity.VendorInfo.SysNo);
            command.SetParameterValue("@POID", entity.PurchaseOrderBasicInfo.PurchaseOrderID);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@ShipTypeSysNo", entity.PurchaseOrderBasicInfo.ShippingType.SysNo);
            command.SetParameterValue("@PayTypeSysNo", entity.PurchaseOrderBasicInfo.PayType.SysNo);
            command.SetParameterValue("@CurrencySysNo", entity.PurchaseOrderBasicInfo.CurrencyCode);
            command.SetParameterValue("@ExchangeRate", entity.PurchaseOrderBasicInfo.ExchangeRate);
            command.SetParameterValue("@TotalAmt", entity.PurchaseOrderBasicInfo.TotalAmt);
            command.SetParameterValue("@ETP", entity.PurchaseOrderBasicInfo.ETP);
            command.SetParameterValue("@Status", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderStatus);
            command.SetParameterValue("@IsConsign", (int)entity.PurchaseOrderBasicInfo.ConsignFlag);
            command.SetParameterValue("@POType", (int)entity.PurchaseOrderBasicInfo.PurchaseOrderType);
            command.SetParameterValue("@PM_ReturnPointSysNo", entity.PurchaseOrderBasicInfo.PM_ReturnPointSysNo);
            command.SetParameterValue("@UsingReturnPoint", entity.PurchaseOrderBasicInfo.UsingReturnPoint);
            command.SetParameterValue("@ReturnPointC3SysNo", entity.PurchaseOrderBasicInfo.ReturnPointC3SysNo);
            command.SetParameterValue("@TaxRate", entity.PurchaseOrderBasicInfo.TaxRate);
            command.SetParameterValue("@PMSysNo", entity.PurchaseOrderBasicInfo.ProductManager.SysNo);
            command.SetParameterValue("@SettlementCompany", entity.PurchaseOrderBasicInfo.SettleCompanySysNo);
            command.SetParameterValue("@ExecptStatus", entity.PurchaseOrderBasicInfo.PurchaseOrderExceptStatus);
            command.SetParameterValue("@Memo", entity.PurchaseOrderBasicInfo.MemoInfo.Memo);
            command.SetParameterValue("@Note", entity.PurchaseOrderBasicInfo.MemoInfo.Note);
            command.SetParameterValue("@PMRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo);
            command.SetParameterValue("@TLRequestMemo", entity.PurchaseOrderBasicInfo.MemoInfo.TLRequestMemo);
            command.SetParameterValue("@InTime", DBNull.Value);
            command.SetParameterValue("@ETATime", entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue ? entity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.ToString("yyyy-MM-dd 00:00:00") : null);
            command.SetParameterValue("@ETAHalfDay", entity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.ToString());
            command.SetParameterValue("@CreateUserSysNo", entity.PurchaseOrderBasicInfo.CreateUserSysNo);
            if (null != entity.PurchaseOrderBasicInfo.StockInfo && entity.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                string stockStr = entity.PurchaseOrderBasicInfo.StockInfo.SysNo.Value.ToString();
                //有中转仓
                if (null != entity.PurchaseOrderBasicInfo.ITStockInfo && entity.PurchaseOrderBasicInfo.ITStockInfo.SysNo.HasValue)
                {
                    command.SetParameterValue("@ITStockSysNo", entity.PurchaseOrderBasicInfo.ITStockInfo.SysNo);
                    command.SetParameterValue("@StockSysNo", entity.PurchaseOrderBasicInfo.StockInfo.SysNo);
                }
                else if (stockStr.Length > 2)
                {
                    command.SetParameterValue("@ITStockSysNo", Int32.Parse(stockStr.Substring(2)));
                    command.SetParameterValue("@StockSysNo", 50);
                }
                else
                {
                    command.SetParameterValue("@ITStockSysNo", DBNull.Value);
                    command.SetParameterValue("@StockSysNo", entity.PurchaseOrderBasicInfo.StockInfo.SysNo);
                }
            }
            else
            {
                command.SetParameterValue("@StockSysNo", entity.PurchaseOrderBasicInfo.StockInfo.SysNo);
            }
            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return entity;
        }

        public int DeletePOItem(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeletePOItem");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery();
        }

        public void UpdatePOMasterTotalAmt(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOMasetToAmt");
            command.SetParameterValue("@SysNo", poSysNo);
            command.ExecuteNonQuery();
        }

        public void UpdateAllAcquireReturnInfo(int poSysNo, int? AcquireReturnPointType, decimal? AcquireReturnPoint)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateAllAcquireReturnInfo");
            command.SetParameterValue("@AcquireReturnPointType", AcquireReturnPointType);
            command.SetParameterValue("@AcquireReturnPoint", AcquireReturnPoint);
            command.SetParameterValue("@POSysNo", poSysNo);
            command.ExecuteNonQuery();
        }

        public PurchaseOrderItemInfo AddPurchaseOrderItemByProduct(PurchaseOrderItemProductInfo productInfo)
        {
            PurchaseOrderItemInfo item = null;
            DataCommand command = DataCommandManager.GetDataCommand("AddPOItemByProductSysNo");
            command.SetParameterValue("@ProductSysNo", productInfo.SysNo);
            command.SetParameterValue("@CurrencySysNo", productInfo.CurrencySysNo);
            item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            return item;
        }

        public PurchaseOrderItemInfo GetExtendPurchaseOrderItemInfo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetJDPriceAndM1AndAndAvailableQty");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            PurchaseOrderItemInfo item = command.ExecuteEntity<PurchaseOrderItemInfo>();
            if (!item.M1.HasValue)
            {
                item.M1 = 0;
            }
            return item;
        }

        public bool IsVirtualStockPurchaseOrderProduct(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("V_INM_Inventory");
            command.SetParameterValue("@ItemSysNumber", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["VirtualQty"] != null && (int)dt.Rows[0]["VirtualQty"] != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<PurchaseOrderItemProductInfo> QueryPurchaseOrderGiftList(List<int> productSysNos)
        {
            if (productSysNos.Count == 0)
            {
                return new List<PurchaseOrderItemProductInfo>();
            }

            List<PurchaseOrderItemProductInfo> productGift = new List<PurchaseOrderItemProductInfo>();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetGiftList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, " sg.productsysno"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sg.status",
                 DbType.Int32, "@sgStatus", QueryConditionOperatorType.Equal, 0);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.status",
                DbType.Int32, "@pStatus", QueryConditionOperatorType.Equal, 2);

                string arrary = "";
                foreach (var sysNo in productSysNos)
                {
                    arrary += sysNo.ToString() + " , ";
                }
                arrary = arrary.Substring(0, arrary.LastIndexOf(","));

                dataCommand.CommandText = builder.BuildQuerySql() + " AND sg.productsysno IN (" + arrary + ")";
                using (IDataReader dr = dataCommand.ExecuteDataReader())
                {
                    while (dr.Read())
                    {
                        productGift.Add(new PurchaseOrderItemProductInfo
                        {
                            Id = dr["productid"] != null ? dr["productid"].ToString() : "",
                            GiftId = dr["GiftId"] != null ? dr["GiftId"].ToString() : "",
                            ProductName = dr["ProductName"] != null ? dr["ProductName"].ToString() : "",
                            OnlineQty = dr["OnlineQty"] != null ? Convert.ToInt32(dr["OnlineQty"].ToString()) : 0
                        });
                    }
                }
            }
            return productGift;
        }

        public List<PurchaseOrderItemProductInfo> QueryPurchaseOrderAccessoriesList(List<int> productSysNos)
        {
            if (productSysNos.Count == 0)
            {
                return null;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPurchaseOrderAccessoriesBySysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, " Value"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product_Accessories.Status",
                 DbType.String, "@Status", QueryConditionOperatorType.Equal, 0);

                string condition = " AND (";
                foreach (var sysNo in productSysNos)
                {
                    condition += "Product_Accessories.ProductSysno = " + sysNo.ToString() + " OR ";
                }
                condition = condition.Substring(0, condition.LastIndexOf("OR"));
                condition += ")";

                var sysNoList = productSysNos.ConvertAll<string>(p => p.ToString()).ToArray();
                string CustomCondition = "atc.ProductSysNo IN (" + string.Join(",", sysNoList) + ")";
                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("OtherCondition", condition).Replace("#CustomCondition#", CustomCondition);
            }
            return dataCommand.ExecuteEntityList<PurchaseOrderItemProductInfo>();
        }

        public void GetPurchaseOrderItemExceptStatus(PurchaseOrderItemInfo info)
        {
            info.ExecptStatus = 0;//ExecptStatus.Normal

            if (info.LastOrderPrice.HasValue)
            {
                if (info.LastOrderPrice != 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("POItemPriceLimit");
                    command.SetParameterValue("@Status", 0);
                    command.SetParameterValue("@OrderPrice", (int)info.LastOrderPrice);
                    command.SetParameterValue("@CompanyCode", info.CompanyCode);
                    DataTable dt = command.ExecuteDataTable();

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        decimal topRate = dr["TopRate"] == null ? 0 : Convert.ToDecimal(dr["TopRate"].ToString());
                        decimal lowRate = dr["LowRate"] == null ? 0 : Convert.ToDecimal(dr["LowRate"].ToString());

                        if (((info.OrderPrice - info.LastOrderPrice) / info.LastOrderPrice.Value == 0 ? 1 : info.LastOrderPrice) > topRate
                        || ((info.OrderPrice - info.LastOrderPrice) / info.LastOrderPrice.Value == 0 ? 1 : info.LastOrderPrice) < lowRate)
                        {
                            info.ExecptStatus = -1;
                        }
                    }
                }
            }
        }

        public int UpdateMailAddressByPOSysNo(int POSysNo, string MailAddress)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateMailAddressByPOSysNo");
            command.SetParameterValue("@SysNo", POSysNo);
            command.SetParameterValue("@MailAddress", MailAddress);
            return command.ExecuteNonQuery();
        }

        public string GetMailAddressByPOSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetMailAddressByPOSysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            return command.ExecuteScalar() == null ? "" : command.ExecuteScalar().ToString();
        }

        public PurchaseOrderItemInfo LoadPOItem(int itemSysNo)
        {
            PurchaseOrderItemInfo result = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetPOItemsBySysNo");
            command.SetParameterValue("@SysNo", itemSysNo);
            result = command.ExecuteEntity<PurchaseOrderItemInfo>();
            return result;
        }

        public void UpdatePurchaseOrderBatchInfo(PurchaseOrderItemInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePoItemBatchInfo");
            cmd.SetParameterValue("@BatchInfo", info.BatchInfo);
            cmd.SetParameterValue("@SysNo", info.ItemSysNo);
            cmd.ExecuteNonQuery();
        }

        public bool CheckReturnPurchaseOrderValid(int stockSysNo, int productSysNo, int quantity, string companyCode)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("CheckReturnPurchaseOrderValid");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, " WarehouseSysNumber desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WarehouseSysNumber",
                DbType.Int32, "@WarehouseSysNumber", QueryConditionOperatorType.Equal, stockSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ItemSysNumber",
                DbType.Int32, "@ItemSysNumber", QueryConditionOperatorType.Equal, productSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                DbType.Int32, "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataSet ds = dataCommand.ExecuteDataSet();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["AvailableQty"]) + Convert.ToInt32(ds.Tables[0].Rows[0]["ConsignQty"]) < Math.Abs(quantity))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public DataSet GetPOHistoryAbsentInvoiceOrWaitingInStock(int vendorSysNo, int status, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOHistoryAbsentInvoiceOrWaitingInStock");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteDataSet();
        }

        public bool IsBatchProduct(PurchaseOrderItemInfo poItem)
        {
            PurchaseOrderItemInfo poItemEntity = new PurchaseOrderItemInfo();
            DataCommand cmd = DataCommandManager.GetDataCommand("IsBatchProductInfo");
            cmd.SetParameterValue("@PreviousProductSysNo", poItem.ProductSysNo);
            poItemEntity = cmd.ExecuteEntity<PurchaseOrderItemInfo>();
            if (poItemEntity != null && poItemEntity.ProductSysNo.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion IPurchaseOrderDA Members

        #region IPurchaseOrderDA Members

        public string GetItemAccessoriesStringByPurchaseOrder(List<int?> productSysNoList, string companyCode)
        {
            if (productSysNoList.Count == 0)
            {
                return string.Empty;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductAccessoriesByProductSysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, " Value"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product_Accessories.Status",
                 DbType.String, "@Status", QueryConditionOperatorType.Equal, 0);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product_Accessories.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
                "@CompanyCode", QueryConditionOperatorType.Equal, companyCode);

                string condition = " AND (";
                foreach (var sysNo in productSysNoList)
                {
                    condition += "Product_Accessories.ProductSysno = " + sysNo.ToString() + " OR ";
                }
                condition = condition.Substring(0, condition.LastIndexOf("OR"));
                condition += ")";
                var sysNoList = productSysNoList.ConvertAll<string>(p => p.ToString()).ToArray();
                string CustomCondition = "atc.ProductSysNo IN (" + string.Join(",", sysNoList) + ")";
                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("OtherCondition", condition).Replace("#CustomCondition#", CustomCondition);
            }

            DataTable dt = dataCommand.ExecuteDataSet().Tables[0];
            string des = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                des += "【" + dr["ProductID"].ToString() + "】 " + dr["Description"].ToString();
            }

            return des;
        }

        public decimal? JDPriceByProductSysNo(int productSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetProduct");
            ProductInfo result = new ProductInfo();
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "v_ici.SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v_ici.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal, productSysNo);

                command.CommandText = builder.BuildQuerySql();
                DataTable dt = command.ExecuteDataSet().Tables[0];
                if (dt.Rows.Count > 0 && dt.Rows[0]["JDPrice"] != null && dt.Rows[0]["JDPrice"].ToString() != "")
                {
                    return Convert.ToDecimal(dt.Rows[0]["JDPrice"]);
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? GetLastPriceBySysNo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProduct_LastPOInfo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["LastPrice"] != null)
            {
                return Convert.ToDecimal(dt.Rows[0]["LastPrice"]);
            }
            return null;
        }

        #endregion IPurchaseOrderDA Members

        public int? GetUnActivatyCount(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Basket_V_INM_Inventory");
            command.SetParameterValue("@ItemSysNumber", productSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["UnActivatyCount"] != null)
                {
                    string count = dt.Rows[0]["UnActivatyCount"].ToString().Trim();
                    if (!string.IsNullOrEmpty(count))
                    {
                        return int.Parse(count);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return null;
        }

        #region IPurchaseOrderDA Members

        public DateTime? GetLastPoDate(int productSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetLastPoDate");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            return dataCommand.ExecuteScalar<Nullable<DateTime>>();
        }

        public int? GetPurchaseOrderReturnPointSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPurchaseOrderReturnPointSysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            var result = command.ExecuteScalar();

            if (result == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }

        public string GetWareHouseReceiptSerialNumber(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetWHReceiptSNBySysNo");
            command.SetParameterValue("@SysNo", poSysNo);
            object o = command.ExecuteScalar();
            if (o == null)
            {
                return "";
            }
            else
            {
                return o.ToString();
            }
        }

        public int UpdateGetWareHouseReceiptSerialNumber(int poSysNo, string receiptSerialNumber)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateWHReceiptSN");
            command.SetParameterValue("@SysNo", poSysNo);
            command.SetParameterValue("@WHReceiptSN", receiptSerialNumber);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据供应商系统编号取得采购单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <returns></returns>
        public List<int> GetPurchaseOrderSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPurchaseOrderSysNoListByVendorSysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
              dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VendorSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, vendorSysNo);
                if (pmSysNoList != null && pmSysNoList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "PMSysNo", DbType.Int32, pmSysNoList);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                return dataCommand.ExecuteFirstColumn<Int32>();
            }
        }

        #endregion IPurchaseOrderDA Members

        public List<PurchaseOrderItemInfo> LoadPOItemAddInfo(List<int> productSysNos)
        {
            string posysnos = productSysNos.Join(",");
            DataCommand dataCommand = DataCommandManager.GetDataCommand("QueryItemStockInfoList");
            dataCommand.ReplaceParameterValue("@ProductSysNos", posysnos);
            return dataCommand.ExecuteEntityList<PurchaseOrderItemInfo>();
        }


        public void LoadPMSaleInfo(int pmUserSysNo, out decimal saleRatePerMonth, out decimal saleTargetPerMonth, out decimal maxAmtPerOrder, out decimal maxAmtPerDay, out decimal tlSaleRatePerMonth, out decimal pmdMaxAmtPerOrder, out decimal pmdMaxAmtPerDay, string companyCode)
        {
            saleRatePerMonth = 0m;
            saleTargetPerMonth = 0m;
            maxAmtPerOrder = 0m;
            maxAmtPerDay = 0m;
            tlSaleRatePerMonth = 0m;
            pmdMaxAmtPerDay = 0m;
            pmdMaxAmtPerOrder = 0m;
            DataCommand command = DataCommandManager.GetDataCommand("GetPMSaleInfor");
            command.SetParameterValue("@PMUserSysNo", pmUserSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt.Rows.Count > 0)
            {
                saleRatePerMonth = (dt.Rows[0]["SaleRatePerMonth"] != null && dt.Rows[0]["SaleRatePerMonth"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["SaleRatePerMonth"]) : 0m;
                saleTargetPerMonth = (dt.Rows[0]["SaleTargetPerMonth"] != null && dt.Rows[0]["SaleTargetPerMonth"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["SaleTargetPerMonth"]) : 0m;
                maxAmtPerOrder = (dt.Rows[0]["MaxAmtPerOrder"] != null && dt.Rows[0]["MaxAmtPerOrder"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["MaxAmtPerOrder"]) : 0m;
                maxAmtPerDay = (dt.Rows[0]["MaxAmtPerDay"] != null && dt.Rows[0]["MaxAmtPerDay"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["MaxAmtPerDay"]) : 0m;
                tlSaleRatePerMonth = (dt.Rows[0]["TLSaleRatePerMonth"] != null && dt.Rows[0]["TLSaleRatePerMonth"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["TLSaleRatePerMonth"]) : 0m;

                pmdMaxAmtPerOrder = (dt.Rows[0]["PMDMaxAmtPerOrder"] != null && dt.Rows[0]["PMDMaxAmtPerOrder"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["PMDMaxAmtPerOrder"]) : 0m;

                pmdMaxAmtPerDay = (dt.Rows[0]["PMDMaxAmtPerDay"] != null && dt.Rows[0]["PMDMaxAmtPerDay"].ToString() != "") ? Convert.ToDecimal(dt.Rows[0]["PMDMaxAmtPerDay"]) : 0m;
            }
        }

        public decimal GetPMInventoryAmt(int pmSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPMInventoryAmt");
            command.SetParameterValue("@pmSysNo", pmSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToDecimal(dt.Rows[0][0]);
            }
            return 0m;
        }

        public DataTable GetC2TotalAmt(string C2SysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetC2TotalAmt");
            command.ReplaceParameterValue("@C2SysNo", C2SysNo);
            return command.ExecuteDataTable();
        }

        public bool RetreatVendorPortalPurchaseOrder(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("RetreatVendorPortalPO");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery() > 0;
        }

        public decimal? GetPurchaseOrderItemLastPrice(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductLastOrderPrice");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            object obj = command.ExecuteScalar();
            if (null != obj && obj.ToString().Length > 0)
            {
                return Convert.ToDecimal(obj.ToString());
            }
            else
            {
                return null;
            }
        }

        public decimal? GetPurchaseOrderItemJingDongPrice(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetJDPrice");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            object obj = command.ExecuteScalar();
            if (null != obj && obj.ToString().Length > 0)
            {
                return Convert.ToDecimal(obj.ToString());
            }
            else
            {
                return null;
            }
        }

        public void UpdatePurchaseOrderInstockAmt(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetPoMasterToAmt");
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            cmd.ExecuteNonQuery();
        }

        public int SetPurchaseOrderStatusClose(int poSysNo, string closeUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetPoStatueIsClose");
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            cmd.SetParameterValue("@CloseUser", closeUser);
            cmd.SetParameterValue("@CloseTime", DateTime.Now);

            return cmd.ExecuteNonQuery();
        }

        public List<PurchaseOrderInfo> GetNeedToClosePurchaseOrderList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNeedClosePoSysNo");
            return cmd.ExecuteEntityList<PurchaseOrderInfo>();
        }

        public List<PurchaseOrderInfo> GetPurchaseOrderForJobETA(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPOForJobETA");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<PurchaseOrderInfo>();
        }

        public int AbandonPOForJOB(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbandonPOForJob");

            cmd.SetParameterValue("@SysNo", poSysNo);
            return cmd.ExecuteNonQuery();
        }

        public int AbandonETAForJOB(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbandonETAForJob");
            cmd.SetParameterValue("@SysNo", poSysNo);
            return cmd.ExecuteNonQuery();
        }

        public int UpdateExtendPOInfoForJob(int poSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateExtendPOInfo");
            cmd.SetParameterValue("@SysNo", poSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteNonQuery();
        }

        public PurchaseOrderItemInfo GetPurchaseOrderItemSalesTrend(int productSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPurchaseOrderItemSalesInfo");
            dataCommand.ReplaceParameterValue("@ProductSysNo", productSysNo);
            return dataCommand.ExecuteEntity<PurchaseOrderItemInfo>();
        }

        public DataTable GetPurchaseOrderItemMinInterestRate(string productSysNos)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetMinInterestRate");
            command.ReplaceParameterValue("@SysNo", productSysNos);
            return command.ExecuteDataTable();
        }

        public List<PurchaseOrderItemInfo> SearchPOItemsList(int poSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPOItemsByHasPagePOSysNo");
            dataCommand.ReplaceParameterValue("@POSysNo", poSysNo);
            return dataCommand.ExecuteEntityList<PurchaseOrderItemInfo>();
        }


        public List<int> GetProductSysNoByPOSysNo(int poSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetProductSysNoByPOSysNo");
            dataCommand.SetParameterValue("@POSysNo", poSysNo);
            return dataCommand.ExecuteEntityList<PurchaseOrderItemInfo>().Select(x => x.ProductSysNo.Value).ToList();
        }

        public int GetCostQuantity(int productSysNo, decimal cost, int stockSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetCostQuantity");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@Cost", cost);
            dataCommand.SetParameterValue("@WarehouseNumber", stockSysNo);
            object obj = dataCommand.ExecuteScalar();
            if (null != obj && obj!=DBNull.Value)
            {
                return Convert.ToInt32(obj);
            }
            else
            {
                return 0;
            }
        }               
    }
}
