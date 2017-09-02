using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.Invoicemgmt.AutoSettledCommission.Model;
using IPPOversea.Invoicemgmt.AutoSettledCommission.DAL;
using System.Text.RegularExpressions;
using Newegg.Oversea.Framework.ExceptionBase;
using System.Transactions;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{
    public class CPSAutomationWorkItem : IWorkItem
    {
        private const string EmailPattern = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        private static Regex regex = new Regex(EmailPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private TxtFileLoger log = new TxtFileLoger("SyncCommissionSettlement");
        private Dictionary<string, object> contextData;

        public CPSAutomationWorkItem(ref Dictionary<string, object> contextData)
        {
            this.contextData = contextData;
        }

        #region IWorkItem Members

        public Dictionary<string, object> ContextData
        {
            get { return this.contextData; }
        }

        public ValidateResult ValidateData()
        {
            return new ValidateResult { IsPass = true };
        }

        public void ProcessWork()
        {
            //1.获取时间
            DateTime now = Settings.SettledDate;
            int day = 20;
            if (Settings.SettledDay.HasValue)
            {
                day = Settings.SettledDay.Value;
            }
            if (now.Day != day)
            {   //默认每月20号结算
#if !Test
                return;
#endif
            }

            // 已结算但未付款
            // 且无兑现单
            List<CommissionSettlementEntity> list = SyncAutomationDAL.GetUnApplyedCommissionSettlement();
            if (list == null || list.Count == 0)
            {
                return;
            }

            Dictionary<int, List<CommissionSettlementEntity>> userCSList = GroupbyCustomer(list);
            foreach(KeyValuePair<int, List<CommissionSettlementEntity>> key in userCSList)
            {
                // 计算 CommissionAmt 总金额
                List<CommissionSettlementEntity> userEntityList = key.Value;
                decimal? total = userEntityList.Sum(x => x.CommissionAmt);
                if (total.HasValue && total.Value >= 100)
                {
                    // 获取用户信息、帐户信息
                    UserInfo userInfo = SyncAutomationDAL.GetUserInfo(key.Key);
                    if (userInfo == null)
                    {
                        continue;
                    }

                    // 总金额大于100,自动申请兑现单
                    try
                    {
                        RequestCashPay(userInfo, userEntityList, total.Value);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 按照用户分组
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Dictionary<int, List<CommissionSettlementEntity>> GroupbyCustomer(List<CommissionSettlementEntity> list)
        {
            Dictionary<int, List<CommissionSettlementEntity>> groupbyCSList = new Dictionary<int, List<CommissionSettlementEntity>>();

            int lastUserSysNo = -1;
            List<CommissionSettlementEntity> userEntityList = null;

            foreach (CommissionSettlementEntity entity in list)
            {
                if (lastUserSysNo == entity.UserSysNo)
                {
                    userEntityList.Add(entity);
                }
                else
                {
                    userEntityList = new List<CommissionSettlementEntity>();
                    userEntityList.Add(entity);

                    groupbyCSList.Add(entity.UserSysNo, userEntityList);
                }

                lastUserSysNo = entity.UserSysNo;
            }

            return groupbyCSList;
        }
        #endregion

        #region 发送邮件
        private void SendMail(UserInfo userInfo)
        {
            if (!IsEmail(userInfo.Email))
            {
                return;
            }

            try
            {
                MailDAL.SendEmail(userInfo.Email, "aaaaaaaa", "bbbbbbbbb");
            }
            catch
            {

            }
        }

        private bool CheckApplyValid(UserInfo userEntity)
        {

            if (string.IsNullOrEmpty(userEntity.BankCode)
                || string.IsNullOrEmpty(userEntity.BankName)
                || string.IsNullOrEmpty(userEntity.BranchBank)
                || string.IsNullOrEmpty(userEntity.BankCardNumber)
                || string.IsNullOrEmpty(userEntity.ReceivableName)
                )
            {
                return false;
            }




            return true;
        }

        /// <summary>
        /// 判断字符串是否是Email地址。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <returns>如果 <paramref name="value"/> 字符串是Email地址，则返回 <b>true</b>; 反之返回 <b>false</b>。</returns>
        public static bool IsEmail(string value)
        {
            return regex.IsMatch(value);
        }
        #endregion

        #region 申请兑现单
        private void RequestCashPay(UserInfo userInfo, List<CommissionSettlementEntity> commissionList, decimal requestAmount)
        {
            //验证数据
            //准备数据，获取用户收款信息
            CommissionToCashRecordEntity data = CheckAndPrepareData(userInfo, commissionList, requestAmount, userInfo.CanProvideInvoice);

            StringBuilder reqCashSysNoBuilder = new StringBuilder();
            commissionList.ForEach(x =>
            {
                reqCashSysNoBuilder.Append(x.SysNo);
                reqCashSysNoBuilder.Append(",");
            });

            string toCashSysNos = reqCashSysNoBuilder.ToString().TrimEnd(',');

            //更新数据
            //1.创建兑现申请单
            //2.修改对应的佣金结算单信息【CommissionToCashRecordSysNo】
            int toCashSysNo = ProcessCreateRequestToCashRecord(toCashSysNos, data);
        }

        private const string BLANK_INFO = "未填写";
        /// <summary>
        /// 数据合法性 & 准备数据
        /// </summary>
        /// <param name="toCashSysNos"></param>
        private CommissionToCashRecordEntity CheckAndPrepareData(UserInfo userInfo, List<CommissionSettlementEntity> commissionSettlementEntitys, decimal requestAmount, string isHasInvoice)
        {
            //1.获取用户信息
            if (string.IsNullOrEmpty(userInfo.BankCode)
                || string.IsNullOrEmpty(userInfo.BankName)
                || string.IsNullOrEmpty(userInfo.BranchBank)
                || string.IsNullOrEmpty(userInfo.BankCardNumber)
                || string.IsNullOrEmpty(userInfo.ReceivableName)
                )
            {
                throw new BusinessException("您的收款账户信息不完整，请完善收款信息后再提交兑现申请！");
            }

            //2.计算兑现佣金金额
            //3.校验账户余额
            decimal unRequestAmt = SyncAutomationDAL.GetUnRequestCommissionSettlementAmt(userInfo.SysNo);          //获取当前用户已结算单是为付款的金额

            if (userInfo.BalanceAmt < 0)
            {
                string msg = string.Format("申请兑现时，账户余额不能少于0元！");
                throw new BusinessException(msg);
            }

            if (userInfo.BalanceAmt + requestAmount + unRequestAmt < 0)
            {
                string msg = string.Format("申请兑现后的余额账户不能少于0元！账户余额{0}元,本次申请金额{1}元,未提交申请{2}元", userInfo.BalanceAmt, requestAmount, unRequestAmt);
                throw new BusinessException(msg);
            }

            //4.设置数据
            CommissionToCashRecordEntity recordEntity = new CommissionToCashRecordEntity
            {
                UserSysNo = userInfo.SysNo,
                Status = "R",
                ToCashAmt = requestAmount,
                BankCode = userInfo.BankCode,
                BankName = userInfo.BankName,
                BranchBank = userInfo.BranchBank,
                BankCardNumber = userInfo.BankCardNumber,
                ReceivableName = userInfo.ReceivableName,
                IsHasInvoice = isHasInvoice,
                InUser = userInfo.SysNo.ToString()
            };

            return recordEntity;
        }

        private static int ProcessCreateRequestToCashRecord(string toCashSysNos, CommissionToCashRecordEntity data)
        {
            int toCashRecordSysNo = 0;

            //开启事务
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if (data.IsHasInvoice == "N")
                {   //不能提供发票，扣除个人所得税。
                    data.AfterTaxAmt = CalculationPersonalIncomeTax(data.ToCashAmt);      //计算去税金额
                }
                toCashRecordSysNo = SyncAutomationDAL.CreateRequestToCashRecord(data);
                if (toCashRecordSysNo <= 0)
                {
                    return toCashRecordSysNo;
                }
                bool isSuccess = SyncAutomationDAL.UpdateCommissionSettlement(toCashRecordSysNo, toCashSysNos);
                if (!isSuccess)
                {
                    return toCashRecordSysNo;
                }
                scope.Complete();
            }

            return toCashRecordSysNo;
        }

        /// <summary>
        /// 计算个人所得税
        /// </summary>
        /// <param name="originalAmt">原始金额</param>
        /// <returns></returns>
        private static decimal CalculationPersonalIncomeTax(decimal originalAmt)
        {
            decimal afterTaxAmt = 0.00m;
            decimal taxAmt = 0.00m;         //税费
            decimal taxableIncome = 0.00m;  //应纳税所得额


            //计算应纳税所得额
            if (originalAmt <= 800)
            {
                taxableIncome = 0;
            }
            else if (originalAmt > 800 && originalAmt <= 4000)
            {
                taxableIncome = originalAmt - 800;
            }
            else if (originalAmt > 4000)
            {
                taxableIncome = originalAmt * 0.8m;
            }

            //计算个税
            if (taxableIncome <= 20000)
            {
                taxAmt = taxableIncome * 0.2m;
            }
            else if (taxableIncome > 20000 && taxableIncome <= 50000)
            {
                taxAmt = taxableIncome * 0.3m - 2000m;
            }
            else if (taxableIncome > 50000)
            {
                taxAmt = taxableIncome * 0.4m - 7000m;
            }

            afterTaxAmt = originalAmt - taxAmt;

            return afterTaxAmt;
        }
        #endregion
    }
}
