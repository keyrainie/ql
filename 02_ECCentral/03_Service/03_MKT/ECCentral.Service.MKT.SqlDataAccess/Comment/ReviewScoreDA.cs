using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.MKT.SqlDataAccess
{

    [VersionExport(typeof(IReviewScoreDA))]
    public class ReviewScoreDA : IReviewScoreDA
    {
        #region 评分项定义
        /// <summary>
        /// 创建评分项定义
        /// </summary>
        /// <param name="item"></param>
        public void CreateReviewScoreItem(ReviewScoreItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Comment_CreateReviewScoreItem");
            //dc.SetParameterValue("@Name",item.Name.Content);
            dc.SetParameterValue<ReviewScoreItem>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑评分项定义
        /// </summary>
        /// <param name="item"></param>
        public void UpdateReviewScoreItem(ReviewScoreItem item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Commnet_UpdateReviewScoreItem");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="sysNo"></param>
        public ReviewScoreItem LoadReviewScoreItem(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Commnet_GetReviewScoreItem");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();

            ReviewScoreItem item = new ReviewScoreItem();
            item = DataMapper.GetEntity<ReviewScoreItem>(dt.Rows[0]);
            return item;
            //item.Name = new LanguageContent("zh-CN", dt.Rows[0]["Name"].ToString().Trim());
            //return DataMapper.GetEntity<ReviewScoreItem>(dt.Rows[0]);
        }

        /// <summary>
        /// 加载评分项定义
        /// </summary>
        /// <param name="sysNo"></param>
        public ReviewScoreItem LoadReviewScoreItem(string name, string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Commnet_GetReviewScoreItemByName");
            dc.SetParameterValue("@Name_Content", name);
            dc.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = dc.ExecuteDataTable();

            ReviewScoreItem item = null;
            if (dt!=null&&dt.Rows.Count > 0)
            {
                item = DataMapper.GetEntity<ReviewScoreItem>(dt.Rows[0]);
            }
            return item;
            //item.Name = new LanguageContent("zh-CN", dt.Rows[0]["Name"].ToString().Trim());
            //return DataMapper.GetEntity<ReviewScoreItem>(dt.Rows[0]);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreValid(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Commnet_UpdateReviewScoreItemStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Active);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public void SetReviewScoreInvalid(List<int> items)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Commnet_UpdateReviewScoreItemStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        #endregion

        #region  评论模式设置（RemarkMode）
        /// <summary>
        /// 加载新闻公告及促销评论模式
        /// </summary>
        public RemarkMode LoadRemarkMode(string companyCode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemarkMode_QueryRemarkModeTypeIsR");
            dc.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = dc.ExecuteDataTable();
            if (dt == null||dt.Rows.Count==0)
                return new RemarkMode();
            else
                return DataMapper.GetEntity<RemarkMode>(dt.Rows[0]);
        }

        /// <summary>
        /// 更新公告及促销评论模式
        /// </summary>
        /// <param name="item"></param>
        public void UpdateRemarkMode(RemarkMode item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemarkMode_UpdateSingleStatusAndWeekendRule");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新讨论，评论，咨询所对应的评论模式
        /// </summary>
        /// <param name="item"></param>
        public void UpdateOtherRemarkMode(RemarkMode item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RemarkMode_UpdateBatchStatusAndWeekendRule");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }
        #endregion
    }
}
