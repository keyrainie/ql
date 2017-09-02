using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using System.Transactions;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(HotSaleCategoryAppService))]
    public class HotSaleCategoryAppService
    {
        /// <summary>
        /// 获取首页排行的位置
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <param name="pageType">页面类型</param>
        /// <returns>位置列表</returns>
        public List<CodeNamePair> GetPosition(string companyCode, string channelID, int pageType)
        {
            return _hotProcessor.GetPosition(companyCode, channelID, pageType);
        }
        private HotSaleCategoryProcessor _hotProcessor = ObjectFactory<HotSaleCategoryProcessor>.Instance;

        public void Insert(HotSaleCategory msg)
        {
            _hotProcessor.Insert(msg);
        }

        public void Update(HotSaleCategory msg)
        {
            _hotProcessor.Update(msg);
        }

        public void Delete(int sysNo)
        {
            _hotProcessor.Delete(sysNo);
        }

        public HotSaleCategory Load(int sysNo)
        {
            return _hotProcessor.Load(sysNo);
        }

        public void UpdateSameGroupAll(HotSaleCategory msg)
        {
            var sameGroupRecords = _hotProcessor.GetSameGroupOtherRecords(msg.SysNo.Value);
            using (TransactionScope ts = new TransactionScope())
            {
                _hotProcessor.Update(msg);
                foreach (var item in sameGroupRecords)
                {
                    //同时更新group name和position
                    item.GroupName = msg.GroupName;
                    item.Position = msg.Position;

                    _hotProcessor.Update(item);
                }

                ts.Complete();
            }
        }
    }
}
