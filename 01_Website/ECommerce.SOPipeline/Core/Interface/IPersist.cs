using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    public interface IPersist
    {
        void Persist(OrderInfo order);
    }
}
