//************************************************************************
// 用户名				泰隆优选
// 系统名				外网搜索优化管理
// 子系统名		        外网搜索优化逻辑实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT.Keyword;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(InternetKeywordProcessor))]
    public class InternetKeywordProcessor
    {
        private static readonly IInternetKeywordDA InternetKeywordDA = ObjectFactory<IInternetKeywordDA>.Instance;

        #region 外网搜索优化业务方法
       
        /// <summary>
        /// 创建外网搜索
        /// </summary>
        /// <param name="keywordInfos"></param>
        /// <returns></returns>
        public virtual void CreateKeyword(List<InternetKeywordInfo> keywordInfos)
        {
            CheckKeywordProcessor.CheckinternetKeywordList(keywordInfos);
           var errorDesc=new List<string>();
            keywordInfos.ForEach(v =>
            {
                CheckKeywordProcessor.CheckKeywordInfo(v);
                var desc = CheckKeywordProcessor.CheckExistSearchkeyword(v.Searchkeyword);
                if (string.IsNullOrEmpty(desc))
                {
                    InternetKeywordDA.InsertKeyword(v);
                }
                else
                {
                    errorDesc.Add(desc);
                }
            });
            if(errorDesc.Count>0)
            {
                var desc = errorDesc.Join("\r\n");
                throw new BizException(desc);
            }

        }

      

        /// <summary>
        /// 修改外网搜索状态
        /// </summary>
        /// <param name="internetKeywords"></param>
        public void ModifyKeywordStatus(List<InternetKeywordInfo> internetKeywords)
        {
            CheckKeywordProcessor.CheckinternetKeywordList(internetKeywords);
            internetKeywords.ForEach(v =>
                    {
                        CheckKeywordProcessor.CheckKeywordStatus(v);
                        if (v != null && v.SysNo != null)
                            InternetKeywordDA.UpdateKeywordStatus(v);                      
                    });
           
        }

      

        #endregion

        #region 检查Keyword逻辑
        private static class CheckKeywordProcessor
        {
            /// <summary>
            /// 检查改外网搜索实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckKeywordInfo(InternetKeywordInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "KeywordIsNull"));
                }
                //var languageCode = Thread.CurrentThread.CurrentUICulture.Name;
                var operateUser = entity.OperateUser;
                if (operateUser == null || String.IsNullOrWhiteSpace(operateUser.UserDisplayName))
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "OperateUserIsNull"));
                }
                var searchkeyword = entity.Searchkeyword;
                if (String.IsNullOrWhiteSpace(searchkeyword))
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "SearchkeywordIsNull"));
                }
            }

            /// <summary>
            /// 检查改外网搜索实体状态
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckKeywordStatus(InternetKeywordInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "KeywordIsNull"));
                }
                if (entity.SysNo == null || entity.SysNo.Value <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "KeywordSysNoIsNull"));
                }
            }

            /// <summary>
            /// 检查外网搜索队列
            /// </summary>
            /// <param name="internetKeywords"></param>
            public static void CheckinternetKeywordList(List<InternetKeywordInfo> internetKeywords)
            {
                if (internetKeywords == null || internetKeywords.Count == 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "InternetKeywordsListIsNull"));
                }

            }
            /// <summary>
            /// 是否存在某个同名的搜索关键字
            /// </summary>
            /// <param name="searchkeyword"></param>
            /// <returns></returns>
            public static string CheckExistSearchkeyword(string searchkeyword)
            {
                if (string.IsNullOrWhiteSpace(searchkeyword))
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.SearchKeyword", "FileNameIsNull"));
                }
                if(searchkeyword.Length>200)
                {
                    var desc = ResouceManager.GetMessageString("MKT.SearchKeyword", "SearchKeywordLength");
                     desc = String.Format(desc, searchkeyword);
                    return desc;
                }
                var result = InternetKeywordDA.GetInternetKeywordInfoByKeyword(searchkeyword);
                if (result!=null)
                {
                    var desc = ResouceManager.GetMessageString("MKT.SearchKeyword", "ExistSearchKeyword");
                    desc = String.Format(desc, searchkeyword);
                    return desc;
                }
                return "";
            }
        }
        #endregion
    }

}

