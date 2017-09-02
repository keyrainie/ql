using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public static  class NullableDecimalExtensions
    {
        public static string ToString(this decimal? d, string format)
        {
            if (d.HasValue)
            {
                return d.Value.ToString(format);
            }
            return string.Empty;
        }
    }
}
