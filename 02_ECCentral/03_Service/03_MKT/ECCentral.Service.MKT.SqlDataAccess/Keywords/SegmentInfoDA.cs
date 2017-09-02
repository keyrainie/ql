using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ISegmentInfoDA))]
    public class SegmentInfoDA : ISegmentInfoDA
    {
        #region  中文词库
        /// <summary>
        /// 检查是否已经存在该中文词库
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckSegmentInfo(SegmentInfo item)
        {
            DataCommand cmd;
            if (item.SysNo > 0)
            {
                cmd = DataCommandManager.GetDataCommand("Keyword_CheckSegmentInfoByUpdate");
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("Keyword_CheckSegmentInfo");
            }

            cmd.SetParameterValue<SegmentInfo>(item);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="info"></param>
        public virtual void AddSegmentInfo(SegmentInfo info)
        {
            info.Status = KeywordsStatus.Waiting;
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_InsertSegment");
            //dc.SetParameterValue("@LanguageCode", info.Keywords.Content);
            dc.SetParameterValue<SegmentInfo>(info);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载中文词库
        /// </summary>
        /// <param name="sysNo"></param>
        public SegmentInfo LoadSegmentInfo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_GetSegment");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable();
            SegmentInfo segment = new SegmentInfo();
            return DataMapper.GetEntity<SegmentInfo>(dt.Rows[0]);
        }

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="item"></param>
        public void SetSegmentInfosValid(List<int> item)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in item)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_SetSegmentStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Active);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="item"></param>
        public void SetSegmentInfosInvalid(List<int> item)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in item)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_SetSegmentStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="item"></param>
        public void DeleteSegmentInfos(List<int> item)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in item)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_DeleteSegments");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑中文词库
        /// </summary>
        /// <param name="item"></param>
        public void UpdateSegmentInfo(SegmentInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Segment_UpdateSegment");
            dc.SetParameterValue(item);
            dc.ExecuteNonQuery();
        }

        #endregion

    }
}
