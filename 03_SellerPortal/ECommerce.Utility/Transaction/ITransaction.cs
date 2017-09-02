using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Utility
{
    public interface ITransaction : IDisposable
    {
        void Complete();
    }
}
