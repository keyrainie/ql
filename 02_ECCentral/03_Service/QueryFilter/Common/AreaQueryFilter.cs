using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Common
{
    public class AreaQueryFilter
    {
        public int? ProvinceSysNumber
        {
            get;
            set;
        }

        public int? CitySysNumber
        {
            get;
            set;
        }

        public int? DistrictSysNumber
        {
            get;
            set;
        }

        public int? Status
        {
            get;
            set;
        }

        public string ProvinceName
        {
            get;
            set;
        }

        public string CityName
        {
            get;
            set;
        }

        public string DistrictName
        {
            get;
            set;
        }

        public int? SysNo
        {
            get;
            set;
        }

        public string Rank
        {
            get;
            set;
        }

        public int? WeightLimit
        {
            get;
            set;
        }

        public decimal? SOAmtLimit
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public PagingInfo PagingInfo
        {
            get;
            set;
        }
    }
}