using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Version
{
    public class VersionInfoModel
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool IsForcedUpdate { get; set; }
        /// <summary>
        /// 是否更新
        /// </summary>
        public bool IsUpdate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadPath { get; set; }
    }
}