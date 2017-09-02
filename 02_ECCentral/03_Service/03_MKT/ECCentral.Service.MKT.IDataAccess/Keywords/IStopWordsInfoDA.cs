using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{

    public interface IStopWordsInfoDA
    {

        #region 阻止词（StopWordsInfo）
        /// <summary>
        /// 检查是否存在相同的阻止词
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckStopWords(StopWordsInfo item);

        /// <summary>
        ///添加阻止词
        /// </summary>
        /// <param name="item"></param>
        void AddStopWords(StopWordsInfo item);

        /// <summary>
        ///根据编号编辑阻止词
        /// </summary>
        /// <param name="item"></param>
        //void EditStopWords(StopWordsInfo item);

        /// <summary>
        ///根据编号列表批量更新阻止词
        /// </summary>
        /// <param name="item"></param>
        void UpdateStopWords(StopWordsInfo items);

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <returns></returns>
        StopWordsInfo LoadStopWords(int sysNo);
        #endregion
    }
}
