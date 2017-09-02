using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public static class PersisteFactory
    {
        private static Dictionary<string, IRealTimePersister> s_persisteProviders = new Dictionary<string, IRealTimePersister>();
        private static object s_SyncObj = new object();

        public static IRealTimePersister GetInstance()
        {
            return GetInstance(null);
        }

        public static IRealTimePersister GetInstance(string name)
        {
            RealTimeSetting setting = RealTimeSection.GetSetting();
            if (name == null || name.Trim().Length <= 0)
            {
                name = setting.DefaultPersisteName;
            }
            if (name == null)
            {
                throw new ConfigurationErrorsException("The default persister name is not configured in config file.");
            }
            if (s_persisteProviders.ContainsKey(name))
            {
                return s_persisteProviders[name];
            }
            lock (s_SyncObj)
            {
                if (s_persisteProviders.ContainsKey(name))
                {
                    return s_persisteProviders[name];
                }
                if (!setting.ContainsKey(name))
                {
                    throw new ConfigurationErrorsException("The persister named '" + name + "' is not be found in config file.");
                }
                PersisteItemConfig item = setting[name];
                if (item.Type == null || item.Type.Trim().Length <= 0)
                {
                    throw new ConfigurationErrorsException("The type of persister '" + name + "' cannot be empty.");
                }
                Type p = Type.GetType(item.Type, true);
                if (!typeof(IRealTimePersister).IsAssignableFrom(p))
                {
                    throw new ConfigurationErrorsException("The type '" + p.AssemblyQualifiedName + "' of persister '" + name + "' dosen't implement the interface '" + typeof(IRealTimePersister).AssemblyQualifiedName + "'.");
                }
                IRealTimePersister rst = (IRealTimePersister)Activator.CreateInstance(p);                
                s_persisteProviders.Add(name, rst);
                return rst;
            }
        }
    }    
}
