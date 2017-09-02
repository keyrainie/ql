using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
namespace ECCentral.Service.MKT.IDataAccess
{
    public interface INewsDA
    {
        #region 公告及促销评论管理

        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="item"></param>
        void BatchSetNewsAdvReplyShow(List<int> items);

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="item"></param>
        void BatchSetNewsAdvReplyHide(List<int> items);

        /// <summary>
        /// 评论回复
        /// </summary>
        /// <param name="item"></param>
        void CreateNewsAdvReply(NewsAdvReply item);

        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="filter"></param>
        void UpdateNewsAdvReply(NewsAdvReply item);

        /// <summary>
        /// 获取公告及促销评论回复创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        List<UserInfo> GetNewAdvReplyCreateUsers(string companyCode ,string channelID);

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        NewsAdvReply LoadNewsAdvReply(int sysNo);
        #endregion

        NewsInfo CreateNewsInfo(NewsInfo news);

        void UpdateNewsInfo(NewsInfo news);

        void UpdateStatus(int newsID, NewsStatus status);

        List<UserInfo> GetCreateUsers(string companyCode, string channelID);

        List<CodeNamePair> GetAllNewTypes(string companyCode, string channelID);

        NewsInfo Load(int sysNo);
        
        /// <summary>
        /// 删除相关图片
        /// </summary>
        /// <param name="image"></param>
        void DeleteNewsAdvReplyImage(string image);
 
    }
}
