using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.Common;
//using IPP.ThirdPart.Interfaces.Contract;
using IPP.InventoryMgmt.JobV31.DataAccess;

namespace IPP.InventoryMgmt.JobV31.BIZ
{
    public abstract class ThirdPartInventoryBPBase
    {
        public ThirdPartInventoryBPBase(CommonConst Common)
        {
            Init(Common);
        }
        public ThirdPartInventoryBPBase() : this(new CommonConst()) { }

        private void Init(CommonConst Common)
        {
            OnRunningBefor = new ThirdPartInventoryRunning(On_RunningBefor);
            //OnRunning = new ThirdPartInventoryRunning(On_Running);
            OnRunningAfter = new ThirdPartInventoryRunning(On_RunningAfter);
            OnError = new ThirdPartInventoryRunningError(On_Error);
            ThirdPartInventory = ThirdPartInventoryDA.Query(Common);
            Count = ThirdPartInventory.Count;
            IsActive = true;
            this.Common = Common;
        }

        protected virtual void On_RunningBefor(object sender, ThirdPartInventoryArgs args) { }
        protected abstract void On_Running(object sender, ThirdPartInventoryArgs args);
        protected virtual void On_RunningAfter(object sender, ThirdPartInventoryArgs args) { }
        protected virtual void On_Error(object sender, ThirdPartInventoryErrorArgs args) { }

        public int Count { get; private set; }

        private List<ThirdPartInventoryEntity> ThirdPartInventory { get; set; }

        public event ThirdPartInventoryRunning OnRunningBefor;

        public event ThirdPartInventoryRunning OnRunningAfter;

        public event ThirdPartInventoryRunningError OnError;

        /// <summary>
        /// 标识当前线程是否处于活动状态
        /// </summary>
        private bool IsActive = true;

        protected CommonConst Common { get; private set; }

        /// <summary>
        /// 退出(终止)线程
        /// </summary>
        public void Abort()
        {
            IsActive = false;
        }
        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            IsActive = true;
            SynInvnetoryQty();
        }

        public void SynInvnetoryQty()
        {
            if (!IsActive)
            {
                throw new Exception("当前线程已终止，请调用Start()重新启动。");
            }

            ThirdPartInventoryArgs args = new ThirdPartInventoryArgs();
            //CommonConst commonConst = new CommonConst();

            int batchNumber = Common.BatchNumber;
            int batch = (batchNumber > ThirdPartInventory.Count) ? 1 : (ThirdPartInventory.Count % batchNumber == 0 ? ThirdPartInventory.Count / batchNumber : (ThirdPartInventory.Count / batchNumber + 1));
            List<ThirdPartInventoryEntity> qtylist;
            for (int i = 0; i < batch; i++)
            {
                qtylist = ThirdPartInventory.Skip(i * batchNumber).Take(batchNumber).ToList();
                args.ThirdPartInventoryList = qtylist;
                try
                {
                    OnRunningBefor(this, args);
                    if (!IsActive)
                    {
                        break;
                    }
                    On_Running(this, args);
                    if (!IsActive)
                    {
                        break;
                    }
                    OnRunningAfter(this, args);
                    if (!IsActive)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ThirdPartInventoryErrorArgs errorArgs = new ThirdPartInventoryErrorArgs();
                    errorArgs.Exception = ex;
                    errorArgs.ThirdPartInventoryList = qtylist;
                    OnError(this, errorArgs);
                    if (!IsActive)
                    {
                        break;
                    }
                }
            }

        }


    }
}
