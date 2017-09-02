using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IBannerDimensionDA
    {
        void CreateBannerDimension(BannerDimension entity);
        void UpdateBannerDimension(BannerDimension entity);
        BannerDimension LoadBannerDimension(int sysNo);

        /// <summary>
        /// 检查同一页面类型下PositionID相同的个数
        /// </summary>
        /// <param name="excludeSysNo">排除系统编号</param>
        /// <param name="pageType">页面类型编号</param>
        /// <param name="positionID">位置编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        int CheckPositionIDCount(int excludeSysNo,int pageType,int positionID,string companyCode,string channelID);

        /// <summary>
        /// 检查同一页面类型下PositionName相同的个数
        /// </summary>
        /// <param name="excludeSysNo">排除系统编号</param>
        /// <param name="pageType">页面类型编号</param>
        /// <param name="positionName">位置名称</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        int CheckPositionNameCount(int excludeSysNo, int pageType, string positionName, string companyCode, string channelID);

        /// <summary>
        /// 根据位置编号获取BannerDimension
        /// </summary>
        /// <param name="sysNo">编号</param>
        /// <param name="positionID">位置编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        BannerDimension GetBannerDimensionByPositionID(int sysNo,int positionID, string companyCode);
    }
}
