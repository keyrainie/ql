using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.DAL;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Biz
{
    /// <summary>
    /// 同步数据Job
    /// </summary>
    public class SyncDataWorkItem : IWorkItem
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
        public SyncDataWorkItem(ref Dictionary<string, object> contextData)
        {
            this.contextData = contextData;
        }


        #region IWorkItem Members
        /// <summary>
        /// 操作
        /// </summary>
        public void ProcessWork()
        {
            //0.获取佣金结算单据SO & RMA
            List<CommissionSettlementItemEntity> commItemEntitys_Create = SyncCommissionSettlementDAL.GetNeedToSynchronizeCreate();
            List<CommissionSettlementItemEntity> commItemEntitys_Update = SyncCommissionSettlementDAL.GetNeedToSynchronizeUpdate();

            //1.同步创建 CommItem中没有的数据
            CreateNeedToSynchronize(commItemEntitys_Create);

            //2.同步更新 CommItem中存在的数据
            UpdateNeedToSynchronize(commItemEntitys_Update);
            
            //3.同步作废CPS单据【SO & RMA 被作废】
            SyncCommissionSettlementDAL.VoidCommissionSettlementItem(Settings.SyncCount);

            SyncCommissionSettlementDAL.UpdateCommissionSettlementItemOrder(Settings.SyncCount);
        }

        private void CreateNeedToSynchronize(List<CommissionSettlementItemEntity> commItemEntitys)
        {
            for (int i = 0; i < commItemEntitys.Count; i++)
            {
                commItemEntitys[i].OrderProductList = new List<OrderProductEntity>();
                if (commItemEntitys[i].Type == "SO")
                {
                    commItemEntitys[i].OrderProductList = SyncCommissionSettlementDAL.GetItemSO(commItemEntitys[i].OrderSysNo);
                }
                else if (commItemEntitys[i].Type == "RMA")
                {
                    commItemEntitys[i].OrderProductList = SyncCommissionSettlementDAL.GetItemRMA(commItemEntitys[i].OrderSysNo);
                }
            }

            //1.计算佣金
            //2.写入数据到表
            for (int i = 0; i < commItemEntitys.Count; i++)
            {
                //commItemEntitys_Create[i].OrderProductList = new List<OrderProductEntity>();
                commItemEntitys[i].CommissionAmt = Math.Abs(commItemEntitys[i].OrderProductList.Sum(x => x.Price * x.Quantity * x.Percentage));
                bool isSuccess = SyncCommissionSettlementDAL.CreateCommissionSettlementItem(commItemEntitys[i]);
                if (!isSuccess)
                {
                    log.WriteLog(string.Format("[Create]同步CPS单据失败！单据编号{0}，单据类型{1}", commItemEntitys[i].OrderSysNo, commItemEntitys[i].Type));
                }
            }
        }

        private void UpdateNeedToSynchronize(List<CommissionSettlementItemEntity> commItemEntitys)
        {
            for (int i = 0; i < commItemEntitys.Count; i++)
            {
                commItemEntitys[i].OrderProductList = new List<OrderProductEntity>();
                if (commItemEntitys[i].Type == "SO")
                {
                    commItemEntitys[i].OrderProductList = SyncCommissionSettlementDAL.GetItemSO(commItemEntitys[i].OrderSysNo);
                }
                else if (commItemEntitys[i].Type == "RMA")
                {
                    
                    commItemEntitys[i].OrderProductList = SyncCommissionSettlementDAL.GetItemRMA(commItemEntitys[i].OrderSysNo);
                }

                //1.计算佣金
                //2.更新数据到表
                commItemEntitys[i].CommissionAmt = commItemEntitys[i].OrderProductList.Sum(x => x.Price * x.Quantity * x.Percentage);
                bool isSuccess = SyncCommissionSettlementDAL.UpdateCommissionSettlementItem(commItemEntitys[i]);
                if (!isSuccess)
                {
                    log.WriteLog(string.Format("[Update]同步CPS单据失败！单据编号{0}，单据类型{1}", commItemEntitys[i].OrderSysNo, commItemEntitys[i].Type));
                }
            }
        }

        /// <summary>
        /// 校验
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
