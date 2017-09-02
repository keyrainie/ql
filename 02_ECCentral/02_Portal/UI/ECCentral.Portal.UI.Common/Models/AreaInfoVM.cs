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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Models
{
    public class AreaInfoVM:ModelBase
    {
        public AreaInfoVM()
        {
           
        }

        //系统编号

        public int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }
        //省份信息
        public int? provinceSysNo;
        public int? ProvinceSysNo
        {
            get { return provinceSysNo; }
            set { SetValue("ProvinceSysNo", ref provinceSysNo, value); }
        }

        public string provinceName;
        public string ProvinceName
        {
            get { return provinceName; }
            set { SetValue("ProvinceName", ref provinceName, value); }
        }
        //市级信息
        public int? citySysNo;
        public int? CitySysNo
        {
            get { return citySysNo; }
            set { SetValue("CitySysNo", ref citySysNo, value); }
        }

        public string cityName;
        public string CityName
        {
            get { return cityName; }
            set { SetValue("CityName",ref cityName,value);}
        }
        //区域信息
        public int? districtSysNo;
        public int? DistrictSysNo
        {
            get { return districtSysNo; }
            set { SetValue("DistrictSysNo", ref districtSysNo, value); }
        }

        public string districtName;
        //[Validate(ValidateType.Required)]
        public string DistrictName
        {
            get { return districtName; }
            set { SetValue("DistrictName", ref districtName, value); }
        }
        //显示的地区
        public string areaName;
        //[Validate(ValidateType.Required)]
        public string AreaName
        {
            get { return areaName; }
            set { SetValue("AreaName", ref areaName, value); }
        }
        //显示的全名
        public string fullName;
        public string FullName
        {
            get { return fullName; }
            set { SetValue("FullName", ref fullName, value); }
        }
        //是否本地 
        public AreaIsLocal? m_IsLocal;
        [Validate(ValidateType.Required)]
        public AreaIsLocal? IsLocal
        {
            get { return m_IsLocal; }
            set { SetValue("IsLocal",ref m_IsLocal,value);}
        }
        //状态
        public AreaStatus? status;
        [Validate(ValidateType.Required)]
        public AreaStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }
        //显示优先级
        public string m_OrderNumber;
        public string OrderNumber
        {
            get { return m_OrderNumber; }
            set { SetValue("OrderNumber", ref m_OrderNumber, value); }
        }
        //最大重量
        public string weightLimit;
        [Validate(ValidateType.Interger)]
        public string WeightLimit
        {
            get { return weightLimit; }
            set { SetValue("WeightLimit", ref weightLimit, value); }
        }
        //免运费的最小订单金额
        public string soamountLimit;
        [Validate(ValidateType.Regex, @"^(-?\d+)(\.\d+)?")]
        public string SOAmountLimit
        {
            get { return soamountLimit; }
            set { SetValue("SOAmountLimit", ref soamountLimit, value); }
        }
        //城市等级
        public string rank;
        public string Rank
        {
            get { return rank; }
            set { SetValue("Rank", ref rank, value); }
        }

    }
}
