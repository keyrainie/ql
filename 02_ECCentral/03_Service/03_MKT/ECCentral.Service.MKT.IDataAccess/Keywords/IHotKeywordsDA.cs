using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IHotKeywordsDA
    {
        #region 热门关键字（HotSearchKeyWord）
        /// <summary>
        /// 预览关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<string> GetHotKeywordsListByPageType(HotSearchKeyWords item);

        /// <summary>
        /// 获取首页关键字
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        string GetHotKeywordsForHomepage(string companyCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<HotSearchKeyWords> GetHotKeywordsEditByKeyword(HotSearchKeyWords item);

        /// <summary>
        /// 添加热门搜索关键字-不显示状态
        /// </summary>
        /// <param name="item"></param>
        void InsertHotKeywordsInfoHidder(HotSearchKeyWords item);

        /// <summary>
        /// 添加热门搜索关键字for SearchKeyword
        /// </summary>
        /// <param name="item"></param>
        void AddHotSearchKeywordsForSearch(HotSearchKeyWords item);

        /// <summary>
        /// 适用底层类别，扩展生效判断
        /// 根据PageID，查询相同的C3前台类别SysNO
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<HotSearchKeyWords> GetOtherHotSearchKeywordECCategoryList(HotSearchKeyWords item);

        /// <summary>
        /// 根据关键字获取类似的关键字列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<string> GetSearchedKeywordsInfoByKeyword(HotSearchKeyWords item);

        /// <summary>
        /// 根据关键字获取类似的关键字列表，IsOnlineShow不为1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<string> GetHotKeywordsInfoByKeywordbyTrue(HotSearchKeyWords item);

        /// <summary>
        /// 根据关键字获取类似的关键字列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        List<string> GetHotKeywordsInfoByKeyword(HotSearchKeyWords item);

        /// <summary>
        /// 获取热门关键字的最大长度,根据已经存在的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool GetHotKeywordsMaxLenthBySysNo(HotSearchKeyWords item);

        /// <summary>
        /// 扩展生效功能
        /// 操作同类后的记录，无则修改，有则增加
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        void OperExpandECCategoryHotSearchKeyword(HotSearchKeyWords item);

        /// <summary>
        /// 判断在同后台类别的前台类别下，是否有相同名称的关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int IsSameNameHotSearchKeyword(HotSearchKeyWords item);

        /// <summary>
        /// 获取热门关键字的最大长度
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetHotKeywordsMaxLenth(HotSearchKeyWords item);

        /// <summary>
        /// 获取热门关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<UserInfo> GetHotKeywordsEditUserList(string companyCode);

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        void BatchSetHotKeywordsInvalid(List<int> items);

        /// <summary>
        /// 添加热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        void AddHotSearchKeywords(HotSearchKeyWords item);

        /// <summary>
        /// 编辑热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        void EditHotSearchKeywords(HotSearchKeyWords item);

        /// <summary>
        /// 更新热门搜索关键字屏蔽状态       ??
        /// </summary>
        /// <param name="item"></param>
        void ChangeHotSearchedKeywordsStatus(List<int> items);

        /// <summary>
        /// 加载热门搜索关键字
        /// </summary>
        /// <returns></returns>
        HotSearchKeyWords LoadHotSearchKeywords(int sysNo);

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        void BatchSetHotKeywordsAvailable(List<int> items);

        #endregion
    }
}
