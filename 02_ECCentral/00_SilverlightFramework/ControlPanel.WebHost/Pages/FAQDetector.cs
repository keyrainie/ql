using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public static class FAQDetector
    {
        private static FAQSection s_faqSection;
        static FAQDetector() 
        {
            s_faqSection = ConfigurationManager.GetSection("faqSetting") as FAQSection;
        }

        public static FAQServiceModelCollection GetServiceSetting()
        {
            if (s_faqSection != null
                && s_faqSection.FAQServices != null)
            {
                return s_faqSection.FAQServices;
            }
            return null;
        }

        public static FAQDBModelCollection GetDBSetting()
        {
            if (s_faqSection != null
                && s_faqSection.FAQDBs != null)
            {
                return s_faqSection.FAQDBs;
            }
            return null;
        }

        public static FAQServiceResult DetectServiceStatus(List<string> urls)
        {
            if (urls == null || urls.Count == 0)
            {
                throw new ArgumentException("urls");
            }
            FAQServiceResult result = new FAQServiceResult();
            result.Detail = new List<FAQServiceModel>();
            Stopwatch watch = new Stopwatch();
            foreach (var url in urls)
            {
                watch.Start();
                var faqModel = new FAQServiceModel();
                try
                {
                    WebRequest request = HttpWebRequest.Create(url);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        watch.Stop();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {                            
                            faqModel.IsSuccessful = true;
                        }
                        else
                        {
                            faqModel.IsSuccessful = false;
                        }
                        faqModel.StatusCode = response.StatusCode.ToString();
                        faqModel.StatusDescrption = response.StatusDescription;
                    }
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    faqModel.IsSuccessful = false;
                    faqModel.ExceptionMessage = ex.Message;
                }
                finally
                {
                    faqModel.URL = url;
                    faqModel.ResponseTime = watch.ElapsedMilliseconds;
                    result.Detail.Add(faqModel);
                    if (watch != null)
                    {
                        watch.Reset();
                    }
                }
            }
            result.Flag = (result.Detail.Where(p => p.IsSuccessful == true).Count() == urls.Count) ? true : false;
            return result;
        }

        public static FAQDBResult DetectDBStatus(List<FAQDBModel> dbModels)
        {
            if (dbModels == null || dbModels.Count == 0)
            {
                throw new ArgumentException("dbModels");
            }
            FAQDBResult result = new FAQDBResult();
            result.Detail = new List<FAQDBModel>();
            Stopwatch watch = new Stopwatch();
            foreach (var db in dbModels)
            {
                watch.Start();
                var dbModel = new FAQDBModel();
                try
                {
                    using (SqlConnection conn = new SqlConnection(db.ConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(db.SqlScript, conn))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                dbModel.IsSuccessful = true;
                            }
                            else
                            {
                                dbModel.IsSuccessful = false;
                            }
                            if (reader != null)
                            {
                                reader.Close();
                            }
                            watch.Stop();
                        }
                    }
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    dbModel.IsSuccessful = false;
                    dbModel.ExceptionMessage = ex.Message;
                }
                finally
                {
                    dbModel.DBInstanceName = db.DBInstanceName;
                    dbModel.DBName = db.DBName;
                    dbModel.SqlScript = db.SqlScript;
                    dbModel.ResponseTime = watch.ElapsedMilliseconds;
                    result.Detail.Add(dbModel);
                    if (watch != null)
                    {
                        watch.Reset();
                    }
                }
            }
            result.Flag = (result.Detail.Where(p => p.IsSuccessful == true).Count() == dbModels.Count) ? true : false;
            return result;
        }
    }

    [ConfigurationCollection(typeof(FAQServiceModel), AddItemName = "faqService", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class FAQServiceModelCollection : ConfigurationElementCollection
    {
        public FAQServiceModel this[int index]
        {
            get
            {
                return base.BaseGet(index) as FAQServiceModel;
            }
        }

        public new FAQServiceModel this[string name]
        {
            get
            {
                return base.BaseGet(name) as FAQServiceModel;
            }
        }

        public void Add(FAQServiceModel faqService)
        {
            base.BaseAdd(faqService);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(FAQServiceModel faqService)
        {
            base.BaseRemove(GetElementKey(faqService));
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FAQServiceModel();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as FAQServiceModel).URL;
        }
    }

    public class FAQServiceModel : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true, IsKey = true)]
        public string URL 
        {
            get
            {
                return base["url"] as string;
            }
            set 
            {
                base["url"] = value;
            } 
        }

        public bool IsSuccessful { get; set; }
        public long ResponseTime { get; set; }
        public string StatusDescrption { get; set; }
        public string StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class FAQServiceResult 
    {
        public List<FAQServiceModel> Detail { get; set; }
        public bool Flag { get; set; }
    }

    [ConfigurationCollection(typeof(FAQDBModel), AddItemName = "faqDB", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class FAQDBModelCollection : ConfigurationElementCollection
    {
        public FAQDBModel this[int index]
        {
            get
            {
                return base.BaseGet(index) as FAQDBModel;
            }
        }

        public new FAQDBModel this[string name]
        {
            get
            {
                return base.BaseGet(name) as FAQDBModel;
            }
        }

        public void Add(FAQDBModel faqDB)
        {
            base.BaseAdd(faqDB);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(FAQDBModel faqDB)
        {
            base.BaseRemove(GetElementKey(faqDB));
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FAQDBModel();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as FAQDBModel).ConnectionString;
        }
    }

    public class FAQDBModel : ConfigurationElement
    {
        [ConfigurationProperty("connectionString", IsRequired = true, IsKey = true)]
        public string ConnectionString 
        {
            get 
            {
                return base["connectionString"] as string;
            }
            set 
            {
                base["connectionString"] = value;
            } 
        }

        [ConfigurationProperty("dbInstanceName")]
        public string DBInstanceName 
        {
            get 
            {
                return base["dbInstanceName"] as string;
            }
            set 
            {
                base["dbInstanceName"] = value;
            }
        }

        [ConfigurationProperty("dbName")]
        public string DBName 
        {
            get
            {
                return base["dbName"] as string;
            }
            set
            {
                base["dbName"] = value;
            }
        }
        public bool IsSuccessful { get; set; }
        public long ResponseTime { get; set; }

        [ConfigurationProperty("execSql")]
        public string SqlScript 
        {
            get
            {
                return base["execSql"] as string;
            }
            set
            {
                base["execSql"] = value;
            }
        }

        public string ExceptionMessage { get; set; }

        [ConfigurationProperty("overrideDBInstanceName", DefaultValue="false")]
        public bool OverrideDBInstanceName 
        {
            get 
            {
                return (bool)(base["overrideDBInstanceName"]);
            }
            set 
            {
                base["overrideDBInstanceName"] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }

    public class FAQDBResult
    {
        public List<FAQDBModel> Detail { get; set; }
        public bool Flag { get; set; }
    }

    public class FAQSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty s_faqService;
        private static readonly ConfigurationProperty s_faqDB;
        private static ConfigurationPropertyCollection properties;
        static FAQSection()
        {
            s_faqService = new ConfigurationProperty("faqServices", typeof(FAQServiceModelCollection));
            s_faqDB = new ConfigurationProperty("faqDBs", typeof(FAQDBModelCollection));
            properties = new ConfigurationPropertyCollection();
            properties.Add(s_faqService);
            properties.Add(s_faqDB);
        }

        [ConfigurationProperty("faqServices")]
        public FAQServiceModelCollection FAQServices
        {
            get
            {
                return base[s_faqService] as FAQServiceModelCollection;
            }
        }

        [ConfigurationProperty("faqDBs")]
        public FAQDBModelCollection FAQDBs
        {
            get
            {
                return base[s_faqDB] as FAQDBModelCollection;
            }
        }
    }
}