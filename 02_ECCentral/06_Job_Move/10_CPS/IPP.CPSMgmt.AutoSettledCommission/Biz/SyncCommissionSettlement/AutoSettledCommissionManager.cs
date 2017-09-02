using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPPOversea.Invoicemgmt.AutoSettledCommission.Common;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{

    public class AutoSettledCommissionManager
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
            
            workItemList.Add(new SettledCommissionWorkItem(ref contextData));           //Sync SO&RMA Data to CPS.
            workItemList.Add(new CPSAutomationWorkItem(ref contextData));           //Sync SO&RMA Data to CPS.

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
