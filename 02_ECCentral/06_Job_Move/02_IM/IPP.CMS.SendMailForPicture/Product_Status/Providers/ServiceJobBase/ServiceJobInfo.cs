using System;
using System.Collections.Generic;
using System.Configuration;

namespace IPP.ContentMgmt.Product_Status.Providers
{
    public class ServiceJobInfo : ConfigurationElement
    {
        [ConfigurationProperty("jobName", IsRequired = true)]
        public string JobName
        {
            get { return (String)this["jobName"]; }
        }

        [ConfigurationProperty("jobType", IsRequired = true)]
        public ServiceJobType JobType
        {
            get { return (ServiceJobType)this["jobType"]; }
        }

        [ConfigurationProperty("runTime", IsRequired = true)]
        public string RunTime
        {
            get { return (string)this["runTime"]; }
        }

        [ConfigurationProperty("provider", IsRequired = true)]
        public string Provider
        {
            get { return (string)this["provider"]; }
        }

        [ConfigurationProperty("errorLog", IsRequired = true)]
        public string ErrorLog
        {
            get { return (string)this["errorLog"]; }
        }

        [ConfigurationProperty("infoLog", IsRequired = true)]
        public string InfoLog
        {
            get { return (string)this["infoLog"]; }
        }

        [ConfigurationProperty("bizLog", IsRequired = true)]
        public string BizLog
        {
            get { return (string)this["bizLog"]; }
        }

        //[ConfigurationProperty("runOnceTimeHour")]
        //public int RunOnceTimeHour
        //{
        //    get { return (int)this["runOnceTimeHour"]; }
        //}

        [ConfigurationProperty("actionName")]
        public string ActionName
        {
            get { return (string)this["actionName"]; }
        }

        [ConfigurationProperty("retryInterval")]
        public int RetryInterval
        {
            get { return (int)this["retryInterval"]; }
        }

        private static object locker = new object();

        private static List<string> runningJobNameList = new List<string>();

        public bool SetRunning()
        {
            lock (locker)
            {
                if (runningJobNameList.Contains(JobName))
                {
                    return false;
                }
                else
                {
                    runningJobNameList.Add(JobName);
                    return true;
                }
            }
        }

        public void ReleaseRunning()
        {
            lock (locker)
            {
                runningJobNameList.Remove(JobName);
            }
        }
    }
}
