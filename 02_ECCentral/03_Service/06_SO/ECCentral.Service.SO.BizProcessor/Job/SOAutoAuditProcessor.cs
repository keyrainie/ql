using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.SO.BizProcessor.Job
{
    [VersionExport(typeof(SOAutoAuditProcessor))]
    public class SOAutoAuditProcessor
    {
        #region PreData List

        #region 需要传入的

        private string m_interOrder;
        private string m_companyCode;
        /// <summary>
        /// 自动审单默认用户编号
        /// </summary>
        int m_auditUserSysNo;

        #endregion

        DateTime m_now;
        int m_auditUserSysNoTemp;

        ISODA m_da;

        /// <summary>
        /// 恶意客户列表
        /// </summary>
        private List<CustomerInfo> m_malevolenceCustomers;
        /// <summary>
        /// 支付方式列表
        /// </summary>
        private List<PayType> m_payTypeList;

        //自动审单控制开关实体
        private AutoAuditSwitch m_auditSwitch;

        //自动审单FP状态明细检查项开关实体
        private FPSwitch m_fpSwitch;

        //自动审单客户类型明细检查项开关实体
        private CustomerTypeSwitch m_customerTypeSwitch;

        //不能通过审单的地址关键字列表
        private List<string> m_dispassReceiveAddressList;

        //不能通过审单的手机或电话关键字列表
        private List<string> m_dispassReceiveCellPhoneOrPhoneList;

        //不能通过审单的收货联系人关键字列表
        private List<string> m_dispassReceiveContactList;

        //不能通过审单的支付方式编号列表
        private List<int> m_dispassPayTypeSysNoList;

        //不能通过自动审单的配送方式编号列表
        private List<int> m_dispassShipTypeSysNoList;

        //获取会员等级及对应金额实体信息列表
        private List<CustomerRankAmt> m_customerRankAmtList;

        /// <summary>
        /// 获取自动审单检查项信息（CS工具箱自动审单维护）
        /// </summary>
        private List<OrderCheckMaster> m_csTBOrderCheckMasterList;

        /// <summary>
        /// 获取自动审单检查项具体明细项信息（CS工具箱自动审单明细项维护）
        /// </summary>
        private List<OrderCheckItem> m_csTBOrderCheckItemList;

        private List<DateTime> m_csNoWorkDayList;

        //节假日
        private List<Holiday> m_holidays;

        #endregion

        #region Entry Point

        /// <summary>
        /// 自动审单
        /// </summary>
        /// <param name="bizLogFile">业务日志文件全名</param>
        public void AuditSO(string interOrder, string companyCode, int auditUserSysNo)
        {
            m_interOrder = interOrder;
            m_companyCode = companyCode;
            m_auditUserSysNo = auditUserSysNo;
            m_now = DateTime.Now;
            m_da = ObjectFactory<ISODA>.Instance;

            //获取自动审单检查项信息
            m_csTBOrderCheckMasterList = ExternalDomainBroker.GetCSTBOrderCheckMasterList(m_companyCode);

            //获取自动审单检查项具体的明细项信息
            m_csTBOrderCheckItemList = new List<OrderCheckItem>();
            m_csTBOrderCheckMasterList.ForEach(p => {
                m_csTBOrderCheckItemList.AddRange(p.OrderCheckItemList);
            });

            //获取非客服工作日列表
            m_csNoWorkDayList = ExternalDomainBroker.GetHolidayList("NoCSWorkTime", m_companyCode)
                                .Where(p => p >= m_now.Date).ToList();

            #region 自动审核暂停项被选中 则进行自动审核的暂停判断
            //如果存在 自动审核暂停时间项的检测  则进行自动审核暂停明细项的进一步判断（当前时间点属于自动审核暂停的时间段范围 则 立即暂停自动审核） 
            if (m_csTBOrderCheckMasterList.Count > 0 && m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "AT" && x.Status == OrderCheckStatus.Invalid); }))
            {
                //过滤,  获取有效停止自动审核时间段 列表信息
                var itemList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "SA" && x.Status == OrderCheckStatus.Invalid && m_now < Convert.ToDateTime(x.Description)); });

                if (itemList != null && itemList.Count() > 0)
                {
                    foreach (var oci in itemList)
                    {
                        if (m_now > Convert.ToDateTime(oci.ReferenceContent))
                        {
                            //jobContext.Message = "当前时间不属于自动审单时间";
                            return;
                        }
                    }
                }
            }
            #endregion

            //从配置文件中获取自动审核一次最多提取的单数
            int topCount = GetTopCount();
            //获取本次审单的记录集
            var tmpSOList = m_da.GetSOList4Audit(topCount, m_companyCode);
            if (tmpSOList.Count == 0)
            {
                //没有符合自动审核的订单
                return;
            }
            //获取今天后的所有节假日
            m_holidays = ExternalDomainBroker.GetHolidayAfterToday(m_companyCode);

            //设置相应的审单检查项
            SetAuditSwitchStatus();

            //设置FP状态检查项明细项信息
            SetAuditFPStatus();

            //设置 客户类型检查明细项
            SetAuditCustomerType();

            //提前取得恶意用户的列表
            m_malevolenceCustomers = ExternalDomainBroker.GetMalevolenceCustomers(m_companyCode);

            //提前取得所有支付方式的列表
            m_payTypeList = ExternalDomainBroker.GetPayTypeList(m_companyCode);

            foreach (var x in tmpSOList)
            {
                try
                {
                    CheckRules(x);
                }
                catch(Exception ex)
                {
                    ExceptionHelper.HandleException(ex);
                    WriteLog(ex.Message);
                }
            }

            EndAuditAll();
        }

        //设置 审单改观检查项 开关状态
        private void SetAuditSwitchStatus()
        {
            m_auditSwitch = new AutoAuditSwitch();

            #region 填充开关状态

            if (m_csTBOrderCheckMasterList.Count > 0)
            {
                //判断 关键字开关 是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "KW" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckKeyWords = true;
                }

                //判断 配送方式项开关 是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "ST" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckShipType = true;
                }

                //判断 支付方式开关 是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "PT" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckPayType = true;
                }

                //判断 增值税发票开关 是否打开 
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "VAT" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckVAT = true;
                }

                //判断 含有对应产品类别的产品的订单(C3类)开关 是否打开 
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "PC3" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckProductType = true;
                }

                //判断 FP状态开关 是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return ((x.CheckType == "FP") && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckFPStatus = true;
                }

                //判 断客户类型开关 是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "CT" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckCustomerType = true;
                }

                //判断 订单中使用积分或者优惠券超过50%开关 是否打开  
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "PC" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckPointPromotion = true;
                }

                //判断 订单金额大于上对应登记上线 的开关是否 打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "AMT" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckOrderAmt = true;
                }

                //判断 配送服务类型  开关是否打开
                if (m_csTBOrderCheckMasterList.Exists(x => { return (x.CheckType == "DT" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_auditSwitch.IsCheckShipServiceType = true;
                }
            }

            #endregion

            #region 填充不能通过自动审单相应列表信息

            //设置不能通过自动审单的关键字列表
            if (m_auditSwitch.IsCheckKeyWords)
            {
                //过滤客户地址类型的关键字信息列表 (模糊匹配)   CA：客户地址
                m_dispassReceiveAddressList = new List<string>();
                var addressList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "CA" && x.Status == OrderCheckStatus.Invalid); });
                foreach (var itemAddress in addressList)
                {
                    m_dispassReceiveAddressList.Add(itemAddress.ReferenceContent);
                }

                //过滤客户电话类型的关键字 信息列表(精确匹配)   CP：客户电话
                m_dispassReceiveCellPhoneOrPhoneList = new List<string>();
                var phoneList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "CP" && x.Status == OrderCheckStatus.Invalid); });
                foreach (var itemPhone in phoneList)
                {
                    m_dispassReceiveCellPhoneOrPhoneList.Add(itemPhone.ReferenceContent);
                }

                //过滤联系人类型的关键字列表信息(模糊匹配)     CN：联系人
                m_dispassReceiveContactList = new List<string>();
                var contactList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "CN" && x.Status == OrderCheckStatus.Invalid); });
                foreach (var itemContact in contactList)
                {
                    m_dispassReceiveContactList.Add(itemContact.ReferenceContent);
                }
            }

            //设置不能通过自动审单的配送方式列表
            if (m_auditSwitch.IsCheckShipType)
            {
                m_dispassShipTypeSysNoList = new List<int>();
                //过滤配送方式列表信息(ST:配送方式类型)
                var shipList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "ST" && x.Status == OrderCheckStatus.Invalid); });
                foreach (var itemShip in shipList)
                {
                    m_dispassShipTypeSysNoList.Add(Convert.ToInt32(itemShip.ReferenceContent));
                }
            }

            //设置不能通过自动审单的支付方式列表
            if (m_auditSwitch.IsCheckPayType)
            {
                m_dispassPayTypeSysNoList = new List<int>();
                //过滤支付方式列表信息(PT:支付方式类型)
                var payList = m_csTBOrderCheckItemList.Where(x => { return (x.ReferenceType == "PT" && x.Status == OrderCheckStatus.Invalid); });
                foreach (var itemPay in payList)
                {
                    m_dispassPayTypeSysNoList.Add(Convert.ToInt32(itemPay.ReferenceContent));
                }
            }

            //设置 订单金额 检查项
            if (m_auditSwitch.IsCheckOrderAmt)
            {
                m_customerRankAmtList = new List<CustomerRankAmt>();
                //过滤 订单金额 及对应的用户等级列表
                var rankList = m_csTBOrderCheckItemList.Where(x => { return ((x.ReferenceType == "1AL" || x.ReferenceType == "2AL" || x.ReferenceType == "3AL" || x.ReferenceType == "4AL" || x.ReferenceType == "5AL") && x.Status == OrderCheckStatus.Invalid); });

                foreach (var item in rankList)
                {
                    CustomerRankAmt crle = new CustomerRankAmt();
                    switch (item.ReferenceType)
                    {
                        case "1AL":
                            crle.CustomerRank = 3;
                            break;
                        case "2AL":
                            crle.CustomerRank = 4;
                            break;
                        case "3AL":
                            crle.CustomerRank = 5;
                            break;
                        case "4AL":
                            crle.CustomerRank = 6;
                            break;
                        case "5AL":
                            crle.CustomerRank = 7;
                            break;
                        default:
                            break;
                    }
                    crle.AmtLimit = Convert.ToDecimal(item.ReferenceContent);
                    m_customerRankAmtList.Add(crle);
                }
            }
            #endregion
        }

        /// <summary>
        /// 设置订单类型 具体的检查项
        /// </summary>
        private void SetAuditFPStatus()
        {
            m_fpSwitch = new FPSwitch();
            #region 填充FP开关状态
            if (m_auditSwitch.IsCheckFPStatus)
            {
                if (m_csTBOrderCheckItemList.Count > 0)
                {
                    if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "KY" && x.Status == OrderCheckStatus.Invalid); }))
                    {
                        m_fpSwitch.IsCheckKY = true;
                    }
                    if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CH" && x.Status == OrderCheckStatus.Invalid); }))
                    {
                        m_fpSwitch.IsCheckCH = true;
                    }
                    if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CC" && x.Status == OrderCheckStatus.Invalid); }))
                    {
                        m_fpSwitch.IsCheckCC = true;
                    }
                    if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "FP" && x.ReferenceContent == "CF" && x.Status == OrderCheckStatus.Invalid); }))
                    {
                        m_fpSwitch.IsCheckCF = true;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 填充客户类型检查明细项开关
        /// </summary>
        private void SetAuditCustomerType()
        {
            m_customerTypeSwitch = new CustomerTypeSwitch();

            if (m_auditSwitch.IsCheckCustomerType && m_csTBOrderCheckItemList.Count > 0)
            {
                if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "ZC" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_customerTypeSwitch.IsCheckCustomerZC = true;
                }
                if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "KY" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_customerTypeSwitch.IsCheckCustomerKY = true;
                }
                if (m_csTBOrderCheckItemList.Exists(x => { return (x.ReferenceType == "CT" && x.ReferenceContent == "QZ" && x.Status == OrderCheckStatus.Invalid); }))
                {
                    m_customerTypeSwitch.IsCheckCustomerQZ = true;
                }
            }
        }

        /// <summary>
        /// 获取一次审核最多的记录数
        /// </summary>
        /// <returns></returns>
        private int GetTopCount()
        {
            int result = 0;
            if (!int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_AutoAudit_TopCount"), out result))
            {
                result = 2000;
            }
            return result;
        }

        #endregion Entry Point

        /// <summary>
        /// 检查规则-公共部分
        /// </summary>
        /// <param name="entity"></param>
        private void CheckRules(SOInfo entity)
        {
            //初始临时审核用户
            m_auditUserSysNoTemp = m_auditUserSysNo;

            #region 已在提取的时候进行过滤
            
            //#region  订单金额为0

            //if (entity.BaseInfo.SOTotalAmount == 0)
            //{
            //    ThrowException(entity, "订单金额为零，不能通过自动审核");
            //}

            //#endregion

            //#region  配送区域不能为空

            //if (!entity.ReceiverInfo.AreaSysNo.HasValue || entity.ReceiverInfo.AreaSysNo.Value <= 0)
            //{
            //    ThrowException(entity, "配送区域无效，不能通过自动审核");
            //}

            //#endregion

            #endregion

            #region  Intel用户订单不能通过自动审核

            var custom = ExternalDomainBroker.GetCustomerInfo(entity.BaseInfo.CustomerSysNo.Value);
            if (custom.BasicInfo.FromLinkSource == m_interOrder)
            {
                ThrowException(entity, "Intel用户所下订单不能通过自动审核");
            }

            #endregion

            #region 订单备注,订单说明,客户备注有特殊要求 - 不能通过自动审核

            if (!string.IsNullOrEmpty(entity.BaseInfo.Memo)
                || !string.IsNullOrEmpty(entity.BaseInfo.Note)
                || !string.IsNullOrEmpty(entity.InvoiceInfo.InvoiceNote))
            {
                ThrowException(entity, "订单备注,订单说明,客户备注有特殊要求");
            }

            #endregion

            #region 参加团购的订单 - 不能通过自动审核

            if (entity.BaseInfo.IsWholeSale.HasValue
                && entity.BaseInfo.IsWholeSale.Value)
            {
                ThrowException(entity, "此订单为团购订单");
            }

            #endregion

            #region 订单为:非待审核状态 - 不能通过自动审核
            //提取提单时已按此条件过滤
            #endregion

            #region 订单为:订单锁定 - 不能通过自动审核
            //提取提单时已按此条件过滤
            #endregion

            #region 订单为恶意用户下单 - 不能通过自动审核

            if (m_malevolenceCustomers.Count > 0
                && m_malevolenceCustomers.Exists(x =>
                {
                    return x.SysNo.Value == entity.BaseInfo.CustomerSysNo.Value;
                }))
            {
                ThrowException(entity, "订单为恶意用户下单");
            }
            #endregion

            #region 产品无现货(即产品存在“虚库不足”，“待采购”字样) - 不能通过自动审核

            //虚库不足，待采购
            if (entity.ShippingInfo.StockType == StockType.SELF
                && entity.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF
                && entity.InvoiceInfo.InvoiceType == InvoiceType.SELF
                && entity.BaseInfo.Status == SOStatus.Origin
                && entity.FPInfo.IsFPCheck.HasValue
                && entity.StatusChangeInfoList.Count == 0
                )
            {
                LoadItems(entity);
                //标志虚库不足和待采购
                ObjectFactory<SOProcessor>.Instance.SetItemOutStockInfo(entity);

                if (entity.Items.Exists(p => p.IsLessStock || p.IsWaitPO))
                {
                    ThrowException(entity, "产品无现货(即产品存在“虚库不足”，“待采购”字样)");
                }
            }

            #endregion

            #region 订单中的信息与关键字匹配则 - 不能通过自动审核

            if (m_auditSwitch.IsCheckKeyWords)
            {
                // 关键字(接收地址)
                if (entity.ReceiverInfo.Address.IsContains(m_dispassReceiveAddressList))
                {
                    ThrowException(entity, "订单中收货地址信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(手机 精确匹配)
                if (entity.ReceiverInfo.MobilePhone.IsEquals(m_dispassReceiveCellPhoneOrPhoneList))
                {
                    ThrowException(entity, "订单中的手机信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(电话 精确匹配)
                if (entity.ReceiverInfo.Phone.IsEquals(m_dispassReceiveCellPhoneOrPhoneList))
                {
                    ThrowException(entity, "订单中的电话信息存在特殊关键字 - 不能通过自动审核");
                }

                // 关键字(收货联系人)      
                if (entity.ReceiverInfo.Name.IsContains(m_dispassReceiveContactList))
                {
                    ThrowException(entity, "订单中收货联系人信息存在特殊关键字 - 不能通过自动审核");
                }
            }

            #endregion

            #region 订单 的配送方式、支付方式 属于限定配送方式 - 不能通过自动审核
            //提取时过滤
            //if ((entity.BaseInfo.PayTypeSysNo ?? 0) == 0)
            //{
            //    ThrowException(entity, "订单的支付方式为空 - 不能通过自动审核");
            //}
            if (m_auditSwitch.IsCheckPayType && m_dispassPayTypeSysNoList.Contains(entity.BaseInfo.PayTypeSysNo.Value))
            {
                ThrowException(entity, "订单的支付方式属于限定支付方式 - 不能通过自动审核");
            }

            //提取时过滤
            //if ((entity.ShippingInfo.ShipTypeSysNo ?? 0) == 0)
            //{
            //    ThrowException(entity, "订单的配送方式为空 - 不能通过自动审核");
            //}
            if (m_auditSwitch.IsCheckShipType && m_dispassShipTypeSysNoList.Contains(entity.ShippingInfo.ShipTypeSysNo.Value))
            {
                ThrowException(entity, "订单的配送方式属于限定配送方式 - 不能通过自动审核");
            }

            #endregion

            #region 根据FP状态判断订单是否可以通过自动审核

            if (m_auditSwitch.IsCheckFPStatus)
            {
                if (m_fpSwitch.IsCheckCC
                 && entity.FPInfo.FPSOType.HasValue
                 && entity.FPInfo.FPSOType.Value == FPSOType.ChaoHuo)
                {
                    ThrowException(entity, "订单为炒货订单，不能通过自动审核");
                }
                else if (m_fpSwitch.IsCheckKY
                    && entity.FPInfo.FPSOType.HasValue
                    && entity.FPInfo.FPSOType.Value == FPSOType.KeYi)
                {
                    ThrowException(entity, "订单为可疑订单，不能通过自动审核");
                }
                else if (m_fpSwitch.IsCheckCH
                    && entity.FPInfo.FPSOType.HasValue
                    && entity.FPInfo.FPSOType.Value == FPSOType.ChuanHuo)
                {
                    ThrowException(entity, "订单为串货订单，不能通过自动审核");
                }

                if (m_fpSwitch.IsCheckCF
                   && entity.IsDuplicateOrder.HasValue
                    && entity.IsDuplicateOrder.Value)
                {
                    ThrowException(entity, "订单为重复订单，不能通过自动审核");
                }
            }

            #endregion

            #region 根据订单的客户类型判断订单是否可以通过审核

            if (m_auditSwitch.IsCheckCustomerType)
            {
                var kfcCustomer = ObjectFactory<ISOKFCDA>.Instance.GetKFCByCustomerSysNo(entity.BaseInfo.CustomerSysNo.Value);
                if (kfcCustomer != null)
                {
                    if (m_customerTypeSwitch.IsCheckCustomerZC && kfcCustomer.KFCType == KFCType.Normal)
                    {
                        ThrowException(entity, "正常用户类型，不能通过自动审核");
                    }
                    if (m_customerTypeSwitch.IsCheckCustomerKY && kfcCustomer.KFCType == KFCType.KeYi)
                    {
                        ThrowException(entity, "可疑用户类型，不能通过自动审核");
                    }
                    if (m_customerTypeSwitch.IsCheckCustomerQZ && kfcCustomer.KFCType == KFCType.QiZha)
                    {
                        ThrowException(entity, "欺诈用户类型，不能通过自动审核");
                    }
                }
            }

            #endregion

            #region 根据 自动审核检查项:增值税发票： 订单中含有增值税发票则不能通过自动审核。
            if (m_auditSwitch.IsCheckVAT)
            {
                if (entity.InvoiceInfo.IsVAT.HasValue && entity.InvoiceInfo.IsVAT.Value)
                {
                    ThrowException(entity, "订单中含有增值税发票 - 不能通过自动审核");
                }
            }
            #endregion

            #region 根据 自动审核检查项:订单中使用积分或者优惠券超过50% ： 单张订单金额中使用积分或者优惠券超过50% - 不能通过自动审核

            if (m_auditSwitch.IsCheckPointPromotion)
            {
                var couponItems = entity.Items.Where(p => p.ProductType == SOProductType.Coupon);
                if ((entity.BaseInfo.PointPay / ExternalDomainBroker.GetPointToMoneyRatio() > entity.BaseInfo.SOAmount / 2.0M)
                    || couponItems.Count() > 0)
                {
                    if (entity.BaseInfo.PointPay / ExternalDomainBroker.GetPointToMoneyRatio() > entity.BaseInfo.SOAmount / 2.0M)
                    {
                        ThrowException(entity, "单张订单金额中使用积分超过50%  - 不能通过自动审核");
                    }
                    if (couponItems.Count() > 0)
                    {
                        //获取优惠券
                        foreach (var item in couponItems)
                        {
                            if (item.Price.Value / 1.0M > entity.BaseInfo.SOAmount / 2.0M)
                            {
                                ThrowException(entity, "单张订单金额中使用优惠券超过50%  - 不能通过自动审核");
                            }
                        }
                    }
                }
            }

            #endregion

            #region 含有对应产品类别的产品的订单(C3类)将不能通过自动审核

            if (m_auditSwitch.IsCheckProductType)
            {
                //3C验证必须先满足现在情况才进行
                //SM.Status = 0 AND (SM.AuditUserSysNo IS NULL OR SM.AuditTime IS NULL)  AND SCKP.IsFPCheck IS NOT NULL AND SCKP.AuditType IS NULL
                if (entity.BaseInfo.Status == SOStatus.Origin 
                    && entity.FPInfo.IsFPCheck.HasValue
                    && entity.StatusChangeInfoList.Count == 0)
                {
                    //商品验证才需要读取Item
                    LoadItems(entity);
                    //判断商品是否有在指定中
                    foreach (var item in entity.Items)
                    {
                        if (item.ProductType != SOProductType.Coupon)
                        {
                            //获取商品信息
                            var product = ExternalDomainBroker.GetProductSimpleInfo(item.ProductSysNo.Value);
                            if (product != null)
                            {
                                if (m_csTBOrderCheckItemList.Exists(p => p.Description == product.ProductBasicInfo.ProductCategoryInfo.SysNo.ToString()
                                                                    && p.ReferenceType == "PC3"
                                                                    && p.Status == OrderCheckStatus.Invalid))
                                {
                                    ThrowException(entity, "订单中有指定C3类产品 - 不能通过自动审核");
                                }
                                if (m_csTBOrderCheckItemList.Exists(p => p.ReferenceContent == product.ProductID
                                                                    && p.ReferenceType == "PID"
                                                                    && p.Status == OrderCheckStatus.Invalid))
                                {
                                    ThrowException(entity, "订单中有指定产品ID - 不能通过自动审核");
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region 自动审单对配送时间按照一日一送、一日两送的规则进行检查

            if (entity.ShippingInfo.DeliveryDate.HasValue)
            {
                if (m_auditSwitch.IsCheckShipServiceType)
                {
                    //过滤 产品类别列表信息 (PC3:产品类别)
                    var csItem = m_csTBOrderCheckItemList.FirstOrDefault(x =>
                                                                        {
                                                                            return ((x.ReferenceType == "DT11" || x.ReferenceType == "DT12")
                                                                                && x.Status == OrderCheckStatus.Invalid
                                                                                && x.ReferenceContent.Trim() == entity.ShippingInfo.ShipTypeSysNo.ToString());
                                                                        });
                    if (csItem != null)
                    {
                        CheckDeliverDateV2(entity, csItem);
                    }
                }
            }

            #endregion

            #region 付款验证
            
            if (m_payTypeList.Count > 0
                && m_payTypeList.Exists(x =>
                {
                    return (x.SysNo == entity.BaseInfo.PayTypeSysNo
                        && (x.IsPayWhenRecv ?? false)
                        );
                }))
            {
                //分支验证货到付款
                CheckCODSO(entity, custom);
                PassCODSO(entity);
                EndAudit(entity);
            }
            else
            {
                //分支验证款到发货
                bool isRetry;
                NetPayInfo soNetPayinfo = CheckPIASO(entity,out isRetry);
                //对于不需要再次审核的记录做处理
                if (!isRetry)
                {
                    PassPIASO(entity, soNetPayinfo);
                    EndAudit(entity);
                }
            }

            #endregion

        }

        /// <summary>
        /// 获取SOItems
        /// </summary>
        /// <param name="entity">订单对象</param>
        private static void LoadItems(SOInfo entity)
        {
            if (entity.Items == null || entity.Items.Count == 0)
            {
                entity.Items = ObjectFactory<SOProcessor>.Instance.GetSOItemsBySOSysNo(entity.SysNo.Value);
            }
        }

        private void CheckDeliverDateV2(SOInfo entity, OrderCheckItem csItem)
        {
            //配送信息
            bool isInvalid = SOAudit.IsInValidDeliveryTime(entity,csItem.Description,m_holidays,false);
            
            if (isInvalid)
            {
                ThrowException(entity, ResourceHelper.Get("SO_Audit_InvalidDeliveryDate"));
            }
        }

        private DateTime AdjustCheckDay(DateTime checkDay)
        {
            while (m_csNoWorkDayList.Contains(checkDay))
            {
                checkDay = checkDay.AddDays(1.0).Date;
            }
            return checkDay;
        }

        #region Check COD

        /// <summary>
        /// 检查规则-货到付款
        /// </summary>
        /// <param name="entity"></param>
        private void CheckCODSO(SOInfo entity,CustomerInfo custom)
        {
            m_auditUserSysNoTemp = 800;    //注意这里一定要是这个用户，Alan说有一个邮件专门根据这个用户来给货到付款的客户发短信
            
            #region 根据订单的客户等级及订单金额判断是否能够通过自动审核

            if (m_auditSwitch.IsCheckOrderAmt)
            {
                CustomerRankAmt currentRank = null;
                switch (custom.Rank)
                {
                    case CustomerRank.Ferrum:
                    case CustomerRank.Copper:
                    case CustomerRank.Silver:
                        currentRank = m_customerRankAmtList.Find(x => { return x.CustomerRank == 3; });
                        break;
                    case CustomerRank.Golden:
                    case CustomerRank.Diamond:
                    case CustomerRank.Crown:
                    case CustomerRank.Supremacy:
                        currentRank = m_customerRankAmtList.Find(x => { return x.CustomerRank == 4; });
                        break;
                    default:
                        break;
                }
                if (currentRank != null && entity.BaseInfo.SOAmount > currentRank.AmtLimit)
                {
                    ThrowException(entity, "订单金额超出了此客户等级对应的订单金额上线-不能通过自动审核");
                }
            }
            #endregion
        }

        /// <summary>
        /// 一单货到付款通过
        /// </summary>
        /// <param name="entity"></param>
        /// <![CDATA[
        /// 1.将订单标记为：1-待出库；
        /// 2.将CheckShipping中的审核类型标记为 0-自动审核
        /// ]]>
        private void PassCODSO(SOInfo entity)
        {
            if (entity.BaseInfo.SplitType == SOSplitType.Force || entity.BaseInfo.SplitType == SOSplitType.Customer)
            {
                ObjectFactory<SOProcessor>.Instance.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Split, SOInfo = entity, Parameter = new object[] { true } });
                m_da.UpdateCheckShippingAuditTypeBySysNo(entity.SysNo.Value, AuditType.Manual, "自动审单通过[货到付款], 自动拆分订单");
                m_da.UpdateSO4AuditUserInfo(entity.SysNo.Value, m_auditUserSysNoTemp);
                ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(new SOLogInfo() { SOSysNo = entity.BaseInfo.SysNo, OperationType = BizLogType.Sale_SO_Audit, Note = "自动审单通过[货到付款], 自动拆分订单" });
            }
            else
            {
                //更新时需要验证当前的SO状态
                bool isSuccess = m_da.UpdateSO4PassAutoAudit(entity.SysNo.Value, m_auditUserSysNoTemp);
                if (isSuccess)
                {
                    m_da.UpdateCheckShippingAuditTypeBySysNo(entity.SysNo.Value, AuditType.Auto, "自动审单通过[货到付款]");
                    ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(new SOLogInfo() { SOSysNo = entity.BaseInfo.SysNo, OperationType = BizLogType.Sale_SO_Audit, Note = "自动审单通过[货到付款]" });
                }
                else
                {
                    ThrowException(entity, "订单不存在或状态已不再是待审核状态");
                }
                //发送审核邮件
                ObjectFactory<SOProcessor>.Instance.SendMessage(entity, false, false, true);
                m_da.AuditSendMailAndUpdateSO(entity.SysNo.Value);
            }
        }

        #endregion Check COD

        #region Check PIA
        /// <summary>
        /// 检查规则-款到发货
        /// </summary>
        /// <param name="entity"></param>
        private NetPayInfo CheckPIASO(SOInfo entity,out bool isRetry)
        {
            NetPayInfo result;

            #region 款到发货订单,不是网络支付类型的 - 不能通过自动审核
            var payType = m_payTypeList.Find(x => { return x.SysNo == entity.BaseInfo.PayTypeSysNo; });

            if (payType == null)
            {
                ThrowException(entity, string.Format("款到发货订单,不能自动审单的支付类型:[{0}]", entity.BaseInfo.PayTypeSysNo));
            }

            if (!(payType.IsNet??false))
            {
                ThrowException(entity, "款到发货订单,不是网络支付");
            }

            if (payType.SysNo != entity.BaseInfo.PayTypeSysNo)
            {
                ThrowException(entity, "款到发货订单,订单中的付款方式和所选的付款方式不符");
            }

            #endregion

            result = ExternalDomainBroker.GetSOValidNetPay(entity.SysNo.Value);

            if (result == null)
            {
                //ThrowException(entity, "款到发货订单,没有网络支付信息");
                //因为支付可能延迟，如果不存在网上支付，需要重新再跑这个订单
                result = new NetPayInfo();
                isRetry = true;
                return result;
            }

            if (entity.BaseInfo.SOType != SOType.GroupBuy)
            {
                //Jin:要加上关税
                #region 订单金额 != 实收+余额支付 - 不能通过自动审核
                decimal? dSOAmount = result.PayAmount
                    + entity.BaseInfo.PrepayAmount
                    + entity.BaseInfo.GiftCardPay;
                decimal? dCalcAmount = entity.BaseInfo.CashPay
                        + entity.BaseInfo.PayPrice
                        + entity.BaseInfo.ShipPrice
                        + entity.BaseInfo.PremiumAmount
                        + entity.BaseInfo.PromotionAmount
                        + entity.BaseInfo.TariffAmount;


                if (Math.Round(dSOAmount.Value, 2, MidpointRounding.AwayFromZero) != Math.Round(dCalcAmount.Value, 2, MidpointRounding.AwayFromZero))
                //if ((result.PayAmount 
                //    + entity.BaseInfo.PrepayAmount 
                //    + entity.BaseInfo.GiftCardPay)
                //    != (entity.BaseInfo.CashPay 
                //        + entity.BaseInfo.PayPrice 
                //        + entity.BaseInfo.ShipPrice 
                //        + entity.BaseInfo.PremiumAmount 
                //        + entity.BaseInfo.PromotionAmount)
                //    )
                {
                    ThrowException(entity, "款到发货订单,未满足订单金额＝实收+余额支付+礼品卡支付");
                }
                #endregion

                #region 款到发货订单,NetPay信息为非Origin状态
                //if (result.Status != NetPayStatus.Origin)
                //{
                //    ThrowException(entity, "款到发货订单,NetPay信息为非Origin状态");
                //}
                #endregion
            }

            #region 款到发货订单,网上支付来源不为Bank和NoNeedPay，不能通过自动审核
            if (result.Source != NetPaySource.Bank && result.Source != NetPaySource.NoNeedPay)
            {
                ThrowException(entity, "款到发货订单,网上支付来源不为Bank和NoNeedPay");
            }

            #endregion

            #region 款到发货订单，验证网上支付金额是否足额
            if (entity.BaseInfo.ReceivableAmount > result.PayAmount)
            {
                ThrowException(entity, "款到发货订单,网上支付金额不足");
            }
            #endregion
            isRetry = false;
            return result;
        }

        /// <summary>
        /// 一单款到发货通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="soNetPayinfo"></param>
        private void PassPIASO(SOInfo entity, NetPayInfo soNetPayinfo)
        {
            //Jin:去掉了NETPAY状态需要为Origin的判断，因为前台网站重构后，对于网上支付，银行回调成功后直接就是审核通过的NETPAY和SOIncome
            //if (entity.BaseInfo.SOType != SOType.GroupBuy || soNetPayinfo.Status == NetPayStatus.Origin)
            //{
            //    ExternalDomainBroker.AuditNetPay(soNetPayinfo.SysNo.Value);
            //}
            //还原原始审核人员
            m_auditUserSysNoTemp = m_auditUserSysNo;

            if (entity.BaseInfo.SplitType == SOSplitType.Force || entity.BaseInfo.SplitType == SOSplitType.Customer)
            {
                ObjectFactory<SOProcessor>.Instance.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Split, SOInfo = entity, Parameter = new object[] { true } });
                m_da.UpdateCheckShippingAuditTypeBySysNo(entity.SysNo.Value, AuditType.Manual, "自动审单通过，自动拆单.");
                m_da.UpdateSO4AuditUserInfo(entity.SysNo.Value, m_auditUserSysNo);
                ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(new SOLogInfo() { SOSysNo = entity.BaseInfo.SysNo, OperationType = BizLogType.Sale_SO_Audit, Note = "自动审单通过，自动拆单." });
            }
            else
            {
                //更新时需要验证当前的SO状态

                //bool isSuccess = m_da.UpdateSO4PassAutoAudit(entity.SysNo.Value,m_auditUserSysNoTemp);
                ObjectFactory<SOProcessor>.Instance.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Audit, SOInfo = entity, Parameter = new object[] { true } });
                //if (isSuccess)
                //{
                    entity.BaseInfo.Status = SOStatus.WaitingOutStock;
                    m_da.UpdateCheckShippingAuditTypeBySysNo(entity.SysNo.Value, AuditType.Auto, "自动审单通过[款到发货]");
                    ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(new SOLogInfo() { SOSysNo = entity.BaseInfo.SysNo, OperationType = BizLogType.Sale_SO_Audit, Note = "自动审单通过[款到发货]" });
                //}
                //else
                //{
                //    ThrowException(entity, "订单不存在或状态已不再是待审核状态");
                //}
                //发送审核邮件
                ObjectFactory<SOProcessor>.Instance.SendMessage(entity, false, false, true);
                //更新自动审核发送邮件的标识
                m_da.AuditSendMailAndUpdateSO(entity.SysNo.Value);
            }
        }

        #endregion Check PIA

        /// <summary>
        /// 未通过验证规则时的处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="message"></param>
        private void ThrowException(SOInfo entity, string message)
        {
            string tmpErrorMessage = string.Format("自动审单未通过{0}: - {1}", entity.SysNo.Value, message);

            m_da.UpdateCheckShippingAuditTypeBySysNo(entity.SysNo.Value, AuditType.Manual, message);

            throw (new ApplicationException(tmpErrorMessage));
        }

        /// <summary>
        /// 一单审核结束
        /// </summary>
        /// <param name="entity"></param>
        private void EndAudit(SOInfo entity)
        {
            WriteLog(string.Format("自动审单通过:{0}", entity.SysNo));
        }

        /// <summary>
        /// 本批全部审核结束
        /// </summary>
        /// <param name="entity"></param>
        private void EndAuditAll()
        {
            WriteLog("本次审单结束");
        }

        /// <summary>
        /// 记录日志文件
        /// </summary>
        /// <param name="content"></param>
        private void WriteLog(string content)
        {

        }

        #region private class(主要用户临时访问数据)

        /// <summary>
        /// 自动审单控制开关
        /// </summary>
        class AutoAuditSwitch
        {
            public bool IsCheckKeyWords { get; set; }

            public bool IsCheckPayType { get; set; }

            public bool IsCheckShipType { get; set; }

            public bool IsCheckVAT { get; set; }

            public bool IsCheckProductType { get; set; }

            public bool IsCheckFPStatus { get; set; }

            public bool IsCheckCustomerType { get; set; }

            public bool IsCheckPointPromotion { get; set; }

            public bool IsCheckOrderAmt { get; set; }

            public bool IsCheckAutoAuditTime { get; set; }

            public bool IsCheckShipServiceType { get; set; }
        }

        /// <summary>
        /// 自动审单FP状态明细检查项开关
        /// </summary>
        class FPSwitch
        {
            /// <summary>
            /// 检测可以订单
            /// </summary>
            public bool IsCheckKY
            {
                get;
                set;
            }

            /// <summary>
            /// 检测串货订单
            /// </summary>
            public bool IsCheckCH
            {
                get;
                set;
            }

            /// <summary>
            /// 检测炒货订单
            /// </summary>
            public bool IsCheckCC
            {
                get;
                set;
            }

            /// <summary>
            /// 检测重复订单
            /// </summary>
            public bool IsCheckCF
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 自动审单客户类型明细检查项开关
        /// </summary>
        public class CustomerTypeSwitch
        {
            /// <summary>
            /// 检测正常客户
            /// </summary>
            public bool IsCheckCustomerZC
            {
                get;
                set;
            }

            /// <summary>
            /// 检测可以客户
            /// </summary>
            public bool IsCheckCustomerKY
            {
                get;
                set;
            }

            /// <summary>
            /// 检测欺诈客户
            /// </summary>
            public bool IsCheckCustomerQZ
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 获取会员等级及对应金额实体信息
        /// </summary>
        public class CustomerRankAmt
        {
            public int CustomerRank { get; set; }

            public decimal AmtLimit { get; set; }
        }

        #endregion
    }
}
