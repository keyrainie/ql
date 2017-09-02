using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public interface IValidate
    {
        bool Validate(OrderInfo order, out string errorMsg);
    }
}
