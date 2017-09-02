using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess.Database.Config;
using System.IO;
using ECommerce.Utility.DataAccess.Database.DbProvider;
using System.Security.Cryptography;
using System.Data.Common;
using System.Runtime.Caching;

namespace ECommerce.Utility.DataAccess
{
    public static class DataCommandManager
    {
        private static object s_SyncObj_For_ConnStr = new object();

        static DataCommandManager()
        {
            ConnectionStringManager.SetConnectionString(GetConnStrSetting);
        }

        #region Encrypt & Decrypt

        private const string m_LongDefaultKey = "Nesc.Oversea";
        private const string m_LongDefaultIV = "Oversea3";

        private static string Encrypt(string encryptionText)
        {
            string result = string.Empty;

            if (encryptionText.Length > 0)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(encryptionText);
                byte[] inArray = Encrypt(bytes);
                if (inArray.Length > 0)
                {
                    result = Convert.ToBase64String(inArray);
                }


            }
            return result;

        }

        private static string Decrypt(string encryptionText)
        {
            string result = string.Empty;

            if (encryptionText.Length > 0)
            {
                byte[] bytes = Convert.FromBase64String(encryptionText);
                byte[] inArray = Decrypt(bytes);
                if (inArray.Length > 0)
                {
                    result = Encoding.Unicode.GetString(inArray);
                }

            }
            return result;

        }

