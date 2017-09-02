using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{

    public interface IThesaurusInfoDA
    {
        #region 同义词（ThesaurusInfo）
        /// <summary>
        ///添加同义词
        /// </summary>
        /// <param name="item"></param>
        void AddThesaurusWords(ThesaurusInfo item);

        /// <summary>
        /// 检查是否存在相同的同义词
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckThesaurusWordsForUpdate(ThesaurusInfo item);

        bool CheckThesaurusWordsForInsert(ThesaurusInfo item);
        /// <summary>
        ///更新同义词
        /// </summary>
        /// <param name="item"></param>
        void UpdateThesaurusInfo(ThesaurusInfo items);
        
        /// <summary>
        ///根据编号编辑同义词
        /// </summary>
        /// <param name="item"></param>
        //void EditThesaurusWords(ThesaurusInfo item);

        /// <summary>
        ///根据编号列表批量更新同义词
        /// </summary>
        /// <param name="item"></param>
        //void BatchUpdateThesaurusWords(List<ThesaurusInfo> items);

        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <returns></returns>
        ThesaurusInfo LoadThesaurusWords(int sysNo);
        #endregion
    }
}
