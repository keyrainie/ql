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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Common.Models
{
    public class AreaQueryFilterVM:ModelBase
    {
        public AreaQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
        }
        public PagingInfo PagingInfo { set; get; }
        //系统编号

        public int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }
        //省份信息
        public int? provinceSysNumber;
        public int? ProvinceSysNumber
        {
            get { return provinceSysNumber; }
            set { SetValue("ProvinceSysNumber", ref provinceSysNumber, value); }
        }

        public string provinceName;
        public string ProvinceName
        {
            get { return provinceName; }
            set { SetValue("ProvinceName", ref provinceName, value); }
        }
        //市级信息
        public string citySysNumber;
        public string CitySysNumber
        {
            get { return citySysNumber; }
            set { SetValue("CitySysNumber", ref citySysNumber, value); }
        }

        public string cityName;
        public string CityName
        {
            get { return cityName; }
            set { SetValue("CityName",ref cityName,value);}
        }
        //区域信息
        public string districtSysNumber;
        public string DistrictSysNumber
        {
            get { return districtSysNumber; }
            set { SetValue("DistrictSysNumber", ref districtSysNumber, value); }
        }

        public string districtName;
        public string DistrictName
        {
            get { return districtName; }
            set { SetValue("DistrictName", ref districtName, value); }
        }
        //状态
        public AreaStatus? status;
        public AreaStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }
        //最大重量
        public string weightLimit;
        [Validate(ValidateType.Interger)]
        public string WeightLimit
        {
            get { return weightLimit; }
            set { SetValue("WeightLimit", ref weightLimit, value); }
        }
        //最大金额
        public string soamtLimit;
        [Validate(ValidateType.Regex, @"^(-?\d+)(\.\d+)?$")]
        public string SOAmtLimit
        {
            get { return soamtLimit; }
            set { SetValue("SOAmtLimit", ref soamtLimit, value); }
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
