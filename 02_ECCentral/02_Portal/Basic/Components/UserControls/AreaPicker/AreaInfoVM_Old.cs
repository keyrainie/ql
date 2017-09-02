using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{

    public class AreaInfoVM_Old:ModelBase
    {
        public AreaLevel Level
        {
            get
            {
                if (ProvinceSysNo == null && CitySysNo == null)
                {
                    return AreaLevel.Province;
                }
                if (ProvinceSysNo != null && CitySysNo == null)
                {
                    return AreaLevel.City;
                }
                else
                {
                    return AreaLevel.District;
                }
            }
        }

        public int? SysNo
        {
            get;
            set;
        }

        public string SysNumber
        {
            get
            {
                return SysNo == null ? "" : SysNo.ToString();
            }
            set
            {
                int sysNo;
                if (int.TryParse(value, out sysNo))
                {
                    SysNo = sysNo;
                }
                else
                {
                    SysNo = null;
                }
            }
        }

        /// <summary>
        /// 省级系统编号
        /// </summary>
        public int? ProvinceSysNo
        {
            get;
            set;
        }

        public string ProvinceSysNumber
        {
            get
            {
                int? provinceSysNo = null;
                switch (this.Level)
                {
                    case AreaLevel.Province:
                        provinceSysNo = this.SysNo;
                        break;
                    case AreaLevel.City:
                    case AreaLevel.District:
                        provinceSysNo = this.ProvinceSysNo;
                        break;
                }
                return provinceSysNo == null ? "" : provinceSysNo.ToString();
            }
        }

        /// <summary>
        /// 市级系统编号
        /// </summary>
        public int? CitySysNo
        {
            get;
            set;
        }

        public string CitySysNumber
        {
            get
            {
                int? citySysNo = null;
                switch (this.Level)
                {
                    case AreaLevel.Province:
                        citySysNo = null;
                        break;
                    case AreaLevel.City:
                        citySysNo = this.SysNo;
                        break;
                    case AreaLevel.District:
                        citySysNo = this.CitySysNo;
                        break;
                }
                return citySysNo == null ? "" : citySysNo.ToString();
            }
        }

        /// <summary>
        /// 省级名称
        /// </summary>
        public string ProvinceName
        {
            get;
            set;
        }

        /// <summary>
        /// 市级名称
        /// </summary>
        public string CityName
        {
            get;
            set;
        }

        /// <summary>
        /// 县（地区）级名称
        /// </summary>
        public string DistrictName
        {
            get;
            set;
        }

        /// <summary>
        /// 地区名称
        /// 如果是省，则是省份名称；如果是市，则是市名称，如果是地区，则是地区名称
        /// </summary>
        public string AreaName
        {
            get
            {
                switch (this.Level)
                {
                    case AreaLevel.Province:
                        return ProvinceName;
                    case AreaLevel.City:
                        return CityName;
                    default:
                        return DistrictName;
                }
            }
        }

        public int? Status
        {
            get;
            set;
        }
    }
}
