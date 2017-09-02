using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(HotKeywordsProcessor))]
    public class HotKeywordsProcessor
    {
        private IHotKeywordsDA keywordDA = ObjectFactory<IHotKeywordsDA>.Instance;
        private const string _strKey = "关键字";

        #region 热门关键字（HotSearchKeyWord）
        /// <summary>
        /// 预览关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual List<string> GetHotKeywordsListByPageType(HotSearchKeyWords item)
        {
            return keywordDA.GetHotKeywordsListByPageType(item);
        }

        /// <summary>
        /// 获取热门关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<UserInfo> GetHotKeywordsEditUserList(string companyCode)
        {
            return keywordDA.GetHotKeywordsEditUserList(companyCode);
        }

        /// <summary>
        /// 批量屏蔽热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetHotKeywordsInvalid(List<int> items)
        {
            keywordDA.BatchSetHotKeywordsInvalid(items);
        }

        /// <summary>
        /// 添加热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddHotSearchKeywords(HotSearchKeyWords item)
        {
            item.PageID = string.IsNullOrEmpty(item.PageID.ToString()) ? 0 : item.PageID.Value;
            List<string> list = new List<string>();
            if (item.IsOnlineShow == ECCentral.BizEntity.MKT.NYNStatus.Yes)
                list = keywordDA.GetHotKeywordsInfoByKeyword(item);
            else
                list = keywordDA.GetHotKeywordsInfoByKeywordbyTrue(item);

            bool bMaxLen = keywordDA.GetHotKeywordsMaxLenthBySysNo(item);

            if (list.Count < 1)
            {
                if (item.PageType == 0 || item.PageType > 100)
                {
                    List<string> listByKeyword = keywordDA.GetSearchedKeywordsInfoByKeyword(item);
                    if (listByKeyword.Count < 1)
                    {
                        if (!bMaxLen)
                            throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));
                        else
                            keywordDA.AddHotSearchKeywordsForSearch(item);
                    }
                    if (item.IsOnlineShow == ECCentral.BizEntity.MKT.NYNStatus.Yes && !bMaxLen)
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));

                    if (item.IsOnlineShow == ECCentral.BizEntity.MKT.NYNStatus.Yes)
                        keywordDA.AddHotSearchKeywords(item);
                    else
                        keywordDA.InsertHotKeywordsInfoHidder(item);
                }
                else if (item.PageType == 3)   //如果是底层类别才有扩展生效处理
                {
                    #region 如果是底层类别才有扩展生效处理
                    
                    if (!item.PageID.HasValue)
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedSelectTheCategory"));
                    
                    if (!bMaxLen)
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));

                    if (item.IsOnlineShow == ECCentral.BizEntity.MKT.NYNStatus.No)
                        keywordDA.InsertHotKeywordsInfoHidder(item);
                    else
                        keywordDA.AddHotSearchKeywords(item);

                    //判断搜索关键字是否有同名的
                    List<string> listByKeyword = keywordDA.GetSearchedKeywordsInfoByKeyword(item);
                    if (listByKeyword.Count < 1)
                    {
                        if (!bMaxLen)
                            throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));
                        else
                            keywordDA.AddHotSearchKeywordsForSearch(item);
                    }

                    //如果选择了“扩展生效”执行以下逻辑
                    if (item.Extend.HasValue && item.Extend.Value)
                    {//查询C3相同的前台类别数据
                        List<HotSearchKeyWords> listByExtend = keywordDA.GetOtherHotSearchKeywordECCategoryList(item);
                        if (listByExtend.Count > 0)
                        {
                            foreach (HotSearchKeyWords keyword in listByExtend)
                            {
                                item.PageID = keyword.PageID.Value;
                                int sameNameSum = 0;
                                sameNameSum = keywordDA.IsSameNameHotSearchKeyword(item);
                                //如果在该前台类别下有同名的关键字，则不执行以下逻辑
                                if (sameNameSum <= 0)
                                {
                                    bool bMaxLenExpand = keywordDA.GetHotKeywordsMaxLenthBySysNo(item);
                                    if (bMaxLenExpand)
                                        keywordDA.OperExpandECCategoryHotSearchKeyword(item);
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region other
                    
                    if (!item.PageID.HasValue)
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedSelectTheCategory"));
                    else
                    {
                        if (!item.PageID.HasValue)
                            throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedSelectTheCategory"));
                       
                        if (!bMaxLen)
                            throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));
                        if (item.IsOnlineShow == ECCentral.BizEntity.MKT.NYNStatus.No)
                            keywordDA.InsertHotKeywordsInfoHidder(item);
                        else
                            keywordDA.AddHotSearchKeywords(item);


                        //判断搜索关键字是否有同名的
                        List<string> listByKeyword = keywordDA.GetSearchedKeywordsInfoByKeyword(item);
                        if (listByKeyword.Count < 1)
                        {
                            if (!bMaxLen)
                                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));
                            else
                                keywordDA.AddHotSearchKeywordsForSearch(item);
                        }
                    }
                    #endregion
                }
            }
            else
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheSameInfo"));
        }

        /// <summary>
        /// 编辑热门搜索关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditHotSearchKeywords(HotSearchKeyWords item)
        {
            List<HotSearchKeyWords> list = keywordDA.GetHotKeywordsEditByKeyword(item);
            if (list.Count < 1)
            {
                if (!item.SysNo.HasValue)
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_InputValueISNull"));
                if (!item.PageID.HasValue)
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedSelectTheCategory"));
                else if (item.IsOnlineShow.HasValue&&item.IsOnlineShow==NYNStatus.Yes&&!keywordDA.GetHotKeywordsMaxLenthBySysNo(item))
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsLengthLessThan"));
                else
                    keywordDA.EditHotSearchKeywords(item);
            }
            else
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheSameInfo"));
        }

        /// <summary>
        /// 更新热门搜索关键字屏蔽状态
        /// </summary>
        /// <param name="item"></param>
        public virtual void ChangeHotSearchedKeywordsStatus(List<int> items)
        {
            keywordDA.ChangeHotSearchedKeywordsStatus(items);
        }

        /// <summary>
        /// 加载热门搜索关键字
        /// </summary>
        /// <returns></returns>
        public virtual HotSearchKeyWords LoadHotSearchKeywords(int sysNo)
        {
            return keywordDA.LoadHotSearchKeywords(sysNo);
        }

        /// <summary>
        /// 批量有效热门关键字状态
        /// </summary>
        /// <param name="items"></param>
        public virtual void BatchSetHotKeywordsAvailable(List<int> items)
        {
            keywordDA.BatchSetHotKeywordsAvailable(items);
        }
        #endregion
    }
}
