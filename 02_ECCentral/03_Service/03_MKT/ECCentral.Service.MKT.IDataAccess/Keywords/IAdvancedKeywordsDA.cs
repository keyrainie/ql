using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.Keywords
{
    public interface IAdvancedKeywordsDA
    {
        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        int AddAdvancedKeywords(AdvancedKeywordsInfo item);

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="item"></param>
        void EditAdvancedKeywords(AdvancedKeywordsInfo item);

        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        AdvancedKeywordsInfo LoadAdvancedKeywordsInfo(int sysNo);

        /// <summary>
        /// 是否存在当前关键字
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckSameAdvancedKeywords(AdvancedKeywordsInfo item);

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <param name="item"></param>
        void CreateAdvancedKeywordsLog(AdvancedKeywordsLog item);
    }
}
