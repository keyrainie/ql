using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess
{
    /// <summary>
    /// 广告商管理（Advertisers）
    /// </summary>
    public interface IAdvertisersDA
    {
        #region 广告商
        /// <summary>
        /// 创建广告商
        /// </summary>
        /// <param name="item"></param>
        void CreateAdvertisers(Advertisers item);

        /// <summary>
        /// 是否存在该广告商
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckExistAdvertisers(Advertisers item);

        /// <summary>
        /// 是否存在该广告商监测代码
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CheckExistAdvertiserMonitorCode(string companyCode, string monitorCode, int? sysNo);

        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <param name="sysNo"></param>
        Advertisers LoadAdvertiser(int sysNo);

        /// <summary>
        /// 编辑广告商
        /// </summary>
        /// <param name="item"></param>
        void UpdateAdvertisers(Advertisers item);

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        void SetAdvertiserValid(List<int> item);

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        void SetAdvertiserInvalid(List<int> item);

        #endregion

        #region 广告效果监视

        /// <summary>
        /// 导出订阅用户Email列表
        /// </summary>
        void ExportToExcel();
        #endregion

    }
}
