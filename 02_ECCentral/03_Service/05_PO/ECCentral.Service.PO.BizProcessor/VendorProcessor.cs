using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.EventMessage.PO;
using ValidStatus = ECCentral.BizEntity.ExternalSYS.ValidStatus;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 供应商 - BizProcessor
    /// </summary>
    [VersionExport(typeof(VendorProcessor))]
    public class VendorProcessor
    {
        private static Dictionary<string, int> _dict = new Dictionary<string, int>();


        #region [Fields]
        private IVendorDA m_VendorDA;
        private IVendorPayTermsDA m_VendorPayTermsDA;
        private IVendorStoreDA m_VendorStoreDA = ObjectFactory<IVendorStoreDA>.Instance;
        private IVendorUserDA m_VendorUserDA = ObjectFactory<IVendorUserDA>.Instance;

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

        public IVendorPayTermsDA VendorPayTermsDA
        {
            get
            {
                if (null == m_VendorPayTermsDA)
                {
                    m_VendorPayTermsDA = ObjectFactory<IVendorPayTermsDA>.Instance;
                }
                return m_VendorPayTermsDA;
            }
        }

        public IVendorStoreDA VendorStoreDA
        {
            get
            {
                if (null == m_VendorStoreDA)
                {
                    m_VendorStoreDA = ObjectFactory<IVendorStoreDA>.Instance;
                }
                return m_VendorStoreDA;
            }
        }

        public IVendorUserDA VendorUserDA
        {
            get
            {
                if (null == m_VendorUserDA)
                {
                    m_VendorUserDA = ObjectFactory<IVendorUserDA>.Instance;
                }
                return m_VendorUserDA;
            }
        }
        #endregion

        /// <summary>
        /// 加载供应商基本信息:
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public VendorBasicInfo LoadVendorBasicInfo(int vendorSysNo)
        {
            VendorInfo getVendorInfo = new VendorInfo();
            getVendorInfo.VendorBasicInfo = new VendorBasicInfo();
            getVendorInfo.VendorServiceInfo = new VendorServiceInfo();

            //加载供应商基本信息,加载供应商售后信息, 加载供应商财务信息:
            getVendorInfo = VendorDA.LoadVendorInfo(vendorSysNo);
            //加载供应商扩展信息
            VendorExtendInfo getVendorExtInfo = VendorDA.LoadVendorExtendInfo(getVendorInfo);
            if (null != getVendorExtInfo)
            {
                getVendorInfo.VendorBasicInfo.ExtendedInfo = getVendorExtInfo;
            }
            return getVendorInfo.VendorBasicInfo;
        }

        /// <summary>
        /// 加载单个供应商信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public VendorInfo LoadVendorInfo(int vendorSysNo)
        {
            VendorInfo getVendorInfo = new VendorInfo();
            getVendorInfo.VendorBasicInfo = new VendorBasicInfo();
            getVendorInfo.VendorServiceInfo = new VendorServiceInfo();
            getVendorInfo.VendorDeductInfo = new VendorDeductInfo();
            //加载商家基本信息,加载商家售后信息, 加载商家财务信息:
            getVendorInfo = VendorDA.LoadVendorInfo(vendorSysNo);
            if (null != getVendorInfo)
            {
                //获取账期相关:
                if (null != getVendorInfo.VendorFinanceInfo.PayPeriodType && getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.HasValue)
                {
                    VendorPayTermsItemInfo getPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName = getPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.DiscribComputer = getPayTermsInfo.DiscribComputer;
                    }
                }
                getVendorInfo.VendorFinanceInfo.FinanceRequestInfo = VendorDA.GetApplyVendorFinanceModifyRequest(vendorSysNo);
                //获取账期相关:
                if (null != getVendorInfo.VendorFinanceInfo.FinanceRequestInfo && getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsNo.HasValue)
                {
                    VendorPayTermsItemInfo getRequestPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getRequestPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsName = getRequestPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.DiscribComputer = getRequestPayTermsInfo.DiscribComputer;
                    }
                }
                if (null != getVendorInfo.VendorFinanceInfo && getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.HasValue)
                {
                    VendorPayTermsItemInfo getPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName = getPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.DiscribComputer = getPayTermsInfo.DiscribComputer.Replace("</br>", Environment.NewLine);
                    }
                }

                //加载商家扩展信息
                VendorExtendInfo getVendorExtInfo = VendorDA.LoadVendorExtendInfo(getVendorInfo);
                if (null != getVendorExtInfo)
                {
                    getVendorInfo.VendorBasicInfo.ExtendedInfo = getVendorExtInfo;
                }
                //加载供应商历史信息
                List<VendorHistoryLog> getVendorHistory = VendorDA.LoadVendorHistoryLog(getVendorInfo);
                if (null != getVendorHistory)
                {
                    getVendorInfo.VendorHistoryLog = getVendorHistory;
                }

                //加载供应商代理信息:
                List<VendorAgentInfo> getVendorAgentInfo = GetVendorAgentInfo(getVendorInfo);
                if (null != getVendorAgentInfo)
                {
                    for (int i = 0; i < getVendorAgentInfo.Count; ++i)
                    {
                        //如果代理信息为空，则在modifyRequest 表中寻找相关的信息
                        if (string.IsNullOrEmpty(getVendorAgentInfo[i].AgentLevel) && string.IsNullOrEmpty(getVendorAgentInfo[i].C2Name) && string.IsNullOrEmpty(getVendorAgentInfo[i].C3Name) && string.IsNullOrEmpty(getVendorAgentInfo[i].ManufacturerInfo.ManufacturerNameGlobal))
                        {
                            List<VendorAgentInfo> getCheckVendorAgentList = VendorDA.GetCheckVendorManufacturerInfo(getVendorAgentInfo[i].AgentSysNo.Value);
                            if (null != getCheckVendorAgentList && 0 < getCheckVendorAgentList.Count)
                            {
                                getVendorAgentInfo[i].AgentLevel = getCheckVendorAgentList[0].AgentLevel;
                                getVendorAgentInfo[i].C2SysNo = getCheckVendorAgentList[0].C2SysNo;
                                getVendorAgentInfo[i].C3SysNo = getCheckVendorAgentList[0].C3SysNo;
                                getVendorAgentInfo[i].C2Name = getCheckVendorAgentList[0].C2Name;
                                getVendorAgentInfo[i].C3Name = getCheckVendorAgentList[0].C3Name;
                                getVendorAgentInfo[i].ManufacturerInfo.SysNo = getCheckVendorAgentList[0].ManufacturerInfo.SysNo;
                                getVendorAgentInfo[i].ManufacturerInfo.ManufacturerNameLocal = getCheckVendorAgentList[0].ManufacturerInfo.ManufacturerNameLocal;
                                getVendorAgentInfo[i].BrandInfo.SysNo = getCheckVendorAgentList[0].BrandInfo.SysNo;
                                getVendorAgentInfo[i].BrandInfo.BrandNameLocal = getCheckVendorAgentList[0].BrandInfo.BrandNameLocal;
                                getVendorAgentInfo[i].RowState = VendorRowState.Unchanged;
                            }
                        }
                    }

                    getVendorInfo.VendorAgentInfoList = getVendorAgentInfo;
                }
                //加载供应商附件信息:
                getVendorInfo.VendorAttachInfo = VendorDA.LoadVendorAttachmentsInfo(getVendorInfo);
                //加载锁定的PM：
                List<VendorHoldPMInfo> lockPMList = VendorDA.GetVendorPMHoldInfoByVendorSysNo(getVendorInfo.SysNo.Value, getVendorInfo.CompanyCode);
                getVendorInfo.VendorBasicInfo.HoldPMList = lockPMList.Where(x => x.IsChecked == true).ToList();
                getVendorInfo.VendorStoreInfoList = ObjectFactory<IVendorStoreDA>.Instance.GetVendorStoreInfoList(getVendorInfo.SysNo.Value);
                getVendorInfo.VendorCustomsInfo = VendorDA.GetVendorCustomsInfo(getVendorInfo.SysNo.Value);
            }
            else
            {
                //未找到供应商
                throw new BizException(GetMessageString("Vendor_VendorNotFound"));
            }
            return getVendorInfo;
        }
        /// <summary>
        /// 根据供应商编号查询对应的代理信息
        /// </summary>
        /// <param name="vendorInfo">供应商实体</param>
        /// <returns></returns>
        public List<VendorAgentInfo> GetVendorAgentInfo(VendorInfo vendorInfo)
        {
            return VendorDA.LoadVendorAgentInfoList(vendorInfo);
        }

        /// <summary>
        /// 加载供应商财务信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public virtual VendorFinanceInfo LoadVendorFinanceInfo(int vendorSysNo)
        {
            VendorInfo getVendorInfo = new VendorInfo();
            getVendorInfo = VendorDA.LoadVendorInfo(vendorSysNo);
            if (null != getVendorInfo)
            {
                //获取账期相关:
                if (null != getVendorInfo.VendorFinanceInfo.PayPeriodType)
                {
                    VendorPayTermsItemInfo getPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName = getPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.DiscribComputer = getPayTermsInfo.DiscribComputer;
                    }
                }
                getVendorInfo.VendorFinanceInfo.FinanceRequestInfo = VendorDA.GetApplyVendorFinanceModifyRequest(vendorSysNo);
                //获取账期相关:
                if (null != getVendorInfo.VendorFinanceInfo.FinanceRequestInfo && null != getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType != null && getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsNo.HasValue)
                {
                    VendorPayTermsItemInfo getRequestPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getRequestPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.PayTermsName = getRequestPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType.DiscribComputer = getRequestPayTermsInfo.DiscribComputer;
                    }
                }
                if (null != getVendorInfo.VendorFinanceInfo && null != getVendorInfo.VendorFinanceInfo.PayPeriodType != null && getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.HasValue)
                {
                    VendorPayTermsItemInfo getPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
                    if (null != getPayTermsInfo)
                    {
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName = getPayTermsInfo.PayTermsName;
                        getVendorInfo.VendorFinanceInfo.PayPeriodType.DiscribComputer = getPayTermsInfo.DiscribComputer.Replace("</br>", Environment.NewLine); ;
                    }
                }
            }
            return getVendorInfo.VendorFinanceInfo;
        }

        /// <summary>
        /// 加载供应商历史记录信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<VendorHistoryLog> LoadVendorHistoryLogInfo(VendorInfo info)
        {
            return VendorDA.LoadVendorHistoryLog(info);
        }

        /// <summary>
        /// 编辑供应商信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public VendorInfo EditVendor(VendorInfo info)
        {
            VendorInfo OldEntity = LoadVendorInfo(info.SysNo.Value);
            if (info == null)
            {
                throw new BizException("null entity");
            }

            //验证供应商实体:
            VerifyForMaintainVendor(info);
            //验证供应商代理信息:
            VerifyVendorManufacturer(info);

            //自动记录操作日志
            string basicMsg = string.Empty;
            string financeMsg = string.Empty;
            string afterSaleMsg = string.Empty;
            string manufacturerMsg = string.Empty;

            if (string.IsNullOrEmpty(OldEntity.VendorBasicInfo.BuyWeekDayVendor))
            {
                OldEntity.VendorBasicInfo.BuyWeekDayVendor = "";
            }
            //如何商家类型有改变，重新设置商家属性（经销、代销）
            if (OldEntity.VendorBasicInfo.ExtendedInfo.InvoiceType != info.VendorBasicInfo.ExtendedInfo.InvoiceType)
            {
                if (info.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG)
                {
                    // 经销: 平台经销供应商
                    info.VendorBasicInfo.ConsignFlag = VendorConsignFlag.Sell;
                }
                else
                {
                    // 代销: 平台入驻卖家，导购模式
                    info.VendorBasicInfo.ConsignFlag = VendorConsignFlag.Consign;
                }
            }

            CompareVendorInfo(OldEntity, info, out basicMsg, out financeMsg, out afterSaleMsg);

            #region 基本信息更改 （和原等级对比发生变化）

            if (!string.IsNullOrEmpty(basicMsg))
            {
                VendorModifyRequestInfo request = new VendorModifyRequestInfo
                {
                    Rank = info.VendorBasicInfo.VendorRank,
                    ActionType = VendorModifyActionType.Edit,
                    RequestType = VendorModifyRequestType.Vendor,
                    VendorSysNo = info.SysNo.Value,
                    Status = VendorModifyRequestStatus.Common,
                    Content = basicMsg
                };
                VendorModifyRequestInfo requestBuyWeekDay = new VendorModifyRequestInfo
                {
                    BuyWeekDay = info.VendorBasicInfo.BuyWeekDayVendor,
                    ActionType = VendorModifyActionType.Edit,
                    RequestType = VendorModifyRequestType.Vendor,
                    VendorSysNo = info.SysNo.Value,
                    Status = VendorModifyRequestStatus.Common,
                    Content = basicMsg
                };
                if (OldEntity != null)
                {
                    if (info.VendorBasicInfo.VendorRank != OldEntity.VendorBasicInfo.VendorRank)
                    {
                        request.Memo = GetMessageString("Vendor_RequestMemo_UpdateVendorLevel");
                        request.Status = VendorModifyRequestStatus.Apply;
                        request.CompanyCode = info.CompanyCode;
                        List<VendorModifyRequestInfo> list = VendorDA.LoadVendorModifyRequests(request);
                        if (list.Count == 0)
                        {
                            using (TransactionScope scope = new TransactionScope())
                            {
                                VendorDA.CreateModifyRequest(request);

                                //发送ESB消息
                                EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                                {
                                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                                    RequestSysNo = request.RequestSysNo.Value
                                });

                                scope.Complete();
                            }
                        }
                    }

                    #region 供应商下单日期更改

                    if (info.VendorBasicInfo.BuyWeekDayVendor != OldEntity.VendorBasicInfo.BuyWeekDayVendor)
                    {
                        requestBuyWeekDay.Memo = GetMessageString("Vendor_RequestMemo_UpdateVendorBuyWeekDay");
                        requestBuyWeekDay.RequestType = VendorModifyRequestType.BuyWeekDay;
                        requestBuyWeekDay.Status = VendorModifyRequestStatus.Apply;
                        requestBuyWeekDay.CompanyCode = info.CompanyCode;
                        List<VendorModifyRequestInfo> list = VendorDA.LoadVendorModifyRequests(requestBuyWeekDay);
                        if (list.Count == 0)
                        {                            
                            using (TransactionScope scope = new TransactionScope())
                            {
                                VendorDA.CreateModifyRequest(requestBuyWeekDay);
                                //发送ESB消息
                                EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                                {
                                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                                    RequestSysNo = requestBuyWeekDay.RequestSysNo.Value
                                });

                                scope.Complete();
                            }
                        }
                    }

                    #endregion 供应商下单日期更改
                }

                //仍然保存原等级
                info.VendorBasicInfo.VendorRank = OldEntity.VendorBasicInfo.VendorRank;
            }

            #endregion 基本信息更改 （和原等级对比发生变化）

            #region 财务信息更改

            if (!string.IsNullOrEmpty(financeMsg))
            {
                VendorModifyRequestInfo request = new VendorModifyRequestInfo
                {
                    ActionType = VendorModifyActionType.Edit,
                    RequestType = VendorModifyRequestType.Finance,
                    VendorSysNo = info.SysNo.Value,
                    Status = VendorModifyRequestStatus.Common,
                    Content = financeMsg,
                    CompanyCode = info.CompanyCode
                };
                using (TransactionScope scope = new TransactionScope())
                {
                    VendorDA.CreateModifyRequest(request);

                    //发送ESB消息
                    EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                    {
                        SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                        RequestSysNo = request.RequestSysNo.Value
                    });

                    scope.Complete();
                }
            }

            #endregion 财务信息更改

            #region 售后信息更改ModifyRequest

            if (!string.IsNullOrEmpty(afterSaleMsg))
            {
                VendorModifyRequestInfo request = new VendorModifyRequestInfo
                {
                    ActionType = VendorModifyActionType.Edit,
                    RequestType = VendorModifyRequestType.AfterSale,
                    VendorSysNo = info.SysNo.Value,
                    Status = VendorModifyRequestStatus.Common,
                    Content = afterSaleMsg
                };
                request.Memo = string.Empty;
                request.CompanyCode = info.CompanyCode;
                using (TransactionScope scope = new TransactionScope())
                {
                    VendorDA.CreateModifyRequest(request);

                    //发送ESB消息
                    EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                    {
                        SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                        RequestSysNo = request.RequestSysNo.Value
                    });

                    scope.Complete();
                }
            }

            #endregion 售后信息更改ModifyRequest

            #region 根据开票方式确定供应商类型

            //设置VendorType
            if (info.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG)
            {
                info.VendorBasicInfo.VendorType = VendorType.IPP;
            }
            else
            {
                info.VendorBasicInfo.VendorType = VendorType.VendorPortal;
            }

            #endregion 根据开票方式确定供应商类型

            VendorInfo editedVendor = null;

            using (TransactionScope scope = new TransactionScope())
            {
                editedVendor = VendorDA.EditVendorInfo(info);

                if (info.VendorAttachInfo != null)
                {
                    VendorDA.CreateOrUpdateVendorAttachInfo(info);
                }

                if (info.VendorBasicInfo.ExtendedInfo != null)
                {
                    VendorDA.CreateOrUpdateVendorExtendInfo(info);
                }

                #region 代理信息更改

                info.VendorAgentInfoList.ForEach(item =>
                {
                    item.CompanyCode = info.CompanyCode;
                    string editmanufacturerMsg = string.Empty;
                    //调用IM接口，获取C2和C3类别名称:
                    string categoryName = string.Empty;
                    if (item.C3SysNo.HasValue)
                    {
                        CategoryInfo c3Info = ExternalDomainBroker.GetCategory3Info(item.C3SysNo.Value);
                        if (null != c3Info)
                        {
                            categoryName = c3Info.CategoryName.Content;
                        }
                    }
                    else
                    {
                        if (item.C2SysNo.HasValue)
                        {
                            CategoryInfo c2Info = ExternalDomainBroker.GetCategory2Info(item.C2SysNo.Value);
                            if (null != c2Info)
                            {
                                categoryName = c2Info.CategoryName.Content;
                            }
                        }
                    }
                    string manufacturerName = VendorDA.GetManufacturerName(item.ManufacturerInfo.SysNo.Value);
                    if (item.RowState == VendorRowState.Added)
                    {
                        VendorAgentInfo createdManufacturer = VendorDA.CreateVendorManufacturerInfo(item, info.SysNo.Value);
                        item.AgentSysNo = createdManufacturer.AgentSysNo;

                        //佣金部分:
                        VendorDA.CreateVMVendorCommissionInfo(info.CreateUserSysNo.Value, item);

                        //新建时，写入空的代理信息，将sysno记入申请表
                        VendorModifyRequestInfo request = new VendorModifyRequestInfo
                        {
                            ManufacturerSysNo = item.ManufacturerInfo.SysNo,
                            AgentLevel = item.AgentLevel,
                            C2SysNo = item.C2SysNo,
                            C3SysNo = item.C3SysNo,
                            ActionType = VendorModifyActionType.Add,
                            RequestType = VendorModifyRequestType.Manufacturer,
                            VendorSysNo = info.SysNo.Value,
                            Status = VendorModifyRequestStatus.Apply,
                            VendorManufacturerSysNo = createdManufacturer.AgentSysNo,
                            SettlePercentage = item.SettlePercentage,
                            SettleType = item.SettleType,
                            BuyWeekDay = item.BuyWeekDay,
                            SendPeriod = item.SendPeriod,
                            BrandSysNo = item.BrandInfo.SysNo.Value
                        };
                        request.Content = string.Format(GetMessageString("Vendor_RequestContent"), manufacturerName, item.AgentLevel, categoryName);
                        request.CompanyCode = info.CompanyCode;
                        VendorDA.CreateModifyRequest(request);
                        //发送ESB消息
                        EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                        {
                            SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = request.RequestSysNo.Value
                        });
                    }
                    else if (item.RowState == VendorRowState.Deleted)
                    {
                        VendorAgentInfo olditem = VendorDA.GetVendorManufacturerBySysNo(item.AgentSysNo.Value);

                        int categorySysNo = olditem.C3SysNo.HasValue ? olditem.C3SysNo.Value : olditem.C2SysNo.Value;
                        CategoryInfo getCategoryInfo = ExternalDomainBroker.GetCategory3Info(categorySysNo);
                        string oldcategoryName = getCategoryInfo == null ? string.Empty : getCategoryInfo.CategoryName.Content;

                        string oldmanufacturerName = VendorDA.GetManufacturerName(olditem.ManufacturerInfo.SysNo.Value);
                        VendorModifyRequestInfo request = new VendorModifyRequestInfo
                        {
                            ManufacturerSysNo = olditem.ManufacturerInfo.SysNo.Value,
                            AgentLevel = olditem.AgentLevel,
                            C2SysNo = olditem.C2SysNo,
                            C3SysNo = olditem.C3SysNo,
                            ActionType = VendorModifyActionType.Delete,
                            RequestType = VendorModifyRequestType.Manufacturer,
                            VendorSysNo = info.SysNo.Value,
                            Status = (int)VendorModifyRequestStatus.Apply,
                            VendorManufacturerSysNo = item.AgentSysNo,
                            SettlePercentage = item.SettlePercentage,
                            SettleType = item.SettleType,
                            SendPeriod = olditem.SendPeriod,
                            BuyWeekDay = olditem.BuyWeekDay,
                            BrandSysNo = olditem.BrandInfo.SysNo
                        };
                        request.Content = string.Format(GetMessageString("Vendor_RequestContent"), oldmanufacturerName, olditem.AgentLevel, oldcategoryName);
                        request.CompanyCode = info.CompanyCode;

                        VendorDA.CreateModifyRequest(request);
                        VendorStoreDA.DeleteStoreBrandFiling(info.SysNo.Value, item.BrandInfo.SysNo.Value);
                       //发送ESB消息
                        EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                        {
                            SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = request.RequestSysNo.Value
                        });
                    }
                    else if (item.RowState == VendorRowState.Modified)
                    {
                        VendorAgentInfo oldVendorManufacturer = VendorDA.GetVendorManufacturerBySysNo(item.AgentSysNo.Value);
                        if (oldVendorManufacturer.AgentLevel != item.AgentLevel
                            || oldVendorManufacturer.C2SysNo != item.C2SysNo
                            || oldVendorManufacturer.C3SysNo != item.C3SysNo
                            || oldVendorManufacturer.ManufacturerInfo.SysNo != item.ManufacturerInfo.SysNo
                            || oldVendorManufacturer.BrandInfo.SysNo != item.BrandInfo.SysNo
                            || oldVendorManufacturer.SendPeriod != (item.SendPeriod.Length == 0 ? null : item.SendPeriod)
                            || oldVendorManufacturer.BuyWeekDay != (item.BuyWeekDay.Length == 0 ? null : item.BuyWeekDay)
                            || oldVendorManufacturer.SettleType != item.SettleType
                            || oldVendorManufacturer.SettlePercentage != item.SettlePercentage
                            )
                        {
                            VendorModifyRequestInfo request = new VendorModifyRequestInfo
                            {
                                ManufacturerSysNo = item.ManufacturerInfo.SysNo,
                                AgentLevel = item.AgentLevel,
                                C2SysNo = item.C2SysNo,
                                C3SysNo = item.C3SysNo,
                                ActionType = VendorModifyActionType.Edit,
                                RequestType = VendorModifyRequestType.Manufacturer,
                                VendorSysNo = info.SysNo.Value,
                                Status = VendorModifyRequestStatus.Apply,
                                VendorManufacturerSysNo = item.AgentSysNo.Value,
                                SettlePercentage = item.SettlePercentage,
                                SettleType = item.SettleType,
                                BuyWeekDay = item.BuyWeekDay,
                                SendPeriod = item.SendPeriod,
                                BrandSysNo = (!item.BrandInfo.SysNo.HasValue ? (int?)null : item.BrandInfo.SysNo.Value)
                            };

                            #region [获取资源]

                            string manufacturerTitle = GetMessageString("Vendor_Manufacturer_Msg");
                            string agentLevelTitle = GetMessageString("Vendor_AgentLevel_Msg");
                            string categoryTitle = GetMessageString("Vendor_Category_Msg");
                            string brandTitle = GetMessageString("Vendor_Brand_Msg");
                            string sendPeriodTitle = GetMessageString("Vendor_SendPeriod_Msg");
                            string buyWeekDayTitle = GetMessageString("Vendor_BuyweekDay_Msg");

                            #endregion [获取资源]

                            if (oldVendorManufacturer.ManufacturerInfo.SysNo != item.ManufacturerInfo.SysNo)
                            {
                                manufacturerMsg += manufacturerTitle + oldVendorManufacturer.ManufacturerInfo.ManufacturerNameLocal + "->" + manufacturerName;
                                editmanufacturerMsg += manufacturerTitle + oldVendorManufacturer.ManufacturerInfo.ManufacturerNameLocal + "->" + manufacturerName;
                            }
                            if (oldVendorManufacturer.AgentLevel != item.AgentLevel)
                            {
                                if (!string.IsNullOrEmpty(manufacturerMsg))
                                {
                                    manufacturerMsg += "，\r\n";
                                    editmanufacturerMsg += "#";
                                }
                                manufacturerMsg += agentLevelTitle + oldVendorManufacturer.AgentLevel + "->" + item.AgentLevel;
                                editmanufacturerMsg += agentLevelTitle + oldVendorManufacturer.AgentLevel + "->" + item.AgentLevel;
                            }
                            if (oldVendorManufacturer.C2SysNo != item.C2SysNo)
                            {
                                if (!string.IsNullOrEmpty(manufacturerMsg))
                                {
                                    manufacturerMsg += "，\r\n";
                                    editmanufacturerMsg += "#";
                                }
                                if (oldVendorManufacturer.C3SysNo != item.C3SysNo)
                                {
                                    manufacturerMsg += categoryTitle + oldVendorManufacturer.C2Name + "/" + oldVendorManufacturer.C3Name
                                       + " ->" + categoryName;

                                    editmanufacturerMsg += categoryTitle + oldVendorManufacturer.C2Name + "/" + oldVendorManufacturer.C3Name
                                      + " ->" + categoryName;
                                }
                                else
                                {
                                    manufacturerMsg += categoryTitle + oldVendorManufacturer.C2Name + " ->" + categoryName;
                                    editmanufacturerMsg += categoryTitle + oldVendorManufacturer.C2Name + " ->" + categoryName;
                                }
                            }
                            else
                            {
                                if (oldVendorManufacturer.C3SysNo != item.C3SysNo)
                                {
                                    if (!string.IsNullOrEmpty(manufacturerMsg))
                                    {
                                        manufacturerMsg += "，\r\n";
                                        editmanufacturerMsg += "#";
                                    }
                                    string cateMsg = string.Empty;
                                    if (!string.IsNullOrEmpty(oldVendorManufacturer.C3Name))
                                    {
                                        cateMsg = oldVendorManufacturer.C2Name + "/" + oldVendorManufacturer.C3Name;
                                    }
                                    else
                                    {
                                        cateMsg = oldVendorManufacturer.C2Name;
                                    }
                                    manufacturerMsg += categoryTitle + cateMsg + " ->" + categoryName;
                                    editmanufacturerMsg += categoryTitle + cateMsg + " ->" + categoryName;
                                }
                            }
                            if (oldVendorManufacturer.BrandInfo.SysNo != item.BrandInfo.SysNo)
                            {
                                manufacturerMsg += brandTitle + oldVendorManufacturer.BrandInfo.BrandNameLocal + "->" + item.BrandInfo.BrandNameLocal;
                                editmanufacturerMsg += "#" + brandTitle + oldVendorManufacturer.BrandInfo.BrandNameLocal + "->" + item.BrandInfo.BrandNameLocal;
                            }

                            if (!(string.IsNullOrEmpty(oldVendorManufacturer.SendPeriod) && string.IsNullOrEmpty(item.SendPeriod)) && oldVendorManufacturer.SendPeriod != item.SendPeriod)
                            {
                                if (!string.IsNullOrEmpty(manufacturerMsg))
                                {
                                    manufacturerMsg += "，\r\n";
                                }
                                manufacturerMsg += sendPeriodTitle + oldVendorManufacturer.SendPeriod + "->" + item.SendPeriod;
                                editmanufacturerMsg += "#" + sendPeriodTitle + oldVendorManufacturer.SendPeriod + "->" + item.SendPeriod;
                            }
                            if (!(string.IsNullOrEmpty(oldVendorManufacturer.BuyWeekDay) && string.IsNullOrEmpty(item.BuyWeekDay)) && oldVendorManufacturer.BuyWeekDay != item.BuyWeekDay)
                            {
                                if (!string.IsNullOrEmpty(manufacturerMsg))
                                {
                                    manufacturerMsg += "，\r\n";
                                }
                                manufacturerMsg += buyWeekDayTitle + oldVendorManufacturer.BuyWeekDay + "->" + item.BuyWeekDay;
                                editmanufacturerMsg += "#" + buyWeekDayTitle + oldVendorManufacturer.BuyWeekDay + "->" + item.BuyWeekDay;
                            }

                            if (!string.IsNullOrEmpty(manufacturerMsg))
                            {
                                manufacturerMsg += "。";
                            }

                            request.Content = editmanufacturerMsg;
                            request.CompanyCode = info.CompanyCode;
                            VendorDA.CreateModifyRequest(request);
                            //发送ESB消息
                            EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                            {
                                SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                                RequestSysNo = request.RequestSysNo.Value
                            });
                            manufacturerMsg += buyWeekDayTitle + oldVendorManufacturer.BuyWeekDay + "->" + item.BuyWeekDay;
                            editmanufacturerMsg += "#" + buyWeekDayTitle + oldVendorManufacturer.BuyWeekDay + "->" + item.BuyWeekDay;
                        }

                        if (!string.IsNullOrEmpty(manufacturerMsg))
                        {
                            manufacturerMsg += "。";
                        }

                        //佣金部分
                        VendorDA.AbandonVMCommissionRule(item.AgentSysNo.Value);
                        VendorDA.CreateVMVendorCommissionInfo(info.CreateUserSysNo.Value, item);
                    }
                    else
                    {
                        //editedVendor.VendorAgentInfoList.Add(item);
                    }
                });

                #endregion 代理信息更改

                #region 扩展信息修改

                VendorDA.CreateOrUpdateVendorExtendInfo(info);

                #endregion 扩展信息修改

                info.VendorStoreInfoList.ForEach(p =>
                {
                    if (p.SysNo.HasValue && p.SysNo.Value > 0)
                    {
                        ObjectFactory<VendorStoreProcessor>.Instance.Update(p);
                    }
                    else
                    {
                        p.VendorSysNo = info.SysNo;
                        var re = ObjectFactory<VendorStoreProcessor>.Instance.Create(p);
                        p.SysNo = re.SysNo;
                    }
                });

                // 关务对接信息更改
                VendorDA.CreateOrUpdateVendorCustomsInfo(info);

                scope.Complete();
            }
            return editedVendor;
        }

        /// <summary>
        /// 创建新供应商
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public VendorInfo CreateVendor(VendorInfo vendorInfo)
        {
            VendorInfo createdVendor = null;


            #region [验证业务模式]
            bool isModePassed = true;
            //if (vendorInfo.VendorBasicInfo.ExtendedInfo.StockType == VendorStockType.NEG && vendorInfo.VendorBasicInfo.ExtendedInfo.ShippingType == VendorShippingType.MET)
            //{
            //    isModePassed = false;
            //}
            //2012.11.20 支持业务模式4
            //if (vendorInfo.VendorBasicInfo.ExtendedInfo.StockType == VendorStockType.NEG && vendorInfo.VendorBasicInfo.ExtendedInfo.ShippingType == VendorShippingType.NEG && vendorInfo.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.MET)
            //{
            //    isModePassed = false;
            //}
            if (!isModePassed)
            {
                //系统不支持您当前选择的业务模式,请修改
                throw new BizException(GetMessageString("Vendor_CheckVendorModeInvalid"));
            }
            #endregion

            //Vendor新实体的Check操作:
            VerifyNewVendorEntity(vendorInfo);

            //生成SellerID:
            InitBannedList();
            vendorInfo.VendorBasicInfo.SellerID = GetSellerID(vendorInfo);
            //代销商是否自动结算
            string AutoAudit = null;

            //设置商家属性（经销、代销）
            if (vendorInfo.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG)
            {
                // 经销: 平台经销供应商
                vendorInfo.VendorBasicInfo.ConsignFlag = VendorConsignFlag.Sell;
            }
            else
            {
                // 代销: 平台入驻卖家，导购模式
                vendorInfo.VendorBasicInfo.ConsignFlag = VendorConsignFlag.Consign;
            }

            //代销商
            if (vendorInfo.VendorBasicInfo.ConsignFlag == VendorConsignFlag.Consign)
            {
                //结算类型为“2-	半月结，每月10/25日，且票到”
                if (vendorInfo.VendorFinanceInfo != null
                   && vendorInfo.VendorFinanceInfo.SettlePeriodType == VendorSettlePeriodType.PerMonth)
                {
                    AutoAudit = "Y";
                }
                else
                {
                    AutoAudit = "N";
                }
            }
            else
            {
                AutoAudit = null;
            }
            //VendorType字段:
            vendorInfo.VendorBasicInfo.VendorType = (vendorInfo.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG ? VendorType.IPP : VendorType.VendorPortal);

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                #region 等级取消界面编辑功能，直接默认为A，新建时商家状态为 待审核
                //如果等级不为C，等级默认为C，将真实等级写入申请表，待批:
                //if (vendorInfo.VendorBasicInfo.VendorRank != VendorRank.C)
                //{
                //    VendorModifyRequestInfo newModifyRequestInfo = new VendorModifyRequestInfo()
                //    {
                //        Rank = vendorInfo.VendorBasicInfo.VendorRank,
                //        ActionType = VendorModifyActionType.Add,
                //        RequestType = VendorModifyRequestType.Vendor
                //    };
                //    vendorInfo.VendorBasicInfo.VendorRank = VendorRank.C;
                //    createdVendor = VendorDA.CreateVendorInfo(vendorInfo);
                //    newModifyRequestInfo.VendorSysNo = createdVendor.SysNo.Value;
                //    newModifyRequestInfo.Status = VendorModifyRequestStatus.Apply;
                //    newModifyRequestInfo.Memo = GetMessageString("Vendor_RequestMemo_UpdateVendorLevel");
                //    //写代销结算类型
                //    newModifyRequestInfo.SettlePeriodType = vendorInfo.VendorFinanceInfo.SettlePeriodType;
                //    newModifyRequestInfo.AutoAudit = (AutoAudit != null ? (AutoAudit == "Y" ? true : false) : (bool?)null);
                //    newModifyRequestInfo.CompanyCode = vendorInfo.CompanyCode;
                //    //写申请表
                //    VendorDA.CreateModifyRequest(newModifyRequestInfo);
                //    //发送ESB消息
                //    EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                //    {
                //        SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                //        RequestSysNo = newModifyRequestInfo.RequestSysNo.Value
                //    });
                //}
                //else
                //{
                //    createdVendor = VendorDA.CreateVendorInfo(vendorInfo);
                //}
                #endregion
                vendorInfo.VendorBasicInfo.VendorRank = VendorRank.A;
                vendorInfo.VendorBasicInfo.VendorStatus = VendorStatus.WaitApprove;
                createdVendor = VendorDA.CreateVendorInfo(vendorInfo);

                vendorInfo.SysNo = createdVendor.SysNo;

                if (vendorInfo.VendorFinanceInfo != null)
                {
                    vendorInfo.SysNo = createdVendor.SysNo;
                    //CRL20146 By Kilin
                    vendorInfo.VendorFinanceInfo.IsAutoAudit = (AutoAudit != null ? (AutoAudit == "Y" ? true : false) : (bool?)null);//代销商是否自动结算

                    createdVendor = VendorDA.CreateOrUpdateVendorExtendInfo(vendorInfo);
                }

                //供应商下单日期更改请求:
                if (!string.IsNullOrEmpty(vendorInfo.VendorBasicInfo.BuyWeekDayVendor))
                {
                    VendorModifyRequestInfo buyWeekDayVendorRequest = new VendorModifyRequestInfo
                    {
                        BuyWeekDay = vendorInfo.VendorBasicInfo.BuyWeekDayVendor,
                        ActionType = VendorModifyActionType.Add,
                        RequestType = VendorModifyRequestType.BuyWeekDay,
                        CompanyCode = vendorInfo.CompanyCode
                    };
                    buyWeekDayVendorRequest.VendorSysNo = createdVendor.SysNo.Value;
                    buyWeekDayVendorRequest.Status = VendorModifyRequestStatus.Apply;
                    buyWeekDayVendorRequest.Memo = GetMessageString("Vendor_RequestMemo_UpdateVendorBuyWeekDay");
                    //写代销结算类型
                    buyWeekDayVendorRequest.SettlePeriodType = vendorInfo.VendorFinanceInfo.SettlePeriodType;
                    buyWeekDayVendorRequest.AutoAudit = AutoAudit == "Y" ? true : false;
                    //写申请表
                    VendorDA.CreateModifyRequest(buyWeekDayVendorRequest);
                    //发送ESB消息
                    EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                    {
                        SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                        RequestSysNo = buyWeekDayVendorRequest.RequestSysNo.Value
                    });
                }

                //供应商代理信息:(所有新增代理信息均需审核,不直接新增)
                vendorInfo.VendorAgentInfoList.ForEach(delegate(VendorAgentInfo agentInfo)
                {
                    agentInfo.CompanyCode = vendorInfo.CompanyCode;
                    VendorAgentInfo createdVendorAgentInfo = VendorDA.CreateVendorManufacturerInfo(agentInfo, createdVendor.SysNo.Value);
                    agentInfo.AgentSysNo = createdVendorAgentInfo.AgentSysNo;
                    VendorDA.CreateVendorCommissionInfo(agentInfo, createdVendor.SysNo.Value, vendorInfo.CreateUserName);

                    VendorModifyRequestInfo requestVendorAgentInfo = new VendorModifyRequestInfo
                    {
                        ActionType = VendorModifyActionType.Add,
                        RequestType = VendorModifyRequestType.Manufacturer,
                        VendorSysNo = createdVendor.SysNo.Value,
                        ManufacturerSysNo = agentInfo.ManufacturerInfo.SysNo.Value,
                        VendorManufacturerSysNo = agentInfo.AgentSysNo.Value,
                        AgentLevel = agentInfo.AgentLevel,
                        C2SysNo = agentInfo.C2SysNo,
                        C3SysNo = agentInfo.C3SysNo,
                        Status = VendorModifyRequestStatus.Apply,
                        SettlePercentage = agentInfo.SettlePercentage,
                        SettleType = agentInfo.SettleType,
                        BuyWeekDay = agentInfo.BuyWeekDay,
                        SendPeriod = agentInfo.SendPeriod,
                        BrandSysNo = agentInfo.BrandInfo.SysNo,
                        CompanyCode = vendorInfo.CompanyCode,
                        //CRL20146 By Kilin
                        //写代销结算类型
                        SettlePeriodType = vendorInfo.VendorFinanceInfo.SettlePeriodType,
                        AutoAudit = (AutoAudit != null ? (AutoAudit == "Y" ? true : false) : (bool?)null)
                    };
                    //写申请表
                    VendorDA.CreateModifyRequest(requestVendorAgentInfo);
                    //发送ESB消息
                    EventPublisher.Publish<VendorRankRequestSubmitMessage>(new VendorRankRequestSubmitMessage()
                    {
                        SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                        RequestSysNo = requestVendorAgentInfo.RequestSysNo.Value
                    });
                });

                vendorInfo.VendorStoreInfoList.ForEach(p =>
                {
                    if (p.SysNo.HasValue && p.SysNo.Value > 0)
                    {
                        ObjectFactory<VendorStoreProcessor>.Instance.Update(p);
                    }
                    else
                    {
                        p.VendorSysNo = createdVendor.SysNo;
                        var re = ObjectFactory<VendorStoreProcessor>.Instance.Create(p);
                        p.SysNo = re.SysNo;
                    }
                });

                VendorDA.CreateEmptyVendorCustomsInfo(createdVendor.SysNo, vendorInfo.CreateUserName);

                ts.Complete();
            }

            if (createdVendor.VendorBasicInfo.EPortSysNo.HasValue && createdVendor.VendorBasicInfo.EPortSysNo>0)
            {
                StockInfoForKJ stockInfo = new StockInfoForKJ();
                stockInfo.StockName = createdVendor.VendorBasicInfo.VendorNameLocal + "仓库";
                stockInfo.StockID = createdVendor.VendorBasicInfo.VendorNameLocal + "仓库ID";
                stockInfo.CountryCode ="AO";
                stockInfo.CompanyCode = createdVendor.CompanyCode;
                stockInfo.LanguageCode = "zh-CN";
                stockInfo.StoreCompanyCode = createdVendor.CompanyCode;
                stockInfo.MerchantSysNo = createdVendor.SysNo.Value;
                stockInfo.Status = 0;
                VendorDA.CreateVendorStock(stockInfo);
            }

            return createdVendor;
        }

        /// <summary>
        /// 创建供应商手动操作日志
        /// </summary>
        /// <param name="historyLog"></param>
        /// <returns></returns>
        public VendorHistoryLog CreateVendorHistoryLog(VendorHistoryLog historyLog)
        {
            if (historyLog == null)
            {
                return null;
            }
            return VendorDA.CreateVendorHistoryLog(historyLog);
        }

        /// <summary>
        /// 检查供应商实体完整性
        /// </summary>
        /// <param name="vendorInfo"></param>
        public void VerifyNewVendorEntity(VendorInfo vendorInfo)
        {
            #region [验证信息完整新]

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.VendorNameLocal))
            //{
            //    // 供应商全称不能为空
            //    throw new BizException(GetMessageString("Vendor_NameEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.VendorBriefName))
            //{
            //    // 供应商简称不能为空
            //    throw new BizException(GetMessageString("Vendor_BriefNameEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.Address))
            //{
            //    // 供应商地址不能为空
            //    throw new BizException(GetMessageString("Vendor_AddressEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.Contact))
            //{
            //    // 供应商联系人不能为空
            //    throw new BizException(GetMessageString("Vendor_ContactEmpty"));
            //}

            ////供应商区域限制:
            //List<WarehouseInfo> warehouseList = ExternalDomainBroker.GetWarehouseList(vendorInfo.CompanyCode);
            ////调用Inventory接口，获取WarehouseList:
            //bool exists = warehouseList.Exists(w =>
            // {
            //     return w.WarehouseName.IndexOf(vendorInfo.VendorBasicInfo.District) > -1;
            // });
            //if (!exists)
            //{
            //    string warehouseString = String.Join(",", warehouseList.Where(x => x.SysNo < 90).Select(w => w.WarehouseName.Substring(0, 2)).ToArray());
            //    // 供应商区域必须为{0}
            //    throw new BizException(string.Format(GetMessageString("Vendor_AreaInvalid"), warehouseString));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.Fax))
            //{
            //    // 供应商传真不能为空
            //    throw new BizException(GetMessageString("Vendor_FaxEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.Phone))
            //{
            //    // 供应商电话不能为空
            //    throw new BizException(GetMessageString("Vendor_PhoneEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.CellPhone))
            //{
            //    // 供应商手机不能为空
            //    throw new BizException(GetMessageString("Vendor_CellPhoneEmpty"));
            //}

            if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.EmailAddress))
            {
                // 业务联系电子邮箱不能为空
                throw new BizException(GetMessageString("Vendor_EmailEmpty"));
            }

            //if (vendorInfo.VendorAgentInfoList.Count == 0)
            //{
            //    // 代理品牌不能为空
            //    throw new BizException(GetMessageString("Vendor_BrandEmpty"));
            //}

            //foreach (var manufacturer in vendorInfo.VendorAgentInfoList)
            //{
            //    if ((manufacturer.C3SysNo == null || manufacturer.C3SysNo.Value == 0) && (manufacturer.C2SysNo == null || manufacturer.C2SysNo.Value == 0))
            //    {
            //        // 产品至少2级类别不能为空
            //        throw new BizException(GetMessageString("Vendor_C2SysNoEmpty"));
            //    }
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorFinanceInfo.BankName))
            //{
            //    // 供应商开户行不能为空
            //    throw new BizException(GetMessageString("Vendor_BankEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorFinanceInfo.AccountContact))
            //{
            //    // 供应商财务联系人不能为空
            //    throw new BizException(GetMessageString("Vendor_FinanceContactEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorFinanceInfo.AccountPhone))
            //{
            //    // 供应商财务联系电话不能为空
            //    throw new BizException(GetMessageString("Vendor_FinancePhoneEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorFinanceInfo.AccountContactEmail))
            //{
            //    // 供应商财务联系邮箱不能为空
            //    throw new BizException(GetMessageString("Vendor_FinanceEmailEmpty"));
            //}

            //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorBasicInfo.VendorNameLocal))
            //{
            //    // 供应商账号不能为空
            //    throw new BizException(GetMessageString("Vendor_AccountEmpty"));
            //}
            //非团购才验证
            if (vendorInfo.VendorBasicInfo.ConsignFlag != VendorConsignFlag.GroupBuying)
            {

                //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorServiceInfo.Contact))
                //{
                //    // 供应商售后联系人不能为空
                //    throw new BizException(GetMessageString("Vendor_ServiceContactEmpty"));
                //}

                //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorServiceInfo.ContactPhone))
                //{
                //    // 供应商售后联系人电话不能为空
                //    throw new BizException(GetMessageString("Vendor_ServiceContactPhoneEmpty"));
                //}

                //if (vendorInfo.VendorServiceInfo.AreaInfo == null || !vendorInfo.VendorServiceInfo.AreaInfo.SysNo.HasValue || vendorInfo.VendorServiceInfo.AreaInfo.SysNo.Value == 0)
                //{
                //    // 供应商售后服务地区不能为空
                //    throw new BizException(GetMessageString("Vendor_ServiceAreaEmpty"));
                //}

                //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorServiceInfo.Address))
                //{
                //    // 供应商售后地址不能为空
                //    throw new BizException(GetMessageString("Vendor_ServiceAddressEmpty"));
                //}

                //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorServiceInfo.RMAServiceArea))
                //{
                //    // 供应商售后服务范围不能为空
                //    throw new BizException(GetMessageString("Vendor_ServiceRegionEmpty"));
                //}

                //if (StringUtility.IsNullOrEmpty(vendorInfo.VendorServiceInfo.RMAPolicy))
                //{
                //    // 供应商售后退货策略不能为空
                //    throw new BizException(GetMessageString("Vendor_RefundStrategyEmpty"));
                //}
            }

            //if (!vendorInfo.VendorFinanceInfo.PayPeriod.HasValue)
            //{
            //    // 请选择账期
            //    throw new BizException(GetMessageString("Vendor_SelectPayPeriod"));
            //}

            #endregion [验证信息完整新]

            #region [检查系统是否存在VendorID,VendorName相同的供应商]

            VendorInfo existVendorNameEntity = VendorDA.LoadVendorInfoByVendorName(vendorInfo.VendorBasicInfo.VendorNameLocal);
            if (existVendorNameEntity != null && existVendorNameEntity.VendorBasicInfo.VendorStatus.HasValue && existVendorNameEntity.VendorBasicInfo.VendorStatus.Value == VendorStatus.Available)
            {
                // 系统已存在一个状态为可用的{0}
                throw new BizException(string.Format(GetMessageString("Vendor_VendorNameExisted"), vendorInfo.VendorBasicInfo.VendorNameLocal));
            }

            #endregion [检查系统是否存在VendorID,VendorName相同的供应商]

            if (VendorDA.CheckVendorFiananceAccountAndConsignExists(vendorInfo.VendorFinanceInfo.AccountNumber, vendorInfo.VendorBasicInfo.ConsignFlag.Value, null, vendorInfo.CompanyCode))
            {
                throw new BizException(GetMessageString("Vendor_CheckVendorFiananceAccountAndConsignExists"));
            }
        }

        /// <summary>
        /// 更新供应商EMail地址:(超长不存 改为记录日志)
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public VendorInfo UpdateVendorEmail(VendorInfo vendorInfo)
        {
            VendorInfo editedVendor = new VendorInfo();
            VendorInfo OldEntity = VendorDA.LoadVendorInfo(vendorInfo.SysNo.Value);
            //超长不存 改为记录日志:
            if (OldEntity.VendorBasicInfo.EmailAddress.Length + vendorInfo.VendorBasicInfo.EmailAddress.Length + 1 > 200)
            {
                editedVendor = OldEntity;
                //写LOG： CommonService.WriteLog<VendorEntity>(editedVendor, "Update Vendor Email : " + Email, editedVendor.SysNo.ToString(), (int)LogType.VendorSettle_Update);
                ExternalDomainBroker.CreateLog("Update Vendor Email : "
           , BizEntity.Common.BizLogType.POC_VendorSettle_Master_Update
           , editedVendor.SysNo.Value
           , editedVendor.CompanyCode);
            }
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    VendorDA.UpdateVendorEmailAddress(vendorInfo.SysNo.Value, vendorInfo.VendorBasicInfo.EmailAddress);
                    scope.Complete();
                }
            }
            return editedVendor;
        }

        public void ApproveVendor(VendorApproveInfo vendorApproveInfo)
        {
            var vendorBasicInfo = LoadVendorBasicInfo(vendorApproveInfo.VendorSysNo);

            if (vendorBasicInfo.VendorStatus != VendorStatus.WaitApprove)
            {
                throw new BizException(GetMessageString("Vendor_CannotVendorApprove"));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                // 更新状态为[已审核]
                vendorApproveInfo.VendorStatus = VendorStatus.Available;
                VendorDA.UpdateVendorStatus(vendorApproveInfo);

                // 创建商家店铺基本信息
                this.CreateStoreInfo(vendorApproveInfo);

                // 创建商家的店铺网页
                this.CreateDefaultStorePagesInfo(vendorApproveInfo, vendorBasicInfo);

                // 创建商家账号
                this.CreateDefaultVendorUser(vendorApproveInfo, vendorBasicInfo);

                ts.Complete();
            }
        }

        /// <summary>
        /// 创建商家店铺基本信息
        /// </summary>
        private void CreateStoreInfo(VendorApproveInfo vendorApproveInfo)
        {
            // 生成商家商检号（其它信息可以不做处理，让商家第一次必须自己登录去设置）
            string storeInspectionNo = NewStoreInspectionNo(vendorApproveInfo.VendorSysNo);
            VendorStoreDA.WriteStoreInspectionNo(vendorApproveInfo.VendorSysNo, storeInspectionNo, vendorApproveInfo.UserName);
        }

        /// <summary>
        /// 创建商家的默认店铺网页
        /// </summary>
        private void CreateDefaultStorePagesInfo(VendorApproveInfo vendorApproveInfo, VendorBasicInfo vendorBasicInfo)
        {
            VendorPresetContent vendorPresetContent = VendorStoreDA.GetVendorPresetContent();

            var storePageHeaderExists        = VendorStoreDA.GetStorePageHeaderBySeller(vendorApproveInfo.VendorSysNo);
            var storePageInfoExists          = VendorStoreDA.GetStorePageInfoListBySeller(vendorApproveInfo.VendorSysNo);
            var publishedStorePageInfoExists = VendorStoreDA.GetPublishedStorePageInfoListBySeller(vendorApproveInfo.VendorSysNo);
            
            if (storePageHeaderExists != null
                || (storePageInfoExists != null && storePageInfoExists.Count > 0)
                || (publishedStorePageInfoExists != null && publishedStorePageInfoExists.Count > 0))
            {
                return;
            }

            // StorePageHeader
            var storePageHeader = new StorePageHeader();
            storePageHeader.SellerSysNo   = vendorApproveInfo.VendorSysNo;
            storePageHeader.HeaderContent = vendorPresetContent.StorePageHeader.HeaderContent;
            VendorStoreDA.CreateStorePageHeader(storePageHeader);

            // StorePageInfo & PublishedStorePageInfo
            foreach (VendorPresetContent.StorePageInfoElement presetStorePageInfo in vendorPresetContent.StorePageInfos)
            {
                var storePageInfo = new StorePageInfo();
                storePageInfo.SellerSysNo = vendorApproveInfo.VendorSysNo;
                storePageInfo.PageTypeKey = presetStorePageInfo.PageTypeKey;
                storePageInfo.PageName    = presetStorePageInfo.PageName;
                storePageInfo.DataValue   = presetStorePageInfo.GetDataValueForStorePageInfo(vendorApproveInfo.VendorSysNo.ToString());
                storePageInfo.LinkUrl     = presetStorePageInfo.GetPreviewLinkUrlForStorePageInfo(vendorApproveInfo.VendorSysNo.ToString()); //非正式发布版可使用预览路由URL
                storePageInfo.Status      = 1;

                int storePageSysNo = VendorStoreDA.CreateStorePageInfo(storePageInfo);

                var publishedStorePageInfo = new PublishedStorePageInfo();
                SimpleEntityCopy.CopyProperties(storePageInfo, publishedStorePageInfo);
                publishedStorePageInfo.LinkUrl = presetStorePageInfo.GetLinkUrlForStorePageInfo(vendorApproveInfo.VendorSysNo.ToString());//非正式发布版可使用正式版本路由URL
                publishedStorePageInfo.StorePageSysNo = storePageSysNo;
                VendorStoreDA.CreatePublishedStorePageInfo(publishedStorePageInfo);
            }
        }

        /// <summary>
        /// 创建商家的默认账号。如果已经存在，将不做，并返回Null。
        /// </summary>
        private VendorUser CreateDefaultVendorUser(VendorApproveInfo vendorApproveInfo, VendorBasicInfo vendorBasicInfo)
        {
            var isCreated = VendorUserDA.IsCreatedVendorUser(vendorApproveInfo.VendorSysNo);
            if (isCreated)
                return null;

            #region 创建 VenderUser
            var vendorUser = new VendorUser();
            vendorUser.VendorSysNo  = vendorApproveInfo.VendorSysNo;
            vendorUser.UserNum      = 1;
            vendorUser.UserID       = vendorApproveInfo.VendorSysNo + "admin";
            vendorUser.UserName     = vendorBasicInfo.VendorBriefName + "管理员";
            vendorUser.Email        = vendorBasicInfo.EmailAddress;
            vendorUser.Status       = ValidStatus.Active;
            vendorUser.InUser       = vendorApproveInfo.UserName;
            vendorUser.EditUser     = vendorApproveInfo.UserName;
            vendorUser.CompanyCode  = vendorApproveInfo.CompanyCode;
            vendorUser.APIStatus    = ValidStatus.DeActive;
            vendorUser.Pwd          = Hash_MD5.GetMD5(AppSettingManager.GetSetting("ExternalSYS", ("VendorUserInitPassword")));

            vendorUser = VendorUserDA.InsertVendorUser(vendorUser);

            List<VendorAgentInfo> vendorAgentInfoList = VendorDA.LoadVendorAgentInfoList(new VendorInfo() { SysNo = vendorApproveInfo.VendorSysNo });
            if (vendorAgentInfoList != null && vendorAgentInfoList.Count > 0)
            {
                foreach (VendorAgentInfo vendorAgentInfo in vendorAgentInfoList)
                {
                    VendorUserDA.InsertVendorUserVendorEx(new VendorUserMapping()
                    {
                        UserSysNo = vendorUser.SysNo,
                        ManufacturerSysNo = vendorAgentInfo.ManufacturerInfo.SysNo,
                        IsAuto = 1,
                        VendorSysNo = vendorUser.VendorSysNo.Value
                    });
                }
            }
            #endregion

            #region 创建角色 VendorUser_Role [RoleName, VendorSysNo]
            Role role = new Role();
            role.RoleName       = vendorBasicInfo.VendorBriefName + "管理员";
            role.VendorSysNo    = vendorApproveInfo.VendorSysNo;
            role.Status         = "A";
            role.InUser         = vendorApproveInfo.UserName;
            role.EditUser       = vendorApproveInfo.UserName;
            role.CompanyCode    = vendorApproveInfo.CompanyCode;
            role = VendorUserDA.CreateRole(role);
            #endregion

            #region 给角色添加权限 VendorUser_Role_Privilege [RoleSysNo, PrivilegeSysNo]
            var privilegeList = VendorUserDA.GetPrivilegeList();
            foreach (var privilege in privilegeList)
            {
                var rolePrivilege = new RolePrivilege();
                rolePrivilege.RoleSysNo         = role.SysNo.Value;
                rolePrivilege.PrivilegeSysNo    = privilege.SysNo;
                VendorUserDA.CreateRolePrivilege(rolePrivilege);
            }
            #endregion

            #region 将用户与角色绑定 VendorUser_User_Role [UserSysNo, RoleSysNo]
            var userRole = new VendorUserRole();
            userRole.UserSysNo = vendorUser.SysNo;
            userRole.RoleSysNo = role.SysNo;
            VendorUserDA.InsertVendorUser_User_Role(userRole);
            #endregion
            
            return vendorUser;
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// 生成账户密钥
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            int seed, asc, num;
            for (int i = 0; i < size; i++)
            {
                seed = random.Next(1, 4);
                switch (seed)
                {
                    case 1: asc = 65; num = 26; break;
                    case 2: asc = 97; num = 26; break;
                    case 3: asc = 48; num = 10; break;
                    default: asc = 65; num = 26; break;
                }
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(num * random.NextDouble() + asc)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 供应商代理信息审核通过
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public VendorModifyRequestInfo PassVendorManufacturer(VendorModifyRequestInfo requestInfo)
        {
            List<VendorModifyRequestInfo> list = VendorDA.LoadVendorModifyRequests(requestInfo);
            using (TransactionScope ts = new TransactionScope())
            {
                list.ForEach(item =>
                {                    
                    item.Status = VendorModifyRequestStatus.VerifyPass;
                    if (item.RequestType == VendorModifyRequestType.Vendor)
                    {
                        if (item.ActionType == VendorModifyActionType.Add
                            || item.ActionType == VendorModifyActionType.Edit)
                        {
                            //供应商新增和更改都是更改主表的等级:
                            VendorDA.UpdateVendorRank(item);
                        }
                    }
                    else if (item.RequestType == VendorModifyRequestType.Manufacturer)
                    {
                        if (item.ActionType == VendorModifyActionType.Add || item.ActionType == VendorModifyActionType.Edit)
                        {
                            VendorDA.UpdateVendorManufacturer(item);
                            StoreBrandFilingInfo brandFiling = new StoreBrandFilingInfo();
                            brandFiling.SellerSysNo = item.VendorSysNo;
                            brandFiling.BrandSysNo = item.BrandSysNo;
                            brandFiling.InspectionNo = NewBrandInspectionNo(item.VendorSysNo); // 生成品牌商检编号
                            brandFiling.AgentLevel = item.AgentLevel;
                            brandFiling.Staus = StoreBrandFilingStatus.Draft;
                            brandFiling.InUserSysNo = requestInfo.CreateUserSysNo;
                            brandFiling.InUserName = ServiceContext.Current.UserDisplayName;
                            VendorStoreDA.InsertStoreBrandFiling(brandFiling);
                        }
                        else if (item.ActionType == VendorModifyActionType.Delete)
                        {
                            if (VendorDA.GetItemCountByVendorManufacturerSysNo(item.VendorManufacturerSysNo.Value) > 0)
                            {
                                throw new BizException(GetMessageString("Vendor_CannotDeleteProduct"));
                            }

                            //佣金部分
                            VendorDA.AbandonVMCommissionRule(item.SysNo.Value);
                            VendorDA.DeleteVendorManufacturer(item);
                        }
                    }
                    else if (item.RequestType == VendorModifyRequestType.BuyWeekDay)
                    {
                        if (item.ActionType == VendorModifyActionType.Add
                            || item.ActionType == VendorModifyActionType.Edit)
                        {
                            //供应商下单日期审核通过。
                            VendorDA.UpdateVendorBuyWeekDay(item);
                        }
                    }
                });

                //发送ESB消息
                EventPublisher.Publish<VendorRankRequestAuditMessage>(new VendorRankRequestAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = requestInfo.VendorSysNo
                });

                ts.Complete();
            }
            return requestInfo;
        }

        private static readonly object lockerNewBrandInspectionNo = new object();
        /// <summary>
        /// 生成商家的品牌商检编号，规则为五位的流水号，左边补0
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        private static string NewBrandInspectionNo(int sellerSysNo)
        {
            lock (lockerNewBrandInspectionNo)
            {
                int newBrandInspectionSeed = ObjectFactory<IVendorStoreDA>.Instance.IncrementStoreBrandInspectionSeed(sellerSysNo);
                return newBrandInspectionSeed.ToString().PadLeft(5, '0');
            }
        }

        private static readonly object lockerNewStoreInspectionNo = new object();
        /// <summary>
        /// 生成商家商检编号，规则为 “3100”+“6位流水号”
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        private static string NewStoreInspectionNo(int sellerSysNo)
        {
            lock (lockerNewStoreInspectionNo)
            {
                int newBrandInspectionSeed = ObjectFactory<IVendorStoreDA>.Instance.IncrementStoreInspectionSeed(sellerSysNo);
                return "3100" + newBrandInspectionSeed.ToString().PadLeft(6, '0');
            }
        }

        /// <summary>
        /// 撤销申请 - 供应商代理信息
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public VendorModifyRequestInfo CancelVendorManufacturer(VendorModifyRequestInfo requestInfo)
        {
            List<VendorModifyRequestInfo> list = VendorDA.LoadVendorModifyRequests(requestInfo);
            using (TransactionScope ts = new TransactionScope())
            {
                list.ForEach(item =>
                {
                    item.Status = VendorModifyRequestStatus.CancelVerify;
                    if (item.RequestType == VendorModifyRequestType.Manufacturer)
                    {
                        if (item.ActionType == VendorModifyActionType.Add)
                        {
                            VendorDA.DeleteVendorManufacturer(item);
                        }
                    }
                });
                VendorModifyRequestInfo request = new VendorModifyRequestInfo
                {
                    VendorSysNo = requestInfo.VendorSysNo,
                    Status = VendorModifyRequestStatus.CancelVerify
                };

                VendorDA.CancelVendorModifyRequest(requestInfo);

                //发送ESB消息
                EventPublisher.Publish<VendorRankRequestCancelMessage>(new VendorRankRequestCancelMessage()
                {
                    CancelUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = requestInfo.VendorSysNo
                });

                ts.Complete();
            }

            return requestInfo;
        }

        /// <summary>
        /// 审核通过 - 供应商财务信息
        /// </summary>
        /// <param name="financeInfo"></param>
        /// <returns></returns>
        public VendorModifyRequestInfo ApproveVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            #region [Check 逻辑]

            if (info == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (info.SysNo <= 0)
            {
                throw new ArgumentException("entity.SysNo must be a positive");
            }

            VendorModifyRequestInfo requestInDB = VendorDA.LoadVendorModifyRequest(info.SysNo.Value, VendorModifyRequestStatus.Apply);

            if (requestInDB == null)
            {
                // 不存在的编号为{0}的待审核申请
                throw new BizException(string.Format(GetMessageString("Vendor_WaitingRequestNotExist"), info.SysNo));
            }

            //if (requestInDB.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    throw new BizException("创建人和审核人不能相同");
            //}

            #endregion [Check 逻辑]

            VendorModifyRequestInfo result = null;
            using (TransactionScope scope = new TransactionScope())
            {
                result = VendorDA.ApproveVendorModifyRequest(info);

                //根据新的合作日期，重新计算累计金额
                VendorDA.CalcTotalPOMoney(result.SysNo.Value);
                //TODO: 发送审核通过邮件:SendMail(result.SysNo, "审核通过");

                //发送ESB消息
                EventPublisher.Publish<VendorFinanceInfoRequestAuditMessage>(new VendorFinanceInfoRequestAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = result.SysNo ?? 0
                });

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// 提交审核 - 供应商财务信息
        /// </summary>
        /// <returns></returns>
        public VendorModifyRequestInfo RequestForApprovalVendorFinanceInfo(VendorModifyRequestInfo requestInfo)
        {
            if (requestInfo == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (requestInfo.ValidDate == null && requestInfo.ExpiredDate != null
                || requestInfo.ValidDate != null && requestInfo.ExpiredDate == null)
            {
                // 合作期限时间必须填写完整!
                throw new BizException(GetMessageString("Vendor_CooperateTimeEmpty"));
            }

            if (requestInfo.ContractAmt == null)
            {
                if (requestInfo.ValidDate == null && requestInfo.ExpiredDate == null)
                {
                    // 合作期限及合作金额至少填写一个!
                    throw new BizException(GetMessageString("Vendor_CooperateTimeAndAmtEmpty"));
                }
            }
            string strMsg = string.Empty;
            CompareFinanceRequest(requestInfo, out strMsg);
            requestInfo.Content = strMsg;
            requestInfo.ActionType = VendorModifyActionType.Edit;
            requestInfo.RequestType = VendorModifyRequestType.Finance;

            VendorModifyRequestInfo result = null;
            using (TransactionScope scope = new TransactionScope())
            {
                result = VendorDA.CreateVendorModifyRequest(requestInfo);

                //发送ESB消息
                EventPublisher.Publish<VendorFinanceInfoRequestSubmitMessage>(new VendorFinanceInfoRequestSubmitMessage()
                {
                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = result.SysNo ?? 0
                });

                scope.Complete();
            }
            //TODO：发送请求审核邮件:SendMail(result.SysNo, "请求审核");

            return result;
        }

        /// <summary>
        /// 审核拒绝 -  供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public VendorModifyRequestInfo DeclineVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            #region [Check逻辑]

            if (info == null)
            {
                throw new BizException("null entity");
            }
            if (info.SysNo <= 0)
            {
                throw new BizException("entity.SysNo must be a positive");
            }
            if (string.IsNullOrEmpty(info.Memo))
            {
                //审核不通过时，审核理由不能为空"
                throw new BizException(GetMessageString("Vendor_AuditMemoEmpty"));
            }
            VendorModifyRequestInfo requestInDB = VendorDA.LoadVendorModifyRequest(info.SysNo.Value, VendorModifyRequestStatus.Apply);
            if (requestInDB == null)
            {
                // 不存在的编号为{0}的待审核申请
                throw new BizException(string.Format(GetMessageString("Vendor_WaitingRequestNotExist"), info.SysNo));
            }

            #endregion [Check逻辑]

            VendorModifyRequestInfo result = null;
            using (TransactionScope scope = new TransactionScope())
            {
                result = VendorDA.DeclineWithdrawVendorModifyRequest(info, VendorModifyRequestStatus.VerifyUnPass);
                //TODO;发邮件:SendMail(result.SysNo, "审核拒绝");

                //发送ESB消息
                EventPublisher.Publish<VendorFinanceInfoRequestRejectMessage>(new VendorFinanceInfoRequestRejectMessage()
                {
                    RejectUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = result.SysNo ?? 0
                });

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// 取消审核 - 供应商财务信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public VendorModifyRequestInfo WithDrawVendorFinanceInfo(VendorModifyRequestInfo info)
        {
            #region [Check 逻辑]

            if (info == null)
            {
                throw new BizException("null entity");
            }

            if (info.SysNo <= 0)
            {
                throw new BizException("entity.SysNo must be a positive");
            }

            #endregion [Check 逻辑]

            VendorModifyRequestInfo requestInDB = VendorDA.LoadVendorModifyRequest(info.SysNo.Value, VendorModifyRequestStatus.Apply);

            if (requestInDB == null)
            {
                // 不存在的编号为{0}的待审核申请
                throw new BizException(string.Format(GetMessageString("Vendor_WaitingRequestNotExist"), info.SysNo));
            }
            VendorModifyRequestInfo result = null;
            using (TransactionScope scope = new TransactionScope())
            {
                result = VendorDA.DeclineWithdrawVendorModifyRequest(info, VendorModifyRequestStatus.CancelVerify);
                //TODO:发邮件:SendMail(result.SysNo, "取消审核");

                //发送ESB消息
                EventPublisher.Publish<VendorFinanceInfoRequestCancelMessage>(new VendorFinanceInfoRequestCancelMessage()
                {
                    CancelUserSysNo = ServiceContext.Current.UserSysNo,
                    RequestSysNo = result.SysNo ?? 0
                });

                scope.Complete();
            }

            return result;
        }

        /// <summary>
        /// 锁定供应商
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int HoldVendor(VendorInfo vendorInfo)
        {
            VendorDA.HoldOrUnholdVendor(vendorInfo.CreateUserSysNo.Value, vendorInfo.SysNo.Value, true, vendorInfo.VendorBasicInfo.HoldReason);

            //调用Invoice接口,锁定供应商关联的付款单状态
            int processNum = ExternalDomainBroker.LockOrUnlockPayItemByVendor(vendorInfo.SysNo.Value, true);

            return processNum;
        }

        /// <summary>
        /// 解锁供应商
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int UnHoldVendor(VendorInfo vendorInfo)
        {
            VendorDA.HoldOrUnholdVendor(vendorInfo.CreateUserSysNo.Value, vendorInfo.SysNo.Value, false, vendorInfo.VendorBasicInfo.HoldReason);

            //调用Invoice接口取消锁定供应商关联的付款单状态
            int processNum = ExternalDomainBroker.LockOrUnlockPayItemByVendor(vendorInfo.SysNo.Value, false);

            return processNum;
        }

        /// <summary>
        /// 锁定、解锁供应商PM：
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="holdUserSysNo"></param>
        /// <param name="reason"></param>
        /// <param name="holdSysNoList"></param>
        /// <param name="unHoldSysNoList"></param>
        public List<int> HoldOrUnholdVendorPM(int vendorSysNo, int holdUserSysNo, string reason, List<int> holdSysNoList, List<int> unHoldSysNoList, string companyCode)
        {
            //写入锁定原因：
            //VendorDA.HoldOrUnholdVendor(holdUserSysNo, vendorSysNo, true, reason);

            VendorDA.UpdateHoldReasonVendorPM(vendorSysNo, false, holdUserSysNo, reason);

            List<VendorHoldPMInfo> listVendorPMHoldInfo = VendorDA.GetVendorPMHoldInfoByVendorSysNo(vendorSysNo, companyCode);

            List<int> pmSysNoList = listVendorPMHoldInfo.Select(p => p.PMSysNo.Value).ToList();
            if (holdSysNoList == null)
            {
                holdSysNoList = new List<int>();
            }
            //新建的pmHold
            List<int> pmHoldCreateList = holdSysNoList.Except(pmSysNoList).ToList();
            //修改的pmHold
            List<int> pmHoldUpdateList = holdSysNoList;
            //修改的pmUnhold
            List<int> pmUnHoldUpdateList = unHoldSysNoList;

            if (pmHoldCreateList.Count > 0)
            {
                foreach (var pmSysNo in pmHoldCreateList)
                {
                    VendorHoldPMInfo entity = new VendorHoldPMInfo()
                    {
                        VendorSysNo = vendorSysNo,
                        InUser = holdUserSysNo.ToString(),
                        PMSysNo = pmSysNo
                    };
                    VendorDA.CreateVendorPMHoldInfo(entity, companyCode);
                }
            }


            if (pmHoldUpdateList.Count > 0)
            {
                StringBuilder sbHold = new StringBuilder();
                sbHold.Append(" (");
                foreach (int sysNo in pmHoldUpdateList)
                {
                    sbHold.AppendFormat("{0},", sysNo);
                }
                string strHodeReplace = sbHold.ToString();
                if (strHodeReplace.EndsWith(","))
                {
                    strHodeReplace = strHodeReplace.Remove(strHodeReplace.Length - 1);
                }
                strHodeReplace += " )";

                VendorDA.EditVendorPMHoldInfo(vendorSysNo, 1, holdUserSysNo.ToString(), strHodeReplace, companyCode);
            }

            if (pmUnHoldUpdateList.Count > 0)
            {
                StringBuilder sbUnHold = new StringBuilder();
                sbUnHold.Append(" (");
                foreach (int sysNo in pmUnHoldUpdateList)
                {
                    sbUnHold.AppendFormat("{0},", sysNo);
                }
                string strUnholdReplace = sbUnHold.ToString();
                if (strUnholdReplace.EndsWith(","))
                {
                    strUnholdReplace = strUnholdReplace.Remove(strUnholdReplace.Length - 1);
                }
                strUnholdReplace += " )";

                VendorDA.EditVendorPMHoldInfo(vendorSysNo, 0, holdUserSysNo.ToString(), strUnholdReplace, companyCode);
            }

            //调用财务接口锁定供应商关联的付款单状态
            //return InvoiceMgmtService.LockOrUnLockPayItemByVendorPM(vendorSysNo, pmHoldSysNoList, pmUnHoldSysNoList);
            return ExternalDomainBroker.LockOrUnlockPayItemByVendorPM(vendorSysNo, true, holdSysNoList, unHoldSysNoList);

        }

        #region 【验证方法】

        private void VerifyForMaintainVendor(VendorInfo entity)
        {
            if (entity.SysNo == 1)
            {
                //此供应商信息为只读,不能修改
                throw new BizException(GetMessageString("Vendor_VendorReadOnly"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.VendorNameGlobal) && StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.VendorNameLocal))
            {
                // 供应商全称不能为空
                throw new BizException(GetMessageString("Vendor_NameEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.VendorBriefName))
            {
                // 供应商简称不能为空
                throw new BizException(GetMessageString("Vendor_BriefNameEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.Address))
            {
                // 供应商地址不能为空
                throw new BizException(GetMessageString("Vendor_AddressEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.Contact))
            {
                // 供应商联系人不能为空
                throw new BizException(GetMessageString("Vendor_ContactEmpty"));
            }
            //调用Inventory接口，获取所有的仓库信息:
            List<WarehouseInfo> warehouseList = ExternalDomainBroker.GetWarehouseList(entity.CompanyCode);
            //供应商区域: 检查供应商的District是否存在于WarehostList中:
            //bool exists = warehouseList.Exists(w =>
            //{
            //    return w.WarehouseName.IndexOf(entity.VendorBasicInfo.District) > -1;
            //});
            //if (!exists)
            //{
            //    string warehouseString = String.Join(",", warehouseList.Where(x => x.SysNo < 90).Select(w => w.WarehouseName.Substring(0, 2)).ToArray());
            //    //供应商区域必须为{0}
            //    throw new BizException(string.Format(GetMessageString("Vendor_AreaInvalid"), warehouseString));
            //}

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.Fax))
            {
                // 供应商传真不能为空
                throw new BizException(GetMessageString("Vendor_FaxEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.Phone))
            {
                // 供应商电话不能为空
                throw new BizException(GetMessageString("Vendor_PhoneEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.CellPhone))
            {
                // 供应商手机不能为空
                throw new BizException(GetMessageString("Vendor_CellPhoneEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorBasicInfo.EmailAddress))
            {
                // 业务联系电子邮箱不能为空
                throw new BizException(GetMessageString("Vendor_EmailEmpty"));
            }

            //if (entity.VendorAgentInfoList.Count == 0)
            //{
            //    // 代理品牌不能为空
            //    throw new BizException(GetMessageString("Vendor_BrandEmpty"));
            //}

            foreach (var manufacturer in entity.VendorAgentInfoList)
            {
                if (manufacturer.C3SysNo == null && manufacturer.C2SysNo == null)
                {
                    // 产品至少2级类别不能为空
                    throw new BizException(GetMessageString("Vendor_C2SysNoEmpty"));
                }
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorFinanceInfo.BankName))
            {
                // 供应商开户行不能为空
                throw new BizException(GetMessageString("Vendor_BankEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorFinanceInfo.AccountContact))
            {
                // 供应商财务联系人不能为空
                throw new BizException(GetMessageString("Vendor_FinanceContactEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorFinanceInfo.AccountPhone))
            {
                // 供应商财务联系电话不能为空
                throw new BizException(GetMessageString("Vendor_FinancePhoneEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorFinanceInfo.AccountContactEmail))
            {
                // 供应商财务联系邮箱不能为空
                throw new BizException(GetMessageString("Vendor_FinanceEmailEmpty"));
            }

            if (StringUtility.IsNullOrEmpty(entity.VendorFinanceInfo.AccountNumber))
            {
                // 供应商账号不能为空
                throw new BizException(GetMessageString("Vendor_AccountEmpty"));
            }
            //非团购才验证
            if (entity.VendorBasicInfo.ConsignFlag != VendorConsignFlag.GroupBuying)
            {
                if (StringUtility.IsNullOrEmpty(entity.VendorServiceInfo.Contact))
                {
                    // 供应商售后联系人不能为空
                    throw new BizException(GetMessageString("Vendor_ServiceContactEmpty"));
                }

                if (StringUtility.IsNullOrEmpty(entity.VendorServiceInfo.ContactPhone))
                {
                    // 供应商售后联系人电话不能为空
                    throw new BizException(GetMessageString("Vendor_ServiceContactPhoneEmpty"));
                }

                if (entity.VendorServiceInfo.AreaInfo == null || entity.VendorServiceInfo.AreaInfo.SysNo == null || !entity.VendorServiceInfo.AreaInfo.SysNo.HasValue)
                {
                    // 供应商售后服务地区不能为空
                    throw new BizException(GetMessageString("Vendor_ServiceAreaEmpty"));
                }

                if (StringUtility.IsNullOrEmpty(entity.VendorServiceInfo.Address))
                {
                    // 供应商售后地址不能为空
                    throw new BizException(GetMessageString("Vendor_ServiceAddressEmpty"));
                }

                if (StringUtility.IsNullOrEmpty(entity.VendorServiceInfo.RMAServiceArea))
                {
                    // 供应商售后服务范围不能为空
                    throw new BizException(GetMessageString("Vendor_ServiceRegionEmpty"));
                }

                if (StringUtility.IsNullOrEmpty(entity.VendorServiceInfo.RMAPolicy))
                {
                    // 供应商售后退货策略不能为空
                    throw new BizException(GetMessageString("Vendor_RefundStrategyEmpty"));
                }
            }
            if (VendorDA.CheckVendorFiananceAccountAndConsignExists(entity.VendorFinanceInfo.AccountNumber, entity.VendorBasicInfo.ConsignFlag.Value, entity.SysNo.Value, entity.CompanyCode))
            {
                throw new BizException(GetMessageString("Vendor_CheckVendorFiananceAccountAndConsignExists"));
            }
        }

        private void VerifyVendorManufacturer(VendorInfo entity)
        {
            var query = from vmItem in entity.VendorAgentInfoList
                        where vmItem.ManufacturerInfo.SysNo != 0 && vmItem.C2SysNo != 0
                        group vmItem by new
                        {
                            vmItem.ManufacturerInfo.SysNo,
                            vmItem.BrandInfo.BrandNameLocal,
                            vmItem.C2SysNo,
                            vmItem.C3SysNo,
                            vmItem.AgentLevel
                        }
                            into g
                            where g.Count() > 1
                            select g;

            if (query.Count() > 0)
            {
                //存在重复的代理信息,请检查：代理厂商、代理品牌 、产品类别 、代理级别
                throw new BizException(GetMessageString("Vendor_SameVendorAgentInfo"));
            }
        }

        /// <summary>
        /// 自动记录操作日志 比较所有信息
        /// </summary>
        /// <param name="oldEntity"></param>
        /// <param name="entity"></param>
        /// <param name="basicMsg"></param>
        /// <param name="financeMsg"></param>
        /// <param name="afterSaleMsg"></param>
        public void CompareVendorInfo(VendorInfo oldEntity, VendorInfo entity, out string basicMsg, out string financeMsg, out string afterSaleMsg)
        {
            basicMsg = string.Empty;
            financeMsg = string.Empty;
            afterSaleMsg = string.Empty;
            string comma = "，";
            if (oldEntity != null)
            {
                #region 基本信息

                if (string.Compare(entity.VendorBasicInfo.VendorID, oldEntity.VendorBasicInfo.VendorID) != 0)
                {
                    basicMsg += GetMessageString("Vendor_VendorID_BasicMsg") + oldEntity.VendorBasicInfo.VendorID + "->" + entity.VendorBasicInfo.VendorID;
                }
                if (string.Compare(entity.VendorBasicInfo.VendorNameLocal, oldEntity.VendorBasicInfo.VendorNameLocal) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorName_BasicMsg") + oldEntity.VendorBasicInfo.VendorNameLocal + "->" + entity.VendorBasicInfo.VendorNameLocal;
                }
                if (entity.VendorBasicInfo.VendorNameGlobal != oldEntity.VendorBasicInfo.VendorNameGlobal)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorNameEng_BasicMsg") + oldEntity.VendorBasicInfo.VendorNameGlobal + "->" + entity.VendorBasicInfo.VendorNameGlobal;
                }
                if (string.Compare(entity.VendorBasicInfo.VendorBriefName, oldEntity.VendorBasicInfo.VendorBriefName) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorBriefName_BasicMsg") + oldEntity.VendorBasicInfo.VendorBriefName + "->" + entity.VendorBasicInfo.VendorBriefName;
                }
                if (string.Compare(entity.VendorBasicInfo.District, oldEntity.VendorBasicInfo.District) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorRegion_BasicMsg") + oldEntity.VendorBasicInfo.District + "->" + entity.VendorBasicInfo.District;
                }
                if (string.Compare(entity.VendorBasicInfo.ZipCode, oldEntity.VendorBasicInfo.ZipCode) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorZip_BasicMsg") + oldEntity.VendorBasicInfo.ZipCode + "->" + entity.VendorBasicInfo.ZipCode;
                }
                if (string.Compare(entity.VendorBasicInfo.Address, oldEntity.VendorBasicInfo.Address) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorAddress_BasicMsg") + oldEntity.VendorBasicInfo.Address + "->" + entity.VendorBasicInfo.Address;
                }
                if (string.Compare(entity.VendorBasicInfo.Contact, oldEntity.VendorBasicInfo.Contact) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_VendorContact_BasicMsg") + oldEntity.VendorBasicInfo.Contact + "->" + entity.VendorBasicInfo.Contact;
                }
                if (string.Compare(entity.VendorBasicInfo.Fax, oldEntity.VendorBasicInfo.Fax) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_Fax_BasicMsg") + oldEntity.VendorBasicInfo.Fax + "->" + entity.VendorBasicInfo.Fax;
                }
                if (string.Compare(entity.VendorBasicInfo.Phone, oldEntity.VendorBasicInfo.Phone) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_Phone_BasicMsg") + oldEntity.VendorBasicInfo.Phone + "->" + entity.VendorBasicInfo.Phone;
                }
                if (string.Compare(entity.VendorBasicInfo.CellPhone, oldEntity.VendorBasicInfo.CellPhone) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_CellPhone_BasicMsg") + oldEntity.VendorBasicInfo.CellPhone + "->" + entity.VendorBasicInfo.CellPhone;
                }
                if (string.Compare(entity.VendorBasicInfo.Website, oldEntity.VendorBasicInfo.Website) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_Site_BasicMsg") + oldEntity.VendorBasicInfo.Website + "->" + entity.VendorBasicInfo.Website;
                }
                if (string.Compare(entity.VendorBasicInfo.EmailAddress, oldEntity.VendorBasicInfo.EmailAddress) != 0)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_Email_BasicMsg") + oldEntity.VendorBasicInfo.EmailAddress + "->" + entity.VendorBasicInfo.EmailAddress;
                }
                if (entity.VendorBasicInfo.VendorStatus != oldEntity.VendorBasicInfo.VendorStatus)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }

                    string availableString = GetMessageString("Vendor_Available_BasicMsg");
                    string unavailableString = GetMessageString("Vendor_UnAvailable_BasicMsg");

                    string oldStatus = (int)oldEntity.VendorBasicInfo.VendorStatus == 0 ? availableString : unavailableString;
                    string newStatus = (int)entity.VendorBasicInfo.VendorStatus == 0 ? availableString : unavailableString;
                    basicMsg += GetMessageString("Vendor_Status_BasicMsg") + oldStatus + "->" + newStatus;
                }
                if (entity.VendorBasicInfo.VendorIsCooperate != oldEntity.VendorBasicInfo.VendorIsCooperate)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }

                    string cooperateYesString = GetMessageString("Vendor_IsCooperateYes_BasicMsg");
                    string cooperateNoString = GetMessageString("Vendor_IsCooperateNo_BasicMsg");

                    string oldIsCooperate = (int)oldEntity.VendorBasicInfo.VendorIsCooperate == 1 ? cooperateYesString : cooperateNoString;
                    string newIsCooperate = (int)entity.VendorBasicInfo.VendorIsCooperate == 1 ? cooperateYesString : cooperateNoString;
                    basicMsg += GetMessageString("Vendor_IsCooperate_BasicMsg") + oldIsCooperate + "->" + newIsCooperate;
                }
                if (entity.VendorBasicInfo.VendorRank != oldEntity.VendorBasicInfo.VendorRank)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_Level_BasicMsg") + oldEntity.VendorBasicInfo.VendorRank + "->" + entity.VendorBasicInfo.VendorRank;
                }
                if (entity.VendorBasicInfo.BuyWeekDayVendor != oldEntity.VendorBasicInfo.BuyWeekDayVendor)
                {
                    if (!string.IsNullOrEmpty(basicMsg))
                    {
                        basicMsg += comma;
                    }
                    basicMsg += GetMessageString("Vendor_BuyWeekDay_BasicMsg") + oldEntity.VendorBasicInfo.BuyWeekDayVendor + "->" + entity.VendorBasicInfo.BuyWeekDayVendor;
                }
                if (!string.IsNullOrEmpty(basicMsg))
                {
                    basicMsg += "。";
                }

                #endregion 基本信息

                #region 财务信息

                if (string.Compare(entity.VendorFinanceInfo.BankName, oldEntity.VendorFinanceInfo.BankName) != 0)
                {
                    financeMsg += GetMessageString("Vendor_Bank_BasicMsg") + oldEntity.VendorFinanceInfo.BankName + "->" + entity.VendorFinanceInfo.BankName;
                }
                if (entity.VendorFinanceInfo.AccountNumber != oldEntity.VendorFinanceInfo.AccountNumber)
                {
                    if (!string.IsNullOrEmpty(financeMsg))
                    {
                        financeMsg += comma;
                    }
                    financeMsg += GetMessageString("Vendor_Account_BasicMsg") + oldEntity.VendorFinanceInfo.AccountNumber + "->" + entity.VendorFinanceInfo.AccountNumber;
                }
                if (entity.VendorFinanceInfo.PayPeriod != oldEntity.VendorFinanceInfo.PayPeriod)
                {
                    if (!string.IsNullOrEmpty(financeMsg))
                    {
                        financeMsg += comma;
                    }
                    financeMsg += GetMessageString("Vendor_PayPeriod_BasicMsg") + oldEntity.VendorFinanceInfo.PayPeriod + "->" + entity.VendorFinanceInfo.PayPeriod;
                }
                if (!string.IsNullOrEmpty(financeMsg))
                {
                    financeMsg += "。";
                }

                #endregion 财务信息

                #region 售后信息

                if (entity.VendorServiceInfo.Contact != oldEntity.VendorServiceInfo.Contact)
                {
                    afterSaleMsg += GetMessageString("Vendor_VendorContact_BasicMsg") + oldEntity.VendorServiceInfo.Contact + "->" + entity.VendorServiceInfo.Contact;
                }
                if (entity.VendorServiceInfo.ContactPhone != oldEntity.VendorServiceInfo.ContactPhone)
                {
                    if (!string.IsNullOrEmpty(afterSaleMsg))
                    {
                        afterSaleMsg += comma;
                    }
                    afterSaleMsg += GetMessageString("Vendor_Phone_BasicMsg") + oldEntity.VendorServiceInfo.ContactPhone + "->" + entity.VendorServiceInfo.ContactPhone;
                }
                if (entity.VendorServiceInfo.AreaInfo.SysNo != oldEntity.VendorServiceInfo.AreaInfo.SysNo)
                {
                    if (!string.IsNullOrEmpty(afterSaleMsg))
                    {
                        afterSaleMsg += comma;
                    }
                    string oldArea = string.Empty;
                    if (oldEntity!=null && oldEntity.VendorServiceInfo!=null && oldEntity.VendorServiceInfo.AreaInfo!=null &&  oldEntity.VendorServiceInfo.AreaInfo.SysNo.HasValue)
                    {
                        oldArea = ExternalDomainBroker.GetAreaInfo(oldEntity.VendorServiceInfo.AreaInfo.SysNo.Value).DistrictName;
                    }
                    string newArea = ExternalDomainBroker.GetAreaInfo(entity.VendorServiceInfo.AreaInfo.SysNo.Value).DistrictName;
                    afterSaleMsg += GetMessageString("Vendor_Province_BasicMsg") + oldArea + "->" + newArea;
                }
                if (entity.VendorServiceInfo.Address != oldEntity.VendorServiceInfo.Address)
                {
                    if (!string.IsNullOrEmpty(afterSaleMsg))
                    {
                        afterSaleMsg += comma;
                    }
                    afterSaleMsg += GetMessageString("Vendor_VendorAddress_BasicMsg") + oldEntity.VendorServiceInfo.Address + "->" + entity.VendorServiceInfo.Address;
                }
                if (entity.VendorServiceInfo.RMAServiceArea != oldEntity.VendorServiceInfo.RMAServiceArea)
                {
                    if (!string.IsNullOrEmpty(afterSaleMsg))
                    {
                        afterSaleMsg += comma;
                    }
                    afterSaleMsg += GetMessageString("Vendor_ServiceArea_BasicMsg") + oldEntity.VendorServiceInfo.RMAServiceArea + "->" + entity.VendorServiceInfo.RMAServiceArea;
                }
                if (entity.VendorServiceInfo.RMAPolicy != oldEntity.VendorServiceInfo.RMAPolicy)
                {
                    if (!string.IsNullOrEmpty(afterSaleMsg))
                    {
                        afterSaleMsg += comma;
                    }
                    afterSaleMsg += GetMessageString("Vendor_RefundPolicy_BasicMsg") + oldEntity.VendorServiceInfo.RMAPolicy + "->" + entity.VendorServiceInfo.RMAPolicy;
                }
                if (!string.IsNullOrEmpty(afterSaleMsg))
                {
                    afterSaleMsg += "。";
                }

                #endregion 售后信息
            }
        }

        /// <summary>
        /// 自动记录操作日志标胶财务需审核的信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="financeMsg"></param>
        private void CompareFinanceRequest(VendorModifyRequestInfo entity, out string financeMsg)
        {
            financeMsg = string.Empty;
            string flag = "，";
            VendorInfo oldEntity = LoadVendorInfo(entity.VendorSysNo);
            if (oldEntity != null)
            {
                #region 财务信息

                if (entity.PayPeriodType.PayTermsNo != oldEntity.VendorFinanceInfo.PayPeriodType.PayTermsNo)
                {
                    VendorPayTermsItemInfo getOldPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(oldEntity.VendorFinanceInfo.PayPeriodType.PayTermsNo.Value);
                    VendorPayTermsItemInfo getNewPayTermsInfo = VendorPayTermsDA.GetVendorPayTermsInfoBySysNo(entity.PayPeriodType.PayTermsNo.Value);
                    financeMsg += GetMessageString("Vendor_PayPeriodType_BasicMsg") + getOldPayTermsInfo.PayTermsName + "->"
                                   + getNewPayTermsInfo.PayTermsName;
                }
                if (entity.ValidDate != oldEntity.VendorFinanceInfo.CooperateValidDate)
                {
                    if (!string.IsNullOrEmpty(financeMsg))
                    {
                        financeMsg += flag;
                    }
                    string oldValidDate = string.Empty;
                    if (oldEntity.VendorFinanceInfo.CooperateValidDate.HasValue)
                    {
                        oldValidDate = entity.ValidDate.Value.ToString("yyyy-MM-dd");
                    }
                    financeMsg += GetMessageString("Vendor_CooperateBeginDate_BasicMsg") + oldValidDate + "->"
                                  + entity.ValidDate.Value.ToString("yyyy-MM-dd");
                }
                if (entity.ExpiredDate != oldEntity.VendorFinanceInfo.CooperateExpiredDate)
                {
                    if (!string.IsNullOrEmpty(financeMsg))
                    {
                        financeMsg += flag;
                    }
                    string oldExpiredDate = string.Empty;
                    if (oldEntity.VendorFinanceInfo.CooperateValidDate.HasValue)
                    {
                        oldExpiredDate = entity.ValidDate.Value.ToString("yyyy-MM-dd");
                    }
                    financeMsg += GetMessageString("Vendor_CooperateEndDate_BasicMsg") + oldExpiredDate + "->"
                                   + entity.ExpiredDate.Value.ToString("yyyy-MM-dd");
                }
                if (entity.ContractAmt != oldEntity.VendorFinanceInfo.CooperateAmt)
                {
                    string cooperateAmtString = GetMessageString("Vendor_CooperateAmt_BasicMsg");

                    if (!string.IsNullOrEmpty(financeMsg))
                    {
                        financeMsg += flag;
                    }
                    if (oldEntity.VendorFinanceInfo.CooperateAmt.HasValue && entity.ContractAmt.HasValue)
                    {
                        financeMsg += cooperateAmtString + decimal.Round(oldEntity.VendorFinanceInfo.CooperateAmt.Value, 2) + "->"
                            + decimal.Round(entity.ContractAmt.Value, 2);
                    }
                    else if (oldEntity.VendorFinanceInfo.CooperateAmt.HasValue)
                    {
                        financeMsg += cooperateAmtString + decimal.Round(oldEntity.VendorFinanceInfo.CooperateAmt.Value, 2) + "->"
                            + " ";
                    }
                    else
                    {
                        financeMsg += cooperateAmtString + " " + "->"
                          + decimal.Round(entity.ContractAmt.Value, 2);
                    }
                }
                if (!string.IsNullOrEmpty(financeMsg))
                {
                    financeMsg += "。";
                }

                #endregion 财务信息
            }
        }

        #endregion 【验证方法】

        #region For Invoice

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorByVendorSysNo(int vendorSysNo)
        {
            return VendorDA.IsHolderVendorByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorByPOSysNo(int poSysNo)
        {
            return VendorDA.IsHolderVendorByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的商家是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return VendorDA.IsHolderVendorByVendorSettleSysNo(vendorSettleSysNo);
        }

        /// <summary>
        /// 判断应付款对应的商家是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo)
        {
            return VendorDA.IsHolderVendorByCollectionSettlementSysNo(collectionSettlementSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorPMByPOSysNo(int poSysNo)
        {
            return VendorDA.IsHolderVendorPMByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的商家PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return VendorDA.IsHolderVendorPMByVendorSettleSysNo(vendorSettleSysNo);
        }

        #endregion For Invoice

        #region SellerID 相关

        public string GetSellerID(VendorInfo entity)
        {
            if (entity.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG)
            {
                var sellerID = VendorDA.GetMaxSellerID(0, entity.CompanyCode);

                if (string.IsNullOrEmpty(sellerID))
                {
                    return "002";
                }

                if (char.IsLetter(sellerID[0]))
                {
                    return CreateSellerID(sellerID).PadLeft(3, '0');
                }
                else
                {
                    var currentValue = Convert.ToInt32(sellerID);

                    if (currentValue > 9999)
                    {
                        throw new BizException(string.Format(GetMessageString("Vendor_SettleIDMaxLength"), currentValue));
                    }

                    if (currentValue == 9999)
                    {
                        return "A000";
                    }
                    else
                    {
                        return (++currentValue).ToString().PadLeft(3, '0');
                    }
                }
            }
            else
            {
                var sellerID = VendorDA.GetMaxSellerID(1, entity.CompanyCode);

                if (string.IsNullOrEmpty(sellerID))
                {
                    return "002";
                }

                if (char.IsLetter(sellerID[0]))
                {
                    return CreateSellerID(sellerID).PadLeft(3, '0');
                }
                else
                {
                    var currentValue = Convert.ToInt32(sellerID);

                    if (currentValue > 999)
                    {
                        throw new BizException(string.Format(GetMessageString("Vendor_SettleIDMaxLength"), currentValue));
                    }

                    if (currentValue == 999)
                    {
                        return "A00";
                    }
                    else
                    {
                        return (++currentValue).ToString().PadLeft(3, '0');
                    }
                }
            }
        }

        private string CreateSellerID(string sellerID)
        {
            sellerID = sellerID.Trim();
            int currentValue = 0;
            var step = _dict.Count;

            for (int i = 0; i < sellerID.Length; i++)
            {
                var key = sellerID.Substring(i, 1);
                if (!_dict.ContainsKey(key))
                {
                    throw new NotSupportedException(string.Format(GetMessageString("Vendor_CharInvalid"), key));
                }

                var number = _dict[key];

                currentValue += number * Convert.ToInt32(Math.Pow(step, sellerID.Length - i - 1));
            }

            currentValue++;
            int left = 0;
            StringBuilder sb = new StringBuilder();

            do
            {
                left = currentValue % step;
                currentValue = currentValue / step;

                var character = _dict.Keys.ToList()[left];

                sb.Insert(0, character);
            } while (currentValue != 0);

            var result = sb.ToString();

            if (result.Length > sellerID.Length)
            {
                throw new OverflowException("SellerID");
            }
            else
            {
                return result;
            }
        }

        private static void InitBannedList()
        {
            var bannedString = AppSettingManager.GetSetting("PO", "BannedCharacters");
            List<string> bandedList = new List<string>();

            if (!string.IsNullOrEmpty(bannedString))
            {
                var strArray = bannedString.Split(',');
                for (var i = 0; i < strArray.Length; i++)
                {
                    bandedList.Add(strArray[i]);
                }
            }

            int index = 0;
            for (int i = 0; i < 10; i++)
            {
                if (!_dict.ContainsKey(i.ToString()))
                    _dict.Add(i.ToString(), index++);
            }

            for (int i = 65; i < 91; i++)
            {
                var strValue = new string((char)i, 1);

                if (!bandedList.Contains(strValue))
                {
                    if (!_dict.ContainsKey(strValue))
                        _dict.Add(strValue, index++);
                }
            }
        }

        #endregion SellerID 相关

        //发送邮件 ：
        private void SendMail(int sysNo, string subject)
        {
            DataTable mailInfoDT = VendorDA.GetVendorMailInfo(sysNo);
            if (null != mailInfoDT && mailInfoDT.Rows.Count > 0)
            {
                string toMailAddress = AppSettingManager.GetSetting("PO", "VendorFinanceMailTo") + "；" + (mailInfoDT.Rows[0]["CreateUserEmailAddress"] == null ? string.Empty : mailInfoDT.Rows[0]["CreateUserEmailAddress"].ToString());

                KeyValueVariables keyValueList = new KeyValueVariables();
                keyValueList.Add("MailSubject", subject);
                keyValueList.Add("VendorName", mailInfoDT.Rows[0]["VendorName"].ToString());
                if (mailInfoDT.Rows[0]["ValidDate"] != null && mailInfoDT.Rows[0]["ExpiredDate"] != null)
                {
                    keyValueList.Add("ValidTime", Convert.ToDateTime(mailInfoDT.Rows[0]["ValidDate"].ToString()).ToString("yyyy-MM-dd") + " - " + Convert.ToDateTime(mailInfoDT.Rows[0]["ExpiredDate"].ToString()).ToString("yyyy-MM-dd"));
                }
                else
                {
                    keyValueList.Add("ValidTime", "&nbsp;");
                }
                keyValueList.Add("ContractAmt", mailInfoDT.Rows[0]["ContractAmt"] == null ? "&nbsp;" : mailInfoDT.Rows[0]["ContractAmt"].ToString());
                keyValueList.Add("District", mailInfoDT.Rows[0]["District"] == null ? "&nbsp;" : mailInfoDT.Rows[0]["District"].ToString());

                if (null != mailInfoDT.Rows[0]["PayPeriodType"])
                {
                    VendorPayPeriodType payType;

                    if (!Enum.TryParse<VendorPayPeriodType>(mailInfoDT.Rows[0]["PayPeriodType"].ToString(), out payType))
                    {
                        throw new BizException(GetMessageString("Vendor_SendMailFailed"));
                    }
                    keyValueList.Add("PayPeriodType", EnumHelper.GetDisplayText(payType));
                }
                else
                {
                    keyValueList.Add("PayPeriodType", "&nbsp;");
                }

                keyValueList.Add("Apply", mailInfoDT.Rows[0]["CreateUserName"] == null ? "&nbsp;" : mailInfoDT.Rows[0]["CreateUserName"].ToString());
                //发送内部邮件:
                EmailHelper.SendEmailByTemplate(toMailAddress, "Vendor_UpdateVendorActionMail", keyValueList, true);
            }
            else
            {
                throw new BizException(GetMessageString("Vendor_SendMailFailed"));
            }
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.Vendor", key);
        }

        public virtual List<KeyValuePair<int, string>> GetVendorNameListByVendorType(VendorType vendorType, string companyCode)
        {
            return VendorDA.GetVendorNameListByVendorType(vendorType, companyCode);
        }
    }
}
