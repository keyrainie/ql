using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(HotSaleCategoryProcessor))]
    public class HotSaleCategoryProcessor
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
            var kvList = CodeNamePairManager.GetList("MKT", "HotSale" + pageType.ToString());
            if (pageType == 0)
            {
                //如果页面类型是首页，还需要附加首页Domain馆
                var sections = ObjectFactory<IHomePageSectionQueryDA>.Instance.GetDomainCodeNames(companyCode, channelID);
                kvList.AddRange(sections);
            }

            return kvList;
        }

        private IHotSaleCategoryDA _hotDA = ObjectFactory<IHotSaleCategoryDA>.Instance;

        public void Insert(HotSaleCategory msg)
        {
            Validate(msg);
            _hotDA.Insert(msg);
        }

        public void Update(HotSaleCategory msg)
        {
            Validate(msg);
            _hotDA.Update(msg);
        }

        public void Delete(int sysNo)
        {
            _hotDA.Delete(sysNo);
        }

        public HotSaleCategory Load(int sysNo)
        {
            return _hotDA.Load(sysNo);
        }

        private void Validate(HotSaleCategory msg)
        {
            msg.SysNo = msg.SysNo ?? 0;
            //同一位置有效记录的组名必须相同
            string existsGroupName = _hotDA.GetExistsGroupNameByPosition(msg);
            if (!string.IsNullOrEmpty(existsGroupName))
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.HotSaleCategory", "HotSaleCategory_ExistsGroupName"), existsGroupName));
            }
            ///验证同位置同组下是否存在重复的分类设置
            if (_hotDA.CheckDuplicateCategory(msg))
            {
                //throw new BizException("当前组下已存在此分类,请修改！");
                throw new BizException(ResouceManager.GetMessageString("MKT.HotSaleCategory", "HotSaleCategory_ExistsCategory"));
            }
            if (msg.Status == ADStatus.Active)
            {
                var existsActiveList = _hotDA.GetByPosition(msg);
                int totalActiveCount = existsActiveList.Count;
                //位置为泰隆优选热卖最多12个有效分类
                if (msg.Position == 109)
                {
                    if (totalActiveCount >= 12)
                    {
                        //throw new BizException(string.Format("当前组最多允许有{0}个有效分类！", 12));
                        throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.HotSaleCategory", "HotSaleCategory_ExceedLimitSpecial1"), 12));
                    }
                }
                else
                {
                    if (totalActiveCount >= 3)
                    {
                        //throw new BizException(string.Format("当前组最多允许有{0}个有效分类！",3));
                        throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.HotSaleCategory", "HotSaleCategory_ExceedLimitSpecial1"), 3));
                    }
                }
            }
        }

        /// <summary>
        /// 获取同位置同组下其它的记录-
        /// </summary>
        /// <param name="relatedSysNo">参照记录的系统编号</param>
        /// <returns></returns>
        public List<HotSaleCategory> GetSameGroupOtherRecords(int relatedSysNo)
        {
            return _hotDA.GetSameGroupOtherRecords(relatedSysNo);
        }
    }
}
