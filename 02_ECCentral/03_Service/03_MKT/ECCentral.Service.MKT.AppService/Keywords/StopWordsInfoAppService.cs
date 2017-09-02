using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{

    [VersionExport(typeof(StopWordsInfoAppService))]
    public class StopWordsInfoAppService
    {

        #region 阻止词（StopWordsInfo）
        /// <summary>
        ///添加阻止词
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddStopWords(StopWordsInfo item)
        {
            ObjectFactory<StopWordsInfoProcessor>.Instance.AddStopWords(item);
        }

        /// <summary>
        ///根据编号编辑阻止词
        /// </summary>
        /// <param name="item"></param>
        //public virtual void EditStopWords(StopWordsInfo item)
        //{
        //    ObjectFactory<StopWordsInfoProcessor>.Instance.EditStopWords(item);
        //}

        /// <summary>
        ///根据编号列表批量更新阻止词
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchUpdateStopWords(List<StopWordsInfo> items)
        {
            ObjectFactory<StopWordsInfoProcessor>.Instance.BatchUpdateStopWords(items);
        }

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <returns></returns>
        public virtual StopWordsInfo LoadStopWords(int sysNo)
        {
            return ObjectFactory<StopWordsInfoProcessor>.Instance.LoadStopWords(sysNo);
        }

        #endregion
    }
}
