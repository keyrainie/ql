using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.WCF;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        private HotKeywordsAppService hotKeywordsAppService = ObjectFactory<HotKeywordsAppService>.Instance;

        #region 热门关键字（HotSearchKeyWord）
        /// <summary>
        /// 预览关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetHotKeywordsListByPageType", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual List<string> GetHotKeywordsListByPageType(HotSearchKeyWords item)
        {
            return hotKeywordsAppService.GetHotKeywordsListByPageType(item);
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/GetHotKeywordsEditUserList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual List<UserInfo> GetHotKeywordsEditUserList(string companyCode)
        {
            return hotKeywordsAppService.GetHotKeywordsEditUserList(companyCode);
        }

        /// <summary>
        /// 添加热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/AddHotSearchKeywords", Method = "POST")]
        public virtual void AddHotSearchKeywords(HotSearchKeyWords item)
        {
            hotKeywordsAppService.AddHotSearchKeywords(item);
        }

        /// <summary>
        /// 编辑热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/EditHotSearchKeywords", Method = "PUT")]
        public virtual void EditHotSearchKeywords(HotSearchKeyWords item)
        {
            hotKeywordsAppService.EditHotSearchKeywords(item);
        }

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchSetHotKeywordsInvalid", Method = "PUT")]
        public virtual void BatchSetHotKeywordsInvalid(List<int> items)
        {
            hotKeywordsAppService.BatchSetHotKeywordsInvalid(items);
        }

        /// <summary>
        /// 更新热门搜索关键字屏蔽状态
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/ChangeHotSearchedKeywordsStatus", Method = "PUT")]
        public virtual void ChangeHotSearchedKeywordsStatus(List<int> items)
        {
            hotKeywordsAppService.ChangeHotSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 加载热门搜索关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/LoadHotSearchKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual HotSearchKeyWords LoadHotSearchKeywords(int sysNo)
        {
            return hotKeywordsAppService.LoadHotSearchKeywords(sysNo);
        }

        /// <summary>
        /// 查询热门关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/KeywordsInfo/QueryHotKeywords", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryHotKeywords(HotKeywordsQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IHotKeywordsQueryDA>.Instance.QueryHotKeywords(filter, out totalCount);

            List<CodeNamePair> pair = ObjectFactory<PageTypeAppService>.Instance.GetPageType(filter.CompanyCode, filter.ChannelID.ToString(), ModuleType.HotKeywords);
            List<ECCategory> category1List = ObjectFactory<IECCategoryQueryDA>.Instance.GetAllECCategory1(filter.CompanyCode, filter.ChannelID.ToString());
            List<ECCategory> category3List = ObjectFactory<IECCategoryQueryDA>.Instance.GetAllECCategory3(filter.CompanyCode, filter.ChannelID.ToString());

            foreach (DataRow dr in dataTable.Rows)
            {
               PageTypePresentationType pageTypePresentationType= PageTypeUtil.ResolvePresentationType(ModuleType.HotKeywords, dr["PageType"].ToString());
               switch (pageTypePresentationType)
               {
                   case PageTypePresentationType.Category1:
                       dr["PageIDName"] = category1List.SingleOrDefault(a => a.SysNo.Value == int.Parse(dr["PageID"].ToString())).Name;
                       break;
                   case PageTypePresentationType.Category3:
                       dr["PageIDName"] = category3List.SingleOrDefault(a => a.SysNo.Value == int.Parse(dr["PageID"].ToString())).Name;
                       break;
                   default:
                       PageResult pr = ObjectFactory<PageTypeAppService>.Instance.GetPage(filter.CompanyCode, filter.ChannelID.ToString(), ModuleType.HotKeywords, dr["PageType"].ToString());
                       if (pr.PageList != null && pr.PageList.Count > 0)
                           dr["PageIDName"] = pr.PageList.SingleOrDefault(a => a.ID.Value == int.Parse(dr["PageID"].ToString())).PageName;
                       break;
               }
            }

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/KeywordsInfo/BatchSetHotKeywordsAvailable", Method = "PUT")]
        public virtual void BatchSetHotKeywordsAvailable(List<int> items)
        {
            hotKeywordsAppService.BatchSetHotKeywordsAvailable(items);
        }
        #endregion
    }
}
