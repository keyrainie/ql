using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful.ResponseMsg
{
    public class AreaQueryResponse
    {
        /// <summary>
        /// 当前地区信息
        /// </summary>
        public AreaInfo CurrentAreaInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 省份列表
        /// </summary>
        public List<AreaInfo> ProviceList
        {
            get;
            set;
        }

        /// <summary>
        /// 城市列表
        /// </summary>
        public List<AreaInfo> CityList
        {
            get;
            set;
        }

        /// <summary>
        /// 地区列表
        /// </summary>
        public List<AreaInfo> DistrictList
        {
            get;
            set;
        }
    }
}