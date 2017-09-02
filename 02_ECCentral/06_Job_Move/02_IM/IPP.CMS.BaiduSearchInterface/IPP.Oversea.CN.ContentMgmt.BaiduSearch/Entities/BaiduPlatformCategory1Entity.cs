using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.Entities
{
    public class BaiduPlatformCategory1Entity : BaiduPlatformCategoryEntityBase
    {
        public override string Loc
        {
            get;
            set;
        }

        public BaiduPlatformCategory1Entity(int category1SysNo, string category1Name, string address, int number, decimal price)
            : base(category1SysNo, category1Name, number, price)
        {
            this.Loc = address;
        }
    }

    public class BaiduPlatformCategory2Entity : BaiduPlatformCategoryEntityBase
    {
        public override string Loc
        {
            get
            {
                return string.Format(AppConfig.Category2Address, this.CategorySysNo.ToString());
            }
            set
            {
            }
        }

        public BaiduPlatformCategory2Entity(int category2SysNo, string category2Name, int number, decimal price)
            : base(category2SysNo, category2Name, number, price)
        {
        }
    }

    public class BaiduPlatformCategory3Entity : BaiduPlatformCategoryEntityBase
    {
        public override string Loc
        {
            get
            {
                return string.Format(AppConfig.Category3Address, this.CategorySysNo.ToString());
            }
            set
            {
            }
        }

        public BaiduPlatformCategory3Entity(int category3SysNo, string category3Name, int number, decimal price)
            : base(category3SysNo, category3Name, number, price)
        {
        }
    }
}
