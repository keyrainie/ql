using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IImageSizeDA))]
    public class ImageSizeDA : IImageSizeDA
    {
        /// <summary>
        /// 得到所有图片尺寸
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetAllImageSize(ImageSizeQueryFilter query, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllImageSize");
            cmd.SetParameterValue("@pagesize", query.PageInfo.PageSize);
            cmd.SetParameterValue("@pageindex", query.PageInfo.PageIndex);
            DataTable dt = new DataTable();
           dt= cmd.ExecuteDataTable();
           totalCount = (int)cmd.GetParameterValue("@TotalCount");
           return dt;

        }

        /// <summary>
        /// 创建新尺寸
        /// </summary>
        /// <param name="info"></param>
        public void CreateImageSzie(ImageSizeInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateImageSzie");
            cmd.SetParameterValue("@ImageWidth", info.ImageWidth);
            cmd.SetParameterValue("@ImageHeight", info.ImageHeight);
            cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// 检查尺寸是否存在
        /// </summary>
        /// <param name="info"></param>
        /// <returns>bool true:存在</returns>
        public bool IsExistsImageSize(ImageSizeInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsImageSize");
            cmd.SetParameterValue("@ImageWidth", info.ImageWidth);
            cmd.SetParameterValue("@ImageHeight", info.ImageHeight);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") > 0;

        }
        /// <summary>
        /// 删除时更新标识位 逻辑删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void UpdateImageSizeFlag(int SysNo) 
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateImageSizeFlag");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.ExecuteNonQuery();
        }


      
    }
}
