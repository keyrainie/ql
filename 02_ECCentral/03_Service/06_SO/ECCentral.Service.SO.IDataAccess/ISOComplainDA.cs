using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOComplainDA
    {
        /// <summary>
        /// 添加投诉信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        SOComplaintInfo InsertComplainMaster(SOComplaintInfo info);

        /// <summary>
        /// 添加投诉历史记录
        /// </summary>
        /// <param name="info"></param>
        void InsertHistory(SOComplaintReplyInfo info);

        /// <summary>
        /// 更新投诉信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        SOComplaintInfo UpdateComplainMaster(SOComplaintInfo info);

        /// <summary>
        /// 指派投诉
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        void Update_AssignInfo(SOComplaintProcessInfo info);
        
        /// <summary>
        /// 检查该信息是否已存在
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool IsSameCaseExist(SOComplaintCotentInfo info, ref object oldComplainID);

        /// <summary>
        /// 获取历史回复信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        List<SOComplaintReplyInfo> GetHistory(SOComplaintInfo info);

        /// <summary>
        /// 根据Id获取信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SOComplaintInfo GetBySysNo(int sysNo);

        /// <summary>
        /// 更新操作员
        /// </summary>
        /// <param name="req"></param>
        void UpdateCompainOperatorUser(SOComplaintProcessInfo req);
    }
}
