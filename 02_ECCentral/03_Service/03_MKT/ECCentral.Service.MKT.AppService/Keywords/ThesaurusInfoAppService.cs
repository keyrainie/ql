using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{

    [VersionExport(typeof(ThesaurusInfoAppService))]
    public class ThesaurusInfoAppService
    {
        #region 同义词（ThesaurusInfo）
        /// <summary>
        ///添加同义词
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddThesaurusWords(ThesaurusInfo item)
        {
            ObjectFactory<ThesaurusInfoProcessor>.Instance.AddThesaurusWords(item);
        }

        /// <summary>
        ///根据编号编辑同义词
        /// </summary>
        /// <param name="item"></param>
        //public virtual void EditThesaurusWords(ThesaurusInfo item)
        //{
        //    ObjectFactory<ThesaurusInfoProcessor>.Instance.EditThesaurusWords(item);
        //}

        /// <summary>
        ///根据编号列表批量更新同义词
        /// </summary>
        /// <param name="item"></param>
        public virtual void BatchUpdateThesaurusWords(List<ThesaurusInfo> items)
        {
            ObjectFactory<ThesaurusInfoProcessor>.Instance.BatchUpdateThesaurusWords(items);
        }

        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <returns></returns>
        public virtual ThesaurusInfo LoadThesaurusWords(int sysNo)
        {
            return ObjectFactory<ThesaurusInfoProcessor>.Instance.LoadThesaurusWords(sysNo);
        }

        #endregion
    }
}
