using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(PriceChangeAppService))]
    public class PriceChangeAppService
    {
        public int SavePriceChange(PriceChangeMaster item)
        {
            return ObjectFactory<PriceChangeProcessor>.Instance.SavePriceChange(item);
        }

        public string ClonePriceChange(List<int> sysNos)
        {
            List<BatchActionItem<int>> items = sysNos.Select(p => new BatchActionItem<int>
            {
                ID = p.ToString(),
                Data = p
            }).ToList();

            var bp = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<int> resutl = BatchActionManager.DoBatchAction<int>(items, (sysno) =>
            {
                PriceChangeMaster item = GetPriceChangeBySysNo(sysno);

                if (item == null)
                {
                    //throw new BizException(string.Format("没有找到记录编号为{0}的记录", sysno));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("Invoice.PriceChange","PriceChange_NotFounTheRecord"), sysno));
                }

                if (item.Status != RequestPriceStatus.Finished && item.Status != RequestPriceStatus.Aborted)
                {
                    //throw new BizException(string.Format("记录编号为{0}的记录的状态不是终止状态或已完成状态", sysno));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("Invoice.PriceChange", "PriceChange_NotStopOrFinshedStatus"), sysno));
                }

                item.Status = RequestPriceStatus.Auditting;
                bp.ClonePriceChange(item);

            });

            return resutl.PromptMessage;
        }

        public PriceChangeMaster GetPriceChangeBySysNo(int sysno)
        {
            return ObjectFactory<PriceChangeProcessor>.Instance.GetPriceChangeBySysNo(sysno);
        }

        public List<PriceChangeMaster> GetPriceChangeByStatus(RequestPriceStatus status)
        {
            return ObjectFactory<PriceChangeProcessor>.Instance.GetPriceChangeByStatus(status);
        }

        public PriceChangeMaster UpdatePriceChange(PriceChangeMaster newItem)
        {
            ObjectFactory<PriceChangeProcessor>.Instance.UpdatePriceChange(newItem);

            return GetPriceChangeBySysNo(newItem.SysNo);
        }

        public string BatchAuditPriceChange(Dictionary<int,string> sysNos)
        {
            List<BatchActionItem<PriceChangeMaster>> items = sysNos.Select(p => new BatchActionItem<PriceChangeMaster>
            {
                ID = p.ToString(),
                Data = new PriceChangeMaster { SysNo = p.Key, AuditMemo = p.Value }
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> resutl = BatchActionManager.DoBatchAction(items, (request) => 
            {
                bl.AuditPriceChange(request);
            });

            return resutl.PromptMessage;
        }

        public string BatchVoidPriceChange(List<int> sysNos)
        {
            List<BatchActionItem<PriceChangeMaster>> items = sysNos.Select(p => new BatchActionItem<PriceChangeMaster>
            {
                ID = p.ToString(),
                Data = new PriceChangeMaster { SysNo = p }
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> resutl = BatchActionManager.DoBatchAction(items, (request) =>
            {
                bl.VoidPriceChange(request);
            });

            return resutl.PromptMessage;
        }

        public VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<PriceChangeProcessor>.Instance.GetVendorBasicInfoBySysNo(vendorSysNo);
        }

        /// <summary>
        /// 人工启动变价单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public string BatchRunPriceChangeByManual(List<int> sysNo)
        {
            List<PriceChangeMaster> infos = new List<PriceChangeMaster>();

            sysNo.ForEach(p => 
            {
                infos.Add(GetPriceChangeBySysNo(p));
            });

            List<BatchActionItem<PriceChangeMaster>> items = infos.Select(p => new BatchActionItem<PriceChangeMaster>
            {
                ID = p.SysNo.ToString(),
                Data = p
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> result = BatchActionManager.DoBatchAction(items, (request) =>
            {
                bl.RunPriceChange(request, false);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// 人工中止变价单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public string BatchAbortedPriceChangeByManual(List<int> sysNo)
        {
            List<PriceChangeMaster> infos = new List<PriceChangeMaster>();

            sysNo.ForEach(p =>
            {
                infos.Add(GetPriceChangeBySysNo(p));
            });

            List<BatchActionItem<PriceChangeMaster>> items = infos.Select(p => new BatchActionItem<PriceChangeMaster>
            {
                ID = p.SysNo.ToString(),
                Data = p
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> result = BatchActionManager.DoBatchAction(items, (request) =>
            {
                bl.AbortedPriceChange(request, false);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// Job启动变价单
        /// </summary>
        /// <returns></returns>
        public string BatchRunPriceChangeByJob()
        {
            List<PriceChangeMaster> infos = ObjectFactory<PriceChangeProcessor>.Instance.GetAuditedPriceChangeInfos();

            List<BatchActionItem<PriceChangeMaster>> items = infos.Select(p => new BatchActionItem<PriceChangeMaster> 
            {
                ID = p.SysNo.ToString(),
                Data = p
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> result = BatchActionManager.DoBatchAction(items, (request) => 
            {
                bl.RunPriceChange(request, true);
            });

            return result.PromptMessage;
        }

        /// <summary>
        /// Job中止变价单
        /// </summary>
        /// <returns></returns>
        public string BatchAbortedPriceChangeByJob()
        {
            List<PriceChangeMaster> infos = ObjectFactory<PriceChangeProcessor>.Instance.GetRunningProceChangeInfos();

            List<BatchActionItem<PriceChangeMaster>> items = infos.Select(p => new BatchActionItem<PriceChangeMaster>
            {
                ID = p.SysNo.ToString(),
                Data = p
            }).ToList();

            var bl = ObjectFactory<PriceChangeProcessor>.Instance;

            BatchActionResult<PriceChangeMaster> result = BatchActionManager.DoBatchAction(items, (request) =>
            {
                bl.AbortedPriceChange(request, true);
            });

            return result.PromptMessage;
        }
        
    }
}
