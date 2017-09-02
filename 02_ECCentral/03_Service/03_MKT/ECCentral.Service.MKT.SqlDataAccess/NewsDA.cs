using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(INewsDA))]
    public class NewsDA : INewsDA
    {
        #region 公告及促销评论管理

        /// <summary>
        /// 批量设置展示
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetNewsAdvReplyShow(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("News_UpdateNewsAdvReply");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", NewsAdvReplyStatus.Show);
            dc.SetParameterValueAsCurrentUserSysNo("LastEditUserSysNo");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量设置屏蔽
        /// </summary>
        /// <param name="items"></param>
        public void BatchSetNewsAdvReplyHide(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("News_UpdateNewsAdvReply");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", NewsAdvReplyStatus.HandHide);
            dc.SetParameterValueAsCurrentUserSysNo("LastEditUserSysNo");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 评论回复
        /// </summary>
        /// <param name="item"></param>
        public void CreateNewsAdvReply(NewsAdvReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_CreateNewsAdvReply");
            dc.SetParameterValue<NewsAdvReply>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新公告及促销评论展示状态
        /// </summary>
        /// <param name="item"></param>
        public void UpdateNewsAdvReply(NewsAdvReply item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_UpdateNewsAdvReply");
            dc.SetParameterValue("@SysNoString", item.SysNo.Value);
            dc.SetParameterValue("@Status", item.Status);
            dc.SetParameterValueAsCurrentUserSysNo("LastEditUserSysNo");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        public List<UserInfo> GetNewAdvReplyCreateUsers(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_GetNewsAdvUser");
            dc.SetParameterValue("@CompanyCode", companyCode);
            return dc.ExecuteEntityList<UserInfo>();
        }

        /// <summary>
        /// 加载公告及促销评论
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public NewsAdvReply LoadNewsAdvReply(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_GetNewsAdvReply");
            dc.SetParameterValue("@SysNo", sysNo);
            //return dc.ExecuteEntity<NewsAdvReply>();


            DataTable dt = dc.ExecuteDataTable();
            NewsAdvReply item = new NewsAdvReply();
            //item.ReplyContent = new LanguageContent("zh-CN", dt.Rows[0]["ReplyContent"].ToString().Trim());
            item = DataMapper.GetEntity<NewsAdvReply>(dt.Rows[0]);
            return item;
        }
        #endregion

        public NewsInfo CreateNewsInfo(NewsInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_CreateNews");
            dc.SetParameterValue<NewsInfo>(entity);
            dc.ExecuteNonQuery();
            entity.SysNo = (int)dc.GetParameterValue("@SysNo");
            return entity;
        }

        public void UpdateNewsInfo(NewsInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_UpdateNews");
            dc.SetParameterValue<NewsInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public void UpdateStatus(int newsID, NewsStatus status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_UpdateStatus");
            dc.SetParameterValue("@SysNo", newsID);
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取创建人的列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        public List<UserInfo> GetCreateUsers(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_GetCreateUsers");
            dc.SetParameterValue("@CompanyCode", companyCode);

            return dc.ExecuteEntityList<UserInfo>();
        }

        /// <summary>
        /// 获取所有的新闻公告类型
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public List<CodeNamePair> GetAllNewTypes(string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("News_GetAllNewTypes");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加渠道参数
            List<CodeNamePair> allNewTypes = cmd.ExecuteEntityList<CodeNamePair>();
            allNewTypes.ForEach(item => item.Name = item.Name.Trim());
            return allNewTypes;
        }

        public NewsInfo Load(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_Load");
            dc.SetParameterValue("@SystemNumber", sysNo);
            return dc.ExecuteEntity<NewsInfo>();
        }

        /// <summary>
        /// 删除相关图片
        /// </summary>
        /// <param name="image"></param>
        public void DeleteNewsAdvReplyImage(string image)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("News_DeleteNewsAdvReplyImage");
            string[] param = image.Split('!');
            dc.SetParameterValue("@Image", param[0]);
            dc.SetParameterValue("@SysNo", param[1]);
            dc.SetParameterValueAsCurrentUserSysNo("UserSysNo");
            dc.ExecuteNonQuery();
        }
    }
}