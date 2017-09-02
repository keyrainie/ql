using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.Invoicemgmt.AutoSettledCommission.DAL;
using IPPOversea.Invoicemgmt.AutoSettledCommission.Model;
using System.Transactions;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{
    /// <summary>
    /// 结算佣金
    /// </summary>
    public class SettledCommissionWorkItem : IWorkItem
    {
        #region
        private TxtFileLoger log = new TxtFileLoger("SyncCommissionSettlement");
        private Dictionary<string, object> contextData;
        /// <summary>
        /// 获取上下文数据
        /// </summary>
        public Dictionary<string, object> ContextData
        {
            get
            {
                return this.contextData;
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contextData">初始化上下文</param>
        public SettledCommissionWorkItem(ref Dictionary<string, object> contextData)
        {
            this.contextData = contextData;
        }


        #region IWorkItem Members


        /// <summary>
        /// 
        /// </summary>
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
                return;
            }
            DateTime endDate = new DateTime(now.Year, now.Month, 1);
            DateTime beginDate = endDate.AddMonths(-1);

            //2.获取结算用户
            List<UserEntity> userEntitys = SettledCommissionDAL.GetSettledUserList(beginDate, endDate);
            //3.依次结算
            for (int i = 0; i < userEntitys.Count; i++)
            {
                //1>获取佣金结算单信息
                int userSysNo = userEntitys[i].UserSysNo;
                List<CommissionSettlementItemEntity> commItemEntitys = SettledCommissionDAL.CommissionSettlementItemBySysNo(userSysNo, beginDate, endDate);

                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = TransactionManager.DefaultTimeout;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    int commSysNo = SyncCommissionSettlementDAL.CreateCommissionSettlement(new CommissionSettlementEntity
                    {
                        UserSysNo = userEntitys[i].UserSysNo,
                        SettledBeginDate = beginDate,
                        SettledEndDate = endDate,
                        InUser = "System Job"
                    });
                    if (commSysNo == 0)
                    {
                        continue;
                    }

                    //2>重新计算佣金
                    foreach (CommissionSettlementItemEntity entity in commItemEntitys)
                    {
                        entity.OrderProductList = new List<OrderProductEntity>();
                        if (entity.Type == "SO")
                        {
                            entity.OrderProductList = SyncCommissionSettlementDAL.GetItemSO(entity.OrderSysNo);

                        }
                        else if (entity.Type == "RMA")
                        {
                            entity.OrderProductList = SyncCommissionSettlementDAL.GetItemRMA(entity.OrderSysNo);
                        }

                        entity.CommissionAmt = entity.OrderProductList.Sum(x => x.Price * x.Quantity * x.Percentage);
                        entity.Status = "S";
                        entity.CommissionSettlementSysNo = commSysNo;
                        bool isSuccess = SettledCommissionDAL.UpdateCommissionSettlementItem(entity);
                        if (!isSuccess)
                        {
                            log.WriteLog(string.Format("结算CPS单据失败！单据编号{0}，单据类型{1}", entity.OrderSysNo, entity.Type));
                        }
                    }
                    decimal totalAmt = 0.00m;

                    commItemEntitys.ForEach(x =>
                        {
                            if (x.Type == "SO")
                            {
                                totalAmt += x.CommissionAmt;
                            }
                            if (x.Type == "RMA")
                            {
                                totalAmt -= x.CommissionAmt;
                            }
                        });

                    bool isUpdate = SyncCommissionSettlementDAL.UpdateCommissionSettlement(commSysNo, totalAmt);
                    if (!isUpdate)
                    {
                        log.WriteLog(string.Format("更新结算单失败！单据编号{0}", commSysNo));
                        continue;
                    }
                    //修改用户余额
                    bool isAdd = SyncCommissionSettlementDAL.AddUserBalanceAmt(userSysNo, totalAmt);

                    if (!isAdd)
                    {
                        log.WriteLog(string.Format("添加用户余额失败！用户编号{0}，余额{1}", userSysNo, totalAmt));
                        continue;
                    }
                    
                    scope.Complete();
                }





                //3>记录数据
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ValidateResult ValidateData()
        {
            return new ValidateResult { IsPass = true };
        }

        #endregion IWorkItem Members

        #region Biz Process

        #endregion Biz Process
    }
}
