using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;


namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(StopWordsInfoProcessor))]
    public class StopWordsInfoProcessor
    {
        private IStopWordsInfoDA keywordDA = ObjectFactory<IStopWordsInfoDA>.Instance;

        #region 阻止词（StopWordsInfo）
        /// <summary>
        ///添加阻止词
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddStopWords(StopWordsInfo item)
        {
            if (keywordDA.CheckStopWords(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheStopWords"));
            else
                keywordDA.AddStopWords(item);
        }

        /// <summary>
        ///根据编号编辑阻止词
        /// </summary>
        /// <param name="item"></param>
        //public virtual void EditStopWords(StopWordsInfo item)
        //{
        //    keywordDA.EditStopWords(item);
        //}

        /// <summary>
        ///根据编号列表批量更新阻止词
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchUpdateStopWords(List<StopWordsInfo> items)
        {
            StringBuilder ex = new StringBuilder();
            foreach (StopWordsInfo item in items)
            {
                if (keywordDA.CheckStopWords(item))
                    ex.AppendLine(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheStopWordsContent") + item.Keywords.Content);
                else
                    keywordDA.UpdateStopWords(item);
            }
            if (!string.IsNullOrEmpty(ex.ToString()))
            {
                ex.AppendLine(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_OtherStopWordsUpdateSuccessful"));
                throw new BizException(ex.ToString());
            }
        }

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <returns></returns>
        public virtual StopWordsInfo LoadStopWords(int sysNo)
        {
            return keywordDA.LoadStopWords(sysNo);
        }
        #endregion
    }
}
