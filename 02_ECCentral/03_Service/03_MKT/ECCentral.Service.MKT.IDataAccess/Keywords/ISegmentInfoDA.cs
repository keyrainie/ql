using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface ISegmentInfoDA
    {
        #region 中文词库
        /// <summary>
        /// 检查是否已经存在该中文词库
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckSegmentInfo(SegmentInfo item);

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="info"></param>
        void AddSegmentInfo(SegmentInfo info);

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="adv"></param>
        void SetSegmentInfosValid(List<int> items);

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="adv"></param>
        void SetSegmentInfosInvalid(List<int> items);

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="adv"></param>
        void DeleteSegmentInfos(List<int> items);

        /// <summary>
        /// 更新中文词库
        /// </summary>
        /// <param name="item"></param>
        void UpdateSegmentInfo(SegmentInfo item);

        /// <summary>
        /// 加载中文词库
        /// </summary>
        /// <param name="item"></param>
        SegmentInfo LoadSegmentInfo(int sysNo);
        #endregion

    }
}
