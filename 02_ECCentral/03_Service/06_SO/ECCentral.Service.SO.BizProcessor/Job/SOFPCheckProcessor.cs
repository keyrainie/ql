using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor.Job
{
    [VersionExport(typeof(SOFPCheckProcessor))]
    public class SOFPCheckProcessor
    {

        #region Member

        private string m_interOrder;

        ISODA m_soDA = ObjectFactory<ISODA>.Instance;

        //最大执行数量
        int m_topCount;

        //公司编码
        string m_companyCode;

        //断货订单支持仓库
        List<int> m_outStockList;

        /// <summary>
        /// 忽略的CustomID集合
        /// </summary>
        public List<string> m_ignoreCustomIDList { get; set; }

        /// <summary>
        /// 是否进行可疑订单检测
        /// </summary>
        bool m_isCheckKeYi = false;

        /// <summary>
        /// 是否进行串货检测
        /// </summary> 
        bool m_isCheckChuanHuo = false;

        /// <summary>
        /// 是否进行炒货检测
        /// </summary>
        bool m_isCheckChaoHuo = false;

        /// <summary>
        /// 是否进行重复订单检测
        /// </summary>
        bool m_isCheckChongFu = false;

        /// <summary>
        /// 一个月内被物流拒收过单拒的客户编号
        /// </summary>
        List<SOInfo> m_autoRMACustomers;

        /// <summary>
        /// 支付方式列表
        /// </summary>
        List<PayType> m_payTypeList;

        /// <summary>
        ///  获取FPCheck检查项的具体明细项信息
        /// </summary>
        List<FPCheckItem> m_fPCheckItemList;

        /// <summary>
        /// 获取FPCheck检查项信息
        /// </summary>
        List<FPCheck> m_fPCheckMasterList;

        /// <summary>
        /// 验证串货的产品编号列表
        /// </summary>
        List<string> m_chuanHuoProductNoList;

        /// <summary>
        /// 验证串货的C3编号列表
        /// </summary>
        List<int> m_chuanHuoC3SysNoList;

        #endregion

        #region Entry Point

        /// <summary>
        /// 获取是否可疑订单的验证标识符
        /// </summary>
        /// <returns></returns>
        private bool GetFPCheckKeYiFlag()
        {
            bool result = false;
            if (m_fPCheckMasterList.Count > 0 && m_fPCheckMasterList.Find(x => { return (x.FPCheckCode == "KY" && x.FPCheckStatus == FPCheckStatus.A); }) != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取是否串单的验证标识符
        /// </summary>
        /// <returns></returns>
        bool GetFPCheckChuanHuoFlag()
        {
            bool result = false;
            if (m_fPCheckMasterList.Count > 0 && m_fPCheckMasterList.Find(x => { return (x.FPCheckCode == "CH" && x.FPCheckStatus == FPCheckStatus.A); }) != null)
            {
                result = true;
            }

            if (result)
            {
                m_chuanHuoProductNoList = new List<string>();
                m_fPCheckItemList.FindAll(x =>
                {
                    return (x.FPCheckItemDataType == "PID" && x.FPCheckItemStatus == FPCheckItemStatus.Invalid);
                }).ForEach(x =>
                {
                    m_chuanHuoProductNoList.Add(x.FPCheckItemDataValue);
                });

                m_chuanHuoC3SysNoList = new List<int>();
                m_fPCheckItemList.FindAll(x =>
                {
                    return (x.FPCheckItemDataType == "PC3" && x.FPCheckItemStatus == FPCheckItemStatus.Invalid);
                }).ForEach(x =>
                {
                    m_chuanHuoC3SysNoList.Add(Convert.ToInt32(x.FPCheckItemDataValue.Trim()));
                });
            }

            return result;
        }

        /// <summary>
        /// 获取是否进行炒单验证的标识符
        /// </summary>
        /// <returns></returns>
        bool GetFPCheckChaoHuoFlag()
        {
            bool result = false;
            if (m_fPCheckMasterList.Count > 0 && m_fPCheckMasterList.Find(x => { return (x.FPCheckCode == "CC" && x.FPCheckStatus == FPCheckStatus.A); }) != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取是否检测重复订单标识符
        /// </summary>
        /// <returns></returns>
        bool GetCheckChongFuFlag()
        {
            //判断是否进行炒货检测          
            bool result = false;
            if (m_fPCheckMasterList.Count > 0 && m_fPCheckMasterList.Find(x => { return (x.FPCheckCode == "CF" && x.FPCheckStatus == FPCheckStatus.A); }) != null)
            {
                result = true;
            }
            return result;
        }

        public void Check(string companyCode, List<string> ignoreCustomIDList, string interorder, List<int> outStockList)
        {
            Init(companyCode, ignoreCustomIDList,interorder, outStockList);
            Check();
        }

        private void Init(string companyCode, List<string> ignoreCustomIDList,string interorder, List<int> outStockList)
        {
            m_topCount = GetTopCount();
            m_companyCode = companyCode;
            m_outStockList = outStockList;
            m_interOrder = interorder;
            /*
            (x.CustomerID.StartsWith("AstraZeneca-")) ||
            (x.CustomerID.StartsWith("Shanda")) ||
            (x.CustomerID.StartsWith("konicaminolta")) ||
            (x.CustomerID.StartsWith("Ricoh-"))
             */
            m_ignoreCustomIDList = ignoreCustomIDList;
        }

        /// <summary>
        /// 获取一次最多的记录数
        /// </summary>
        /// <returns></returns>
        private int GetTopCount()
        {
            int result = 0;
            if (!int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_FPCheck_TopCount"), out result))
            {
                result = 2000;
            }
            return result;
        }

        /// <summary>
        /// 开始FP验证
        /// </summary>
        /// <param name="jobContext">Job运行上下文</param>
        void Check()
        {
            //提取验证的订单列表
            List<SOInfo> tmpSOENtityList = m_soDA.GetFPCheckSOList(m_topCount, m_companyCode);
            //无新单时退出
            if (null == tmpSOENtityList
                || tmpSOENtityList.Count == 0)
            {
                //jobContext.Message += "没有附合条件的订单记录" + Environment.NewLine;
                //WriteLog("没有附合条件的订单记录");
                return;
            }

            //提前取得所有支付方式的列表
            m_payTypeList = ExternalDomainBroker.GetPayTypeList(m_companyCode);

            //提前取得一个月内被物流拒收过单拒的客户编号
            m_autoRMACustomers = m_soDA.GetAutoRMASOInfoListInTime(m_companyCode);

            //提取FP检查项列表
            m_fPCheckMasterList = ExternalDomainBroker.GetFPCheckList(m_companyCode);

            //提取FP检查项列表明细项
            m_fPCheckItemList = new List<FPCheckItem>();
            m_fPCheckMasterList.ForEach(p => {
                m_fPCheckItemList.AddRange(p.FPCheckItemList);
            });

            //提前取得是否进行可疑订单检测是标识符
            m_isCheckKeYi = GetFPCheckKeYiFlag();

            //提前取得是否进行串单检测标识符
            m_isCheckChuanHuo = GetFPCheckChuanHuoFlag();

            //提前取得是否进行炒货订单检测标识符
            m_isCheckChaoHuo = GetFPCheckChaoHuoFlag();

            //提前获取是否进行重复订单检测标识符
            m_isCheckChongFu = GetCheckChongFuFlag();

            foreach (var x in tmpSOENtityList)
            {
                CheckSingle(x);
            }
        }

        /// <summary>
        /// 检查具体的一单
        /// </summary>
        /// <param name="soEntity4FPCheck"></param>
        void CheckSingle(SOInfo soEntity4FPCheck)
        {
            var custom = ExternalDomainBroker.GetCustomerBasicInfo(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value);

            //用户没有查询到不验证（可以以前的数据没有了）
            if (custom == null) return;
            //某些特定用户不用验证
            if (custom.FromLinkSource == m_interOrder
                || m_ignoreCustomIDList.Exists(p => custom.CustomerID.StartsWith(p)))
            {
                return;
            }

            List<string> tmpResons = new List<string>();

            FPStatus tmpFPstatus = FPStatus.Normal;
            bool IsMarkRed = false; //是否飘红可疑标记

            if (m_isCheckKeYi)
            {
                #region 检查疑单

                if (m_soDA.IsSpiteCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value, m_companyCode))
                {

                    tmpFPstatus = FPStatus.Suspect;
                    tmpResons.Add("此用户是恶意用户，之前有过不良的购物记录");
                    InsertKFC(soEntity4FPCheck);
                }
                else
                {
                    // 如果是货到付款
                    bool isRej = false;
                    if (m_payTypeList.Exists(item => item.SysNo == soEntity4FPCheck.BaseInfo.PayTypeSysNo && item.IsPayWhenRecv.HasValue && item.IsPayWhenRecv.Value))
                    {
                        bool isNewCustom = m_soDA.IsNewCustom(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value);
                        if (isNewCustom)
                        {
                            if (m_soDA.IsRejectionCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, soEntity4FPCheck.ReceiverInfo.Address, soEntity4FPCheck.ReceiverInfo.MobilePhone, soEntity4FPCheck.ReceiverInfo.Phone, m_companyCode))
                            {
                                isRej = true;
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                            }
                            else if (soEntity4FPCheck.ReceiverInfo.Phone.IndexOf(",") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceiverInfo.Phone.Split(',');
                                foreach (string sp in singlePhone)
                                {
                                    if (m_soDA.IsRejectionCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, "", "", sp, m_companyCode))
                                    {
                                        isRej = true;
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                                        break;
                                    }
                                }
                            }
                            else if (soEntity4FPCheck.ReceiverInfo.Phone.IndexOf("，") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceiverInfo.Phone.Split('，');
                                foreach (string sp in singlePhone)
                                {
                                    if (m_soDA.IsRejectionCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, "", "", sp, m_companyCode))
                                    {
                                        isRej = true;
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过拒收记录，请谨慎处理");
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_soDA.IsRejectionCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, "", "", "", m_companyCode))
                            {
                                isRej = true;
                                tmpFPstatus = FPStatus.Suspect;
                                if (m_soDA.IsNewRejectionCustomerB(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value))
                                {
                                    tmpResons.Add("新用户，该用户之前有过拒收货物的订单记录，请谨慎处理");
                                }
                                else
                                {
                                    tmpResons.Add("该用户拒收订单的比例超过限度，请谨慎处理");
                                }
                            }
                        }

                        if (isRej != true)
                        {
                            // 是新客户并且没有通过手机验证
                            if (isNewCustom && (!custom.CellPhoneConfirmed.HasValue || !custom.CellPhoneConfirmed.Value))
                            {

                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新客户");
                                tmpResons.Add("没有通过手机验证的货到付款订单");

                                if (soEntity4FPCheck.BaseInfo.SOAmount > 500)
                                {
                                    tmpFPstatus = FPStatus.Suspect;//标为可疑单
                                    tmpResons.Add("订单金额在500元以上");
                                    IsMarkRed = true; //设置可疑信息飘红
                                }
                                else
                                {
                                    tmpFPstatus = FPStatus.Suspect;//标为可疑单
                                    tmpResons.Add("订单金额在0-500元");
                                }

                                if (m_soDA.GetSOCount4OneDay(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value) > 1)
                                {
                                    tmpFPstatus = FPStatus.Suspect;
                                    tmpResons.Add("一天之中存在多个不同的收货地址的订单信息");
                                    IsMarkRed = true;
                                }
                            }
                            else if (m_autoRMACustomers.FirstOrDefault(z => { return z.BaseInfo.CustomerSysNo == soEntity4FPCheck.BaseInfo.CustomerSysNo; }) != null)
                            {
                                //一个月内物流拒收过此用户的订单标为可疑单
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("一个月内物流拒收过此用户的订单");
                                IsMarkRed = true;
                            }
                        }
                        else
                            InsertKFC(soEntity4FPCheck);
                    }
                    else  //款到发货
                    {
                        if (m_soDA.IsNewOccupyStockCustomerA(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value))
                        {
                            if (m_soDA.IsOccupyStockCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, soEntity4FPCheck.ReceiverInfo.Address, soEntity4FPCheck.ReceiverInfo.MobilePhone, soEntity4FPCheck.ReceiverInfo.Phone, m_companyCode))
                            {
                                tmpFPstatus = FPStatus.Suspect;
                                tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                InsertKFC(soEntity4FPCheck);
                            }
                            else if (soEntity4FPCheck.ReceiverInfo.Phone.IndexOf(",") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceiverInfo.Phone.Split(',');
                                foreach (string sp in singlePhone)
                                {
                                    if (m_soDA.IsOccupyStockCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, "", "", sp, m_companyCode))
                                    {
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                        InsertKFC(soEntity4FPCheck);
                                        break;
                                    }
                                }
                            }
                            else if (soEntity4FPCheck.ReceiverInfo.Phone.IndexOf("，") >= 0)
                            {
                                string[] singlePhone = soEntity4FPCheck.ReceiverInfo.Phone.Split('，');
                                foreach (string sp in singlePhone)
                                {
                                    if (m_soDA.IsOccupyStockCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, "", "", sp, m_companyCode))
                                    {
                                        tmpFPstatus = FPStatus.Suspect;
                                        tmpResons.Add("新用户，该用户的地址（或手机号码或固定电话）有过恶意占库存记录，请谨慎处理");
                                        InsertKFC(soEntity4FPCheck);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_soDA.IsOccupyStockCustomer(soEntity4FPCheck.BaseInfo.CustomerSysNo, null, null, null, m_companyCode))
                            {
                                InsertKFC(soEntity4FPCheck);
                                tmpFPstatus = FPStatus.Suspect;
                                if (m_soDA.IsNewOccupyStockCustomerB(soEntity4FPCheck.BaseInfo.CustomerSysNo.Value))
                                {
                                    tmpResons.Add("新用户，该用户之前有过连续作废订单记录，请谨慎处理");
                                }
                                else
                                {
                                    tmpResons.Add("该用户之前有过连续作废订单记录，请谨慎处理");
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #region 获取订单的Item列表信息
            List<SOItemInfo> tmpSingleSO = new List<SOItemInfo>();
            //订单状态必须为这几种：Origin WaitingOutStock  WaitingManagerAudit
            if (soEntity4FPCheck.BaseInfo.Status == SOStatus.Origin
                || soEntity4FPCheck.BaseInfo.Status == SOStatus.WaitingOutStock
                || soEntity4FPCheck.BaseInfo.Status == SOStatus.WaitingManagerAudit)
            {
                var itemList = m_soDA.GetSOItemsBySOSysNo(soEntity4FPCheck.SysNo.Value);
                tmpSingleSO.AddRange(itemList.Where(p => p.ProductType == SOProductType.Product));
            }

            if (tmpSingleSO.Count == 0)
            {
                tmpResons.Add("空单");
            }
            #endregion

            if (m_isCheckChuanHuo
                && tmpFPstatus != FPStatus.Suspect)//可疑优先于炒货,如果已经确认是可疑单,就不用再验证串货了
            {
                if (tmpSingleSO.Count > 0)
                {
                    string currentSOIPAddress = soEntity4FPCheck.ClientInfo.CustomerIPAddress;
                    DateTime currentSOCreateTime = soEntity4FPCheck.BaseInfo.CreateTime.Value;

                    #region 检查串货订单
                    //检查PRD
                    foreach (var item in tmpSingleSO)
                    {
                        //仅在遇到要求串货检查的Item时才去检测,减少数据库的访问次数
                        if (m_chuanHuoProductNoList.Find(t => { return (t == item.ProductID); }) != null)
                        {
                            var list = m_soDA.GetChuanHuoSOListByProduct(item.ProductSysNo.Value, currentSOIPAddress, currentSOCreateTime, m_companyCode);
                            if (list.Count > 1)
                            {
                                tmpFPstatus = FPStatus.ChuanHuo;
                                break;
                            }
                        }
                    }

                    //检查C3小类
                    List<int> chuanHuoSOSysNosByC3 = (from a in m_chuanHuoC3SysNoList
                                                      from b in tmpSingleSO
                                                      where a == b.C3SysNo
                                                      select a).ToList();

                    foreach (int c3No in chuanHuoSOSysNosByC3)
                    {
                        var list = m_soDA.GetChuanHuoSOListByC3(c3No, currentSOIPAddress, currentSOCreateTime, m_companyCode);
                        if (list.Count > 1)
                        {
                            tmpFPstatus = FPStatus.ChuanHuo;
                            break;
                        }
                    }

                    if (tmpFPstatus == FPStatus.ChuanHuo)
                    {
                        tmpResons.Add("串货订单");
                    }
                }
                    #endregion
            }

            if (m_isCheckChongFu)
            {
                #region 检查重复订单
                if (tmpSingleSO.Count > 0)
                {
                    foreach (var item in tmpSingleSO)
                    {
                        var tmpDuplicateSOs = m_soDA.GetDuplicatSOList(item.ProductSysNo.Value, soEntity4FPCheck.BaseInfo.CustomerSysNo.Value, soEntity4FPCheck.BaseInfo.CreateTime.Value, m_companyCode);
                        if (tmpDuplicateSOs.Count > 1)
                        {
                            string duplicateMarket = "重复订单";
                            if (!tmpResons.Contains(duplicateMarket))
                            {
                                tmpResons.Add(duplicateMarket);
                            }

                            //StringBuilder tmpDuplicateSOSysNosb = new StringBuilder();
                            //foreach (var y in tmpDuplicateSOs)
                            //{
                            //    tmpDuplicateSOSysNosb.Append(string.Format("{0},", y.SysNo));
                            //    if (tmpDuplicateSOSysNosb.Length > 400)
                            //        break;
                            //}
                            //tmpDuplicateSOSysNoString = tmpDuplicateSOSysNosb.ToString().TrimEnd(",".ToCharArray());

                            //只获取30个订单即可，替换原来的长度切断(为什么有更新的限制，暂时去掉)
                            string tmpDuplicateSOSysNoString = string.Join(",", tmpDuplicateSOs.Select(p => p.SysNo.Value));

                            m_soDA.UpdateMarkException(tmpDuplicateSOSysNoString, item.ProductSysNo.Value, tmpDuplicateSOSysNoString);
                        }
                    }
                }
                #endregion
            }

            if (m_isCheckChaoHuo
                && (!string.IsNullOrEmpty(soEntity4FPCheck.ReceiverInfo.MobilePhone) || !string.IsNullOrEmpty(soEntity4FPCheck.ReceiverInfo.Phone))
                )
            {
                #region 检查炒货订单

                #region PreData
                int SysNoCount = 3;//订单数量 (默认需要大于的最小订单数量为1)
                int hoursLimit = 24; //需从配置表中读取

                var fPCheckItemEntityTemp = m_fPCheckItemList.Find(t => { return (t.FPCheckItemDataType == "小时之内订单数量大于" && t.FPCheckItemStatus == FPCheckItemStatus.Invalid); });
                if (fPCheckItemEntityTemp != null)
                {
                    if (fPCheckItemEntityTemp.FPCheckItemDataValue != string.Empty)
                    {
                        hoursLimit = Convert.ToInt32(fPCheckItemEntityTemp.FPCheckItemDataValue.Split('|')[0].ToString());
                        SysNoCount = Convert.ToInt32(fPCheckItemEntityTemp.FPCheckItemDataValue.Split('|')[1].ToString());
                    }
                }

                int? PointPromotionFlag = null;//判断是否进行订单优惠券积分的,检测标识符,赋任何值表示此条件有效;
                int? ShipPriceFlag = null;//判断是否进行订单中运费金额为0的,检测标识符,赋任何值表示此条件有效;
                int? IsVATFlag = null;//判断是否进行订单中勾选开具增值税发票的,检测标识符,赋任何值表示此条件有效;

                if (m_fPCheckItemList.Count > 0 && m_fPCheckItemList.Find(t => { return (t.FPCheckItemDataType == "订单中使用优惠券/积分等优惠" && t.FPCheckItemStatus == FPCheckItemStatus.Invalid); }) != null)
                {
                    PointPromotionFlag = 1;
                }
                if (m_fPCheckItemList.Count > 0 && m_fPCheckItemList.Find(t => { return (t.FPCheckItemDataType == "订单中运费金额为0" && t.FPCheckItemStatus == FPCheckItemStatus.Invalid); }) != null)
                {
                    ShipPriceFlag = 1;
                }
                if (m_fPCheckItemList.Count > 0 && m_fPCheckItemList.Find(t => { return (t.FPCheckItemDataType == "订单中勾选开具增值税发票" && t.FPCheckItemStatus == FPCheckItemStatus.Invalid); }) != null)
                {
                    IsVATFlag = 1;
                }
                #endregion

                var chaoHuoList = m_soDA.GetChaoHuoSOList(
                      soEntity4FPCheck.ReceiverInfo.MobilePhone
                    , soEntity4FPCheck.ReceiverInfo.Phone
                    , hoursLimit
                    , soEntity4FPCheck.BaseInfo.CreateTime.Value
                    , PointPromotionFlag
                    , ShipPriceFlag
                    , IsVATFlag
                    , m_companyCode
                    );

                if (chaoHuoList.Count > SysNoCount)
                {
                    if (tmpResons.Contains("重复订单"))
                    {
                        tmpResons.Clear();
                    }

                    tmpFPstatus = FPStatus.ChaoHuo;

                    #region //收集炒货的订单号
                    int lenReasons = 0;

                    tmpResons.ForEach(x =>
                    {
                        lenReasons += x.Length;
                    }
                    );

                    lenReasons += tmpResons.Count;

                    StringBuilder tmpChaoHuoSOSysNosb = new StringBuilder();
                    string tmpChaoHuoSysNoString = string.Empty;
                    string ChaoHuoSysNo;

                    foreach (var x in chaoHuoList)
                    {
                        ChaoHuoSysNo = string.Format("{0},", x.SysNo);
                        if ((lenReasons + tmpChaoHuoSOSysNosb.Length + ChaoHuoSysNo.Length - 1) > 200)
                        {
                            break;
                        }
                        tmpChaoHuoSOSysNosb.Append(ChaoHuoSysNo);
                    }

                    tmpChaoHuoSysNoString = tmpChaoHuoSOSysNosb.ToString().TrimEnd(",".ToCharArray());

                    tmpResons.Add(tmpChaoHuoSysNoString);
                    #endregion

                    string soSysNos = string.Join(",", chaoHuoList.Select(p => p.SysNo.Value));
                    m_soDA.UpdateMarkFPStatus(soSysNos, (int)tmpFPstatus, tmpChaoHuoSysNoString, false);
                }
                #endregion
            }

            #region 检查本地仓断货订单
            if (m_outStockList.Count > 0)
            {
                string localWH = ExternalDomainBroker.GetLocalWarehouseNumber(soEntity4FPCheck.ReceiverInfo.AreaSysNo.Value, m_companyCode);
                if (!string.IsNullOrEmpty(localWH)
                    && m_outStockList.Exists(os => os.ToString() == localWH)
                    && m_soDA.CountNotLocalWHSOItem(soEntity4FPCheck.SysNo.Value, localWH) > 0)
                {
                    m_soDA.UpdateLocalWHMark(soEntity4FPCheck.SysNo.Value, localWH, 1);
                }
            }

            #endregion

            if (tmpFPstatus == FPStatus.Normal
                && tmpResons.Count == 0)
            {
                tmpResons.Add("正常单");
            }

            string temReson = string.Empty;
            foreach (string reson in tmpResons)
            {
                temReson += reson + ";";
            }
            temReson = temReson.TrimEnd(';');

            m_soDA.UpdateMarkFPStatus(soEntity4FPCheck.SysNo.ToString(), (int)tmpFPstatus, temReson, IsMarkRed);
        }

        private void InsertKFC(SOInfo entity)
        {
            KnownFraudCustomer req = new KnownFraudCustomer
            {
                CreateDate = DateTime.Now,
                CreateUserName = "OrderJob",
                CustomerSysNo = entity.BaseInfo.CustomerSysNo,
                KFCType = KFCType.KeYi,
                IPAddress = entity.ClientInfo.CustomerIPAddress,
                MobilePhone = entity.ReceiverInfo.MobilePhone,
                ShippingAddress = entity.ReceiverInfo.Address,
                ShippingContact = entity.ReceiverInfo.Name,
                Status = FPStatus.Normal,
                Telephone = entity.ReceiverInfo.Phone,
                CompanyCode = m_companyCode
            };
            ObjectFactory<ISOKFCDA>.Instance.InsertKnowFrandCustomer(req);
        }

        #endregion Entry Point
    }
}
