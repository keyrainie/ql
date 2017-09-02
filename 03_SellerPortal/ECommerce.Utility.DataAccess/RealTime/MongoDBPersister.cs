using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;
using System.Configuration;
using Nesoft.Utility.DataAccess;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class MongoDBPersister : IRealTimePersister
    {
        #region IRealTimePersister Members

        public void Persiste<T>(RealTimeData<T> data) where T : class
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