        private static byte[] Encrypt(byte[] bytesData)
        {
            byte[] result = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                ICryptoTransform cryptoServiceProvider = CreateAlgorithm().CreateEncryptor();
                using (CryptoStream stream2 = new CryptoStream(stream, cryptoServiceProvider, CryptoStreamMode.Write))
                {
                    stream2.Write(bytesData, 0, bytesData.Length);
                    stream2.FlushFinalBlock();
                    stream2.Close();
                    result = stream.ToArray();
                }
                stream.Close();
            }
            return result;
        }

        private static byte[] Decrypt(byte[] bytesData)
        {
            byte[] result = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                ICryptoTransform cryptoServiceProvider = CreateAlgorithm().CreateDecryptor();
                using (CryptoStream stream2 = new CryptoStream(stream, cryptoServiceProvider, CryptoStreamMode.Write))
                {
                    stream2.Write(bytesData, 0, bytesData.Length);
                    stream2.FlushFinalBlock();
                    stream2.Close();
                    result = stream.ToArray();
                }
                stream.Close();
            }
            return result;
        }

        private static Rijndael CreateAlgorithm()
        {
            Rijndael rijndael = new RijndaelManaged();
            rijndael.Mode = CipherMode.CBC;
            byte[] key = Encoding.Unicode.GetBytes(m_LongDefaultKey);
            byte[] iv = Encoding.Unicode.GetBytes(m_LongDefaultIV);
            rijndael.Key = key;
            rijndael.IV = iv;
            return rijndael;
        }

        #endregion

        private static List<DatabaseInstance> GetDatabaseList()
        {
            if (ConfigHelper.DatabaseListFilePath == null || ConfigHelper.DatabaseListFilePath.Trim().Length <= 0
                || File.Exists(ConfigHelper.DatabaseListFilePath.Trim()) == false)
            {
                return null;
            }
            return CacheManager.GetWithLocalCache<List<DatabaseInstance>>("DA_DataCommandManager.GetDatabaseList",
                ()=> {
                    var list = ConfigHelper.LoadDatabaseListFile();
                    if (list != null && list.DatabaseInstances != null && list.DatabaseInstances.Length > 0)
                    {
                        List<DatabaseInstance> rst = new List<DatabaseInstance>(list.DatabaseInstances.Length);
                        foreach (var db in list.DatabaseInstances)
                        {
                            if (db != null && string.IsNullOrWhiteSpace(db.Name) == false
                                && string.IsNullOrWhiteSpace(db.ConnectionString) == false)
                            {
                                if (rst.Exists(x=>x.Name == db.Name.Trim()))
                                {
                                    throw new ApplicationException("Duplidated database name '" + db.Name + "' in configuration file '" + ConfigHelper.DatabaseListFilePath + "'.");
                                }
                                rst.Add(new DatabaseInstance()
                                {
                                    Name = db.Name.Trim(),
                                    ConnectionString = Decrypt(db.ConnectionString),
                                    Type = db.Type
                                });
                            }
                        }
                        return rst;
                    }
                    return null;
                }, ConfigHelper.DatabaseListFilePath);
        }

        private static ConnectionStringManager.ConnStrSetting GetConnStrSetting(string name)
        {
            List<DatabaseInstance> dbList = GetDatabaseList();
            if (dbList == null || dbList.Count <= 0)
            {
                return null;
            }
            DatabaseInstance db = dbList.Find(x => x.Name == name);
            if (db != null)
            {
                return new ConnectionStringManager.ConnStrSetting(db.Name, db.ConnectionString, db.Type);
            }
            return null;
        }

        private static Dictionary<string, DataCommandConfig> GetAllDataCommandConfigInfosFromCache()
        {
            const string cacheKey = "DA_GetAllDataCommandConfigInfosFromCache";
            Dictionary<string, DataCommandConfig> rst = MemoryCache.Default.Get(cacheKey) as Dictionary<string, DataCommandConfig>;
            if (rst != null)
            {
                return rst;
            }
            lock (cacheKey)
            {
                rst = MemoryCache.Default.Get(cacheKey) as Dictionary<string, DataCommandConfig>;
                if (rst != null)
                {
                    return rst;
                }

                List<string> configFileList;
                rst = GetAllDataCommandConfigInfos(out configFileList);
                string p = ConfigHelper.SqlConfigListFilePath;
                if (!string.IsNullOrWhiteSpace(p) && File.Exists(p.Trim()))
                {
                    configFileList.Add(p);
                }
                else
                {
                    return rst; // 说明没有配置任何的sql config，那么就不能缓存
                }
                CacheItemPolicy cp = new CacheItemPolicy();
                cp.ChangeMonitors.Add(new HostFileChangeMonitor(configFileList));
                MemoryCache.Default.Set(cacheKey, rst, cp);
                return rst;
            }
        }

        private static Dictionary<string, DataCommandConfig> GetAllDataCommandConfigInfos(out List<string> configFileList)
        {
            DataCommandFileList fileList = ConfigHelper.LoadSqlConfigListFile();
            if (fileList == null || fileList.FileList == null || fileList.FileList.Length <= 0)
            {
                configFileList = new List<string>(1);
                return new Dictionary<string, DataCommandConfig>(0);
            }
            configFileList = new List<string>(fileList.FileList.Length + 1);
            Dictionary<string, DataCommandConfig> cache = new Dictionary<string, DataCommandConfig>();
            foreach (var file in fileList.FileList)
            {
                string path = file.FileName;
                string root = Path.GetPathRoot(path);
                if (root == null || root.Trim().Length <= 0)
                {
                    path = Path.Combine(ConfigHelper.ConfigFolder, path);
                }
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    configFileList.Add(path);
                }
                DataOperations op = ConfigHelper.LoadDataCommandList(path);
                if (op != null && op.DataCommand != null && op.DataCommand.Length > 0)
                {
                    foreach (var da in op.DataCommand)
                    {
                        if (cache.ContainsKey(da.Name))
                        {
                            throw new ApplicationException("Duplicate name '" + da.Name + "' for data command in file '" + path + "'.");
                        }
                        cache.Add(da.Name, da);
                    }
                }
            }
            return cache;
        }

        public static DataCommand GetDataCommand(string sqlNameInConfig)
        {
            Dictionary<string, DataCommandConfig> commandConfig = GetAllDataCommandConfigInfosFromCache();
            if (!commandConfig.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            return new DataCommand(commandConfig[sqlNameInConfig]);
        }

        public static CustomDataCommand CreateCustomDataCommandFromConfig(string sqlNameInConfig)
        {
            Dictionary<string, DataCommandConfig> commandConfig = GetAllDataCommandConfigInfosFromCache();
            if (!commandConfig.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            return new CustomDataCommand(commandConfig[sqlNameInConfig]);
        }

        internal static CustomDataCommand CreateCustomDataCommandFromSql(string sql, string dataBase)
        {
            DataCommandConfig config = new DataCommandConfig();
            config.Database = dataBase;
            config.CommandText = sql;
            return new CustomDataCommand(config);
        }        
    }
}
