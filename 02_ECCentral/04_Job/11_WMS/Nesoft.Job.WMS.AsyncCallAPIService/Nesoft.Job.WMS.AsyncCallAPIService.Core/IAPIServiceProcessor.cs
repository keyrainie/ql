using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public interface IAPIServiceProcessor
    {
        string Process(string requestData);
    }
}
