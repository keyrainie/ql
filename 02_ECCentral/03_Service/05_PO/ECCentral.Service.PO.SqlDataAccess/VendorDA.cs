using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.BizEntity.PO;

using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IVendorDA))]
    public class VendorDA : IVendorDA
    {
        #region IVendorDA Members

        public BizEntity.PO.VendorInfo LoadVendorInfo(int vendorSysNo)
        {
            VendorInfo returnVendorInfo = new VendorInfo();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorbySysNoRequestRank");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "V.SysNo",
                DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, vendorSysNo);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                returnVendorInfo = dataCommand.ExecuteEntity<VendorInfo>();
                //Vendor venderHasRank = QueryHasRANKVendor(sysNo);
                //if (venderHasRank != null)
                //{
                //    vendor.RANK = venderHasRank.RANK;
                //    vendor.RequestRank = venderHasRank.RequestRank;
                //}
                //if (vendor.Status == (int)VendorVerifyStatus.VerifyUnPass)
                //{
                //    vendor.PayPeriodTypeNew = vendor.PayPeriodType;
                //}

                //if (vendor.Status == (int)VendorVerifyStatus.Apply)
                //{
                //    if (vendor.ValidDate.HasValue)
                //    {
                //        vendor.ValidDateNew = vendor.ValidDate.Value.ToString(AppConst.DATETIME_DATE);
                //    }
                //    if (vendor.ExpiredDate.HasValue)
                //    {
                //        vendor.ExpiredDateNew = vendor.ExpiredDate.Value.ToString(AppConst.DATETIME_DATE);
                //    }
                //    if (vendor.ContractAmt.HasValue)
                //    {
                //        vendor.ContractAmtNew = vendor.ContractAmt.Value.ToString(AppConst.DECIMAL_FORMAT);
                //    }
                //    vendor.PayPeriodTypeNew = vendor.PayPeriodType;
                //}
                //else
                //{
                //    vendor.ValidDateNew = "";
                //    vendor.ExpiredDateNew = "";
                //    vendor.ContractAmtNew = "";
                //}
                //if (vendor.ValidDate.HasValue)
                //{
                //    vendor.ValidDateOld = vendor.ValidDate.Value.ToString(AppConst.DATETIME_DATE);
                //}
                //if (vendor.ExpiredDate.HasValue)
                //{
                //    vendor.ExpiredDateOld = vendor.ExpiredDate.Value.ToString(AppConst.DATETIME_DATE);
                //}
                //if (vendor.ContractAmt.HasValue)
                //{
                //    vendor.ContractAmtOld = vendor.ContractAmt.Value.ToString(AppConst.DECIMAL_FORMAT);
                //}
                //if (vendor.TotalPOMoney != null)
                //{
                //    vendor.TotalPOMoney = Convert.ToDecimal(vendor.TotalPOMoney).ToString(AppConst.DECIMAL_FORMAT);
                //}

                return returnVendorInfo;
            }
        }

        public VendorInfo LoadVendorInfoByVendorName(string vendorName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorByVendorName");
            command.SetParameterValue("@VendorName", vendorName);
            return command.ExecuteEntity<VendorInfo>();
        }

        public VendorExtendInfo LoadVendorExtendInfo(VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorExInfo");
            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
            return command.ExecuteEntity<VendorExtendInfo>();
        }

        public List<VendorHistoryLog> LoadVendorHistoryLog(VendorInfo vendorInfo)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                StartRowIndex = 0,
                MaximumRows = Int32.MaxValue
            };
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorHitoryByVendorSysNo");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "a.CreateTime desc"))
            {
                if (vendorInfo.SysNo.HasValue && vendorInfo.SysNo.Value > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.VendorSysNo",
                    DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, vendorInfo.SysNo.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.Int32, "@CompanyCode", QueryConditionOperatorType.Equal, vendorInfo.CompanyCode);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }

            dataCommand.SetParameterValue("@StartNumber", pagingInfo.StartRowIndex);
            dataCommand.SetParameterValue("@EndNumber", pagingInfo.MaximumRows);

            List<VendorHistoryLog> returnList = dataCommand.ExecuteEntityList<VendorHistoryLog>();


            #region 构造页面显示内容
            foreach (VendorHistoryLog item in returnList)
            {
                if (!string.IsNullOrEmpty(item.AuditUserName) && item.AuditTime.HasValue)
                {
                    item.DisplayName = item.DisplayName + " -> " + item.AuditUserName;
                    //item.HistoryDate = item.CreateTime + " -> " + item.AuditTime.Value;
                }
                if (item.RequestType == VendorModifyRequestType.Finance || (item.RequestType == VendorModifyRequestType.Vendor && !string.IsNullOrEmpty(item.Memo)) || item.RequestType == VendorModifyRequestType.Manufacturer)//需要审核的，要写备注
                {
                    if (item.Status != VendorModifyRequestStatus.Common)
                    {
                        item.Memo += "(" + EnumHelper.GetDisplayText(item.Status.Value) + ")";
                    }
                }
            }
            #endregion
            return returnList;
        }

        public List<VendorAgentInfo> LoadVendorAgentInfoList(VendorInfo vendorInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetManufacturerByVendorSysNo");
            dataCommand.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
            List<VendorAgentInfo> list = dataCommand.ExecuteEntityList<VendorAgentInfo>();
            //反序列化SaleRules的XML
            list.ForEach(delegate(VendorAgentInfo agentInfo)
            {
                agentInfo.VendorCommissionInfo.SaleRuleEntity = new VendorStagedSaleRuleEntity();
                if (!string.IsNullOrEmpty(agentInfo.VendorCommissionInfo.StagedSaleRuleItemsXml))
                {
                    StringReader xmlReader = new StringReader(agentInfo.VendorCommissionInfo.StagedSaleRuleItemsXml);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(VendorStagedSaleRuleEntity));
                    VendorStagedSaleRuleEntity saleRuleEntity = xmlSerializer.Deserialize(xmlReader) as VendorStagedSaleRuleEntity;
                    if (null != saleRuleEntity)
                    {
                        agentInfo.VendorCommissionInfo.SaleRuleEntity = saleRuleEntity;
                    }
                }
            });
            return list;
        }

        public VendorAgentInfo LoadVendorAgentInfoByVendorAndProductID(int vendorSysNo, int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorManufacturerByVendorNoAndProductNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            return command.ExecuteEntity<VendorAgentInfo>();
        }

        public VendorAgentInfo SaveVendorAgentInfo(VendorAgentInfo vendorAgentInfo)
        {
            throw new NotImplementedException();
        }

        public VendorInfo SaveVendorBasicInfo(VendorInfo vendorInfo)
        {
            throw new NotImplementedException();
        }

        public VendorInfo CreateVendorInfo(VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendor");
            //command.SetParameterValue("@VendorID", entity.VendorID);
            command.SetParameterValue("@VendorName", vendorInfo.VendorBasicInfo.VendorNameLocal);
            command.SetParameterValue("@EnglishName", vendorInfo.VendorBasicInfo.VendorNameGlobal);
            command.SetParameterValue("@BriefName", vendorInfo.VendorBasicInfo.VendorBriefName);
            command.SetParameterValue("@VendorType", (int)vendorInfo.VendorBasicInfo.VendorType);
            command.SetParameterValue("@District", vendorInfo.VendorBasicInfo.District);
            command.SetParameterValue("@Address", vendorInfo.VendorBasicInfo.Address);
            command.SetParameterValue("@Zip", vendorInfo.VendorBasicInfo.ZipCode);
            command.SetParameterValue("@Phone", vendorInfo.VendorBasicInfo.Phone);
            command.SetParameterValue("@Fax", vendorInfo.VendorBasicInfo.Fax);
            command.SetParameterValue("@Email", vendorInfo.VendorBasicInfo.EmailAddress);
            command.SetParameterValue("@Site", vendorInfo.VendorBasicInfo.Website);
            command.SetParameterValue("@Bank", vendorInfo.VendorFinanceInfo.BankName);
            command.SetParameterValue("@Account", vendorInfo.VendorFinanceInfo.AccountNumber);
            command.SetParameterValue("@TaxNo", vendorInfo.VendorFinanceInfo.TaxNumber);
            command.SetParameterValue("@Contact", vendorInfo.VendorBasicInfo.Contact);
            command.SetParameterValue("@Comment", vendorInfo.VendorBasicInfo.Comment);
            command.SetParameterValue("@Note", vendorInfo.VendorBasicInfo.Note);
            command.SetParameterValue("@Status", (int)vendorInfo.VendorBasicInfo.VendorStatus.Value);
            command.SetParameterValue("@RMAPolicy", vendorInfo.VendorServiceInfo.RMAPolicy);
            command.SetParameterValue("@PayPeriod", vendorInfo.VendorFinanceInfo.PayPeriod);
            command.SetParameterValue("@RepairAddress", vendorInfo.VendorServiceInfo.Address);
            command.SetParameterValue("@RepairAreaSysNo", vendorInfo.VendorServiceInfo.AreaInfo.SysNo);
            command.SetParameterValue("@RepairContact", vendorInfo.VendorServiceInfo.Contact);
            command.SetParameterValue("@RepairContactPhone", vendorInfo.VendorServiceInfo.ContactPhone);
            command.SetParameterValue("@Cellphone", vendorInfo.VendorBasicInfo.CellPhone);
            command.SetParameterValue("@AcctContactName", vendorInfo.VendorFinanceInfo.AccountContact);
            command.SetParameterValue("@AcctPhone", vendorInfo.VendorFinanceInfo.AccountPhone);
            command.SetParameterValue("@RMAServiceArea", vendorInfo.VendorServiceInfo.RMAServiceArea);
            command.SetParameterValue("@IsConsign", (int)vendorInfo.VendorBasicInfo.ConsignFlag);
            command.SetParameterValue("@PayPeriodType", vendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo);
            command.SetParameterValue("@ValidDate", vendorInfo.VendorFinanceInfo.CooperateValidDate);
            command.SetParameterValue("@ExpiredDate", vendorInfo.VendorFinanceInfo.CooperateExpiredDate);
            command.SetParameterValue("@ContractAmt", vendorInfo.VendorFinanceInfo.CooperateAmt);
            command.SetParameterValue("@TotalPOMoney", vendorInfo.VendorFinanceInfo.TotalPOAmt);
            command.SetParameterValueAsCurrentUserSysNo("@CreateVendorUserSysNo");
            command.SetParameterValue("@VendorContractInfo", vendorInfo.VendorBasicInfo.VendorContractInfo);
            command.SetParameterValue("@IsCooperate", (int)vendorInfo.VendorBasicInfo.VendorIsCooperate.Value);
            command.SetParameterValue("@AcctContactEmail", vendorInfo.VendorFinanceInfo.AccountContactEmail);
            command.SetParameterValue("@SellerID", vendorInfo.VendorBasicInfo.SellerID);
            command.SetParameterValue("@RepairPostcode", vendorInfo.VendorServiceInfo.ZipCode);
            command.SetParameterValue("@CompanyCode", vendorInfo.CompanyCode);
            command.SetParameterValue("@PaySettleCompany", LegacyEnumMapper.ConvertPaySettleCompany(vendorInfo.VendorBasicInfo.PaySettleCompany));
            command.SetParameterValue("@EPortSysNo", vendorInfo.VendorBasicInfo.EPortSysNo);
            VendorInfo vendorEntity = command.ExecuteEntity<VendorInfo>();
            vendorEntity.VendorBasicInfo.ExtendedInfo.IsShowStore = vendorInfo.VendorBasicInfo.ExtendedInfo.IsShowStore;
            vendorEntity.VendorBasicInfo.ExtendedInfo.QQNumber = vendorInfo.VendorBasicInfo.ExtendedInfo.QQNumber;
            vendorEntity.VendorBasicInfo.ExtendedInfo.IM_Enabled = vendorInfo.VendorBasicInfo.ExtendedInfo.IM_Enabled;

            CreateOrEditIsShowStore(vendorEntity);
            return vendorEntity;
        }

        public VendorAttachInfo SaveVendorAttachmentInfo(VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateOrUpdateVendorAttachment");

            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
            command.SetParameterValue("@VendorApplyForm", vendorInfo.VendorAttachInfo.VendorApplyForm);
            command.SetParameterValue("@EnterpriseBusinessLicence", vendorInfo.VendorAttachInfo.EnterpriseBusinessLicence);
            command.SetParameterValue("@OrganizeCodeCertificate", vendorInfo.VendorAttachInfo.OrganizeCodeCertificate);
            command.SetParameterValue("@TaxationAffairsRegistration", vendorInfo.VendorAttachInfo.TaxationAffairsRegistration);
            command.SetParameterValue("@AgreementBeingSold", vendorInfo.VendorAttachInfo.AgreementBeingSold);
            command.SetParameterValue("@AgreementConsign", vendorInfo.VendorAttachInfo.AgreementConsign);
            command.SetParameterValue("@AgreementAfterSold", vendorInfo.VendorAttachInfo.AgreementAfterSold);
            command.SetParameterValue("@AgreementOther", vendorInfo.VendorAttachInfo.AgreementOther);
            command.SetParameterValue("@CompanyCode", vendorInfo.CompanyCode);

            return command.ExecuteEntity<VendorAttachInfo>();
        }

        public void UpdateVendorBuyWeekDay(VendorModifyRequestInfo modifyRequestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorBuyWeekDay");
            command.SetParameterValue("@VendorSysNo", modifyRequestInfo.VendorSysNo);
            command.SetParameterValue("@BuyWeekDay", modifyRequestInfo.BuyWeekDay);
            command.SetParameterValue("@SysNo", modifyRequestInfo.SysNo);
            command.SetParameterValue("@Status", (int)modifyRequestInfo.Status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.ExecuteNonQuery();
        }

        public VendorModifyRequestInfo CreateModifyRequest(VendorModifyRequestInfo requestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateModifyRequest");
            command.SetParameterValue("@VendorSysNo", requestInfo.VendorSysNo);
            command.SetParameterValue("@CompanyCode", requestInfo.CompanyCode);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@Status", requestInfo.Status);
            command.SetParameterValue("@RequestType", (int)requestInfo.RequestType);
            command.SetParameterValue("@RANK", requestInfo.Rank);
            command.SetParameterValue("@ActionType", (int?)requestInfo.ActionType);
            command.SetParameterValue("@ManufacturerSysNo", requestInfo.ManufacturerSysNo);
            command.SetParameterValue("@VendorManufacturerSysNo", requestInfo.VendorManufacturerSysNo);
            command.SetParameterValue("@LEVEL", requestInfo.AgentLevel);
            command.SetParameterValue("@C2SysNo", requestInfo.C2SysNo);
            command.SetParameterValue("@C3SysNo", requestInfo.C3SysNo);
            command.SetParameterValue("@Content", requestInfo.Content);
            command.SetParameterValue("@Memo", requestInfo.Memo);
            if (requestInfo.SettleType.HasValue)
            {
                command.SetParameterValue("@SettleType", requestInfo.SettleType.Value.ToString());
            }
            else
            {
                command.SetParameterValue("@SettleType", "");
            }

            command.SetParameterValue("@SettlePercentage", requestInfo.SettlePercentage.HasValue ? requestInfo.SettlePercentage.Value : 0);
            command.SetParameterValue("@BuyWeekDay", requestInfo.BuyWeekDay);
            command.SetParameterValue("@SendPeriod", requestInfo.SendPeriod);
            command.SetParameterValue("@BrandSysNo", requestInfo.BrandSysNo);

            //CRL20146 By Kilin
            command.SetParameterValue("@SettlePeriodType", (int?)requestInfo.SettlePeriodType);
            command.SetParameterValue("@AutoAudit", requestInfo.AutoAudit.HasValue && requestInfo.AutoAudit == true ? "Y" : null);
            command.SetParameterValue("@CompanyCode", requestInfo.CompanyCode);
            requestInfo.RequestSysNo = command.ExecuteScalar<int>();
            if (requestInfo.RequestSysNo != null && requestInfo.RequestSysNo.Value > 0)
            {
                return requestInfo;
            }
            else
            {
                return null;
            }
        }

        public BizEntity.PO.VendorModifyRequestInfo CreateVendorModifyRequest(BizEntity.PO.VendorModifyRequestInfo requestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorModifyRequest");

            command.SetParameterValue("@VendorSysNo", requestInfo.VendorSysNo);
            command.SetParameterValue("@PayPeriodType", requestInfo.PayPeriodType.PayTermsNo);
            command.SetParameterValue("@ValidDate", requestInfo.ValidDate);
            command.SetParameterValue("@ExpiredDate", requestInfo.ExpiredDate);
            command.SetParameterValue("@ContractAmt", requestInfo.ContractAmt);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@NewVendorName", requestInfo.VendorName); //request vendor new name
            command.SetParameterValue("@Status", (int)VendorModifyRequestStatus.Apply);
            command.SetParameterValue("@Content", requestInfo.Content);
            command.SetParameterValue("@ActionType", requestInfo.ActionType);
            command.SetParameterValue("@RequestType", (int?)requestInfo.RequestType);
            command.SetParameterValue("@AutoAudit", requestInfo.AutoAudit.HasValue && requestInfo.AutoAudit == true ? "Y" : null);

            //CRL20146 By Kilin
            command.SetParameterValue("@SettlePeriodType", (int?)requestInfo.SettlePeriodType);
            command.SetParameterValue("@CompanyCode", requestInfo.CompanyCode);

            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }

        public VendorModifyRequestInfo UpdateVendormodifyRequestStatus(VendorModifyRequestInfo requestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorModifyRequestStatus");

            command.SetParameterValue("@Status", (int)requestInfo.Status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@AuditTime", DateTime.Now);
            command.SetParameterValue("@SysNo", requestInfo.RequestSysNo);
            command.SetParameterValue("@Memo", requestInfo.Memo);
            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }

        public VendorAgentInfo CreateVendorManufacturerInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorManufacturer");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.SetParameterValue("@ManufacturerSysNo", DBNull.Value);
            command.SetParameterValue("@C2SysNo", DBNull.Value);
            command.SetParameterValue("@AgentLevel", DBNull.Value);
            command.SetParameterValue("@Status", (int)ValidStatus.A);
            command.SetParameterValue("@C3SysNo", DBNull.Value);
            command.SetParameterValue("@CompanyCode", vendorAgentInfo.CompanyCode);

            return command.ExecuteEntity<VendorAgentInfo>();
        }

        public VendorAgentInfo CreateVendorCommissionInfo(VendorAgentInfo vendorAgentInfo, int vendorSysNo, string createUserName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVMCommissionRule");

            string salesRuleStr = null;
            if (vendorAgentInfo.VendorCommissionInfo != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems.Count > 0)
            {
                salesRuleStr = SerializationUtility.XmlSerialize(vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity);
            }

            command.SetParameterValue("@VendorManufacturerSysNo", vendorAgentInfo.AgentSysNo);
            command.SetParameterValue("@OrderCommissionFee", vendorAgentInfo.VendorCommissionInfo.OrderCommissionAmt);
            command.SetParameterValue("@DeliveryFee", vendorAgentInfo.VendorCommissionInfo.DeliveryFee);
            command.SetParameterValue("@RentFee", vendorAgentInfo.VendorCommissionInfo.RentFee);
            command.SetParameterValue("@InUser", createUserName);
            command.SetParameterValue("@EditUser", createUserName);
            command.SetParameterValue("@SalesRule", salesRuleStr);
            command.SetParameterValue("@CompanyCode", vendorAgentInfo.CompanyCode);
            command.ExecuteNonQuery();
            return vendorAgentInfo;
        }

        public List<VendorAgentInfo> GetCheckVendorManufacturerInfo(int vendorManufacturerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorCheckInfo");
            command.SetParameterValue("@VendorManufacturerSysNo", vendorManufacturerSysNo);
            return command.ExecuteEntityList<VendorAgentInfo>();
        }

        public void UpdateVendorEmailAddress(int vendorSysNo, string newMailAddress)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorEmailBySysNo");
            //是Vendor 表编号, 不是VendorID
            command.SetParameterValue("@SysNo", vendorSysNo);
            command.SetParameterValue("@Email", newMailAddress);
            command.ExecuteNonQuery();
        }

        public BizEntity.PO.VendorHistoryLog CreateVendorHistoryLog(BizEntity.PO.VendorHistoryLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorHistoryLog");
            command.SetParameterValue("@Content", log.HistoryReason);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@Status", (int)ValidStatus.A);
            command.SetParameterValue("@VendorSysNo", log.VendorSysNo);
            command.SetParameterValue("@CompanyCode", log.CompanyCode);
            return command.ExecuteEntity<VendorHistoryLog>();
        }

        public List<BizEntity.PO.VendorModifyRequestInfo> LoadVendorModifyRequests(BizEntity.PO.VendorModifyRequestInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorModifyRequest");
            command.SetParameterValue("@VendorSysNo", info.VendorSysNo);
            command.SetParameterValue("@RequestType", (int)info.RequestType);
            return command.ExecuteEntityList<VendorModifyRequestInfo>();
        }

        public int DeleteVendorManufacturer(BizEntity.PO.VendorModifyRequestInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteVendorManufacturerBySysNo");
            command.SetParameterValue("@VendorManufacturerSysNo", info.VendorManufacturerSysNo);
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@Status", (int)info.Status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            return command.ExecuteNonQuery();
        }

        public void CancelVendorModifyRequest(BizEntity.PO.VendorModifyRequestInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CancelModifyRequest");
            command.SetParameterValue("@VendorSysNo", info.VendorSysNo);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@RequestType", (int)info.RequestType);
            command.SetParameterValue("@Status", (int)VendorModifyRequestStatus.CancelVerify);
            command.ExecuteNonQuery();
        }

        public BizEntity.PO.VendorModifyRequestInfo LoadVendorModifyRequest(int sysNo, BizEntity.PO.VendorModifyRequestStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorModifyRequestBySysNoAndStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", (int)status);
            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }

        public BizEntity.PO.VendorModifyRequestInfo ApproveVendorModifyRequest(BizEntity.PO.VendorModifyRequestInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ApproveVendorModifyRequest");

            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@Status", (int)VendorModifyRequestStatus.VerifyPass);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@Memo", info.Memo);

            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }

        public void CalcTotalPOMoney(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CalculateTotalPOMoney");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.ExecuteNonQuery();
        }

        public BizEntity.PO.VendorModifyRequestInfo DeclineWithdrawVendorModifyRequest(BizEntity.PO.VendorModifyRequestInfo info, BizEntity.PO.VendorModifyRequestStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeclineWithdrawVendorModifyRequest");
            DateTime auitDate = DateTime.Now;
            if (string.IsNullOrEmpty(info.Memo))
            {
                info.Memo = string.Empty;
            }
            else if (status == VendorModifyRequestStatus.VerifyUnPass)
            {
                info.Memo = info.Memo + "，" + auitDate;
            }
            command.SetParameterValue("@Status", (int)status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@AuditTime", auitDate);
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@Memo", info.Memo);

            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }


        /// <summary>
        /// 写入锁定原因
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="isHold"></param>
        /// <param name="holdUser"></param>
        /// <param name="holdReason"></param>
        /// <returns></returns>
        public bool UpdateHoldReasonVendorPM(int sysNo, bool isHold, int holdUser, string holdReason)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HoldUnholdPMVendor");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@HoldUser", holdUser);
            command.SetParameterValue("@HoldReason", holdReason);

            return command.ExecuteNonQuery() == 1;
        }

        public void UpdateVendorStatus(VendorApproveInfo vendorApproveInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorStatus");
            command.SetParameterValue("@SysNo", vendorApproveInfo.VendorSysNo);
            command.SetParameterValue("@Status", vendorApproveInfo.VendorStatus);
            command.ExecuteNonQuery();
        }

        public void CreateEmptyVendorCustomsInfo(int? merchantSysNo, string userName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateEmptyVendorCustomsInfo");
            command.SetParameterValue("@MerchantSysNo", merchantSysNo);
            command.SetParameterValue("@UserName", userName);
            command.ExecuteNonQuery();
        }

        public VendorCustomsInfo GetVendorCustomsInfo(int merchantSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorCustomsInfo");
            command.SetParameterValue("@MerchantSysNo", merchantSysNo);
            return command.ExecuteEntity<VendorCustomsInfo>();
        }

        public void CreateOrUpdateVendorCustomsInfo(VendorInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateOrUpdateVendorCustomsInfo");
            command.SetParameterValue("@MerchantSysNo", info.SysNo);
            command.SetParameterValue("@InspectionCode", info.VendorCustomsInfo.InspectionCode);
            command.SetParameterValue("@KJTCode", info.VendorCustomsInfo.KJTCode);
            command.SetParameterValue("@CBTMerchantCode", info.VendorCustomsInfo.CBTMerchantCode);
            command.SetParameterValue("@CBTMerchantName", info.VendorCustomsInfo.CBTMerchantName);
            command.SetParameterValue("@CBTSRC_NCode", info.VendorCustomsInfo.CBTSRC_NCode);
            command.SetParameterValue("@CBTREC_NCode", info.VendorCustomsInfo.CBTREC_NCode);
            command.SetParameterValue("@EasiPaySecretKey", info.VendorCustomsInfo.EasiPaySecretKey);
            command.SetParameterValue("@ReceiveCurrencyCode", info.VendorCustomsInfo.ReceiveCurrencyCode);
            command.SetParameterValue("@PayCurrencyCode", info.VendorCustomsInfo.PayCurrencyCode);
            command.SetParameterValue("@FTAppId", info.VendorCustomsInfo.FTAppId);
            command.SetParameterValue("@FTAppSecretKey", info.VendorCustomsInfo.FTAppSecretKey);
            command.SetParameterValue("@CBTSODeclareSecretKey", info.VendorCustomsInfo.CBTSODeclareSecretKey);
            command.SetParameterValue("@CBTProductDeclareSecretKey", info.VendorCustomsInfo.CBTProductDeclareSecretKey);
            command.SetParameterValue("@InspectionProductDeclareSecretKey", info.VendorCustomsInfo.InspectionProductDeclareSecretKey);
            command.SetParameterValue("@KJTAppId", info.VendorCustomsInfo.KJTAppId);
            command.SetParameterValue("@KJTAppSecretKey", info.VendorCustomsInfo.KJTAppSecretKey);
            command.ExecuteNonQuery();
        }

        public void HoldOrUnholdVendor(int userSysNo, int vendorSysNo, bool isHold, string holdReason)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HoldUnholdVendor");
            command.SetParameterValue("@SysNo", vendorSysNo);
            command.SetParameterValue("@HoldMark", isHold);
            command.SetParameterValue("@HoldUser", userSysNo);
            command.SetParameterValue("@HoldReason", holdReason);
            command.ExecuteNonQuery();
        }

        public BizEntity.PO.VendorInfo EditVendorInfo(BizEntity.PO.VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EditVendor");
            command.SetParameterValue("@SysNo", vendorInfo.SysNo);
            command.SetParameterValue("@VendorID", vendorInfo.VendorBasicInfo.VendorID);
            command.SetParameterValue("@VendorName", vendorInfo.VendorBasicInfo.VendorNameLocal);
            command.SetParameterValue("@EnglishName", vendorInfo.VendorBasicInfo.VendorNameGlobal);
            command.SetParameterValue("@BriefName", vendorInfo.VendorBasicInfo.VendorBriefName);
            command.SetParameterValue("@VendorType", (int)vendorInfo.VendorBasicInfo.VendorType);
            command.SetParameterValue("@District", vendorInfo.VendorBasicInfo.District);
            command.SetParameterValue("@Address", vendorInfo.VendorBasicInfo.Address);
            command.SetParameterValue("@Zip", vendorInfo.VendorBasicInfo.ZipCode);
            command.SetParameterValue("@Phone", vendorInfo.VendorBasicInfo.Phone);
            command.SetParameterValue("@Fax", vendorInfo.VendorBasicInfo.Fax);
            command.SetParameterValue("@Email", vendorInfo.VendorBasicInfo.EmailAddress);
            command.SetParameterValue("@Site", vendorInfo.VendorBasicInfo.Website);
            command.SetParameterValue("@Bank", vendorInfo.VendorFinanceInfo.BankName);
            command.SetParameterValue("@Account", vendorInfo.VendorFinanceInfo.AccountNumber);
            command.SetParameterValue("@TaxNo", vendorInfo.VendorFinanceInfo.TaxNumber);
            command.SetParameterValue("@Contact", vendorInfo.VendorBasicInfo.Contact);
            command.SetParameterValue("@Comment", vendorInfo.VendorBasicInfo.Comment);
            command.SetParameterValue("@Note", vendorInfo.VendorBasicInfo.Note);
            command.SetParameterValue("@Status", (int)vendorInfo.VendorBasicInfo.VendorStatus);
            command.SetParameterValue("@RMAPolicy", vendorInfo.VendorServiceInfo.RMAPolicy);
            command.SetParameterValue("@PayPeriod", vendorInfo.VendorFinanceInfo.PayPeriod);
            command.SetParameterValue("@RepairAddress", vendorInfo.VendorServiceInfo.Address);
            command.SetParameterValue("@RepairAreaSysNo", vendorInfo.VendorServiceInfo.AreaInfo.SysNo);
            command.SetParameterValue("@RepairContact", vendorInfo.VendorServiceInfo.Contact);
            command.SetParameterValue("@RepairContactPhone", vendorInfo.VendorServiceInfo.ContactPhone);
            command.SetParameterValue("@Cellphone", vendorInfo.VendorBasicInfo.CellPhone);
            command.SetParameterValue("@AcctContactName", vendorInfo.VendorFinanceInfo.AccountContact);
            command.SetParameterValue("@AcctPhone", vendorInfo.VendorFinanceInfo.AccountPhone);
            command.SetParameterValue("@RMAServiceArea", vendorInfo.VendorServiceInfo.RMAServiceArea);
            command.SetParameterValue("@IsConsign", (int)vendorInfo.VendorBasicInfo.ConsignFlag);
            command.SetParameterValue("@PayPeriodType", vendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo);
            command.SetParameterValue("@VendorContractInfo", vendorInfo.VendorBasicInfo.Contact);
            command.SetParameterValue("@IsCooperate", (int)vendorInfo.VendorBasicInfo.VendorIsCooperate);
            command.SetParameterValue("@AcctContactEmail", vendorInfo.VendorFinanceInfo.AccountContactEmail);
            command.SetParameterValue("@RepairPostcode", vendorInfo.VendorServiceInfo.ZipCode);
            command.SetParameterValue("@IsToLease", vendorInfo.VendorBasicInfo.VendorIsToLease);
            command.SetParameterValue("@DeductSysNo", vendorInfo.VendorDeductInfo.DeductSysNo);
            command.SetParameterValue("@CalcType", (int)vendorInfo.VendorDeductInfo.CalcType);
            command.SetParameterValue("@DeductPercent", vendorInfo.VendorDeductInfo.DeductPercent / 100.0m);
            command.SetParameterValue("@FixAmt", vendorInfo.VendorDeductInfo.FixAmt);
            command.SetParameterValue("@MaxAmt", vendorInfo.VendorDeductInfo.MaxAmt);
            command.SetParameterValue("@EPortSysNo", vendorInfo.VendorBasicInfo.EPortSysNo);
            command.ExecuteNonQuery();
            CreateOrEditIsShowStore(vendorInfo);
            return vendorInfo;
        }

        public BizEntity.PO.VendorInfo CreateOrUpdateVendorAttachInfo(BizEntity.PO.VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateOrUpdateVendorAttachment");

            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
            command.SetParameterValue("@VendorApplyForm", vendorInfo.VendorAttachInfo.VendorApplyForm);
            command.SetParameterValue("@EnterpriseBusinessLicence", vendorInfo.VendorAttachInfo.EnterpriseBusinessLicence);
            command.SetParameterValue("@OrganizeCodeCertificate", vendorInfo.VendorAttachInfo.OrganizeCodeCertificate);
            command.SetParameterValue("@TaxationAffairsRegistration", vendorInfo.VendorAttachInfo.TaxationAffairsRegistration);
            command.SetParameterValue("@AgreementBeingSold", vendorInfo.VendorAttachInfo.AgreementBeingSold);
            command.SetParameterValue("@AgreementConsign", vendorInfo.VendorAttachInfo.AgreementConsign);
            command.SetParameterValue("@AgreementAfterSold", vendorInfo.VendorAttachInfo.AgreementAfterSold);
            command.SetParameterValue("@AgreementOther", vendorInfo.VendorAttachInfo.AgreementOther);
            command.SetParameterValue("@CompanyCode", vendorInfo.CompanyCode);

            command.ExecuteEntity<VendorInfo>();
            return vendorInfo;
        }

        public BizEntity.PO.VendorInfo CreateOrUpdateVendorExtendInfo(BizEntity.PO.VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateOrUpdateVendorExInfo");

            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo);
            command.SetParameterValue("@StockType", vendorInfo.VendorBasicInfo.ExtendedInfo.StockType);
            command.SetParameterValue("@ShippingType", vendorInfo.VendorBasicInfo.ExtendedInfo.ShippingType.Value.ToString());
            command.SetParameterValue("@InvoiceType", vendorInfo.VendorBasicInfo.ExtendedInfo.InvoiceType.Value.ToString());
            command.SetParameterValue("@LogoPath", DBNull.Value);
            command.SetParameterValue("@MerchantRate", vendorInfo.VendorBasicInfo.ExtendedInfo.MerchantRate);
            command.SetParameterValue("@BriefInfo", vendorInfo.VendorBasicInfo.ExtendedInfo.BriefInfo);
            command.SetParameterValue("@Bulletin", vendorInfo.VendorBasicInfo.ExtendedInfo.Bulletin);
            command.SetParameterValue("@DefaultStock", vendorInfo.VendorBasicInfo.ExtendedInfo.DefaultStock);
            command.SetParameterValue("@SettlePeriodType", vendorInfo.VendorFinanceInfo.SettlePeriodType.HasValue ? (int)vendorInfo.VendorFinanceInfo.SettlePeriodType : (int?)null);
            if (vendorInfo.VendorFinanceInfo.IsAutoAudit.HasValue)
            {
                command.SetParameterValue("@AutoAudit", vendorInfo.VendorFinanceInfo.IsAutoAudit == true ? "Y" : "N");
            }
            else
            {
                command.SetParameterValue("@AutoAudit", DBNull.Value);
            }

            command.SetParameterValue("@CurrencyCode", "CNY");
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@CompanyCode", vendorInfo.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", vendorInfo.CompanyCode);

            command.ExecuteEntity<VendorInfo>();
            return vendorInfo;
        }

        public string GetManufacturerName(int manufacturersysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetManufacturerName");
            command.SetParameterValue("@ManufacturerSysNo", manufacturersysno);
            return command.ExecuteScalar<string>();
        }

        public BizEntity.PO.VendorAgentInfo CreateVMVendorCommissionInfo(int createUserSysNo, BizEntity.PO.VendorAgentInfo vendorAgentInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVMCommissionRule");

            string salesRuleStr = null;

            if (vendorAgentInfo.VendorCommissionInfo != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems != null
                && vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems.Count > 0)
            {
                salesRuleStr = SerializationUtility.XmlSerialize(vendorAgentInfo.VendorCommissionInfo.SaleRuleEntity);
            }

            command.SetParameterValue("@VendorManufacturerSysNo", vendorAgentInfo.AgentSysNo);
            command.SetParameterValue("@OrderCommissionFee", vendorAgentInfo.VendorCommissionInfo.OrderCommissionAmt);
            command.SetParameterValue("@DeliveryFee", vendorAgentInfo.VendorCommissionInfo.DeliveryFee);
            command.SetParameterValue("@RentFee", vendorAgentInfo.VendorCommissionInfo.RentFee);
            command.SetParameterValueAsCurrentUserAcct("@InUser");
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.SetParameterValue("@SalesRule", salesRuleStr);
            command.SetParameterValue("@CompanyCode", vendorAgentInfo.CompanyCode);

            command.ExecuteNonQuery();
            return vendorAgentInfo;
        }

        public BizEntity.PO.VendorAgentInfo GetVendorManufacturerBySysNo(int vendorManufacturerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorManufacturerBySysNo");
            command.SetParameterValue("@SysNo", vendorManufacturerSysNo);
            return command.ExecuteEntity<VendorAgentInfo>();
        }

        public int AbandonVMCommissionRule(int agentInfoSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AbandonVMCommissionRule");

            command.SetParameterValue("@VendorManufacturerSysNo", agentInfoSysNo);
            return command.ExecuteNonQuery();
        }

        public int UpdateVendorRank(BizEntity.PO.VendorModifyRequestInfo requestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorRank");
            command.SetParameterValue("@VendorSysNo", requestInfo.VendorSysNo);
            command.SetParameterValue("@Rank", requestInfo.Rank.Value.ToString());
            command.SetParameterValue("@SysNo", requestInfo.SysNo);
            command.SetParameterValue("@Status", requestInfo.Status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            return command.ExecuteNonQuery();
        }

        public int UpdateVendorManufacturer(BizEntity.PO.VendorModifyRequestInfo requestInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorManufacturer");
            command.SetParameterValue("@VendorManufacturerSysNo", requestInfo.VendorManufacturerSysNo);
            command.SetParameterValue("@C2SysNo", requestInfo.C2SysNo);
            command.SetParameterValue("@C3SysNo", requestInfo.C3SysNo);
            command.SetParameterValue("@ManufacturerSysNo", requestInfo.ManufacturerSysNo);
            command.SetParameterValue("@Level", requestInfo.AgentLevel);
            command.SetParameterValue("@SysNo", requestInfo.SysNo);
            command.SetParameterValue("@Status", requestInfo.Status);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.SetParameterValue("@SettleType", requestInfo.SettleType);
            command.SetParameterValue("@SettlePercentage", requestInfo.SettlePercentage);
            command.SetParameterValue("@SendPeriod", requestInfo.SendPeriod);
            command.SetParameterValue("@BuyWeekDay", requestInfo.BuyWeekDay);
            command.SetParameterValue("@BrandSysNo", requestInfo.BrandSysNo);
            return command.ExecuteNonQuery();
        }

        public int GetItemCountByVendorManufacturerSysNo(int vendorManufacturerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductCountByVendorManufacturerNumber");
            command.SetParameterValue("@VendorManufacturerSysNo", vendorManufacturerSysNo);
            return command.ExecuteScalar<int>();
        }

        public int LoadVendorPayPeriodType(int? vendorSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPayPeriodType");
            dataCommand.SetParameterValue("@VendorSysNo", vendorSysNo);
            try
            {
                return (int)dataCommand.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
        }

        public BizEntity.PO.VendorModifyRequestInfo GetApplyVendorFinanceModifyRequest(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetApplyVendorModifyRequest");
            command.SetParameterValue("@SysNo", vendorSysNo);
            return command.ExecuteEntity<VendorModifyRequestInfo>();
        }

        public BizEntity.PO.VendorAttachInfo LoadVendorAttachmentsInfo(BizEntity.PO.VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorAttachment");
            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo.Value);
            return command.ExecuteEntity<VendorAttachInfo>();
        }

        //ChrisHe
        public List<AttachmentForApplyFor> LoadAttachmentForApplyForInfo(VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetApplyForAttachment");
            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo.Value);
            return command.ExecuteEntityList<AttachmentForApplyFor>();
        }

        public List<ApplyInfo> LoadApplyInfo(VendorInfo vendorInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetApplyForInfo");
            command.SetParameterValue("@VendorSysNo", vendorInfo.SysNo.Value);
            return command.ExecuteEntityList<ApplyInfo>();
        }

        public string GetMaxSellerID(int vendorType, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetMaxSellerID");
            command.SetParameterValue("@VendorType", vendorType);
            command.SetParameterValue("@CompanyCode", companyCode);

            var result = command.ExecuteScalar();

            return result == DBNull.Value ? null : result.ToString();
        }

        #endregion IVendorDA Members

        private void CreateOrEditIsShowStore(VendorInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateOrEditIsShowStore");
            command.SetParameterValue("@IsShowStore", entity.VendorBasicInfo.ExtendedInfo.IsShowStore ? "Y" : "N");
            command.SetParameterValue("@VendorSysNo", entity.SysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@QQNumber", entity.VendorBasicInfo.ExtendedInfo.QQNumber);
            command.SetParameterValue("@IM_Enabled", entity.VendorBasicInfo.ExtendedInfo.IM_Enabled ? "Y" : "N");
            command.ExecuteNonQuery();
        }

        #region For Invoice

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByVendorSysNo(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByPOSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 判断应付款对应的商家是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorByVendorSettleSysNo");
            command.SetParameterValue("@VendorSettleSysNo", vendorSettleSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 判断应付款对应的商家是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorByCollectionSettlementSysNo");
            command.SetParameterValue("@CollectionSettlementSysNo", collectionSettlementSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorPMByPOSysNo(int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorPMByPOSysNo");
            command.SetParameterValue("@POSysNo", poSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        /// <summary>
        /// 判断应付款对应的商家PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsHolderVendorPMByVendorSettleSysNo");
            command.SetParameterValue("@VendorSettleSysNo", vendorSettleSysNo);

            return command.ExecuteScalar<int>() == 1;
        }

        #endregion For Invoice

        public List<KeyValuePair<int, string>> GetVendorNameListByVendorType(VendorType vendorType, string companyCode)
        {
            List<KeyValuePair<int, string>> returnList = new List<KeyValuePair<int, string>>();
            DataCommand command = DataCommandManager.GetDataCommand("GetVendor");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@VendorType", (int)vendorType);
            DataTable dt = command.ExecuteDataTable();
            if (null != dt && 0 < dt.Rows.Count)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    returnList.Add(new KeyValuePair<int, string>(Convert.ToInt32(dr["Key"].ToString()), dr["Value"].ToString()));
                }
            }
            return returnList;
        }

        public DataTable GetVendorMailInfo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorFinanceMailInfo");

            command.SetParameterValue("@SysNo", sysNo);
            DataTable dt = command.ExecuteDataTable();
            return dt;
        }

        public bool CheckVendorFiananceAccountAndConsignExists(string account, BizEntity.PO.VendorConsignFlag consignFlag, int? vendorSysNo, string companyCode)
        {
            List<VendorInfo> vendorList = new List<VendorInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("CheckVendorFiananceAccountAndConsignExists");
            command.SetParameterValue("@IsConsign", (int)consignFlag);
            command.SetParameterValue("@AccountNumber", account);
            if (vendorSysNo.HasValue)
            {
                command.SetParameterValue("@VendorSysNo", vendorSysNo);
            }
            else
            {
                command.SetParameterValue("@VendorSysNo", -999);

            }
            command.SetParameterValue("@CompanyCode", companyCode);
            vendorList = command.ExecuteEntityList<VendorInfo>();
            return vendorList.Count > 0 ? true : false;
        }

        public List<VendorHoldPMInfo> GetVendorPMHoldInfoByVendorSysNo(int vendorSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorPMHoldInfoByVendorSysNo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<VendorHoldPMInfo>();
        }

        public VendorHoldPMInfo CreateVendorPMHoldInfo(VendorHoldPMInfo info, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorPMHoldInfo");
            command.SetParameterValue("@VendorSysNo", info.VendorSysNo);
            command.SetParameterValue("@PMSysNo", info.PMSysNo);
            command.SetParameterValue("@HoldMark", 1);
            command.SetParameterValue("@InUser", info.InUser);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@StoreCompanyCode", companyCode);

            return command.ExecuteEntity<VendorHoldPMInfo>();
        }

        public int EditVendorPMHoldInfo(int vendorSysNo, int holdMark, string editUser, string pMSysNoIn, string companyCode)
        {
            //int sysNO = -1;
            DataCommand command = DataCommandManager.GetDataCommand("EditPMHoldInfo");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            //command.SetParameterValue("@PMSysNo", vendorPMHoldInfoEntity.PMSysNo);
            command.ReplaceParameterValue("#PMSysNoIn", pMSysNoIn);
            command.SetParameterValue("@HoldMark", holdMark);
            command.SetParameterValue("@EditUser", editUser);
            command.SetParameterValue("@CompanyCode", companyCode);
            //CommonHelper.SetCommonParams(command);
            return command.ExecuteNonQuery();
            //return sysNO;
        }

        /// <summary>
        /// 创建跨境供应商仓库
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int CreateVendorStock(StockInfoForKJ stockInfo)
        {
            int StockSysNo = NewStockSysNo();
            if (StockSysNo > 0)
            {
                DataCommand command = DataCommandManager.GetDataCommand("SaveStockForKJ");
                command.SetParameterValue("@SysNo", StockSysNo);
                command.SetParameterValue("@StockID", stockInfo.StockID);
                command.SetParameterValue("@StockName", stockInfo.StockName);
                command.SetParameterValue("@CompanyCode", stockInfo.CompanyCode);
                command.SetParameterValue("@LanguageCode", stockInfo.LanguageCode);
                command.SetParameterValue("@StoreCompanyCode", stockInfo.StoreCompanyCode);
                command.SetParameterValue("@CountryCode", stockInfo.CountryCode);
                command.SetParameterValue("@MerchantSysNo", stockInfo.MerchantSysNo);
                command.SetParameterValue("@Status", stockInfo.Status);
                return command.ExecuteNonQuery();
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 生成一个新的仓库编号
        /// </summary>
        /// <returns></returns>
        private static int NewStockSysNo()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("NewStockSysNo");
            return dc.ExecuteScalar<int>();

        }
    }
}
