using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(HotKeywordsAppService))]
    public class HotKeywordsAppService
    {
        #region 热门关键字（HotSearchKeyWord）
        /// <summary>
        /// 预览关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual List<string> GetHotKeywordsListByPageType(HotSearchKeyWords item)
        {
            return ObjectFactory<HotKeywordsProcessor>.Instance.GetHotKeywordsListByPageType(item);
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<UserInfo> GetHotKeywordsEditUserList(string companyCode)
        {
            return ObjectFactory<HotKeywordsProcessor>.Instance.GetHotKeywordsEditUserList(companyCode);
        }

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetHotKeywordsInvalid(List<int> items)
        {
            ObjectFactory<HotKeywordsProcessor>.Instance.BatchSetHotKeywordsInvalid(items);
        }

        /// <summary>
        /// 添加热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddHotSearchKeywords(HotSearchKeyWords item)
        {
            ObjectFactory<HotKeywordsProcessor>.Instance.AddHotSearchKeywords(item);
        }

        /// <summary>
        /// 编辑热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditHotSearchKeywords(HotSearchKeyWords item)
        {
            ObjectFactory<HotKeywordsProcessor>.Instance.EditHotSearchKeywords(item);
        }

        /// <summary>
        /// 更新热门搜索关键字屏蔽状态
        /// </summary>
        /// <param name="item"></param>
        public virtual void ChangeHotSearchedKeywordsStatus(List<int> items)
        {
            ObjectFactory<HotKeywordsProcessor>.Instance.ChangeHotSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 加载热门搜索关键字
        /// </summary>
        /// <returns></returns>
        public virtual HotSearchKeyWords LoadHotSearchKeywords(int sysNo)
        {
            return ObjectFactory<HotKeywordsProcessor>.Instance.LoadHotSearchKeywords(sysNo);
        }

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetHotKeywordsAvailable(List<int> items)
        {
            ObjectFactory<HotKeywordsProcessor>.Instance.BatchSetHotKeywordsAvailable(items);
        }
        #endregion
    }
}
