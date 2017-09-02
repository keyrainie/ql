using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AutoSendMail
{
    internal sealed class QueryResult : IDisposable
    {
        public DataTable ResultTable { get; set; }

        public Dictionary<string, object> OutputParams { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            if (ResultTable != null)
            {
                this.ResultTable.Dispose();
            }
        }

        #endregion
    }
}
