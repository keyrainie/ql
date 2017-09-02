using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(DefaultKeywordsProcessor))]
    public class DefaultKeywordsProcessor
    {
        private IDefaultKeywordsDA _defaultKeywordDA = ObjectFactory<IDefaultKeywordsDA>.Instance;

        #region 默认关键字（DefaultKeywordsInfo）
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddDefaultKeywords(DefaultKeywordsInfo item)
        {
            //创建时默认SysNo为0，用于CheckDuplicate
            item.SysNo = 0;
            ValidateEntity(item);
            using (TransactionScope ts = new TransactionScope())
            {
                _defaultKeywordDA.AddDefaultKeywords(item);
                if (item.PageType.HasValue && item.PageID.HasValue)
                {
                    var pType = PageTypeUtil.ResolvePresentationType(ModuleType.DefaultKeywords, item.PageType.ToString());
                    //处理扩展生效
                    if (pType == PageTypePresentationType.Category3 && item.Extend == true)
                    {
                        var relatedECCategory3List = ObjectFactory<IECCategoryDA>.Instance.GetRelatedECCategory3SysNo(item.PageID.Value);
                        foreach (var rc3 in relatedECCategory3List)
                        {
                            item.PageID = rc3.C3SysNo;
                            if (!_defaultKeywordDA.CheckDuplicate(item))
                            {
                                _defaultKeywordDA.AddDefaultKeywords(item);
                            }
                        }
                    }
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// 编辑默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditDefaultKeywords(DefaultKeywordsInfo item)
        {
            ValidateEntity(item);
            _defaultKeywordDA.EditDefaultKeywords(item);
        }

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <returns></returns>
        public virtual DefaultKeywordsInfo LoadDefaultKeywordsInfo(int sysNo)
        {
            return _defaultKeywordDA.LoadDefaultKeywordsInfo(sysNo);
        }

        private void ValidateEntity(DefaultKeywordsInfo item)
        {
            if (string.IsNullOrEmpty(item.Keywords.Content))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedTheKeywordsValue"));
            if (item.BeginDate == item.EndDate)
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_TheKeywordsValidDateCannotBeSame"));
            if (item.Status==ADStatus.Active && _defaultKeywordDA.CheckDuplicate(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheKeywordsInTheSamePagePostion"));
        }

        #endregion
    }
}
