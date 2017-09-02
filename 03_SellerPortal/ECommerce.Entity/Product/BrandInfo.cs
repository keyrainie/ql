using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class BrandInfo : EntityBase
    {
        public int SysNo { get; set; }
        public string BrandName_Ch { get; set; }
        public string BrandName_En { get; set; }
        public string Description { get; set; }
        public string StatusChar { get; set; }
        public ValidStatus Status 
        {
            get
            {
                if (this.StatusChar == "A")
                    return ValidStatus.Active;
                else
                    return ValidStatus.DeActive;
            }
        }
        public string BrandCode { get; set; }

        public string BrandName
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(BrandName_Ch);
                if (!string.IsNullOrEmpty(BrandName_En))
                {
                    builder.Append("(" + BrandName_En + ")");

                }
                return builder.ToString();
            }
        }

        public int C3SysNo { get; set; }
    }
}
