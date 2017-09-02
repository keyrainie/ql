using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.Common;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Biz
{
    
    public class SyncCommissionSettlementManager
    {
        List<IWorkItem> workItemList;
        Dictionary<string, object> contextData;
        List<ValidateResult> validateResultList;

        /// <summary>
        /// 创建工作流程
        /// </summary>
        public void CreateWorkFlow()
        {
            //0.Init work item list.
            workItemList = new List<IWorkItem>();
            validateResultList = new List<ValidateResult>(AppConst.InitSize_ValidateResultList);

            contextData = new Dictionary<string, object>();

            //<--
            //1.create work item.
            //2.set context data references.

            workItemList.Add(new SyncDataWorkItem(ref contextData));      //Init SO&RMA Data.

            //... ... can move to config.
            //-->

            //Init context data.
        }

        /// <summary>
        /// 财务对账
        /// </summary>
        public void ProcessSyncCommissionSettlement()
        {
            validateResultList.Clear();

            for (int i = 0; i < this.workItemList.Count; i++)
            {
                ValidateResult result = this.workItemList[i].ValidateData();
                validateResultList.Add(result);

                this.workItemList[i].ProcessWork();
            }
        }
    }
}
