using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;
using System.Configuration;
using Nesoft.Utility.DataAccess.Database;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class SqlPersister : IRealTimePersister
    {
        #region IRealTimePersister Members

        public void Persiste<T>(RealTimeData<T> data) where T:class
        {            
            using (TransactionScope scope = new TransactionScope())
            {               
                InsertRealTimeData(data);

                //string name = typeof(T).Name;
                //var extensionConfig = ConfigHelper.GetExtensionConfig(name);
                ////持久化扩展表数据                
                //if (extensionConfig != null)
                //{
                //    var type = Type.GetType(extensionConfig.ExtensionType);
                //    IRealTimeExtensionPersister extension = Invoker.CreateInstance(type) as IRealTimeExtensionPersister;
                //    if (extension != null)
                //    {
                //        extension.Persiste<T>(data.Body);
                //    }
                //}                        

                scope.Complete();
            }
        }

        private void InsertRealTimeData<T>(RealTimeData<T> data) where T : class
        {
            if(IsDataExist(data.BusinessKey,data.BusinessDataType))
            {
                MoveDataToHistory(data.BusinessKey,data.BusinessDataType);
            }
            
            DataCommand command = DataCommandManager.GetDataCommand("RealTime_PersisteData");            
            command.SetParameterValue("BusinessKey", data.BusinessKey);
            command.SetParameterValue("BusinessDataType", data.BusinessDataType);
            command.SetParameterValue("ChangeType", data.ChangeType);
            command.SetParameterValue("CustomerSysNo", data.CustomerSysNo);            
            command.SetParameterValue("@BusinessData", SerializationUtility.XmlSerialize(data.Body));
            command.ExecuteNonQuery();
        }

        private void MoveDataToHistory(int businessKey, BusinessDataType dataType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("RealTime_MoveDataToHistory");
            command.SetParameterValue("@BusinessKey", businessKey);
            command.SetParameterValue("@BusinessDataType", dataType);
            command.ExecuteNonQuery();
        }

        private bool IsDataExist(int businessKey, BusinessDataType dataType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("RealTime_IsDataExist");
            command.SetParameterValue("@BusinessKey", businessKey);
            command.SetParameterValue("@BusinessDataType", dataType);
            int result = command.ExecuteScalar<int>();
            return result > 0;
        }
        #endregion
    }
}
